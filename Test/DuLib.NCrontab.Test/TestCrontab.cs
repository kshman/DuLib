using Du.NCrontab;

namespace Test.Du.NCrontab;

[TestClass]
public class TestCrontab
{
	[TestMethod]
	public async Task 시작과정지_시작하기()
	{
		var count = 0;

		var run = new Crontab();
		run.Enter += (_, _) => { count++; };

		using (var cts = new CancellationTokenSource(1000))
			await run.StartAsync(cts.Token);

		Assert.IsFalse(run.IsRunning);
		Assert.AreEqual(count, 0);
	}

	[TestMethod]
	public async Task 시작과정지_끝내기()
	{
		var run = new Crontab();

		var task = run.StartAsync();
		await Task.Delay(1000);

		run.Stop();
		await task;

		Assert.IsFalse(run.IsRunning);
	}

	[TestMethod]
	public async Task 시작과정지_디스포즈()
	{
		var run = new Crontab();

		using (var cts = new CancellationTokenSource(1000))
			await run.StartAsync(cts.Token);

		run.Dispose();

		Assert.IsFalse(run.IsRunning);
	}

	[TestMethod]
	public void 시작과정지_태스크_없음()
	{
		var count = 0;

		var run = new Crontab();
		run.Enter += (_, _) => { count++; };

		using (var cts = new CancellationTokenSource(1000))
			run.Start(cts.Token);

		Assert.IsFalse(run.IsRunning);
		Assert.AreEqual(0, count);
	}

	[TestMethod]
	public void 태스크_한개_동기()
	{
		var sch = CrontabSchedule.Parse("* * * * * *", true);
		var item = new TaskObject();

		var run = new Crontab();
		run.AddTask(sch, (_) => item.Run());

		using (var cts = new CancellationTokenSource(1100))
			run.Start(cts.Token);

		Assert.IsFalse(run.IsRunning);
		Assert.AreEqual(1, item.Count);
	}

	[TestMethod]
	public void 태스크_한개_비동기()
	{
		var sch = CrontabSchedule.Parse("* * * * * *", true);
		var item = new TaskObject();

		var run = new Crontab();
		run.AddTask(sch, async (_) => await item.RunAsync());

		using (var cts = new CancellationTokenSource(1100))
			run.Start(cts.Token);

		Assert.IsFalse(run.IsRunning);
		Assert.AreEqual(1, item.Count);
	}

	[TestMethod]
	public void 태스크_여러개()
	{
		var run = new Crontab();

		var items = new TaskObject[3];
		for (var i = 0; i < items.Length; i++)
		{
			items[i] = new TaskObject();
			items[i].AddTaskPerSecond(run);
		}

		using (var cts = new CancellationTokenSource(1100))
			run.Start(cts.Token);

		foreach (var t in items)
			Assert.AreEqual(1, t.Count);
	}

	[TestMethod]
	public void 태스크_아이디로_삭제()
	{
		var run = new Crontab();

		var id1 = run.AddTask("* * * * *", (_) => { });
		var id2 = run.AddTask("* * * * *", (_) => { });
		var id3 = ulong.MaxValue - 1;

		var res = run.RemoveTask(id1, id2, id3);

		Assert.AreEqual(2, res);
	}

	[TestMethod]
	public void 태스크_태스크로_삭제()
	{
		var run = new Crontab();

		var task1 = new CrontabTask("* * * * *", (_) => { });
		run.AddTask(task1);
		var task2 = new CrontabTask("* * * * *", (_) => { });
		run.AddTask(task2);
		var task3 = new CrontabTask("* * * * *", (_) => { });

		var res = run.RemoveTask(task1, task2, task3);

		Assert.AreEqual(2, res);
	}

	[TestMethod]
	public async Task 태스크_실행중_다지우기()
	{
		var run = new Crontab();
		var events = new List<CrontabEnterEventArg>();
		run.Enter += (_, e) => events.Add(e);

		using (var cts = new CancellationTokenSource(1500))
		{
			// ReSharper disable once AccessToDisposedClosure
			var task = Task.Run(async () => await run.StartAsync(cts.Token), cts.Token);

			var ids = Enumerable.Range(0, 10);
			Parallel.ForEach(ids, _ => run.AddTask("* * * * *", _ => { }));

			run.RemoveTaskAll();

			await task;
		}

		Assert.AreEqual(0, run.TaskCount);
		Assert.AreEqual(0, events.Count, "실행 시간에 따라 실패할 경우가 있어요");
	}

	[TestMethod]
	public void 태스크_태스트_얻기()
	{
		var run = new Crontab();

		var id1 = run.AddTask("* * * * *", (_) => { });
		var id2 = run.AddTask("* * * * *", (_) => { });
		var id3 = run.AddTask("* * * * *", (_) => { });

		var task1 = run.FindTask(id1);
		var task2 = run.FindTask(id2);
		var task3 = run.FindTask(id3);

		Assert.AreEqual(id1, task1?.Id);
		Assert.AreEqual(id2, task2?.Id);
		Assert.AreEqual(id3, task3?.Id);
	}

	[TestMethod]
	public void 다음날짜_시작_날짜로_얻기()
	{
		var run = new Crontab();

		run.AddTask("*/1 * * * *", (_) => { });
		run.AddTask("*/2 * * * *", (_) => { });
		run.AddTask("*/3 * * * *", (_) => { });

		var begin = new DateTime(2000, 1, 1, 0, 0, 0);
		var res = run.GetNextOccurrence(begin).ToArray();

		Assert.AreEqual(3, res.Length);
	}

	[TestMethod]
	public void 다음날짜_시작과_끝_날짜로_얻기()
	{
		var run = new Crontab();

		run.AddTask("*/1 * * * *", (_) => { });
		run.AddTask("*/2 * * * *", (_) => { });
		run.AddTask("*/3 * * * *", (_) => { });

		var begin = new DateTime(2000, 1, 1, 0, 0, 0);
		var end = begin.AddHours(1);
		var res = run.GetNextOccurrences(begin, end).ToArray();

		Assert.AreEqual(59, res.Length);
		Assert.AreEqual(0, res.Count(r => !r.Tasks.Any()));
		Assert.AreEqual(20, res.Count(r => r.Tasks.Count() == 1));
		Assert.AreEqual(30, res.Count(r => r.Tasks.Count() == 2));
		Assert.AreEqual(9, res.Count(r => r.Tasks.Count() == 3));
		Assert.AreEqual(0, res.Count(r => r.Tasks.Count() > 3));
	}

	[TestMethod]
	public void 루프_예외_안던지기()
	{
		var run = new Crontab { IsThrowException = false };
		run.AddTask("* * * * * *", (_) => TaskObject.ThrowException(), true);

		var e = Assert.ThrowsException<Exception>(() =>
		{
			using var cts = new CancellationTokenSource(1100);
			run.Start(cts.Token);

			throw new Exception("ok");
		});

		Assert.AreEqual("ok", e?.Message);
	}

	[TestMethod]
	public void 루프_예외_던지기()
	{
		var run = new Crontab { IsThrowException = true };
		run.AddTask("* * * * * *", (_) => TaskObject.ThrowException(), true);

		Assert.ThrowsException<Exception>(() =>
		{
			using var cts = new CancellationTokenSource(1100);
			run.Start(cts.Token);
		});
	}

	[TestMethod]
	public void 루프_카운트()
	{
		var run = new Crontab { IsThrowException = true };
		run.AddTask("* * * * * *", (_) => { }, true);

		using (var cts = new CancellationTokenSource(1100))
			run.Start(cts.Token);

		Assert.IsTrue(run.LoopCount > 0);
	}

	[TestMethod]
	public void 루프_비동기_안기다림()
	{
		var run = new Crontab { IsWaitAsyncTask = false };

		var b = false;
		run.AddTask("* * * * * *", async (_) =>
		{
			b = true;
			await Task.Delay(10, _);
		}, true);

		using (var cts = new CancellationTokenSource(1500))
			run.Start(cts.Token);

		Assert.IsTrue(b);
	}

	[TestMethod]
	public void 루프_이벤트핸들러()
	{
		var run = new Crontab { IsWaitAsyncTask = false };

		var count = 0;
		run.Enter += (_, _) => { count++; };
		run.Leave += (_, _) => { count++; };
		run.AddTask("* * * * * *", (_) => { count++; }, true);

		using (var cts = new CancellationTokenSource(1100))
			run.Start(cts.Token);

		Assert.AreEqual(3, count);
	}
}

public class TaskObject
{
	public int Count { get; private set; }

	public TaskObject()
	{
		Count = 0;
	}

	public void Run()
	{
		Count++;
	}

	public Task RunAsync()
	{
		Run();
		return Task.CompletedTask;
	}

	public void AddTaskPerSecond(Crontab run)
	{
		run.AddTask("* * * * * *", (_) => Run(), true);
	}

	public static void ThrowException()
	{
		throw new Exception("Throw");
	}
}

using System.Globalization;
using Du.NCrontab;

namespace Test.Du.NCrontab;

[TestClass]
public class TestCrontabSchedule
{
	private const string TimeFormat = "dd/MM/yyyy HH:mm:ss";

	private static readonly string[] TimeFormats =
	{
		"yyyy-MM-dd",
		"yyyy-MM-dd HH:mm",
		"yyyy-MM-dd HH:mm:ss",
		"dd/MM/yyyy HH:mm:ss",
	};

	[TestMethod]
	public void CannotParseNullString()
	{
		var e = Assert.ThrowsException<ArgumentNullException>(() => CrontabSchedule.Parse(null));
		Assert.AreEqual("expression", e?.ParamName);
	}

	[TestMethod]
	public void CannotParseEmptyString()
	{
		Assert.ThrowsException<CrontabException>(() => CrontabSchedule.Parse(string.Empty));
	}

	[TestMethod]
	public void TryParseNullString() 
		=> Assert.IsNull(CrontabSchedule.TryParse(null));

	[TestMethod]
	public void TryParseEmptyString() 
		=> Assert.IsNull(CrontabSchedule.TryParse(string.Empty));

	[TestMethod]
	public void AllTimeString()
	{
		Assert.AreEqual("* * * * *", CrontabSchedule.Parse("* * * * *").ToString());
	}

	[TestMethod]
	public void SixPartAllTimeString()
	{
		Assert.AreEqual("* * * * * *", CrontabSchedule.Parse("* * * * * *", true).ToString());
	}

	[TestMethod]
	public void CannotParseWhenSecondsRequired()
	{
		Assert.ThrowsException<CrontabException>(() => CrontabSchedule.Parse("* * * * *", true));
	}

	[DataRow("* 1-3 * * *", "* 1-2,3 * * *", false)]
	[DataRow("* * * 1,3,5,7,9,11 *", "* * * */2 *", false)]
	[DataRow("10,25,40 * * * *", "10-40/15 * * * *", false)]
	[DataRow("* * * 1,3,8 1-2,5", "* * * 삼월,일월,팔월 금,월-화", false)]
	[DataRow("1 * 1-3 * * *", "1 * 1-2,3 * * *", true)]
	[DataRow("22 * * * 1,3,5,7,9,11 *", "22 * * * */2 *", true)]
	[DataRow("33 10,25,40 * * * *", "33 10-40/15 * * * *", true)]
	[DataRow("55 * * * 1,3,8 1-2,5", "55 * * * 삼월,일월,팔월 금,월-화", true)]

	[DataTestMethod]
	public void Formatting(string format, string expression, bool includingSeconds)
	{
		Assert.AreEqual(format, CrontabSchedule.Parse(expression, includingSeconds).ToString());
	}

	/// <summary>
	/// Tests to see if the cron class can calculate the previous matching
	/// time correctly in various circumstances.
	/// </summary>
	[DataRow("01/01/2003 00:00:00", "* * * * *", "01/01/2003 00:01:00", false)]
	[DataRow("01/01/2003 00:01:00", "* * * * *", "01/01/2003 00:02:00", false)]
	[DataRow("01/01/2003 00:02:00", "* * * * *", "01/01/2003 00:03:00", false)]
	[DataRow("01/01/2003 00:59:00", "* * * * *", "01/01/2003 01:00:00", false)]
	[DataRow("01/01/2003 01:59:00", "* * * * *", "01/01/2003 02:00:00", false)]
	[DataRow("01/01/2003 23:59:00", "* * * * *", "02/01/2003 00:00:00", false)]
	[DataRow("31/12/2003 23:59:00", "* * * * *", "01/01/2004 00:00:00", false)]

	[DataRow("28/02/2003 23:59:00", "* * * * *", "01/03/2003 00:00:00", false)]
	[DataRow("28/02/2004 23:59:00", "* * * * *", "29/02/2004 00:00:00", false)]

	// Second tests
	[DataRow("01/01/2003 00:00:00", "45 * * * * *", "01/01/2003 00:00:45", true)]

	[DataRow("01/01/2003 00:00:00", "45-47,48,49 * * * * *", "01/01/2003 00:00:45", true)]
	[DataRow("01/01/2003 00:00:45", "45-47,48,49 * * * * *", "01/01/2003 00:00:46", true)]
	[DataRow("01/01/2003 00:00:46", "45-47,48,49 * * * * *", "01/01/2003 00:00:47", true)]
	[DataRow("01/01/2003 00:00:47", "45-47,48,49 * * * * *", "01/01/2003 00:00:48", true)]
	[DataRow("01/01/2003 00:00:48", "45-47,48,49 * * * * *", "01/01/2003 00:00:49", true)]
	[DataRow("01/01/2003 00:00:49", "45-47,48,49 * * * * *", "01/01/2003 00:01:45", true)]

	[DataRow("01/01/2003 00:00:00", "2/5 * * * * *", "01/01/2003 00:00:02", true)]
	[DataRow("01/01/2003 00:00:02", "2/5 * * * * *", "01/01/2003 00:00:07", true)]
	[DataRow("01/01/2003 00:00:50", "2/5 * * * * *", "01/01/2003 00:00:52", true)]
	[DataRow("01/01/2003 00:00:52", "2/5 * * * * *", "01/01/2003 00:00:57", true)]
	[DataRow("01/01/2003 00:00:57", "2/5 * * * * *", "01/01/2003 00:01:02", true)]

	// See: https://github.com/atifaziz/NCrontab/issues/90
	[DataRow("24/02/2021 09:50:35", "* 0-1 10 * * *", "24/02/2021 10:00:00", true)]

	// Minute tests
	[DataRow("01/01/2003 00:00:00", "45 * * * *", "01/01/2003 00:45:00", false)]

	[DataRow("01/01/2003 00:00:00", "45-47,48,49 * * * *", "01/01/2003 00:45:00", false)]
	[DataRow("01/01/2003 00:45:00", "45-47,48,49 * * * *", "01/01/2003 00:46:00", false)]
	[DataRow("01/01/2003 00:46:00", "45-47,48,49 * * * *", "01/01/2003 00:47:00", false)]
	[DataRow("01/01/2003 00:47:00", "45-47,48,49 * * * *", "01/01/2003 00:48:00", false)]
	[DataRow("01/01/2003 00:48:00", "45-47,48,49 * * * *", "01/01/2003 00:49:00", false)]
	[DataRow("01/01/2003 00:49:00", "45-47,48,49 * * * *", "01/01/2003 01:45:00", false)]

	[DataRow("01/01/2003 00:00:00", "2/5 * * * *", "01/01/2003 00:02:00", false)]
	[DataRow("01/01/2003 00:02:00", "2/5 * * * *", "01/01/2003 00:07:00", false)]
	[DataRow("01/01/2003 00:50:00", "2/5 * * * *", "01/01/2003 00:52:00", false)]
	[DataRow("01/01/2003 00:52:00", "2/5 * * * *", "01/01/2003 00:57:00", false)]
	[DataRow("01/01/2003 00:57:00", "2/5 * * * *", "01/01/2003 01:02:00", false)]

	// See: https://github.com/atifaziz/NCrontab/issues/90
	[DataRow("24/02/2021 09:50:35", "* * 10 * * *", "24/02/2021 10:00:00", true)]
	[DataRow("24/02/2021 09:50:35", "* 55 * * * *", "24/02/2021 09:55:00", true)]

	[DataRow("01/01/2003 00:00:30", "3 45 * * * *", "01/01/2003 00:45:03", true)]

	[DataRow("01/01/2003 00:00:30", "6 45-47,48,49 * * * *", "01/01/2003 00:45:06", true)]
	[DataRow("01/01/2003 00:45:30", "6 45-47,48,49 * * * *", "01/01/2003 00:46:06", true)]
	[DataRow("01/01/2003 00:46:30", "6 45-47,48,49 * * * *", "01/01/2003 00:47:06", true)]
	[DataRow("01/01/2003 00:47:30", "6 45-47,48,49 * * * *", "01/01/2003 00:48:06", true)]
	[DataRow("01/01/2003 00:48:30", "6 45-47,48,49 * * * *", "01/01/2003 00:49:06", true)]
	[DataRow("01/01/2003 00:49:30", "6 45-47,48,49 * * * *", "01/01/2003 01:45:06", true)]

	[DataRow("01/01/2003 00:00:30", "9 2/5 * * * *", "01/01/2003 00:02:09", true)]
	[DataRow("01/01/2003 00:02:30", "9 2/5 * * * *", "01/01/2003 00:07:09", true)]
	[DataRow("01/01/2003 00:50:30", "9 2/5 * * * *", "01/01/2003 00:52:09", true)]
	[DataRow("01/01/2003 00:52:30", "9 2/5 * * * *", "01/01/2003 00:57:09", true)]
	[DataRow("01/01/2003 00:57:30", "9 2/5 * * * *", "01/01/2003 01:02:09", true)]

	// Hour tests
	[DataRow("20/12/2003 10:00:00", " * 3/4 * * *", "20/12/2003 11:00:00", false)]
	[DataRow("20/12/2003 00:30:00", " * 3   * * *", "20/12/2003 03:00:00", false)]
	[DataRow("20/12/2003 01:45:00", "30 3   * * *", "20/12/2003 03:30:00", false)]

	// Day of month tests
	[DataRow("07/01/2003 00:00:00", "30  *  1 * *", "01/02/2003 00:30:00", false)]
	[DataRow("01/02/2003 00:30:00", "30  *  1 * *", "01/02/2003 01:30:00", false)]

	[DataRow("01/01/2003 00:00:00", "10  * 22    * *", "22/01/2003 00:10:00", false)]
	[DataRow("01/01/2003 00:00:00", "30 23 19    * *", "19/01/2003 23:30:00", false)]
	[DataRow("01/01/2003 00:00:00", "30 23 21    * *", "21/01/2003 23:30:00", false)]
	[DataRow("01/01/2003 00:01:00", " *  * 21    * *", "21/01/2003 00:00:00", false)]
	[DataRow("10/07/2003 00:00:00", " *  * 30,31 * *", "30/07/2003 00:00:00", false)]

	// Test month rollovers for months with 28,29,30 and 31 days
	[DataRow("28/02/2002 23:59:59", "* * * 3 *", "01/03/2002 00:00:00", false)]
	[DataRow("29/02/2004 23:59:59", "* * * 3 *", "01/03/2004 00:00:00", false)]
	[DataRow("31/03/2002 23:59:59", "* * * 4 *", "01/04/2002 00:00:00", false)]
	[DataRow("30/04/2002 23:59:59", "* * * 5 *", "01/05/2002 00:00:00", false)]

	// Test month 30,31 days
	[DataRow("01/01/2000 00:00:00", "0 0 15,30,31 * *", "15/01/2000 00:00:00", false)]
	[DataRow("15/01/2000 00:00:00", "0 0 15,30,31 * *", "30/01/2000 00:00:00", false)]
	[DataRow("30/01/2000 00:00:00", "0 0 15,30,31 * *", "31/01/2000 00:00:00", false)]
	[DataRow("31/01/2000 00:00:00", "0 0 15,30,31 * *", "15/02/2000 00:00:00", false)]

	[DataRow("15/02/2000 00:00:00", "0 0 15,30,31 * *", "15/03/2000 00:00:00", false)]

	[DataRow("15/03/2000 00:00:00", "0 0 15,30,31 * *", "30/03/2000 00:00:00", false)]
	[DataRow("30/03/2000 00:00:00", "0 0 15,30,31 * *", "31/03/2000 00:00:00", false)]
	[DataRow("31/03/2000 00:00:00", "0 0 15,30,31 * *", "15/04/2000 00:00:00", false)]

	[DataRow("15/04/2000 00:00:00", "0 0 15,30,31 * *", "30/04/2000 00:00:00", false)]
	[DataRow("30/04/2000 00:00:00", "0 0 15,30,31 * *", "15/05/2000 00:00:00", false)]

	[DataRow("15/05/2000 00:00:00", "0 0 15,30,31 * *", "30/05/2000 00:00:00", false)]
	[DataRow("30/05/2000 00:00:00", "0 0 15,30,31 * *", "31/05/2000 00:00:00", false)]
	[DataRow("31/05/2000 00:00:00", "0 0 15,30,31 * *", "15/06/2000 00:00:00", false)]

	[DataRow("15/06/2000 00:00:00", "0 0 15,30,31 * *", "30/06/2000 00:00:00", false)]
	[DataRow("30/06/2000 00:00:00", "0 0 15,30,31 * *", "15/07/2000 00:00:00", false)]

	[DataRow("15/07/2000 00:00:00", "0 0 15,30,31 * *", "30/07/2000 00:00:00", false)]
	[DataRow("30/07/2000 00:00:00", "0 0 15,30,31 * *", "31/07/2000 00:00:00", false)]
	[DataRow("31/07/2000 00:00:00", "0 0 15,30,31 * *", "15/08/2000 00:00:00", false)]

	[DataRow("15/08/2000 00:00:00", "0 0 15,30,31 * *", "30/08/2000 00:00:00", false)]
	[DataRow("30/08/2000 00:00:00", "0 0 15,30,31 * *", "31/08/2000 00:00:00", false)]
	[DataRow("31/08/2000 00:00:00", "0 0 15,30,31 * *", "15/09/2000 00:00:00", false)]

	[DataRow("15/09/2000 00:00:00", "0 0 15,30,31 * *", "30/09/2000 00:00:00", false)]
	[DataRow("30/09/2000 00:00:00", "0 0 15,30,31 * *", "15/10/2000 00:00:00", false)]

	[DataRow("15/10/2000 00:00:00", "0 0 15,30,31 * *", "30/10/2000 00:00:00", false)]
	[DataRow("30/10/2000 00:00:00", "0 0 15,30,31 * *", "31/10/2000 00:00:00", false)]
	[DataRow("31/10/2000 00:00:00", "0 0 15,30,31 * *", "15/11/2000 00:00:00", false)]

	[DataRow("15/11/2000 00:00:00", "0 0 15,30,31 * *", "30/11/2000 00:00:00", false)]
	[DataRow("30/11/2000 00:00:00", "0 0 15,30,31 * *", "15/12/2000 00:00:00", false)]

	[DataRow("15/12/2000 00:00:00", "0 0 15,30,31 * *", "30/12/2000 00:00:00", false)]
	[DataRow("30/12/2000 00:00:00", "0 0 15,30,31 * *", "31/12/2000 00:00:00", false)]
	[DataRow("31/12/2000 00:00:00", "0 0 15,30,31 * *", "15/01/2001 00:00:00", false)]

	// Other month tests (including year rollover)
	[DataRow("01/12/2003 05:00:00", "10 * * 6 *", "01/06/2004 00:10:00", false)]
	[DataRow("04/01/2003 00:00:00", " 1 2 3 * *", "03/02/2003 02:01:00", false)]
	[DataRow("01/07/2002 05:00:00", "10 * * 이월,사월-유월 *", "01/02/2003 00:10:00", false)]
	[DataRow("01/01/2003 00:00:00", "0 12 1 6 *", "01/06/2003 12:00:00", false)]
	[DataRow("11/09/1988 14:23:00", "* 12 1 6 *", "01/06/1989 12:00:00", false)]
	[DataRow("11/03/1988 14:23:00", "* 12 1 6 *", "01/06/1988 12:00:00", false)]
	[DataRow("11/03/1988 14:23:00", "* 2,4-8,15 * 6 *", "01/06/1988 02:00:00", false)]
	[DataRow("11/03/1988 14:23:00", "20 * * 일월,이월,삼월,사월,오월,유월,칠월,팔월,구월-시월,십일월,십이월 *", "11/03/1988 15:20:00", false)]

	// Day of week tests
	[DataRow("26/06/2003 10:00:00", "30 6 * * 0", "29/06/2003 06:30:00", false)]
	[DataRow("26/06/2003 10:00:00", "30 6 * * 일", "29/06/2003 06:30:00", false)]
	[DataRow("26/06/2003 10:00:00", "30 6 * * 일", "29/06/2003 06:30:00", false)]
	[DataRow("19/06/2003 00:00:00", "1 12 * * 2", "24/06/2003 12:01:00", false)]
	[DataRow("24/06/2003 12:01:00", "1 12 * * 2", "01/07/2003 12:01:00", false)]

	[DataRow("01/06/2003 14:55:00", "15 18 * * 월요일", "02/06/2003 18:15:00", false)]
	[DataRow("02/06/2003 18:15:00", "15 18 * * 월요일", "09/06/2003 18:15:00", false)]
	[DataRow("09/06/2003 18:15:00", "15 18 * * 월요일", "16/06/2003 18:15:00", false)]
	[DataRow("16/06/2003 18:15:00", "15 18 * * 월요일", "23/06/2003 18:15:00", false)]
	[DataRow("23/06/2003 18:15:00", "15 18 * * 월요일", "30/06/2003 18:15:00", false)]
	[DataRow("30/06/2003 18:15:00", "15 18 * * 월요일", "07/07/2003 18:15:00", false)]

	[DataRow("01/01/2003 00:00:00", "* * * * 월요일", "06/01/2003 00:00:00", false)]
	[DataRow("01/01/2003 12:00:00", "45 16 1 * 월요일", "01/09/2003 16:45:00", false)]
	[DataRow("01/09/2003 23:45:00", "45 16 1 * 월요일", "01/12/2003 16:45:00", false)]

	// Leap year tests
	[DataRow("01/01/2000 12:00:00", "1 12 29 2 *", "29/02/2000 12:01:00", false)]
	[DataRow("29/02/2000 12:01:00", "1 12 29 2 *", "29/02/2004 12:01:00", false)]
	[DataRow("29/02/2004 12:01:00", "1 12 29 2 *", "29/02/2008 12:01:00", false)]

	// Non-leap year tests
	[DataRow("01/01/2000 12:00:00", "1 12 28 2 *", "28/02/2000 12:01:00", false)]
	[DataRow("28/02/2000 12:01:00", "1 12 28 2 *", "28/02/2001 12:01:00", false)]
	[DataRow("28/02/2001 12:01:00", "1 12 28 2 *", "28/02/2002 12:01:00", false)]
	[DataRow("28/02/2002 12:01:00", "1 12 28 2 *", "28/02/2003 12:01:00", false)]
	[DataRow("28/02/2003 12:01:00", "1 12 28 2 *", "28/02/2004 12:01:00", false)]
	[DataRow("29/02/2004 12:01:00", "1 12 28 2 *", "28/02/2005 12:01:00", false)]

	[DataRow("01/01/2000 12:00:00", "40 14/1 * * *", "01/01/2000 14:40:00", false)]
	[DataRow("01/01/2000 14:40:00", "40 14/1 * * *", "01/01/2000 15:40:00", false)]

	[DataTestMethod]
	public void Evaluations(string startTimeString, string cronExpression, string nextTimeString, bool includingSeconds)
		=> CronCall(startTimeString, cronExpression, nextTimeString, includingSeconds);

	[DataRow(" *  * * * *  ", "01/01/2003 00:00:00", "01/01/2003 00:00:00", false)]
	[DataRow(" *  * * * *  ", "31/12/2002 23:59:59", "01/01/2003 00:00:00", false)]
	[DataRow(" *  * * * 월요일", "31/12/2002 23:59:59", "01/01/2003 00:00:00", false)]
	[DataRow(" *  * * * 월요일", "01/01/2003 00:00:00", "02/01/2003 00:00:00", false)]
	[DataRow(" *  * * * 월요일", "01/01/2003 00:00:00", "02/01/2003 12:00:00", false)]
	[DataRow("30 12 * * 월요일", "01/01/2003 00:00:00", "06/01/2003 12:00:00", false)]

	[DataRow(" *  *  * * * *  ", "01/01/2003 00:00:00", "01/01/2003 00:00:00", true)]
	[DataRow(" *  *  * * * *  ", "31/12/2002 23:59:59", "01/01/2003 00:00:00", true)]
	[DataRow(" *  *  * * * 월요일", "31/12/2002 23:59:59", "01/01/2003 00:00:00", true)]
	[DataRow(" *  *  * * * 월요일", "01/01/2003 00:00:00", "02/01/2003 00:00:00", true)]
	[DataRow(" *  *  * * * 월요일", "01/01/2003 00:00:00", "02/01/2003 12:00:00", true)]
	[DataRow("10 30 12 * * 월요일", "01/01/2003 00:00:00", "06/01/2003 12:00:10", true)]

	[DataTestMethod]
	public void FiniteOccurrences(string cronExpression, string startTimeString, string endTimeString, bool includingSeconds)
		=> CronFinite(cronExpression, startTimeString, endTimeString, includingSeconds);

	//
	// Test to check we don't loop indefinitely looking for a February
	// 31st because no such date would ever exist!
	//

	[TestCategory("Performance")]
	[Timeout(1000)]
	[DataRow("* * 31 이월 *", false)]
	[DataRow("* * * 31 이월 *", true)]
	[DataTestMethod]
	public void DontLoopIndefinitely(string expression, bool includingSeconds)
		=> CronFinite(expression, "01/01/2001 00:00:00", "01/01/2010 00:00:00", includingSeconds);

	private static void BadField(string expression, bool includingSeconds)
	{
		Assert.ThrowsException<CrontabException>(() => CrontabSchedule.Parse(expression, includingSeconds));
		Assert.IsNull(CrontabSchedule.TryParse(expression, includingSeconds));
	}

	[DataRow("bad * * * * *", false)]
	[DataTestMethod]
	public void BadSecondsField(string expression, bool includingSeconds) 
		=> BadField(expression, includingSeconds);

	[DataRow("bad * * * *", false)]
	[DataRow("* bad * * * *", true)]
	[DataTestMethod]
	public void BadMinutesField(string expression, bool includingSeconds) 
		=> BadField(expression, includingSeconds);

	[DataRow("* bad * * *", false)]
	[DataRow("* * bad * * *", true)]
	[DataTestMethod]
	public void BadHoursField(string expression, bool includingSeconds)
		=> BadField(expression, includingSeconds);

	[DataRow("* * bad * *", false)]
	[DataRow("* * * bad * *", true)]
	[DataTestMethod]
	public void BadDayField(string expression, bool includingSeconds) 
		=> BadField(expression, includingSeconds);

	[DataRow("* * * bad *", false)]
	[DataRow("* * * * bad *", true)]
	[DataTestMethod]
	public void BadMonthField(string expression, bool includingSeconds)
		=> BadField(expression, includingSeconds);

	[DataRow("* * * * mon,bad,wed", false)]
	[DataRow("* * * * * mon,bad,wed", true)]
	[DataTestMethod]
	public void BadDayOfWeekField(string expression, bool includingSeconds)
		=> BadField(expression, includingSeconds);

	[DataRow("* 1,2,3,456,7,8,9 * * *", false)]
	[DataRow("* * 1,2,3,456,7,8,9 * * *", true)]
	[DataTestMethod]
	public void OutOfRangeField(string expression, bool includingSeconds)
		=> BadField(expression, includingSeconds);

	[DataRow("* 1,Z,3,4 * * *", false)]
	[DataRow("* * 1,Z,3,4 * * *", true)]
	[DataTestMethod]
	public void NonNumberValueInNumericOnlyField(string expression, bool includingSeconds)
		=> BadField(expression, includingSeconds);

	[DataRow("* 1/Z * * *", false)]
	[DataRow("* * 1/Z * * *", true)]
	[DataTestMethod]
	public void NonNumericFieldInterval(string expression, bool includingSeconds)
		=> BadField(expression, includingSeconds);

	[DataRow("* 3-l2 * * *", false)]
	[DataRow("* * 3-l2 * * *", true)]
	[DataTestMethod]
	public void NonNumericFieldRangeComponent(string expression, bool includingSeconds) 
		=> BadField(expression, includingSeconds);

	/// <summary>
	/// Test case for
	/// <a href="https://github.com/atifaziz/NCrontab/issues/21">issue
	/// #21</a> (GetNextOccurrence throws if next occurrence produces
	/// invalid time).
	/// </summary>
	[TestMethod]
	public void GetNextOccurrences_NextOccurrenceInvalidTime_ShouldStopAtLastValidTime()
	{
		var schedule = CrontabSchedule.Parse("0 0 29 이월 월요일");
		var occurrences = schedule.GetNextOccurrences(new DateTime(9988, 1, 1), DateTime.MaxValue);
		Assert.AreEqual(new DateTime(9988, 2, 29), occurrences.Last());
	}

	// Instead of using strings and parsing as date,
	// consider NUnit's TestCaseData:
	// https://github.com/nunit/docs/wiki/TestCaseData
	[DataRow("0 0 29 이월 월요일", "2017-01-01", "2017-12-31", "2017-12-31")]
	[DataRow("0 0 29 이월 월요일", "9000-01-01", "9008-12-31", "9008-02-29")]
	[DataTestMethod]
	public void GetNextOccurrence(string expression, string startDate, string endDate, string expectedValue)
	{
		var schedule = CrontabSchedule.Parse(expression);
		var start = Time(startDate);
		var end = Time(endDate);
		var expected = Time(expectedValue);

		var occurrence = schedule.GetNextOccurrence(start, end);

		Assert.AreEqual(expected, occurrence);
	}


	private static void CronCall(string startTimeString, string cronExpression, string nextTimeString, bool includeSecond)
	{
		var schedule = CrontabSchedule.Parse(cronExpression, includeSecond);
		var next = schedule.GetNextOccurrence(Time(startTimeString));

		Assert.AreEqual(nextTimeString, TimeString(next),
			"Occurrence of <{0}> after <{1}>.", cronExpression, startTimeString);
	}

	private static void CronFinite(string cronExpression, string startTimeString, string endTimeString, bool includeSecond)
	{
		var schedule = CrontabSchedule.Parse(cronExpression, includeSecond);
		var occurrence = schedule.GetNextOccurrence(Time(startTimeString), Time(endTimeString));

		Assert.AreEqual(endTimeString, TimeString(occurrence),
			"Occurrence of <{0}> after <{1}> did not terminate with <{2}>.",
			cronExpression, startTimeString, endTimeString);
	}

	private static string TimeString(DateTime time)
		=> time.ToString(TimeFormat, CultureInfo.InvariantCulture);

	private static DateTime Time(string str)
		=> DateTime.ParseExact(str, TimeFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);
}

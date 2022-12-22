using Du.Data;
using Du.Globalization;
using Du.Terminal;

namespace Test.Du.Zero;

[TestClass]
public class DataTest
{
    private static readonly string s_line_key_string = "TEST=1\nVALUE=200\nREAL=12.34\nBAD=bad\n";
    private static readonly string s_line_key_int = "10=1\n20=2\n30=bad\n";

    [TestInitialize]
    public void Initialize()
    {

    }

    [TestCleanup]
    public void CleanUp()
    {

    }

    [TestMethod]
    public void TestLineStringDb()
    {
        var u = LineStringDb<int>.Empty();
        u.AddFromContext(s_line_key_string, new StringToIntConverter());
        Assert.IsNotNull(u);
    }

    [TestMethod]
    public void TestLineStringDb_IStringConverter()
    {
        var s = LineStringDb<string>.FromContext(s_line_key_string, new StringToStringConverter());
        Assert.AreEqual("bad", s["BAD"]);

        var i = LineStringDb<int>.FromContext(s_line_key_string, new StringToIntConverter());
        Assert.AreEqual(0, i.Get("REAL"));

        var f = LineStringDb<double>.FromContext(s_line_key_string, new StringToDoubleConverter());
        Assert.AreEqual(12.34, f["REAL"]);
    }

    [TestMethod]
    public void TestLineIntDb()
    {
        var u = LineIntDb<int>.Empty();
        u.AddFromContext(s_line_key_int, new StringToIntConverter());
        Assert.IsNotNull(u);
    }

    [TestMethod]
    public void TestLineIntDb_IStringConverter()
    {
        var s = LineIntDb<string>.FromContext(s_line_key_int, new StringToStringConverter());
        Assert.AreEqual("bad", s[30]);

        var i = LineIntDb<int>.FromContext(s_line_key_int, new StringToIntConverter());
        Assert.AreEqual(0, i[30]);
    }

    [TestMethod]
    public void TestLineDb()
    {
        var l = LineDb.Empty();
        Assert.AreEqual(0, l.Count);

        l.AddFromContext(s_line_key_string);
        Assert.AreEqual(4, l.Count);

        l = LineDb.FromContext(s_line_key_string);
        Assert.AreEqual(4, l.Count);

        Assert.ThrowsException<KeyNotFoundException>(() =>
        {
            var t = l["Undefined"];
        });
    }

    [TestMethod]
    public void TestLineDbV3()
    {
        var line_v3 = s_line_key_string + s_line_key_int;

        var l = LineDbV3.Empty();
        l.AddFromContext(line_v3, true);
        Assert.AreEqual(7, l.Count);


        l = LineDbV3.FromContext(line_v3, false);
        l.Set("KEY", "STRING");
        l.Set(1234, "INT");
        Assert.AreEqual("STRING", l.Get("KEY"));
        Assert.AreEqual("INT", l.Get(1234));

        Assert.AreEqual("oo", l.Get("NOKEY", "oo"));
        Assert.AreEqual("oo", l.Get(65536, "oo"));

        Assert.IsTrue(l.Try("KEY", out string _));
        Assert.IsTrue(l.Try(1234, out string _));

        Assert.IsTrue(l.Count > 0);

        l.Clear();
        Assert.IsTrue(l.Count == 0);
    }

    [TestMethod]
    public void TestGetAttribute()
    {
        var e = TestEnum.No;
        Assert.AreEqual("아뇨", e.GetDescription());
    }

    [TestMethod]
    public void TestGetMessage()
    {
        try
        {
            throw new Exception("MSG");
        }
        catch (Exception ex)
        {
            Assert.AreEqual("MSG", ex.GetMessage());
        }
    }

    [TestMethod]
    public void TestTaskAwait()
    {
        int n = 0;
        var task = Task.Run(() =>
        {
            n = 1;
            Task.Delay(100);
        });
        task.TaskAwait();

        Assert.AreEqual(1, n);
    }

    [TestMethod]
    public void TestSubArray()
    {
        var b = new byte[] { 2, 4, 6, 8, 10 };
        var c = b.SubArray(2, 3);

        Assert.AreEqual(6, c[0]);
        Assert.AreEqual(8, c[1]);
    }

    [TestMethod]
    public void TestEorzeaNow()
    {
        var e = EorzeaTime.Now;
        Assert.IsTrue(e is { Hour: > 0, Minute: > 0 });
    }
}

public enum TestEnum
{
    [System.ComponentModel.Description("아뇨")]
    No,
    [System.ComponentModel.Description("예")]
    Yes,
}

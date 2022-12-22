using Du;

namespace Test.Du.Zero;

[TestClass]
public class ConverterTest
{
	[DataRow("1", 1)]
	[DataRow("2147483640", 2147483640)]
	[DataRow("1234567890123456789", 1234567890123456789)]
	[DataRow("이건숫자가아냐", 0)]
	[DataRow("10.10", 0)]
	[DataTestMethod]
	public void ToLong(string value, long expect)
	{
		Assert.AreEqual(expect, Converter.ToLong(value));
	}

	[DataRow("1", 1)]
	[DataRow("2147483640", 2147483640)]
	[DataRow("1234567890123456789", 0)]
	[DataRow("이건숫자가아냐", 0)]
	[DataRow("10.10", 0)]
	[DataTestMethod]
	public void ToInt(string value, long expect)
	{
		Assert.AreEqual(expect, Converter.ToInt(value));
	}

	[DataRow("1", 1)]
	[DataRow("32760", 32760)]
	[DataRow("60000", 0)]
	[DataRow("123456789", 0)]
	[DataRow("이건숫자가아냐", 0)]
	[DataRow("10.10", 0)]
	[DataTestMethod]
	public void ToShort(string value, long expect)
	{
		Assert.AreEqual(expect, Converter.ToShort(value));
	}

	[DataRow("1", 1)]
	[DataRow("32760", 32760)]
	[DataRow("60000", 60000)]
	[DataRow("-10", 0)]
	[DataRow("이건숫자가아냐", 0)]
	[DataRow("10.10", 0)]
	[DataTestMethod]
	public void ToUshort(string value, long expect)
	{
		Assert.AreEqual(expect, Converter.ToUshort(value));
	}

	[DataRow("true", true)]
	[DataRow("True", true)]
	[DataRow("tRUE", true)]
	[DataRow("false", false)]
	[DataRow("의미없다", false)]
	[DataTestMethod]
	public void ToBool(string value, bool expect)
	{
		Assert.AreEqual(expect, Converter.ToBool(value));
	}

	[DataRow("1", 1.0f)]
	[DataRow("32760", 32760.0f)]
	[DataRow("-4321", -4321.0f)]
	[DataRow("이건숫자가아냐", 0.0f)]
	[DataRow("10.10", 10.10f)]
	[DataTestMethod]
	public void ToFloat(string value, float expect)
	{
		Assert.AreEqual(expect, Converter.ToFloat(value));
	}

	[DataRow("ffffffff", -1 /*4294967295*/)]
	[DataTestMethod]
	public void ToColorArgb(string value, int expect)
	{
		Assert.AreEqual(expect, Converter.ToColorArgb(value).ToArgb());
	}

	[DataRow("127.0.0.1")]
	[DataRow("123.254.1.123")]
	[DataRow("11.1.11.1")]
	[DataTestMethod]
	public void ToIpAddressFromIpv4(string value)
	{
		Assert.AreEqual(value, Converter.ToIpAddressFromIpv4(value).ToString());
	}

	[DataRow("ABCD", "41424344")]
	[DataRow("개똥이", "EAB09CEB98A5EC9DB4")]
	[DataRow("!@#$%", "2140232425")]
	[DataTestMethod]
	public void EncodingDecodingString(string value, string expect)
	{
		Assert.AreEqual(expect, Converter.EncodingString(value));
		Assert.AreEqual(value, Converter.DecodingString(expect));
	}

	[DataRow("ABCD", "QUJDRA==")]
	[DataRow("개똥이", "6rCc65il7J20")]
	[DataRow("!@#$%", "IUAjJCU=")]
	[DataTestMethod]
	public void EncodingDecodingBase64(string value, string expect)
	{
		Assert.AreEqual(expect, Converter.EncodingBase64(value));
		Assert.AreEqual(value, Converter.DecodingBase64(expect));
	}

	[DataRow("ABCD", "040000001F8B080000000000000A737472760100A52017DB04000000")]
	[DataRow("개똥이", "090000001F8B080000000000000A7BB561CEEB194BDFCCDD02009511D4F809000000")]
	[DataRow("!@#$%", "050000001F8B080000000000000A53745056510500CC64702105000000")]
	[DataTestMethod]
	public void CompressDecompressString(string value, string expect)
	{
		Assert.AreEqual(expect, Converter.CompressString(value));
		Assert.AreEqual(value, Converter.DecompressString(expect));
	}
}

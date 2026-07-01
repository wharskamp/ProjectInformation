using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectInformation.Core;

namespace ProjectInformation.Tests;

[TestClass]
public sealed class SignatureEnrichmentExtractorTests
{
    [TestMethod]
    public void Extract_FindsCompanyBusinessPhoneAndMobilePhone()
    {
        var body = """
            Hallo,

            Met vriendelijke groet,
            Anne Tester
            Celsius Benelux B.V.
            T 0318-123456
            M +31 6 12345678
            """;

        var result = SignatureEnrichmentExtractor.Extract(body);

        Assert.AreEqual("Celsius Benelux B.V.", result.Company);
        Assert.AreEqual("0318-123456", result.BusinessTelephoneNumber);
        Assert.AreEqual("+31 6 12345678", result.MobileTelephoneNumber);
    }

    [TestMethod]
    public void Extract_SupportsRequiredPhoneFormats()
    {
        var result = SignatureEnrichmentExtractor.Extract("""
            Test B.V.
            06 12345678
            088-1234567
            """);

        Assert.AreEqual("088-1234567", result.BusinessTelephoneNumber);
        Assert.AreEqual("06 12345678", result.MobileTelephoneNumber);
    }

    [DataTestMethod]
    [DataRow("06-12345678", "", "06-12345678")]
    [DataRow("+31 6 12345678", "", "+31 6 12345678")]
    [DataRow("0318-123456", "0318-123456", "")]
    [DataRow("088-1234567", "088-1234567", "")]
    public void Extract_ClassifiesFirstBusinessAndMobileNumber(string phoneLine, string expectedBusiness, string expectedMobile)
    {
        var result = SignatureEnrichmentExtractor.Extract($"Test B.V.{Environment.NewLine}{phoneLine}");

        Assert.AreEqual(expectedBusiness, result.BusinessTelephoneNumber);
        Assert.AreEqual(expectedMobile, result.MobileTelephoneNumber);
    }
}

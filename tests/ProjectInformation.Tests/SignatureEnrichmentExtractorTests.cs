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
            06-12345678
            088-1234567
            """);

        Assert.AreEqual("088-1234567", result.BusinessTelephoneNumber);
        Assert.AreEqual("06-12345678", result.MobileTelephoneNumber);
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectInformation.Core;

namespace ProjectInformation.Tests;

[TestClass]
public sealed class CompanyResolverTests
{
    [DataTestMethod]
    [DataRow("jan@heijmans.nl", "Heijmans")]
    [DataRow("piet@homij.nl", "Homij")]
    [DataRow("klaas@wfsadvies.nl", "WFS Advies")]
    [DataRow("info@celsius.nl", "Celsius")]
    public void FromEmailDomain_UsesKnownDomainMappings(string email, string expectedCompany)
    {
        Assert.AreEqual(expectedCompany, CompanyResolver.FromEmailDomain(email));
    }

    [TestMethod]
    public void Resolve_UsesConfiguredPriority()
    {
        var result = CompanyResolver.Resolve(
            contactCompany: "",
            exchangeCompany: "Exchange Company",
            signatureCompany: "Signature Company",
            emailAddress: "jan@heijmans.nl");

        Assert.AreEqual("Exchange Company", result);
    }
}

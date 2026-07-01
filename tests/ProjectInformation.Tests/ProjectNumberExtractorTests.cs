using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectInformation.Core;

namespace ProjectInformation.Tests;

[TestClass]
public sealed class ProjectNumberExtractorTests
{
    [TestMethod]
    public void Extract_SupportsAllRequiredProjectNumberFormats()
    {
        var result = ProjectNumberExtractor.Extract("P12345 P-23456 Project 34567 P2415");

        CollectionAssert.AreEqual(
            new[] { "P12345", "P23456", "P2415", "P34567" },
            result.ToArray());
    }

    [TestMethod]
    public void Extract_ReturnsUniqueAlphabeticallySortedProjects()
    {
        var result = ProjectNumberExtractor.Extract("P2415 P2415 P-1234 Project 2415");

        CollectionAssert.AreEqual(
            new[] { "P1234", "P2415" },
            result.ToArray());
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectInformation.Core;

namespace ProjectInformation.Tests;

[TestClass]
public sealed class ProjectNumberExtractorTests
{
    [TestMethod]
    public void Extract_SupportsAllRequiredProjectNumberFormats()
    {
        var result = ProjectNumberExtractor.Extract("P12345 P-23456 Project 34567");

        CollectionAssert.AreEqual(
            new[] { "P12345", "P23456", "P34567" },
            result.ToArray());
    }
}

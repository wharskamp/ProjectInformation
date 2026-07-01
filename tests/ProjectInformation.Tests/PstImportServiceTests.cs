using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectInformation.PstReader;

namespace ProjectInformation.Tests;

[TestClass]
public sealed class PstImportServiceTests
{
    [TestMethod]
    public async Task ImportAsync_ReturnsNotImplementedResult()
    {
        var service = new PstImportService();

        var result = await service.ImportAsync("sample.pst");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("PST import is not implemented yet.", result.Message);
    }
}

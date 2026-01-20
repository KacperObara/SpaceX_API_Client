using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Data;
using NUnit.Framework;
using NSubstitute;

public class CsvDataSourceTests
{
    [Test]
    public async Task GetFileData_ReturnsParsedData()
    {
        var mockLoader = Substitute.For<IFileLoader>();
        mockLoader
            .LoadText(Arg.Any<string>())
            .Returns(UniTask.FromResult("Header1,Header2\n" +
                                                 "ValueA,ValueB\n" +
                                                 "ValueC,ValueD"));

        var dataSource = new CsvDataSource(mockLoader);
        List<string[]> result = await dataSource.GetFileData("test.csv");

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("ValueA", result[0][0]);
        Assert.AreEqual("ValueD", result[1][1]);
    }
}
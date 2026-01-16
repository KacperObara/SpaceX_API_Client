using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Data;
using NUnit.Framework;
using NSubstitute;

public class CSVReaderTests
{
    [Test]
    public async Task LoadCsv_ReturnsParsedData()
    {
        var mockLoader = Substitute.For<IFileLoader>();
        mockLoader
            .LoadText(Arg.Any<string>())
            .Returns(UniTask.FromResult("Header1,Header2\n" +
                                                 "ValueA,ValueB\n" +
                                                 "ValueC,ValueD"));

        CSVReader.FileLoader = mockLoader;

        List<string[]> result = null;
        
        
        await CSVReader.LoadCsvFile("test.csv", r => result = r);


        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("ValueA", result[0][0]);
        Assert.AreEqual("ValueD", result[1][1]);
    }
    
    // Have to delete mock data, because Domain Reloading is disabled in the project
    [TearDown]
    public void TearDown()
    {
        CSVReader.FileLoader = null;
    }
}
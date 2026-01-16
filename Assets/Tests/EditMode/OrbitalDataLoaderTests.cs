using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Data;
using NSubstitute;
using NUnit.Framework;

// We test if loader properly loads data from csv
public class OrbitalDataLoaderTests
{
    private IOrbitalDataService _mockService;
    private OrbitalDataLoader _loader;

    [SetUp]
    public void Setup()
    {
        _mockService = Substitute.For<IOrbitalDataService>();
        _loader = new OrbitalDataLoader(_mockService);
        CSVReader.FileLoader = Substitute.For<IFileLoader>();
    }
    
    [Test]
    public async Task Load_WhenCsvParsed_SetsParsedDataIntoService()
    {
        string csv = string.Join("\n",
            "H1,H2,H3,H4,H5,H6,H7,H8,H9,H10,H11,H12",
            "0,2020-01-01,1,2,3,4,5,6,7,8,9,10",
            "0,2020-01-02,10,20,30,40,50,60,70,80,90,100"
        );

        CSVReader.FileLoader.LoadText(Arg.Any<string>())
            .Returns(UniTask.FromResult(csv));

        List<OrbitalData> mockOrbitalData = null;
        _mockService.SetData(Arg.Do<List<OrbitalData>>(d => mockOrbitalData = d));


        await _loader.Load();


        Assert.NotNull(mockOrbitalData);
        Assert.AreEqual(2, mockOrbitalData.Count);
        Assert.AreEqual(new DateTime(2020, 1, 1), mockOrbitalData[0].Time);
        Assert.AreEqual(10, mockOrbitalData[0].SemimajorAxis);
        Assert.AreEqual(1, mockOrbitalData[0].Eccentricity);
        Assert.AreEqual(3, mockOrbitalData[0].Inclination);
        Assert.AreEqual(90, mockOrbitalData[1].TrueAnomaly);
    }
    
    [TearDown]
    public void TearDown()
    {
        CSVReader.FileLoader = null;
    }
}

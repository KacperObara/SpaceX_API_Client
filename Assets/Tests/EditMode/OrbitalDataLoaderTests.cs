using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Data;
using NSubstitute;
using NSubstitute.Extensions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// We test if loader properly loads data from csv
public class OrbitalDataLoaderTests
{
    private IOrbitalDataService _mockService;
    private IDataSource _mockDataSource;
    private OrbitalDataLoader _loader;

    [SetUp]
    public void Setup()
    {
        _mockService = Substitute.For<IOrbitalDataService>();
        _mockDataSource = Substitute.For<IDataSource>();
        
        _loader = new OrbitalDataLoader(_mockService, _mockDataSource);
    }
    
    [Test]
    public async Task Load_WhenCsvParsed_SetsParsedDataIntoService()
    {
        List<string[]> fakeCsv = new List<string[]>
        {
            new string[] { "0", "2020-01-01", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" },
            new string[] { "0", "2020-01-02", "10", "20", "30", "40", "50", "60", "70", "80", "90", "100" }
        };

        _mockDataSource.GetFileData(Arg.Any<string>()).Returns(_ => UniTask.FromResult<List<string[]>>(fakeCsv));
        
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
}

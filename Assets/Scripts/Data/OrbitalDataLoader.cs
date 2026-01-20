using System;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using RG.OrbitalElements;
using Tools;
using UnityEngine;

namespace Data
{
    public interface IOrbitalDataLoader
    {
        UniTask Load();
    }

    public class OrbitalDataLoader : IOrbitalDataLoader
    {
        private readonly IOrbitalDataService _dataService;
        private readonly IDataSource _dataSource;
        
        private const string CsvFileName = "EarthOrbitalData.csv";

        public OrbitalDataLoader(
            IOrbitalDataService dataService, 
            IDataSource dataSource)
        {
            _dataService = dataService;
            _dataSource = dataSource;
        }
    
        public async UniTask Load()
        {
            await LoadCsv();
        }

        private async UniTask LoadCsv()
        {
            List<string[]> csvData = await _dataSource.GetFileData(CsvFileName);

            List<OrbitalData> dataList = new List<OrbitalData>(csvData.Count);
            CultureInfo culture = CultureInfo.InvariantCulture; // Prevents issues with different decimal separators like , or .

            foreach (var row in csvData)
            {
                DateTime time = DateTime.Parse(row[1], culture);

                OrbitalData data = new OrbitalData
                {
                    Time = time,
                    Eccentricity = double.Parse(row[2], culture),
                    SemimajorAxis = double.Parse(row[11], culture),
                    Inclination = double.Parse(row[4], culture),
                    LongitudeOfAscendingNode = double.Parse(row[5], culture),
                    PeriapsisArgument = double.Parse(row[6], culture),
                    TrueAnomaly = double.Parse(row[10], culture),
                };

                Vector3Double pos = OrbitalPositionToVector3(
                    data.SemimajorAxis,
                    data.Eccentricity,
                    data.Inclination,
                    data.LongitudeOfAscendingNode,
                    data.PeriapsisArgument,
                    data.TrueAnomaly);

                const float scaleFactor = 2.5f / 696_340; // Radius of Unity sun divided by the real sun 

                pos.x *= scaleFactor;
                pos.y *= scaleFactor;
                pos.z *= scaleFactor;

                data.Position = pos;
                dataList.Add(data);
            }

            _dataService.SetData(dataList);
        }
     
        private static Vector3Double OrbitalPositionToVector3(
            double semimajorAxis,
            double eccentricity,
            double inclination,
            double longitudeOfAscendingNode,
            double periapsisArgument,
            double trueAnomaly)
        {
            double deg2Rad = Math.PI / 180.0;
            double num1 = semimajorAxis;
            double d1 = inclination * deg2Rad;
            double num2 = longitudeOfAscendingNode * deg2Rad;
            double num3 = periapsisArgument * deg2Rad;
            double d2 = trueAnomaly * deg2Rad;
            double num4 = 1.0 - eccentricity * eccentricity;
            double num5 = num1 * num4 / (1.0 + eccentricity * Math.Cos(d2));

            return new Vector3Double(
                num5 * (Math.Cos(d2 + num3) * Math.Cos(num2) -
                        Math.Sin(d2 + num3) * Math.Cos(d1) * Math.Sin(num2)),
                num5 * (Math.Cos(d2 + num3) * Math.Sin(num2) +
                        Math.Sin(d2 + num3) * Math.Cos(d1) * Math.Cos(num2)),
                num5 * (Math.Sin(d2 + num3) * Math.Sin(d1)));
        }
    }

    [Serializable]
    public struct OrbitalData
    {
        public DateTime Time;
        public double SemimajorAxis;
        public double Eccentricity;
        public double Inclination;
        public double LongitudeOfAscendingNode;
        public double PeriapsisArgument;
        public double TrueAnomaly;
        public Vector3Double Position;
    
        public Vector3 PositionVector3 => Position.ToVector3();
    }
}
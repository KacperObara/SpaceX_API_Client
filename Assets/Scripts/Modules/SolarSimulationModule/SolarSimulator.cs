using System;
using Data;
using TMPro;
using UnityEngine;
using VContainer;

namespace Modules.SolarSimulationModule
{
    public class SolarSimulator : MonoBehaviour
    {
        [Inject] private readonly IOrbitalDataService _dataService;
    
        [SerializeField] private Transform _sunTransform;
        [SerializeField] private Transform _earthTransform;
        [SerializeField] private LineRenderer _tailRenderer;
        [SerializeField] private TextMeshProUGUI _orbitalDataText;
    
        [SerializeField] private float _hoursPerSecond = 24f; // Simulation speed
        [SerializeField] private int _tailLength = 20; // Number of points
    
        private int _currentIndex = 0;
        private float _timer = 0f;
    
        public void TickSimulation(float deltaTime)
        {
            if (_dataService.OrbitalData.Count == 0) 
                return;
        
            _timer += deltaTime;

            DateTime currentTime = _dataService.OrbitalData[_currentIndex].Time;
            DateTime previousTime = _dataService.OrbitalData[Mathf.Max(0, _currentIndex - 1)].Time;
            TimeSpan timeSpan = currentTime - previousTime;
            int hoursBetween = (int)timeSpan.TotalHours;

            if (_timer * _hoursPerSecond >= hoursBetween)
            {
                _timer = 0f;
                _currentIndex++;
                if (_currentIndex >= _dataService.OrbitalData.Count)
                {
                    _currentIndex = 0;
                }
                ApplyPosition(_currentIndex);
            }
        }

        void ApplyPosition(int idx)
        {
            OrbitalData data = _dataService.OrbitalData[idx];
            _earthTransform.position = _sunTransform.position + data.PositionVector3;
            UpdateTail(idx);
            UpdateUI(data);
        }

        private void UpdateUI(OrbitalData data)
        {
            _orbitalDataText.text = $"Date: {data.Time.ToLocalTime():d}\n" +
                                    $"Semi-major axis au: {data.SemimajorAxis:F5}\n" +
                                    $"Eccentricity: {data.Eccentricity:F5}\n" +
                                    $"Inclination: {data.Inclination:F5}\n" +
                                    $"Longitude of asc. node: {data.LongitudeOfAscendingNode:F5}\n" +
                                    $"Argument of periapsis: {data.PeriapsisArgument:F5}\n" +
                                    $"True Anomaly: {data.TrueAnomaly:F5}";
        }

        void UpdateTail(int currentIndex)
        {
            _tailRenderer.positionCount = Mathf.Min(_tailLength, currentIndex);
            int tailStartIndex = currentIndex - _tailRenderer.positionCount + 1;
        
            for (int i = 0; i < _tailRenderer.positionCount; i++)
            {
                int posIndex = (tailStartIndex + i) % _dataService.OrbitalData.Count;
                _tailRenderer.SetPosition(i, _sunTransform.position + _dataService.OrbitalData[posIndex].PositionVector3);
            }
        }
    }
}



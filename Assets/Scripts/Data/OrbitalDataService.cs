using System.Collections.Generic;

namespace Data
{
    public interface IOrbitalDataService
    {
        IReadOnlyList<OrbitalData> OrbitalData { get; }
        public void SetData(List<OrbitalData> data);
    }

    public class OrbitalDataService : IOrbitalDataService
    {
        public IReadOnlyList<OrbitalData> OrbitalData { get; private set; }

        public void SetData(List<OrbitalData> data)
        {
            OrbitalData = data;
        }
    }
}
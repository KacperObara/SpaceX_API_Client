using System.Collections.Generic;

namespace Data
{
    public interface ILaunchDataService
    {
        IReadOnlyList<LaunchData> LaunchData { get; }
        public void SetData(List<LaunchData> data);
    }

    public class LaunchDataService : ILaunchDataService
    {
        public IReadOnlyList<LaunchData> LaunchData { get; private set; }

        public void SetData(List<LaunchData> data)
        {
            LaunchData = data;
        }
    }
}
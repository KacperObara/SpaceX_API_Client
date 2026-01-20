using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Data
{
    public interface ILaunchDataLoader
    {
        UniTask Load(Action<int> onProgress);
    }

    public class LaunchDataLoader : ILaunchDataLoader
    {
        private const string LaunchesUrl = "https://api.spacexdata.com/v4/launches";
        private const string RocketsUrl  = "https://api.spacexdata.com/v4/rockets";
        private const string PayloadsUrl = "https://api.spacexdata.com/v4/payloads";

        private readonly ILaunchDataService _service;
        private readonly IDataProvider _provider;
        private readonly CancellationToken _ct;

        private List<LaunchData> _launches;
        private Dictionary<string, RocketData> _rockets;
        private Dictionary<string, PayloadData> _payloads;
        private Dictionary<string, Sprite> _rocketImages;
    
        public LaunchDataLoader(
            ILaunchDataService service, 
            IDataProvider provider, 
            CancellationToken ct)
        {
            _service = service;
            _provider = provider;
            _ct = ct;
        }

        public async UniTask Load(Action<int> progress)
        {
            progress?.Invoke(0);

            // 1. Load launches via API client
            await LoadLaunches();
            progress?.Invoke(40);

            // 2. Load only rockets referenced in launches via API client
            var rocketIds = ExtractUniqueIds(_launches.Select(l => l.RocketId));
            await LoadRockets(rocketIds);
            progress?.Invoke(60);
        
            _ct.ThrowIfCancellationRequested();

            // 3. Load only payloads referenced in launches via API client
            var payloadIds = ExtractUniqueIds(_launches.SelectMany(l => l.PayloadIds));
            await LoadPayloads(payloadIds);
            progress?.Invoke(80);

            // 4. Load rocket images in parallel
            await LoadRocketImages();
            progress?.Invoke(100);

            _ct.ThrowIfCancellationRequested();
        
            // 5. Construct final composite data
            LinkData();

            // 6. Load data into service
            _service.SetData(_launches);
        }
    
        private static List<string> ExtractUniqueIds(IEnumerable<string> ids) => new HashSet<string>(ids.Where(id => !string.IsNullOrEmpty(id))).ToList();
    
        private async UniTask LoadLaunches()
        {
            string json = await _provider.GetRequest(LaunchesUrl, _ct);
            _launches = JsonConvert.DeserializeObject<List<LaunchData>>(json);

            // Display newest first
            _launches.Reverse();
        }

        private async UniTask LoadRockets(List<string> rocketIds)
        {
            string json = await _provider.GetRequestsById(RocketsUrl, rocketIds, _ct);

            var parsed = JsonConvert.DeserializeObject<APIClient.QueryResult<RocketData>>(json);
            _rockets = parsed.Docs.ToDictionary(r => r.Id, r => r);
        }

        private async UniTask LoadPayloads(List<string> payloadIds)
        {
            string json = await _provider.GetRequestsById(PayloadsUrl, payloadIds, _ct);

            var parsed = JsonConvert.DeserializeObject<APIClient.QueryResult<PayloadData>>(json);
            _payloads = parsed.Docs.ToDictionary(p => p.Id, p => p);
        }
    
        private async UniTask LoadRocketImages()
        {
            _rocketImages = new Dictionary<string, Sprite>();

            var tasks = new List<UniTask>();

            foreach (var rocket in _rockets.Values)
            {
                if (rocket.ImageIds != null && rocket.ImageIds.Count > 0)
                {
                    string url = rocket.ImageIds[0];
                    tasks.Add(LoadSingleRocketImage(url));
                }
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask LoadSingleRocketImage(string url)
        {
            using var req = UnityWebRequestTexture.GetTexture(url);
            await req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
                return;

            Texture2D original = DownloadHandlerTexture.GetContent(req);
            Texture2D small = DownscaleTexture(original, 256, 256);

            _rocketImages[url] = Sprite.Create(
                small,
                new Rect(0, 0, small.width, small.height),
                new Vector2(0.5f, 0.5f)
            );
        }

        private Texture2D DownscaleTexture(Texture2D source, int width, int height)
        {
            RenderTexture rt = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(source, rt);

            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;

            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);

            return tex;
        }
    
        // Link rockets and payloads to launches
        private void LinkData()
        {
            foreach (var launch in _launches)
            {
                // Rocket
                if (_rockets.TryGetValue(launch.RocketId, out var rocket))
                {
                    rocket.CachedImage = 
                        (rocket.ImageIds != null && rocket.ImageIds.Count > 0 && 
                         _rocketImages.TryGetValue(rocket.ImageIds[0], out var sprite))
                            ? sprite
                            : null;

                    launch.Rocket = rocket;
                }

                // Payloads
                launch.Payloads = new List<PayloadData>();
                foreach (var id in launch.PayloadIds)
                {
                    if (_payloads.TryGetValue(id, out var payload))
                        launch.Payloads.Add(payload);
                }
            }
        }
    }


    [Serializable]
    public class LaunchData
    {
        public string Name;
    
        public bool Upcoming;
    
        [JsonProperty("rocket")]
        public string RocketId;
        public RocketData Rocket;
    
        [JsonProperty("payloads")]
        public List<string> PayloadIds;
        public List<PayloadData> Payloads;
    }

    [Serializable]
    public struct RocketData
    {
        public string Id;
        public string Name;
        public string Country;
    
        [JsonProperty("flickr_images")]
        public List<string> ImageIds;
    
        public Sprite CachedImage;
    }

    [Serializable]
    public struct PayloadData
    {
        public string Id;
        public string Name;
        public string Type;
        [JsonProperty("mass_kg")]
        public string Mass;
        public List<string> Customers;
    }
}
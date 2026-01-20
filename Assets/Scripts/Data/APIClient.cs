using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Data
{
    public interface IDataProvider
    {
        UniTask<string> GetRequest(string url, CancellationToken ct);
        UniTask<string> GetRequestsById(string baseUrl, List<string> ids, CancellationToken ct);
    }
    
    public class APIClient : IDataProvider
    {
        public async UniTask<string> GetRequest(string url, CancellationToken ct)
        {
            using (UnityWebRequest req = UnityWebRequest.Get(url))
            {
                req.timeout = 10;
                await req.SendWebRequest().WithCancellation(ct);

                if (req.result != UnityWebRequest.Result.Success)
                    throw new Exception(req.error);

                return req.downloadHandler.text;
            }
        }
    
        // Complicated query, but it's much more efficient to get only required data, 
        // instead of downloading all and filtering or making multiple API requests one by one.
        public async UniTask<string> GetRequestsById(string baseUrl, List<string> ids, CancellationToken ct)
        {
            var body = new Dictionary<string, object>
            {
                ["query"] = new Dictionary<string, object>
                {
                    ["_id"] = new Dictionary<string, object>
                    {
                        ["$in"] = ids
                    }
                },
                ["options"] = new Dictionary<string, object>
                {
                    ["limit"] = ids.Count
                }
            };
    
            string json = JsonConvert.SerializeObject(body);
            string finalUrl = baseUrl + "/query";
    
            using UnityWebRequest req = new UnityWebRequest(finalUrl, "POST");
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
    
            await req.SendWebRequest().WithCancellation(ct);
        
            if (req.result != UnityWebRequest.Result.Success)
                throw new Exception(req.error);
    
            return req.downloadHandler.text;
        }

    
        // This is needed, because SpaceX API returns results in a "Docs" field
        [Serializable]
        public class QueryResult<T>
        {
            public List<T> Docs;
        }
    }
}
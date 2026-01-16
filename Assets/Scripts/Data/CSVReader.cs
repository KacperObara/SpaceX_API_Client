using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Data
{
    public interface IFileLoader
    {
        UniTask<string> LoadText(string path);
    }

    public class UnityWebRequestFileLoader : IFileLoader
    {
        public async UniTask<string> LoadText(string path)
        {
            using var request = UnityWebRequest.Get(path);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception($"Failed to load file: {request.error}");

            return request.downloadHandler.text;
        }
    }

    public static class CSVReader
    {
        private static IFileLoader _fileLoader;

        // Need to reset default file loader, otherwise tests will overwrite it
        public static IFileLoader FileLoader 
        {
            get => _fileLoader ??= new UnityWebRequestFileLoader();
            set => _fileLoader = value;
        }
        
        public static async UniTask LoadCsvFile(string csvFileName, Action<List<string[]>> onComplete)
        {
            string path = Path.Combine(Application.streamingAssetsPath, csvFileName);

            try
            {
                // I'm using web request instead of normal file read because requests are compatible with mobile devices
                using (UnityWebRequest uwr = UnityWebRequest.Get(path))
                {
                    await uwr.SendWebRequest();

                    if (uwr.result == UnityWebRequest.Result.Success)
                    {
                        string text = uwr.downloadHandler.text;
                        
                        string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
                        onComplete?.Invoke(ParseLines(lines));
                    }
                    else
                    {
                        Debug.LogError($"Failed to load CSV from {path}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    
        private static List<string[]> ParseLines(string[] lines)
        {
            var result = new List<string[]>();
            for (int i = 1; i < lines.Length; i++)  // i=1 to skip header
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                result.Add(lines[i].Split(',')); // Parser returns tokens by splitting by commas.
            }
            return result;
        }
    }
}
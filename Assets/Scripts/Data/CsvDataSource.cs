using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Data
{
    public interface IDataSource
    {
        UniTask<List<string[]>> GetFileData(string fileName);
    }
    
    public interface IFileLoader
    {
        UniTask<string> LoadText(string path);
    }
    
    public class UnityWebRequestFileLoader : IFileLoader
    {
        public async UniTask<string> LoadText(string path)
        {
            using UnityWebRequest request = UnityWebRequest.Get(path);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception($"Failed to load file: {request.error}");

            return request.downloadHandler.text;
        }
    }
    
    public class CsvDataSource : IDataSource
    {
        private readonly IFileLoader _fileLoader;
        
        public CsvDataSource(IFileLoader fileLoader)
        {
            _fileLoader = fileLoader;
        }

        public async UniTask<List<string[]>> GetFileData(string fileName)
        {
            string path = Path.Combine(Application.streamingAssetsPath, fileName);
            string text = await _fileLoader.LoadText(path);
            
            return ParseCsv(text);
        }

        private List<string[]> ParseCsv(string text)
        {
            var result = new List<string[]>();
            string[] lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            
            for (int i = 1; i < lines.Length; i++) // Skip header (i=1)
            {
                result.Add(lines[i].Split(',')); // Parser returns tokens by splitting by commas.
            }
            return result;
        }
    }
}
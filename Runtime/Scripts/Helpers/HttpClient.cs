using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Dynamic.Json;
using TezosSDK.Tezos;
using UnityEngine;
using UnityEngine.Networking;

namespace TezosSDK.Helpers
{
    public class HttpClient
    {
        private string BaseAddress { get; }
        private int RequestTimeout { get; }

        protected HttpClient(TezosConfigSO config)
        {
            BaseAddress = config.Rpc.EndsWith("/") ? config.Rpc : $"{config.Rpc}/";
            RequestTimeout = config.RequestTimeoutSeconds;
        }

        protected IEnumerator GetJson<T>(string path)
        {
            var request = GetUnityWebRequest(UnityWebRequest.kHttpVerbGET, path);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Logger.LogError($"GetJson request failed with error: {request.error} - on url: {request.url}");
                request.Dispose();
                yield break;
            }
            
            // Check if the downloaded text is not null or empty.
            if (string.IsNullOrWhiteSpace(request.downloadHandler.text))
            {
                Logger.LogError("No data or empty JSON received.");
                request.Dispose();
                yield break;
            }

            if (typeof(T) == typeof(string))
            {
                yield return DJson.Parse(request.downloadHandler.text, JsonOptions.DefaultOptions);
            }
            else
            {
                yield return JsonSerializer.Deserialize<T>(request.downloadHandler.text, JsonOptions.DefaultOptions);
            }

            request.Dispose();
        }

        protected IEnumerator PostJson<T>(string path, object data)
        {
            var serializedData = JsonSerializer.Serialize(data, JsonOptions.DefaultOptions);
            var request = GetUnityWebRequest(UnityWebRequest.kHttpVerbPOST, path);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(serializedData));
            request.SetRequestHeader(HttpHeaders.ContentType.Key, HttpHeaders.ContentType.Value);
            request.SendWebRequest();
            yield return new WaitUntil(() => request.isDone);
            yield return JsonSerializer.Deserialize<T>(request.downloadHandler.text, JsonOptions.DefaultOptions);
            request.Dispose();
        }

        private UnityWebRequest GetUnityWebRequest(string method, string path)
        {
            var request = new UnityWebRequest($"{BaseAddress}{path}", method);
            Logger.LogDebug($"Sending {method} request to {request.url}");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader(HttpHeaders.Accept.Key, HttpHeaders.Accept.Value);
            request.SetRequestHeader(HttpHeaders.UserAgent.Key, HttpHeaders.UserAgent.Value);
            request.timeout = RequestTimeout;
            return request;
        }
    }

    internal static class HttpHeaders
    {
        public static KeyValuePair<string, string> ContentType => new("Content-Type", "application/json");
        public static KeyValuePair<string, string> Accept => new("Accept", "application/json");
        public static KeyValuePair<string, string> UserAgent => new("User-Agent", "tezos-unity-sdk");
    }
}
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ExampleYGDateTime
{
    public class DailyRewardUnityService : DailyRewardService
    {
        private const string SERVER_URI = "https://worldtimeapi.org/api/timezone/Europe/Moscow";

        private bool isNetworkError;
        private bool isHttpError;
        private bool isLoaded;
        private bool isCompleteLoaded;
        private bool isLocalDataFounded;

        protected override async UniTask SendRequest()
        {
            try
            {
                UnityWebRequest webRequest = UnityWebRequest.Get(SERVER_URI);
                await webRequest.SendWebRequest();
                Debug.Log($"[DailyRewardService] =>  Server webRequest uri -> {webRequest.uri}");
                Debug.Log($"[DailyRewardService] =>  Server webRequest isDone -> {webRequest.isDone}");
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        isNetworkError = true;
                        isLoaded = true;
                        Debug.Log($"[DailyRewardService] =>  Network Error! -> {webRequest.error}");
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        isHttpError = true;
                        isLoaded = true;
                        Debug.Log($"[DailyRewardService] =>  Data Processing Error! -> {webRequest.error}");
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        isHttpError = true;
                        isLoaded = true;
                        Debug.Log($"[DailyRewardService] => Protocol Error! -> {webRequest.error}");
                        break;
                    case UnityWebRequest.Result.Success:
                        string json = webRequest.downloadHandler.text;
                        ServerTimeResponse response = JsonUtility.FromJson<ServerTimeResponse>(json);
                        serverTime = response.unixtime;
                        Debug.Log($"[DailyRewardService] => Server time in second -> {serverTime}");
                        isCompleteLoaded = true;
                        isLoaded = true;
                        break;
                }
            }
            catch (Exception)
            {
                isNetworkError = true;
                isLoaded = true;
                throw;
            }
        }
    }
}
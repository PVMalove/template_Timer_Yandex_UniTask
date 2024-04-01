using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ExampleYGDateTime
{
    public class DailyRewardUnityService : DailyRewardService
    {
        private const string SERVER_URI = "https://worldtimeapi.org/api/timezone/Europe/Moscow";
        private UnityWebRequest webRequest;

        public override async UniTask<bool> CheckConnection()
        {
            try
            {
                webRequest = UnityWebRequest.Get(SERVER_URI);
                await webRequest.SendWebRequest();
                Debug.Log($"[DailyRewardService] =>  Server webRequest uri -> {webRequest.uri}");
                Debug.Log($"[DailyRewardService] =>  Server webRequest isDone -> {webRequest.isDone}");
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        Debug.LogError($"[DailyRewardService] =>  Network Error! -> {webRequest.error}");
                        return false;
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError($"[DailyRewardService] =>  Data Processing Error! -> {webRequest.error}");
                        return false;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError($"[DailyRewardService] => Protocol Error! -> {webRequest.error}");
                        return false;
                    case UnityWebRequest.Result.Success:
                        return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return default;
        }
        protected override int GetServerTimeNow()
        {
            int serverTime;
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    string json = webRequest.downloadHandler.text;
                    ServerTimeResponse response = JsonUtility.FromJson<ServerTimeResponse>(json);
                    serverTime = response.unixtime;
                    Debug.Log($"[DailyRewardService] => Server time in second -> {serverTime}");
                    break;
                default:
                    throw new Exception($"[DailyRewardService] => Unknown Error! -> {webRequest.error}");
            }
            return serverTime;
        }
    }
}
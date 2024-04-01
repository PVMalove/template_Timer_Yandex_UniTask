using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ExampleYGDateTime
{
    public class DailyRewardYandexService : DailyRewardService
    {
        private UnityWebRequest webRequest;

        public override async UniTask<bool> CheckConnection()
        {
            try
            {
                webRequest = UnityWebRequest.Head(Application.absoluteURL);
                webRequest.SetRequestHeader("cache-control", "no-cache");
                await webRequest.SendWebRequest();
                Debug.Log($"[DailyRewardYandexService] =>  Server webRequest isDone -> {webRequest.isDone}");
                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        Debug.LogError($"[DailyRewardYandexService] =>  Network Error! -> {webRequest.error}");
                        return false;
                    case UnityWebRequest.Result.DataProcessingError:
                        Debug.LogError($"[DailyRewardYandexService] =>  Data Processing Error! -> {webRequest.error}");
                        return false;
                    case UnityWebRequest.Result.ProtocolError:
                        Debug.LogError($"[DailyRewardYandexService] => Protocol Error! -> {webRequest.error}");
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
                    string dateString = webRequest.GetResponseHeader("date");
                    Debug.Log($"[DailyRewardYandexService] => Yandex server time -> {dateString}");
                    DateTimeOffset date = DateTimeOffset.ParseExact(dateString, "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal);
                    Debug.Log($"[DailyRewardYandexService] => Server time in date -> {date}");
                    serverTime = (int)date.ToUnixTimeSeconds();
                    Debug.Log($"[DailyRewardYandexService] => Server time in second -> {serverTime}");
                    break;
                default:
                    throw new Exception($"[DailyRewardYandexService] => Unknown Error! -> {webRequest.error}");
            }

            return serverTime;
        }
    }
}
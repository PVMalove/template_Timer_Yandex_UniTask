using System;
using System.Globalization;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ExampleYGDateTime
{
    public class DailyRewardYandexService : DailyRewardService
    {
        private bool isNetworkError;
        private bool isHttpError;
        private bool isLoaded;
        private bool isCompleteLoaded;
        private bool isLocalDataFounded;

        protected override async UniTask SendRequest()
        {
            try
            {
                using UnityWebRequest webRequest = UnityWebRequest.Head(Application.absoluteURL);
                webRequest.SetRequestHeader("cache-control", "no-cache");
                await webRequest.SendWebRequest();

                switch (webRequest.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        isNetworkError = true;
                        isLoaded = true;
                        Debug.Log($"[DailyRewardYandexService] => Network Error! -> {webRequest.error}");
                        break;
                    case UnityWebRequest.Result.DataProcessingError:
                        isHttpError = true;
                        isLoaded = true;
                        Debug.Log($"[DailyRewardYandexService] => Data Processing Error! -> {webRequest.error}");
                        break;
                    case UnityWebRequest.Result.ProtocolError:
                        isHttpError = true;
                        isLoaded = true;
                        Debug.Log($"[DailyRewardYandexService] => Protocol Error! -> {webRequest.error}");
                        break;
                    case UnityWebRequest.Result.Success:
                        string dateString = webRequest.GetResponseHeader("date");
                        Debug.Log($"[DailyRewardYandexService] => Yandex server time -> {dateString}");
                        DateTimeOffset date = DateTimeOffset.ParseExact(dateString, "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                            CultureInfo.InvariantCulture);
                        serverTime = (int)date.ToUnixTimeSeconds();
                        Debug.Log($"[DailyRewardYandexService] => Server time in second -> {serverTime}");
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
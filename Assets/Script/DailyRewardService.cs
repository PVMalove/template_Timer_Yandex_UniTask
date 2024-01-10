using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YG;

namespace ExampleYGDateTime
{
    public abstract class DailyRewardService
    {
        protected int serverTime;
        
        private int timerTime;
        private double localTime;
        private int deltaTime;
        private bool isActiveTimer;
        private string timeLeft;
        private const int TIME_1H = 3600;
        public event Action onCompletedInitTimer;
        
        private string dateTime;
        private int deltaDateTime;
        private bool isActiveTimeData;
        public event Action onCompletedGetDateTime;

        public string DateTime
        {
            get
            {
                UpdateDateTime();
                return dateTime;
            }
        }

        public string TimeLeft
        {
            get
            {
                UpdateTimeLeftTimer();
                return timeLeft;
            }
        }

        public bool IsActiveTimer => isActiveTimer;

        public async UniTaskVoid GetDateTimeServerYandex()
        {
            await GetServerTimeNow();
            int serverTimeNow = await GetServerTimeNow();
            double localDateTime = Time.realtimeSinceStartupAsDouble;
            deltaDateTime = (int)(serverTimeNow - localDateTime);

            onCompletedGetDateTime?.Invoke();
        }

        public void SetTimerRewardData()
        {
            YandexGame.savesData.lastReceiveCoinsTime = timerTime;
            YandexGame.savesData.isActiveTimer = isActiveTimer;
            YandexGame.SaveProgress();
            Debug.Log($"[DailyRewardService] =>  Save timer reward data / isActiveTimer -> {YandexGame.savesData.isActiveTimer}");
        }

        public async UniTaskVoid InitializeTimerRewardReceived()
        {
            isActiveTimeData = YandexGame.savesData.isActiveTimer;
            if (isActiveTimeData)
            {
                int serverTimeNow = await GetServerTimeNow();
                timerTime = YandexGame.savesData.lastReceiveCoinsTime;
                localTime = Time.realtimeSinceStartupAsDouble;
                deltaTime = (int)(serverTimeNow - localTime);
                isActiveTimer = true;

                Debug.Log($"[DailyRewardService] =>  Initialize timer -> {IsActiveTimer}");
            }
            else
            {
                isActiveTimer = false;
                Debug.Log($"[DailyRewardService] =>  Initialize timer -> {IsActiveTimer}");
            }

            onCompletedInitTimer?.Invoke();
        }
        
        public bool CheckTimerRewardEnded()
        {
            return Time.realtimeSinceStartupAsDouble + deltaTime > timerTime;
        }

        public async UniTask StartTimerRewardReceived()
        {
            int serverTimeNow = await GetServerTimeNow();
            timerTime = TIME_1H + serverTimeNow;
            localTime = Time.realtimeSinceStartupAsDouble;
            deltaTime = (int)(serverTimeNow - localTime);
            isActiveTimeData = true;
            isActiveTimer = true;

            Debug.Log($"[DailyRewardService] =>  Start timer reward received -> {IsActiveTimer}");
        }

        protected abstract UniTask SendRequest();

        private async UniTask<int> GetServerTimeNow()
        {
            await SendRequest();
            return serverTime;
        }
        
        private void UpdateDateTime()
        {
            double calculateTime = deltaDateTime + Time.realtimeSinceStartupAsDouble;
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds((int)calculateTime);
            dateTime = dateTimeOffset.ToString("dd/MM/yyyy HH:mm:ss");
        }
        
        private void UpdateTimeLeftTimer()
        {
            double calculateTimeLeft = timerTime - Time.realtimeSinceStartupAsDouble - deltaTime;
            int minutes = Mathf.FloorToInt((float)calculateTimeLeft / 60);
            int seconds = Mathf.FloorToInt((float)calculateTimeLeft % 60);
            timeLeft = $"{minutes:00}:{seconds:00}";
        }
    }
}
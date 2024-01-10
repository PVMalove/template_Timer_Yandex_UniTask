using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace ExampleYGDateTime
{
    public class ExampleReward : MonoBehaviour
    {
        [SerializeField] private GameObject rewardContainer;
        [SerializeField] private Button addCoinsButton;
        [SerializeField] private GameObject timerContent;
        [SerializeField] private TextMeshProUGUI timerText;
        
        [SerializeField] private TextMeshProUGUI coinCountText_Example;
        
        private DailyRewardService rewardService;
        private bool isActiveTimer;

        private void Awake()
        {
#if UNITY_EDITOR
            Debug.Log("UNITY_EDITOR_DAILY_SERVICE");
            rewardService = new DailyRewardUnityService();
#else
            Debug.Log("YANDEX_DAILY_SERVICE");
            rewardService = new DailyRewardYandexService();
#endif
        }
        
        private void Start()
        {
            coinCountText_Example.text = YandexGame.savesData.CoinCount.ToString(); rewardService.onCompletedInitTimer += OnCompletedInitializeReward;
            rewardService.InitializeTimerRewardReceived().Forget();
            addCoinsButton.onClick.AddListener(AddCoinsWatchingAdsOnClick);
            YandexGame.RewardVideoEvent += RewardedViewingAds;
        }

        private void FixedUpdate()
        {
            if (!rewardService.IsActiveTimer || isActiveTimer) return;

            if (!timerContent.activeSelf)
            {
                timerContent.SetActive(true); 
                Debug.Log($"[ExampleReward] !_timerContent.activeSelf - {timerContent.activeSelf}");
            }

            timerText.text = rewardService.TimeLeft;
            
            if (!rewardService.CheckTimerRewardEnded()) return;
            Debug.Log($"[ExampleReward] CheckTimerRewardEnded -> {rewardService.CheckTimerRewardEnded()}");

            isActiveTimer = true;
            timerContent.SetActive(false);

            Debug.Log("[ExampleReward] End timer");
        }

        private void OnDisable()
        {
            rewardService.onCompletedInitTimer -= OnCompletedInitializeReward;
            addCoinsButton.onClick.RemoveListener(AddCoinsWatchingAdsOnClick);
            YandexGame.RewardVideoEvent -= RewardedViewingAds;
        }

        private void OnCompletedInitializeReward()
        {
            Debug.Log("[ExampleReward] -> OnCompletedInitializeReward");
            rewardContainer.SetActive(true);
            if (rewardService.IsActiveTimer)
            {
                Debug.Log("[ExampleReward] -> _dailyRewardService.IsActiveTimer");
                addCoinsButton.gameObject.SetActive(false);
                timerContent.SetActive(true);
            }
            else
            {
                Debug.Log("[ExampleReward] -> _dailyRewardService.IsActiveTimer = false");
                addCoinsButton.gameObject.SetActive(true);
                timerContent.SetActive(false);
            }
        }
        
        private void AddCoinsWatchingAdsOnClick()
        {
            addCoinsButton.gameObject.SetActive(false);
            YandexGame.RewVideoShow(1);
        }
        
        private async void RewardedViewingAds(int id)
        {
            switch (id)
            {
                case 1:
                    YandexGame.savesData.CoinCount += 100;
                    coinCountText_Example.text = YandexGame.savesData.CoinCount.ToString();
                    await rewardService.StartTimerRewardReceived();
                    rewardService.SetTimerRewardData();
                    break;
            }
        }
    }
}
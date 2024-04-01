using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace ExampleYGDateTime
{
    public class ExampleReword : MonoBehaviour
    {
        [SerializeField] private GameObject rewardContainer;
        [SerializeField] private Button addCoinsButton;
        [SerializeField] private GameObject timerContent;
        [SerializeField] private TextMeshProUGUI timerText;

        [SerializeField] private TextMeshProUGUI coinCountText_Example;
        [SerializeField] private Button resetSaveButton;

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

        private async void Start()
        {
            coinCountText_Example.text = YandexGame.savesData.CoinCount.ToString();
            await LoadDateTimeReward();
        }

        private void OnEnable()
        {
            addCoinsButton.onClick.AddListener(AddCoinsWatchingAdsOnClick);
            resetSaveButton.onClick.AddListener(OnClickResetSaveButton);
            YandexGame.RewardVideoEvent += RewardedViewingAds;
        }

        private void FixedUpdate()
        {
            if (!rewardService.IsActiveTimer) return;

            if (!timerContent.activeSelf)
            {
                timerContent.SetActive(true);
                Debug.Log($"[ExampleReward] !_timerContent.activeSelf - {timerContent.activeSelf}");
            }

            timerText.text = rewardService.TimeLeft;

            if (!rewardService.CheckTimerRewardEnded()) return;
            Debug.Log($"[ExampleReward] CheckTimerRewardEnded -> {rewardService.CheckTimerRewardEnded()}");

            addCoinsButton.gameObject.SetActive(true);
            timerContent.SetActive(false);
            rewardService.IsActiveTimer = false;
            Debug.Log("[ExampleReward] End timer");
        }

        private void OnDisable()
        {
            addCoinsButton.onClick.RemoveListener(AddCoinsWatchingAdsOnClick);
            resetSaveButton.onClick.RemoveListener(OnClickResetSaveButton);
            YandexGame.RewardVideoEvent -= RewardedViewingAds;
        }

        private async UniTask LoadDateTimeReward()
        {
            bool isTimeLoaded = await rewardService.CheckConnection();
            if (isTimeLoaded)
            {
                rewardService.InitializeTimerRewardReceived();
                CompletedInitializeReward();
            }
            else
            {
                coinCountText_Example.text = "Error2";
            }
        }


        private void CompletedInitializeReward()
        {
            rewardContainer.SetActive(true);
            if (rewardService.IsActiveTimer)
            {
                addCoinsButton.gameObject.SetActive(false);
                timerContent.SetActive(true);
            }
            else
            {
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
                    bool isTimeLoaded = await rewardService.CheckConnection();

                    if (isTimeLoaded)
                    {
                        rewardService.StartTimerRewardReceived();
                        rewardService.SetTimerRewardData();
                        YandexGame.savesData.CoinCount += 100;
                        coinCountText_Example.text = YandexGame.savesData.CoinCount.ToString();
                    }
                    else
                    {
                        coinCountText_Example.text = "Error2";
                    }

                    break;
            }
        }

        private void OnClickResetSaveButton()
        {
            YandexGame.ResetSaveProgress();
            YandexGame.SaveProgress();
            coinCountText_Example.text = YandexGame.savesData.CoinCount.ToString();
            addCoinsButton.gameObject.SetActive(true);
            rewardService.IsActiveTimer = false;
            CompletedInitializeReward();
        }
    }
}
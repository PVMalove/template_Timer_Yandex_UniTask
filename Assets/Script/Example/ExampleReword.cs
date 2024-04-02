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

        [SerializeField] private GameObject warningPopup;
        
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
            YandexGame.ErrorVideoEvent += RewardedViewingAdsError;
        }

        private void FixedUpdate()
        {
            if (!rewardService.IsActiveTimer) return;

            if (!timerContent.activeSelf)
            {
                timerContent.SetActive(true);
            }

            timerText.text = rewardService.TimeLeft;

            if (!rewardService.CheckTimerRewardEnded()) return;

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
            YandexGame.ErrorVideoEvent -= RewardedViewingAdsError;
        }

        private async UniTask LoadDateTimeReward()
        {
            bool isTimeLoaded = await rewardService.CheckConnection();
            if (isTimeLoaded)
            {
                warningPopup.SetActive(false);
                rewardService.InitializeTimerRewardReceived();
                CompletedInitializeReward();
            }
            else
            {
                warningPopup.SetActive(true);
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

        private async void RewardedViewingAdsError()
        {
            bool isTimeLoaded = await rewardService.CheckConnection();
            if (!isTimeLoaded)
            {
                warningPopup.SetActive(true);
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
                        YandexGame.savesData.CoinCount += 100;
                        coinCountText_Example.text = YandexGame.savesData.CoinCount.ToString();
                        rewardService.SetTimerRewardData();
                    }
                    else
                    {
                        warningPopup.SetActive(true);
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
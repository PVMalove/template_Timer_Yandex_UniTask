using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace ExampleYGDateTime
{
    public class ExampleDateTime : MonoBehaviour
    {
        [SerializeField] private GameObject dateTimeContent;
        [SerializeField] private TextMeshProUGUI dateTimeYandexText;

        private DailyRewardService rewardService;
        private bool isActive;
        
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
            await LoadDateTime();
        }

        private async UniTask LoadDateTime()
        {
            bool isTimeLoaded = await rewardService.CheckConnection();
            if (isTimeLoaded)
            {
                rewardService.GetDateTimeServer();
                isActive = true;
                dateTimeContent.SetActive(true);
            }
            else
            {
                dateTimeYandexText.text = $"No internet connection...";
                isActive = false;
                dateTimeContent.SetActive(true);
            }
        }

        private void FixedUpdate()
        {
            if (isActive)
                dateTimeYandexText.text = $"UTC: {rewardService.DateTimeNow}";
        }
    }
}
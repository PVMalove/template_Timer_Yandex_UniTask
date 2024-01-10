using TMPro;
using UnityEngine;

namespace ExampleYGDateTime
{
    public class ExampleDateTime : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dateTimeYandexText;
        [SerializeField] private GameObject dateTimeContent;

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

        private void Start()
        {
            rewardService.onCompletedGetDateTime += OnCompletedGetServerTime;
            rewardService.GetDateTimeServerYandex().Forget();
        }

        private void FixedUpdate()
        {
            if (isActive)
                dateTimeYandexText.text = $"UTC: {rewardService.DateTime}";
        }

        private void OnDisable()
        {
            rewardService.onCompletedGetDateTime -= OnCompletedGetServerTime;
        }

        private void OnCompletedGetServerTime()
        {
            dateTimeContent.SetActive(true);
            isActive = true;
        }
    }
}
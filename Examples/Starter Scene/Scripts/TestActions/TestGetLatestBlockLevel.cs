using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tezos.StarterScene
{
    public class TestGetLatestBlockLevel : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _resultText;

        private void Start()
        {
            _button.onClick.AddListener(OnGetLatestBlockLevelButtonClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnGetLatestBlockLevelButtonClicked);
        }

        private void OnGetLatestBlockLevelButtonClicked()
        {
            _resultText.text = "Pending...";
            
            var routine = TezosManager.Instance.GetLatestBlockLevel(latestBlockLevel =>
            {
                _resultText.text = latestBlockLevel.ToString();
            });
            CoroutineRunner.Instance.StartWrappedCoroutine(routine);
        }
    }
}
using Beacon.Sdk.Beacon.Sign;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.Json;

namespace Tezos.StarterScene
{
    public class TestSignMessage : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Button _button;

        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private TMP_InputField _inputField;

        private void Start()
        {
            _button.onClick.AddListener(OnButtonClicked);
            TezosManager.Instance.MessageReceiver.PayloadSigned += OnMessageSigned;
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
            TezosManager.Instance.MessageReceiver.PayloadSigned -= OnMessageSigned;
        }

        private void OnButtonClicked()
        {
            _resultText.text = "";

            TezosManager.Instance.RequestSignPayload(SignPayloadType.micheline, _inputField.text);
        }

        private void OnMessageSigned(string result)
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                _resultText.text = result;
                var json = JsonSerializer.Deserialize<JsonElement>(result);
                var signature = json.GetProperty("signature").GetString();
                _resultText.text = "Signed.";
            });
        }
    }
}

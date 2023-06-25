using System;
using System.Collections.Generic;
using Netezos.Encoding;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tezos.StarterScene
{
    public class TestFA2GetAccountAddressBalance : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _contractAddressText;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private TMP_InputField _inputField;

        private void Start()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            _button.interactable = false;
            _resultText.text = "Pending...";

            var input = new MichelinePrim
            {
                Prim = PrimType.Pair,
                Args = new List<IMicheline>
                {
                    new MichelineString(_inputField.text),
                    new MichelineInt(0)
                }
            };

            CoroutineRunner.Instance.StartWrappedCoroutine(
                TezosManager.Instance.API.ReadView(
                    contractAddress: _contractAddressText.text,
                    entrypoint: "get_balance",
                    input: input,
                    callback: result =>
                    {
                        var intProp = result.GetProperty("int");
                        var intValue = Convert.ToInt32(intProp.ToString());
                        _button.interactable = true;
                        _resultText.text = intValue.ToString();
                    }));
        }
    }
}

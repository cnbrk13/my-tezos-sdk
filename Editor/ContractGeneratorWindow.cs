using UnityEditor;
using UnityEngine;

namespace Tezos.Editor.Windows
{
    public class ContractGeneratorWindow : EditorWindow
    {
        private string contractAddress = "";
        private string contractName = "";
        private string michelsonCode = "";
        private string errorMessage = "";
        private string successMessage = "";

        [MenuItem("Tezos SDK/Contract Generator")]
        public static void ShowWindow()
        {
            GetWindow<ContractGeneratorWindow>("Tezos Contract Generator");
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            contractName = EditorGUILayout.TextField("Contract Name: ", contractName);
            EditorGUILayout.Space();
            
            contractAddress = EditorGUILayout.TextField("Contract Address: ", contractAddress);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Michelson Code", GUILayout.Width(200));
            michelsonCode = EditorGUILayout.TextArea(michelsonCode, GUILayout.Height(60));
            EditorGUILayout.Space();
            
            ValidateFields();
            
            if (!string.IsNullOrEmpty(errorMessage))
            {
                EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
            }

            if (!string.IsNullOrEmpty(successMessage))
            {
                EditorGUILayout.HelpBox(successMessage, MessageType.Info);
            }
            
            if (GUILayout.Button("Generate"))
            {
                GenerateContract();
            }
        }
        
        private void ValidateFields()
        {
            if (string.IsNullOrEmpty(contractName))
            {
                errorMessage = "Contract Name is required.";
            }
            else if (string.IsNullOrEmpty(contractAddress))
            {
                errorMessage = "Contract Address is required.";
            }
            else if (string.IsNullOrEmpty(michelsonCode))
            {
                errorMessage = "Michelson code is required.";
            }
            else
            {
                errorMessage = "";
            }
        }
        
        private void GenerateContract()
        {
            successMessage = "";

            if (!string.IsNullOrEmpty(errorMessage))
                return;
            
            // TODO: Library for generating high-level contract interfaces from Michelson code.
            
            successMessage = "Contract created.";
        }
    }
}
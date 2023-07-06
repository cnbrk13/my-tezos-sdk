using UnityEngine;

namespace TezosSDK.Scripts.IpfsUploader
{
    public static class UploaderFactory
    {
        public static IFileUploader GetPinataUploader()
        {
            IFileUploader uploader = null;
#if UNITY_WEBGL && !UNITY_EDITOR
            uploader = WebUploaderHelper.InitWebFileLoader();
#elif UNITY_EDITOR
            var editorUploaderGameObject = GameObject.Find(nameof(EditorUploader));
            uploader = editorUploaderGameObject != null
                ? editorUploaderGameObject.GetComponent<EditorUploader>()
                : new GameObject(nameof(EditorUploader)).AddComponent<EditorUploader>();
#endif
            return uploader;
        }

        public static IFileUploader GetOnchainUploader()
        {
            return new EditorUploader();
        }
    }
}
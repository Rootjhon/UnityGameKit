/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: ABLoadManifestAsyncOperation.cs
			// Describle:
			// Created By:  Jhon
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/
using UnityEngine;

namespace BaseKit
{
    sealed class ABLoadManifestAsyncOperation : LoadOperation
    {
        #region [Fields]
        private bool LoadProcessError = false;
        private AssetBundleCreateRequest ABCRequest;
        private AssetBundleRequest ABRequest;
        public LoadAssetCallback<AssetBundleManifest> OperationCallback { get; set; }
        #endregion

        #region [Construct]
        public ABLoadManifestAsyncOperation() { }
        ~ABLoadManifestAsyncOperation()
        {
            ABCRequest = null; ABRequest = null;
            OperationCallback = null;
        }
        #endregion

        #region [Abstract]
        public override bool IsDone()
        {
            if (true == LoadProcessError) return true;
            if (false == ABCRequest.isDone) return false;

            if (null == ABRequest)
            {
                ABRequest = ABCRequest.assetBundle.LoadAssetAsync<AssetBundleManifest>(CallbackParam.AssetName);
            }

            return ABRequest.isDone;
        }
        public override void Finish()
        {
            if (null != OperationCallback) OperationCallback(GetAsset<AssetBundleManifest>(), CallbackParam);
        }
        #endregion

        #region [Inherit]
        public override T GetAsset<T>()
        {
            if (ABRequest != null && ABRequest.isDone)
                return ABRequest.asset as T;
            else
                return default(T);
        }
        public override bool Execute()
        {
            if (string.IsNullOrEmpty(CallbackParam.AssetName) || CallbackParam.AssetName.ToLower().Equals("null"))
            {
#if LogFlag
                Debug.LogError("ABLoadManifestAsyncOperation.cs Execute Error AssetName Error ,AssetName : " + CallbackParam.AssetName);
#endif
                LoadProcessError = true;
                return true;
            }
            string tempPath = ResourceManager.GetSingle().ConvertDiskSavaPath(CallbackParam.AssetBundleName);
            ABCRequest = AssetBundle.LoadFromFileAsync(tempPath);
            return false;
        }
        #endregion
    }
}
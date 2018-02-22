/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: ABLoadManifestOperation.cs
			// Describle:
			// Created By:  Jhon_朱俊强
			// Date&Time:  2017/12/11 星期一 13:42:13
            // Modify History: 
            //
//----------------------------------------------------------------*/

using UnityEngine;

namespace BaseKit
{
    sealed class ABLoadManifestOperation : LoadOperation
    {
        #region [Fields]
        private AssetBundleManifest mAssetBundleManifest;
        public LoadAssetCallback<AssetBundleManifest> OperationCallback { get; set; }
        #endregion

        #region [Construct]
        public ABLoadManifestOperation() { }
        ~ABLoadManifestOperation()
        {
            mAssetBundleManifest = null;
            OperationCallback = null;
        }
        #endregion

        #region [Abstract]
        public override bool IsDone() { return true; }
        public override void Finish()
        {
            if (null != OperationCallback) OperationCallback(GetAsset<AssetBundleManifest>(), CallbackParam);
        }
        #endregion

        #region [Inherit]
        public override T GetAsset<T>()
        {
            if (mAssetBundleManifest != null)
                return mAssetBundleManifest as T;
            else
                return default(T);
        }
        public override bool Execute()
        {
            if (string.IsNullOrEmpty(CallbackParam.AssetName) || CallbackParam.AssetName.ToLower().Equals("null"))
            {
#if LogFlag
                Debug.LogError("ABLoadManifestOperation.cs Execute Error AssetName Error ,AssetName : " + CallbackParam.AssetName);
#endif
                return true;
            }
            string tempPath = ResourceManager.GetSingle().ConvertDiskSavaPath(CallbackParam.AssetBundleName);
            AssetBundle tempAssetBundle = AssetBundle.LoadFromFile(tempPath);
            mAssetBundleManifest = tempAssetBundle.LoadAsset<AssetBundleManifest>(CallbackParam.AssetName);
            return true;
        }
        #endregion
    }
}
/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: ABLoadAssetOperation.cs
			// Describle:
			// Created By:  Jhon_朱俊强
			// Date&Time:  2017/12/11 星期一 14:08:35
            // Modify History: 
            //
//----------------------------------------------------------------*/

using UnityEngine;
using System.Collections.Generic;

namespace BaseKit
{
    sealed class ABLoadAssetOperation<TAssetType> : LoadOperation where TAssetType : UnityEngine.Object
    {
        #region [Fields]
        private TAssetType mTarget;
        private List<string> Dependcies;
        public LoadAssetCallback<TAssetType> OperationCallback { get; private set; }
        #endregion

        #region [Construct]
        public ABLoadAssetOperation(LoadAssetCallback<TAssetType> varCallBack) { OperationCallback = varCallBack; }
        ~ABLoadAssetOperation()
        {
            Dependcies.Clear(); Dependcies = null;
            mTarget = null; OperationCallback = null;
        }
        #endregion

        #region [Abstract]
        public override bool IsDone() { return true; }
        public override void Finish()
        {
            if (null != OperationCallback) OperationCallback(mTarget, CallbackParam);
        }
        #endregion

        #region [Inherit]
        public override T GetAsset<T>() { return mTarget as T; }
        public override bool Execute()
        {
            if (string.IsNullOrEmpty(CallbackParam.AssetBundleName) || string.IsNullOrEmpty(CallbackParam.AssetName) || CallbackParam.AssetName.ToLower().Equals("null"))
            {
#if LogFlag
                Debug.LogError("ABLoadAssetOperation.cs Execute Error AssetName Error,AssetBundleName :" + CallbackParam.AssetBundleName + " ,AssetName : " + CallbackParam.AssetName);
#endif
                return true;
            }
            if (false == ResourceManager.GetSingle().AnalysisDependenciesReady(CallbackParam.AssetBundleName, out Dependcies))
            {
#if LogFlag
                Debug.LogError("ABLoadAssetOperation.cs Execute AnalysisDependenciesReady Error, AssetBundleName :" + CallbackParam.AssetBundleName + " ,AssetName : " + CallbackParam.AssetName);
#endif
                return true;
            }
            LoadDependcies();
            LoadMainAsset();
            return true;
        }
        #endregion

        #region [Business]
        private void LoadDependcies()
        {
            if (Dependcies.Count == 0) return;

            for (int i = 0; i < Dependcies.Count; i++)
            {
                string tempPath = ResourceManager.GetSingle().ConvertDiskSavaPath(Dependcies[i]);
                var tempBundle = AssetBundle.LoadFromFile(tempPath);
                LoadedAssetBundle tempLAB;
                ResourceManager.GetSingle().CacheLoadedBundle(Dependcies[i], tempBundle, out tempLAB);
                tempLAB.LoadAsset(Dependcies[i]);
            }
        }
        private void LoadMainAsset()
        {
            LoadedAssetBundle tempLastAB;
            bool tempAlreadyLoad = ResourceManager.GetSingle().GetCacheBundle(CallbackParam.AssetBundleName, out tempLastAB);
            if (false == tempAlreadyLoad)
            {
                string tempPath = ResourceManager.GetSingle().ConvertDiskSavaPath(CallbackParam.AssetBundleName);
                var tempBundle = AssetBundle.LoadFromFile(tempPath);
                ResourceManager.GetSingle().CacheLoadedBundle(CallbackParam.AssetBundleName, tempBundle, out tempLastAB);
            }
            mTarget = tempLastAB.LoadAsset<TAssetType>(CallbackParam.AssetName);
        }
        #endregion
    }
}

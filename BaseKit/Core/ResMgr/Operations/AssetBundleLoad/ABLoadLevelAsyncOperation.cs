/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: ABLoadLevelAsyncOperation.cs
			// Describle:
			// Created By:  Jhon
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace BaseKit
{
    sealed class ABLoadLevelAsyncOperation : LoadOperation
    {
        #region [Fields]
        private bool LoadProcessError = false;
        private bool LoadDependciesFin = false;
        private List<string> Dependcies;
        private AsyncOperation Request;
        private LoadSceneMode mLoadSceneMode;
        private AssetBundleCreateRequest DependRequest;
        private AssetBundleCreateRequest ABCRequest;
        public LoadAssetCallback OperationCallback { get; private set; }
        #endregion

        #region [Construct]
        public ABLoadLevelAsyncOperation(LoadSceneMode varLoadSceneMode, LoadAssetCallback varCallBack)
        {
            mLoadSceneMode = varLoadSceneMode;
            OperationCallback = varCallBack;
        }
        ~ABLoadLevelAsyncOperation()
        {
            Dependcies.Clear(); Dependcies = null;
            Request = null; DependRequest = null;
            ABCRequest = null; OperationCallback = null;
        }
        #endregion

        #region [Abstract]
        public override bool IsDone()
        {
            if (true == LoadProcessError) return true;
            if (null != Request) return Request.isDone;

            if (null != ABCRequest)
            {
                if (true == ABCRequest.isDone)
                {
                    LoadedAssetBundle tempLAB;
                    ResourceManager.GetSingle().CacheLoadedBundle(CallbackParam.AssetBundleName, ABCRequest.assetBundle, out tempLAB);
                    Request = SceneManager.LoadSceneAsync(CallbackParam.AssetName, mLoadSceneMode);
                    return Request.isDone;
                }
                return false;
            }

            if (true == LoadDependciesFin)
            {
                LoadedAssetBundle tempLAB;
                if (true == ResourceManager.GetSingle().GetCacheBundle(CallbackParam.AssetBundleName, out tempLAB))
                {
                    Request = SceneManager.LoadSceneAsync(CallbackParam.AssetName, mLoadSceneMode);
                    return Request.isDone;
                }
                string tempPath = ResourceManager.GetSingle().ConvertDiskSavaPath(CallbackParam.AssetBundleName);
                ABCRequest = AssetBundle.LoadFromFileAsync(tempPath);
                return false;
            }

            LoadDependciesFin = LoadDependcies();
            return false;
        }
        public override void Finish()
        {
            if (null != OperationCallback) OperationCallback(CallbackParam);
        }
        #endregion

        #region [Inherit]
        public override bool Execute()
        {
            if (string.IsNullOrEmpty(CallbackParam.AssetBundleName) || string.IsNullOrEmpty(CallbackParam.AssetName) || CallbackParam.AssetName.ToLower().Equals("null"))
            {
#if LogFlag
                Debug.LogError("ABLoadLevelOperation.cs Execute Error AssetName Error ,AssetBundleName :" + CallbackParam.AssetBundleName + " ,AssetName : " + CallbackParam.AssetName);
#endif
                LoadProcessError = true;
                return true;
            }
            if (false == ResourceManager.GetSingle().AnalysisDependenciesReady(CallbackParam.AssetBundleName, out Dependcies))
            {
                LoadProcessError = true;
#if LogFlag
                Debug.LogError("ABLoadLevelOperation.cs Execute AnalysisDependenciesReady Error, AssetBundleName :" + CallbackParam.AssetBundleName + " ,AssetName : " + CallbackParam.AssetName);
#endif
                return true;
            }
            if (0 == Dependcies.Count) LoadDependciesFin = true;
            return false;
        }
        #endregion

        #region [Business]
        private bool LoadDependcies()
        {
            if (null == DependRequest)
            {
                if (0 == Dependcies.Count) return true;
                string tempPath = ResourceManager.GetSingle().ConvertDiskSavaPath(Dependcies[0]);
                DependRequest = AssetBundle.LoadFromFileAsync(tempPath);
                return false;
            }
            if (true == DependRequest.isDone)
            {
                LoadedAssetBundle tempLAB;
                ResourceManager.GetSingle().CacheLoadedBundle(Dependcies[0], DependRequest.assetBundle, out tempLAB);
                tempLAB.LoadAsset(Dependcies[0]);
                Dependcies.RemoveAt(0);
                if (0 == Dependcies.Count) return true;
                string tempPath = ResourceManager.GetSingle().ConvertDiskSavaPath(Dependcies[0]);
                DependRequest = AssetBundle.LoadFromFileAsync(tempPath);
            }
            return false;
        }
        #endregion
    }
}
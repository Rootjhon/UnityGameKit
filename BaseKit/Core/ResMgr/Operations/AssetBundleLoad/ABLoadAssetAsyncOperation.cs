/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: ABLoadAssetAsyncOperation.cs
			// Describle:
			// Created By:  Jhon
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;

namespace BaseKit
{
    sealed class ABLoadAssetAsyncOperation<TAssetType> : LoadOperation where TAssetType : UnityEngine.Object
    {
        #region [Fields]
        private bool LoadDependciesFin = false;
        private bool AlreadyLoad = false;
        private List<string> Dependcies;
        private AssetBundleCreateRequest ABCRequest;
        private AssetBundleRequest ABRequest;
        private AssetBundleCreateRequest DependRequest;
        public LoadAssetCallback<TAssetType> OperationCallback { get; private set; }
        #endregion

        #region [Construct]
        public ABLoadAssetAsyncOperation(LoadAssetCallback<TAssetType> varCallBack) { OperationCallback = varCallBack; }
        ~ABLoadAssetAsyncOperation()
        {
            Dependcies.Clear(); Dependcies = null;
            ABCRequest = null; ABRequest = null;
            DependRequest = null; OperationCallback = null;
        }
        #endregion

        #region [Abstract]
        public override bool IsDone()
        {
            if (true == AlreadyLoad) return ABRequest.isDone;

            if (null != ABCRequest)
            {
                if (ABCRequest.isDone)
                {
                    if (null != ABRequest)
                    {
                        return ABRequest.isDone;
                    }

                    LoadedAssetBundle tempLAB;
                    if (false == ResourceManager.GetSingle().CacheLoadedBundle(CallbackParam.AssetBundleName, ABCRequest.assetBundle, out tempLAB))
                    {
                        return true;
                    }
                    ABRequest = tempLAB.LoadAssetAsync<TAssetType>(CallbackParam.AssetName);
                    return ABRequest.isDone;
                }
                else
                {
                    return false;
                }
            }

            if (true == LoadDependciesFin)
            {
                LoadedAssetBundle tempLAB;
                AlreadyLoad = ResourceManager.GetSingle().GetCacheBundle(CallbackParam.AssetBundleName, out tempLAB);
                if (true == AlreadyLoad)
                {
                    ABRequest = tempLAB.LoadAssetAsync<TAssetType>(CallbackParam.AssetName);
                    return ABRequest.isDone;
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
            if (null != OperationCallback) OperationCallback(GetAsset<TAssetType>(), CallbackParam);
        }
        #endregion

        #region [Inherit]
        public override T GetAsset<T>()
        {
            if (ABRequest != null && ABRequest.isDone)
            {
                return ABRequest.asset as T;
            }
            return default(T);
        }
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
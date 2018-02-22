/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: ResourceLLevelAsyncOperation.cs
			// Describle:
			// Created By:  Jhon
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BaseKit
{
    sealed class ResourceLLevelAsyncOperation : LoadOperation
    {
        #region [Fields]
        private LoadSceneMode mLoadSceneMode;
        private AsyncOperation Request;
        private bool LoadProcessError = false;
        public LoadAssetCallback OperationCallback { get; private set; }
        #endregion

        #region [Construct]
        public ResourceLLevelAsyncOperation(LoadSceneMode varLoadSceneMode, LoadAssetCallback varCallBack)
        {
            mLoadSceneMode = varLoadSceneMode;
            OperationCallback = varCallBack;
        }
        ~ResourceLLevelAsyncOperation() { Request = null; OperationCallback = null; }
        #endregion

        #region [Abstract]
        public override bool IsDone()
        {
            if (null == Request || true == LoadProcessError)
            {
#if LogFlag
                Debug.LogError("ResourceLoadLevelOperation.cs IsDone Error happen in LoadProcess.");
#endif
                return true;
            }
            return Request.isDone;
        }
        public override void Finish()
        {
            if (null != OperationCallback) OperationCallback(CallbackParam);
        }
        #endregion

        #region [Inherit]
        public override bool Execute()
        {
            if (string.IsNullOrEmpty(CallbackParam.AssetName))
            {
                LoadProcessError = true;
#if LogFlag
                Debug.LogError("ResourceLLevelAsyncOperation.cs Execute LevelName IsNullOrEmpty.");
#endif
                return true;
            }
            Request = SceneManager.LoadSceneAsync(CallbackParam.AssetName, mLoadSceneMode);
            return false;
        }
        #endregion
    }
}
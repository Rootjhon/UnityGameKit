/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: ResourceLoadLevelOperation.cs
			// Describle:
			// Created By:  Jhon_朱俊强
			// Date&Time:  2017/12/6 星期三 18:25:22
            // Modify History: 
            //
//----------------------------------------------------------------*/
using UnityEngine.SceneManagement;

namespace BaseKit
{
    sealed class ResourceLoadLevelOperation : LoadOperation
    {
        #region [Fields]
        private LoadSceneMode mLoadSceneMode;
        public LoadAssetCallback OperationCallback { get; private set; }
        #endregion

        #region [Construct]
        public ResourceLoadLevelOperation(LoadSceneMode varLoadSceneMode, LoadAssetCallback varCallBack)
        {
            mLoadSceneMode = varLoadSceneMode;
            OperationCallback = varCallBack;
        }
        ~ResourceLoadLevelOperation() { OperationCallback = null; }
        #endregion

        #region [Abstract]
        public override bool IsDone() { return true; }
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
#if LogFlag
                UnityEngine.Debug.LogError("ResourceLLevelAsyncOperation.cs Execute LevelName IsNullOrEmpty.");
#endif
                return true;
            }
            SceneManager.LoadScene(CallbackParam.AssetName, mLoadSceneMode);
            return true;
        }
        #endregion
    }
}
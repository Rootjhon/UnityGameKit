/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: SLABLoadLevelOperation.cs
			// Describle:
			// Created By:  Jhon_朱俊强
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/

#if EditorVersion
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BaseKit
{
    sealed class SLABLoadLevelOperation : LoadOperation
    {
        #region [Fields]
        private LoadSceneMode mLoadSceneMode;
        private AsyncOperation Request;
        private bool LoadProcessError = false;
        public LoadAssetCallback OperationCallback { get; private set; }
        #endregion

        #region [Construct]
        public SLABLoadLevelOperation(LoadSceneMode varLoadSceneMode, LoadAssetCallback varCallBack)
        {
            mLoadSceneMode = varLoadSceneMode;
            OperationCallback = varCallBack;
        }
        ~SLABLoadLevelOperation() { Request = null; OperationCallback = null; }
        #endregion

        #region [Abstract]
        public override bool IsDone()
        {
            if (null == Request || true == LoadProcessError)
            {
#if LogFlag
                Debug.LogError("SLABLoadLevelOperation.cs IsDone Error happen in LoadProcess.");
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
            if (string.IsNullOrEmpty(CallbackParam.AssetName)) LoadProcessError = true;
            string[] tempLevelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(CallbackParam.AssetBundleName, CallbackParam.AssetName);
            if (tempLevelPaths.Length == 0)
            {
                //@TODO: The error needs to differentiate that an asset bundle name doesn't exist
                //from that there right scene does not exist in the asset bundle...
#if LogFlag
                Debug.LogError(string.Format("SLABLoadLevelOperation.cs Execute There is no scene with name \" {0} \" in {1}.",
                    CallbackParam.AssetName, CallbackParam.AssetBundleName));
#endif
                return true;
            }

            if (mLoadSceneMode == LoadSceneMode.Additive)
            {
                Request = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode(tempLevelPaths[0]);
            }
            else
            {
                Request = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode(tempLevelPaths[0]);
            }
            return false;
        }
        #endregion
    }
}
#endif
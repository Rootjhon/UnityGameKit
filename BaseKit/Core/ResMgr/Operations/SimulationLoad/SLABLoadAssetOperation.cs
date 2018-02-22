/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: SLABLoadAssetOperation.cs
			// Describle:
			// Created By:  Jhon_朱俊强
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/

#if EditorVersion
using UnityEngine;
using UnityEditor;

namespace BaseKit
{
    sealed class SLABLoadAssetOperation<TAssetType> : LoadOperation where TAssetType : UnityEngine.Object
    {
        #region [Fields]
        private TAssetType mTarget;
        public LoadAssetCallback<TAssetType> OperationCallback { get; private set; }
        #endregion

        #region [Construct]
        public SLABLoadAssetOperation(LoadAssetCallback<TAssetType> varCallBack) { OperationCallback = varCallBack; }
        ~SLABLoadAssetOperation() { OperationCallback = null; }
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
            string[] tempAssetPaths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(CallbackParam.AssetBundleName, CallbackParam.AssetName);
            if (tempAssetPaths.Length == 0)
            {
#if LogFlag
                Debug.LogError("SLABLoadAssetOperation.cs Execute There is no asset with name \"" + CallbackParam.AssetName + "\" in " + CallbackParam.AssetBundleName);
#endif
                return true;
            }
            // @TODO: Now i only get the main object from the first asset. Should consider type also.
            //mTarget = AssetDatabase.LoadMainAssetAtPath(tempAssetPaths[0]);
            mTarget = AssetDatabase.LoadAssetAtPath<TAssetType>(tempAssetPaths[0]);
            return false;
        }
        #endregion
    }
}
#endif
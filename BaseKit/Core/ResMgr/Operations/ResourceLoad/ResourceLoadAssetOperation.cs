/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: ResourceLoadAssetOperation.cs
			// Describle:
			// Created By:  Jhon_朱俊强
			// Date&Time:  2017/12/6 星期三 17:57:57
            // Modify History: 
            //
//----------------------------------------------------------------*/

using UnityEngine;
using System.IO;

namespace BaseKit
{
    sealed class ResourceLoadAssetOperation<TAssetType> : LoadOperation where TAssetType : UnityEngine.Object
    {
        #region [Fields]
        private TAssetType mTarget;
        public LoadAssetCallback<TAssetType> OperationCallback { get; private set; }
        #endregion

        #region [Construct]
        public ResourceLoadAssetOperation(LoadAssetCallback<TAssetType> varCallBack) { OperationCallback = varCallBack; }
        ~ResourceLoadAssetOperation() { mTarget = null; OperationCallback = null; }
        #endregion

        #region [Abstract]
        /// <summary>
        /// 是否完成加载，True = 完成;
        /// </summary>
        /// <returns></returns>
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
            if (string.IsNullOrEmpty(CallbackParam.AssetName))
            {
#if LogFlag
                Debug.LogError("ResourceLoadAssetOperation.cs Execute AssetName IsNullOrEmpty.");
#endif
                return true;
            }
            mTarget = Resources.Load<TAssetType>(Path.Combine(CallbackParam.AssetBundleName, CallbackParam.AssetName));
            return true;
        }
        #endregion
    }
}
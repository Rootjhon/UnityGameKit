/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: ResourceLAssetAsyncOperation.cs
			// Describle:
			// Created By:  Jhon
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System.IO;
using System;

namespace BaseKit
{
    sealed class ResourceLAssetAsyncOperation<TAssetType> : LoadOperation where TAssetType : UnityEngine.Object
    {
        #region [Fields]
        private ResourceRequest mRequest;
        private bool BoolError = false;
        public LoadAssetCallback<TAssetType> OperationCallback { get; private set; }
        #endregion

        #region [Construct]
        public ResourceLAssetAsyncOperation(LoadAssetCallback<TAssetType> varCallback) { OperationCallback = varCallback; }
        ~ResourceLAssetAsyncOperation() { mRequest = null; OperationCallback = null; }
        #endregion

        #region [Abstract]
        /// <summary>
        /// 是否完成加载，True = 完成;
        /// </summary>
        /// <returns></returns>
        public override bool IsDone()
        {
            if (BoolError || mRequest == null) return true;
            return mRequest.isDone;
        }
        public override void Finish()
        {
            if (null != OperationCallback) OperationCallback(GetAsset<TAssetType>(), CallbackParam);
        }
        #endregion

        #region [Inherit]
        public override T GetAsset<T>()
        {
            if (mRequest != null && mRequest.isDone)
            {
                return mRequest.asset as T;
            }
            return default(T);
        }
        public override bool Execute()
        {
            if (string.IsNullOrEmpty(CallbackParam.AssetName))
            {
                BoolError = true;
#if LogFlag
                Debug.LogError("ResourceLAssetAsyncOperation.cs Execute AssetName IsNullOrEmpty.");
#endif
                return true;
            }
            mRequest = Resources.LoadAsync<TAssetType>(Path.Combine(CallbackParam.AssetBundleName, CallbackParam.AssetName));
            return mRequest.isDone;
        }
        #endregion
    }
}
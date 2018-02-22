/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: LoadedAssetBundle.cs
			// Describle:
			// Created By:  Jhon
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/
using System;
using UnityEngine;
using System.Collections.Generic;

namespace BaseKit
{
    /// <summary>
    /// 封装AssetBundle,内部管理引用关系;
    /// </summary>
    public class LoadedAssetBundle
    {
        #region [Fields]
        private AssetBundle mAssetBundle;
        private Dictionary<string, int> mRefDic;
        #endregion

        #region [Construct]
        /// <summary>
        /// ;
        /// </summary>
        /// <param name="varAssetBundle"></param>
        public LoadedAssetBundle(AssetBundle varAssetBundle)
        {
            mAssetBundle = varAssetBundle;
        }
        #endregion

        #region [PublicTools]
        /// <summary>
        /// 卸载AB引用;
        /// </summary>
        /// <param name="varAssetName"></param>
        /// <param name="varTruthResult"> 这个AB包是否被卸载了</param>
        /// <returns></returns>
        public bool UnLoad(string varAssetName, ref bool varTruthResult)
        {
            varTruthResult = false;
            if (null == mAssetBundle)
            {
#if LogFlag
                Debug.LogError("LoadedAssetBundle.cs UnLoad null == AssetBundle.");
#endif
                return false;
            }
            if (null == mRefDic)
            {
#if LogFlag
                Debug.LogWarning("LoadedAssetBundle.cs UnLoad null == mRefDic.");
#endif
                return false;
            }
            if (false == mRefDic.ContainsKey(varAssetName))
            {
#if LogFlag
                Debug.LogWarning("LoadedAssetBundle.cs UnLoad Not Contains Asset : " + varAssetName);
#endif
                return false;
            }
            mRefDic[varAssetName] -= 1;
            if (mRefDic[varAssetName] <= 0) mRefDic.Remove(varAssetName);
            if (mRefDic.Count == 0)
            {
                varTruthResult = true;
                mAssetBundle.Unload(true);
            }
            return true;
        }
        /// <summary>
        /// 同步加载资源;
        /// </summary>
        /// <param name="varAssetName">资源名</param>
        /// <returns></returns>
        public UnityEngine.Object LoadAsset(string varAssetName)
        {
            AddAssetRefTree(varAssetName);
            return mAssetBundle.LoadAsset(varAssetName);
        }
        /// <summary>
        /// 同步加载资源;
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="varAssetName">资源名</param>
        /// <returns></returns>
        public T LoadAsset<T>(string varAssetName) where T : UnityEngine.Object
        {
            AddAssetRefTree(varAssetName);
            return mAssetBundle.LoadAsset<T>(varAssetName);
        }
        /// <summary>
        /// 异步加载资源;
        /// </summary>
        /// <param name="varAssetName">资源名</param>
        /// <param name="varAssetType">资源类型</param>
        /// <returns></returns>
        public AssetBundleRequest LoadAssetAsync(string varAssetName, Type varAssetType)
        {
            AddAssetRefTree(varAssetName);
            return mAssetBundle.LoadAssetAsync(varAssetName, varAssetType);
        }
        /// <summary>
        /// 异步加载资源;
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="varAssetName">资源名</param>
        /// <returns></returns>
        public AssetBundleRequest LoadAssetAsync<T>(string varAssetName)
        {
            AddAssetRefTree(varAssetName);
            return mAssetBundle.LoadAssetAsync<T>(varAssetName);
        }
        /// <summary>
        /// 外部对该AB包的引用;
        /// </summary>
        /// <param name="varAssetName">资源名</param>
        public void AddAssetRefTree(string varAssetName)
        {
            if (null == mRefDic)
            {
                mRefDic = new Dictionary<string, int>();
                mRefDic.Add(varAssetName, 1);
            }
            else if (true == mRefDic.ContainsKey(varAssetName))
            {
                mRefDic[varAssetName] += 1;
            }
            else
            {
                mRefDic.Add(varAssetName, 1);
            }
        }
        #endregion
    }
}
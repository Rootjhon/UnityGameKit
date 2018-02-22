/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: ResourceManagerDefine.cs
			// Describle:
			// Created By:  Jhon
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/
using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace BaseKit
{
    #region [Delegate]
    /// <summary>
    /// 加载回调;
    /// </summary>
    /// <typeparam name="T">资源类型</typeparam>
    /// <param name="varObj">加载得到的资源</param>
    /// <param name="varParam">回调参数</param>
    public delegate void LoadAssetCallback<T>(T varObj, ResourceLoadParam varParam) where T : UnityEngine.Object;
    /// <summary>
    /// 加载回调;
    /// </summary>
    /// <param name="varParam">回调参数</param>
    public delegate void LoadAssetCallback(ResourceLoadParam varParam);
    #endregion

    #region [ResourceMode]
    /// <summary>
    /// 资源加载模式;
    /// </summary>
    public enum ResourceMode
    {
        /// <summary>
        /// Unity 内置Resources加载模式;
        /// </summary>
        Resources,
        /// <summary>
        /// AB包加载模式;
        /// </summary>
        AssetBundle,
#if EditorVersion
        /// <summary>
        /// 模拟AB包模式(不建议发布使用);
        /// </summary>
        AssetBundleSimulation
#endif
    }
    #endregion

    /// <summary>
    /// 资源管理器(单例);
    /// </summary>
    public partial class ResourceManager
    {
        #region [Fields]
        private static ResourceManager mInstance;
        private short mEmptyRunTimes = 0;

        #region [AssetBundleCache]
        private AssetBundleManifest mAssetBundleManifest;
        private Dictionary<string, LoadedAssetBundle> mLoadedBundleDic;
        #endregion

        #region [LoadOperationFields]
        private MonoBehaviour mGameService;
        private List<LoadOperation> AdvancedOperations;
        private List<LoadOperation> WaitingOperations;
        #endregion

        #endregion

        #region [PublicTools]
        /// <summary>
        /// 获取资源管理器;
        /// </summary>
        /// <returns></returns>
        public static ResourceManager GetSingle()
        {
            mInstance = mInstance == null ? new ResourceManager() : mInstance;
            return mInstance;
        }
        /// <summary>
        /// 当前等待队列的操作数量;
        /// </summary>
        /// <returns></returns>
        public int WaittingOperationCount()
        {
            if (null == WaitingOperations) return 0;
            return WaitingOperations.Count;
        }
        /// <summary>
        /// 释放无用的资源(该API会调用GC);
        /// </summary>
        public void UnloadUnusedAssets()
        {
            ReleaseManagedMemory();
        }
        /// <summary>
        /// 返回资源的磁盘目录;
        /// </summary>
        /// <param name="varPath">资源名(路径)</param>
        /// <returns></returns>
        public string ConvertDiskSavaPath(string varPath)
        {
            return Path.Combine(ResConfig.ResFolderPath, varPath);
        }
        /// <summary>
        /// 释放AssetBundle资源包引用;
        /// </summary>
        /// <param name="varBundleName">AssetBundle包名</param>
        /// <param name="varAssetName">资源名</param>
        /// <returns></returns>
        public bool ReleaseBundleRef(string varBundleName, string varAssetName)
        {
            if (true == string.IsNullOrEmpty(varBundleName))
            {
#if LogFlag
                Debug.LogWarning("ResourceManager.cs ReleaseBundleRef varAssetPath IsNullOrEmpty .");
#endif
                return false;
            }
            if (null == mLoadedBundleDic || 0 == mLoadedBundleDic.Count) return false;
            bool tempTruth = false;
            LoadedAssetBundle tempLAB;
            if (true == mLoadedBundleDic.TryGetValue(varBundleName, out tempLAB))
            {
                tempLAB.UnLoad(varAssetName, ref tempTruth);
                if (true == tempTruth) mLoadedBundleDic.Remove(varBundleName);

                string[] tempAllDeps = mAssetBundleManifest.GetAllDependencies(varBundleName);
                for (int i = 0; i < tempAllDeps.Length; i++)
                {
                    if (false == mLoadedBundleDic.ContainsKey(tempAllDeps[i]))
                    {
#if LogFlag
                        Debug.LogError("ResourceManager.cs ReleaseBundleRef Depend is not exit before Main asset. Depend : " + tempAllDeps[i] + "  Main Asset :" + varAssetName);
#endif
                        continue;
                    }
                    mLoadedBundleDic[tempAllDeps[i]].UnLoad(tempAllDeps[i], ref tempTruth);
                    if (true == tempTruth) mLoadedBundleDic.Remove(tempAllDeps[i]);
                }
            }
            else
            {
#if LogFlag
                Debug.LogError("ResourceManager.cs ReleaseBundleRef mLoadedBundleDic Can't Get varBundleName : " + varBundleName);
#endif
                return false;
            }
            return true;
        }

        #region [InitResourceEnv]
        /// <summary>
        /// 初始化加载环境;
        /// </summary>
        /// <param name="varGameService">该脚本应该挂载在程序运行周期中持久存在的物体上（DontDestroyOnLoad）</param>
        public void InitResourceEnv(MonoBehaviour varGameService)
        {
            InitResourceEnv<string>(varGameService, null, string.Empty);
        }
        /// <summary>
        /// 初始化加载环境;
        /// </summary>
        /// <param name="varGameService">该脚本挂载在程序运行周期中持久存在的物体上（DontDestroyOnLoad）</param>
        /// <param name="varAction">初始化完成回调</param>
        public void InitResourceEnv(MonoBehaviour varGameService, Action varAction)
        {
            if (null == varAction)
            {
                InitResourceEnv(varGameService);
                return;
            }
            InitResourceEnv<string>(varGameService, ((string varStr) => { varAction(); }), string.Empty);
        }
        /// <summary>
        /// 初始化加载环境;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="varGameService">该脚本挂载在程序运行周期中持久存在的物体上（DontDestroyOnLoad）</param>
        /// <param name="varAction">初始化完成回调</param>
        /// <param name="varParam">回调参数</param>
        public void InitResourceEnv<T>(MonoBehaviour varGameService, Action<T> varAction, T varParam)
        {
            if (varGameService == null)
            {
#if LogFlag
                Debug.LogError("ResourceManager.cs InitResourceEnv varGameService == null .");
#endif
                return;
            }
            AdvancedOperations = new List<LoadOperation>();
            WaitingOperations = new List<LoadOperation>();
            mLoadedBundleDic = new Dictionary<string, LoadedAssetBundle>();
            mGameService = varGameService;
            mGameService.StartCoroutine(LoadLooper());
            if (null != varAction) { varAction(varParam); }
        }
        #endregion

        #region [Manifest]
        /// <summary>
        /// 初始化AssetBundleManifest;
        /// </summary>
        /// <param name="varManifest"></param>
        public void InitAssetBundleManifest(AssetBundleManifest varManifest)
        {
            mAssetBundleManifest = varManifest;
        }
        /// <summary>
        /// 同步初始化AssetBundleManifest;
        /// </summary>
        /// <typeparam name="T">回调参数类型</typeparam>
        /// <param name="varManifestName">AssetBundleManifest的文件名</param>
        /// <param name="varAction">回调函数</param>
        /// <param name="varParam">回调参数</param>
        public void InitAssetBundleManifest<T>(string varManifestName, Action<T> varAction, T varParam)
        {
            ABLoadManifestOperation tempOperation = new ABLoadManifestOperation();
            tempOperation.CallbackParam = new ResourceLoadParam(varManifestName, "AssetBundleManifest");
            tempOperation.OperationCallback = delegate (AssetBundleManifest varABMani, ResourceLoadParam varRParam)
            {
                if (null == varABMani)
                {
#if LogFlag
                    Debug.LogError("ResourceManager.cs SelfInitOperate LoadManifest Error.");
#endif
                }
                else
                {
                    mAssetBundleManifest = varABMani;
                }
                if (null != varAction) varAction(varParam);
            };
            AdvancedOperations.Add(tempOperation);
        }
        /// <summary>
        /// 同步初始化AssetBundleManifest;
        /// </summary>
        /// <param name="varManifestName">AssetBundleManifest的文件名</param>
        /// <param name="varAction">回调函数</param>
        public void InitAssetBundleManifest(string varManifestName, Action varAction)
        {
            InitAssetBundleManifest(varManifestName, (varStr) => { if (null != varAction) varAction(); }, string.Empty);
        }
        /// <summary>
        /// 异步初始化AssetBundleManifest;
        /// </summary>
        /// <typeparam name="T">回调参数类型</typeparam>
        /// <param name="varManifestName">AssetBundleManifest的文件名</param>
        /// <param name="varAction">回调函数</param>
        /// <param name="varParam">回调参数</param>
        public void InitAssetBundleManifestAsync<T>(string varManifestName, Action<T> varAction, T varParam)
        {
            mLoadedBundleDic = new Dictionary<string, LoadedAssetBundle>();
            ABLoadManifestAsyncOperation tempOperation = new ABLoadManifestAsyncOperation();
            tempOperation.CallbackParam = new ResourceLoadParam(varManifestName, "AssetBundleManifest");
            tempOperation.OperationCallback = delegate (AssetBundleManifest varABMani, ResourceLoadParam varRParam)
            {
                if (null == varABMani)
                {
#if LogFlag
                    Debug.LogError("ResourceManager.cs SelfInitOperate LoadManifest Error.");
#endif
                }
                else
                {
                    mAssetBundleManifest = varABMani;
                }
                if (null != varAction) varAction(varParam);
            };
            AdvancedOperations.Add(tempOperation);
        }
        /// <summary>
        /// 异步初始化AssetBundleManifest;
        /// </summary>
        /// <param name="varManifestName">AssetBundleManifest的文件名</param>
        /// <param name="varAction">回调函数</param>
        public void InitAssetBundleManifestAsync(string varManifestName, Action varAction)
        {
            InitAssetBundleManifestAsync(varManifestName, (varStr) => { if (null != varAction) varAction(); }, string.Empty);
        }
        #endregion

        #region [LoadAsset]
        /// <summary>
        /// 同步加载资源;
        /// </summary>
        /// <typeparam name="TAssetType">资源类型</typeparam>
        /// <typeparam name="TParam">回调参数类型</typeparam>
        /// <param name="varBundleName">AB包名/文件夹路径</param>
        /// <param name="varAssetName">资源名</param>
        /// <param name="varPriority">是否优先加载</param>
        /// <param name="varCallBack">加载完成回调</param>
        /// <param name="varBackParam">回调参数</param>
        /// <returns></returns>
        public LoadOperation LoadAsset<TAssetType, TParam>(string varBundleName, string varAssetName, bool varPriority, LoadAssetCallback<TAssetType> varCallBack, TParam varBackParam) where TAssetType : UnityEngine.Object
        {
            return LoadAssetInternal<TAssetType, TParam>(varBundleName, varAssetName, varPriority, varCallBack, varBackParam);
        }
        /// <summary>
        /// 同步加载资源;
        /// </summary>
        /// <typeparam name="TAssetType">资源类型</typeparam>
        /// <typeparam name="TParam">回调参数类型</typeparam>
        /// <param name="varBundleName">AB包名/文件夹路径</param>
        /// <param name="varAssetName">资源名</param>
        /// <param name="varCallBack">加载完成回调</param>
        /// <param name="varBackParam">回调参数</param>
        /// <returns></returns>
        public LoadOperation LoadAsset<TAssetType, TParam>(string varBundleName, string varAssetName, LoadAssetCallback<TAssetType> varCallBack, TParam varBackParam) where TAssetType : UnityEngine.Object
        {
            return LoadAssetInternal<TAssetType, TParam>(varBundleName, varAssetName, false, varCallBack, varBackParam);
        }
        /// <summary>
        /// 同步加载资源;
        /// </summary>
        /// <typeparam name="TAssetType">资源类型</typeparam>
        /// <param name="varBundleName">AB包名/文件夹路径</param>
        /// <param name="varAssetName">资源名</param>
        /// <param name="varCallBack">加载完成回调</param>
        /// <returns></returns>
        public LoadOperation LoadAsset<TAssetType>(string varBundleName, string varAssetName, LoadAssetCallback<TAssetType> varCallBack) where TAssetType : UnityEngine.Object
        {
            return LoadAssetInternal<TAssetType, string>(varBundleName, varAssetName, false, varCallBack, string.Empty);
        }
        /// <summary>
        /// 同步加载资源;
        /// </summary>
        /// <typeparam name="TAssetType">资源类型</typeparam>
        /// <param name="varBundleName">AB包名/文件夹路径</param>
        /// <param name="varAssetName">资源名</param>
        /// <param name="varPriority">是否优先加载</param>
        /// <param name="varCallBack">加载完成回调</param>
        /// <returns></returns>
        public LoadOperation LoadAsset<TAssetType>(string varBundleName, string varAssetName, bool varPriority, LoadAssetCallback<TAssetType> varCallBack) where TAssetType : UnityEngine.Object
        {
            return LoadAssetInternal<TAssetType, string>(varBundleName, varAssetName, varPriority, varCallBack, string.Empty);
        }
        /// <summary>
        /// 异步加载资源;
        /// </summary>
        /// <typeparam name="TAssetType">资源类型</typeparam>
        /// <typeparam name="TParam">回调参数类型</typeparam>
        /// <param name="varBundleName">AB包名/文件夹路径</param>
        /// <param name="varAssetName">资源名</param>
        /// <param name="varPriority">是否优先加载</param>
        /// <param name="varCallBack">加载完成回调</param>
        /// <param name="varBackParam">回调参数</param>
        /// <returns></returns>
        public LoadOperation LoadAssetAsync<TAssetType, TParam>(string varBundleName, string varAssetName, bool varPriority, LoadAssetCallback<TAssetType> varCallBack, TParam varBackParam) where TAssetType : UnityEngine.Object
        {
            return LoadAssetAsyncInternal<TAssetType, TParam>(varBundleName, varAssetName, varPriority, varCallBack, varBackParam);
        }
        /// <summary>
        /// 异步加载资源;
        /// </summary>
        /// <typeparam name="TAssetType">资源类型</typeparam>
        /// <typeparam name="TParam">回调参数类型</typeparam>
        /// <param name="varBundleName">AB包名/文件夹路径</param>
        /// <param name="varAssetName">资源名</param>
        /// <param name="varCallBack">加载完成回调</param>
        /// <param name="varBackParam">回调参数</param>
        /// <returns></returns>
        public LoadOperation LoadAssetAsync<TAssetType, TParam>(string varBundleName, string varAssetName, LoadAssetCallback<TAssetType> varCallBack, TParam varBackParam) where TAssetType : UnityEngine.Object
        {
            return LoadAssetAsyncInternal<TAssetType, TParam>(varBundleName, varAssetName, false, varCallBack, varBackParam);
        }
        /// <summary>
        /// 异步加载资源;
        /// </summary>
        /// <typeparam name="TAssetType">资源类型</typeparam>
        /// <param name="varBundleName">AB包名/文件夹路径</param>
        /// <param name="varAssetName">资源名</param>
        /// <param name="varCallBack">加载完成回调</param>
        /// <returns></returns>
        public LoadOperation LoadAssetAsync<TAssetType>(string varBundleName, string varAssetName, LoadAssetCallback<TAssetType> varCallBack) where TAssetType : UnityEngine.Object
        {
            return LoadAssetAsyncInternal<TAssetType, string>(varBundleName, varAssetName, false, varCallBack, string.Empty);
        }
        /// <summary>
        /// 异步加载资源;
        /// </summary>
        /// <typeparam name="TAssetType">资源类型</typeparam>
        /// <param name="varBundleName">AB包名/文件夹路径</param>
        /// <param name="varAssetName">资源名</param>
        /// <param name="varPriority">是否优先加载</param>
        /// <param name="varCallBack">加载完成回调</param>
        /// <returns></returns>
        public LoadOperation LoadAssetAsync<TAssetType>(string varBundleName, string varAssetName, bool varPriority, LoadAssetCallback<TAssetType> varCallBack) where TAssetType : UnityEngine.Object
        {
            return LoadAssetAsyncInternal<TAssetType, string>(varBundleName, varAssetName, varPriority, varCallBack, string.Empty);
        }
        #endregion

        #region [LoadLevel]
        /// <summary>
        /// 同步加载(.Unity)场景文件;
        /// </summary>
        /// <param name="varBundleName">AB包名/文件夹路径</param>
        /// <param name="varLevelName">场景资源名</param>
        /// <param name="varLoadSceneMode">该场景的加载模式</param>
        /// <returns></returns>
        public LoadOperation LoadLevel(string varBundleName, string varLevelName, LoadSceneMode varLoadSceneMode)
        {
            return LoadLevelInternal<string>(varBundleName, varLevelName, varLoadSceneMode, null, string.Empty);
        }
        /// <summary>
        /// 同步加载(.Unity)场景文件;
        /// </summary>
        /// <param name="varBundleName">AB包名/文件夹路径</param>
        /// <param name="varLevelName">场景资源名</param>
        /// <param name="varLoadSceneMode">该场景的加载模式</param>
        /// <param name="varCallBack">完成加载的回调</param>
        /// <returns></returns>
        public LoadOperation LoadLevel(string varBundleName, string varLevelName, LoadSceneMode varLoadSceneMode, LoadAssetCallback varCallBack)
        {
            return LoadLevelInternal<string>(varBundleName, varLevelName, varLoadSceneMode, varCallBack, string.Empty);
        }
        /// <summary>
        /// 同步加载(.Unity)场景文件;
        /// </summary>
        /// <typeparam name="TParam">回调参数类型</typeparam>
        /// <param name="varBundleName">AB包名/文件夹路径</param>
        /// <param name="varLevelName">场景资源名</param>
        /// <param name="varLoadSceneMode">该场景的加载模式</param>
        /// <param name="varCallBack">完成加载的回调</param>
        /// <param name="varBackParam">回调参数</param>
        /// <returns></returns>
        public LoadOperation LoadLevel<TParam>(string varBundleName, string varLevelName, LoadSceneMode varLoadSceneMode, LoadAssetCallback varCallBack, TParam varBackParam)
        {
            return LoadLevelInternal<TParam>(varBundleName, varLevelName, varLoadSceneMode, varCallBack, varBackParam);
        }
        /// <summary>
        /// 异步加载(.Unity)场景文件;
        /// </summary>
        /// <param name="varBundleName">AB包名/文件夹路径</param>
        /// <param name="varLevelName">场景资源名</param>
        /// <param name="varLoadSceneMode">该场景的加载模式</param>
        /// <returns></returns>
        public LoadOperation LoadLevelAsync(string varBundleName, string varLevelName, LoadSceneMode varLoadSceneMode)
        {
            return LoadLevelAsyncInternal<string>(varBundleName, varLevelName, varLoadSceneMode, null, string.Empty);
        }
        /// <summary>
        /// 异步加载(.Unity)场景文件;
        /// </summary>
        /// <param name="varBundleName">AB包名/文件夹路径</param>
        /// <param name="varLevelName">场景资源名</param>
        /// <param name="varLoadSceneMode">该场景的加载模式</param>
        /// <param name="varCallBack">完成加载的回调</param>
        /// <returns></returns>
        public LoadOperation LoadLevelAsync(string varBundleName, string varLevelName, LoadSceneMode varLoadSceneMode, LoadAssetCallback varCallBack)
        {
            return LoadLevelAsyncInternal<string>(varBundleName, varLevelName, varLoadSceneMode, varCallBack, string.Empty);
        }
        /// <summary>
        /// 异步加载(.Unity)场景文件;
        /// </summary>
        /// <typeparam name="TParam">回调参数类型</typeparam>
        /// <param name="varBundleName">AB包名/文件夹路径</param>
        /// <param name="varLevelName">场景资源名</param>
        /// <param name="varLoadSceneMode">该场景的加载模式</param>
        /// <param name="varCallBack">完成加载的回调</param>
        /// <param name="varBackParam">回调参数</param>
        /// <returns></returns>
        public LoadOperation LoadLevelAsync<TParam>(string varBundleName, string varLevelName, LoadSceneMode varLoadSceneMode, LoadAssetCallback varCallBack, TParam varBackParam)
        {
            return LoadLevelAsyncInternal<TParam>(varBundleName, varLevelName, varLoadSceneMode, varCallBack, varBackParam);
        }
        #endregion

        #region [AssetBundleHelper]
        /// <summary>
        /// 缓存已加载的AssetBundle;
        /// </summary>
        /// <param name="varAssetBundleName">AB包名</param>
        /// <param name="varBundle">AssetBundle</param>
        /// <param name="varLoadedResult">返回封装的AssetBundle(使用该封装的替换原有AssetBundle的操作)</param>
        /// <returns>缓存操作是否成功</returns>
        public bool CacheLoadedBundle(string varAssetBundleName, AssetBundle varBundle, out LoadedAssetBundle varLoadedResult)
        {
            varLoadedResult = new LoadedAssetBundle(varBundle);
            if (true == string.IsNullOrEmpty(varAssetBundleName) || null == varBundle)
            {
#if LogFlag
                Debug.LogError("ResourceManager.cs CacheLoadedBundle Param illegal .");
#endif
                return false;
            }
            if (true == mLoadedBundleDic.ContainsKey(varAssetBundleName))
            {
#if LogFlag
                Debug.LogWarning("ResourceManager.cs CacheLoadedBundle AssetPath already cache.");
#endif
                return false;
            }
            mLoadedBundleDic.Add(varAssetBundleName, varLoadedResult);
            return true;
        }
        /// <summary>
        /// 获取已加载的AB包;
        /// </summary>
        /// <param name="varAssetBundleName">AB包名</param>
        /// <param name="varLoadedBundle">返回封装的AssetBundle(使用该封装的替换原有AssetBundle的操作)</param>
        /// <returns>是否获取到已加载的AB包</returns>
        public bool GetCacheBundle(string varAssetBundleName, out LoadedAssetBundle varLoadedBundle)
        {
            varLoadedBundle = null;
            if (null == mLoadedBundleDic)
            {
#if LogFlag
                Debug.LogError("ResourceManager.cs GetCacheBundle null == mLoadedBundleDic, you should call ResourceManager->InitResourceEnv() frist .");
#endif
                return false;
            }
            bool tempR = mLoadedBundleDic.TryGetValue(varAssetBundleName, out varLoadedBundle);
            return tempR;
        }
        /// <summary>
        /// 解析所需加载的依赖;
        /// </summary>
        /// <param name="varBundleName">AB包名</param>
        /// <param name="varDependencies">需要加载的依赖</param>
        /// <returns>是否成功解析</returns>
        public bool AnalysisDependenciesReady(string varBundleName, out List<string> varDependencies)
        {
            varDependencies = null;
            if (null == mAssetBundleManifest)
            {
#if LogFlag
                Debug.LogError("ResourceManager.cs LoadDependenciesReady null == AssetBundleManifest,can't Analysis Dependencies.");
#endif
                return false;
            }
            if (null == mLoadedBundleDic)
            {
#if LogFlag
                Debug.LogError("ResourceManager.cs LoadDependenciesReady null == mLoadedBundleDic, you should call ResourceManager->InitResourceEnv() frist .");
#endif
                return false;
            }
            varDependencies = new List<string>();
            string[] tempAllDeps = mAssetBundleManifest.GetAllDependencies(varBundleName);
            for (int i = 0; i < tempAllDeps.Length; i++)
            {
                if (true == mLoadedBundleDic.ContainsKey(tempAllDeps[i]))
                {
                    mLoadedBundleDic[tempAllDeps[i]].AddAssetRefTree(tempAllDeps[i]);
                    continue;
                }
                varDependencies.Add(tempAllDeps[i]);
            }
            return true;
        }
        #endregion

        #region [CustomLoadOperation]
        /// <summary>
        /// 将自定义加载操作压入加载队列;
        /// </summary>
        /// <param name="varLoadOperation">自定义加载操作</param>
        /// <returns></returns>
        public LoadOperation PushCustomOperationInProcess(LoadOperation varLoadOperation)
        {
            return PushCustomOperationInProcess(varLoadOperation,false);
        }
        /// <summary>
        /// 将自定义加载操作压入加载队列;
        /// </summary>
        /// <param name="varLoadOperation">自定义加载操作</param>
        /// <param name="varPriority">是否进入优先队列</param>
        /// <returns></returns>
        public LoadOperation PushCustomOperationInProcess(LoadOperation varLoadOperation,bool varPriority)
        {
            if (varLoadOperation == null)
            {
                Debug.LogError("ResourceManager.cs PushOperationInProcess ,Want In Process Operation is NULL.");
                return varLoadOperation;
            }

            if (varPriority)
            {
                AdvancedOperations.Add(varLoadOperation);
            }
            else
            {
                WaitingOperations.Add(varLoadOperation);
            }
            AutoPrepareEnv();
            return varLoadOperation;
        }
        #endregion

        #endregion

        #region [EditorShow]

#if EditorVersion
        private GameObject mWaitShowObj;
        private void ShowWaitOpertionNum()
        {
            if (mGameService == null || mGameService.gameObject == null) return;

            if (mWaitShowObj == null)
            {
                mWaitShowObj = new GameObject();
                mWaitShowObj.transform.parent = mGameService.transform;
            }
            mWaitShowObj.name = "[WaitLoadNum] " + WaitingOperations.Count;
        }
#endif
        #endregion
    }
}
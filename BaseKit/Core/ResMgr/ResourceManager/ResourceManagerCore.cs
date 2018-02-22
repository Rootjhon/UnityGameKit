/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: ResourceManager.cs
			// Describle:
			// Created By:  Jhon
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace BaseKit
{
    public partial class ResourceManager
    {
        #region [MainProcess]
        private IEnumerator LoadLooper()
        {
            while (true)
            {
                if (AdvancedOperations.Count > 0)
                {
                    mEmptyRunTimes = 0;
                    for (int i = 0; i < AdvancedOperations.Count; i++)
                    {
                        LoadOperation tempOper = AdvancedOperations[0];
#if LogFlag
                        DateTime tempBegin = DateTime.Now;
                        Debug.Log(string.Format("ResourceManager.cs Looper yield return new coroutine [{0}.{1}] .",
                            tempOper.CallbackParam.AssetBundleName, tempOper.CallbackParam.AssetName));
#endif
                        if (false == tempOper.Execute())
                        {
                            yield return mGameService.StartCoroutine(tempOper);
                        }
#if LogFlag
                        Debug.Log(string.Format("ResourceManager.cs Looper success [{0}.{1}] Cost second : {2}ms."
                        , tempOper.CallbackParam.AssetBundleName, tempOper.CallbackParam.AssetName
                        , DateTime.Now.Subtract(tempBegin).TotalMilliseconds));
#endif
                        try
                        {
                            tempOper.Finish();
                        }
                        catch (Exception e)
                        {
#if LogFlag
                            string tempError = string.Format("ResourceManager.cs Looper success,but some exception occuried. LoadOperation : [{0}],Asset : [{1}|{2}] \n Exception : {3} \n StackTrace : {4}",
                                tempOper, tempOper.CallbackParam.AssetBundleName, tempOper.CallbackParam.AssetName, e.Message, e.StackTrace);
                            Debug.LogError(tempError);
#endif
                        }
                        AdvancedOperations.RemoveAt(0);
                        i--;
                    }
                }
                if (WaitingOperations.Count > 0)
                {
                    mEmptyRunTimes = 0;
                    LoadOperation tempOper = WaitingOperations[0];
#if LogFlag
                    DateTime tempBegin = DateTime.Now;
                    Debug.Log(string.Format("ResourceManager.cs Looper yield return new coroutine [{0}.{1}] .",
                        tempOper.CallbackParam.AssetBundleName, tempOper.CallbackParam.AssetName));
#endif
                    if (false == tempOper.Execute())
                    {
                        yield return mGameService.StartCoroutine(tempOper);
                    }
#if LogFlag
                    Debug.Log(string.Format("ResourceManager.cs Looper success [{0}.{1}] Cost second : {2}ms."
                        , tempOper.CallbackParam.AssetBundleName, tempOper.CallbackParam.AssetName
                        , DateTime.Now.Subtract(tempBegin).TotalMilliseconds));
#endif
                    try
                    {
                        tempOper.Finish();
                    }
                    catch (Exception e)
                    {
#if LogFlag
                        string tempError = string.Format("ResourceManager.cs Looper success,but some exception occuried. LoadOperation : [{0}],Asset : [{1}|{2}] \n Exception : {3} \n StackTrace : {4}", 
                            tempOper, tempOper.CallbackParam.AssetBundleName, tempOper.CallbackParam.AssetName,e.Message,e.StackTrace);
                        Debug.LogError(tempError);
#endif
                    }
                    WaitingOperations.RemoveAt(0);
                }

                mEmptyRunTimes++;
#if EditorVersion
                ShowWaitOpertionNum();
#endif
                if (mEmptyRunTimes >= ResConfig.EmptyRunLimit)
                {
                    yield break;
                }
                yield return null;
            }
        }
        #endregion

        #region [LoadMethod]
        private LoadOperation LoadAssetAsyncInternal<TAssetType, TParam>(string varBundleName, string varAssetName, bool varPriority, LoadAssetCallback<TAssetType> varCallBack, TParam varBackParam) where TAssetType : UnityEngine.Object
        {
            LoadOperation tempOperation = AnalysisEnvAssetAsyncOperation<TAssetType>(varBundleName, varAssetName, varCallBack);
            tempOperation.CallbackParam = new ResourceLoadParam<TParam>(varBundleName, varAssetName, varBackParam);

            if (varPriority)
            {
                AdvancedOperations.Add(tempOperation);
            }
            else
            {
                WaitingOperations.Add(tempOperation);
            }
            AutoPrepareEnv();
            return tempOperation;
        }
        private LoadOperation LoadAssetInternal<TAssetType, TParam>(string varBundleName, string varAssetName, bool varPriority, LoadAssetCallback<TAssetType> varCallBack, TParam varBackParam) where TAssetType : UnityEngine.Object
        {
            LoadOperation tempOperation = AnalysisEnvAssetOperation<TAssetType>(varBundleName, varAssetName, varCallBack);
            tempOperation.CallbackParam = new ResourceLoadParam<TParam>(varBundleName, varAssetName, varBackParam);

            if (varPriority)
            {
                AdvancedOperations.Add(tempOperation);
            }
            else
            {
                WaitingOperations.Add(tempOperation);
            }
            AutoPrepareEnv();
            return tempOperation;
        }
        private LoadOperation LoadLevelAsyncInternal<TParam>(string varBundleName, string varLevelName, LoadSceneMode varLoadSceneMode, LoadAssetCallback varCallBack, TParam varBackParam)
        {
            LoadOperation tempOperation = AnalysisEnvLevelAsyncOperation(varBundleName, varLevelName, varLoadSceneMode, varCallBack);
            tempOperation.CallbackParam = new ResourceLoadParam<TParam>(varBundleName, varLevelName, varBackParam);
            //是否需要新建一个队列;
            WaitingOperations.Insert(0, tempOperation);
            AutoPrepareEnv();
            return tempOperation;
        }
        private LoadOperation LoadLevelInternal<TParam>(string varBundleName, string varLevelName, LoadSceneMode varLoadSceneMode, LoadAssetCallback varCallBack, TParam varBackParam)
        {
            LoadOperation tempOperation = AnalysisEnvLevelOperation(varBundleName, varLevelName, varLoadSceneMode, varCallBack);
            tempOperation.CallbackParam = new ResourceLoadParam<TParam>(varBundleName, varLevelName, varBackParam);
            //是否需要新建一个队列;
            WaitingOperations.Insert(0, tempOperation);
            AutoPrepareEnv();
            return tempOperation;
        }
        #endregion

        #region [SelfKit]
        private LoadOperation AnalysisEnvAssetAsyncOperation<TAssetTypet>(string varBundleName, string varAssetName, LoadAssetCallback<TAssetTypet> varCallBack) where TAssetTypet : UnityEngine.Object
        {
            LoadOperation tempResult = null;
            switch (ResConfig.CurLoadModel)
            {
                case ResourceMode.Resources:
                default:
                    {
                        tempResult = new ResourceLAssetAsyncOperation<TAssetTypet>(varCallBack);
                    }
                    break;
                case ResourceMode.AssetBundle:
                    {
                        tempResult = new ABLoadAssetAsyncOperation<TAssetTypet>(varCallBack);
                    }
                    break;
#if EditorVersion
                case ResourceMode.AssetBundleSimulation:
                    {
                        tempResult = new SLABLoadAssetOperation<TAssetTypet>(varCallBack);
                    }
                    break;
#endif
            }
            return tempResult;
        }
        private LoadOperation AnalysisEnvAssetOperation<T>(string varBundleName, string varAssetName, LoadAssetCallback<T> varCallBack) where T : UnityEngine.Object
        {
            LoadOperation tempResult = null;
            switch (ResConfig.CurLoadModel)
            {
                case ResourceMode.Resources:
                default:
                    {
                        tempResult = new ResourceLoadAssetOperation<T>(varCallBack);
                    }
                    break;
                case ResourceMode.AssetBundle:
                    {
                        tempResult = new ABLoadAssetOperation<T>(varCallBack);
                    }
                    break;
#if EditorVersion
                case ResourceMode.AssetBundleSimulation:
                    {
                        tempResult = new SLABLoadAssetOperation<T>(varCallBack);
                    }
                    break;
#endif
            }
            return tempResult;
        }
        private LoadOperation AnalysisEnvLevelAsyncOperation(string varBundleName, string varLevelName, LoadSceneMode varLoadSceneMode, LoadAssetCallback varCallBack)
        {
            LoadOperation tempResult = null;
            switch (ResConfig.CurLoadModel)
            {
                case ResourceMode.Resources:
                default:
                    {
                        tempResult = new ResourceLLevelAsyncOperation(varLoadSceneMode, varCallBack);
                    }
                    break;
                case ResourceMode.AssetBundle:
                    {
                        tempResult = new ABLoadLevelAsyncOperation(varLoadSceneMode, varCallBack);
                    }
                    break;
#if EditorVersion
                case ResourceMode.AssetBundleSimulation:
                    {
                        tempResult = new SLABLoadLevelOperation(varLoadSceneMode, varCallBack);
                    }
                    break;
#endif
            }
            return tempResult;
        }
        private LoadOperation AnalysisEnvLevelOperation(string varBundleName, string varLevelName, LoadSceneMode varLoadSceneMode, LoadAssetCallback varCallBack)
        {
            LoadOperation tempResult = null;
            switch (ResConfig.CurLoadModel)
            {
                case ResourceMode.Resources:
                default:
                    {
                        tempResult = new ResourceLoadLevelOperation(varLoadSceneMode, varCallBack);
                    }
                    break;
                case ResourceMode.AssetBundle:
                    {
                        tempResult = new ABLoadLevelOperation(varLoadSceneMode, varCallBack);
                    }
                    break;
#if EditorVersion
                case ResourceMode.AssetBundleSimulation:
                    {
                        tempResult = new SLABLoadLevelOperation(varLoadSceneMode, varCallBack);
                    }
                    break;
#endif
            }
            return tempResult;
        }
        private void AutoPrepareEnv()
        {
            if (mEmptyRunTimes >= ResConfig.EmptyRunLimit)
            {
                mEmptyRunTimes = 0;
                mGameService.StartCoroutine(LoadLooper());
            }
        }
        private void ReleaseManagedMemory()
        {
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
        #endregion
    }
}
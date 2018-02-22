/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: ResourceLoadParam.cs
			// Describle:
			// Created By:  Jhon
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/

namespace BaseKit
{
    /// <summary>
    /// 加载的资源信息;
    /// </summary>
    public class ResourceLoadParam
    {
        #region [Fields]
        /// <summary>
        /// 资源名;
        /// </summary>
        public string AssetName { private set; get; }
        /// <summary>
        /// AB包名/文件夹路径;
        /// </summary>
        public string AssetBundleName { private set; get; }
        #endregion

        #region [Construct]
        /// <summary>
        /// 加载的资源信息;
        /// </summary>
        /// <param name="varAssetBundleName">AB包名/文件夹路径</param>
        /// <param name="varAssetName">资源名</param>
        public ResourceLoadParam(string varAssetBundleName, string varAssetName)
        {
            AssetBundleName = varAssetBundleName;
            AssetName = varAssetName;
        }
        #endregion
    }
    /// <summary>
    /// 加载的资源信息;
    /// </summary>
    /// <typeparam name="TCallBack">回调参数类型</typeparam>
    public class ResourceLoadParam<TCallBack> : ResourceLoadParam
    {
        #region [Fields]
        /// <summary>
        /// 回调参数;
        /// </summary>
        public TCallBack Param { private set; get; }
        #endregion

        #region [Construct]
        /// <summary>
        /// 加载的资源信息;
        /// </summary>
        /// <param name="varAssetBundleName">AB包名/文件夹路径</param>
        /// <param name="varAssetName">资源名</param>
        /// <param name="varParam">回调参数</param>
        public ResourceLoadParam(string varAssetBundleName, string varAssetName, TCallBack varParam) :
            base(varAssetBundleName, varAssetName)
        { Param = varParam; }
        #endregion
    }
}
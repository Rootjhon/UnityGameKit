/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: ResConfig.cs
			// Describle:
			// Created By:  Jhon_朱俊强
			// Date&Time:  2017/11/27 星期一 11:34:57
            // Modify History: 
            //
//----------------------------------------------------------------*/

namespace BaseKit
{
    /// <summary>
    /// 资源加载管理器的配置;
    /// </summary>
    public static class ResConfig
    {
        #region [Fields]
        private static ResourceMode mLoadModel;
        private static string mResFolderPath;
        private static short mEmptyRunLimit = 1000;
        #endregion

        #region [Property]
        /// <summary>
        /// 当前加载模式;
        /// </summary>
        public static ResourceMode CurLoadModel
        {
            get { return mLoadModel; }
            set { mLoadModel = value; }
        }
        /// <summary>
        /// 资源路径(非Resource模式时需配置);
        /// </summary>
        public static string ResFolderPath
        {
            get { return mResFolderPath; }
            set { mResFolderPath = value; }
        }
        /// <summary>
        /// 资源加载管理器空转上限;
        /// </summary>
        public static short EmptyRunLimit
        {
            get { return mEmptyRunLimit; }
            set { mEmptyRunLimit = value; }
        }
        #endregion
    }
}
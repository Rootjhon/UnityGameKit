/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: LoadOperation.cs
			// Describle:
			// Created By:  Jhon
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/

using System.Collections;
using UnityEngine;

namespace BaseKit
{
    /// <summary>
    /// 加载操作抽象类;
    /// </summary>
    public abstract class LoadOperation : IEnumerator
    {
        #region [Fields]
        /// <summary>
        /// 该加载操作的资源信息;
        /// </summary>
        public ResourceLoadParam CallbackParam { get; set; }
        #endregion

        #region [Construct]
        /// <summary>
        /// ;
        /// </summary>
        ~LoadOperation() { CallbackParam = null; }
        #endregion

        #region [PublicTools]
        /// <summary>
        /// 迭代器当前状态;
        /// </summary>
        public object Current { get { return null; } }
        /// <summary>
        /// 转向下一个状态;
        /// </summary>
        /// <returns></returns>
        public bool MoveNext() { return !IsDone(); }
        /// <summary>
        /// 重置迭代器;
        /// </summary>
        public void Reset() { }
        #endregion

        #region [Abstract]
        /// <summary>
        /// 是否完成加载，True = 完成;
        /// </summary>
        /// <returns></returns>
        public abstract bool IsDone();
        /// <summary>
        /// 该加载操作完成时;
        /// </summary>
        public abstract void Finish();
        #endregion

        #region [Virtual]
        /// <summary>
        /// 获取该加载操作结果的资源;
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public virtual T GetAsset<T>() where T :UnityEngine.Object
        {
            return default(T);
        }
        /// <summary>
        /// 执行加载操作,返回True为立即回调;
        /// </summary>
        /// <returns></returns>
        public virtual bool Execute() { return false; }
        #endregion
    }
}
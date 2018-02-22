/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: ICaraEvent.cs
			// Describle:
			// Created By:  Jhon
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/

namespace BaseKit
{
    /// <summary>
    /// 事件监听约束;
    /// </summary>
    public interface ICareEvent
    {
        /// <summary>
        /// 注册事件;
        /// </summary>
        void RegisterEvent();
        /// <summary>
        /// 反注册事件;
        /// </summary>
        void UnRegisterEvent();
    }
}

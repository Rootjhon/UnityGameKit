/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: Event.cs
			// Describle:
			// Created By:  Jhon
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/

namespace BaseKit
{
    /// <summary>
    /// 事件抽象类;
    /// </summary>
    public abstract class BaseEvent
    {
        /// <summary>
        /// 事件ID;
        /// </summary>
        protected int mEventID;
        /// <summary>
        /// 事件ID;
        /// </summary>
        public int pEventID
        {
            get { return mEventID; }
            set { mEventID = value; }
        }
    }
}
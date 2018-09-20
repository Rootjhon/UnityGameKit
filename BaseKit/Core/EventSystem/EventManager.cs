/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: EventManager.cs
			// Describle:
			// Created By:  Jhon
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/

using System.Collections.Generic;

namespace BaseKit
{
    /// <summary>
    /// 事件回调;
    /// </summary>
    /// <param name="varEvent">触发的事件</param>
    public delegate void EventFuntion(BaseEvent varEvent);
    
    /// <summary>
    /// 事件管理器;
    /// </summary>
    public abstract class EventManager
    {
        List<EventFuntion> mDeleteMsgHandlers;

        /// <summary>
        /// 所有注册的消息列表;
        /// </summary>
        private Dictionary<int, List<EventFuntion>> mMsgHandlers;
        /// <summary>
        /// ;
        /// </summary>
        protected EventManager()
        {
            mMsgHandlers = new Dictionary<int, List<EventFuntion>>();
            mDeleteMsgHandlers = new List<EventFuntion>();
        }

        /// <summary>
        /// 获取所有以注册事件的个数;
        /// </summary>
        /// <returns></returns>
        public int GetRegisterEventCount()
        {
            return mMsgHandlers.Count;
        }

        /// <summary>
        /// 获取某一事件的监听者个数;
        /// </summary>
        /// <param name="varMsgID"></param>
        /// <returns></returns>
        public int GetRegisterEventCountById(int varMsgID)
        {
            if (mMsgHandlers == null || mMsgHandlers.Count == 0)
            {
                return -1;
            }
            List<EventFuntion> tmpFuns = null;
            if (mMsgHandlers.TryGetValue(varMsgID, out tmpFuns))
            {
                return tmpFuns.Count;
            }
            return 0;
        }

        /// <summary>
        /// 事件广播;
        /// </summary>
        /// <param name="varMsgID"></param>
        /// <param name="varEvent"></param>
        protected void NotifyEvent(int varMsgID, BaseEvent varEvent)
        {
            List<EventFuntion> tmpFuncs;
            if (mMsgHandlers.Count == 0 || mMsgHandlers.TryGetValue(varMsgID, out tmpFuncs) == false)
                return;
            if (tmpFuncs == null)
                return;

            for (int i = 0; i < mDeleteMsgHandlers.Count; i++)
            {
                EventFuntion temRem = mDeleteMsgHandlers[i];
                if (tmpFuncs.Contains(temRem) == true)
                {
                    tmpFuncs.Remove(temRem);
                    mDeleteMsgHandlers.RemoveAt(i);
                    i--;
                }
            }
            if (tmpFuncs.Count > 0)
            {
                for (int i = 0; i < tmpFuncs.Count; i++)
                {
                    EventFuntion tmpFunc = tmpFuncs[i];
                    if (tmpFunc == null || tmpFunc.Target == null)
                    {
                        mDeleteMsgHandlers.Add(tmpFunc);
                    }
                    else
                    {
                        tmpFunc(varEvent);
                    }
                }
            }
        }

        /// <summary>
        /// 注册消息句柄;
        /// </summary>
        /// <param name="varMsgID"></param>
        /// <param name="varFunc"></param>
        protected void RegisterMsgHandler(int varMsgID, EventFuntion varFunc)
        {
            if (varFunc == null)
                return;

            List<EventFuntion> tmpFuns;
            if (mMsgHandlers.Count == 0 || mMsgHandlers.TryGetValue(varMsgID, out tmpFuns) == false)
            {
                tmpFuns = new List<EventFuntion>();
                mMsgHandlers.Add(varMsgID, tmpFuns);
            }
            else
            {
                if (tmpFuns != null)
                {
                    for (int i = 0; i < mDeleteMsgHandlers.Count; i++)
                    {
                        EventFuntion temRem = mDeleteMsgHandlers[i];
                        if (tmpFuns.Contains(temRem) == true)
                        {
                            tmpFuns.Remove(temRem);
                            mDeleteMsgHandlers.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            for (int i = 0; i < tmpFuns.Count; i++)
            {
                EventFuntion func = tmpFuns[i];
                if (func == varFunc)
                {
                    return;
                }
            }
            tmpFuns.Add(varFunc);
        }

        /// <summary>
        /// 移除已经注册的消息句柄;
        /// </summary>
        /// <param name="varMsgID"></param>
        /// <param name="varFunc"></param>
        protected void UnRegisterMsgHandler(int varMsgID, EventFuntion varFunc)
        {
            if (varFunc == null || mMsgHandlers.Count == 0)
                return;
            List<EventFuntion> tmpFuns;
            if (mMsgHandlers.TryGetValue(varMsgID, out tmpFuns) == false)
                return;

            for (int i = tmpFuns.Count - 1; i >= 0; --i)
            {
                EventFuntion tmpFun = tmpFuns[i];
                if (tmpFun == null || tmpFun.Target == null)
                {
                    mDeleteMsgHandlers.Add(tmpFun);
                    continue;
                }
                if (varFunc != null && tmpFun.Target == varFunc.Target)
                {
                    mDeleteMsgHandlers.Add(tmpFun);
                    return;
                }
            }
        }

        /// <summary>
        /// 移除所有已注册的消息;
        /// </summary>
        protected void UnRegisterAllMsgHandlers()
        {
            if (mMsgHandlers != null)
            {
                mMsgHandlers.Clear();
            }
        }
    }
}

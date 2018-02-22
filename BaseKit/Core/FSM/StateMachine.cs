/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: StateMachine.cs
			// Describle:
			// Created By:  Jhon
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/
using UnityEngine;
using System.Collections.Generic;

namespace BaseKit
{
    /// <summary>
    /// 简单状态机管理器;
    /// </summary>
    public class StateMachine
    {
        #region [Fields]
        /// <summary>
        /// 状态集合,K=状态名,V=状态;
        /// </summary>
        protected Dictionary<string, State> mStateDic;
        /// <summary>
        /// 上一个状态;
        /// </summary>
        public State mLastState;
        /// <summary>
        /// 当前状态;
        /// </summary>
        protected State mCurrentState;
        /// <summary>
        /// 下一个状态;
        /// </summary>
        protected State mNextState;
        #endregion

        #region [ConstructedFunction]
        /// <summary>
        /// 简单状态机管理器;
        /// </summary>
        public StateMachine()
        {
            mCurrentState = null;
            mNextState = null;
        }
        #endregion

        #region [BusinessLogic]
        /// <summary>
        /// 由MonoBehaviour.Update调起;
        /// </summary>
        public virtual void Update()
        {
            PromoteStateCodition();
        }
        /// <summary>
        /// 驱动状态的运行;
        /// </summary>
        private void PromoteStateCodition()
        {
            if (null != mCurrentState)
            {
                mCurrentState.Update();
            }
            if (null != mNextState)
            {
                if (null != mCurrentState)
                {
                    mCurrentState.OnEnd();
                }
                mCurrentState = mNextState;
                mNextState = null;
                mCurrentState.OnBegin();
            }
        }
        #endregion

        #region [PublicTools]
        /// <summary>
        /// 向状态机中注册状态;
        /// </summary>
        /// <param name="varState"></param>
        /// <returns></returns>
        public bool RegisterState(State varState)
        {
            if (null == varState)
            {
                return false;
            }
            if (null == mStateDic)
            {
                mStateDic = new Dictionary<string, State>();
            }
            string tempName = varState.pStateName;
            if (true == mStateDic.ContainsKey(tempName))
            {
                return false;
            }
            varState.pStateMachine = this;
            mStateDic.Add(tempName, varState);
            return true;
        }
        /// <summary>
        /// 解注册状态;
        /// </summary>
        /// <param name="varStateName"></param>
        /// <returns></returns>
        public bool UnRegisterState(string varStateName)
        {
            if (null == mStateDic) return false;
            State tempState = null;
            if (false == mStateDic.TryGetValue(varStateName, out tempState))
            {
                return false;
            }
            tempState.pStateMachine = null;
            mStateDic.Remove(varStateName);
            return true;
        }

        /// <summary>
        /// 设置下一帧执行的状态;
        /// </summary>
        /// <param name="varStateName"></param>
        /// <returns></returns>
        public virtual bool SetNextState(string varStateName)
        {
            State tempState = GetStateByName(varStateName);
            if (null == tempState) return false;
            mLastState = mCurrentState;
            if (tempState == mNextState)
            {
                return false;
            }
            mNextState = tempState;
            return true;
        }

        /// <summary>
        /// 通过名称找到相应的状态;
        /// </summary>
        /// <param name="varStateName"></param>
        /// <returns></returns>
        public State GetStateByName(string varStateName)
        {
            if (null == mStateDic) return null;
            State state = null;
            if (false == mStateDic.TryGetValue(varStateName, out state))
            {
#if LogFlag
                Debug.LogError("TryGet State but not register or something error , StateName = " + varStateName);
#endif
            }
            return state;
        }
        /// <summary>
        /// 获取下一帧需要执行的状态;
        /// </summary>
        /// <returns></returns>
        public State GetNextState()
        {
            return mNextState;
        }
        /// <summary>
        /// 获取当前的状态;
        /// </summary>
        /// <returns></returns>
        public State GetCurrentState()
        {
            return mCurrentState;
        }
        /// <summary>
        /// 获取上一个执行的状态;
        /// </summary>
        /// <returns></returns>
        public State GetLastState()
        {
            return mLastState;
        }
#endregion
    }
}

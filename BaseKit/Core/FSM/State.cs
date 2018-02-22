/*----------------------------------------------------------------
            // Copyright © 2017 Jhon
            // 
            // FileName: State.cs
			// Describle:
			// Created By:  Jhon
			// Date&Time:  
            // Modify History: 
            //
//----------------------------------------------------------------*/

namespace BaseKit
{
    /// <summary>
    /// 状态抽象类;
    /// </summary>
    public abstract class State
    {
        #region [Member]
        /// <summary>
        /// 该状态的名称;
        /// </summary>
        public string pStateName { get; set; }
        /// <summary>
        /// 所属的状态机实例;
        /// </summary>
        public StateMachine pStateMachine { get; set; }
        #endregion

        #region [Construct]
        /// <summary>
        /// 状态抽象类;
        /// </summary>
        /// <param name="varStateName">状态名</param>
        public State(string varStateName) { pStateName = varStateName; OnCreated(); }
        /// <summary>
        /// 状态抽象类;
        /// </summary>
        public State() { pStateName = string.Empty; OnCreated(); }
        #endregion

        #region [Virtual]
        /// <summary>
        /// 该状态被创建时;
        /// </summary>
        public virtual void OnCreated() { }
        /// <summary>
        /// 该状态开启时;
        /// </summary>
        public virtual void OnBegin() { }
        /// <summary>
        /// 该状态的Update;
        /// </summary>
        public virtual void Update() { }
        /// <summary>
        /// 该状态结束时;
        /// </summary>
        public virtual void OnEnd() { }
        #endregion
    }
}

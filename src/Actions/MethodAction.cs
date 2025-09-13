using System;
using HutongGames.PlayMaker;

namespace Silksong.FsmUtil.Actions
{
    /// <summary>
    ///     FsmStateAction that invokes methods.
    /// </summary>
    public class MethodAction : FsmStateAction
    {
        /// <summary>
        ///     The method to invoke.
        /// </summary>
        public Action<MethodAction>? Method;

        /// <summary>
        ///     Resets the action.
        /// </summary>
        public override void Reset()
        {
            Method = null;
            base.Reset();
        }

        /// <summary>
        ///     Called when the action is being processed.
        /// </summary>
        public override void OnEnter()
        {
            Method?.Invoke(this);
            Finish();
        }
    }
}

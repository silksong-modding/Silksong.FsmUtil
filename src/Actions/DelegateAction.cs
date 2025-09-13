using System;
using HutongGames.PlayMaker;

namespace Silksong.FsmUtil.Actions
{
    /// <summary>
    ///     FsmStateAction that invokes methods with the  argument.
    ///     You will likely use this with a `HutongGames.PlayMaker.NamedVariable` as the generic argument
    /// </summary>
    public class DelegateAction<TArg> : FsmStateAction
    {
        /// <summary>
        ///     The method to invoke.
        /// </summary>
        public Action<TArg>? Method;

        /// <summary>
        ///     The method to invoke.
        /// </summary>
        public TArg? Arg;

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
            if (Method != null && Arg != null)
            {
                Method.Invoke(Arg);
            }

            if ((!(Arg is Action action)) || (action != Finish))
            {
                Finish();
            }
        }
    }
}
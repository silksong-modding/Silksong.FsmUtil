using HutongGames.PlayMaker;
using System;

namespace Silksong.FsmUtil.Actions
{
    /// <summary>
    ///     An action that executes a single zero-argument function, then exits.
    /// </summary>
    public class LambdaAction : FsmStateAction
    {
        /// <summary>
        ///     The method to invoke.
        /// </summary>
        public Action? Method;

        /// <summary>
        ///     If true, execute the function repeatedly on every update frame.
        /// </summary>
        public bool EveryFrame = false;

        /// <summary>
        ///     Resets the action.
        /// </summary>
        public override void Reset()
        {
            Method = null;
            EveryFrame = false;
            base.Reset();
        }

        /// <summary>
        ///     Called when the action is being processed.
        /// </summary>
        public override void OnEnter()
        {
            if (Method != null)
            {
                Method.Invoke();
            }

            if (!EveryFrame)
            {
                Finish();
            }
        }

        /// <summary>
        ///     Called every update frame.
        /// </summary>
        public override void OnUpdate()
        {
            if (EveryFrame)
            {
                if (Method != null)
                {
                    Method.Invoke();
                }
            }
        }
    }
}

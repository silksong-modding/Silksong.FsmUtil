using HutongGames.PlayMaker;
using System;

namespace Silksong.FsmUtil.Actions
{
    /// <summary>
    ///     An action that executes a single zero-argument function, then exits.
    /// </summary>
    /// <param name="action">The function to invoke.</param>
    /// <param name="everyFrame">If true, execute the function repeatedly on every update frame.</param>
    public class LambdaAction(Action action, bool everyFrame = false) : FsmStateAction
    {
        /// <inheritdoc/>
        public override void OnEnter()
        {
            action?.Invoke();
            if (!everyFrame) Finish();
        }

        /// <inheritdoc/>
        public override void OnUpdate()
        {
            if (everyFrame) action?.Invoke();
        }
    }
}

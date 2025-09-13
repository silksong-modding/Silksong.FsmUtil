using System;
using Silksong.FsmUtil.Actions;
using HutongGames.PlayMaker;
using JetBrains.Annotations;
using UnityEngine;

namespace Silksong.FsmUtil;

/// <summary>
///     Utils specifically for PlayMakerFSMs.
/// </summary>
public static class FsmUtil
{
    #region Get a FSM

    /// <summary>
    ///     Locates a PlayMakerFSM by name and preprocesses it.
    /// </summary>
    /// <param name="go">The GameObject to search on</param>
    /// <param name="fsmName">The name of the FSM</param>
    /// <returns>The found FSM, null if not found</returns>
    [PublicAPI]
    public static PlayMakerFSM? GetFsmPreprocessed(this GameObject go, string fsmName)
    {
        foreach (PlayMakerFSM fsm in go.GetComponents<PlayMakerFSM>())
        {
            if (fsm.FsmName == fsmName)
            {
                fsm.Preprocess();
                return fsm;
            }
        }
        return null;
    }

    #endregion
    
    #region Get

    private static TVal? GetItemFromArray<TVal>(TVal[] origArray, Func<TVal, bool> isItemCheck) where TVal : class
    {
        foreach (TVal item in origArray)
        {
            if (isItemCheck(item))
            {
                return item;
            }
        }
        return null;
    }
    
    private static TVal[] GetItemsFromArray<TVal>(TVal[] origArray, Func<TVal, bool> isItemCheck) where TVal : class
    {
        int foundItems = 0;
        foreach (TVal item in origArray)
        {
            if (isItemCheck(item))
            {
                foundItems++;
            }
        }
        if (foundItems == origArray.Length)
        {
            return origArray;
        }
        if (foundItems == 0)
        {
            return [];
        }
        TVal[] retArray = new TVal[foundItems];
        int foundProgress = 0;
        foreach (TVal item in origArray)
        {
            if (isItemCheck(item))
            {
                retArray[foundProgress] = item;
                foundProgress++;
            }
        }
        return retArray;
    }
    
    /// <summary>
    ///     Gets a state in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state</param>
    /// <returns>The found state, null if none are found</returns>
    [PublicAPI]
    public static FsmState? GetState(this PlayMakerFSM fsm, string stateName) => GetItemFromArray(fsm.FsmStates, x => x.Name == stateName);

    /// <summary>
    ///     Gets a transition in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the from state</param>
    /// <param name="eventName">The name of the event</param>
    /// <returns>The found transition, null if none are found</returns>
    [PublicAPI]
    public static FsmTransition? GetTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.GetState(stateName)!.GetTransition(eventName);

    /// <inheritdoc cref="GetTransition(PlayMakerFSM, string, string)"/>
    /// <param name="state">The state</param>
    /// <param name="eventName">The name of the event</param>
    [PublicAPI]
    public static FsmTransition? GetTransition(this FsmState state, string eventName) => GetItemFromArray(state.Transitions, x => x.EventName == eventName);

    /// <summary>
    ///     Gets an action in a PlayMakerFSM.
    /// </summary>
    /// <typeparam name="TAction">The type of the action that is wanted</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state</param>
    /// <param name="index">The index of the action</param>
    /// <returns>The action, null if it can't be found</returns>
    [PublicAPI]
    public static TAction? GetAction<TAction>(this PlayMakerFSM fsm, string stateName, int index) where TAction : FsmStateAction => fsm.GetState(stateName)!.GetAction<TAction>(index);

    /// <inheritdoc cref="GetAction{TAction}(PlayMakerFSM, string, int)"/>
    /// <param name="state">The state</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static TAction? GetAction<TAction>(this FsmState state, int index) where TAction : FsmStateAction => state.Actions[index] as TAction;

    /// <summary>
    ///     Gets an action in a PlayMakerFSM.
    /// </summary>
    /// <typeparam name="TAction">The type of the action that is wanted</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state</param>
    /// <returns>An array of actions</returns>
    [PublicAPI]
    public static TAction[] GetActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction => fsm.GetState(stateName)!.GetActionsOfType<TAction>();

    /// <inheritdoc cref="GetActionsOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The state</param>
    [PublicAPI]
    public static TAction[] GetActionsOfType<TAction>(this FsmState state) where TAction : FsmStateAction => (GetItemsFromArray<FsmStateAction>(state.Actions, x => x is TAction) as TAction[])!;

    /// <summary>
    ///     Gets first action of a given type in an FsmState.  
    /// </summary>
    /// <typeparam name="TAction">The type of actions to remove</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to get the actions from</param>
    [PublicAPI]
    public static TAction? GetFirstActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction => fsm.GetState(stateName)!.GetFirstActionOfType<TAction>();

    /// <inheritdoc cref="GetFirstActionOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static TAction? GetFirstActionOfType<TAction>(this FsmState state) where TAction : FsmStateAction
    {
        int firstActionIndex = -1;
        for (int i = 0; i < state.Actions.Length; i++)
        {
            if (state.Actions[i] is TAction)
            {
                firstActionIndex = i;
                break;
            }
        }

        if (firstActionIndex == -1)
            return null;
        return state.GetAction<TAction>(firstActionIndex);
    }

    /// <summary>
    ///     Gets last action of a given type in an FsmState.  
    /// </summary>
    /// <typeparam name="TAction">The type of actions to remove</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to get the actions from</param>
    [PublicAPI]
    public static TAction? GetLastActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) where TAction : FsmStateAction => fsm.GetState(stateName)!.GetLastActionOfType<TAction>();

    /// <inheritdoc cref="GetLastActionOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static TAction? GetLastActionOfType<TAction>(this FsmState state) where TAction : FsmStateAction
    {
        int lastActionIndex = -1;
        for (int i = state.Actions.Length - 1; i >= 0; i--)
        {
            if (state.Actions[i] is TAction)
            {
                lastActionIndex = i;
                break;
            }
        }

        if (lastActionIndex == -1)
            return null;
        return state.GetAction<TAction>(lastActionIndex);
    }

    #endregion Get

    #region Add

    private static TVal[] AddItemToArray<TVal>(TVal[] origArray, TVal value)
    {
        TVal[] newArray = new TVal[origArray.Length + 1];
        origArray.CopyTo(newArray, 0);
        newArray[origArray.Length] = value;
        return newArray;
    }
    
    /// <summary>
    ///     Adds a state in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state</param>
    /// <returns>The created state</returns>
    [PublicAPI]
    public static FsmState AddState(this PlayMakerFSM fsm, string stateName) => fsm.AddState(new FsmState(fsm.Fsm) { Name = stateName });

    /// <inheritdoc cref="AddState(PlayMakerFSM, string)"/>
    /// <param name="fsm">The fsm</param>
    /// <param name="state">The state</param>
    [PublicAPI]
    public static FsmState AddState(this PlayMakerFSM fsm, FsmState state)
    {
        FsmState[] origStates = fsm.FsmStates;
        FsmState[] states = AddItemToArray(origStates, state);
        fsm.Fsm.States = states;
        // TODO: CHECK: is this necessary? afaik it saves data for each state, which might be needed for copying/applying state changes for other things
        fsm.Fsm.SaveActions();
        return states[origStates.Length];
    }

    /// <summary>
    ///     Copies a state in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="fromState">The name of the state to copy</param>
    /// <param name="toState">The name of the new state</param>
    /// <returns>The new state</returns>
    [PublicAPI]
    public static FsmState CopyState(this PlayMakerFSM fsm, string fromState, string toState)
    {
        FsmState? from = fsm.GetState(fromState);
        // save the actions before we create a new state from this, as the copy constructor will create the new actions from the saved action data from the state we put in, and that is only updated if we call .SaveActions()
        from?.SaveActions();
        FsmState copy = new FsmState(from)
        {
            Name = toState
        };
        foreach (FsmTransition transition in copy.Transitions)
        {
            // This is because playmaker is bad, it has to be done extra
            transition.ToFsmState = fsm.GetState(transition.ToState);
        }
        fsm.AddState(copy);
        return copy;
    }

    /// <summary>
    ///     Adds a transition in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state from which the transition starts</param>
    /// <param name="eventName">The name of transition event</param>
    /// <param name="toState">The name of the new state</param>
    /// <returns>The event of the transition</returns>
    [PublicAPI]
    public static FsmEvent AddTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.GetState(stateName)!.AddTransition(eventName, toState);

    /// <inheritdoc cref="AddTransition(FsmState, string, string)"/>
    [PublicAPI]
    public static FsmEvent AddTransition(this FsmState state, string eventName, string toState)
    {
        var ret = FsmEvent.GetFsmEvent(eventName);
        FsmTransition[] transitions = AddItemToArray(state.Transitions, new FsmTransition
        {
            ToState = toState,
            ToFsmState = state.Fsm.GetState(toState),
            FsmEvent = ret
        });
        state.Transitions = transitions;
        return ret;
    }

    /// <summary>
    ///     Adds a global transition in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="globalEventName">The name of transition event</param>
    /// <param name="toState">The name of the new state</param>
    /// <returns>The event of the transition</returns>
    [PublicAPI]
    public static FsmEvent AddGlobalTransition(this PlayMakerFSM fsm, string globalEventName, string toState)
    {
        var ret = new FsmEvent(globalEventName) { IsGlobal = true };
        FsmTransition[] transitions = AddItemToArray(fsm.FsmGlobalTransitions, new FsmTransition
        {
            ToState = toState,
            ToFsmState = fsm.GetState(toState),
            FsmEvent = ret
        });
        fsm.Fsm.GlobalTransitions = transitions;
        return ret;
    }

    /// <summary>
    ///     Adds an action in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the action is added</param>
    /// <param name="action">The action</param>
    [PublicAPI]
    public static void AddAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action) => fsm.GetState(stateName)!.AddAction(action);

    /// <inheritdoc cref="AddAction(PlayMakerFSM, string, FsmStateAction)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="action">The action</param>
    [PublicAPI]
    public static void AddAction(this FsmState state, FsmStateAction action)
    {
        FsmStateAction[] actions = AddItemToArray(state.Actions, action);
        state.Actions = actions;
        action.Init(state);
    }

    /// <summary>
    ///     Adds a method in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the method is added</param>
    /// <param name="method">The method that will be invoked</param>
    [PublicAPI]
    public static void AddMethod(this PlayMakerFSM fsm, string stateName, Action<MethodAction> method) => fsm.GetState(stateName)!.AddMethod(method);

    /// <inheritdoc cref="AddMethod(PlayMakerFSM, string, Action{MethodAction})"/>
    /// <param name="state">The fsm state</param>
    /// <param name="method">The method that will be invoked</param>
    [PublicAPI]
    public static void AddMethod(this FsmState state, Action<MethodAction> method) => state.AddAction(new MethodAction { Method = method });

    /// <summary>
    ///     Adds a method with a parameter in a PlayMakerFSM.
    /// </summary>
    /// <typeparam name="TArg">The type of the parameter of the function</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the method is added</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="arg">The argument for the method</param>
    [PublicAPI]
    public static void AddMethod<TArg>(this PlayMakerFSM fsm, string stateName, Action<TArg?> method, TArg? arg) => fsm.GetState(stateName)!.AddMethod(method, arg);

    /// <inheritdoc cref="AddMethod{TArg}(PlayMakerFSM, string, Action{TArg}, TArg)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="arg">The argument for the method</param>
    [PublicAPI]
    public static void AddMethod<TArg>(this FsmState state, Action<TArg?> method, TArg? arg) => state.AddAction(new FunctionAction<TArg> { Method = method, Arg = arg });

    #endregion Add

    #region Insert

    private static TVal[] InsertItemIntoArray<TVal>(TVal[] origArray, TVal value, int index)
    {
        int origArrayCount = origArray.Length;
        if (index < 0 || index > (origArrayCount + 1))
        {
            throw new ArgumentOutOfRangeException($"Index {index} was out of range for array with length {origArrayCount}!");
        }
        TVal[] actions = new TVal[origArrayCount + 1];
        int i;
        for (i = 0; i < index; i++)
        {
            actions[i] = origArray[i];
        }
        actions[index] = value;
        for (i = index; i < origArrayCount; i++)
        {
            actions[i + 1] = origArray[i];
        }
        return actions;
    }
    
    /// <summary>
    ///     Inserts an action in a PlayMakerFSM.  
    ///     Trying to insert an action out of bounds will cause a `ArgumentOutOfRangeException`.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the action is added</param>
    /// <param name="action">The action</param>
    /// <param name="index">The index to place the action in</param>
    /// <returns>bool that indicates whether the insertion was successful</returns>
    [PublicAPI]
    public static void InsertAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index) => fsm.GetState(stateName)!.InsertAction(action, index);

    /// <inheritdoc cref="InsertAction(PlayMakerFSM, string, FsmStateAction, int)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="action">The action</param>
    /// <param name="index">The index to place the action in</param>
    [PublicAPI]
    public static void InsertAction(this FsmState state, FsmStateAction action, int index)
    {
        FsmStateAction[] actions = InsertItemIntoArray(state.Actions, action, index);
        state.Actions = actions;
        action.Init(state);
    }

    /// <summary>
    ///     Inserts a method in a PlayMakerFSM.
    ///     Trying to insert a method out of bounds will cause a `ArgumentOutOfRangeException`.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the method is added</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="index">The index to place the action in</param>
    /// <returns>bool that indicates whether the insertion was successful</returns>
    [PublicAPI]
    public static void InsertMethod(this PlayMakerFSM fsm, string stateName, Action<MethodAction> method, int index) => fsm.GetState(stateName)!.InsertMethod(method, index);

    /// <inheritdoc cref="InsertMethod(PlayMakerFSM, string, Action{MethodAction}, int)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="index">The index to place the action in</param>
    [PublicAPI]
    public static void InsertMethod(this FsmState state, Action<MethodAction> method, int index) => state.InsertAction(new MethodAction { Method = method }, index);

    /// <summary>
    ///     Inserts a method with a parameter in a PlayMakerFSM.
    ///     Trying to insert a method out of bounds will cause a `ArgumentOutOfRangeException`.
    /// </summary>
    /// <typeparam name="TArg">The type of the parameter of the function</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the method is added</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="arg">The argument for the method</param>
    /// <param name="index">The index to place the action in</param>
    /// <returns>bool that indicates whether the insertion was successful</returns>
    [PublicAPI]
    public static void InsertMethod<TArg>(this PlayMakerFSM fsm, string stateName, Action<TArg?> method, TArg? arg, int index) => fsm.GetState(stateName)!.InsertMethod(method, arg, index);

    /// <inheritdoc cref="InsertMethod{TArg}(PlayMakerFSM, string, Action{TArg}, TArg, int)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="method">The method that will be invoked</param>
    /// <param name="arg">The argument for the method</param>
    /// <param name="index">The index to place the action in</param>
    [PublicAPI]
    public static void InsertMethod<TArg>(this FsmState state, Action<TArg?> method, TArg? arg, int index) => state.InsertAction(new FunctionAction<TArg> { Method = method, Arg = arg }, index);

    #endregion Insert

    #region Replace
    
    /// <summary>
    ///     Replaces an action in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the action is replaced</param>
    /// <param name="action">The action</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static void ReplaceAction(this PlayMakerFSM fsm, string stateName, FsmStateAction action, int index) => fsm.GetState(stateName)!.ReplaceAction(action, index);

    /// <summary>
    ///     Replaces an action in a PlayMakerFSM state.
    /// </summary>
    /// <param name="state">The state in which the action is replaced</param>
    /// <param name="action">The action</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static void ReplaceAction(this FsmState state, FsmStateAction action, int index)
    {
        state.Actions[index] = action;
        action.Init(state);
    }

    /// <summary>
    ///     Replaces all actions in a PlayMakerFSM state.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state in which the actions are to be replaced</param>
    /// <param name="actions">The new actions of the state</param>
    [PublicAPI]
    public static void ReplaceAllActions(this PlayMakerFSM fsm, string stateName, params FsmStateAction[] actions) => fsm.GetState(stateName)!.ReplaceAllActions(actions);

    /// <summary>
    ///     Replaces all actions in a PlayMakerFSM state.
    /// </summary>
    /// <param name="state">The fsm state</param>
    /// <param name="actions">The action</param>
    [PublicAPI]
    public static void ReplaceAllActions(this FsmState state, params FsmStateAction[] actions)
    {
        state.Actions = actions;
        foreach (FsmStateAction action in actions)
        {
            action.Init(state);
        }
    }

    #endregion Replace

    #region Change

    /// <summary>
    ///     Changes a transition endpoint in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state from which the transition starts</param>
    /// <param name="eventName">The event of the transition</param>
    /// <param name="toState">The new endpoint of the transition</param>
    /// <returns>bool that indicates whether the change was successful</returns>
    [PublicAPI]
    public static bool ChangeTransition(this PlayMakerFSM fsm, string stateName, string eventName, string toState) => fsm.GetState(stateName)!.ChangeTransition(eventName, toState);

    /// <inheritdoc cref="ChangeTransition(PlayMakerFSM, string, string, string)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="eventName">The event of the transition</param>
    /// <param name="toState">The new endpoint of the transition</param>
    [PublicAPI]
    public static bool ChangeTransition(this FsmState state, string eventName, string toState)
    {
        var transition = state.GetTransition(eventName);
        if (transition == null)
        {
            return false;
        }
        transition.ToState = toState;
        transition.ToFsmState = state.Fsm.GetState(toState);
        return true;
    }

    #endregion Change

    #region Remove

    private static TVal[] RemoveItemsFromArray<TVal>(TVal[] origArray, Func<TVal, bool> shouldBeRemovedCallback)
    {
        int amountOfRemoved = 0;
        foreach (TVal tmpValue in origArray)
        {
            if (shouldBeRemovedCallback(tmpValue))
            {
                amountOfRemoved++;
            }
        }
        if (amountOfRemoved == 0)
        {
            return origArray;
        }
        TVal[] newArray = new TVal[origArray.Length - amountOfRemoved];
        for (int i = origArray.Length - 1; i >= 0; i--)
        {
            TVal tmpValue = origArray[i];
            if (shouldBeRemovedCallback(tmpValue))
            {
                amountOfRemoved--;
                continue;
            }
            newArray[i - amountOfRemoved] = tmpValue;
        }
        return newArray;
    }

    /// <summary>
    ///     Removes a state in a PlayMakerFSM.  
    ///     Trying to remove a state that doesn't exist will result in the states not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to remove</param>
    [PublicAPI]
    public static void RemoveState(this PlayMakerFSM fsm, string stateName) => fsm.Fsm.States = RemoveItemsFromArray(fsm.FsmStates, x => x.Name == stateName);

    /// <summary>
    ///     Removes a transition in a PlayMakerFSM.  
    ///     Trying to remove a transition that doesn't exist will result in the transitions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state from which the transition starts</param>
    /// <param name="eventName">The event of the transition</param>
    [PublicAPI]
    public static void RemoveTransition(this PlayMakerFSM fsm, string stateName, string eventName) => fsm.GetState(stateName)!.RemoveTransition(eventName);

    /// <inheritdoc cref="RemoveTransition(PlayMakerFSM, string, string)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="eventName">The event of the transition</param>
    [PublicAPI]
    public static void RemoveTransition(this FsmState state, string eventName) => state.Transitions = RemoveItemsFromArray(state.Transitions, x => x.EventName == eventName);

    /// <summary>
    ///     Removes all transitions to a specified transition in a PlayMakerFSM.  
    ///     Trying to remove a transition that doesn't exist will result in the transitions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="toState">The target of the transition</param>
    [PublicAPI]
    public static void RemoveTransitionsTo(this PlayMakerFSM fsm, string toState)
    {
        foreach (FsmState state in fsm.FsmStates)
        {
            state.RemoveTransitionsTo(toState);
        }
    }

    /// <summary>
    ///     Removes all transitions from a state to another specified state in a PlayMakerFSM.  
    ///     Trying to remove a transition that doesn't exist will result in the transitions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state from which the transition starts</param>
    /// <param name="toState">The target of the transition</param>
    [PublicAPI]
    public static void RemoveTransitionsTo(this PlayMakerFSM fsm, string stateName, string toState) => fsm.GetState(stateName)!.RemoveTransitionsTo(toState);

    /// <inheritdoc cref="RemoveTransitionsTo(PlayMakerFSM, string, string)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="toState">The event of the transition</param>
    [PublicAPI]
    public static void RemoveTransitionsTo(this FsmState state, string toState) => state.Transitions = RemoveItemsFromArray(state.Transitions, x => x.ToState == toState);

    /// <summary>
    ///     Removes all transitions from a state in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state from which the transition starts</param>
    [PublicAPI]
    public static void RemoveTransitions(this PlayMakerFSM fsm, string stateName) => fsm.GetState(stateName)!.RemoveTransitions();

    /// <inheritdoc cref="RemoveTransitions(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static void RemoveTransitions(this FsmState state) => state.Transitions = [];

    /// <summary>
    ///     Removes an action in a PlayMakerFSM.  
    ///     Trying to remove an action that doesn't exist will result in the actions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state with the action</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static bool RemoveAction(this PlayMakerFSM fsm, string stateName, int index) => fsm.GetState(stateName)!.RemoveAction(index);

    /// <inheritdoc cref="RemoveAction(PlayMakerFSM, string, int)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static bool RemoveAction(this FsmState state, int index)
    {
        FsmStateAction[] origActions = state.Actions;
        if (index < 0 || index >= origActions.Length)
        {
            return false;
        }
        FsmStateAction[] newActions = new FsmStateAction[origActions.Length - 1];
        int newActionsCount = newActions.Length;
        int i;
        for (i = 0; i < index; i++)
        {
            newActions[i] = origActions[i];
        }
        for (i = index; i < newActionsCount; i++)
        {
            newActions[i] = origActions[i + 1];
        }

        state.Actions = newActions;
        return true;
    }

    /// <summary>
    ///     Removes all actions of a given type in a PlayMakerFSM.
    /// </summary>
    /// <typeparam name="TAction">The type of actions to remove</typeparam>
    /// <param name="fsm">The fsm</param>
    [PublicAPI]
    public static void RemoveActionsOfType<TAction>(this PlayMakerFSM fsm)
    {
        foreach (FsmState state in fsm.FsmStates)
        {
            state.RemoveActionsOfType<TAction>();
        }
    }

    /// <summary>
    ///     Removes all actions of a given type in an FsmState.  
    /// </summary>
    /// <typeparam name="TAction">The type of actions to remove</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to remove the actions from</param>
    [PublicAPI]
    public static void RemoveActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) => fsm.GetState(stateName)!.RemoveActionsOfType<TAction>();

    /// <inheritdoc cref="RemoveActionsOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static void RemoveActionsOfType<TAction>(this FsmState state) => state.Actions = RemoveItemsFromArray(state.Actions, x => x is TAction);

    /// <summary>
    ///     Removes first action of a given type in an FsmState.  
    /// </summary>
    /// <typeparam name="TAction">The type of actions to remove</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to remove the actions from</param>
    [PublicAPI]
    public static void RemoveFirstActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) => fsm.GetState(stateName)!.RemoveFirstActionOfType<TAction>();

    /// <inheritdoc cref="RemoveFirstActionOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static void RemoveFirstActionOfType<TAction>(this FsmState state)
    {
        int firstActionIndex = -1;
        for (int i = 0; i < state.Actions.Length; i++)
        {
            if (state.Actions[i] is TAction)
            {
                firstActionIndex = i;
                break;
            }
        }

        if (firstActionIndex == -1)
            return;
        state.RemoveAction(firstActionIndex);
    }

    /// <summary>
    ///     Removes last action of a given type in an FsmState.  
    /// </summary>
    /// <typeparam name="TAction">The type of actions to remove</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to remove the actions from</param>
    [PublicAPI]
    public static void RemoveLastActionOfType<TAction>(this PlayMakerFSM fsm, string stateName) => fsm.GetState(stateName)!.RemoveLastActionOfType<TAction>();

    /// <inheritdoc cref="RemoveLastActionOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static void RemoveLastActionOfType<TAction>(this FsmState state)
    {
        int lastActionIndex = -1;
        for (int i = state.Actions.Length - 1; i >= 0; i--)
        {
            if (state.Actions[i] is TAction)
            {
                lastActionIndex = i;
                break;
            }
        }

        if (lastActionIndex == -1)
            return;
        state.RemoveAction(lastActionIndex);
    }

    #endregion Remove

    #region Disable

    /// <summary>
    ///     Disables an action in a PlayMakerFSM.  
    ///     Trying to disable an action that doesn't exist will result in the actions not being changed.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state with the action</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static bool DisableAction(this PlayMakerFSM fsm, string stateName, int index) => fsm.GetState(stateName)!.DisableAction(index);

    /// <inheritdoc cref="DisableAction(PlayMakerFSM, string, int)"/>
    /// <param name="state">The fsm state</param>
    /// <param name="index">The index of the action</param>
    [PublicAPI]
    public static bool DisableAction(this FsmState state, int index)
    {
        if (index < 0 || index >= state.Actions.Length)
        {
            return false;
        }
        state.Actions[index].Enabled = false;
        return true;
    }

    /// <summary>
    ///     Disables all actions of a given type in a PlayMakerFSM.
    /// </summary>
    /// <typeparam name="TAction">The type of actions to disable</typeparam>
    /// <param name="fsm">The fsm</param>
    [PublicAPI]
    public static void DisableActionsOfType<TAction>(this PlayMakerFSM fsm)
    {
        foreach (FsmState state in fsm.FsmStates)
        {
            state.DisableActionsOfType<TAction>();
        }
    }

    /// <summary>
    ///     Disables all actions of a given type in an FsmState.  
    /// </summary>
    /// <typeparam name="TAction">The type of actions to disable</typeparam>
    /// <param name="fsm">The fsm</param>
    /// <param name="stateName">The name of the state to disable the actions from</param>
    [PublicAPI]
    public static void DisableActionsOfType<TAction>(this PlayMakerFSM fsm, string stateName) => fsm.GetState(stateName)!.DisableActionsOfType<TAction>();

    /// <inheritdoc cref="DisableActionsOfType{TAction}(PlayMakerFSM, string)"/>
    /// <param name="state">The fsm state</param>
    [PublicAPI]
    public static void DisableActionsOfType<TAction>(this FsmState state)
    {
        foreach (FsmStateAction action in state.Actions)
        {
            if (action is TAction)
            {
                action.Enabled = false;
            }
        }
    }

    #endregion Disable

    #region FSM Variables

    private static TVar[] MakeNewVariableArray<TVar>(TVar[] orig, string name) where TVar : NamedVariable, new()
    {
        TVar[] newArray = new TVar[orig.Length + 1];
        orig.CopyTo(newArray, 0);
        newArray[orig.Length] = new TVar { Name = name };
        return newArray;
    }

    /// <summary>
    ///     Adds a fsm variable in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="name">The name of the new variable</param>
    /// <returns>The newly created variable</returns>
    [PublicAPI]
    public static FsmFloat AddFloatVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.FloatVariables, name);
        fsm.FsmVariables.FloatVariables = tmp;
        return tmp[^1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmInt AddIntVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.IntVariables, name);
        fsm.FsmVariables.IntVariables = tmp;
        return tmp[^1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmBool AddBoolVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.BoolVariables, name);
        fsm.FsmVariables.BoolVariables = tmp;
        return tmp[^1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmString AddStringVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.StringVariables, name);
        fsm.FsmVariables.StringVariables = tmp;
        return tmp[^1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector2 AddVector2Variable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.Vector2Variables, name);
        fsm.FsmVariables.Vector2Variables = tmp;
        return tmp[^1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector3 AddVector3Variable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.Vector3Variables, name);
        fsm.FsmVariables.Vector3Variables = tmp;
        return tmp[^1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmColor AddColorVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.ColorVariables, name);
        fsm.FsmVariables.ColorVariables = tmp;
        return tmp[^1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmRect AddRectVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.RectVariables, name);
        fsm.FsmVariables.RectVariables = tmp;
        return tmp[^1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmQuaternion AddQuaternionVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.QuaternionVariables, name);
        fsm.FsmVariables.QuaternionVariables = tmp;
        return tmp[^1];
    }

    /// <inheritdoc cref="AddFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmGameObject AddGameObjectVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = MakeNewVariableArray(fsm.FsmVariables.GameObjectVariables, name);
        fsm.FsmVariables.GameObjectVariables = tmp;
        return tmp[^1];
    }

    private static TVar? FindInVariableArray<TVar>(TVar[] orig, string name) where TVar : NamedVariable, new()
    {
        foreach (TVar item in orig)
        {
            if (item.Name == name)
            {
                return item;
            }
        }
        return null;
    }

    /// <summary>
    ///     Finds a fsm variable in a PlayMakerFSM.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="name">The name of the variable</param>
    /// <returns>The variable, null if not found</returns>
    [PublicAPI]
    public static FsmFloat? FindFloatVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.FloatVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmInt? FindIntVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.IntVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmBool? FindBoolVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.BoolVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmString? FindStringVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.StringVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector2? FindVector2Variable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.Vector2Variables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector3? FindVector3Variable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.Vector3Variables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmColor? FindColorVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.ColorVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmRect? FindRectVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.RectVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmQuaternion? FindQuaternionVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.QuaternionVariables, name);

    /// <inheritdoc cref="FindFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmGameObject? FindGameObjectVariable(this PlayMakerFSM fsm, string name) => FindInVariableArray(fsm.FsmVariables.GameObjectVariables, name);

    /// <summary>
    ///     Gets a fsm variable in a PlayMakerFSM. Creates a new one if none with the name are present.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="name">The name of the variable</param>
    /// <returns>The variable</returns>
    [PublicAPI]
    public static FsmFloat GetFloatVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindFloatVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddFloatVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmInt GetIntVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindIntVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddIntVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmBool GetBoolVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindBoolVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddBoolVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmString GetStringVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindStringVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddStringVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector2 GetVector2Variable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindVector2Variable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddVector2Variable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmVector3 GetVector3Variable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindVector3Variable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddVector3Variable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmColor GetColorVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindColorVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddColorVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmRect GetRectVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindRectVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddRectVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmQuaternion GetQuaternionVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindQuaternionVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddQuaternionVariable(name);
    }

    /// <inheritdoc cref="GetFloatVariable(PlayMakerFSM, string)"/>
    [PublicAPI]
    public static FsmGameObject GetGameObjectVariable(this PlayMakerFSM fsm, string name)
    {
        var tmp = fsm.FindGameObjectVariable(name);
        if (tmp != null)
            return tmp;
        return fsm.AddGameObjectVariable(name);
    }

    #endregion FSM Variables

    #region Log

    /// <summary>
    ///     Adds actions to a PlayMakerFSM so it gives a log message before and after every single normal action.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    /// <param name="additionalLogging">Flag if, additionally, every log should also log the state of all fsm variables</param>
    [PublicAPI]
    public static void MakeLog(this PlayMakerFSM fsm, bool additionalLogging = false)
    {
        foreach (var s in fsm.FsmStates)
        {
            for (int i = s.Actions.Length - 1; i >= 0; i--)
            {
                fsm.InsertAction(s.Name, new LogAction { Text = $"{i}" }, i);
                if (additionalLogging)
                {
                    fsm.InsertMethod(s.Name, (self) =>
                    {
                        var fsmVars = fsm.FsmVariables;
                        foreach (var variable in fsmVars.FloatVariables)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[FloatVariables] - '{variable.Name}': '{variable.Value}'");
                        foreach (var variable in fsmVars.IntVariables)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[IntVariables] - '{variable.Name}': '{variable.Value}'");
                        foreach (var variable in fsmVars.BoolVariables)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[BoolVariables] - '{variable.Name}': '{variable.Value}'");
                        foreach (var variable in fsmVars.StringVariables)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[StringVariables] - '{variable.Name}': '{variable.Value}'");
                        foreach (var variable in fsmVars.Vector2Variables)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[Vector2Variables] - '{variable.Name}': '({variable.Value.x}, {variable.Value.y})'");
                        foreach (var variable in fsmVars.Vector3Variables)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[Vector3Variables] - '{variable.Name}': '({variable.Value.x}, {variable.Value.y}, {variable.Value.z})'");
                        foreach (var variable in fsmVars.ColorVariables)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[ColorVariables] - '{variable.Name}': '{variable.Value}'");
                        foreach (var variable in fsmVars.RectVariables)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[RectVariables] - '{variable.Name}': '{variable.Value}'");
                        foreach (var variable in fsmVars.QuaternionVariables)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[QuaternionVariables] - '{variable.Name}': '{variable.Value}'");
                        foreach (var variable in fsmVars.GameObjectVariables)
                            InternalLogger.Log($"[{fsm.gameObject.name}]:[{fsm.FsmName}]:[GameObjectVariables] - '{variable.Name}': '{variable.Value}'");
                    }, i + 1);
                }
            }
        }
    }

    /// <summary>
    ///     Logs the fsm and its states and transitions.
    /// </summary>
    /// <param name="fsm">The fsm</param>
    [PublicAPI]
    public static void Log(this PlayMakerFSM fsm)
    {
        Log($"FSM \"{fsm.name}\"");
        Log($"{fsm.FsmStates.Length} States");
        foreach (var s in fsm.FsmStates)
        {
            Log($"\tState \"{s.Name}\"");
            foreach (var t in s.Transitions)
            {
                Log($"\t\t-> \"{t.ToState}\" via \"{t.EventName}\"");
            }
        }
        Log($"{fsm.FsmGlobalTransitions.Length} Global Transitions");
        foreach (var t in fsm.FsmGlobalTransitions)
        {
            Log($"\tGlobal Transition \"{t.EventName}\" to \"{t.ToState}\"");
        }
    }

    private static void Log(string msg)
    {
        InternalLogger.Log($"[Core]:[FsmUtil]:[FsmUtil] - {msg}");
    }

    #endregion Log
}
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MVP.Model
{
    /// <summary>
    /// Represents a simple reflection-based StateMachine.
    /// </summary>
    public class StateMachine
    {
        /// <summary>
        /// A static wrapper around the constructor for a StateMachine.
        /// Returns true, iff the StateMachine was constructed successfully. False, iff the provided stateMachineDefinition was malformed or a declared handler was not found within the supplied objects.
        /// <br></br><br></br>
        /// Grammar for the stateMachineDefinition:
        /// <br></br>StartToken=>             StateDefinitionToken;
        /// <br></br>StartToken=>             StateDefinitionToken; StartToken
        /// <br></br>StateDefinitionToken=>   (StateDeclarationToken) -> {}
        /// <br></br>StateDefinitionToken=>   (StateDeclarationToken) -> {StateChangeToken}
        /// <br></br>StateDeclarationToken=>  uniqueStateName
        /// <br></br>StateDeclarationToken=>  uniqueStateName, onStateEnterHandlerName
        /// <br></br>StateDeclarationToken=>  uniqueStateName, onStateEnterHandlerName, onStateLeaveHandlerName
        /// <br></br>StateChangeToken=>       stateChangeConditionName: targetStateName
        /// <br></br>StateChangeToken=>       stateChangeConditionName: targetStateName, StateChangeToken
        /// <br></br><br></br>
        /// Example for a simple three-state stateMachineDefinition:
        /// <br></br>(State1) -> {ConditionA: State2, ConditionB: State3};
        /// <br></br>(State2, OnEnter2, OnLeave2) -> {ConditionC: State3};
        /// <br></br>(State3, OnEnter3) -> {};
        /// <br></br><br></br>
        /// Some parts of the grammar are not enforced (e.g. trailing semicolon, trailing comma, whitespace and brackets are all ignored).
        /// The user is adviced to adhere to the grammer in order to keep the stateMachineDefinition readable.
        /// </summary>
        /// <param name="stateMachineDefinition">The string defining a StateMachine according to the grammar</param>
        /// <param name="stateMachine">The resulting StateMachine object, iff true is returned. null otherwise</param>
        /// <param name="targetObjectsForStateMachine">The objects which contain the stateEnter and stateLeave handler methods. Only directly declared and public methods are considered, not inherited ones. Also, method names need to be unique throughout all supplied objects</param>
        /// <returns>whether or not the construction of the StateMachine was successful.</returns>
        #region TryCreateStateMachine
        public static bool TryCreateStateMachine(string stateMachineDefinition, out StateMachine stateMachine, params object[] targetObjectsForStateMachine)
        {
            stateMachine = null;
            try
            {
                stateMachine = new StateMachine(stateMachineDefinition, targetObjects: targetObjectsForStateMachine);
                return true;
            }
            catch (StateMachineException e)
            {
                Debug.LogWarning(e.Message);
                return false;
            }
        }
        #endregion

        /// <summary>
        /// Use TryCreateStateMachine instead!
        /// 
        /// The constructor for a StateMachine.
        /// Fails and throws a StateMachineException, iff the provided stateMachineDefinition was malformed or a declared handler was not found within the supplied objects.
        /// <br></br><br></br>
        /// Grammar for the stateMachineDefinition:
        /// <br></br>StartToken=>             StateDefinitionToken;
        /// <br></br>StartToken=>             StateDefinitionToken; StartToken
        /// <br></br>StateDefinitionToken=>   (StateDeclarationToken) -> {}
        /// <br></br>StateDefinitionToken=>   (StateDeclarationToken) -> {StateChangeToken}
        /// <br></br>StateDeclarationToken=>  uniqueStateName
        /// <br></br>StateDeclarationToken=>  uniqueStateName, onStateEnterHandlerName
        /// <br></br>StateDeclarationToken=>  uniqueStateName, onStateEnterHandlerName, onStateLeaveHandlerName
        /// <br></br>StateChangeToken=>       stateChangeConditionName: targetStateName
        /// <br></br>StateChangeToken=>       stateChangeConditionName: targetStateName, StateChangeToken
        /// <br></br><br></br>
        /// Example for a simple three-state stateMachineDefinition:
        /// <br></br>(State1) -> {ConditionA: State2, ConditionB: State3};
        /// <br></br>(State2, OnEnter2, OnLeave2) -> {ConditionC: State3};
        /// <br></br>(State3, OnEnter3) -> {};
        /// <br></br><br></br>
        /// Some parts of the grammar are not enforced (e.g. trailing semicolon, trailing comma, whitespace and brackets are all ignored).
        /// The user is adviced to adhere to the grammer in order to keep the stateMachineDefinition readable.
        /// </summary>
        /// <param name="stateMachineDefinition">The string defining a StateMachine according to the grammar</param>
        /// <param name="targetObjects">The objects which contain the stateEnter and stateLeave handler methods. Only directly declared and public methods are considered, not inherited ones. Also, method names need to be unique throughout all supplied objects</param>
        /// <exception cref="StateMachineException">if the provided string is malformed.</exception>
        #region Constructor
        public StateMachine(string stateMachineDefinition, params object[] targetObjects)
        {
            var availableMethods = ExtractAvailableMethods(targetObjects);
            var conditionsForEachState = new Dictionary<State, Dictionary<Condition, string>>();
            foreach (var stateDefinition in SplitIntoStateDefinitions(stateMachineDefinition))
            {
                (var header, var stateChanges) = SplitIntoHeaderAndStateChanges(stateDefinition);
                var stateHeader = ExtractStateNameAndActions(header);
                var state = CreateState(stateHeader, availableMethods);
                if (!currentState.HasValue)
                    currentState = state;
                var stateConditions = ExtractStateChanges(stateChanges);
                conditionsForEachState.Add(state, stateConditions);
            }
            currentState?.InvokeOnEnter();
            SetStateChanges(conditionsForEachState);
        }
        #endregion

        /// <summary>
        /// Converts this StateMachine into it's string-representation.
        /// The resulting string is similar to the stateMachineDefinition used to create the StateMachine, with the exception of the event-handlers being anonymous to the States and therefore not present in the resulting string.
        /// </summary>
        /// <returns>the string-representation of this StateMachine.</returns>
        #region ToString
        public override string ToString()
        {
            var str = "State Machine:\n";
            foreach (var state in states.Values)
                str += state.ToString() + ";\n";
            return str;
        }
        #endregion

        /// <summary>
        /// Changes the current state of this StateMachine with respect to the provided condition and the defined state-changes.
        /// If a state-change isn't defined for the current state and the provided condition, the StateMachine remains in it's current state.
        /// Invokes the corresponding event handlers iff the state changes. 
        /// </summary>
        /// <param name="condition">The condition for the state-change.</param>
        public void ChangeState(Condition condition) => currentState = currentState?.GetNextState(condition);

        /// <summary>
        /// Changes the current state of this StateMachine with respect to the provided condition and the defined state-changes.
        /// If a state-change isn't defined for the current state and the provided condition, the StateMachine remains in it's current state.
        /// Invokes the corresponding event handlers iff the state changes. 
        /// </summary>
        /// <param name="condition">The condition for the state-change in it's string form.</param>
        public void ChangeState(string condition) => ChangeState(Condition.GetConditionInstance(condition));

        /// <summary>
        /// Forces the current state to change to the target state regardless of the defined state-changes and their conditions.
        /// The StateMachine can only be set into a state with which the StateMachine was originally constructed.
        /// Invokes event handlers iff the state changes. When the target state is equal to the current state, no event handlers are invoked.
        /// </summary>
        /// <param name="stateName">The target state to set the StateMachine into</param>
        /// <returns>whether the target state is part of this StateMachine and is unequal to the current state -> a state-change occured.</returns>
        #region SetState
        public bool SetState(string stateName)
        {
            if (!(currentState?.Name.Equals(stateName) ?? true) && states.ContainsKey(stateName))
            {
                currentState?.InvokeOnLeave();
                currentState = states[stateName];
                currentState?.InvokeOnEnter();
                return true;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Forces the current state to change to the target state regardless of the defined state-changes and their conditions.
        /// The StateMachine can only be set into a state with which the StateMachine was originally constructed.
        /// No event handlers are invoked.
        /// </summary>
        /// <param name="stateName">The target state to set the StateMachine into</param>
        /// <returns>whether the target state is part of this StateMachine and is unequal to the current state -> a state-change occured.</returns>
        #region SetStateNoInvoke
        public bool SetStateNoInvoke(string stateName)
        {
            if (!(currentState?.Name.Equals(stateName) ?? true) && states.ContainsKey(stateName))
            {
                currentState = states[stateName];
                return true;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Checks whether this StateMachine's current State has the same name as the provided State.
        /// </summary>
        /// <param name="state">The provided State to check equality with.</param>
        /// <returns>Whether this StateMachine is in the provided State.</returns>
        public bool IsInState(State state) => currentState.HasValue && currentState.Value.Name == state.Name;

        /// <summary>
        /// Checks whether this StateMachine's current State has the same name as the provided state name.
        /// </summary>
        /// <param name="state">The provided state name to check equality with.</param>
        /// <returns>Whether this StateMachine is in the specified State.</returns>
        public bool IsInState(string state) => currentState.HasValue && currentState.Value.Name == state;

        #region Internal
        private static readonly char[] SEMICOLON = { ';' };
        private static readonly string[] ARROW = { "->" };
        private static readonly char[] COMMA = { ',' };
        private static readonly char[] COLON = { ':' };
        private static readonly Action EMPTY_ACTION = () => { };
        private static readonly string ERR_MALFORMED_STATE_DEF_SYNTAX = "Syntax error in state-definition! Correct syntax is '(...) -> {...}'.";
        private static readonly string ERR_NO_STATE_NAME = "No State name in state-definition found! Name needs to be specified '(STATE_NAME [, ON_STATE_ENTER [, ON_STATE_LEAVE ]])'.";
        private static readonly string ERR_NO_AVAILABLE_METHOD = "No available method found in supplied objects! Methods must have the same name as specified in the state-definition, have no parameters, be public, not be a constructor and not be abstract to be usable by the state-machine via reflection.";
        private static readonly string ERR_DUPLICATE_METHOD_NAMES = "Duplicate method names found in supplied objects! Methods need to have different names to be usable by the state machine via reflection.";
        private static readonly string ERR_DUPLICATE_STATE_NAMES = "Duplicate state names found! States within a state machine must be uniquely named.";
        private static readonly string ERR_MALFORMED_STATE_CHANGE_SYNTAX = "Syntax error in state-change definition! Correct syntax is 'CONDITION: NEXT_STATE'.";
        private static readonly string ERR_DUPLICATE_CONDITION = "Duplicate condition found in state-definition! Each state is allowed a maximum of one instance per condition-type.";
        private static readonly string ERR_NO_TARGET_STATE = "Target state of state-change does not exist! All future states need to be defined.";
        private static readonly string ERR_NO_STATE = "Syntax error! The provided string does not specify at least one valid state.";

        private static Dictionary<string, Action> ExtractAvailableMethods(object[] targetObjects)
        {
            var availableMethodNames = new Dictionary<string, Action>();
            foreach (var obj in targetObjects)
                foreach (var methodInfo in obj.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public))
                    if (!methodInfo.IsConstructor && methodInfo.GetParameters().Length == 0)
                        if (availableMethodNames.ContainsKey(methodInfo.Name))
                            throw new StateMachineException(ERR_DUPLICATE_METHOD_NAMES + "  ''" + methodInfo.Name + "()'' in ''" + obj.GetType().FullName + "''");
                        else availableMethodNames.Add(methodInfo.Name, () => methodInfo.Invoke(obj, new object[0]));
            return availableMethodNames;
        }

        private static string[] SplitIntoStateDefinitions(string machineDefinition)
        {
            var stateDefinitions = machineDefinition.Replace("\r", "").Replace("\t", "").Replace("\n", "").Replace(" ", "").Split(SEMICOLON, StringSplitOptions.RemoveEmptyEntries);
            if (stateDefinitions.Length == 0)
                throw new StateMachineException(ERR_NO_STATE);
            return stateDefinitions;
        }

        private static (string, string) SplitIntoHeaderAndStateChanges(string stateDefinition)
        {
            var headerAndStateChanges = stateDefinition.Split(ARROW, StringSplitOptions.RemoveEmptyEntries);
            if (headerAndStateChanges.Length != 2)
                throw new StateMachineException(ERR_MALFORMED_STATE_DEF_SYNTAX + "  ''" + stateDefinition + "''");
            var header = headerAndStateChanges[0].Replace("(", "").Replace(")", "");
            var stateChanges = headerAndStateChanges[1].Replace("{", "").Replace("}", "");
            return (header, stateChanges);
        }

        private static StateHeader ExtractStateNameAndActions(string stateHeader)
        {
            var headerSplit = stateHeader.Split(COMMA, StringSplitOptions.RemoveEmptyEntries);
            if (headerSplit.Length < 1)
                throw new StateMachineException(ERR_NO_STATE_NAME + "  ''" + stateHeader + "''");
            var stateName = headerSplit[0];
            if (headerSplit.Length > 2)
                return new StateHeader(stateName, headerSplit[1], headerSplit[2]);
            if (headerSplit.Length > 1)
                return new StateHeader(stateName, headerSplit[1]);
            return new StateHeader(stateName);
        }

        private static Dictionary<Condition, string> ExtractStateChanges(string stateChanges)
        {
            var conditionsAndNextStates = new Dictionary<Condition, string>();
            foreach (var stateChange in stateChanges.Split(COMMA, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!stateChange.Contains(":"))
                    throw new StateMachineException(ERR_MALFORMED_STATE_CHANGE_SYNTAX + "  ''" + stateChange + "''");
                var conditonNameAndNextState = stateChange.Split(COLON, StringSplitOptions.RemoveEmptyEntries);
                var condition = Condition.GetConditionInstance(conditonNameAndNextState[0]);
                if (conditionsAndNextStates.ContainsKey(condition))
                    throw new StateMachineException(ERR_DUPLICATE_CONDITION + "  ''" + condition.Name + "'' in ''" + stateChanges + "''");
                conditionsAndNextStates.Add(condition, conditonNameAndNextState[1]);
            }
            return conditionsAndNextStates;
        }

        private Dictionary<string, State> states = new Dictionary<string, State>();
        private State? currentState = null;

        private State CreateState(StateHeader stateHeader, Dictionary<string, Action> availableMethods)
        {
            if (states.ContainsKey(stateHeader.StateName))
                throw new StateMachineException(ERR_DUPLICATE_STATE_NAMES + "  ''" + stateHeader.StateName + "''");
            var onEnterAction = EMPTY_ACTION;
            if (stateHeader.HasOnEnterName)
                if (availableMethods.ContainsKey(stateHeader.OnEnterName))
                    onEnterAction = availableMethods[stateHeader.OnEnterName];
                else throw new StateMachineException(ERR_NO_AVAILABLE_METHOD + "  ''" + stateHeader.OnEnterName + "''");
            var onLeaveAction = EMPTY_ACTION;
            if (stateHeader.HasOnLeaveName)
                if (availableMethods.ContainsKey(stateHeader.OnLeaveName))
                    onLeaveAction = availableMethods[stateHeader.OnLeaveName];
                else throw new StateMachineException(ERR_NO_AVAILABLE_METHOD + "  ''" + stateHeader.OnLeaveName + "''");
            states.Add(stateHeader.StateName, new State(stateHeader.StateName, onEnterAction, onLeaveAction));
            return states[stateHeader.StateName];
        }

        private void SetStateChanges(Dictionary<State, Dictionary<Condition, string>> conditionsForEachState)
        {
            foreach (var state in conditionsForEachState.Keys)
                foreach (var condition in conditionsForEachState[state].Keys)
                {
                    var targetStateName = conditionsForEachState[state][condition];
                    if (!states.ContainsKey(targetStateName))
                        throw new StateMachineException(ERR_NO_TARGET_STATE + "  ''" + targetStateName + "''");
                    state.SetFutureState(condition, states[targetStateName]);
                }
        }

        private struct StateHeader
        {
            public StateHeader(string stateName, string onEnterName, string onLeaveName)
            {
                StateName = stateName;
                HasOnEnterName = true;
                OnEnterName = onEnterName;
                HasOnLeaveName = true;
                OnLeaveName = onLeaveName;
            }

            public StateHeader(string stateName, string onEnterName)
            {
                StateName = stateName;
                HasOnEnterName = true;
                OnEnterName = onEnterName;
                HasOnLeaveName = false;
                OnLeaveName = null;
            }

            public StateHeader(string stateName)
            {
                StateName = stateName;
                HasOnEnterName = false;
                OnEnterName = null;
                HasOnLeaveName = false;
                OnLeaveName = null;
            }

            public string StateName { get; }
            public bool HasOnEnterName { get; }
            public string OnEnterName { get; }
            public bool HasOnLeaveName { get; }
            public string OnLeaveName { get; }
        }
        #endregion
    }

    /// <summary>
    /// An excpetion for when a StateMachine creation fails.
    /// </summary>
    #region StateMachineException
    public class StateMachineException : Exception
    {
        public StateMachineException(string msg) : base(msg) { }
    }
    #endregion
}
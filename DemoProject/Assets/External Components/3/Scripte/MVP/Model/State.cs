using System;
using System.Collections.Generic;
using UnityEngine;

namespace MVP.Model
{
    /// <summary>
    /// Represents a state within a state machine
    /// </summary>
    public struct State
    {
        /// <summary>
        /// The name of this state.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Get: retrieves the future State corresponding to the provided condition, iff present.
        /// Set: defines/removes the future State for the provided condition.
        /// </summary>
        /// <param name="condition">The condition for which to get/set the future state</param>
        /// <returns>the future State for this condition.</returns>
        #region Operator []
        public State? this[Condition condition]
        {
            get
            {
                if (followUpStates.ContainsKey(condition))
                    return followUpStates[condition];
                else return this;
            }
            set
            {
                if (value.HasValue)
                    followUpStates[condition] = value.Value;
                else followUpStates.Remove(condition);
            }
        }
        #endregion

        /// <summary>
        /// Constructs a new State given a name.
        /// </summary>
        /// <param name="name">The name of the State</param>
        #region Constructor(string)
        public State(string name)
        {
            this.Name = name;
            this.followUpStates = new Dictionary<Condition, State>();
            this.onStateEnter = () => { };
            this.onStateLeave = () => { };
        }
        #endregion

        /// <summary>
        /// Constructs a new State given a name.
        /// </summary>
        /// <param name="name">The name of the State</param>
        /// <param name="onStateEnter">The action to perform when this State is entered from another State</param>
        #region Constructor(string, Action)
        public State(string name, Action onStateEnter)
        {
            this.Name = name;
            this.followUpStates = new Dictionary<Condition, State>();
            this.onStateEnter = onStateEnter;
            this.onStateLeave = () => { };
        }
        #endregion

        /// <summary>
        /// Constructs a new State given a name.
        /// </summary>
        /// <param name="name">The name of the State</param>
        /// <param name="onStateEnter">The action to perform when this State is entered from another State</param>
        /// <param name="onStateLeave">The action to perform when another State is entered from this State</param>
        #region Constructor(string, Action, Action)
        public State(string name, Action onStateEnter, Action onStateLeave)
        {
            this.Name = name;
            this.followUpStates = new Dictionary<Condition, State>();
            this.onStateEnter = onStateEnter;
            this.onStateLeave = onStateLeave;
        }
        #endregion

        /// <summary>
        /// Alternative to the []-operator set-mode.
        /// Defines/removes the future State for the provided condition.
        /// </summary>
        /// <param name="condition">The condition for which to set the future state</param>
        /// <param name="futureState">The future state to set/override</param>
        public void SetFutureState(Condition condition, State futureState) => this[condition] = futureState;

        /// <summary>
        /// Converts this State into it's string-representation.
        /// The resulting string contains the State's name as well as all it's follow-up States and their conditions.
        /// </summary>
        /// <returns>the string-representation of this State.</returns>
        #region ToString
        public override string ToString()
        {
            var str = Name + " {";
            foreach (var condition in followUpStates.Keys)
                str += condition.ToString() + ": " + followUpStates[condition].Name + ", ";
            return str + "}";
        }
        #endregion

        /// <summary>
        /// Leaves this State, finds the next State given the provided condition and enters the next State.
        /// If the provided condition is unknown to the State, the State remains the same.
        /// </summary>
        /// <param name="condition">The condition to switch States</param>
        /// <returns>the next State given the provided condition.</returns>
        #region GetNextState
        public State GetNextState(Condition condition)
        {
            if (followUpStates.TryGetValue(condition, out var nextState))
            {
                this.onStateLeave.Invoke();
                nextState.onStateEnter.Invoke();
                return nextState;
            }
            else
            {
                Debug.Log($"The state '{Name}' does not have a condition '{condition.Name}' for a state change!");
                return this;
            }
        }
        #endregion

        /// <summary>
        /// Finds the next State given the provided condition.
        /// No actions are performed on this State and the next State.
        /// If the provided condition is unknown to the State, the State remains the same.
        /// </summary>
        /// <param name="condition">The condition to switch States</param>
        /// <returns>the next State given the provided condition.</returns>
        #region PeekNextState
        public State PeekNextState(Condition condition)
        {
            if (followUpStates.TryGetValue(condition, out var nextState))
                return nextState;
            else return this;
        }
        #endregion

        /// <summary>
        /// Manually invokes the "onStateEnter" event handler of this State.
        /// </summary>
        public void InvokeOnEnter() => onStateEnter.Invoke();

        /// <summary>
        /// Manually invokes the "onStateLeave" event handler of this State.
        /// </summary>
        public void InvokeOnLeave() => onStateLeave.Invoke();

        #region Internal
        private Dictionary<Condition, State> followUpStates;
        private Action onStateEnter;
        private Action onStateLeave;
        #endregion
    }
}
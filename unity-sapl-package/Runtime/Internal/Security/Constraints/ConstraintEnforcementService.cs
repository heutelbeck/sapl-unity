/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using csharp.sapl.constraint.api;
using csharp.sapl.pdp.api;
using Newtonsoft.Json.Linq;
using UnityEngine;
using static csharp.sapl.constraint.ISignal;

namespace Sapl.Internal.Security.Constraints
{
	///<summary>
	/// 
	/// The ConstraintEnforcementService is responsible for collecting executable
	/// constraint handlers in bundles for the PEP whenever the PDP sends a new
	/// decision. The PEP in return will execute the matching handlers in the
	/// protected code path.
	///
	///</summary>
	public class ConstraintEnforcementService
	{
		private readonly List<IRunnableConstraintHandlerProvider> globalRunnableProviders;
		private readonly List<IGameObjectConsumerConstraintHandlerProvider> globalGameObjectConsumerProviders;
		private readonly List<IJsonConsumerConstraintHandlerProvider> globalJsonConsumerProviders;
		private readonly List<IBoolFunctionConstraintHandlerProvider> globalBoolFunctionHandlerProviders;



		/// <summary>Initializes a new instance of the <see cref="ConstraintEnforcementService" /> class.</summary>
		/// <param name="globalRunnableProviders">The global runnable providers.</param>
		public ConstraintEnforcementService(List<IRunnableConstraintHandlerProvider> globalRunnableProviders)
		{
			this.globalRunnableProviders = globalRunnableProviders;
			this.globalGameObjectConsumerProviders = new List<IGameObjectConsumerConstraintHandlerProvider>();
			this.globalJsonConsumerProviders = new List<IJsonConsumerConstraintHandlerProvider>();
			this.globalBoolFunctionHandlerProviders = new List<IBoolFunctionConstraintHandlerProvider>();
		}


        /// <summary>Initializes a new instance of the <see cref="ConstraintEnforcementService" /> class.</summary>
        /// <param name="globalRunnableProviders">The global runnable providers.</param>
        /// <param name="globalGameObjectConsumerProviders">The global game object consumer providers.</param>
        public ConstraintEnforcementService(List<IRunnableConstraintHandlerProvider> globalRunnableProviders,
							List<IGameObjectConsumerConstraintHandlerProvider> globalGameObjectConsumerProviders)
		{
			this.globalRunnableProviders = globalRunnableProviders;
			this.globalGameObjectConsumerProviders = globalGameObjectConsumerProviders;
			this.globalJsonConsumerProviders = new List<IJsonConsumerConstraintHandlerProvider>();
			this.globalBoolFunctionHandlerProviders = new List<IBoolFunctionConstraintHandlerProvider>();
		}


        /// <summary>Initializes a new instance of the <see cref="ConstraintEnforcementService" /> class.</summary>
        /// <param name="globalRunnableProviders">The global runnable providers.</param>
        /// <param name="globalGameObjectConsumerProviders">The global game object consumer providers.</param>
        /// <param name="globalJsonConsumerProviders">The global json consumer providers.</param>
        public ConstraintEnforcementService(List<IRunnableConstraintHandlerProvider> globalRunnableProviders,
							List<IGameObjectConsumerConstraintHandlerProvider> globalGameObjectConsumerProviders,
							List<IJsonConsumerConstraintHandlerProvider> globalJsonConsumerProviders)
		{
			this.globalRunnableProviders = globalRunnableProviders;
			this.globalGameObjectConsumerProviders = globalGameObjectConsumerProviders;
			this.globalJsonConsumerProviders = globalJsonConsumerProviders;
			this.globalBoolFunctionHandlerProviders = new List<IBoolFunctionConstraintHandlerProvider>();
		}


        /// <summary>Initializes a new instance of the <see cref="ConstraintEnforcementService" /> class.</summary>
        /// <param name="globalRunnableProviders">The global runnable providers.</param>
        /// <param name="globalGameObjectConsumerProviders">The global game object consumer providers.</param>
        /// <param name="globalJsonConsumerProviders">The global json consumer providers.</param>
        /// <param name="globalBoolFunctionHandlerProviders">The global bool function handler providers.</param>
        public ConstraintEnforcementService(List<IRunnableConstraintHandlerProvider> globalRunnableProviders,
							List<IGameObjectConsumerConstraintHandlerProvider> globalGameObjectConsumerProviders,
							List<IJsonConsumerConstraintHandlerProvider> globalJsonConsumerProviders,
							List<IBoolFunctionConstraintHandlerProvider> globalBoolFunctionHandlerProviders)
		{
			this.globalRunnableProviders = globalRunnableProviders;
			this.globalGameObjectConsumerProviders = globalGameObjectConsumerProviders;
			this.globalJsonConsumerProviders = globalJsonConsumerProviders;
			this.globalBoolFunctionHandlerProviders = globalBoolFunctionHandlerProviders;
		}


		/// <summary>constraints in the decision, or throws AccessDeniedException, if
		/// bundle cannot be constructed.</summary>
		/// <param name="decision">The decision.</param>
		/// <param name="onDecisionBundle">The on decision bundle.</param>
		/// <param name="onExecutionBundle">The on execution bundle.</param>
		/// <exception cref="System.Exception">No handler for obligation: " + String.Join(",", unhandledObligations)</exception>
		public void BundleFor(AuthorizationDecision decision, out OnDecisionConstraintHandlerBundle onDecisionBundle, out OnExecutionConstraintHandlerBundle onExecutionBundle)
		{
			var unhandledObligations = new HashSet<JToken>();
			if (decision.Obligations != null)
				unhandledObligations = new HashSet<JToken>(decision.Obligations);

			// @formatter:off
			onDecisionBundle = new OnDecisionConstraintHandlerBundle(
					RunnableHandlersForSignal(Signal.ON_DECISION, decision, unhandledObligations),
					GameObjectConsumerHandlers(Signal.ON_DECISION, decision, unhandledObligations),
					JsonConsumerHandlers(Signal.ON_DECISION, decision, unhandledObligations),
					BoolFunctionHandlers(Signal.ON_DECISION, decision, unhandledObligations));

			onExecutionBundle = new OnExecutionConstraintHandlerBundle(
					RunnableHandlersForSignal(Signal.ON_EXECUTION, decision, unhandledObligations),
					GameObjectConsumerHandlers(Signal.ON_EXECUTION, decision, unhandledObligations),
					JsonConsumerHandlers(Signal.ON_EXECUTION, decision, unhandledObligations),
					BoolFunctionHandlers(Signal.ON_EXECUTION, decision, unhandledObligations));

			if (unhandledObligations.Count > 0)
			{
				throw new Exception("No handler for obligation: " + String.Join(",", unhandledObligations));
			}
		}

		private Action<GameObject> GameObjectConsumerHandlers(Signal signal, AuthorizationDecision decision, HashSet<JToken> unhandledObligations)
		{
			Action<GameObject> obligationHandlers = Obligation(ConstructGameObjectConsumerHandlersForConstraints(signal, decision.Obligations,
					c => unhandledObligations.Remove(c)));

			Action<GameObject> adviceHandlers = Advice(ConstructGameObjectConsumerHandlersForConstraints(signal, decision.Advice,
					_ => { }));

			return ConsumeWithBoth(obligationHandlers, adviceHandlers);
		}
#nullable enable
		private Action<GameObject> ConstructGameObjectConsumerHandlersForConstraints(Signal signal, JArray? constraints, Action<JToken> onHandlerFound)
		{
			Action<GameObject> handlers = _ => { };

			if (constraints == null)
				return handlers;

			foreach (var constraint in constraints)
			{
				foreach (var provider in globalGameObjectConsumerProviders)
				{
					if (provider.GetSignal().Equals(signal) && provider.IsResponsible((JToken)constraint))
					{
						onHandlerFound.Invoke(constraint);
						handlers = ConsumeWithBoth(handlers, provider.GetHandler(constraint));
					}
				}
			}

			return handlers;
		}
#nullable disable
		private Action RunnableHandlersForSignal(Signal signal, AuthorizationDecision decision,
				HashSet<JToken> unhandledObligations)
		{
			var onDecisionObligationHandlers = Obligation(ConstructRunnableHandlersForConstraints(signal,
					decision.Obligations,
					c => unhandledObligations.Remove(c)));

			var onDecisionAdviceHandlers = Advice(ConstructRunnableHandlersForConstraints(signal,
					decision.Advice,
					_ => { }));

			return RunBoth(onDecisionObligationHandlers, onDecisionAdviceHandlers);
		}
#nullable enable
		private Action ConstructRunnableHandlersForConstraints(Signal signal, JArray? constraints,
							Action<JToken> onHandlerFound)
		{
			Action handlers = () => { };

			if (constraints == null)
				return handlers;

			foreach (var constraint in constraints)
			{
				foreach (var provider in globalRunnableProviders)
				{
					if (provider.GetSignal().Equals(signal) && provider.IsResponsible((JToken)constraint))
					{
						onHandlerFound.Invoke(constraint);
						handlers = RunBoth(handlers, provider.GetHandler(constraint));
					}
				}
			}
			return handlers;
		}

#nullable disable
		private Action<JToken> JsonConsumerHandlers(Signal signal, AuthorizationDecision decision, HashSet<JToken> unhandledObligations)
		{
			Action<JToken> obligationHandlers = Obligation(ConstructJsonConsumerHandlersForConstraints(signal, decision.Obligations,
					c => unhandledObligations.Remove(c)));

			Action<JToken> adviceHandlers = Advice(ConstructJsonConsumerHandlersForConstraints(signal, decision.Advice,
					_ => { }));

			return ConsumeWithBoth(obligationHandlers, adviceHandlers);
		}
#nullable enable
		private Action<JToken> ConstructJsonConsumerHandlersForConstraints(Signal signal, JArray? constraints, Action<JToken> onHandlerFound)
		{
			Action<JToken> handlers = _ => { };

			if (constraints == null)
				return handlers;

			foreach (var constraint in constraints)
			{
				foreach (var provider in globalJsonConsumerProviders)
				{
					if (provider.GetSignal().Equals(signal) && provider.IsResponsible((JToken)constraint))
					{
						onHandlerFound.Invoke(constraint);
						handlers = ConsumeWithBoth(handlers, provider.GetHandler(constraint));
					}
				}
			}
			return handlers;
		}
#nullable disable
		private Func<Boolean> BoolFunctionHandlers(Signal signal, AuthorizationDecision decision, HashSet<JToken> unhandledObligations)
		{
			Func<Boolean> obligationHandlers = Obligation(ConstructBoolFunctionHandlersForConstraints(signal, decision.Obligations,
					c => unhandledObligations.Remove(c)));

			Func<Boolean> adviceHandlers = Advice(ConstructBoolFunctionHandlersForConstraints(signal, decision.Advice,
					_ => { }));

			return CheckBoth(obligationHandlers, adviceHandlers);
		}
#nullable enable
		private Func<Boolean> ConstructBoolFunctionHandlersForConstraints(Signal signal, JArray? constraints, Action<JToken> onHandlerFound)
		{
			Func<Boolean> handlers = () => { return true; };

			if (constraints == null)
				return handlers;

			foreach (var constraint in constraints)
			{
				foreach (var provider in globalBoolFunctionHandlerProviders)
				{
					if (provider.GetSignal().Equals(signal) && provider.IsResponsible((JToken)constraint))
					{
						onHandlerFound.Invoke(constraint);
						handlers = CheckBoth(handlers, provider.GetHandler(constraint));
					}
				}
			}
			return handlers;
		}
#nullable disable
		private Action Obligation(Action handlers)
		{
			return () =>
			{
				try
				{
					handlers.Invoke();
				}
				catch (Exception t)
				{
					throw new ObligationExecutionFailedException("Failed to execute runnable obligation handler", t);
				}
			};
		}

		private Action Advice(Action handlers)
		{
			return () =>
			{
				try
				{
					handlers.Invoke();
				}
				catch (Exception t)
				{
					throw new AdviceExecutionFailedException("Failed to execute runnable advice handler", t);
				}
			};
		}

		private Action<T> Obligation<T>(Action<T> handlers)
		{
			return s =>
			{
				try
				{
					handlers.Invoke(s);
				}
				catch (Exception t)
				{
					throw new ObligationExecutionFailedException("Failed to execute consumer obligation handler " + s.ToString(), t);
				}
			};
		}

		private Action<T> Advice<T>(Action<T> handlers)
		{
			return s =>
			{
				try
				{
					handlers.Invoke(s);
				}
				catch (Exception t)
				{
					throw new AdviceExecutionFailedException("Failed to execute consumer advice handler " + s.ToString(), t);
				}
			};
		}

		private Func<T> Obligation<T>(Func<T> handlers)
		{
			return () =>
			{
				try
				{
					return handlers();
				}
				catch (Exception t)
				{
					throw new ObligationExecutionFailedException("Failed to execute function obligation handler", t);
				}
			};
		}

		private Func<T> Advice<T>(Func<T> handlers)
		{
			return () =>
			{
				try
				{
					return handlers();
				}
				catch (Exception t)
				{
					throw new AdviceExecutionFailedException("Failed to execute function advice handler", t);
				}
			};
		}

		private Action RunBoth(Action a, Action b)
		{
			return () =>
			{
				a.Invoke();
				b.Invoke();
			};
		}

		private Func<Boolean> CheckBoth(Func<Boolean> a, Func<Boolean> b)
		{
			return () =>
			{
				if (a() & b())
					return true;
				else
					return false;
			};
		}

		private Action<T> ConsumeWithBoth<T>(Action<T> a, Action<T> b)
		{
			return (t) =>
			{
				a.Invoke(t);
				b.Invoke(t);
			};
		}
	}
}

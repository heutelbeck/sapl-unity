using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Newtonsoft.Json.Linq;
using csharp.sapl.pdp.api;
using csharp.sapl.constraint.api;
using static csharp.sapl.constraint.api.IRunnableConstraintHandlerProvider;

namespace csharp.sapl.constraint
{
	/**
	 * 
	 * The ConstraintEnforcementService is responsible for collecting executable
	 * constraint handlers in bundles for the PEP whenever the PDP sends a new
	 * decision. The PEP in return will execute the matching handlers in the
	 * protected code path.
	 * 
	 * @author Dominic Heutelbeck
	 *
	 */
	public class ConstraintEnforcementService
	{
		private readonly MultiValueDictionary<Signal, IRunnableConstraintHandlerProvider> globalRunnableIndex;
		private readonly List<IConsumerConstraintHandlerProvider<object>> globalConsumerProviders;

		/**
		 * Constructor with dependency injection of all beans implementing handler
		 * providers.
		 * 
		 * @param globalRunnableProviders            all
		 *                                           RunnableConstraintHandlerProvider
		 * @param globalConsumerProviders            all
		 *                                           ConsumerConstraintHandlerProvider
		 * @param globalSubscriptionHandlerProviders all SubscriptionHandlerProvider
		 * @param globalRequestHandlerProviders      all RequestHandlerProvider
		 * @param globalMappingHandlerProviders      all
		 *                                           MappingConstraintHandlerProvider
		 * @param globalErrorMappingHandlerProviders all
		 *                                           ErrorMappingConstraintHandlerProvider
		 * @param globalErrorHandlerProviders        all ErrorHandlerProvider
		 * @param filterPredicateProviders           all
		 *                                           FilterPredicateConstraintHandlerProvider
		 * @param methodInvocationHandlerProviders   all
		 *                                           MethodInvocationConstraintHandlerProvider
		 * @param mapper                             the global ObjectMapper
		 */
		public ConstraintEnforcementService(List<IRunnableConstraintHandlerProvider> globalRunnableProviders)
					//List<IConsumerConstraintHandlerProvider<object>> globalConsumerProviders)
		{
			//this.globalConsumerProviders = globalConsumerProviders;

			globalRunnableIndex = new MultiValueDictionary<Signal, IRunnableConstraintHandlerProvider>();
			foreach (var provider in globalRunnableProviders)
			{
				globalRunnableIndex.Add(provider.GetSignal(), provider);
			}
		}

		/**
		 * Takes the decision and derives a wrapped resource access point Flux where the
		 * decision is enforced. I.e., access is granted or denied, and all constraints
		 * are handled.
		 * 
		 * @param <T>                 event type
		 * @param decision            a decision
		 * @param resourceAccessPoint a Flux to be protected
		 * @param clazz               the class of the event type
		 * @return a Flux where the decision is enforced.
		 */
		//public <T> Flux<T> enforceConstraintsOfDecisionOnResourceAccessPoint(AuthorizationDecision decision,
		//Flux<T> resourceAccessPoint, Class<T> clazz)
		//{
		//	var wrapped = resourceAccessPoint;
		//	wrapped = replaceIfResourcePresent(wrapped, decision.getResource(), clazz);
		//	try
		//	{
		//		return reactiveTypeBundleFor(decision, clazz).wrap(wrapped);
		//	}
		//	catch (AccessDeniedException e)
		//	{
		//		return Flux.error(e);
		//	}
		//}

		/**
		 * @param <T>      event type
		 * @param decision a decision
		 * @param clazz    class of the event type
		 * @return a ReactiveTypeConstraintHandlerBundle with handlers for all
		 *         constraints in the decision, or throws AccessDeniedException, if
		 *         bundle cannot be constructed.
		 */
		//		public ReactiveTypeConstraintHandlerBundle<T> reactiveTypeBundleFor<T>(AuthorizationDecision decision)
		////				Class<T> clazz)
		//		{
		//			var unhandledObligations = new HashSet<JToken>(decision.Obligations);

		//			// @formatter:off
		//			var bundle = new ReactiveTypeConstraintHandlerBundle<T>(
		//					runnableHandlersForSignal(Signal.ON_DECISION, decision, unhandledObligations),
		//					runnableHandlersForSignal(Signal.ON_CANCEL, decision, unhandledObligations),
		//					runnableHandlersForSignal(Signal.ON_COMPLETE, decision, unhandledObligations),
		//					runnableHandlersForSignal(Signal.ON_TERMINATE, decision, unhandledObligations),
		//					runnableHandlersForSignal(Signal.AFTER_TERMINATE, decision, unhandledObligations));
		//					//subscriptionHandlers(decision, unhandledObligations),
		//					//requestHandlers(decision, unhandledObligations),
		//					//onNextHandlers(decision, unhandledObligations, clazz),
		//					//mapNextHandlers(decision, unhandledObligations, clazz),
		//					//onErrorHandlers(decision, unhandledObligations),
		//					//mapErrorHandlers(decision, unhandledObligations),
		//					//filterConstraintHandlers(decision, unhandledObligations),
		//					//methodInvocationHandlers(decision, unhandledObligations));
		//			// @formatter:on

		//			if (unhandledObligations.Count > 0)
		//			{
		//				//throw new AccessDeniedException("No handler for obligation: " + unhandledObligations);
		//				throw new Exception("No handler for obligation: " + unhandledObligations);
		//			}

		//			return bundle;
		//		}

		public ConstraintHandlerBundle BundleFor(AuthorizationDecision decision)
		{
			var unhandledObligations = new HashSet<JToken>();
			if (decision.Obligations != null)
				unhandledObligations = new HashSet<JToken>(decision.Obligations);

			// @formatter:off
			var bundle = new ConstraintHandlerBundle(
					RunnableHandlersForSignal(Signal.ON_DECISION, decision, unhandledObligations),
					RunnableHandlersForSignal(Signal.ON_CANCEL, decision, unhandledObligations),
					RunnableHandlersForSignal(Signal.ON_COMPLETE, decision, unhandledObligations),
					RunnableHandlersForSignal(Signal.ON_TERMINATE, decision, unhandledObligations),
					RunnableHandlersForSignal(Signal.AFTER_TERMINATE, decision, unhandledObligations));
					//ConsumerHandlers(decision, unhandledObligations));
			//subscriptionHandlers(decision, unhandledObligations),
			//requestHandlers(decision, unhandledObligations),
			//onNextHandlers(decision, unhandledObligations, clazz),
			//mapNextHandlers(decision, unhandledObligations, clazz),
			//onErrorHandlers(decision, unhandledObligations),
			//mapErrorHandlers(decision, unhandledObligations),
			//filterConstraintHandlers(decision, unhandledObligations),
			//methodInvocationHandlers(decision, unhandledObligations));
			// @formatter:on

			if (unhandledObligations.Count > 0)
			{
				//throw new AccessDeniedException("No handler for obligation: " + unhandledObligations);
				throw new Exception("No handler for obligation: " + String.Join(",", unhandledObligations));
			}

			return bundle;
		}

		private Action<T> ConsumerHandlers<T>(AuthorizationDecision decision, HashSet<JToken> unhandledObligations)
		{
			var obligationHandlers = Obligation(ConstructConsumerHandlersForConstraints<T>(decision.Obligations,
					c => unhandledObligations.Remove(c),
					typeof(T)));

			var adviceHandlers = Advice(ConstructConsumerHandlersForConstraints<T>(decision.Advice,
					_ => { },
					typeof(T)));

			return ConsumeWithBoth(obligationHandlers, adviceHandlers);
		}

		//@SuppressWarnings("unchecked") // false positive. "support()" does check
		private Action<T> ConstructConsumerHandlersForConstraints<T>(JArray? constraints, Action<JToken> onHandlerFound, Type type)
		{
			Action<T> handlers = _ => { };

			if (constraints == null)
				return handlers;

			foreach (var constraint in constraints)
			{
				foreach (var provider in globalConsumerProviders)
				{
					if (provider.Supports(type) && provider.IsResponsible(constraint))
					{
						onHandlerFound.Invoke(constraint);
						handlers = ConsumeWithBoth(handlers, provider.GetHandler(constraint));
					}
				}
			}

			return handlers;
		}

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

		private Action ConstructRunnableHandlersForConstraints(Signal signal, JArray? constraints,
							Action<JToken> onHandlerFound)
		{
			Action handlers = () => { };

			if (constraints == null)
				return handlers;


			foreach (var constraint in constraints)
			{
				foreach (KeyValuePair<Signal, List<IRunnableConstraintHandlerProvider>> kvp in globalRunnableIndex)
				{
					if (kvp.Key.Equals(signal))
					{
						foreach (var provider in kvp.Value)
						{
							if (provider.IsResponsible((JToken) constraint))
							{
								onHandlerFound.Invoke(constraint);
								handlers = RunBoth(handlers, provider.GetHandler(constraint));
							}
						}
					}

				}
			}
			return handlers;
		}

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
					//Exceptions.throwIfFatal(t);
					//throw new AccessDeniedException("Failed to execute runnable obligation handler", t);
					throw new Exception("Failed to execute runnable obligation handler", t);
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
					throw new Exception("Failed to execute runnable advice handler", t);
					//Exceptions.throwIfFatal(t);
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
					throw new Exception("Failed to execute obligation handler " + s.ToString(), t);
					//Exceptions.throwIfFatal(t);
					//throw new AccessDeniedException("Failed to execute obligation handler " + s.getClass().getSimpleName(),
					//		t);
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
					throw new Exception("Failed to execute runnable advice handler", t);
					//Exceptions.throwIfFatal(t);
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

		private Action<T> ConsumeWithBoth<T>(Action<T> a, Action<T> b)
		{
			return (t) =>
			{
				a.Invoke(t);
				b.Invoke(t);
			};
		}

		private Action<T> ConsumeWithBoth<T>(Action<T> a, Action<object> b)
		{
			return (t) =>
			{
				a.Invoke(t);
				b.Invoke(t);
			};
		}

		public void RunConstraint()
		{
//			constructRunnableHandlersForConstraints
		}
	}
}

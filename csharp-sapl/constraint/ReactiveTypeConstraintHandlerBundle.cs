using System;
using System.Collections.Generic;
using System.Text;

namespace csharp.sapl.constraint
{
	/*
	 * Copyright © 2017-2022 Dominic Heutelbeck (dominic@heutelbeck.com)
	 *
	 * Licensed under the Apache License, Version 2.0 (the "License");
	 * you may not use this file except in compliance with the License.
	 * You may obtain a copy of the License at
	 *
	 *     http://www.apache.org/licenses/LICENSE-2.0
	 *
	 * Unless required by applicable law or agreed to in writing, software
	 * distributed under the License is distributed on an "AS IS" BASIS,
	 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	 * See the License for the specific language governing permissions and
	 * limitations under the License.
	 */

	/**
	 * 
	 * This bundle aggregates all constraint handlers for a specific decision which
	 * are useful in a reactive scenario.
	 * 
	 * 
	 * @author Dominic Heutelbeck
	 *
	 * @param <T> return type
	 */
	public class ReactiveTypeConstraintHandlerBundle<T>
	{

		// @formatter:off
		private static readonly Action NOP = () => { };

		private Action onDecisionHandlers = NOP;
		private Action onCancelHandlers = NOP;
		private Action onCompleteHandlers = NOP;
		private Action onTerminateHandlers = NOP;
		private Action afterTerminateHandlers = NOP;
		//private Action<Subscription> onSubscribeHandlers = __=>{};
		//private LongConsumer onRequestHandlers = __=>{};
		//private Action<T> doOnNextHandlers = __=>{ };
		//private Function<T, T> onNextMapHandlers = x=>x;
		//private Consumer<Throwable> doOnErrorHandlers = __=>{ };
		//private Function<Throwable, Throwable> onErrorMapHandlers = x=>x;
		//private Predicate<Object> filterPredicateHandlers = __=>true;
		//private Consumer<MethodInvocation> methodInvocationHandlers = __=>{ };
		// @formatter:on

		public ReactiveTypeConstraintHandlerBundle()
		{

		}

		public ReactiveTypeConstraintHandlerBundle(Action onDecisionHandlers, Action onCancelHandlers, Action onCompleteHandlers,
					Action onTerminateHandlers, Action afterTerminateHandlers)
		{
			this.onDecisionHandlers = onDecisionHandlers;
			this.onCancelHandlers = onCancelHandlers;
			this.onCompleteHandlers = onCompleteHandlers;
			this.onTerminateHandlers = onTerminateHandlers;
			this.afterTerminateHandlers = afterTerminateHandlers;
		}

		/**
		 * Runs all onSubscription handlers.
		 * 
		 * @param s the Subscription.
		 */
		//public void handleOnSubscribeConstraints(Subscription s)
		//{
		//	onSubscribeHandlers.accept(s);
		//}

		/**
		 * Executes all onNext constraint handlers, potentially transforming the value.
		 * 
		 * @param value a return value
		 * @return the return value after constraint handling
		 */
		//public T handleAllOnNextConstraints(T value)
		//{
		//	handleOnNextConstraints(value);
		//	return handleOnNextMapConstraints(value);
		//}

		//private T handleOnNextMapConstraints(T value)
		//{
		//	return onNextMapHandlers.apply(value);
		//}

		//private void handleOnNextConstraints(T value)
		//{
		//	doOnNextHandlers.accept(value);
		//}

		/**
		 * Runs all onRequest handlers.
		 * 
		 * @param value number of events requested
		 */
		//public void handleOnRequestConstraints(Long value)
		//{
		//	onRequestHandlers.accept(value);
		//}

		/**
		 * Runs all onComplete handlers.
		 */
		public void handleOnCompleteConstraints()
		{
			onCompleteHandlers.Invoke();
		}

		/**
		 * Runs all onTerminate handlers.
		 */
		public void handleOnTerminateConstraints()
		{
			onTerminateHandlers.Invoke();
		}

		/**
		 * Runs all onDecision handlers.
		 */
		public void handleOnDecisionConstraints()
		{
			onDecisionHandlers.Invoke();
		}

		/**
		 * Runs all afterTerminate handlers.
		 */
		public void handleAfterTerminateConstraints()
		{
			afterTerminateHandlers.Invoke();
		}

		/**
		 * Runs all onCancel handlers.
		 */
		public void handleOnCancelConstraints()
		{
			onCancelHandlers.Invoke();
		}

		/**
		 * Runs all method invocation handlers. These handlers may modify the
		 * methodInvocation.
		 * 
		 * @param methodInvocation
		 */
		//public void handleMethodInvocationHandlers(MethodInvocation methodInvocation)
		//{
		//	methodInvocationHandlers.accept(methodInvocation);
		//}

		/**
		 * Executes all onError constraint handlers, potentially transforming the error.
		 *
		 * @param error the error
		 * @return the error after all handlers have run
		 */
		//public Throwable handleAllOnErrorConstraints(Throwable error)
		//{
		//	handleOnErrorConstraints(error);
		//	return handleOnErrorMapConstraints(error);
		//}

		//private Throwable handleOnErrorMapConstraints(Throwable error)
		//{
		//	return onErrorMapHandlers.apply(error);
		//}

		//private void handleOnErrorConstraints(Throwable error)
		//{
		//	doOnErrorHandlers.accept(error);
		//}

		/**
		 * Wires the handlers into the matching reactive signals.
		 * 
		 * @param resourceAccessPoint a reactive resource access point
		 * @return the resource access point with the different handlers wired to their
		 *         respective hooks.
		 */
		//public Flux<T> wrap(Flux<T> resourceAccessPoint)
		//{
		//	var wrapped = resourceAccessPoint.doOnRequest(onRequestHandlers).doOnSubscribe(onSubscribeHandlers)
		//			.filter(filterPredicateHandlers).onErrorMap(onErrorMapHandlers).doOnError(doOnErrorHandlers)
		//			.map(onNextMapHandlers).doOnNext(doOnNextHandlers).doOnCancel(onCancelHandlers)
		//			.doOnComplete(onCompleteHandlers).doOnTerminate(onTerminateHandlers)
		//			.doAfterTerminate(afterTerminateHandlers);
		//	return onDecision(onDecisionHandlers).thenMany(wrapped);
		//}

		//private Mono<Void> onDecision(Runnable handlers)
		//{
		//	return Mono.fromRunnable(handlers);
		//}

}

}

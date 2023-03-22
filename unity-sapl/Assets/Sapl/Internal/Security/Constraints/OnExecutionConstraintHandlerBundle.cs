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
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Sapl.Internal.Security.Constraints
{
	///<summary>
	///
	/// This bundle aggregates all constraint handlers for a specific decision which
	/// are useful to execute OnExecution.
	///
	///</summary>
	public class OnExecutionConstraintHandlerBundle
	{
		private static readonly Action NOP = () => { };

		private readonly Action runnableHandlers = NOP;
		private readonly Action<GameObject> gameObjectConsumerHandlers = _ => { };
		private readonly Action<JToken> jsonConsumerHandlers = _ => { };
		private readonly Func<Boolean> boolFunctionHandlers = () => { return true; };

        /// <summary>Initializes a new instance of the <see cref="OnExecutionConstraintHandlerBundle" /> class.</summary>
        public OnExecutionConstraintHandlerBundle()
		{

		}

        /// <summary>Initializes a new instance of the <see cref="OnExecutionConstraintHandlerBundle" /> class.</summary>
        /// <param name="runnableHandlers">The runnable handlers.</param>
        /// <param name="gameObjectConsumerHandlers">The game object consumer handlers.</param>
        /// <param name="jsonConsumerHandlers">The json consumer handlers.</param>
        public OnExecutionConstraintHandlerBundle(Action runnableHandlers, Action<GameObject> gameObjectConsumerHandlers,
					Action<JToken> jsonConsumerHandlers)
		{
			this.runnableHandlers = runnableHandlers;
			this.gameObjectConsumerHandlers = gameObjectConsumerHandlers;
			this.jsonConsumerHandlers = jsonConsumerHandlers;
		}

        /// <summary>Initializes a new instance of the <see cref="OnExecutionConstraintHandlerBundle" /> class.</summary>
        /// <param name="runnableHandlers">The runnable handlers.</param>
        /// <param name="gameObjectConsumerHandlers">The game object consumer handlers.</param>
        /// <param name="jsonConsumerHandlers">The json consumer handlers.</param>
        /// <param name="boolFunctionHandlers">The bool function handlers.</param>
        public OnExecutionConstraintHandlerBundle(Action runnableHandlers, Action<GameObject> gameObjectConsumerHandlers,
					Action<JToken> jsonConsumerHandlers, Func<Boolean> boolFunctionHandlers)
		{
			this.runnableHandlers = runnableHandlers;
			this.gameObjectConsumerHandlers = gameObjectConsumerHandlers;
			this.jsonConsumerHandlers = jsonConsumerHandlers;
			this.boolFunctionHandlers = boolFunctionHandlers;
		}


		///<summary>
		/// Runs all runnable handlers.
		///</summary>
		public void HandleRunnableHandlers()
		{
			runnableHandlers.Invoke();
		}

		///<summary>
		/// Runs all <see cref="GameObject"/> consumer handlers.
		///</summary>
		///<param name="gameObject"></param>
		public void HandleGameObjectConsumerHandlers(GameObject gameObject)
		{
			gameObjectConsumerHandlers.Invoke(gameObject);
		}

		///<summary>
		/// Runs all JSON consumer handlers.
		///</summary>
		///<param name="json"></param>
		public void HandleJsonConsumerHandlers(JToken json)
		{
			jsonConsumerHandlers.Invoke(json);
		}

		///<summary>
		/// Runs all Boolean Function handlers.
		///</summary>
		public Boolean HandleBoolFunctionHandlers()
		{
			return boolFunctionHandlers();
		}
	}

}

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
using csharp.sapl.constraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace csharp.sapl.pdp.api
{
	public class AuthorizationProvider : IObservable<AuthorizationDecision>, IAuthorizationProvider
	{
		private List<IObserver<AuthorizationDecision>> observers;
		private IManagePDPConnection connectionManager;
		private static readonly object padlock = new object();
		private static AuthorizationProvider current;
		private bool initialized;

		private AuthorizationProvider() { }

		public static AuthorizationProvider Current
		{
			get
			{
				lock (padlock)
				{
					if (current == null)
					{
						current = new AuthorizationProvider();
					}
					return current;
				}
			}
		}

		void IAuthorizationProvider.InitializeConnection(IManagePDPConnection connectionManager)
		{
			if (!initialized)
			{
				this.connectionManager = connectionManager;
				observers = new List<IObserver<AuthorizationDecision>>();
				initialized = true;
			}
		}
		public IDisposable Subscribe(IObserver<AuthorizationDecision> observer)
		{
			if (!observers.Contains(observer))
			{
				observers.Add(observer);
			}
			return new Unsubscriber(observers, observer);
		}
		void IAuthorizationProvider.TrackDecision(AuthorizationSubscription subscription, IObserver<AuthorizationDecision> observer, string relativeUri)
		{
			if (subscription == null)
			{
				observer.OnError(new AuthorizationDecisionUnknownException());
			}
			else
			{
				connectionManager.SubscribeDecision(subscription, observer, relativeUri);
			}
		}

		void IAuthorizationProvider.TrackDecisionOnce(AuthorizationSubscription subscription, IObserver<AuthorizationDecision> observer, string relativeUri)
		{
			if (subscription == null)
			{
				observer.OnError(new AuthorizationDecisionUnknownException());
			}
			else
			{
				connectionManager.SubscribeDecisionOnce(subscription, observer, relativeUri);
			}
		}

		public AuthorizationPublisher GetPublisher(string subscriptionId)
		{
			if (observers.Any())
			{
				return observers.Cast<AuthorizationPublisher>()
					.FirstOrDefault(o => o.SubscriptionId.Equals(subscriptionId));
			}
			return null;
		}

		public AuthorizationPublisher GetPublisher(AuthorizationSubscription subscription)
		{
			if (observers.Any())
			{
				return observers.Cast<AuthorizationPublisher>()
					.FirstOrDefault(o => o.Subscription.Equals(subscription));
			}
			return null;
		}

		public void EndTransmission()
		{
			foreach (var observer in observers.ToArray())
				if (observers.Contains(observer))
					observer.OnCompleted();

			observers.Clear();
		}
	}

	internal class Unsubscriber : IDisposable
	{
		private List<IObserver<AuthorizationDecision>> observers;
		private IObserver<AuthorizationDecision> _observer;

		public Unsubscriber(List<IObserver<AuthorizationDecision>> observers, IObserver<AuthorizationDecision> observer)
		{
			this.observers = observers;
			this._observer = observer;
		}

		public void Dispose()
		{
			if (_observer != null && observers.Contains(_observer))
			{
				observers.Remove(_observer);
			}
		}
	}
}

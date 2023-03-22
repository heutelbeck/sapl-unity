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
using csharp.sapl.pdp.api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace csharp.sapl.constraint
{
	public class MultiAuthorizationProvider : IObservable<MultiAuthorizationDecision>
	{
		private List<MultiAuthorizationPublisher> observers;
		private IManagePDPConnection connectionManager;

		public MultiAuthorizationProvider(IManagePDPConnection connectionManager)
		{
			this.connectionManager = connectionManager;
			observers = new List<MultiAuthorizationPublisher>();
		}
		public IDisposable Subscribe(MultiAuthorizationPublisher observer)
		{
			if (!observers.Contains(observer))
				observers.Add(observer);
			return new Unsubscriber(observers, observer);
		}
		public void TrackDecisions(MultiAuthorizationSubscription subscription, string relativeUri)
		{
			foreach (var observer in observers)
			{
				if (subscription == null)
				{
					observer.OnError(new AuthorizationDecisionUnknownException());
				}
				else
				{
					connectionManager.SubscribeAllDecisions(subscription, observer, relativeUri);
				}
			}
		}
		public void TrackIdentifiableDecisions(MultiAuthorizationSubscription subscription, string relativeUri)
		{
			foreach (var observer in observers)
			{
				if (subscription == null)
				{
					observer.OnError(new AuthorizationDecisionUnknownException());
				}
				else
				{
					connectionManager.SubscribeAllIdentifiableDecisions(subscription, observer, relativeUri);

				}
			}
		}

		public void TrackIdentifiableDecisionsOnce(MultiAuthorizationSubscription subscription, string relativeUri)
		{
			foreach (var observer in observers)
			{
				if (subscription == null)
				{
					observer.OnError(new AuthorizationDecisionUnknownException());
				}
				else
				{
					connectionManager.SubscribeAllIdentifiableDecisionsOnce(subscription, observer, relativeUri);

				}
			}
		}

		public void EndTransmission()
		{
			foreach (var observer in observers.ToArray())
				if (observers.Contains(observer))
					observer.OnCompleted();

			observers.Clear();
		}

		public IDisposable Subscribe(IObserver<MultiAuthorizationDecision> observer)
		{
			if (!observers.Contains(observer))
				observers.Add((MultiAuthorizationPublisher)observer);
			return new Unsubscriber(observers, (MultiAuthorizationPublisher)observer);
		}

	}

	internal class Unsubscriber : IDisposable
	{
		private List<MultiAuthorizationPublisher> observers;
		private MultiAuthorizationPublisher _observer;

		public Unsubscriber(List<MultiAuthorizationPublisher> observers, MultiAuthorizationPublisher observer)
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

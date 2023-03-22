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
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace csharp.sapl.pdp.api
{
	public class AuthorizationPublisher : IObserver<AuthorizationDecision>, INotifyDecisionChanged, IAuthorizationPublisher
	{
		private IDisposable unsubscriber;
		private AuthorizationSubscription subscription;
		private AuthorizationDecision authorizationDecision;
		private string subscriptionId;
		private Exception error;

		public Exception Error => error;
		public AuthorizationProvider Provider { get; private set; }

		public AuthorizationDecision AuthorizationDecision => authorizationDecision;
		public string SubscriptionId => subscriptionId;
		public AuthorizationSubscription Subscription => subscription;

		public AuthorizationPublisher(AuthorizationSubscription subscription)
		{
			this.subscription = subscription;
			this.authorizationDecision = new AuthorizationDecision(Decision.INDETERMINATE);
		}

		public AuthorizationPublisher(string subscriptionId)
		{
			this.subscriptionId = subscriptionId;
			this.authorizationDecision = new AuthorizationDecision(Decision.INDETERMINATE);
		}
#nullable enable
		public virtual void Subscribe(IObservable<AuthorizationDecision>? provider)
		{
			if (provider != null)
			{
				Provider = (AuthorizationProvider)provider;
				unsubscriber = provider.Subscribe(this);
			}
		}
#nullable disable
		public virtual void OnCompleted()
		{
			Unsubscribe();
		}

		public virtual void OnError(Exception error)
		{
			this.authorizationDecision = new AuthorizationDecision(Decision.NOT_APPLICABLE);
			this.error = error;
			OnPropertyChanged(nameof(AuthorizationDecision));
		}

		public virtual void OnNext(AuthorizationDecision value)
		{
			this.authorizationDecision = value;
			OnPropertyChanged(nameof(AuthorizationDecision));
		}

		public virtual void Unsubscribe()
		{
			unsubscriber.Dispose();
		}

		#region implements INotifyPropertyChanged

		public event PropertyChangedEventHandler PropertyChanged;
#nullable enable
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}
#nullable disable
		#endregion

		public INotifyDecisionChanged AuthorizationDecisionBusinessObject => authorizationDecision;
	}
}

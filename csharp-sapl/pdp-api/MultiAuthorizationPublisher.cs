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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace csharp.sapl.pdp.api
{
	public class MultiAuthorizationPublisher : IObserver<MultiAuthorizationDecision>, INotifyDecisionChanged, IAuthorizationPublisher
	{
		#region private fields

		private Exception error;
		private MultiAuthorizationSubscription multiAuthzSubscription;
		private MultiAuthorizationDecision multiAuthorizationDecision;

		#endregion

		#region properties

		public MultiAuthorizationDecision MultiAuthorizationDecision => multiAuthorizationDecision;
		public MultiAuthorizationSubscription MultiAuthzSubscription => multiAuthzSubscription;
		public Exception Error => error;

		#endregion



		public MultiAuthorizationPublisher(MultiAuthorizationSubscription multiAuthzSubscription)
		{
			this.multiAuthzSubscription = multiAuthzSubscription;
			this.multiAuthorizationDecision = MultiAuthorizationDecision.Indeterminate();
			OnPropertyChanged(nameof(MultiAuthzSubscription));
		}

		public void OnCompleted()
		{
			throw new NotImplementedException();
		}

		public void OnError(Exception error)
		{
			this.error = error;
		}

		public void OnNext(MultiAuthorizationDecision value)
		{
			this.multiAuthorizationDecision = value;
			OnPropertyChanged(nameof(MultiAuthorizationDecision));

			UpdateSinglePublisher();
		}

		public void OnNext(IdentifiableAuthorizationDecision value)
		{
			if (multiAuthorizationDecision == null)
			{
				this.multiAuthorizationDecision = new MultiAuthorizationDecision();
			}
			this.multiAuthorizationDecision.SetAuthorizationDecisionForSubscriptionWithId(value.AuthorizationSubscriptionId, value.AuthorizationDecision);
			OnPropertyChanged(nameof(MultiAuthorizationDecision));
			UpdateSinglePublisher();
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
			if (propertyName.Equals(nameof(MultiAuthorizationDecision)))
			{
				MultiAuthorizationDecision.UpdateDecisions();
			}
		}

		protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		private void UpdateSinglePublisher()
		{
			foreach (var authorizationDecision in this.MultiAuthorizationDecision.AuthorizationDecisions)
			{
				IObserver<AuthorizationDecision> publisher = new AuthorizationPublisher(authorizationDecision.Key);
				publisher.OnNext(authorizationDecision.Value);
			}
		}


		public INotifyDecisionChanged AuthorizationDecisionBusinessObject => this.MultiAuthorizationDecision;
	}
}

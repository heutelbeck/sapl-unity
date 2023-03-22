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
using System.IO;
using csharp.sapl.pdp.api;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;

namespace pdp_remote_test
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		AuthorizationSubscription authorizationSubscription;
		private AuthorizationPublisher authorizationPublisher;

		public MainWindow()
		{
			InitializeComponent();
			CreateSubscription();
			connectButton.IsEnabled = true;
			sendButton.IsEnabled = true;
		}

		private void CreateSubscription()
		{
			authorizationSubscription = new AuthorizationSubscription(
						JValue.CreateString("ANNA"), JValue.CreateString("Move"), JValue.CreateString("GameObject"), JValue.CreateString("null")
					 );
			var serialized = Newtonsoft.Json.JsonConvert.SerializeObject(authorizationSubscription);
			messageTextBox.Text = serialized.ToLower();
			userTextBox.Text = "No Function yet";
			RemotePolicyDecisionPoint decisionPoint = new RemotePolicyDecisionPoint();
			this.authorizationPublisher = (AuthorizationPublisher)decisionPoint.Decide(authorizationSubscription);
			this.DataContext = authorizationPublisher;
		}

		private void ConnectButton_Click(object sender, RoutedEventArgs e)
		{
			messageTextBox.Text = String.Empty;
			sendButton.IsEnabled = true;
		}

		private void SendButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				authorizationSubscription = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorizationSubscription>(messageTextBox.Text);
				RemotePolicyDecisionPoint decisionPoint = new RemotePolicyDecisionPoint();
				this.authorizationPublisher = (AuthorizationPublisher)decisionPoint.DecideOnce(authorizationSubscription);
				//if (isOnce.IsChecked.HasValue && isOnce.IsChecked.Value)
				//{
				//    this.authorizationPublisher = (AuthorizationPublisher)decisionPoint.DecideOnce(authorizationSubscription);
				//}
				//else
				//{
				//    this.authorizationPublisher = (AuthorizationPublisher)decisionPoint.Decide(authorizationSubscription);
				//}
				this.DataContext = authorizationPublisher;
			}
			catch (Exception exception)
			{

				throw new InvalidDataException(exception.Message);
			}
			sendButton.IsEnabled = false;
		}

		private void MessageTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
		{
			sendButton.IsEnabled = true;
		}
	}
}

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
using System.Windows;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Windows.Controls;
using System;

namespace pdp_remote_multi_test_deicide_all_test
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		MultiAuthorizationSubscription authorizationSubscription;
		private MultiAuthorizationPublisher authorizationPublisher;
		public MainWindow()
		{
			InitializeComponent();
			CreateSubscription();
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
				authorizationSubscription = Newtonsoft.Json.JsonConvert.DeserializeObject<MultiAuthorizationSubscription>(messageTextBox.Text);
				RemotePolicyDecisionPoint decisionPoint = new RemotePolicyDecisionPoint();
				this.authorizationPublisher = (MultiAuthorizationPublisher)decisionPoint.DecideAll(authorizationSubscription);
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

		private void CreateSubscription()
		{
			authorizationSubscription = new MultiAuthorizationSubscription();
			authorizationSubscription.AddAuthorizationSubscription("id1", "Robert", "Move", "GameObject", null);
			authorizationSubscription.AddAuthorizationSubscription("id2", "Manfred", "Move", "GameObject", null);
			authorizationSubscription.AddAuthorizationSubscription("id3", "Simon", "Move", "GameObject", null);
			//messageTextBox.Text = Newtonsoft.Json.JsonConvert.DeserializeObject<MultiAuthorizationSubscription>(messageTextBox.Text);
			var serialized = JsonConvert.SerializeObject(authorizationSubscription);
			messageTextBox.Text = serialized;
			userTextBox.Text = "No Function yet";
			connectButton.IsEnabled = false;
			sendButton.IsEnabled = true;
		}
	}
}

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using csharp.sapl.pdp.api;
using sandbox_simon_r.Observer_Pattern.SAPL;
using System.Reactive.Linq;
using System.Reactive;
using System.Reactive.Disposables;

namespace playground
{
	class Program
	{
		public Action TestAction()
		{
			return () => Console.WriteLine("HUHU");
		}
		// Das Exchangeable wird ein AuthorizaionSubsriptionMonitor
		static void Mainoooo(string[] args)
		{
			int sleepTime = 1000;
			AuthorizationDecision tempData = AuthorizationDecision.DENY;

			IObservable<AuthorizationDecision> source = Observable.Create<AuthorizationDecision>
			(
				(IObserver<AuthorizationDecision> observer) =>
				{					
					observer.OnNext(tempData);

					while (true)
					{
						tempData = new AuthorizationDecision(Decision.PERMIT);
						observer.OnNext(tempData);

						System.Threading.Thread.Sleep(sleepTime);
						tempData = new AuthorizationDecision(Decision.DENY);
						observer.OnNext(tempData);

						System.Threading.Thread.Sleep(sleepTime);
						tempData = new AuthorizationDecision(Decision.NOT_APPLICABLE);
						observer.OnNext(tempData);

						System.Threading.Thread.Sleep(sleepTime);
						tempData = new AuthorizationDecision(Decision.INDETERMINATE);
						observer.OnNext(tempData);
					}



					//observer.OnCompleted(Console.WriteLine("End of Subscription."));
					return Disposable.Empty;
				}
			);

			IObserver<AuthorizationDecision> observer = Observer.Create<AuthorizationDecision>
				(
					(value) => Console.WriteLine("New Decision from PDP: {0}", value.Decision + "\n")
				);

			source.Subscribe(observer);


			//IObservable<int> source = Observable.Range(1, 10);
			//IObserver<int> obsvr = Observer.Create<int>(
			//	x => Console.WriteLine("OnNext: {0}", x),
			//	ex => Console.WriteLine("OnError: {0}", ex.Message),
			//	() => Console.WriteLine("OnCompleted"));
			//IDisposable subscription = source.Subscribe(obsvr);
			//Console.WriteLine("Press ENTER to unsubscribe...");
			//Console.ReadLine();
			//subscription.Dispose();

			//TemperatureMonitor monitor = new TemperatureMonitor();
			//TemperatureReporter reporter1 = new TemperatureReporter();
			//TemperatureReporter reporter2 = new TemperatureReporter();

			//reporter1.Subscribe(monitor);
			//reporter2.Subscribe(monitor);

			//monitor.GetTemperature();

			//AuthorizationDecisionStream decisionStream = new AuthorizationDecisionStream();
			//AuthorizationDecisionConsumer decisionConsumer = new AuthorizationDecisionConsumer();
			//decisionConsumer.Subscribe(decisionStream);

			//decisionStream.mock();

			//JValue name = new JValue("Peter");
			//JValue action = new JValue("klick");
			//JValue resource = new JValue("");
			//JValue environment = new JValue("");
			//AuthorizationSubscription subscription = new AuthorizationSubscription(name, action, resource, environment);
			//string sub = JsonConvert.SerializeObject(subscription);
			//Console.WriteLine(sub);

			//Console.WriteLine(subscription.ToString());

			//string decision = "{\"decision\":\"PERMIT\"}";
			//Console.WriteLine(decision);

			//JObject jdecision = JsonConvert.DeserializeObject<JObject>(decision);
			//Console.WriteLine(jdecision.ToString());

			//Console.WriteLine("Hello World!");

			//Product product = new Product();
			//product.Name = "Apple";
			//product.ExpiryDate = new DateTime(2008, 12, 28);
			//product.Price = 4;
			//product.Sizes = new string[] { "Small", "Medium", "Large" };

			//string output = JsonConvert.SerializeObject(product);
			////{
			////  "Name": "Apple",
			////  "ExpiryDate": "2008-12-28T00:00:00",
			////  "Price": 3.99,
			////  "Sizes": [
			////    "Small",
			////    "Medium",
			////    "Large"
			////  ]
			////}

			//Console.WriteLine(output);

			//Product deserializedProduct = JsonConvert.DeserializeObject<Product>(output);
		}
	}
}

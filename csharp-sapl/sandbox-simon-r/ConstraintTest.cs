using System;
using System.Collections.Generic;
using System.Text;
using csharp.sapl.constraint;
using csharp.sapl.constraint.api;
using csharp.sapl.pdp.api;
using Newtonsoft.Json.Linq;

namespace sandbox_simon_r
{
	class ConstraintTest
	{
		private static void Main(string[] args)
		{
			// Runnable Constraint Handler erzeugen
			LoggingConstraint logging = new LoggingConstraint();
			HelloWorldConstraint hello = new HelloWorldConstraint();
			// Liste om Typ "IRunnableConstraintHandlerProvider" anlegen und Handler hinzufügen
			List<IRunnableConstraintHandlerProvider> runnableList = new List<IRunnableConstraintHandlerProvider>();
			runnableList.Add(logging);
			runnableList.Add(hello);

			// Consumer Constraint Handler erzeugen
			ConsumerOfIntConstraint intConsumer = new ConsumerOfIntConstraint();
			// Liste om Typ "IConsumerConstraintHandlerProvider" anlegen und Handler hinzufügen
			List<IConsumerConstraintHandlerProvider<object>> consumerList = new List<IConsumerConstraintHandlerProvider<object>>();
			consumerList.Add(intConsumer);

			// Authorization Decision anlegen
			JArray jArray = new JArray("logging");
			jArray.Add(JToken.FromObject("hello"));
			//jArray.Add(JToken.FromObject("IntConsumer"));
			AuthorizationDecision decision = new AuthorizationDecision().withObligations(jArray);

			//// Alle Runnable Handler im ConstraintEnforcementService anmelden
			//ConstraintEnforcementService enforcementService = new ConstraintEnforcementService(runnableList);

			//// ConstraintHandlerBundle für DIESE Decision erzeugen
			//ConstraintHandlerBundle bundle = enforcementService.BundleFor(decision);

			//// Alle verbundenen Runnable Handler aus dem Bundle ausführen
			//bundle.handleOnDecisionConstraints();
			//bundle.handleOnCompleteConstraints();
			//bundle.handleOnCancelConstraints();
			//bundle.handleOnTerminateConstraints();
			//bundle.handleAfterTerminateConstraints();
		}
	}
}

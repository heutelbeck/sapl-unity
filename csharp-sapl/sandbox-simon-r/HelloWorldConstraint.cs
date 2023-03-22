using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using csharp.sapl.constraint.api;
using csharp.sapl.pdp.api;
using Newtonsoft.Json.Linq;

namespace sandbox_simon_r
{
	class HelloWorldConstraint : IRunnableConstraintHandlerProvider
	{
		private static readonly IRunnableConstraintHandlerProvider.Signal signal = IRunnableConstraintHandlerProvider.Signal.ON_DECISION;
		private static readonly JToken constraintToken = JToken.FromObject("hello");
		private static readonly Action action = HelloWorld;

		public Action GetHandler(JToken constraint)
		{
			return action;
		}

		public IRunnableConstraintHandlerProvider.Signal GetSignal()
		{
			return signal;
		}

		public bool IsResponsible(JToken constraint)
		{
			return constraint.Equals(constraintToken);
		}

		private static void HelloWorld()
		{
			Console.WriteLine("Hello World!");
		}
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using csharp.sapl.constraint.api;
using csharp.sapl.pdp.api;
using Newtonsoft.Json.Linq;

namespace sandbox_simon_r
{
	class LoggingConstraint : IRunnableConstraintHandlerProvider
	{
		private static readonly IRunnableConstraintHandlerProvider.Signal signal = IRunnableConstraintHandlerProvider.Signal.ON_DECISION;
		private static readonly JToken constraintToken = JToken.FromObject("logging");
		private static readonly Action action = logging;

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

		private static void logging()
		{
			var culture = new CultureInfo("de-DE");
			Console.WriteLine("{0}", DateTime.Now.ToString(culture));
		}
	}
}

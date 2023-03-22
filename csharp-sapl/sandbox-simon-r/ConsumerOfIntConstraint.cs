using System;
using System.Collections.Generic;
using System.Text;
using csharp.sapl.constraint;
using csharp.sapl.constraint.api;
using Newtonsoft.Json.Linq;

namespace sandbox_simon_r
{
	class ConsumerOfIntConstraint : IConsumerConstraintHandlerProvider<object>
	{
		private static readonly JToken constraintToken = JToken.FromObject("IntConsumer");
		private static readonly Action<object> action = consumeInt;

		public Action<object> GetHandler(JToken constraint)
		{
			return action;
		}

		public bool IsResponsible(JToken constraint)
		{
			return constraint.Equals(constraintToken);
		}

		private static void consumeInt(object value)
		{
			Console.WriteLine("Int Consumer: {0}", value);
		}

        public ISignal.Signal GetSignal()
        {
            throw new NotImplementedException();
        }
    }
}

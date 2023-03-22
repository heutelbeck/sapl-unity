using System;
using System.Collections.Generic;
using System.Text;
using csharp.sapl.pdp.api;

namespace sandbox_simon_r.Observer_Pattern.SAPL
{
    public class AuthorizationDecisionConsumer : IObserver<AuthorizationDecision>
    {
        private IDisposable unsubscriber;
        private AuthorizationDecision lastDecision = null;

        public virtual void Subscribe(IObservable<AuthorizationDecision> provider)
        {
            unsubscriber = provider.Subscribe(this);
        }

        public virtual void Unsubscribe()
        {
            unsubscriber.Dispose();
        }

        public virtual void OnCompleted()
        {
            Console.WriteLine("End of Subscription.");
        }

        public virtual void OnError(Exception error)
        {
            Console.WriteLine(error.Message + "\n");
        }

        public virtual void OnNext(AuthorizationDecision value)
        {
            lastDecision = value;
            Console.WriteLine("New Decision from PDP: {0}", value.Decision + "\n");
        }
    }
}

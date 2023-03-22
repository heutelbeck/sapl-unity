using System;
using System.Collections.Generic;
using csharp.sapl.pdp.api;

namespace sandbox_simon_r.Observer_Pattern.SAPL
{
    public class AuthorizationDecisionStream : IObservable<AuthorizationDecision>
    {
        List<IObserver<AuthorizationDecision>> observers;

        public AuthorizationDecisionStream()
        {
            observers = new List<IObserver<AuthorizationDecision>>();
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<AuthorizationDecision>> _observers;
            private IObserver<AuthorizationDecision> _observer;

            public Unsubscriber(List<IObserver<AuthorizationDecision>> observers, IObserver<AuthorizationDecision> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (!(_observer == null)) _observers.Remove(_observer);
            }
        }

        public IDisposable Subscribe(IObserver<AuthorizationDecision> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber(observers, observer);
        }

        public void mock()
        {
            AuthorizationDecision tempData;
            int sleepTime = 1000;

            tempData = new AuthorizationDecision(Decision.PERMIT);
            foreach (var observer in observers)
                observer.OnNext(tempData);

            System.Threading.Thread.Sleep(sleepTime);
            tempData = new AuthorizationDecision(Decision.DENY);
            foreach (var observer in observers)
                observer.OnNext(tempData);

            System.Threading.Thread.Sleep(sleepTime);
            tempData = new AuthorizationDecision(Decision.NOT_APPLICABLE);
            foreach (var observer in observers)
                observer.OnNext(tempData);

            System.Threading.Thread.Sleep(sleepTime);
            tempData = new AuthorizationDecision(Decision.INDETERMINATE);
            foreach (var observer in observers)
                observer.OnNext(tempData);

            System.Threading.Thread.Sleep(sleepTime);
            foreach (var observer in observers)
                observer.OnError(new Exception("Hier beliebiges Error Handling!"));

            foreach (var observer in observers.ToArray())
                if (observer != null) observer.OnCompleted();

            observers.Clear();
        }
    }
}
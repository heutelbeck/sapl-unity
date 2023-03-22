﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

namespace sandbox_simon_r
{
    class ObserveEvent_Simple
    {
        public static event EventHandler SimpleEvent;

        private static void Main()
        {
            Console.WriteLine("Setup observable");
            // To consume SimpleEvent as an IObservable:
            var eventAsObservable = Observable.FromEventPattern(
                    ev => SimpleEvent += ev,
                    ev => SimpleEvent -= ev);

            // SimpleEvent is null until we subscribe
            Console.WriteLine(SimpleEvent == null ? "SimpleEvent == null" : "SimpleEvent != null");

            Console.WriteLine("Subscribe");
            //Create two event subscribers
            var s = eventAsObservable.Subscribe(args => Console.WriteLine("Received event for s subscriber"));
            var t = eventAsObservable.Subscribe(args => Console.WriteLine("Received event for t subscriber"));

            // After subscribing the event handler has been added
            Console.WriteLine(SimpleEvent == null ? "SimpleEvent == null" : "SimpleEvent != null");

            Console.WriteLine("Raise event");
            if (null != SimpleEvent)
            {
                SimpleEvent(null, EventArgs.Empty);
            }

            // Allow some time before unsubscribing or event may not happen
            Thread.Sleep(100);

            Console.WriteLine("Unsubscribe");
            s.Dispose();
            t.Dispose();

            // After unsubscribing the event handler has been removed
            Console.WriteLine(SimpleEvent == null ? "SimpleEvent == null" : "SimpleEvent != null");

            Console.ReadKey();
        }
    }
}

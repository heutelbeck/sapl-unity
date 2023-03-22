using EvtSource;
using Sandbox.Damian.FTK.Data;
using Sandbox.Damian.FTK.Data.Entities;
using Sandbox.Damian.FTK.Data.Serialization;
using FTK.Network;
using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using static EvtSource.EventSourceReader;

namespace Sandbox.Damian.FTK.Network.Clients
{
    /// <summary>
    /// A base class for REST-API based data providers.
    /// Cache is populated using the REST-API.
    /// </summary>
    /// <typeparam name="T">The type of data object cached and provided by this RestClientDataProvider.</typeparam>
    /// <typeparam name="P">The type of the client for singleton initialization.</typeparam>
    public abstract class RestClientDataProviderBase<T, P> : DataProviderBase<T, P>
        where T : JSONSerializable<T>, IJSONSerializable, IHasId
        where P : RestClientDataProviderBase<T, P>
    {

        /// <summary>
        /// Use this method to cancel all subscriptions of this client to the backend.
        /// </summary>
        public static void StopProviding()
        {
            foreach (var subscription in openSubscriptions.Values)
                subscription.Dispose();
            openSubscriptions.Clear();
        }

        protected static async Task<ProviderResponse<V>> SendRequest<V>(HttpMethod method, Uri uri)
            where V : JSONSerializable<V>, IJSONSerializable
        {
            var httpResponse = await NetworkUtils.SendRequestAsync(method, uri);
            if (!httpResponse.IsSuccessStatusCode)
            {
                Debug.LogWarning(httpResponse.ReasonPhrase);
                return ProviderResponse<V>.EmptyResponse();
            }
            var content = await httpResponse.Content.ReadAsStringAsync();
            try
            {
                return ProviderResponse<V>.Response(JSONSerializable<V>.FromJSONString(content));
            }
            catch (Exception err)
            {
                Debug.LogWarning(err.Message);
                Debug.LogWarning(content);
            }
            return ProviderResponse<V>.EmptyResponse();
        }

        protected abstract string PollingUri { get; }
        protected abstract string SseUri { get; }
        protected abstract HttpMethod PollingMethod { get; }

        protected override void CancelSubscriptionToUpdates(string id)
        {
            Debug.Log("Canceling...");
            if (openSubscriptions.TryGetValue(id, out var subscription))
            {
                subscription.Dispose();
                openSubscriptions.TryRemove(id, out _);
                Debug.Log("Canceled!");
            }
        }

        protected override Task<ProviderResponse<T>> Resupply(string id) => SendRequest<T>(PollingMethod, new Uri(string.Format(PollingUri, id)));

        protected override void Resupply(string id, Action<T> callback)
        {
            Resupply(id).ContinueWith(response =>
            {
                if (response.Result.HasValue)
                    callback.Invoke(response.Result.Value);
            });
        }

        protected override void SubscribeToUpdates(string id)
        {
            if (openSubscriptions.ContainsKey(id))
                if (openSubscriptions[id].TargetState)
                
                    return;
                
                else CancelSubscriptionToUpdates(id);
            var evtSourceReader = new EventSourceReader(new Uri(string.Format(SseUri, id)));
            evtSourceReader.MessageReceived += OnMessageRecieved;
            evtSourceReader.Disconnected += HandleOnDisconnected(id);
            openSubscriptions.TryAdd(id, new EvtSourceSubscription(evtSourceReader));
            evtSourceReader.Start();
        }

        private static ConcurrentDictionary<string, EvtSourceSubscription> openSubscriptions = new ConcurrentDictionary<string, EvtSourceSubscription>();

        private void OnMessageRecieved(object sender, EventSourceMessageEventArgs evt)
        {
            try
            {
                Cache(JSONSerializable<T>.FromJSONString(evt.Message));
            }
            catch (Exception err)
            {
                Debug.LogWarning(err.Message);
                Debug.LogWarning(evt.Message);
            }
        }
        private DisconnectEventHandler HandleOnDisconnected(string id)
        {
            return (object sender, DisconnectEventArgs e) =>
            {
                if (openSubscriptions.TryGetValue(id, out var subscription) && subscription.TargetState)
                {
                    if (!subscription.Reader.IsDisposed)
                        subscription.Dispose();
                    if (e.Exception != null)
                        Debug.LogWarning(e.Exception.Message);
                    Task.Run(TryRenewSubscription(id, e.ReconnectDelay));
                }
            };
        }

        private Action TryRenewSubscription(string id, int reconnectDelay)
        {
            return async () =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(reconnectDelay));
                SubscribeToUpdates(id);
            };
        }

        private struct EvtSourceSubscription
        {
            internal bool TargetState { get; private set; }
            internal EventSourceReader Reader { get; }

            internal EvtSourceSubscription(EventSourceReader reader)
            {
                TargetState = true;
                Reader = reader;
            }

            internal void Dispose()
            {
                TargetState = false;
                Reader.Dispose();
            }
        }
    }
}
using Sandbox.Damian.FTK.Data.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Sandbox.Damian.FTK.Data
{
    /// <summary>
    /// A base class for data-providing objects.
    /// Implements a default cache for it's data objects.
    /// </summary>
    /// <typeparam name="T">The type of data object cached and provided by this DataProvider.</typeparam>
    /// <typeparam name="P">The type of the provider for singleton initialization.</typeparam>
    public abstract class DataProviderBase<T, P> : PublicSingleton<P>
        where T : IHasId
        where P : DataProviderBase<T, P>
    {
        /// <summary>
        /// Awaitable.
        /// Asynchronously provides the requestes object matching the provided id, or null iff such an object doesn't exist.
        /// </summary>
        /// <param name="id">The requested object's id.</param>
        /// <returns>a ProviderResponse containing the response. Empty iff such an object is non-existent.</returns>
        #region ProvideOnce
        public async Task<ProviderResponse<T>> ProvideOnce(string id)
        {
            ScheduleCachePurge();
            if (cache.ContainsKey(id))
                return ProviderResponse<T>.Response(cache[id].Get());
            var response = await Resupply(id);
            if (response.HasValue)
                Cache(response.Value);
            return response;
        }
        #endregion

        /// <summary>
        /// Asynchronously provides the requestes object matching the provided id.
        /// The callback will not be invoked iff such an object doesn't exist.
        /// </summary>
        /// <param name="id">The requested object's id.</param>
        /// <param name="callback">Invoked, once/iff the object has been retrieved.</param>
        #region ProvideOnce
        public void ProvideOnce(string id, Action<T> callback)
        {
            ScheduleCachePurge();
            if (cache.ContainsKey(id))
            {
                UnityEngine.Debug.Log("ContainsKey: " + id + " Element: " + cache[id].Get());
                callback.Invoke(cache[id].Get());
                return;
            }
            Resupply(id, entry =>
            {
                if (entry == null)
                    return;
                Cache(entry);
                callback(entry);
            });
        }
        #endregion

        /// <summary>
        /// Subscribes to updates of a provided/providable object.
        /// Replaces a previous subscription, iff present.
        /// When a new object is cached or a cached object is updated, the provided action will be invoked with the new/updated object.
        /// </summary>
        /// <param name="id">The id of the object to listen for updates to.</param>
        /// <param name="subscriber">The subscriber. Subscriptions are stored as weak references -> if the subscriber is collected/finalized, it's subscriptions will be voided.</param>
        /// <param name="onUpdate">The event handler to consume updates. A subscriber cannot subscribe with the same event handler multiple times.</param>
        /// <returns>true, iff there has not been a previous subscription. False otherwise.</returns>
        #region Subscribe
        public bool Subscribe(string id, object subscriber, Action<T> onUpdate)
        {
            ScheduleCachePurge();
            var tryAdd = cache.TryAdd(id, new CacheEntry(default));
            var alreadySubscribed = cache[id].IsSubscribed();
            var newSubscription = cache[id].PutSubscription(subscriber, onUpdate);
            if (!alreadySubscribed)
                SubscribeToUpdates(id);
            return newSubscription;
        }
        #endregion

        /// <summary>
        /// Explicitly cancels all subscriptions of all objects of a subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber to cancel all subscriptions for.</param>
        #region CancelSubscriptions
        public void CancelSubscriptions(object subscriber)
        {
            foreach (var id in cache.Keys)
                CancelSubscriptions(id, subscriber);
        }
        #endregion

        /// <summary>
        /// Explicitly cancels the subscription of an object with matching id.
        /// </summary>
        /// <param name="id">The object's id to no longer recieve any updates for.</param>
        /// <param name="subscriber">The subscriber to cancel the subscription for.</param>
        /// <returns>Whether a subscription existed.</returns>
        #region CancelSubscriptions
        public bool CancelSubscriptions(string id, object subscriber)
        {
            ScheduleCachePurge();
            var test = cache.TryGetValue(id, out var entry);
            Debug.Log(entry);
            if (test && entry.IsSubscribed())
            {
                var previouslySubscribed = entry.CancelSubscriptions(subscriber);
                Debug.Log(entry.IsSubscribed());
                if (!entry.IsSubscribed())
                {
                    CancelSubscriptionToUpdates(id);
                }
                return previouslySubscribed;
            }
            return false;
        }
        #endregion

        /// <summary>
        /// Caches an object locally.
        /// If a different object with a matching id is already cached, it gets overridden.
        /// Returns true whenever the cache's state changes.
        /// null will not be cached.
        /// </summary>
        /// <param name="entry">The object to cache.</param>
        /// <returns>false, if the entry is null or it is equal to an already cached entry. True, if the entry hasn't been cached yet or a different entry with the same id has been overridden.</returns>
        #region Cache
        public bool Cache(T entry)
        {
            ScheduleCachePurge();
            if (entry == null)
                return false;
            cache.TryAdd(entry.Id, new CacheEntry(default));
            return cache[entry.Id].Set(entry);
        }
        #endregion

        #region Internal
        private static readonly long CACHE_TIMEOUT = 18000000000L; // 30min in 100ns-ticks
        private static readonly long PURGE_INTERVALL = 1200000000L; //  2min in 100ns-ticks

        protected abstract Task<ProviderResponse<T>> Resupply(string id);
        protected abstract void Resupply(string id, Action<T> callback);
        protected abstract void SubscribeToUpdates(string id);
        protected abstract void CancelSubscriptionToUpdates(string id);

        protected readonly ConcurrentDictionary<string, CacheEntry> cache = new ConcurrentDictionary<string, CacheEntry>();

        private bool isPurgeScheduled = false;

        protected void ScheduleCachePurge()
        {
            if (!isPurgeScheduled)
            {
                isPurgeScheduled = true;
                Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(TimeSpan.FromTicks(PURGE_INTERVALL));
                        PurgeCache();
                    }
                    finally
                    {
                        isPurgeScheduled = false;
                    }
                });
            }
        }

        private void PurgeCache()
        {
            var timeoutThreshold = DateTime.UtcNow.AddTicks(-CACHE_TIMEOUT).Ticks;
            var entriesToPurge = new HashSet<string>();
            foreach (var id in cache.Keys)
            {
                if (cache[id].IsTimedOut(timeoutThreshold) && !cache[id].IsSubscribed())
                    entriesToPurge.Add(id);
                if (!cache[id].HasSubscriptions)
                    CancelSubscriptionToUpdates(id);
            }
            foreach (var id in entriesToPurge)
                cache.TryRemove(id, out _);
        }

        protected struct CacheEntry
        {

            internal CacheEntry(T entry)
            {
                subscriptions = new Dictionary<object, Action<T>>();
                lastAccessed = DateTime.UtcNow.Ticks;
                this.entry = entry;
            }

            internal bool HasSubscriptions { get => subscriptions.Count > 0; }

            internal bool Set(T entry)
            {
                lastAccessed = DateTime.UtcNow.Ticks;
                if (this.entry?.Equals(entry) ?? false)
                    return false;
                this.entry = entry;
                InvokeSubscriptions();
                return true;
            }

            internal T Get()
            {
                lastAccessed = DateTime.UtcNow.Ticks;
                return entry;
            }

            internal T Peek() => entry;

            internal bool IsTimedOut(long timeoutThreshold) => lastAccessed < timeoutThreshold;

            internal bool IsSubscribed()
            {
                return subscriptions.Count > 0;
            }

            internal bool PutSubscription(object subscriber, Action<T> onUpdate)
            {
                lastAccessed = DateTime.UtcNow.Ticks;
                var newSubscription = !subscriptions.TryGetValue(subscriber, out _);
                subscriptions.Remove(subscriber);
                subscriptions.Add(subscriber, onUpdate);
                if (newSubscription && entry != null)
                {
                    var thiz = this;
                    Task.Run(() => onUpdate.Invoke(thiz.entry));
                }
                return newSubscription;
            }

            internal bool CancelSubscriptions(object subscriber) => subscriptions.Remove(subscriber);

            private long lastAccessed;
            private T entry;
            private readonly Dictionary<object, Action<T>> subscriptions;

            private void InvokeSubscriptions()
            {
                foreach (var keyValuePair in subscriptions)
                    keyValuePair.Value.Invoke(entry);
            }
        }
        #endregion
    }

    /// <summary>
    /// Wraps a DataProvider's response.
    /// If a non existing object was queried, the response will be in the state 'HasValue = false'.
    /// </summary>
    /// <typeparam name="T">The type of the wrapped response object.</typeparam>
    public struct ProviderResponse<T>
    {
        /// <summary>
        /// A static constructor for an empty ProviderResponse.
        /// </summary>
        /// <returns>a ProviderResponse which has HasValue set to false.</returns>
        public static ProviderResponse<T> EmptyResponse() => new ProviderResponse<T>(false, default);

        /// <summary>
        /// A static constructor for a populated ProviderResponse.
        /// </summary>
        /// <param name="obj">The object to wrap the ProviderResponse around</param>
        /// <returns>a populated ProviderResponse which has HasValue set to true.</returns>
        public static ProviderResponse<T> Response(T obj) => new ProviderResponse<T>(true, obj);

        /// <summary>
        /// Whether or not this ProviderResponse contains a valid response object.
        /// </summary>
        public bool HasValue { get; }

        /// <summary>
        /// The object of the ProviderResponse.
        /// Do not use if HasValue is set to false!
        /// </summary>
        public T Value { get; }

        #region Internal
        private ProviderResponse(bool hasValue, T value)
        {
            HasValue = hasValue;
            Value = value;
        }
        #endregion
    }
}
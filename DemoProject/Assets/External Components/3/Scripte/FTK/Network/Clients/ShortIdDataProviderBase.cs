using Sandbox.Damian.FTK.Data;
using Sandbox.Damian.FTK.Data.Entities;
using Sandbox.Damian.FTK.Data.Serialization;
using System;
using System.Threading.Tasks;
using FTK.Network;
namespace Sandbox.Damian.FTK.Network.Clients
{
    /// <summary>
    /// A base class for REST-API based data providers with objects using an alternative short id as well as the normal id.
    /// Cache is populated using the REST-API.
    /// Using the short id is less eficient than using the normal id. Use the normal id whenever possible.
    /// </summary>
    /// <typeparam name="T">The type of data object cached and provided by this RestClientDataProvider.</typeparam>
    /// <typeparam name="P">The type of the client for singleton initialization.</typeparam>
    public abstract class ShortIdDataProviderBase<T, P> : RestClientDataProviderBase<T, P>
        where T : JSONSerializable<T>, IJSONSerializable, IHasShortId
        where P : ShortIdDataProviderBase<T, P>
    {
        /// <summary>
        /// If possible, use ProvideOnce instead.
        /// 
        /// Awaitable.
        /// Asynchronously provides the requestes object matching the provided short id, or null iff such an object doesn't exist.
        /// </summary>
        /// <param name="shortId">The requested object's short id.</param>
        /// <returns>a ProviderResponse containing the response. Empty iff such an object is non-existent.</returns>
        #region ProvideOnceByShortId
        public async Task<ProviderResponse<T>> ProvideOnceByShortId(string shortId)
        {
            ScheduleCachePurge();
            if (TryGetCacheEntryByShortId(shortId, out var cachedEntry))
                return ProviderResponse<T>.Response(cachedEntry);
            var response = await ResupplyByShortId(shortId);
            if (response.HasValue)
                Cache(response.Value);
            return response;
        }
        #endregion

        /// <summary>
        /// If possible, use ProvideOnce instead.
        /// 
        /// Asynchronously provides the requestes object matching the provided short id.
        /// The callback will not be invoked iff such an object doesn't exist.
        /// </summary>
        /// <param name="shortId">The requested object's short id.</param>
        /// <param name="callback">Invoked, once/iff the object has been retrieved.</param>
        #region ProvideOnceByShortId
        public void ProvideOnceByShortId(string shortId, Action<T> callback)
        {
            ScheduleCachePurge();
            if (TryGetCacheEntryByShortId(shortId, out var cachedEntry))
            {
                callback.Invoke(cachedEntry);
                return;
            }
            ResupplyByShortId(shortId, entry =>
            {
                if (entry == null)
                    return;
                Cache(entry);
                callback(entry);
            });
        }
        #endregion

        /// <summary>
        /// If possible, use Subscribe instead.
        /// 
        /// Subscribes to updates of a provided/providable object.
        /// When a new object is cached or a cached object is updated, the provided action will be invoked with the new/updated object.
        /// </summary>
        /// <param name="shortId">The short id of the object to listen for updates to.</param>
        /// <param name="subscriber">The subscriber. Subscriptions are stored as weak references -> if the subscriber is collected/finalized, it's subscriptions will be voided.</param>
        /// <param name="onUpdate">The event handler to consume updates. A subscriber cannot subscribe with the same event handler multiple times.</param>
        #region SubscribeByShortId
        public void SubscribeByShortId(string shortId, object subscriber, Action<T> onUpdate)
        {
            if (TryGetCacheEntryByShortId(shortId, out var cachedEntry))
            {
                var id = cachedEntry.Id;
                Subscribe(id, subscriber, onUpdate);
                return;
            }
            ProvideOnceByShortId(shortId, entry =>
            {
                onUpdate.Invoke(entry);
                Subscribe(entry.Id, subscriber, onUpdate);
            });
        }
        #endregion

        /// <summary>
        /// If possible, use CancelSubscriptions instead.
        /// 
        /// Explicitly cancels all subscriptions of an object with matching short id.
        /// </summary>
        /// <param name="shortId">The object's short id to no longer recieve any updates for.</param>
        /// <param name="subscriber">The subscriber to cancel the subscriptions for.</param>
        #region CancelSubscriptionsByShortId
        public void CancelSubscriptionsByShortId(string shortId, object subscriber)
        {
            if (TryGetCacheEntryByShortId(shortId, out var cachedEntry))
            {
                var id = cachedEntry.Id;
                CancelSubscriptions(id, subscriber);
            }
        }
        #endregion

        #region Internal
        protected abstract string ShortIdPollingUri { get; }

        private bool TryGetCacheEntryByShortId(string shortId, out T cachedEntry)
        {
            cachedEntry = null;
            foreach (var entry in cache.Values)
                if (entry.Peek().ShortId == shortId)
                {
                    cachedEntry = entry.Get();
                    return true;
                }
            return false;
        }

        private async Task<ProviderResponse<T>> ResupplyByShortId(string shortId)
        {
            var httpResponse = await NetworkUtils.SendRequestAsync(PollingMethod, new Uri(string.Format(ShortIdPollingUri, shortId)));
            if (!httpResponse.IsSuccessStatusCode)
                return ProviderResponse<T>.EmptyResponse();
            var content = await httpResponse.Content.ReadAsStringAsync();
            try
            {
                var obj = JSONSerializable<T>.FromJSONString(content);
                return ProviderResponse<T>.Response(obj);
            }
            catch { }
            return ProviderResponse<T>.EmptyResponse();
        }

        private void ResupplyByShortId(string shortId, Action<T> callback)
        {
            ResupplyByShortId(shortId).ContinueWith(response =>
            {
                if (response.Result.HasValue)
                    callback.Invoke(response.Result.Value);
            });
        }
        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FTK.Network
{
    /// <summary>
    /// A utility-class for asynchronously sending HTTP-requests without risking port-exhaustion.
    /// </summary>
    public static class NetworkUtils
    {
        private static readonly string authorization = "authorization";
        private static readonly Lazy<HttpClient> _client = new Lazy<HttpClient>(() => new HttpClient());

        private static readonly Lazy<Queue<Request>> _requestQueue =
            new Lazy<Queue<Request>>(() => new Queue<Request>());

        private static readonly Lazy<Dictionary<Uri, ServicePoint>> _servicePoints =
            new Lazy<Dictionary<Uri, ServicePoint>>(() => new Dictionary<Uri, ServicePoint>());

        private static bool _threadQueued = false;
        private static int _connectionLimit = 10;
        private static object _accessMutex = new System.Object();
        private static int _socketSemaphore = 0;
        private static int _backoff = 1;

        /// <summary>
        /// Timeout in seconds for all future Requests.
        /// </summary>
        public static double Timeout
        {
            get
            {
                lock (_accessMutex) return _client.Value.Timeout.TotalSeconds;
            }
            set
            {
                lock (_accessMutex) _client.Value.Timeout = TimeSpan.FromSeconds(value);
            }
        }

        /// <summary>
        /// Limit of simultaneous sockets for all future Requests.
        /// </summary>
        public static int ConnectionLimit
        {
            get
            {
                lock (_accessMutex) return _connectionLimit;
            }
            set
            {
                lock (_accessMutex)
                {
                    _connectionLimit = value;
                    foreach (ServicePoint p in _servicePoints.Value.Values)
                        p.ConnectionLimit = value;
                }
            }
        }

        /// <summary>
        /// Limit of service points for all future Requests.
        /// Unbounded, if value if set to 0.
        /// </summary>
        public static int ServicePointLimit
        {
            get { return ServicePointManager.MaxServicePoints; }
            set { ServicePointManager.MaxServicePoints = value; }
        }

        /// <summary>
        /// AWAITABLE
        /// Sends the given HTTP-request asynchronously.
        /// </summary>
        /// <param name="request">The request to send</param>
        /// <returns>The HTTP-response</returns>
        public static Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
        {
            TaskCompletionSource<HttpResponseMessage>
                completionSource = new TaskCompletionSource<HttpResponseMessage>();
            SendRequest(request, result => completionSource.SetResult(result));
            return completionSource.Task;
        }

        /// <summary>
        /// AWAITABLE
        /// Sends a HTTP-request of type "method" to "uri" asynchronously.
        /// </summary>
        /// <param name="method">The HTTP-Method</param>
        /// <param name="uri">The request-URI</param>
        /// <returns>The HTTP-response</returns>
        public static Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, Uri uri)
        {
            TaskCompletionSource<HttpResponseMessage>
                completionSource = new TaskCompletionSource<HttpResponseMessage>();
            SendRequest(method, uri, result => completionSource.SetResult(result));
            return completionSource.Task;
        }

        /// <summary>
        /// AWAITABLE
        /// Sends a HTTP-request of type "method" to "uri" asynchronously.
        /// </summary>
        /// <param name="method">The HTTP-Method</param>
        /// <param name="uri">The request-URI</param>
        /// <param name="content">The HttpContent to send with the request</param>
        /// <returns>The HTTP-response</returns>
        public static Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, Uri uri, HttpContent content)
        {
            TaskCompletionSource<HttpResponseMessage>
                completionSource = new TaskCompletionSource<HttpResponseMessage>();
            SendRequest(method, uri, content, result => completionSource.SetResult(result));
            return completionSource.Task;
        }

        /// <summary>
        /// AWAITABLE
        /// Sends a HTTP-request of type "method" to "uri" asynchronously.
        /// </summary>
        /// <param name="method">The HTTP-Method</param>
        /// <param name="uri">The request-URI</param>
        /// <param name="credentials">This string will be put into the "authorization"-header of the request</param>
        /// <returns>The HTTP-response</returns>
        public static Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, Uri uri, string credentials)
        {
            TaskCompletionSource<HttpResponseMessage>
                completionSource = new TaskCompletionSource<HttpResponseMessage>();
            SendRequest(method, uri, credentials, result => completionSource.SetResult(result));
            return completionSource.Task;
        }

        /// <summary>
        /// AWAITABLE
        /// Sends a HTTP-request of type "method" to "uri" asynchronously.
        /// </summary>
        /// <param name="method">The HTTP-Method</param>
        /// <param name="uri">The request-URI</param>
        /// <param name="content">The HttpContent to send with the request</param>
        /// <param name="credentials">This string will be put into the "authorization"-header of the request</param>
        /// <returns>The HTTP-response</returns>
        public static Task<HttpResponseMessage> SendRequestAsync(HttpMethod method, Uri uri, HttpContent content,
            string credentials)
        {
            TaskCompletionSource<HttpResponseMessage>
                completionSource = new TaskCompletionSource<HttpResponseMessage>();
            SendRequest(method, uri, content, credentials, result => completionSource.SetResult(result));
            return completionSource.Task;
        }

        /// <summary>
        /// Sends the given HTTP-request.
        /// The corresponding response can be consumed by "callback".
        /// </summary>
        /// <param name="request">The request to send</param>
        /// <param name="callback">The response callback</param>
        public static void SendRequest(HttpRequestMessage request, Action<HttpResponseMessage> callback)
        {
            UpdateServicePointsAndEnqueue(new Request(request, callback));
        }

        /// <summary>
        /// Sends a HTTP-request of type "method" to "uri".
        /// The corresponding response can be consumed by "callback".
        /// </summary>
        /// <param name="method">The HTTP-Method</param>
        /// <param name="uri">The request-URI</param>
        /// <param name="callback">The response callback</param>
        public static void SendRequest(HttpMethod method, Uri uri, Action<HttpResponseMessage> callback)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(method, uri);
            UpdateServicePointsAndEnqueue(new Request(requestMessage, callback));
        }

        /// <summary>
        /// Sends a HTTP-request of type "method" to "uri".
        /// The corresponding response can be consumed by "callback".
        /// </summary>
        /// <param name="method">The HTTP-Method</param>
        /// <param name="uri">The request-URI</param>
        /// <param name="content">The HttpContent to send with the request</param>
        /// <param name="callback">The response callback</param>
        public static void SendRequest(HttpMethod method, Uri uri, HttpContent content,
            Action<HttpResponseMessage> callback)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(method, uri);
            requestMessage.Content = content;
            UpdateServicePointsAndEnqueue(new Request(requestMessage, callback));
        }

        /// <summary>
        /// Sends a HTTP-request of type "method" to "uri" with the specified credential-string.
        /// The corresponding response can be consumed by "callback".
        /// </summary>
        /// <param name="method">The HTTP-Method</param>
        /// <param name="uri">The request-URI</param>
        /// <param name="credentials">This string will be put into the "authorization"-header of the request</param>
        /// <param name="callback">The response callback</param>
        public static void SendRequest(HttpMethod method, Uri uri, string credentials,
            Action<HttpResponseMessage> callback)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(method, uri);
            requestMessage.Headers.Add(authorization, credentials);
            UpdateServicePointsAndEnqueue(new Request(requestMessage, callback));
        }

        /// <summary>
        /// Sends a HTTP-request of type "method" to "uri" with the specified credential-string.
        /// The corresponding response can be consumed by "callback".
        /// </summary>
        /// <param name="method">The HTTP-Method</param>
        /// <param name="uri">The request-URI</param>
        /// <param name="content">The HttpContent to send with the request</param>
        /// <param name="credentials">This string will be put into the "authorization"-header of the request</param>
        /// <param name="callback">The response callback</param>
        public static void SendRequest(HttpMethod method, Uri uri, HttpContent content, string credentials,
            Action<HttpResponseMessage> callback)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(method, uri);
            requestMessage.Content = content;
            requestMessage.Headers.Add(authorization, credentials);
            UpdateServicePointsAndEnqueue(new Request(requestMessage, callback));
        }

        private static void UpdateServicePointsAndEnqueue(Request request)
        {
            Uri uri = request.RequestMessage.RequestUri;
            lock (_accessMutex)
            {
                if (!_servicePoints.Value.ContainsKey(uri))
                {
                    ServicePoint p = ServicePointManager.FindServicePoint(uri);
                    p.ConnectionLimit = _connectionLimit;
                    _servicePoints.Value.Add(uri, p);
                }

                _requestQueue.Value.Enqueue(request);
                //Debug.Log(_requestQueue.Value.Count);
                //if (!_requestThread.Value.IsAlive)
                //    _requestThread.Value.Start();
            }
            
            StartThreadIfNotRunning();
        }

        private static void StartThreadIfNotRunning()
        {
            lock (_accessMutex)
                if (!_threadQueued)
                {
                    _threadQueued = true;
                    Task.Run(RequestThreadTask);
                }
        }

        private static void RequestThreadTask()
        {
            while (true)
            {
                if (!RequestsInQueue())
                    break;
                if (AllowSendRequest())
                {
                    Request request;
                    lock (_accessMutex)
                    {
                        request = _requestQueue.Value.Dequeue();
                        _socketSemaphore++;
                    }

                    Task.Run(() => SendRequestBackground(request));
                }
                else Backoff();
            }

            lock (_accessMutex)
                _threadQueued = false;
        }

        private static async void SendRequestBackground(Request request)
        {
            HttpRequestMessage requestMessage = request.RequestMessage;
            
            HttpResponseMessage responseMessage = await _client.Value.SendAsync(requestMessage);
            lock (_accessMutex)
            {
                _socketSemaphore--;
                _backoff = 1;
            }

            request.Callback.Invoke(responseMessage);
        }

        private static bool AllowSendRequest()
        {
            lock (_accessMutex)
                return _socketSemaphore < _connectionLimit;
        }

        private static bool RequestsInQueue()
        {
            lock (_accessMutex)
                return _requestQueue.Value.Count > 0;
        }

        private static void Backoff()
        {
            lock (_accessMutex)
            {
                Thread.Sleep(_backoff);
                if (_backoff < 1025)
                    _backoff <<= 1;
            }
        }

        private readonly struct Request
        {
            public Request(HttpRequestMessage requestMessage, Action<HttpResponseMessage> callback)
            {
                RequestMessage = requestMessage;
                Callback = callback;
            }

            public HttpRequestMessage RequestMessage { get; }
            public Action<HttpResponseMessage> Callback { get; }
        }


       
    }
}

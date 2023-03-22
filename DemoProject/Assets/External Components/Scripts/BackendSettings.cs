using UnityEngine;

namespace FaPra.ScriptableObjects {
    /// <summary>
    /// Represents Settings for Connection to Augmented Manufacturing including API Endpoint paths, used in Clients.
    /// </summary>
    [CreateAssetMenu(fileName = "New Backend settings", menuName = "BackendSettings")]
    public class BackendSettings : ScriptableObject {
        // Supported Web Protocols
        public enum WebProtocol {
            http,
            https,
        };
        
        // Connection and Authentication
        public WebProtocol Protocol { get { return protocol; } }
        public string Host { get { return host; } }
        public int Port { get { return port; } }
        public string User { get { return user; } }
        public string Password { get { return password; } }

        // API Endpoints
        public string ApiProcessingStations { get { return apiProcessingStations; } }
        public string ApiPrintJobs { get { return apiPrintJobs; } }
        public string ApiOctoprintProgress { get { return apiOctoprintProgress; } }
        public string ApiProductionsPlan { get { return apiProductionsPlan; } }
        public string ApiOctoprintTemperature { get { return apiOctoprintTemperature; } }
        public string ApiPrintjobController { get { return apiPrintjobController; } }
        public string ApiOctoprintStateClient { get { return apiOctoprintStateClient; } }
        public string ApiAnomaly { get { return apiAnomaly; } }
        public string ApiAnomalyOperation { get { return apiAnomalyOperation; } }

        [SerializeField]
        private WebProtocol protocol = WebProtocol.http;
        [SerializeField]
        private string host = "localhost";
        [SerializeField]
        private int port = 8080;
        [SerializeField]
        private string user;
        [SerializeField]
        private string password;

        [SerializeField]
        private string apiProcessingStations = "/api/processingstations";
        [SerializeField]
        private string apiPrintJobs = "/api/subscriptions/printjobsummary";
        [SerializeField]
        private string apiOctoprintProgress = "/api/subscriptions/machine";
        [SerializeField]
        private string apiProductionsPlan = "/api/production-plans";
        [SerializeField]
        private string apiOctoprintTemperature = "/api/subscriptions/machine";
        [SerializeField]
        private string apiPrintjobController = "/api/printjobs";
        [SerializeField]
        private string apiOctoprintStateClient = "/api/subscriptions/machine";
        [SerializeField]
        private string apiAnomaly = "/api/subscriptions/machine";
        [SerializeField]
        private string apiAnomalyOperation = "/api/machines";
    }
}
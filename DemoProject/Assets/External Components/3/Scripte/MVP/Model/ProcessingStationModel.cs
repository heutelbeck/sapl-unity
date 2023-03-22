using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MVP.Model {
    public class ProcessingStationModel : IModel
    {
        private static List<Printer> printers;
        private static List<BuildUnit> buildUnits;
        private static List<NaturalCoolingUnit> coolingUnits;
        protected static List<ProcessingStation> processingStations;
        private string processingStationID;
        private StateMachine stateMachine;
        public Action selfDestructevent;

        public static readonly Condition START = Condition.GetConditionInstance("START");
        public static readonly Condition SUBSCRIBED = Condition.GetConditionInstance("SUBSCRIBE");
        public static readonly Condition FATAL_ERROR = Condition.GetConditionInstance("FATAL_ERROR");

        private static readonly string STATE_MACHINE_DEF =
            "(Init)                        -> {START: Start, FATAL_ERROR: FatalError};" +
            "(Start, OnStart)              -> {SUBSCRIBE: Default, FATAL_ERROR: FatalError};" +
            "(Default)                     -> {FATAL_ERROR: FatalError};" +
            "(FatalError, OnFatalError)    -> {};";

        ModelType IModel.ModelType => ModelType.PROCESSING_STATION;

        void IModel.ChangeState(Condition condition)
        {
            stateMachine.ChangeState(condition);
        }

        
       

        public ProcessingStationModel(string processingStationID)
        {
            this.processingStationID = processingStationID;
            stateMachine = new StateMachine(STATE_MACHINE_DEF, this);

        }

        public ModelType ModelType => ModelType.PROCESSING_STATION;

        public void ChangeState(Condition condition) => stateMachine.ChangeState(condition);

        public void Destroy()
        {
            //TODO
            //ProcessingStationClient.Instance?.CancelSubscription(this);
        }

        public string GetProcessingStationID() => processingStationID;

        // TODO replace
        public static List<Device> getProcessingStations()
        {
            processingStations = new List<ProcessingStation>();
            ProcessingStation processingStation = new ProcessingStation("ProcessingStation", "Processing Station 1");
            processingStations.Add(processingStation);
            List<Device> devices;
            devices = NewRESTClient.getProcessingStations();
            return devices;
        }

        //public async void AddManufacturingItemsToStorageUnit(Dictionary<string, List<string>> manufacturingItemIds, string storageUnitId)
        //{
        //    UriBuilder uriBuilder = new UriBuilder();
        //    uriBuilder.Scheme = "http";
        //    uriBuilder.Port = 8080;
        //    uriBuilder.Host = "localhost";
        //    uriBuilder.Path = "api/storageunits/changeStorageUnit";
        //    uriBuilder.Query = "fromStorageUnit=" + storageUnitId + "&toStorageUnit=" + this.storageUnitId;
        //    Uri uri = uriBuilder.Uri;
        //    HttpContent content = new StringContent(JsonConvert.SerializeObject(manufacturingItemIds, Formatting.Indented), Encoding.UTF8, "application/json");
        //    UnityEngine.Debug.Log("json: " + JsonConvert.SerializeObject(manufacturingItemIds, Formatting.Indented));
        //    UnityEngine.Debug.Log("uri: " + uri.AbsoluteUri);
        //    HttpResponseMessage response = await NetworkUtils.SendRequestAsync(System.Net.Http.HttpMethod.Put, uri, content);
        //    UnityEngine.Debug.Log("statuscode: " + response.StatusCode + " content: " + response.Content);
        //}
    }


}

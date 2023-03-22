using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using MVP.Model;
using MVP.View;

namespace MVP.Presenter {
    public class ProcessingStationPresenter : SubPresenterWithModel
    {
        [SerializeField] private GameObject prefabBuildUnit;
        [SerializeField] private GameObject prefabProcessingStation;
        [SerializeField] private GameObject positionForProcessingStation1;
        [SerializeField] private GameObject positionsForProcessingStations;
        [SerializeField] private GameObject positionsForBuildUnits;

        static List<Device> devices;
        protected static List<ProcessingStation> processingStations;
        private static SceneInput sceneInput;
        private bool presenterInitialized = false;
        private bool modelInitialized = false;
        private ProcessingStationModel model;
        private PresenterBase parent;


        public override bool Initialized { get => presenterInitialized && modelInitialized; }



        public override bool InitModel(IModel model)
        {
            if (!typeof(ProcessingStationModel).IsAssignableFrom(model.GetType()))
                return false;
            this.model = model as ProcessingStationModel;
            this.model.selfDestructevent += OnModelSelfDestruct;
            return modelInitialized = true;
        }

        public override bool InitPresenter(PresenterBase parent)
        {
            this.parent = parent;
            return presenterInitialized = true;
        }

        public override void ApplyCondition(Condition condition)
        {
            model.ChangeState(condition);
        }

        private void OnModelSelfDestruct()
        {

        }

        public static List<Device> GetDevices()
        {
            devices = ProcessingStationModel.getProcessingStations();
            return devices;
        }

        public SceneInput GetSceneInput() 
        {
            sceneInput = new SceneInput(prefabBuildUnit, prefabProcessingStation, positionForProcessingStation1, positionsForProcessingStations, positionsForBuildUnits);
            return sceneInput; 
        }
       

        public static void arrangeProcessingStation(List<Device> devices, SceneInput sceneInput)
        {
            int index = 0;
            GameObject newObject;
            Transform transformForNewObject;

            foreach (var device in devices)
            {
                processingStations.Add(new ProcessingStation(device.id, device.name));
                transformForNewObject = transformForNewObject =sceneInput.PositionsForProcessingStations.transform.GetChild(index); ;
                newObject = Instantiate(sceneInput.PrefabProcessingStationt, transformForNewObject.position, sceneInput.PrefabProcessingStationt.transform.rotation);
                newObject.name = device.id;
                index = index + 1;
            }

        }
    }
}


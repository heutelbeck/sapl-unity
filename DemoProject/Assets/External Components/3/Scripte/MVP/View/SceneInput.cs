using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVP.View {

    public class SceneInput
    {

        [SerializeField] private GameObject prefabBuildUnit;
        [SerializeField] private GameObject prefabProcessingStation;
        [SerializeField] private GameObject positionForProcessingStation1;
        [SerializeField] private GameObject positionsForProcessingStations;
        [SerializeField] private GameObject positionsForBuildUnits;
        public GameObject PrefabBuildUnit
        {
            get { return prefabBuildUnit; }

        }

        public GameObject PrefabProcessingStationt
        {
            get { return prefabProcessingStation; }

        }
        public GameObject PositionForProcessingStation1
        {
            get { return positionForProcessingStation1; }

        }

        public GameObject PositionsForProcessingStations
        {
            get { return positionsForProcessingStations; }

        }

        public GameObject PositionsForBuildUnits
        {
            get { return positionsForBuildUnits; }

        }

        public SceneInput(GameObject prefabBuildUnit, GameObject prefabProcessingStation, GameObject positionForProcessingStation1, GameObject positionsForProcessingStations, GameObject positionsForBuildUnits)
        {

            this.prefabBuildUnit = prefabBuildUnit;
            this.prefabBuildUnit = prefabBuildUnit;
            this.positionForProcessingStation1 = positionForProcessingStation1;
            this.positionsForProcessingStations = positionsForProcessingStations;
            this.positionsForBuildUnits = positionsForBuildUnits;

        }

    }
}
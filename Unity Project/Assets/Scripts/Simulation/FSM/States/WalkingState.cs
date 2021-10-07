using UnityEngine;
using UnityEngine.UI;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using System;

public class WalkingState : ISimulationState
{
    private readonly SimulationStatePattern simulation;

    public WalkingState(SimulationStatePattern simulationStatePattern)
    {
        simulation = simulationStatePattern;
    }

    

    public void EnterState()
    {
        //simulation.Map.ImageLayer.SetLayerSource(ImagerySourceType.MapboxSatellite);
        simulation.Map.SetZoom(simulation.SimulationZoom);
        simulation.Directions.Query();
    }

    public void UpdateState()
    {
        if (simulation.Visualizer.State == ModuleState.Finished && simulation.readyToStart && !simulation.started)
        {
            simulation.started = true;

            //Enable buildings for simulation
            try
            {
                simulation.Map.VectorData.FindFeatureSubLayerWithName("ExtrudedBuildings").Filtering.GetFilter(0).SetNumberIsLessThan("height", 9999999999);
            }catch(Exception e)
            {
                Debug.Log(e.Message);
            }

            Vector3 startPoint = simulation.Directions.dat[0];
            float h = simulation.Map.QueryElevationInUnityUnitsAt(VectorExtensions.GetGeoPosition(startPoint, simulation.Map.CenterMercator, simulation.Map.WorldRelativeScale));
            startPoint = new Vector3(startPoint.x, h, startPoint.z);
            simulation.Directions.dat[0] = startPoint;

            simulation.Player = GameObject.Instantiate(simulation.PlayerPrefab, startPoint, Quaternion.identity);
            simulation.PlayerCam = simulation.Player.GetComponentInChildren<Camera>();
            simulation.PlayerPanel = simulation.PlayerCam.transform.GetChild(0).GetChild(0).gameObject;
            simulation.PlayerVRButton = simulation.PlayerPanel.transform.GetChild(0).GetComponent<Button>();

            simulation.WearVisorPanel.transform.localRotation = Quaternion.identity;
            simulation.WearVisorPanel.GetComponent<Canvas>().worldCamera = simulation.PlayerCam;

            simulation.MainCam.enabled = false;

            simulation.VRManager.TurnOnHybridInput();

            //simulation.Directions._directionsGO.GetComponent<MeshRenderer>().enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && simulation.VR_on)
        {
            simulation.VRManager.SwitchTo2D();
        }
    }

    private void ExitState()
    {
        //simulation.Map.ImageLayer.SetLayerSource(ImagerySourceType.MapboxSatelliteStreet);
        simulation.started = false;
        simulation.readyToStart = false;
        if (simulation.Player != null)
            simulation.Player.gameObject.Destroy();
    }

    public void ToNotStartedState()
    {
        ExitState();

        simulation.notStartedState.EnterState();
        simulation.currentState = simulation.notStartedState;
    }

    public void ToWalkingState()
    {
        Debug.LogError("Passed from walking to walking.");
    }

    public void ToStreetViewState()
    {
        ExitState();

        simulation.streetViewState.EnterState();
        simulation.currentState = simulation.streetViewState;
    }
}

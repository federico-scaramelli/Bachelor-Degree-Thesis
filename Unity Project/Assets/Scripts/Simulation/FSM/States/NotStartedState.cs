using UnityEngine;
using UnityEngine.XR;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Gvr.Internal;
using System;

public class NotStartedState : ISimulationState
{
    private readonly SimulationStatePattern simulation;

    public NotStartedState(SimulationStatePattern simulationStatePattern)
    {
        simulation = simulationStatePattern;
    }

    public void EnterState()
    {
        //temp
        if(simulation.FromInputField.text == "")
        {
            simulation.FromInputField.text = "one world trade center";
            simulation.SpeedInputField.text = "10";
            simulation.TimeInputField.text = "10";

            //simulation.FromInputField.text = "empire state building";
            //simulation.FromInputField.text = "manhattan";
            //simulation.SpeedInputField.text = "3";
            //simulation.TimeInputField.text = "0.1";
            //simulation.SpeedInputField.text = "15";
            //simulation.TimeInputField.text = "5";
            //simulation.ToInputField.text = "fredrick douglass houses";
            //simulation.FromInputField.text = "lincoln towers";
            //simulation.FromInputField.text = "clusone";
            //simulation.ToInputField.text = "spiazzi di gromo";
        }


        //Handle Cams 
        simulation.MainCam.enabled = true;
        simulation.MenuPanel.SetActive(true);
        simulation.Map.Root.gameObject.SetActive(true);

        //UI
        simulation.FromInputField.interactable = true;

        //Handle VR/2D
        simulation.VRManager.TurnOnMouseInput();

        //Disable buildings for performance
        try
        {
            simulation.Map.VectorData.FindFeatureSubLayerWithName("ExtrudedBuildings").Filtering.GetFilter(0).SetNumberIsGreaterThan("height", 99999);
        }catch(Exception e)
        {
            Debug.Log(e.Message);
        }


        //Pan & zoom & extent
        if (simulation.Directions._directionsGO == null) //Se non ho ancora calcolato il percorso
        {
            simulation.Map.gameObject.GetComponent<QuadTreeCamMov>().enabled = true;
            simulation.MainCam.gameObject.GetComponent<CameraHandler>().enabled = false;
            simulation.readyToStart = false;
            simulation.Map.SetExtent(MapExtentType.CameraBounds);
            simulation.Map.Options.extentOptions.defaultExtents.cameraBoundsOptions.SetOptions(simulation.MainCam, 1, 1);
        }
        else
        {
            //Mantieni AlongPath 
            simulation.Directions.Query();
            simulation.Map.gameObject.GetComponent<QuadTreeCamMov>().enabled = false;
            simulation.MainCam.gameObject.GetComponent<CameraHandler>().enabled = true;
        }
    }

    public void UpdateState()
    {
        
    }

    public void ExitState()
    {
        simulation.MenuPanel.SetActive(false);
        simulation.MainCam.gameObject.GetComponent<CameraHandler>().enabled = false;
        simulation.Map.gameObject.GetComponent<QuadTreeCamMov>().enabled = false;
    }

    public void ToNotStartedState()
    {
        
    }

    public void ToWalkingState()
    {
        ExitState();
        simulation.LoadingPanel.SetActive(true);

        simulation.walkingState.EnterState();
        simulation.currentState = simulation.walkingState;
    }

    public void ToDirectionsState()
    {
        Debug.LogError("Passed from not started to directions.");
    }

    public void ToStreetViewState()
    {
        ExitState();
        simulation.MainCam.enabled = false;
        simulation.Map.Root.gameObject.SetActive(false);

        simulation.streetViewState.EnterState();
        simulation.currentState = simulation.streetViewState;
    }

    public void ToArrivedState()
    {
        Debug.LogError("Passed from not started to arrived.");
    }
}

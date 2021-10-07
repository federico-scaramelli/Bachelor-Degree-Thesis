using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Mapbox.Utils;
using Gvr.Internal;
using UnityEngine.XR;
using UnityEngine.Networking;

public class StreetViewState : ISimulationState
{

    private readonly SimulationStatePattern simulation;

    public StreetViewState(SimulationStatePattern simulationStatePattern)
    {
        simulation = simulationStatePattern;
    }

    public void EnterState()
    {
        simulation.SVPanel.SetActive(true);
        simulation.Sphere.enabled = true;
        simulation.SVCam.enabled = true;

        simulation.WearVisorPanel.transform.localRotation = Quaternion.identity;
        simulation.WearVisorPanel.GetComponent<Canvas>().worldCamera = simulation.SVCam;

        simulation.VRManager.TurnOnHybridInput();
        XRSettings.enabled = false;
        simulation.SV_VRButton.GetComponentInChildren<Text>().text = "VR";
        simulation.VR_on = false;

        simulation.LoadingPanel.SetActive(false);
    }


    public void UpdateState()
    {
        if(!simulation.WearVisorPanel.activeInHierarchy) //&& !XRSettings.enabled)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && simulation.VR_on)
            {
                simulation.VRManager.SwitchTo2D();
            }
            if (Application.platform == RuntimePlatform.Android)
            {
                Quaternion rot;
                if (simulation.VRManager.TryGetCenterEyeNodeStateRotation(out rot))
                    simulation.SVCamRig.transform.localRotation = rot;
                else
                    Debug.Log("no gyro");
            }
        }
    }


    public void ToNotStartedState()
    {
        simulation.SVPanel.SetActive(false);
        simulation.Sphere.enabled = false;

        simulation.notStartedState.EnterState();
        simulation.currentState = simulation.notStartedState;
    }

    public void ToWalkingState()
    {
        
    }

    public void ToStreetViewState()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR;
using Gvr.Internal;

public class VRManager : MonoBehaviour
{
    SimulationStatePattern _statePattern;

    #region VR/2D References
    [Header("VR/2D References")]
    [SerializeField]
    GvrEditorEmulator _gvrEditorEmulator;
    public GvrEditorEmulator GvrEditorEmulator
    {
        get
        {
            return _gvrEditorEmulator;
        }
    }

    [SerializeField]
    GraphicRaycaster _graphicRaycaster;
    public GraphicRaycaster GraphicRaycaster
    {
        get
        {
            return _graphicRaycaster;
        }
    }

    [SerializeField]
    EventSystem _eventSystem;
    public EventSystem EventSystem
    {
        get
        {
            return _eventSystem;
        }
    }
    #endregion

    private void Awake()
    {
        _statePattern = GameObject.FindObjectOfType<SimulationStatePattern>();
    }

    //To use normal UI
    public void TurnOnMouseInput()
    {
        XRSettings.enabled = false;
        //Off VR Head Input Object
        _gvrEditorEmulator.gameObject.SetActive(false);
    }

    public void TurnOnHybridInput()
    {
        //if (_statePattern.currentState == _statePattern.streetViewState)
        //    XRDevice.DisableAutoXRCameraTracking(_statePattern.SVCam, true);
        //else if (_statePattern.currentState == _statePattern.walkingState)
        //    XRDevice.DisableAutoXRCameraTracking(_statePattern.PlayerCam, true);
        //On VR Head Input Object
        _gvrEditorEmulator.gameObject.SetActive(true);
    }

    public IEnumerator SwitchToVR()
    {
        yield return StartCoroutine(WaitForVisor());

        if (_statePattern.WearVisorPanel.GetComponent<WearVisorPanel>().useVR)
        {
            XRSettings.enabled = true;

            if(_statePattern.currentState == _statePattern.streetViewState)
            {
                XRDevice.DisableAutoXRCameraTracking(_statePattern.SVCam, true);

                _statePattern.SVPanel.SetActive(false);
                _statePattern.SV_VRButton.GetComponentInChildren<Text>().text = "2D";
                _statePattern.VR_on = true;
            }else if(_statePattern.currentState == _statePattern.walkingState)
            {
                XRDevice.DisableAutoXRCameraTracking(_statePattern.PlayerCam, true);

                _statePattern.PlayerPanel.SetActive(false);
                _statePattern.PlayerVRButton.GetComponentInChildren<Text>().text = "2D";
                _statePattern.VR_on = true;
            }
            
        }
        else
        {
            //GvrCursorHelper.HeadEmulationActive = false;
        }
        _statePattern.WearVisorPanel.GetComponent<WearVisorPanel>().useVR = true;
    }

    public void SwitchTo2D()
    {
        XRSettings.enabled = false;

        ResetCameras();

        if (_statePattern.currentState == _statePattern.streetViewState)
        {
            _statePattern.SVPanel.SetActive(true);
            _statePattern.SV_VRButton.GetComponentInChildren<Text>().text = "VR";
            _statePattern.VR_on = false;
        }
        else if (_statePattern.currentState == _statePattern.walkingState)
        {
            _statePattern.PlayerPanel.SetActive(true);
            _statePattern.PlayerVRButton.GetComponentInChildren<Text>().text = "VR";
            _statePattern.VR_on = false;
        }
    }

    IEnumerator WaitForVisor()
    {
        Button[] buttons;
        if (_statePattern.currentState == _statePattern.streetViewState)
            buttons = _statePattern.SVPanel.GetComponentsInChildren<Button>();
        else
            buttons = _statePattern.PlayerPanel.GetComponentsInChildren<Button>();

        foreach (Button btn in buttons)
            btn.enabled = false;
        _statePattern.WearVisorPanel.SetActive(true);
        //GvrCursorHelper.HeadEmulationActive = false; //Cursore visibile
        _statePattern.VRManager.TurnOnMouseInput();

        //Wear visor panel
        yield return StartCoroutine(_statePattern.WearVisorPanel.GetComponentInParent<WearVisorPanel>().StartTimer());

        _statePattern.WearVisorPanel.SetActive(false);
        //GvrCursorHelper.HeadEmulationActive = true;
        foreach (Button btn in buttons)
            btn.enabled = true;

        _statePattern.VRManager.TurnOnHybridInput();
    }

    List<XRNodeState> nodeStatesCache = new List<XRNodeState>();
    public bool TryGetCenterEyeNodeStateRotation(out Quaternion rotation)
    {
        InputTracking.GetNodeStates(nodeStatesCache);
        for (int i = 0; i < nodeStatesCache.Count; i++)
        {
            XRNodeState nodeState = nodeStatesCache[i];
            if (nodeState.nodeType == XRNode.CenterEye)
            {
                if (nodeState.TryGetRotation(out rotation))
                {
                    //Debug.Log(rotation);
                    return true;
                }
            }
        }
        rotation = Quaternion.identity;
        return false;
    }

    //From VR to 2D 
    public void ResetCameras()
    {
        for (int i = 0; i < Camera.allCameras.Length; i++)
        {
            Camera cam = Camera.allCameras[i];
            if (cam.enabled && cam.stereoTargetEye != StereoTargetEyeMask.None)
            {
                cam.transform.localPosition = Vector3.zero;
                cam.transform.localRotation = Quaternion.identity;
            }
        }
    }
}

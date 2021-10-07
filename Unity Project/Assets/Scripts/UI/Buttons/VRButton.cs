using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.XR;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Map;

public class VRButton : MonoBehaviour
{
    SimulationStatePattern _statePattern;
    Button _button;

    void Start()
    {
        _statePattern = GameObject.FindObjectOfType<SimulationStatePattern>();
        _button = this.GetComponent<Button>();

        _button.onClick.AddListener(VRClick);
    }

    void Update()
    {
        
    }

    void VRClick()
    {
        if (_button.GetComponentInChildren<Text>().text.Equals("VR"))
        {
            StartCoroutine(_statePattern.VRManager.SwitchToVR());
        }
        else
            _statePattern.VRManager.SwitchTo2D();
    }
}

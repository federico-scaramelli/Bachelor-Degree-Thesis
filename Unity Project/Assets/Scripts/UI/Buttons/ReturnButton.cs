using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReturnButton : MonoBehaviour
{
    //REFERENCES
    SimulationStatePattern _statePattern;
    Geocoder _geocoder;

    Button _button;

    void Start()
    {
        _statePattern = GameObject.FindObjectOfType<SimulationStatePattern>();  
        _button = this.GetComponent<Button>();
        _button.onClick.AddListener(Click);
    }

    void Click()
    {
        if (_statePattern.currentState == _statePattern.streetViewState)
        {
            _statePattern.currentState.ToNotStartedState();

            //if (_statePattern.previousState == _statePattern.arrivedState)
            //{
            //    _statePattern.currentState.ToNotStartedState();
            //}
            //else if (_statePattern.previousState == _statePattern.notStartedState)
            //{
            //    _statePattern.currentState.ToNotStartedState();
            //}
            //else
            //    Debug.LogError("Si è raggiunto lo stato SV in maniera ambigua.");
        }
    }
}

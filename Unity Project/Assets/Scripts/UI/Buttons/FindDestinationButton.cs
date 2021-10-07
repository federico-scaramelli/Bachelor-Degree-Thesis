using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindDestinationButton : MonoBehaviour
{
    SimulationStatePattern _statePattern;
    Button _button;

    void Awake()
    {
        _statePattern = FindObjectOfType<SimulationStatePattern>();
        _button = this.GetComponent<Button>();
        _button.onClick.AddListener(Click);
    }

    void Click()
    {
        _statePattern.DestinationCalc.FindDestinationPoint();
    }

    void Update()
    {
        if(_statePattern.Time.CompareTo(0) > 0 && _statePattern.Speed.CompareTo(0) > 0 && _statePattern.fromGeocoded)
            _button.interactable = true;
        else
            _button.interactable = false;
    }
}

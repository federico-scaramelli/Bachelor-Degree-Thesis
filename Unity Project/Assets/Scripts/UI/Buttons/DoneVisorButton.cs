using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoneVisorButton : MonoBehaviour
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
        this.GetComponentInParent<WearVisorPanel>().useVR = true;
        this.GetComponentInParent<WearVisorPanel>().StopTimer();
    }
}

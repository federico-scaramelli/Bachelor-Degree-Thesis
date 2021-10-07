using System.Collections;
using UnityEngine.UI;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using UnityEngine;

public class StopButton : MonoBehaviour
{
    //REFERENCES
    SimulationStatePattern _statePattern;
    Button _button;


    void Start()
    {
        _statePattern = GameObject.FindObjectOfType<SimulationStatePattern>();

        _button = this.GetComponent<Button>();
        _button.onClick.AddListener(Click);
    }

    public void Click()
    {
        _statePattern.LoadingPanel.SetActive(true);
        _statePattern.currentState.ToNotStartedState();
    }
}

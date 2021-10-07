using UnityEngine.UI;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Map;
using UnityEngine;

public class PlayButton : MonoBehaviour
{
    //REFERENCES
    SimulationStatePattern _statePattern;

    Button _button;

    void Start()
    {
        _statePattern = GameObject.FindObjectOfType<SimulationStatePattern>();

        _button = this.GetComponent<Button>();
        _button.onClick.AddListener(PlayClick);
    }

    void Update()
    {
        if (_statePattern.readyToStart)
            _button.interactable = true;
        else
            _button.interactable = false;
    }

    public void PlayClick()
    {
        _statePattern.currentState.ToWalkingState();
    }
}

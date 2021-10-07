using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Map;

public class DirectionsButton : MonoBehaviour
{
    DirectionsCalc _directions;
    SimulationStatePattern _statePattern;
    Button _button;

    void Start()
    {
        _statePattern = GameObject.FindObjectOfType<SimulationStatePattern>();
        _directions = GameObject.FindObjectOfType<DirectionsCalc>();
        _button = this.GetComponent<Button>();

        if (_directions == null)
            Debug.LogError("Directions | NullReference");
        else
           _button.onClick.AddListener(DirectionsClick);
    }

    void Update()
    {
        if (_statePattern.Geocoder.ShouldRoot())
            _button.interactable = true;
        else
            _button.interactable = false;

        if (_statePattern.loadingSView)
            _statePattern.DirectionsButton.interactable = false;

        if (!_statePattern.destinationFound || !_statePattern.fromGeocoded)
            _statePattern.DirectionsButton.interactable = false;
    }

    void DirectionsClick()
    {
        _statePattern.LoadingPanel.SetActive(true);
        _directions.CallQuery();
    }
}

using Mapbox.Unity;
using Mapbox.Geocoding;
using System;
using UnityEngine.UI;
using UnityEngine;

public class SaveStartLocationInput : MonoBehaviour
{
    InputField _inputField;

    ForwardGeocodeResource _resource;

    SimulationStatePattern _statePattern;

    bool _hasResponse;

    public bool HasResponse { get => _hasResponse; set => _hasResponse = value; }

    public event Action<ForwardGeocodeResponse> OnGeocoderResponse = delegate { };

    void Awake()
    {
        _inputField = GetComponent<InputField>();
        _inputField.onEndEdit.AddListener(HandleUserEndInput);
        _inputField.onValueChanged.AddListener(HandleUserInputChanging);
        _resource = new ForwardGeocodeResource("");
        _statePattern = GameObject.FindObjectOfType<SimulationStatePattern>();
    }

    void HandleUserInputChanging(string searchString)
    {
        _statePattern.fromGeocoded = false;
    }


    void HandleUserEndInput(string searchString)
    {
        _statePattern.LoadingPanel.SetActive(true);

        HasResponse = false;
        if (!string.IsNullOrEmpty(searchString))
        {
            _resource.Query = searchString;
            MapboxAccess.Instance.Geocoder.Geocode(_resource, HandleGeocoderResponse);
        }else
            _statePattern.LoadingPanel.SetActive(false);
    }

    void HandleGeocoderResponse(ForwardGeocodeResponse response)
    {
        if (response.Features.Count == 0)
            HasResponse = false;
        else
            HasResponse = true;

        if (!HasResponse)
        {
            Debug.Log("No geocode response for " + _inputField.text);
            _statePattern.LoadingPanel.SetActive(false);
            _statePattern.Directions.ReturnToNoDirections();
        }
        else
            OnGeocoderResponse(response);
    }
}
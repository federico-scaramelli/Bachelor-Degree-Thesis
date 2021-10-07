using UnityEngine.UI;
using Mapbox.Utils;
using UnityEngine;

public class StreetViewButton : MonoBehaviour
{
    //REFERENCES
    SimulationStatePattern _statePattern;
    Geocoder _geocoder;
    DestinationCalc _destinationCalc;
    Button _button;
    [HideInInspector]
    public Vector2d _coordinates;
    //TYPE
    enum Type { From, To };
    [SerializeField] Type _type;


    void Start()
    {
        _statePattern = GameObject.FindObjectOfType<SimulationStatePattern>();
        _geocoder = _statePattern.Geocoder;
        _destinationCalc = _statePattern.DestinationCalc;

        _button = this.GetComponent<Button>();
        _button.onClick.AddListener(Click);
    }

    void Update()
    {
        if (_type == Type.To)
        {
            if (_statePattern.destinationFound)
                _button.interactable = true;
            else
                _button.interactable = false;
        }

        if(_type == Type.From)
        {
            if (_statePattern.fromGeocoded)
                _button.interactable = true;
            else
                _button.interactable = false;
        }

        if(_statePattern.loadingSView)
            _statePattern.StreetViewToButton.interactable = false;
    }

    public void Click()
    {
        _statePattern.loadingSView = true;
        if (_type == Type.From)
            _coordinates = _geocoder.StartLocationCoordinates;
        else if (_type == Type.To)
            _coordinates = _destinationCalc.EndLocationCoordinates;
        StartCoroutine(_statePattern.StreetViewGenerator.DownloadImages(_coordinates));
    }
}

using UnityEngine;
using Mapbox.Utils;
using Mapbox.Geocoding;
using Mapbox.Unity.Map.TileProviders;

public class Geocoder : MonoBehaviour
{
    [SerializeField]
    SaveStartLocationInput _startLocation;

    SimulationStatePattern _statePattern;

    Vector2d _startLocationCoordinates;
    public Vector2d StartLocationCoordinates { get => _startLocationCoordinates;}

    private void Awake()
    {
        _statePattern = GameObject.FindObjectOfType<SimulationStatePattern>();
    }

    void Start()
    {
        _startLocation.OnGeocoderResponse += StartLocation_OnGeocoderResponse;
    }

    private void OnDestroy()
    {
        if(_startLocation != null)
        {
            _startLocation.OnGeocoderResponse -= StartLocation_OnGeocoderResponse;
        }
    }

    void StartLocation_OnGeocoderResponse(ForwardGeocodeResponse response)
    {
        //Se NON è gia stato geocodificato lo stesso luogo, aggiorna la mappa al nuovo luogo di partenza;
        if (!_startLocationCoordinates.Equals(response.Features[0].Center))
        {
            //se ho gia calcolato la direzione
            if (_statePattern.Directions._directionsGO != null)
                _statePattern.Directions.ReturnToNoDirections(); //eliminala

            _startLocationCoordinates = response.Features[0].Center;
            _statePattern.Map.UpdateMap(_startLocationCoordinates, _statePattern.Map.Zoom);
            _statePattern.Map.GetComponent<SpawnOnMap>().Spawn(new Vector2d[] { _startLocationCoordinates });
            _statePattern.destinationFound = false;
        }
        else //altrimenti spegni il loading panel
            _statePattern.LoadingPanel.SetActive(false);

        //oppure
        //se la distanza tra la posizione attuale della mappa e il luogo di partenza dovesse essere superiore a X
        if (Vector2d.Distance(response.Features[0].Center, _statePattern.Map.CenterLatitudeLongitude) * 1000 > 5)
        { 
            if(!(_statePattern.Map.TileProvider is PathTileProvider))
            {
                _startLocationCoordinates = response.Features[0].Center;
                _statePattern.Map.UpdateMap(_startLocationCoordinates, _statePattern.Map.Zoom);
                _statePattern.Map.GetComponent<SpawnOnMap>().Spawn(new Vector2d[] { _startLocationCoordinates });
            }
        }
        else //altrimenti spegni il loading panel
            _statePattern.LoadingPanel.SetActive(false);

        _statePattern.fromGeocoded = true;
    }

    public bool ShouldRoot()
    {
        return _startLocation.HasResponse && _statePattern.destinationFound;
    }
}

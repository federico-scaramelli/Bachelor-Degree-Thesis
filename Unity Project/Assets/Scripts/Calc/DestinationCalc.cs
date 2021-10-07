using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Mapbox.Unity.Map.TileProviders;
using Mapbox.Unity;
using Mapbox.Utils;
using Mapbox.CheapRulerCs;

public class DestinationCalc : MonoBehaviour
{
    SimulationStatePattern _statePattern;

    double[] _startLocationCoordinates = new double[2];

    Transform marker;

    Vector2d _endLocationCoordinates;
    public Vector2d EndLocationCoordinates { get => _endLocationCoordinates; }

    CheapRuler ruler;


    void Awake()
    {
        _statePattern = FindObjectOfType<SimulationStatePattern>();
    }


    public void FindDestinationPoint()
    {
        if(_statePattern.Map.TileProvider is PathTileProvider)
        {
            _statePattern.Directions.ReturnToNoDirections();
        }


        _startLocationCoordinates[0] = _statePattern.Geocoder.StartLocationCoordinates.x;
        _startLocationCoordinates[1] = _statePattern.Geocoder.StartLocationCoordinates.y;
        if (ruler != null)
            ruler = null;
        ruler = new CheapRuler(_startLocationCoordinates[0], CheapRulerUnits.Meters);

        double distance = _statePattern.Speed * (_statePattern.Time / 60) * 1000;

        double[] endLocation = ruler.Destination(_startLocationCoordinates, distance, Random.Range(0, 360));
        //double[] endLocation = ruler.Destination(_startLocationCoordinates, distance, -120);
        _endLocationCoordinates.x = endLocation[0];
        _endLocationCoordinates.y = endLocation[1];

        _statePattern.Map.UpdateMap(_endLocationCoordinates, _statePattern.Map.Zoom);

        _statePattern.Map.GetComponent<SpawnOnMap>().Spawn(new Vector2d[] { _statePattern.Geocoder.StartLocationCoordinates, _endLocationCoordinates});

        _statePattern.destinationFound = true;
    }
}

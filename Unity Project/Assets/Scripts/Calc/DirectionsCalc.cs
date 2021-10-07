using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Directions;
using Mapbox.Unity.MeshGeneration.Modifiers;
using Mapbox.Unity.MeshGeneration.Data;
using Mapbox.Unity;
using Mapbox.Utils;
using Mapbox.Unity.Utilities;
using Mapbox.Unity.Map.TileProviders;

public class DirectionsCalc : MonoBehaviour
{
    Geocoder _geocoder;
    AbstractMap _map;
    SimulationStatePattern _statePattern;
    DestinationCalc _destinationCalc;
    
    private Directions _directions;
    [HideInInspector]
    public GameObject _directionsGO = null;
    private int _counter;

    Vector2d[] _waypoints = new Vector2d[2];

    [SerializeField]
    MeshModifier[] meshModifiers;
    [SerializeField]
    Material _material;

    private List<Vector2d> _pathPoints = new List<Vector2d>();
    public List<Vector2d> PathPoints { get => _pathPoints; set => _pathPoints = value; }


    private void Start()
    {
        _statePattern = GameObject.FindObjectOfType<SimulationStatePattern>();
        _directions = MapboxAccess.Instance.Directions;
        _geocoder = _statePattern.Geocoder;
        _destinationCalc = _statePattern.DestinationCalc;
        _map = _statePattern.Map;

        foreach (var modifier in meshModifiers)
        {
            modifier.Initialize();
        }
    }


    public void CallQuery()
    {
        _waypoints[0] = _geocoder.StartLocationCoordinates;
        _waypoints[1] = _destinationCalc.EndLocationCoordinates;

        _map.SetZoom(_statePattern.DirectionsZoom);

        Query();
    }


    public void Query()
    {
        _statePattern.readyToStart = false;

        var _directionResource = new DirectionResource(_waypoints, RoutingProfile.Walking);
        _directionResource.Steps = true;
        //Debug.Log(_directionResource.GetUrl());
        
        _directions.Query(_directionResource, HandleDirectionsResponse);
    }

    [HideInInspector]
    public List<Vector3> dat = new List<Vector3>();
    void HandleDirectionsResponse(DirectionsResponse response)
    {
        if(response == null || response.Routes == null || response.Routes.Count < 1)
        {
            Debug.Log("No directions from " + _statePattern.FromInputField.text + " to destination.");
            _statePattern.readyToStart = false;
            _statePattern.LoadingPanel.SetActive(false);

            if (_directionsGO != null)
                ReturnToNoDirections();

            return;
        }

       
        //if (_directionsGO != null)
            ReturnToNoDirections();

        _statePattern.LoadingPanel.SetActive(true);

        _directionsGO = new GameObject("Direction path");

        if (!(_statePattern.Map.TileProvider is PathTileProvider))
        {
            _statePattern.Map.SetExtent(MapExtentType.AlongPath);
        }
        _statePattern.Map.Options.extentOptions.defaultExtents.alongPathOptions.SetOptions(response, _statePattern.AlongPathNearTiles);

        if (_statePattern.currentState == _statePattern.notStartedState)
            _statePattern.Map.UpdateMap(_waypoints[0], _statePattern.DirectionsZoom);
        else if (_statePattern.currentState == _statePattern.walkingState)
            _statePattern.Map.UpdateMap(_waypoints[0], _statePattern.SimulationZoom);
    }


    public void CalculatePointsData()
    {
        dat = new List<Vector3>();
        foreach (var point in PathPoints)
        {
            float h = _map.QueryElevationInUnityUnitsAt(point);

            Vector3 location = Conversions.GeoToWorldPosition(point.x, point.y, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz();
            location = new Vector3(location.x, h + (float)_statePattern.PathOffset, location.z);

            dat.Add(location);
        }

        var feat = new VectorFeatureUnity();
        feat.Points.Add(dat);

        var meshData = new MeshData();
        foreach (MeshModifier mod in meshModifiers.Where(x => x.Active))
        {
            mod.Run(feat, meshData, _map.WorldRelativeScale);
        }

        if(!(_statePattern.currentState == _statePattern.walkingState))
            CreatePathGO(meshData);
        else
        {
            _statePattern.LoadingPanel.SetActive(false);
            _statePattern.readyToStart = true;
            _directionsGO.transform.SetParent(_map.Root);
            _directionsGO.transform.position = _map.transform.position;

        }
    }
    

    GameObject CreatePathGO(MeshData data)
    {
        _directionsGO.transform.position = _map.transform.position;
        var mesh = _directionsGO.AddComponent<MeshFilter>().mesh;
        mesh.subMeshCount = data.Triangles.Count;
        mesh.SetVertices(data.Vertices);

        _counter = data.Triangles.Count;
        for (int i = 0; i < _counter; i++)
        {
            var triangle = data.Triangles[i];
            mesh.SetTriangles(triangle, i);
        }

        _counter = data.UV.Count;
        for(int i = 0; i < _counter; i++)
        {
            var uv = data.UV[i];
            mesh.SetUVs(i, uv);
        }

        mesh.RecalculateNormals();
        _directionsGO.AddComponent<MeshRenderer>().material = _material;

        _statePattern.readyToStart = true;

        _directionsGO.transform.SetParent(_map.Root);

        if(_statePattern.currentState == _statePattern.notStartedState)
        {
            _statePattern.Map.gameObject.GetComponent<QuadTreeCamMov>().enabled = false;
            _statePattern.MainCam.gameObject.GetComponent<CameraHandler>().enabled = true;
            //move cam to waypoint 0
            _statePattern.MainCam.transform.position = new Vector3(_statePattern.Directions.dat[0].x, _statePattern.MainCam.transform.position.y, _statePattern.Directions.dat[0].z);
        }

        _statePattern.LoadingPanel.SetActive(false);

        return _directionsGO;
    }

    public void ReturnToNoDirections()
    {
        if(_directionsGO != null)
        {
            _directionsGO.Destroy();
            _directionsGO = null;
            _statePattern.readyToStart = false;
        }

        _statePattern.MainCam.fieldOfView = _statePattern.FovDirections;
        _statePattern.Map.SetExtent(MapExtentType.CameraBounds);
        _statePattern.Map.Options.extentOptions.defaultExtents.cameraBoundsOptions.SetOptions(_statePattern.MainCam, 1, 1);

        if (_statePattern.currentState == _statePattern.notStartedState)
        {
            _statePattern.Map.gameObject.GetComponent<QuadTreeCamMov>().enabled = true;
            _statePattern.MainCam.gameObject.GetComponent<CameraHandler>().enabled = false;
        }


        //move cam to waypoint 0
        Vector3 startLocation = Conversions.GeoToWorldPosition(_statePattern.Geocoder.StartLocationCoordinates, _map.CenterMercator, _map.WorldRelativeScale).ToVector3xz();
        _statePattern.MainCam.transform.position = new Vector3(startLocation.x, _statePattern.MainCam.transform.position.y, startLocation.z);

        _statePattern.Map.UpdateMap(_waypoints[0], _statePattern.DirectionsZoom);
    }
}

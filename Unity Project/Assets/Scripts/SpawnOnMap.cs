using UnityEngine;
using Mapbox.Utils;
using Mapbox.Unity.Map;
using Mapbox.Unity.MeshGeneration.Factories;
using Mapbox.Unity.Utilities;
using System.Collections.Generic;

public class SpawnOnMap : MonoBehaviour
{

    SimulationStatePattern _statePattern;
    AbstractMap _map;

    Vector2d[] _locations;

	[SerializeField]
	float _spawnScale = 100f;

	Transform _markerPrefab;

	List<Transform> _spawnedObjects;


    void Start()
    {
        _statePattern = FindObjectOfType<SimulationStatePattern>();
        _map = _statePattern.Map;
        _markerPrefab = _statePattern.MarkerPrefab;
        _spawnedObjects = null;
    }

    public void Clear()
    {
        foreach (Transform t in _spawnedObjects)
        {
            t.gameObject.Destroy();
        }
        _spawnedObjects.Clear();
        _spawnedObjects = null;
    }

	public void Spawn(Vector2d[] _locations)
	{
        if (_spawnedObjects != null)
        {
            Clear();
        }

        this._locations = _locations;

		_spawnedObjects = new List<Transform>();
		for (int i = 0; i < _locations.Length; i++)
		{
			var instance = Instantiate(_markerPrefab);
			instance.transform.localPosition = _map.GeoToWorldPosition(_locations[i], true);
			instance.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
			_spawnedObjects.Add(instance);
		}
	}

	private void Update()
	{
        if(_spawnedObjects != null)
        {
            int count = _spawnedObjects.Count;
            for (int i = 0; i < count; i++)
            {
                var spawnedObject = _spawnedObjects[i];
                var location = _locations[i];
                spawnedObject.transform.localPosition = _map.GeoToWorldPosition(location, true);
                spawnedObject.transform.localScale = new Vector3(_spawnScale, _spawnScale, _spawnScale);
            }
        }
	}
}
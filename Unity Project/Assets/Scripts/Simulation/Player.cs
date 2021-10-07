using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.CheapRulerCs;
using Mapbox.Utils;


public class Player : MonoBehaviour
{
    Vector3 previousPos = Vector3.zero;
    Vector3 deltaPos = Vector3.zero;

    SimulationStatePattern _statePattern;
    DirectionsCalc _directions;

    Vector2d prevPosCoord;
    Vector2d nextPosCoord;
    CheapRuler ruler;

    bool moving;

    bool moveToInExecution = false;

    public void Start()
    {
        _statePattern = GameObject.FindObjectOfType<SimulationStatePattern>();
        _directions = _statePattern.Directions;

        GetPositions();
    }

    
    void Update()
    {
        if (!_statePattern.WearVisorPanel.activeInHierarchy)
        {
            if(!moveToInExecution)
                StartCoroutine(MoveTo());

            if (Application.platform == RuntimePlatform.Android)
            {
                Quaternion rot;
                if (_statePattern.VRManager.TryGetCenterEyeNodeStateRotation(out rot))
                {
                    _statePattern.PlayerCam.transform.localRotation = rot;
                }
            }
        }
        else
        {
            this.StopAllCoroutines();
            moveToInExecution = false;
        }
    }

    List<Vector3> futurePositions;
    bool interruption;
    public void GetPositions()
    {
        futurePositions = _directions.dat;

        if (futurePositions != null && moving)
        {
            interruption = true;
        }
        if (!moving)
        {
            interruption = false;
            MoveToNextPlace();
        }
    }

    public Vector3 nextPos;
    public void MoveToNextPlace()
    {
        if (futurePositions.Count > 0)
        {
            nextPos = futurePositions[0];
            futurePositions.Remove(nextPos);
            nextPos.y += (float)_statePattern.SimCameraOffset;

            moving = true;
            StartCoroutine(MoveTo());
        }
        else if (futurePositions.Count <= 0)
        {
            moving = false;

            _statePattern.LoadingPanel.SetActive(true);
            _statePattern.VRManager.TurnOnMouseInput();
            _statePattern.PlayerCam.enabled = false;
            _statePattern.MainCam.enabled = true;
            _statePattern.StartCoroutine(_statePattern.StreetViewGenerator.DownloadImages(_statePattern.DestinationCalc.EndLocationCoordinates));
        }
    }

    Vector3 prevPos;
    public IEnumerator MoveTo()
    {
        moveToInExecution = true;

        prevPos = transform.position;

        //GameObject.Instantiate(_statePattern.MarkerPrefab, nextPos, Quaternion.identity);

        prevPosCoord = _statePattern.Map.WorldToGeoPosition(prevPos);
        nextPosCoord = _statePattern.Map.WorldToGeoPosition(nextPos);

        double time = CalculateTime();
        float t = 0;

        StartCoroutine(LookAtNextPos());

        while (t < 1 && !interruption)
        {
            t += Time.deltaTime / (float)time;

            transform.localPosition = Vector3.Lerp(prevPos, nextPos, t);

            yield return null;
        }

        interruption = false;
        MoveToNextPlace();
    }

    double CalculateTime()
    {
        double timeToMove = 0;


        if (ruler != null)
            ruler = null;
        //ruler = new CheapRuler(prevPosCoord.x, CheapRulerUnits.Kilometers);
        Mapbox.Map.UnwrappedTileId id = Conversions.LatitudeLongitudeToTileId(prevPos.x, prevPos.y, _statePattern.Map.AbsoluteZoom);
        ruler = CheapRuler.FromTile(id.Y, id.Z, CheapRulerUnits.Kilometers);

        double[] a = new double[] { prevPosCoord.x, prevPosCoord.y };
        double[] b = new double[] { nextPosCoord.x, nextPosCoord.y };
        
        timeToMove = (ruler.Distance(a,b) / (_statePattern.Speed)); //in ore
        timeToMove = timeToMove * 60 * 60; //in secondi 

        //Debug.Log(prevPosCoord + "   -   " + nextPosCoord);
        //Debug.Log(ruler.Distance(a, b));
        //Debug.Log(timeToMove);

        return timeToMove;
    }

    IEnumerator LookAtNextPos()
    {
        Quaternion neededRotation = Quaternion.LookRotation(nextPos - transform.position);
        Quaternion thisRotation = transform.localRotation;

        float t = 0;
        while (t < 1.0f)
        {
            t += Time.deltaTime / 0.25f;
            var rotationValue = Quaternion.Slerp(thisRotation, neededRotation, t);
            transform.rotation = Quaternion.Euler(0, rotationValue.eulerAngles.y, 0);
            yield return null;
        }
    }

    void OnDestroy()
    {
        _statePattern.Player = null;
        _statePattern.PlayerCam = null;
        //_statePattern.PlayerReticle = null;
        _statePattern.PlayerPanel = null;
        _statePattern.PlayerVRButton = null;
    }
}

using System.Collections;
using System.Collections.Generic;
using Mapbox.Map;
using Mapbox.Utils;
using UnityEngine;

namespace Mapbox.Unity.Map.TileProviders
{
    public class PathTileProvider : AbstractTileProvider
    {
        private SimulationStatePattern _statePattern;

        [SerializeField] private PathTileProviderOptions _pathTileProviderOptions;
        private bool _initialized = false;

        public override void OnInitialized()
        {
            _statePattern = GameObject.Find("SimulationManager").GetComponent<SimulationStatePattern>();
            _statePattern.Directions.PathPoints = new List<Vector2d>();

            if (Options != null)
            {
                _pathTileProviderOptions = (PathTileProviderOptions)Options;
            }
            else
            {
                _pathTileProviderOptions = new PathTileProviderOptions();
            }

            _initialized = true;
            _currentExtent.activeTiles = new HashSet<UnwrappedTileId>();
        }

        public IEnumerator UpdateTiles()
        {
            


            int pointsLength = _pathTileProviderOptions.response.Routes[0].Geometry.Count;
            List<Vector2d> points = _pathTileProviderOptions.response.Routes[0].Geometry;
            Vector2d lastPoint = points[pointsLength - 1];

            for (int i = 0; i < pointsLength; i++)
            {
                Vector2d point = points[i];
                Vector2d nextPoint;
                double m;

                if (!point.Equals(lastPoint))
                    nextPoint = points[i + 1];
                else
                    nextPoint = new Vector2d(-1, -1);


                yield return addTile(point.x, point.y);

                if (!nextPoint.x.Equals(-1) && !nextPoint.y.Equals(-1))
                {
                    if (!point.y.Equals(nextPoint.y) && !point.x.Equals(nextPoint.x))
                    {
                        m = (point.y - nextPoint.y) / (point.x - nextPoint.x);

                        if (point.x.CompareTo(nextPoint.x) < 0) //current < next 
                        {
                            for (double x = point.x; x <= nextPoint.x; x += _statePattern.DirectionPointsDistance)
                            {
                                double y = (m * x) - (m * point.x) + point.y;
                                yield return addTile(x, y);
                            }
                        }
                        else //current > next 
                        {
                            for (double x = point.x; x >= nextPoint.x; x -= _statePattern.DirectionPointsDistance)
                            {
                                double y = (m * x) - (m * point.x) + point.y;
                                yield return addTile(x, y);
                            }
                        }
                    }
                    else if (point.y.Equals(nextPoint.y) && !point.x.Equals(nextPoint.x))
                    {
                        m = 0;
                        //y=y1
                        if (point.x.CompareTo(nextPoint.x) < 0)
                        {
                            for (double x = point.x; x <= nextPoint.x; x += _statePattern.DirectionPointsDistance)
                            {
                                double y = point.y;
                                yield return addTile(x, y);
                            }
                        }
                        else
                        {
                            for (double x = point.x; x >= nextPoint.x; x -= _statePattern.DirectionPointsDistance)
                            {
                                double y = point.y;
                                yield return addTile(x, y);
                            }
                        }
                    }
                    else if (!point.y.Equals(nextPoint.y) && point.x.Equals(nextPoint.x))
                    {
                        m = 0;
                        //x=x1
                        if (point.y.CompareTo(nextPoint.y) < 0)
                        {
                            for (double y = point.y; y <= nextPoint.y; y += _statePattern.DirectionPointsDistance)
                            {
                                double x = point.x;
                                yield return addTile(x, y);
                            }
                        }
                        else
                        {
                            for (double y = point.y; y >= nextPoint.y; y -= _statePattern.DirectionPointsDistance)
                            {
                                double x = point.x;
                                yield return addTile(x, y);
                            }
                        }
                    }
                    else
                        Debug.LogError("A=B");
                }
                //yield return new WaitForSeconds(0.001f);
                yield return null;
            }
            OnExtentChanged();
        }

        public override void UpdateTileExtent()
        {
            if (!_initialized || _pathTileProviderOptions == null)
            {
                return;
            }

            _currentExtent.activeTiles.Clear();

            StartCoroutine("UpdateTiles");
        }

        private IEnumerator addTile(double x, double y)
        {

            //Debug.Log("Adding tile "+x+" "+y);

            _pathTileProviderOptions.visibleBuffer = _statePattern.AlongPathNearTiles;
            Vector2d point = new Vector2d(x, y);
            if(!_statePattern.Directions.PathPoints.Contains(point))
                _statePattern.Directions.PathPoints.Add(point);

            UnwrappedTileId tile = new UnwrappedTileId();

            tile = TileCover.CoordinateToTileId(point, _map.AbsoluteZoom);

            if (!_currentExtent.activeTiles.Contains(tile))
                _currentExtent.activeTiles.Add(tile);
            
            for (int _x = tile.X - _pathTileProviderOptions.visibleBuffer; _x <= (tile.X + _pathTileProviderOptions.visibleBuffer); _x++)
            {
                for (int _y = tile.Y - _pathTileProviderOptions.visibleBuffer; _y <= (tile.Y + _pathTileProviderOptions.visibleBuffer); _y++)
                {
                    var nearTile = new UnwrappedTileId(_map.AbsoluteZoom, _x, _y);
                    if (!_currentExtent.activeTiles.Contains(nearTile))
                        _currentExtent.activeTiles.Add(nearTile);
                    //yield return new WaitForSeconds(0.001f);
                }
                //yield return new WaitForSeconds(0.001f);
            }
            //yield return new WaitForSeconds(0.001f);
            yield return null;
        }

        public override bool Cleanup(UnwrappedTileId tile)
        {
            return (!_currentExtent.activeTiles.Contains(tile));
        }

    }
}

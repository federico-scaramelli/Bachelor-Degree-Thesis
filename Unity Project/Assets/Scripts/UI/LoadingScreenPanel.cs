using UnityEngine;
using Mapbox.Unity.Map;
using UnityEngine.UI;

public class LoadingScreenPanel : MonoBehaviour
{
    SimulationStatePattern _statePattern;

    [SerializeField]
    Text _text;
    [SerializeField]
    Image _background;

    [SerializeField]
    AnimationCurve _curve;

    AbstractMap _map;
    void Awake()
    {
        _statePattern = FindObjectOfType<SimulationStatePattern>();

        _map = _statePattern.Map;
        _map.OnInitialized += _map_OnInitialized;
    }

    void _map_OnInitialized()
    {

        var visualizer = _map.MapVisualizer;
        _text.text = "LOADING";
        visualizer.OnMapVisualizerStateChanged += (s) =>
        {

            if (this == null)
                return;

            if (s == ModuleState.Finished)
            {
                _statePattern.LoadingPanel.SetActive(false);
                //Debug.Log("LOADED");
            }
            else if (s == ModuleState.Working)
            {
                //Debug.Log("LOADING");
                _statePattern.LoadingPanel.SetActive(true);
            }

        };
    }

    void Update()
    {
        var t = _curve.Evaluate(Time.time);
        _text.color = Color.Lerp(Color.clear, Color.white, t);
        _background.color = Color.Lerp(new Color(0,0,0,0.5f), new Color(0, 0, 0, 0.8f), t);
    }
}
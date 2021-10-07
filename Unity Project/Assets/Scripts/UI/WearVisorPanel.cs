using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WearVisorPanel : MonoBehaviour
{
    [SerializeField]
    GameObject _content;
    [SerializeField]
    Text _text;

    [SerializeField]
    string baseText = "Wear your visor!";

    float timeToWear;
    private float _timer;

    [HideInInspector]
    public bool useVR = true;

    [SerializeField]
    AnimationCurve _curve;

    SimulationStatePattern _statePattern;

    void Awake()
    {
        _statePattern = FindObjectOfType<SimulationStatePattern>();

        timeToWear = _statePattern.TimeToWear;

        _text.text = baseText + "\n" + timeToWear.ToString();
    }

    public void StopTimer()
    {
        _timer = 0;
    }

    public void ResetTimer()
    {
        _timer = timeToWear;
    }

    public IEnumerator StartTimer()
    {
        ResetTimer();
        _text.text = baseText + "\n" + _timer.ToString();

        while (_timer > 0)
        {
            yield return new WaitForSeconds(1.0f);
            _timer--;
            _text.text = baseText + "\n" + _timer.ToString();
        }
    }

    void Update()
    {
        var t = _curve.Evaluate(Time.time);
        _text.color = Color.Lerp(new Color(1,1,1,0.1f), Color.white, t);
    }
}

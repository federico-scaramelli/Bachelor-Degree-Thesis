using UnityEngine;
using UnityEngine.UI;

public class SaveInputTime : MonoBehaviour
{
    SimulationStatePattern _statePattern;

    void Awake()
    {
        _statePattern = FindObjectOfType<SimulationStatePattern>(); 
    }

    public void SaveTime()
    {
        string text = this.GetComponent<InputField>().text;
        if (!text.Equals(string.Empty))
            _statePattern.Time = float.Parse(text);
        else
            _statePattern.Time = 0;
    }
}

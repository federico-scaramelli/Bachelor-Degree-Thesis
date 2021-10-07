using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveInputSpeed : MonoBehaviour
{
    SimulationStatePattern _statePattern;

    void Awake()
    {
        _statePattern = FindObjectOfType<SimulationStatePattern>();
    }

    public void SaveSpeed()
    {
        string text = this.GetComponent<InputField>().text;
        if(!text.Equals(string.Empty))
            _statePattern.Speed = float.Parse(text);
        else
            _statePattern.Speed = 0;
    }
}

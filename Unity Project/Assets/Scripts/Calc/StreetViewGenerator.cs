using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Mapbox.Utils;

public class StreetViewGenerator : MonoBehaviour
{
    private SimulationStatePattern _statePattern;
    private Button _button;
    //right; left; bottom; top; front; back;
    private Texture2D[] _cubefaceTextures = new Texture2D[6];
    private readonly CubemapFace[] _cubemapFaces = new CubemapFace[] {
        CubemapFace.PositiveX, CubemapFace.NegativeX,
        CubemapFace.PositiveY, CubemapFace.NegativeY,
        CubemapFace.PositiveZ, CubemapFace.NegativeZ
    };
    private readonly int[] _heading = new int[] { 90, 270, 0, 0, 0, 180 };
    private readonly int[] _pitch = new int[] { 0, 0, -90, 90, 0, 0 };
    private readonly Color[] pixels = {
        new Color(0.906f, 0.902f, 0.894f, 1.000f),
        new Color(0.910f, 0.906f, 0.898f, 1.000f),
        new Color(0.898f, 0.894f, 0.886f, 1.000f),
        new Color(0.878f, 0.875f, 0.867f, 1.000f),
        new Color(0.867f, 0.863f, 0.855f, 1.000f),
        new Color(0.875f, 0.871f, 0.863f, 1.000f),
        new Color(0.882f, 0.878f, 0.871f, 1.000f),
        new Color(0.882f, 0.878f, 0.871f, 1.000f),
        new Color(0.851f, 0.847f, 0.839f, 1.000f),
        new Color(0.863f, 0.859f, 0.851f, 1.000f),
    };
    bool founded;

    void Awake()
    {
        _statePattern = FindObjectOfType<SimulationStatePattern>();
    }

    public IEnumerator DownloadImages(Vector2d _coordinates)
    {
        //Debug.Log("Downloading images");

        _statePattern.LoadingPanel.SetActive(true);

        for (int i = 0; i < 6; i++)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(
                "https://maps.googleapis.com/maps/api/streetview?size=512x512&location=" +
                _coordinates.x.ToString(CultureInfo.InvariantCulture) + "," + _coordinates.y.ToString(CultureInfo.InvariantCulture) +
                "&heading=" + _heading[i] + "&pitch=" + _pitch[i] + "&key=AIzaSyBP0njGliiPjbNHxBtfqKzA51hogwCORUg");

            //UnityWebRequest www = UnityWebRequestTexture.GetTexture(
            //    "https://maps.googleapis.com/maps/api/streetview?size=512x512&location=" +
            //    _statePattern.FromInputField.text +
            //    "&heading=" + _heading[i] + "&pitch=" + _pitch[i] + "&key=AIzaSyBP0njGliiPjbNHxBtfqKzA51hogwCORUg");

            www.timeout = 5;
            yield return www.SendWebRequest();


            if (www.isNetworkError)
            {
                Debug.Log("Riavvio il download dell'immagine n." + i);
                i--;
                continue; //=i++
            }


            _cubefaceTextures[i] = DownloadHandlerTexture.GetContent(www);
        }


        if (!CheckIfImageExists(_cubefaceTextures[0]))
        {
            Debug.Log("Immagine non trovata");
            _statePattern.LoadingPanel.SetActive(false);
            _statePattern.SVNotFoundPanel.SetActive(true);
        }
        else
            GenerateCubemap();
    }

    void GenerateCubemap()
    {
        Cubemap _cubemap = _statePattern.Cubemap;
        for (int i = 0; i < 6; i++)
        {
            _cubemap.SetPixels(_cubefaceTextures[i].GetPixels(), _cubemapFaces[i]);
        }
        _cubemap.SmoothEdges();
        _cubemap.Apply();

        _statePattern.loadingSView = false;

        if (_statePattern.MainCam.enabled)
            _statePattern.MainCam.enabled = false;
        _statePattern.currentState.ToStreetViewState();
    }

    private bool CheckIfImageExists(Texture2D img)
    {
        bool founded = false;
        for (int j = 0; j < 10; j++)
        {
            if (!ComparePixels(_cubefaceTextures[0].GetPixel(j, 1), pixels[j]))
                founded = true;
        }
        return founded;
    }

    private bool ComparePixels(Color a, Color b)
    {
        return a.ToString().Equals(b.ToString());
    }

    public void Return()
    {
        _statePattern.SVNotFoundPanel.SetActive(false);
        _statePattern.loadingSView = false;
    }
}

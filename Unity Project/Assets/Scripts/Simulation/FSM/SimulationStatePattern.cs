using UnityEngine.UI;
using UnityEngine;
using UnityEngine.XR;
using Mapbox.Unity.Map;
using Mapbox.Unity.Map.TileProviders;
using Mapbox.Utils;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class SimulationStatePattern : MonoBehaviour
{
    #region Menu UI References
    [Header("Menu UI References")]
    [SerializeField]
    private GameObject _menuPanel;

    [SerializeField]
    private Button _streetViewFromButton;

    [SerializeField]
    private Button _streetViewToButton;

    [SerializeField]
    private InputField _fromInputField;

    [SerializeField]
    private InputField _speedInputField;

    [SerializeField]
    private InputField _timeInputField;

    [SerializeField]
    private Button _findDestinationButton;

    [SerializeField]
    private Button _directionsButton;

    [SerializeField]
    private Button _playButton;
    #endregion


    #region StreetView UI References
    [Header("StreetView UI References")]
    [SerializeField]
    private GameObject _svPanel;

    [SerializeField]
    private Button _svVRButton;

    [SerializeField]
    private GameObject _svNotFoundPanel;
    #endregion


    #region Simulation UI References
    [Header("Simulation UI References")]
    [SerializeField]
    private GameObject _playerPanel;

    [SerializeField]
    private Button _playerVRButton;

    [SerializeField]
    private Button _pauseButton;

    [SerializeField]
    private Button _stopButton;
    #endregion


    #region UI References
    [Header("UI References")]
    [SerializeField]
    private GameObject _wearVisorPanel;

    [SerializeField]
    private GameObject _loadingPanel;
    #endregion


    #region Simulation References
    [Header("Simulation References")]
    [SerializeField]
    private Transform _playerPrefab;

    private Transform _player;

    private DestinationCalc _destinationCalc;

    private DirectionsCalc _directions;

    private Geocoder _geocoder;

    private StreetViewGenerator _streetViewGenerator;

    private AbstractMap _map;

    private AbstractMapVisualizer _visualizer;

    private MeshRenderer _sphere;

    [SerializeField]
    private Cubemap _cubemap;

    [SerializeField]
    private Transform _markerPrefab;
    #endregion


    #region VR/Cam References
    [Header("VR/2D References")]
    private VRManager _vrManager;

    private Camera _mainCam;

    private Camera _svCam;

    private GameObject _svCamRig;

    private Camera _playerCam;
    #endregion


    #region Directions Parameters
    [Header("Directions Parameters")]
    [SerializeField]
    [Range(0, 22)]
    private float directionsZoom = 12;

    [SerializeField]
    [Range(0.0001f, 0.01f)]
    private double directionPointsDistance = 0.0025;

    [SerializeField]
    [Range(0, 3)]
    private int _alongPathNearTiles = 1;

    [SerializeField]
    [Range(0, 50)]
    private int pathOffset = 15;

    [SerializeField]
    [Range(10, 50)]
    private float fovDirections = 18;
    #endregion


    #region Simulation Parameters
    [Header("Simulation Parameters")]
    [SerializeField]
    [Range(0, 22)]
    private int simulationZoom = 17;

    [SerializeField]
    [Range(10f, 40f)]
    private float zoomSpeed = 20f;

    [SerializeField]
    [Range(300, 5000)]
    private int dirPanSpeed = 1000;

    [SerializeField]
    [Range(0, 50)]
    private int playerOffset = 30;

    [SerializeField]
    [Range(50, 65)]
    private float fovStreetView = 60;

    [SerializeField]
    [Range(1, 60)]
    private int timeToWear = 15;

    [HideInInspector]
    [Range(3,15)]
    private float speed;

    [HideInInspector]
    [Range(5, 360)]
    private float time;
    #endregion


    #region Encapsulations
    public GameObject MenuPanel { get => _menuPanel; set => _menuPanel = value; }
    public GameObject SVPanel { get => _svPanel; set => _svPanel = value; }
    public Button PlayButton { get => _playButton; set => _playButton = value; }
    public Button StreetViewFromButton { get => _streetViewFromButton; set => _streetViewFromButton = value; }
    public Button StreetViewToButton { get => _streetViewToButton; set => _streetViewToButton = value; }
    public InputField FromInputField { get => _fromInputField; set => _fromInputField = value; }
    public Button DirectionsButton { get => _directionsButton; set => _directionsButton = value; }
    public GameObject LoadingPanel { get => _loadingPanel; set => _loadingPanel = value; }
    public Button PauseButton { get => _pauseButton; set => _pauseButton = value; }
    public Button StopButton { get => _stopButton; set => _stopButton = value; }
    public Button SV_VRButton { get => _svVRButton; set => _svVRButton = value; }
    public Transform PlayerPrefab { get => _playerPrefab; set => _playerPrefab = value; }
    public Transform Player { get => _player; set => _player = value; }
    public DirectionsCalc Directions { get => _directions; set => _directions = value; }
    public Geocoder Geocoder { get => _geocoder; set => _geocoder = value; }
    public AbstractMap Map { get => _map; set => _map = value; }
    public AbstractMapVisualizer Visualizer { get => _visualizer; set => _visualizer = value; }
    public MeshRenderer Sphere { get => _sphere; set => _sphere = value; }
    public float DirectionsZoom { get => directionsZoom; set => directionsZoom = value; }
    public double DirectionPointsDistance { get => directionPointsDistance; set => directionPointsDistance = value; }
    public int AlongPathNearTiles { get => _alongPathNearTiles; set => _alongPathNearTiles = value; }
    public int PathOffset { get => pathOffset; set => pathOffset = value; }
    public float FovDirections { get => fovDirections; set => fovDirections = value; }
    public int SimulationZoom { get => simulationZoom; set => simulationZoom = value; }
    public float ZoomSpeed { get => zoomSpeed; set => zoomSpeed = value; }
    public int DirPanSpeed { get => dirPanSpeed; set => dirPanSpeed = value; }
    public int SimCameraOffset { get => playerOffset; set => playerOffset = value; }
    public float FovStreetView { get => fovStreetView; set => fovStreetView = value; }
    public int TimeToWear { get => timeToWear; set => timeToWear = value; }
    public VRManager VRManager { get => _vrManager; set => _vrManager = value; }
    public GameObject WearVisorPanel { get => _wearVisorPanel; set => _wearVisorPanel = value; }
    public Camera MainCam { get => _mainCam; set => _mainCam = value; }
    public GameObject SVNotFoundPanel { get => _svNotFoundPanel; set => _svNotFoundPanel = value; }
    public Cubemap Cubemap { get => _cubemap; set => _cubemap = value; }
    public StreetViewGenerator StreetViewGenerator { get => _streetViewGenerator; set => _streetViewGenerator = value; }
    public Camera SVCam { get => _svCam; set => _svCam = value; }
    public Camera PlayerCam { get => _playerCam; set => _playerCam = value; }
    public GameObject SVCamRig { get => _svCamRig; set => _svCamRig = value; }
    public InputField SpeedInputField { get => _speedInputField; set => _speedInputField = value; }
    public InputField TimeInputField { get => _timeInputField; set => _timeInputField = value; }
    public Button FindDestinationButton { get => _findDestinationButton; set => _findDestinationButton = value; }
    public float Speed { get => speed; set => speed = value; }
    public float Time { get => time; set => time = value; }
    public DestinationCalc DestinationCalc { get => _destinationCalc; set => _destinationCalc = value; }
    public Transform MarkerPrefab { get => _markerPrefab; set => _markerPrefab = value; }
    public GameObject PlayerPanel { get => _playerPanel; set => _playerPanel = value; }
    public Button PlayerVRButton { get => _playerVRButton; set => _playerVRButton = value; }
    #endregion


    #region Flags
    [Header("Flags")]
    [HideInInspector]
    public bool readyToStart = false;
    [HideInInspector]
    public bool fromGeocoded = false;
    [HideInInspector]
    public bool destinationFound = false;
    [HideInInspector]
    public bool loadingSView = false;
    [HideInInspector]
    public bool started = false;
    [HideInInspector]
    public bool VR_on = false;
    #endregion


    #region FSM
    [Header("Finite State Machine")]
    public ISimulationState currentState;
    public NotStartedState notStartedState;
    public WalkingState walkingState;
    public StreetViewState streetViewState;
    #endregion


    void Awake()
    {
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 0;

        StartCoroutine(TurnOnCardboard());

        #region References
        notStartedState = new NotStartedState(this);
        walkingState = new WalkingState(this);
        streetViewState = new StreetViewState(this);

        Directions = GameObject.FindObjectOfType<DirectionsCalc>();
        Map = GameObject.FindObjectOfType<AbstractMap>();
        Geocoder = GameObject.FindObjectOfType<Geocoder>();
        StreetViewGenerator = FindObjectOfType<StreetViewGenerator>();
        DestinationCalc = FindObjectOfType<DestinationCalc>();
        Sphere = GameObject.FindGameObjectWithTag("Sphere").GetComponent<MeshRenderer>();
        Visualizer = Map.MapVisualizer;
        VRManager = GameObject.FindObjectOfType<VRManager>();

        MainCam = GameObject.FindGameObjectWithTag("MenuCam").GetComponent<Camera>();
        SVCam = GameObject.FindGameObjectWithTag("SVCam").GetComponent<Camera>();
        SVCamRig = SVCam.transform.parent.gameObject;
        #endregion

        TurnOffAllGameObjects();

        Map.OnInitialized += _map_StatusChange;
    }

    IEnumerator TurnOnCardboard()
    {
        XRSettings.enabled = false;
        XRSettings.LoadDeviceByName("cardboard");
        yield return null;
    }

    void TurnOffAllGameObjects()
    {
        Sphere.enabled = false;
        SVCam.enabled = false;
        MainCam.enabled = false;
        MenuPanel.SetActive(false);
        SVPanel.SetActive(false);
        LoadingPanel.SetActive(false);
        MenuPanel.SetActive(false);
        SVNotFoundPanel.SetActive(false);
        WearVisorPanel.SetActive(false);
    }


    void Start()
    {
        currentState = notStartedState;
        currentState.EnterState();
    }

    void Update()
    {
        currentState.UpdateState();
    }

    void _map_StatusChange()
    {
        Visualizer.OnMapVisualizerStateChanged += (s) =>
        {

            if (this == null)
                return;

            if (s == ModuleState.Finished)
            {
                //if (toGeocoded && fromGeocoded && ((_map.Zoom - directionsZoom) < Constants.EpsilonFloatingPoint || (_map.Zoom - simulationZoom) < Constants.EpsilonFloatingPoint))
                if (destinationFound && fromGeocoded && _map.TileProvider is PathTileProvider)
                {
                    if (Directions._directionsGO != null)
                        Directions.CalculatePointsData();
                }
            }
        };
    }
}


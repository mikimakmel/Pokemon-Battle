using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;

public class SC_MenuLogic : MonoBehaviour
{
    private int currentSelection = 1;
    public bool isMenuEnabled = true;
    private Dictionary<string, GameObject> MenuScreens;
    private Dictionary<string, GameObject> MenuObjects;
    private GlobalEnums.MenuScreens currentScreen;
    private Stack screensStack = new Stack();
    public SC_GameLogic SC_GameLogic;
    public SC_DeckMenuLogic SC_DeckMenuLogic;
    public SC_OptionsMenuLogic SC_OptionsMenuLogic;
    public SC_LoadingMenuLogic SC_LoadingMenuLogic;
    public AudioSource menuMusic;
    public AudioSource deckMusic;
    public AudioSource buttonClick;
    public AudioSource backClick;
    public AudioSource sliderClick;
    public Image Img_Fade;

    // Multiplayer
    public Listener listener;
    private Dictionary<string, object> matchRoomData;
    private List<string> roomIds;
    public string userId = "";
    private string currRoomId;
    private int roomIdx = 0;
    private bool isConnected = false;

    static SC_MenuLogic instance;
    public static SC_MenuLogic Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("SC_MenuLogic").GetComponent<SC_MenuLogic>();

            return instance;
        }
    }

    private void OnEnable()
    {
        Listener.OnConnect += OnConnect;
        Listener.OnRoomsInRange += OnRoomsInRange;
        Listener.OnCreateRoom += OnCreateRoom;
        Listener.OnGetLiveRoomInfo += OnGetLiveRoomInfo;
        Listener.OnJoinRoom += OnJoinRoom;
        Listener.OnUserJoinRoom += OnUserJoinRoom;
        Listener.OnGameStarted += OnGameStarted;
        Listener.OnDisconnect += OnDisconnect;
    }

    private void OnDisable()
    {
        Listener.OnConnect -= OnConnect;
        Listener.OnRoomsInRange -= OnRoomsInRange;
        Listener.OnCreateRoom -= OnCreateRoom;
        Listener.OnGetLiveRoomInfo -= OnGetLiveRoomInfo;
        Listener.OnJoinRoom -= OnJoinRoom;
        Listener.OnUserJoinRoom -= OnUserJoinRoom;
        Listener.OnGameStarted -= OnGameStarted;
        Listener.OnDisconnect -= OnDisconnect;
    }

    public IEnumerator fadeIn(float _duration)
    {
        Img_Fade.canvasRenderer.SetAlpha(1f);
        MenuScreens["Screen_FadeIn"].SetActive(true);
        Img_Fade.CrossFadeAlpha(0f, _duration, false);
        yield return new WaitForSeconds(_duration);
        MenuScreens["Screen_FadeIn"].SetActive(false);
    }

    private void initMultiplayer()
    {
        if (listener == null)
            listener = new Listener();

        WarpClient.initialize(Constants.apiKey, Constants.secretKey);
        WarpClient.GetInstance().AddConnectionRequestListener(listener);
        WarpClient.GetInstance().AddChatRequestListener(listener);
        WarpClient.GetInstance().AddUpdateRequestListener(listener);
        WarpClient.GetInstance().AddLobbyRequestListener(listener);
        WarpClient.GetInstance().AddNotificationListener(listener);
        WarpClient.GetInstance().AddRoomRequestListener(listener);
        WarpClient.GetInstance().AddZoneRequestListener(listener);
        WarpClient.GetInstance().AddTurnBasedRoomRequestListener(listener);

        matchRoomData = new Dictionary<string, object>();
        matchRoomData.Add("Level", 0);

        userId = System.DateTime.Now.Ticks.ToString();
        Debug.Log(userId + " is trying to connect...");
        WarpClient.GetInstance().Connect(userId);
    }

    public void Awake()
    {
        InitMenuObjects();
        InitMenuScreens();
        initMultiplayer();
    }

    private void Update()
    {
        HandleMainMenu();
    }

    private void InitMenuObjects()
    {
        MenuObjects = new Dictionary<string, GameObject>();
        GameObject[] _objects2 = GameObject.FindGameObjectsWithTag("MenuObjects");
        foreach (GameObject obj in _objects2)
            MenuObjects.Add(obj.name, obj);
    }

    private void InitMenuScreens()
    {
        MenuScreens = new Dictionary<string, GameObject>();
        GameObject[] _objects = GameObject.FindGameObjectsWithTag("MenuScreens");
        foreach (GameObject obj in _objects)
        {
            MenuScreens.Add(obj.name, obj);
            obj.SetActive(false);
        }

        MenuScreens["Screen_MainMenu"].SetActive(true);
        MenuScreens["Screen_BattleTransition"].SetActive(true);
        currentScreen = GlobalEnums.MenuScreens.MainMenu;
        screensStack.Push(currentScreen);
    }

    public void ChangeScreen(GlobalEnums.MenuScreens _NewScreen)
    {
        MenuScreens["Screen_" + currentScreen].SetActive(false);
        MenuScreens["Screen_" + _NewScreen].SetActive(true);

        if (currentScreen != GlobalEnums.MenuScreens.Loading && currentScreen != GlobalEnums.MenuScreens.Options)
            screensStack.Push(currentScreen);

        currentScreen = _NewScreen;
    }

    private void HandleMainMenu()
    {
        if(isMenuEnabled == true)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentSelection < 4)
                    currentSelection++;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentSelection > 1)
                    currentSelection--;
            }
            UpdateMainMenu();

            if (Input.GetKeyDown(KeyCode.Z))
            {
                switch (currentSelection)
                {
                    case 1:
                        menuMusic.Stop();
                        SC_GameLogic.EnterBattle();
                        isMenuEnabled = false;
                        this.enabled = false;
                        break;

                    case 2:
                        if (isConnected)
                        {
                            isMenuEnabled = false;
                            SC_DeckMenuLogic.initDeckMenu();
                            SC_DeckMenuLogic.isDeckMenuEnabled = true;
                            ChangeScreen(GlobalEnums.MenuScreens.Deck);
                            menuMusic.Stop();
                            deckMusic.Play();
                        }
                        break;

                    case 3:
                        isMenuEnabled = false;
                        SC_OptionsMenuLogic.isOptionsMenuEnabled = true;
                        ChangeScreen(GlobalEnums.MenuScreens.Options);
                        break;

                    case 4:
                        Application.Quit();
                        break;
                }

                buttonClick.Play();
            }

            if (isConnected == true)
            {
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            }
            else if (isConnected == false)
            {
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough | FontStyles.Bold;
                WarpClient.GetInstance().Connect(userId);
            }
        }
    }

    private void UpdateMainMenu()
    {
        switch (currentSelection)
        {
            case 1:
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().fontSize = 34;
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                break;

            case 2:
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().fontSize = 34;
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                break;

            case 3:
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().fontSize = 34;
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                break;

            case 4:
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().color = Constants.GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().fontSize = 34;
                break;
        }
    }

    public void handleBackPress()
    {
        ChangeScreen((GlobalEnums.MenuScreens)screensStack.Pop());
    }

    public void PlayMultiplayer(int _level)
    {
        ChangeScreen(GlobalEnums.MenuScreens.Loading);
        matchRoomData["Level"] = _level;
        isMenuEnabled = false;
        Debug.Log("PlayMultiplayer: Searching for room... match LEVEL: " + _level);
        menuMusic.Stop();
        WarpClient.GetInstance().GetRoomsInRange(1, 2);
    }


    #region Events

    private void OnConnect(bool _IsSuccess)
    {
        if (_IsSuccess)
        {
            isConnected = true;
            Debug.Log("OnConnect: Connected.");
        }
        else
        {
            isConnected = false;
            Debug.Log("OnConnect: NOT Connected.");
        }
    }

    private void OnRoomsInRange(bool _IsSuccess, MatchedRoomsEvent eventObj)
    {
        Debug.Log("OnRoomsInRange: " + eventObj.getRoomsData().Length + " Rooms.");
        if (_IsSuccess)
        {
            roomIds = new List<string>();
            foreach (var roomData in eventObj.getRoomsData())
            {
                Debug.Log("RoomId: " + roomData.getId());
                Debug.Log("Room Owner: " + roomData.getRoomOwner());
                roomIds.Add(roomData.getId());
            }

            roomIdx = 0;
            DoRoomSearchLogic();
        }
        else
        {
            Debug.Log("OnRoomsInRange: Failed to load rooms.");
        }
    }

    private void DoRoomSearchLogic()
    {
        if (roomIdx < roomIds.Count)
        {
            Debug.Log("Get Room Details (" + roomIds[roomIdx] + ")");
            WarpClient.GetInstance().GetLiveRoomInfo(roomIds[roomIdx]);
        }
        else
        {
            Debug.Log("Create Room...");
            WarpClient.GetInstance().CreateTurnRoom("Battle", userId, 2, matchRoomData, 32);
        }
    }

    private void OnCreateRoom(bool _IsSuccess, string _RoomId)
    {
        if (_IsSuccess)
        {
            Debug.Log("Room " + _RoomId + " Created, waiting for opponent...");
            currRoomId = _RoomId;
            WarpClient.GetInstance().JoinRoom(currRoomId);
            WarpClient.GetInstance().SubscribeRoom(currRoomId);
        }
    }

    private void OnGetLiveRoomInfo(LiveRoomInfoEvent eventObj)
    {
        Dictionary<string, object> _prams = eventObj.getProperties();
        if (_prams != null && _prams.ContainsKey("Level"))
        {
            int _value = int.Parse(_prams["Level"].ToString());
            if (_value == int.Parse(matchRoomData["Level"].ToString()))
            {
                currRoomId = eventObj.getData().getId();
                Debug.Log("Joining Room " + currRoomId);
                WarpClient.GetInstance().JoinRoom(currRoomId);
                WarpClient.GetInstance().SubscribeRoom(currRoomId);
            }
            else
            {
                roomIdx++;
                DoRoomSearchLogic();
            }
        }
    }

    private void OnJoinRoom(bool _IsSuccess, string _RoomId)
    {
        if (_IsSuccess)
            Debug.Log("Succefully Joined Room " + _RoomId);
        else
            Debug.Log("Failed to Joined Room " + _RoomId);
    }

    private void OnUserJoinRoom(RoomData eventObj, string _UserId)
    {
        if (userId != _UserId)
        {
            Debug.Log(_UserId + " Have joined the room");
            WarpClient.GetInstance().startGame();
        }
    }

    private void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        SC_LoadingMenuLogic.canUserCancel = false;
        Debug.Log("Started Game, " + _NextTurn + " Turn to Play");
    }

    private void OnDisconnect(bool _IsSuccess)
    {
        Debug.Log("Disconnected");
        isConnected = false;
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MiniJSON;
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
    public AudioSource menuMusic;


    // Multiplayer
    public Listener listener;
    public string userId = "";
    private Dictionary<string, object> matchRoomData;
    private List<string> roomIds;
    private int roomIdx = 0;
    private string currRoomId;
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

    public void UpdateStatus(string _NewStatus)
    {
        MenuObjects["Text_Status"].GetComponent<Text>().text = _NewStatus;
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
        MenuObjects["Text_UserID"].GetComponent<Text>().text = userId;
        WarpClient.GetInstance().Connect(userId);
        UpdateStatus("Connecting...");
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
        MenuObjects["Button_Multiplayer"].GetComponent<Button>().interactable = false;
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
                            SC_DeckMenuLogic.isDeckMenuEnabled = true;
                            ChangeScreen(GlobalEnums.MenuScreens.Deck);
                            //PlayMultiplayer();
                        }
                        break;

                    case 3:
                        ChangeScreen(GlobalEnums.MenuScreens.Options);
                        break;

                    case 4:
                        Application.Quit();
                        break;
                }
            }

            if (isConnected == true)
            {
                MenuObjects["Button_Multiplayer"].GetComponent<Button>().interactable = true;
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
            }
            else if (isConnected == false)
            {
                MenuObjects["Button_Multiplayer"].GetComponent<Button>().interactable = false;
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Strikethrough | FontStyles.Bold;
            }
        }
    }

    private void UpdateMainMenu()
    {
        switch (currentSelection)
        {
            case 1:
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().fontSize = 34;
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                break;

            case 2:
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().fontSize = 34;
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                break;

            case 3:
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().fontSize = 34;
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                break;

            case 4:
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Play"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Multiplayer"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                MenuObjects["Text_Options"].GetComponent<TextMeshProUGUI>().fontSize = 32;
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().color = GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                MenuObjects["Text_Exit"].GetComponent<TextMeshProUGUI>().fontSize = 34;
                break;
        }
    }

    private Color GetColorFromHexString(string hexString)
    {
        byte red = System.Convert.ToByte(hexString.Substring(0, 2), 16);
        byte green = System.Convert.ToByte(hexString.Substring(2, 2), 16);
        byte blue = System.Convert.ToByte(hexString.Substring(4, 2), 16);
        return new Color32(red, green, blue, 255);
    }

    public void handleMusicSlider()
    {
        int _value = (int)MenuObjects["Slider_Music"].GetComponent<Slider>().value;
        MenuObjects["Audio_MenuMusic"].GetComponent<AudioSource>().volume = (float)(_value * 0.1);
    }

    //public void Multiplayer_Slider_Bet()
    //{
    //    int _value = (int)MenuObjects["Multiplayer_Slider_Bet"].GetComponent<Slider>().value;
    //    MenuObjects["Multiplayer_Text_BetValue"].GetComponent<Text>().text = "$" + _value.ToString();
    //}

    //public void Options_Slider_Music()
    //{
    //    int _value = (int)MenuObjects["Options_Slider_Music"].GetComponent<Slider>().value;
    //    MenuObjects["Options_Text_MusicSliderValue"].GetComponent<Text>().text = _value.ToString();
    //    MenuObjects["Music_Background"].GetComponent<AudioSource>().volume = (float)(_value * 0.1);
    //}

    //public void Options_Slider_Sfx()
    //{
    //    int _value = (int)MenuObjects["Options_Slider_Sfx"].GetComponent<Slider>().value;
    //    MenuObjects["Options_Text_SfxSliderValue"].GetComponent<Text>().text = _value.ToString();
    //}

    public void handleBackPress()
    {
        ChangeScreen((GlobalEnums.MenuScreens)screensStack.Pop());
    }

    //public IEnumerator loadingPokeballs(float waitTime)
    //{
    //    while (true)
    //    {
    //        MenuObjects["Loading_Img_Pokeball1"].SetActive(false);
    //        MenuObjects["Loading_Img_Pokeball2"].SetActive(false);
    //        MenuObjects["Loading_Img_Pokeball3"].SetActive(false);
    //        yield return StartCoroutine(hidePokeballs(waitTime));
    //        MenuObjects["Loading_Img_Pokeball1"].SetActive(true);
    //        yield return StartCoroutine(hidePokeballs(waitTime));
    //        MenuObjects["Loading_Img_Pokeball2"].SetActive(true);
    //        yield return StartCoroutine(hidePokeballs(waitTime));
    //        MenuObjects["Loading_Img_Pokeball3"].SetActive(true);
    //        yield return StartCoroutine(hidePokeballs(waitTime));
    //    }
    //}

    //IEnumerator hidePokeballs(float waitTime)
    //{
    //    yield return new WaitForSeconds(waitTime);
    //}

    public void PlayMultiplayer(int _level)
    {
        ChangeScreen(GlobalEnums.MenuScreens.Loading);
        matchRoomData["Level"] = _level;
        isMenuEnabled = false;
        //this.enabled = false;
        Debug.Log("PlayMultiplayer: Searching for room... LEVEL: " + _level);
        UpdateStatus("Searching for room...");
        WarpClient.GetInstance().GetRoomsInRange(1, 2);
    }

    #region Events

    private void OnConnect(bool _IsSuccess)
    {
        if (_IsSuccess)
        {
            Debug.Log("OnConnect: Connected " + _IsSuccess);
            UpdateStatus("Connected!");
            isConnected = true;
        }
        else UpdateStatus("Connection Error");
    }

    private void OnRoomsInRange(bool _IsSuccess, MatchedRoomsEvent eventObj)
    {
        Debug.Log("OnRoomsInRange: " + _IsSuccess + "   getRoomsData.Length: " + eventObj.getRoomsData().Length);
        if (_IsSuccess)
        {
            UpdateStatus("Parsing Rooms");
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
        else UpdateStatus("Error Fetching Rooms in Range");
    }

    private void DoRoomSearchLogic()
    {
        if (roomIdx < roomIds.Count)
        {
            Debug.Log("Get Room Details (" + roomIds[roomIdx] + ")");
            UpdateStatus("Get Room Details (" + roomIds[roomIdx] + ")");
            WarpClient.GetInstance().GetLiveRoomInfo(roomIds[roomIdx]);
        }
        else
        {
            Debug.Log("Create Room...");
            UpdateStatus("Create Room...");
            WarpClient.GetInstance().CreateTurnRoom("Battle", userId, 2, matchRoomData, 30);
        }
    }

    private void OnCreateRoom(bool _IsSuccess, string _RoomId)
    {
        if (_IsSuccess)
        {
            Debug.Log("Room " + _RoomId + " Created, waiting for opponent...");
            UpdateStatus("Room Created, waiting for opponent...");
            currRoomId = _RoomId;
            WarpClient.GetInstance().JoinRoom(currRoomId);
            WarpClient.GetInstance().SubscribeRoom(currRoomId);
        }
        else UpdateStatus("Failed to create Room");
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
                UpdateStatus("Joining Room " + currRoomId);
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
        {
            UpdateStatus("Succefully Joined Room " + _RoomId);
            Debug.Log("Succefully Joined Room " + _RoomId);
        }
        else
        {
            UpdateStatus("Failed to Joined Room " + _RoomId);
            Debug.Log("Failed to Joined Room " + _RoomId);
        }
    }

    private void OnUserJoinRoom(RoomData eventObj, string _UserId)
    {
        if (userId != _UserId)
        {
            Debug.Log(_UserId + " Have joined the room");
            UpdateStatus(_UserId + " Have joined the room");
            WarpClient.GetInstance().startGame();
        }
    }

    private void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        UpdateStatus("Started Game, " + _NextTurn + " Turn to Play");
        Debug.Log("Started Game, " + _NextTurn + " Turn to Play");
        //MenuObjects["Menu"].SetActive(false);
        //MenuObjects["Game"].SetActive(true);
    }

    private void OnDisconnect(bool _IsSuccess)
    {
        Debug.Log("Disconnected");
        isConnected = false;
    }

    #endregion
}

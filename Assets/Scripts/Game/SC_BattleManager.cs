using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MiniJSON;
using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;

public class SC_BattleManager : MonoBehaviour
{
    private int currentSelection;
    private int currentMove;
    private string currentTurnID;
    public List<Sprite> battleBackgrounds;
    public Image Img_battleBG;
    public Button button_play;

    // states
    private GlobalEnums.Turns currentTurn;
    private GlobalEnums.BattleMenus currentMenu;
    private GlobalEnums.BattleStates battleState;
    private GlobalEnums.MessageBoxState MessageState;

    // bools
    private bool isMultiplayer;
    private bool isInBattle;
    private bool isWaitingForRespond;
    private bool isSelectionMenuEnabled;
    private bool isMovesMenuEnabled;
    private bool isFoeAttackingATM;
    private bool canExit;

    [Header("Scripts")]
    public SC_GameLogic SC_GameLogic;
    public SC_MenuLogic SC_MenuLogic;
    public SC_DeckMenuLogic SC_DeckMenuLogic;
    private SC_BasePokemon foePokemon;
    private SC_BasePokemon playerPokemon;
    private SC_PokemonMove attackMove;

    [Header("Cameras")]
    public GameObject menuCamera;
    public GameObject battleCamera;

    [Header("Sounds")]
    public AudioSource victoryMusic;
    public AudioSource losingMusic;
    public AudioSource clickSound;
    public AudioSource runSound;

    [Header("Foe")]
    public Image Img_foePokemon;
    public Text Text_pokemonNameFoe;
    public Text Text_pokemonLvlFoe;
    public GameObject foeHealthBar;

    [Header("Player")]
    public Image Img_playerPokemon;
    public Text Text_pokemonNamePlayer;
    public Text Text_pokemonLvlPlayer;
    public Text Text_PlayerHP;
    public GameObject playerHealthBar;

    [Header("Selection")]
    public GameObject SelectionMenu;
    public Text Fight;
    public Text Run;

    [Header("Message")]
    public GameObject MessageMenu;
    public Text MessageText;

    [Header("Moves")]
    public GameObject MovesMenu;
    public GameObject MovesDetails;
    public Text PPstats;
    public Text PPtype;
    public Text MoveType;
    public Text Move1;
    public Text Move2;
    public Text Move3;
    public Text Move4;

    // Texts
    private string FightTxt;
    private string RunTxt;
    private string Move1Txt;
    private string Move2Txt;
    private string Move3Txt;
    private string Move4Txt;

    private void OnEnable()
    {
        Listener.OnGameStarted += OnGameStarted;
        Listener.OnMoveCompleted += OnMoveCompleted;
        Listener.OnGameStopped += OnGameStopped;
    }

    private void OnDisable()
    {
        Listener.OnGameStarted -= OnGameStarted;
        Listener.OnMoveCompleted -= OnMoveCompleted;
        Listener.OnGameStopped -= OnGameStopped;
    }

    private void Awake()
    {
        initText();
        initBattle();
    }

    private void Update()
    {
        ManageBattleFlow();
    }

    private void ManageBattleFlow()
    {
        if (battleCamera.activeInHierarchy == false)
            return;

        if (isInBattle == false)
        {
            ChangeMenu(GlobalEnums.BattleMenus.Message);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            clickSound.Play();

            if (canExit == true)
                BackToMainMenu();

            if (battleState == GlobalEnums.BattleStates.GameOver && battleCamera.activeInHierarchy == true)
            {
                FinishBattle();
                canExit = true;
                return;
            }
            else isWaitingForRespond = false;

            //if (MessageState == GlobalEnums.MessageBoxState.WaitingForAttack)
            //{
            //    MessageState = GlobalEnums.MessageBoxState.EnterBattle;
            //}

            if (MessageState == GlobalEnums.MessageBoxState.EnterBattle)
            {
                MessageState = GlobalEnums.MessageBoxState.Selection;
                isSelectionMenuEnabled = true;
                ChangeMenu(GlobalEnums.BattleMenus.Selection);
                ManageMessageBox();
                return;
            }

            if (MessageState == GlobalEnums.MessageBoxState.Attack && isMultiplayer == false)
            {
                if (currentTurn == GlobalEnums.Turns.PlayersTurn)
                {
                    currentTurn = GlobalEnums.Turns.FoesTurn;
                }
                else if (currentTurn == GlobalEnums.Turns.FoesTurn && isFoeAttackingATM != true)
                {
                    currentTurn = GlobalEnums.Turns.PlayersTurn;
                    MessageState = GlobalEnums.MessageBoxState.Selection;
                    ChangeMenu(GlobalEnums.BattleMenus.Selection);
                    ManageMessageBox();
                    isWaitingForRespond = false;
                    isSelectionMenuEnabled = true;
                    isMovesMenuEnabled = false;
                }

                return;
            }
            else if (MessageState == GlobalEnums.MessageBoxState.Attack && isMultiplayer == true)
            {
                if (currentTurn == GlobalEnums.Turns.FoesTurn && isFoeAttackingATM != true)
                {
                    currentTurn = GlobalEnums.Turns.PlayersTurn;
                    MessageState = GlobalEnums.MessageBoxState.Selection;
                    ChangeMenu(GlobalEnums.BattleMenus.Selection);
                    ManageMessageBox();
                    isWaitingForRespond = false;
                    isSelectionMenuEnabled = true;
                    isMovesMenuEnabled = false;
                }
                else
                {
                    MessageState = GlobalEnums.MessageBoxState.WaitingForAttack;
                    ManageMessageBox();
                }

                return;
            }
        }

        if (isWaitingForRespond == true)
        {
            ManageMessageBox(attackMove);
        }
        else
        {
            if (battleState == GlobalEnums.BattleStates.Start)
            {
                battleState = GlobalEnums.BattleStates.Battling;

                if (isMultiplayer == true)
                {
                    if (currentTurn == GlobalEnums.Turns.PlayersTurn)
                    {
                        MessageState = GlobalEnums.MessageBoxState.Selection;
                        isSelectionMenuEnabled = true;
                        ChangeMenu(GlobalEnums.BattleMenus.Selection);
                        ManageMessageBox();
                    }
                    else if (currentTurn == GlobalEnums.Turns.FoesTurn)
                    {
                        MessageState = GlobalEnums.MessageBoxState.WaitingForAttack;
                        ManageMessageBox();
                    }

                    if (isInBattle == true && currentTurn == GlobalEnums.Turns.PlayersTurn && MessageState != GlobalEnums.MessageBoxState.Attack)
                    {
                        ManagePlayersTurn();
                    }
                }
                else
                {
                    currentTurn = GetRandomTurn();

                    if (currentTurn == GlobalEnums.Turns.FoesTurn)
                    {
                        AttackRandomly();
                    }
                    else
                    {
                        MessageState = GlobalEnums.MessageBoxState.Selection;
                        ManageMessageBox();
                    }
                }
            }

            if (isInBattle == true && currentTurn == GlobalEnums.Turns.PlayersTurn && MessageState != GlobalEnums.MessageBoxState.Attack)
            {
                ManagePlayersTurn();
            }
            else if (isInBattle == true && currentTurn == GlobalEnums.Turns.FoesTurn)
            {
                isFoeAttackingATM = true;
                isSelectionMenuEnabled = false;
                isMovesMenuEnabled = false;
                isWaitingForRespond = true;

                if (isMultiplayer == false)
                    AttackRandomly();
                else
                {
                    MessageState = GlobalEnums.MessageBoxState.WaitingForAttack;
                    ManageMessageBox();
                }
            }
        }
    }

    private void initText()
    {
        FightTxt = Fight.text;
        RunTxt = Run.text;
        Move1Txt = Move1.text;
        Move2Txt = Move2.text;
        Move3Txt = Move3.text;
        Move4Txt = Move4.text;
    }

    private void ManagePlayersTurn()
    {
        if (currentMenu == GlobalEnums.BattleMenus.Selection && isSelectionMenuEnabled == true)
        {
            isMovesMenuEnabled = false;
            HandleSelectionMenu();
        }
        else if (currentMenu == GlobalEnums.BattleMenus.Moves && isMovesMenuEnabled == true)
        {
            isSelectionMenuEnabled = false;
            HandleMovesMenu();
        }
    }

    public Sprite getRandomBackground(int _index = 000)
    {
        if (_index == 000)
        {
            int _randomIndex = UnityEngine.Random.Range(0, battleBackgrounds.Count);
            return battleBackgrounds[_randomIndex];
        }
        else
            return battleBackgrounds[_index];
    }

    private void RestartBattle()
    {
        isInBattle = false;
        isMultiplayer = false;
        isWaitingForRespond = true;
        isSelectionMenuEnabled = false;
        isMovesMenuEnabled = false;
        isFoeAttackingATM = false;
        canExit = false;
        MessageState = GlobalEnums.MessageBoxState.EnterBattle;
        battleState = GlobalEnums.BattleStates.Start;
        currentMenu = GlobalEnums.BattleMenus.Message;
        currentTurn = GlobalEnums.Turns.PlayersTurn;
        attackMove = null;
    }

    public void initBattle()
    {
        Img_battleBG.sprite = getRandomBackground();
        RestartBattle();
        initPlayer();
        initFoe();
    }

    private void initMultiplayerBattle()
    {
        initBattle();
        Dictionary<string, object> _toSend = new Dictionary<string, object>();
        int _pokemonID;

        if (SC_DeckMenuLogic.currentCardIndex == -1)
            _pokemonID = SC_GameLogic.getRandomPokemonFromList(SC_GameLogic.allPokemons).ID;
        else
            _pokemonID = SC_GameLogic.allPokemons[SC_DeckMenuLogic.currentCardIndex].ID;

        initPlayer(_pokemonID, SC_DeckMenuLogic.currentSliderValue);
        _toSend.Add("firstPokemonID", _pokemonID);
        int _randomIndex = UnityEngine.Random.Range(0, battleBackgrounds.Count);
        Img_battleBG.sprite = battleBackgrounds[_randomIndex];
        _toSend.Add("battleBackgroundIndex", _randomIndex);
        string _send = MiniJSON.Json.Serialize(_toSend);
        Debug.Log("Sending: " + playerPokemon.name);
        WarpClient.GetInstance().sendMove(_send);
    }

    private void initFoe(int _pokemonID = 000, int _pokemonLvl = 000)
    {
        if (_pokemonID == 000)
            foePokemon = SC_GameLogic.getRandomPokemonFromList(SC_GameLogic.allPokemons);
        else
            foePokemon = SC_GameLogic.getPokemonByID(_pokemonID);

        Img_foePokemon.sprite = foePokemon.frontImage;
        Text_pokemonNameFoe.text = foePokemon.name;

        
        if (_pokemonLvl == 000)
            Text_pokemonLvlFoe.text = "Lv" + foePokemon.level.ToString();
        else
            Text_pokemonLvlFoe.text = "Lv" + _pokemonLvl.ToString();

        foePokemon.HpStats.curr = foePokemon.HpStats.max;
        foePokemon.healthBar = foeHealthBar.GetComponent<SC_HealthBar>();
        foePokemon.healthBar.SetHealthBarScale(1f);
        for (int i = 0; i < foePokemon.moves.Count; i++)
        {
            foePokemon.moves[i] = Instantiate(foePokemon.moves[i]);
            foePokemon.moves[i].currPP = foePokemon.moves[i].maxPP;
        }
    }

    private void initPlayer(int _pokemonID = 000, int _pokemonLvl = 000)
    {
        if (_pokemonID == 000)
            playerPokemon = SC_GameLogic.getRandomPokemonFromList(SC_GameLogic.allPokemons);
        else
            playerPokemon = SC_GameLogic.getPokemonByID(_pokemonID);

        Img_playerPokemon.sprite = playerPokemon.backImage;
        Text_pokemonNamePlayer.text = playerPokemon.name;

        if (_pokemonLvl == 000)
            Text_pokemonLvlPlayer.text = "Lv" + playerPokemon.level.ToString();
        else
            Text_pokemonLvlPlayer.text = "Lv" + _pokemonLvl.ToString();

        playerPokemon.HpStats.curr = playerPokemon.HpStats.max;
        Text_PlayerHP.text = playerPokemon.HpStats.curr.ToString() + "/" + playerPokemon.HpStats.max.ToString();
        playerPokemon.healthBar = playerHealthBar.GetComponent<SC_HealthBar>();
        playerPokemon.healthBar.SetHealthBarScale(1f);
        Move1Txt = playerPokemon.moves[0].name;
        Move2Txt = playerPokemon.moves[1].name;
        Move3Txt = playerPokemon.moves[2].name;
        Move4Txt = playerPokemon.moves[3].name;
        for (int i = 0; i < playerPokemon.moves.Count; i++)
        {
            playerPokemon.moves[i] = Instantiate(playerPokemon.moves[i]);
            playerPokemon.moves[i].currPP = playerPokemon.moves[i].maxPP;
        }
    }

    private void ManageMessageBox(SC_PokemonMove _move = null)
    {
        if (MessageState == GlobalEnums.MessageBoxState.EnterBattle)
        {
            ChangeMenu(GlobalEnums.BattleMenus.Message);
            MessageText.text = foePokemon.name + "  Wants  To  Battle!";
        }
        else if (MessageState == GlobalEnums.MessageBoxState.Selection)
        {
            ChangeMenu(GlobalEnums.BattleMenus.Selection);
            MessageText.text = "What  Will  " + playerPokemon.name + "  Do?";
        }
        else if (MessageState == GlobalEnums.MessageBoxState.WaitingForAttack)
        {
            ChangeMenu(GlobalEnums.BattleMenus.Message);
            //isWaitingForRespond = true;
            MessageText.text = "Enemy  Pokemon  Is  Attacking.";
        }
        else if (MessageState == GlobalEnums.MessageBoxState.Attack)
        {
            ChangeMenu(GlobalEnums.BattleMenus.Message);
            isWaitingForRespond = true;

            if (_move != null)
            {
                if (currentTurn == GlobalEnums.Turns.PlayersTurn)
                    MessageText.text = playerPokemon.name + "  Used  " + _move.name + "!";
                else if (currentTurn == GlobalEnums.Turns.FoesTurn)
                    MessageText.text = "Oh No! \nEnemy  " + foePokemon.name + "  Used  " + _move.name + "!";
            }          
        }
        else if (MessageState == GlobalEnums.MessageBoxState.GameOver)
        {
            ChangeMenu(GlobalEnums.BattleMenus.Message);

            if (playerPokemon.HpStats.curr <= 0)
                MessageText.text = playerPokemon.name + "  Defeated!  \nMaybe  Next  Time...";
            else if (foePokemon.HpStats.curr <= 0)    
                MessageText.text = "Enemy  " + foePokemon.name + "  Defeated! \nGood  Job!";
        }
    }

    private void UpdateSelectionMenu()
    {
        switch (currentSelection)
        {
            case 1:
                Fight.text = "> " + FightTxt;
                Run.text = RunTxt;
                break;

            case 2:
                Fight.text = FightTxt;
                Run.text = "> " + RunTxt;
                break;
        }
    }

    private GlobalEnums.Turns GetRandomTurn()
    {
        int randomNum = UnityEngine.Random.Range(0, 2);
        GlobalEnums.Turns randTurn = (GlobalEnums.Turns)randomNum;
        return randTurn;
    }

    private void UpdateMoveDetailBox(SC_PokemonMove move)
    {
        PPstats.GetComponent<Text>().text = move.currPP.ToString() + "/" + move.maxPP.ToString();
        PPtype.text = "TYPE/" + move.type.ToString();
    }

    private void UpdateMovesMenu()
    {
        if (currentTurn == GlobalEnums.Turns.PlayersTurn)
        {
            switch (currentMove)
            {
                case 1:
                    UpdateMoveDetailBox(playerPokemon.moves[0]);
                    Move1.text = "> " + Move1Txt;
                    Move2.text = Move2Txt;
                    Move3.text = Move3Txt;
                    Move4.text = Move4Txt;
                    break;

                case 2:
                    UpdateMoveDetailBox(playerPokemon.moves[1]);
                    Move1.text = Move1Txt;
                    Move2.text = "> " + Move2Txt;
                    Move3.text = Move3Txt;
                    Move4.text = Move4Txt;
                    break;

                case 3:
                    UpdateMoveDetailBox(playerPokemon.moves[2]);
                    Move1.text = Move1Txt;
                    Move2.text = Move2Txt;
                    Move3.text = "> " + Move3Txt;
                    Move4.text = Move4Txt;
                    break;

                case 4:
                    UpdateMoveDetailBox(playerPokemon.moves[3]);
                    Move1.text = Move1Txt;
                    Move2.text = Move2Txt;
                    Move3.text = Move3Txt;
                    Move4.text = "> " + Move4Txt;
                    break;
            }
        }
    }

    private void HandleSelectionMenu()
    {
        if (currentTurn == GlobalEnums.Turns.PlayersTurn)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentSelection < 2)
                    currentSelection++;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentSelection > 1)
                    currentSelection--;
            }
            if (Input.GetKeyDown(KeyCode.Z) && MessageState != GlobalEnums.MessageBoxState.EnterBattle)
            {
                if (currentSelection == 1)
                {
                    ChangeMenu(GlobalEnums.BattleMenus.Moves);
                    isSelectionMenuEnabled = false;
                    isMovesMenuEnabled = true;
                }
                else if (currentSelection == 2)
                {
                    isInBattle = false;
                    BackToMainMenu();
                    runSound.Play();
                }
            }

            UpdateSelectionMenu();
        }
    }

    private void BackToMainMenu()
    {
        SC_GameLogic.battleMusic.Stop();
        victoryMusic.Stop();
        losingMusic.Stop();
        menuCamera.SetActive(true);
        battleCamera.SetActive(false);
        SC_MenuLogic.isMenuEnabled = true;
        SC_MenuLogic.menuMusic.Play();
        SC_MenuLogic.enabled = true;
        button_play.interactable = true;
        SC_MenuLogic.isMenuEnabled = true;
    }

    private void HandleMovesMenu()
    {
        if (currentMenu == GlobalEnums.BattleMenus.Moves && currentTurn == GlobalEnums.Turns.PlayersTurn)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentMove == 1)
                    currentMove = 3;
                else if (currentMove == 2)
                    currentMove = 4;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentMove == 3)
                    currentMove = 1;
                else if (currentMove == 4)
                    currentMove = 2;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (currentMove == 1)
                    currentMove = 2;
                else if (currentMove == 3)
                    currentMove = 4;
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (currentMove == 2)
                    currentMove = 1;
                else if (currentMove == 4)
                    currentMove = 3;
            }

            UpdateMovesMenu();

            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (currentTurn == GlobalEnums.Turns.PlayersTurn)
                {
                    if (isMultiplayer == true)
                    {
                        Dictionary<string, object> _toSend = new Dictionary<string, object>();
                        _toSend.Add("attackMoveID", playerPokemon.moves[currentMove - 1].ID);
                        string _send = MiniJSON.Json.Serialize(_toSend);
                        WarpClient.GetInstance().sendMove(_send);
                    }

                    AttackOpponent(playerPokemon, playerPokemon.moves[currentMove - 1], foePokemon);
                }

                isSelectionMenuEnabled = false;
                isMovesMenuEnabled = false;
            }

        }
    }

    private void FinishBattle()
    {
        SC_GameLogic.battleMusic.Stop();
        attackMove.moveSound.Stop();

        if (currentTurn == GlobalEnums.Turns.PlayersTurn)
            victoryMusic.Play();
        else if (currentTurn == GlobalEnums.Turns.FoesTurn)
            losingMusic.Play();

        isInBattle = false;
        isWaitingForRespond = true;
        MessageState = GlobalEnums.MessageBoxState.GameOver;
        battleState = GlobalEnums.BattleStates.GameOver;
        ManageMessageBox();
    }

    private void AttackRandomly()
    {
        int randomIndex = UnityEngine.Random.Range(0, 4);
        AttackOpponent(foePokemon, foePokemon.moves[randomIndex], playerPokemon);
        isFoeAttackingATM = false;
    }

    private void AttackOpponent(SC_BasePokemon _attackPokemon, SC_PokemonMove _attackMove, SC_BasePokemon _defensePokemon)
    {
        if (canExit != true)
            _attackMove.moveSound.Play();
        currentMenu = GlobalEnums.BattleMenus.Message;
        MessageState = GlobalEnums.MessageBoxState.Attack;
        attackMove = _attackMove;
        ManageMessageBox(attackMove);
        isWaitingForRespond = true;

        _attackMove.currPP--;

        float _lvl = (float)_attackPokemon.level;
        float _power = (float)attackMove.power;
        float _A = (float)_attackPokemon.attack;
        float _D = (float)_defensePokemon.defense;
        float _modifier = (float)(UnityEngine.Random.Range(0.8f, 1f));
        float _damage = (((((((2f * _lvl) / 5f) + 2f) * _power) * (_A / _D)) + 2f) / 50f) * _modifier;
        //float damage = (((((((2f * lvl) / 5f) + 2f) * power) * (A/D)) / 50f) + 2f) * modifier;
        //Debug.Log("damage: " + damage);

        if (currentTurn == GlobalEnums.Turns.PlayersTurn)
            StartCoroutine(FlashAfterAttack(Img_foePokemon, 4, 0.1f));
        else if (currentTurn == GlobalEnums.Turns.FoesTurn)
            StartCoroutine(FlashAfterAttack(Img_playerPokemon, 4, 0.1f));

        if (_defensePokemon.HpStats.curr - _damage < 1)
        {
            _defensePokemon.HpStats.curr = 0;
            _defensePokemon.healthBar.SetHealthBarScale(0);
            Text_PlayerHP.text = ((int)playerPokemon.HpStats.curr).ToString() + "/" + playerPokemon.HpStats.max.ToString();
            battleState = GlobalEnums.BattleStates.GameOver;
        }
        else
        {
            float newHP = (_defensePokemon.HpStats.curr - _damage) / _defensePokemon.HpStats.max;
            _defensePokemon.HpStats.curr -= _damage;
            _defensePokemon.healthBar.SetHealthBarScale(newHP);
            Text_PlayerHP.text = ((int)playerPokemon.HpStats.curr).ToString() + "/" + playerPokemon.HpStats.max.ToString();
        }
    }

    private IEnumerator FlashAfterAttack(Image pokemon, int numOfTimes, float delay)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < numOfTimes; i++)
        {
            pokemon.color = new Color(pokemon.color.r, pokemon.color.g, pokemon.color.b, 0.1f);
            yield return new WaitForSeconds(delay);
            pokemon.color = new Color(pokemon.color.r, pokemon.color.g, pokemon.color.b, 1);
            yield return new WaitForSeconds(delay);
        }
    }

    //private IEnumerator AIdelay(float delay)
    //{
    //    yield return new WaitForSeconds(delay);
    //    AttackRandomly();
    //    yield return new WaitForSeconds(delay);
    //    isFoeAttackingATM = false;
    //}

    public void ChangeMenu(GlobalEnums.BattleMenus menu)
    {
        switch (menu)
        {
            case GlobalEnums.BattleMenus.Selection:
                SelectionMenu.gameObject.SetActive(true);
                MessageMenu.gameObject.SetActive(true);
                MovesMenu.gameObject.SetActive(false);
                MovesDetails.gameObject.SetActive(false);
                break;

            case GlobalEnums.BattleMenus.Moves:
                SelectionMenu.gameObject.SetActive(false);
                MessageMenu.gameObject.SetActive(false);
                MovesMenu.gameObject.SetActive(true);
                MovesDetails.gameObject.SetActive(true);
                break;

            case GlobalEnums.BattleMenus.Message:
                SelectionMenu.gameObject.SetActive(false);
                MessageMenu.gameObject.SetActive(true);
                MovesMenu.gameObject.SetActive(false);
                MovesDetails.gameObject.SetActive(false);
                break;
        }

        currentSelection = 1;
        currentMove = 1;
        isInBattle = true;
        currentMenu = menu;
    }

    #region Events
    private void OnGameStarted(string _Sender, string _RoomId, string _NextTurn)
    {
        currentTurnID = _NextTurn;

        if (SC_MenuLogic.Instance.userId == _NextTurn)
        {
            currentTurn = GlobalEnums.Turns.PlayersTurn;
            initMultiplayerBattle();
        }
        else
        {
            currentTurn = GlobalEnums.Turns.FoesTurn;
        }
    }

    private void OnMoveCompleted(MoveEvent _Move)
    {
        Dictionary<string, object> _data = (Dictionary<string, object>)MiniJSON.Json.Deserialize(_Move.getMoveData());

        if (_data != null && _data.ContainsKey("firstPokemonID") && _data.ContainsKey("battleBackgroundIndex") && _Move.getSender() != SC_MenuLogic.Instance.userId)
        {
            initBattle();
            Img_battleBG.sprite = battleBackgrounds[int.Parse(_data["battleBackgroundIndex"].ToString())];

            if (SC_DeckMenuLogic.currentCardIndex == -1)
                initPlayer(000, SC_DeckMenuLogic.currentSliderValue);
            else
                initPlayer(SC_GameLogic.allPokemons[SC_DeckMenuLogic.currentCardIndex].ID, SC_DeckMenuLogic.currentSliderValue);

            initFoe(int.Parse(_data["firstPokemonID"].ToString()), SC_DeckMenuLogic.currentSliderValue);

            Dictionary<string, object> _toSend = new Dictionary<string, object>();
            _toSend.Add("secondPokemonID", playerPokemon.ID);
            string _send = MiniJSON.Json.Serialize(_toSend);
            Debug.Log("First: " + foePokemon.name);
            WarpClient.GetInstance().sendMove(_send);
        }
        else if (_data != null && _data.ContainsKey("secondPokemonID") && _Move.getSender() != SC_MenuLogic.Instance.userId)
        {
            initFoe(int.Parse(_data["secondPokemonID"].ToString()), SC_DeckMenuLogic.currentSliderValue);

            Dictionary<string, object> _toSend = new Dictionary<string, object>();
            _toSend.Add("startBattle", true);
            string _send = MiniJSON.Json.Serialize(_toSend);
            Debug.Log("Second: " + foePokemon.name);
            WarpClient.GetInstance().sendMove(_send);
        }
        else if (_data != null && _data.ContainsKey("startBattle"))
        {
            SC_GameLogic.EnterBattle(true);
            isInBattle = true;
            isMultiplayer = true;
            Debug.Log("start: " + playerPokemon.name);
        }

        if (_data != null && _data.ContainsKey("attackMoveID") && _Move.getSender() != SC_MenuLogic.Instance.userId)
        {
            int _attackMoveID = int.Parse(_data["attackMoveID"].ToString());
            currentTurn = GlobalEnums.Turns.FoesTurn;
            isFoeAttackingATM = true;
            isSelectionMenuEnabled = false;
            isMovesMenuEnabled = false;
            isWaitingForRespond = true;
            AttackOpponent(foePokemon, SC_GameLogic.getMoveByID(_attackMoveID), playerPokemon);
            isFoeAttackingATM = false;
            return;
        }
        else if (_data != null && _data.ContainsKey("attackMoveID") && _Move.getSender() == SC_MenuLogic.Instance.userId)
        {
            return;
        }

        if (_Move.getNextTurn() == SC_MenuLogic.Instance.userId)
            currentTurn = GlobalEnums.Turns.PlayersTurn;
        else
            currentTurn = GlobalEnums.Turns.FoesTurn;
    }

    private void OnGameStopped(string _Sender, string _RoomId)
    {
        Debug.Log("Game Over");
    }
    #endregion
}

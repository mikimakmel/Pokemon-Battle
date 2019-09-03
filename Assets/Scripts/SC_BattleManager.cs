using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SC_BattleManager : MonoBehaviour
{
    public int currentSelection;
    public int currentMove;
    public GameObject menuCamera;
    public GameObject battleCamera;
    public GameObject foeHealthBar;
    public GameObject playerHealthBar;
    private SC_BasePokemon foePokemon;
    private SC_BasePokemon playerPokemon;
    public SC_GameLogic SC_GameLogic;
    public SC_Player player;
    private SC_PokemonMove attackMove;
    public AudioSource victoryMusic;
    public AudioSource losingMusic;
    public AudioSource clickSound;
    public AudioSource runSound;
    public List<Sprite> battleBackgrounds;
    public Image Img_battleBG;

    // states
    private GlobalEnums.Turns currentTurn;
    private GlobalEnums.BattleMenus currentMenu;
    private GlobalEnums.BattleStates battleState;
    private GlobalEnums.MessageBoxState MessageState;

    // bools
    private bool isInBattle;
    private bool isWaitingForRespond;
    //private bool isFoeStartingBattle;
    private bool isSelectionMenuEnabled;
    private bool isMovesMenuEnabled;
    private bool isFoeAttackingATM;
    private bool canExit;

    // foe
    public Image Img_foePokemon;
    public Text Text_pokemonNameFoe;
    public Text Text_pokemonLvlFoe;

    //player
    public Image Img_playerPokemon;
    public Text Text_pokemonNamePlayer;
    public Text Text_pokemonLvlPlayer;
    public Text Text_PlayerHP;

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
    public Text PP;
    public Text PPstats;
    public Text MoveType;
    public Text Move1;
    public Text Move2;
    public Text Move3;
    public Text Move4;

    public string FightTxt;
    public string RunTxt;
    public string Move1Txt;
    public string Move2Txt;
    public string Move3Txt;
    public string Move4Txt;

    private void Awake()
    {
        FightTxt = Fight.text;
        RunTxt = Run.text;
        Move1Txt = Move1.text;
        Move2Txt = Move2.text;
        Move3Txt = Move3.text;
        Move4Txt = Move4.text;

        initBattle();
    }

    private void Update()
    {
        if (battleCamera.activeInHierarchy == false)
            return;

        if (isInBattle != true)
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

            if (MessageState == GlobalEnums.MessageBoxState.EnterBattle)
            {
                MessageState = GlobalEnums.MessageBoxState.Selection;
                isSelectionMenuEnabled = true;
                ChangeMenu(GlobalEnums.BattleMenus.Selection);
                ManageMessageBox();
                return;
            }

            if (MessageState == GlobalEnums.MessageBoxState.Attack)
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
                currentTurn = GetRandomTurn();

                if (currentTurn == GlobalEnums.Turns.FoesTurn)
                {
                    StartCoroutine(AIdelay(2f));
                }
                else
                {
                    MessageState = GlobalEnums.MessageBoxState.Selection;
                    ManageMessageBox();
                }
            }

            if (isInBattle == true && currentTurn == GlobalEnums.Turns.PlayersTurn && MessageState != GlobalEnums.MessageBoxState.Attack)
            {
                ManagePlayersTurn();
            }
            else if (isInBattle == true && currentTurn == GlobalEnums.Turns.FoesTurn && isFoeAttackingATM != true)
            {
                isFoeAttackingATM = true;
                isSelectionMenuEnabled = false;
                isMovesMenuEnabled = false;
                isWaitingForRespond = true;
                StartCoroutine(AIdelay(2f));
            }
        }
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

    public Sprite getRandomBackground()
    {
        int randomIndex = UnityEngine.Random.Range(0, battleBackgrounds.Count);
        return battleBackgrounds[randomIndex];
    }

    private void RestartBattle()
    {
        isInBattle = false;
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

    private void initFoe()
    {
        foePokemon = SC_GameLogic.getRandomPokemonFromList(SC_GameLogic.allPokemons);
        Img_foePokemon.sprite = foePokemon.frontImage;
        Text_pokemonNameFoe.text = foePokemon.name;
        Text_pokemonLvlFoe.text = "Lv" + foePokemon.level.ToString();
        foePokemon.HpStats.curr = foePokemon.HpStats.max;
        foePokemon.healthBar = foeHealthBar.GetComponent<SC_HealthBar>();
        foePokemon.healthBar.SetHealthBarScale(1f);
        for (int i = 0; i < foePokemon.moves.Count; i++)
        {
            foePokemon.moves[i].currPP = foePokemon.moves[i].maxPP;
        }
    }

    private void initPlayer()
    {
        playerPokemon = SC_GameLogic.getRandomPokemonFromList(SC_GameLogic.allPokemons);
        Img_playerPokemon.sprite = playerPokemon.backImage;
        Text_pokemonNamePlayer.text = playerPokemon.name;
        Text_pokemonLvlPlayer.text = "Lv" + playerPokemon.level.ToString();
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
        GameObject.Find("Text_PPstats").GetComponent<Text>().text =
            move.currPP.ToString() + "/" + move.maxPP.ToString();
        GameObject.Find("Text_Type").GetComponent<Text>().text = "TYPE/" + move.type.ToString();
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
        GameObject.Find("Button_play").GetComponent<Button>().interactable = true;
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
    }

    private void AttackOpponent(SC_BasePokemon _attackPokemon, SC_PokemonMove _attackMove, SC_BasePokemon _defensePokemon)
    {
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

        if (_defensePokemon.HpStats.curr - _damage < 0)
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

    private IEnumerator AIdelay(float delay)
    {
        //yield return new WaitForSeconds(delay);
        AttackRandomly();
        yield return new WaitForSeconds(0);
        isFoeAttackingATM = false;
    }

    private IEnumerator delay(float delay)
    {
        yield return new WaitForSeconds(delay);
    }

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
}


// blabla wants to battle!
// Go! pokemonName!
// (players attack) pokemonName used pokemonMove!
// (foes attack) enemy pokemonName used pokemonMove!
// (players lose) pokemonName defeated!
// (foes lose) enemy pokemonName defeated!
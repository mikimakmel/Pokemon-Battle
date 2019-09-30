using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using MiniJSON;
using AssemblyCSharp;
using com.shephertz.app42.gaming.multiplayer.client;
using com.shephertz.app42.gaming.multiplayer.client.events;

public class SC_DeckMenuLogic : MonoBehaviour
{
    public Button Button_PokemonCard;
    public Sprite arrow;
    public Sprite arrowSelected;
    public Sprite randomPokemonCard;
    public Button Button_LeftArrow;
    public Button Button_RightArrow;
    public Slider Slider_Level;
    public Image Image_SliderBackground;
    public Text Text_Level;
    public Text Text_LevelNumber; 
    public Text Text_PokemonName;
    public TextMeshProUGUI Text_PlayDeck;
    public SC_GameLogic SC_GameLogic;
    public SC_MenuLogic SC_MenuLogic;
    public SC_LoadingMenuLogic SC_LoadingMenuLogic;

    public bool isDeckMenuEnabled;
    private int currentSelection;
    public int currentCardIndex;
    private int pokemonListSize;
    public int currentSliderValue;

    private void Awake()
    {
        initDeckMenu();
    }

    void Update()
    {
        HandleDeckMenu();
    }

    public void initDeckMenu()
    {
        isDeckMenuEnabled = false;
        currentSelection = 1;
        currentCardIndex = -1;
        currentSliderValue = 40;
        pokemonListSize = SC_GameLogic.allPokemons.Count;
        Button_PokemonCard.image.sprite = randomPokemonCard;
        Text_PokemonName.text = "Random Pokemon";
        Text_LevelNumber.text = currentSliderValue.ToString();
        Slider_Level.value = currentSliderValue;
    }

    private void HandleDeckMenu()
    {
        if (isDeckMenuEnabled == true)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (currentSelection == 1)
                    currentSelection = 2;
                else if (currentSelection == 3)
                    handleLevelSlider(GlobalEnums.Directions.Right);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (currentSelection == 2)
                    currentSelection = 1;
                else if (currentSelection == 3)
                    handleLevelSlider(GlobalEnums.Directions.Left);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentSelection == 1 || currentSelection == 2)
                    currentSelection = 3;
                else if (currentSelection == 3)
                    currentSelection = 4;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentSelection == 3)
                    currentSelection = 1;
                else if (currentSelection == 4)
                    currentSelection = 3;
            }

            UpdateDeckMenu();

            if (Input.GetKeyDown(KeyCode.Z))
            {
                switch (currentSelection)
                {
                    case 1:
                        handleCardsSwitch(GlobalEnums.Directions.Left);
                        break;

                    case 2:
                        handleCardsSwitch(GlobalEnums.Directions.Right);
                        break;

                    case 4:
                        SC_MenuLogic.menuMusic.Stop();
                        SC_MenuLogic.deckMusic.Stop();
                        SC_MenuLogic.buttonClick.Play();
                        isDeckMenuEnabled = false;
                        SC_LoadingMenuLogic.isLoadingMenuEnabled = true;
                        SC_MenuLogic.PlayMultiplayer(currentSliderValue);
                        break;
                }
            }

            if (Input.GetKeyDown(KeyCode.X))
                handleDeckBackPress();
        }
    }

    private void handleDeckBackPress()
    {
        SC_MenuLogic.backClick.Play();
        SC_MenuLogic.menuMusic.Play();
        SC_MenuLogic.deckMusic.Stop();
        isDeckMenuEnabled = false;
        SC_MenuLogic.isMenuEnabled = true;
        SC_MenuLogic.handleBackPress();
        initDeckMenu();
    }

    private void UpdateDeckMenu()
    {
        switch (currentSelection)
        {
            case 1:
                Button_LeftArrow.image.sprite = arrowSelected;
                Button_LeftArrow.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                Button_RightArrow.image.sprite = arrow;
                Button_RightArrow.transform.localScale = new Vector3(1f, 1f, 1f);
                Text_Level.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                Text_Level.fontSize = 22;
                Text_LevelNumber.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                Text_LevelNumber.fontSize = 22;
                Slider_Level.transform.localScale = new Vector3(1f, 1f, 1f);
                Slider_Level.image.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                Text_PlayDeck.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                Text_PlayDeck.fontSize = 40;
                break;

            case 2:
                Button_LeftArrow.image.sprite = arrow;
                Button_LeftArrow.transform.localScale = new Vector3(1f, 1f, 1f);
                Button_RightArrow.image.sprite = arrowSelected;
                Button_RightArrow.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                Text_Level.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                Text_Level.fontSize = 22;
                Text_LevelNumber.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                Text_LevelNumber.fontSize = 22;
                Slider_Level.transform.localScale = new Vector3(1f, 1f, 1f);
                Slider_Level.image.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                Text_PlayDeck.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                Text_PlayDeck.fontSize = 40;
                break;

            case 3:
                Button_LeftArrow.image.sprite = arrow;
                Button_LeftArrow.transform.localScale = new Vector3(1f, 1f, 1f);
                Button_RightArrow.image.sprite = arrow;
                Button_RightArrow.transform.localScale = new Vector3(1f, 1f, 1f);
                Text_Level.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                Text_Level.fontSize = 26;
                Text_LevelNumber.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                Text_LevelNumber.fontSize = 26;
                Slider_Level.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
                Slider_Level.image.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                Text_PlayDeck.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                Text_PlayDeck.fontSize = 40;
                break;

            case 4:
                Button_LeftArrow.image.sprite = arrow;
                Button_LeftArrow.transform.localScale = new Vector3(1f, 1f, 1f);
                Button_RightArrow.image.sprite = arrow;
                Button_RightArrow.transform.localScale = new Vector3(1f, 1f, 1f);
                Text_Level.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                Text_Level.fontSize = 22;
                Text_LevelNumber.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                Text_LevelNumber.fontSize = 22;
                Slider_Level.transform.localScale = new Vector3(1f, 1f, 1f);
                Slider_Level.image.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                Text_PlayDeck.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                Text_PlayDeck.fontSize = 42;
                break;
        }
    }

    private void handleCardsSwitch(GlobalEnums.Directions _direction)
    {
        SC_MenuLogic.buttonClick.Play();

        if (_direction == GlobalEnums.Directions.Left)
        {
            if (currentCardIndex == -1)
            {
                currentCardIndex = pokemonListSize - 1;
            }
            else if (currentCardIndex == 0)
            {
                currentCardIndex = - 1;
            }
            else
            {
                currentCardIndex--;
            }
        }
        else if (_direction == GlobalEnums.Directions.Right)
        {
            if (currentCardIndex == -1)
            {
                currentCardIndex = 0;
            }
            else if (currentCardIndex == pokemonListSize - 1)
            {
                currentCardIndex = -1;
            }
            else
            {
                currentCardIndex++;
            }
        }

        if (currentCardIndex == -1)
        {
            Button_PokemonCard.image.sprite = randomPokemonCard;
            Text_PokemonName.text = "Random Pokemon";
        }
        else
        {
            Button_PokemonCard.image.sprite = SC_GameLogic.allPokemons[currentCardIndex].cardImage;
            Text_PokemonName.text = SC_GameLogic.allPokemons[currentCardIndex].name;
        }
    }

    private void handleLevelSlider(GlobalEnums.Directions _direction)
    {
        SC_MenuLogic.sliderClick.Play();

        if (_direction == GlobalEnums.Directions.Left)
        {
            if (currentSliderValue > 5 && currentSliderValue <= 100)
            {
                currentSliderValue--;
            }
        }
        else if (_direction == GlobalEnums.Directions.Right)
        {
            if (currentSliderValue >= 5 && currentSliderValue < 100)
            {
                currentSliderValue++;
            }
        }

        Text_LevelNumber.text = currentSliderValue.ToString();
        Slider_Level.value = currentSliderValue;
    }

}

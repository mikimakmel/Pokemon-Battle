using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SC_OptionsMenuLogic : MonoBehaviour
{
    private Dictionary<string, GameObject> GameMusic;
    private Dictionary<string, GameObject> GameSfx;
    private Dictionary<string, GameObject> movesSounds;
    public SC_MenuLogic SC_MenuLogic;
    public Slider Slider_Music;
    public Slider Slider_Sfx;
    public TextMeshProUGUI Text_Music;
    public TextMeshProUGUI Text_Sfx;
    public bool isOptionsMenuEnabled;
    private int currentSelection;
    private int musicSliderValue;
    private int sfxSliderValue;

    private void Awake()
    {
        initGameMusic();
        initGameSfx();
        initMovesSounds();
        initOptionsMenu();
    }

    void Update()
    {
        HandleOptionsMenu();
    }

    private void initGameMusic()
    {
        GameMusic = new Dictionary<string, GameObject>();
        GameObject[] _objects = GameObject.FindGameObjectsWithTag("GameMusic");
        foreach (GameObject obj in _objects)
        {
            if (obj != null)
                GameMusic.Add(obj.name, obj);
        }
    }

    private void initGameSfx()
    {
        GameSfx = new Dictionary<string, GameObject>();
        GameObject[] _objects = GameObject.FindGameObjectsWithTag("GameSfx");
        foreach (GameObject obj in _objects)
        {
            if (obj != null)
                GameSfx.Add(obj.name, obj);
        }
    }

    private void initMovesSounds()
    {
        movesSounds = new Dictionary<string, GameObject>();
        GameObject[] _objects = GameObject.FindGameObjectsWithTag("MovesSounds");
        foreach (GameObject obj in _objects)
        {
            if (obj != null)
                movesSounds.Add(obj.name, obj);
        }
    }

    private void initOptionsMenu()
    {
        isOptionsMenuEnabled = false;
        currentSelection = 1;
        musicSliderValue = (int)Slider_Music.value;
        sfxSliderValue = (int)Slider_Sfx.value;
        UpdateOptionsMenu();
    }

    public void handleMusicSlider(GlobalEnums.Directions _direction)
    {
        if (_direction == GlobalEnums.Directions.Left)
        {
            if (musicSliderValue > 0 && musicSliderValue <= 10)
                musicSliderValue--;
        }
        else if (_direction == GlobalEnums.Directions.Right)
        {
            if (musicSliderValue >= 0 && musicSliderValue < 10)
                musicSliderValue++;
        }

        Slider_Music.value = musicSliderValue;

        foreach (GameObject music in GameMusic.Values)
        {
            if (music != null)
                music.GetComponent<AudioSource>().volume = (float)(musicSliderValue * 0.1);
        }
    }

    public void handleSfxSlider(GlobalEnums.Directions _direction)
    {
        if (_direction == GlobalEnums.Directions.Left)
        {
            if (sfxSliderValue > 0 && sfxSliderValue <= 10)
                sfxSliderValue--;
        }
        else if (_direction == GlobalEnums.Directions.Right)
        {
            if (sfxSliderValue >= 0 && sfxSliderValue < 10)
                sfxSliderValue++;
        }

        Slider_Sfx.value = sfxSliderValue; 

        foreach (GameObject sfx in GameSfx.Values)
        {
            if (sfx != null)
                sfx.GetComponent<AudioSource>().volume = (float)(sfxSliderValue * 0.1);
        }

        foreach (GameObject sfx in movesSounds.Values)
        {
            if (sfx != null)
                sfx.GetComponent<AudioSource>().volume = (float)(sfxSliderValue * 0.1);
        }
    }

    private void HandleOptionsMenu()
    {
        if (isOptionsMenuEnabled == true)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentSelection == 1)
                    currentSelection = 2;
                UpdateOptionsMenu();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (currentSelection == 2)
                    currentSelection = 1;
                UpdateOptionsMenu();
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (currentSelection == 1)
                    handleMusicSlider(GlobalEnums.Directions.Right);
                else if (currentSelection == 2)
                    handleSfxSlider(GlobalEnums.Directions.Right);
                SC_MenuLogic.sliderClick.Play();
                UpdateOptionsMenu();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (currentSelection == 1)
                    handleMusicSlider(GlobalEnums.Directions.Left);
                else if (currentSelection == 2)
                    handleSfxSlider(GlobalEnums.Directions.Left);
                SC_MenuLogic.sliderClick.Play();
                UpdateOptionsMenu();
            }
           
            if (Input.GetKeyDown(KeyCode.X))
            {
                handleOptionsBackPress();
                SC_MenuLogic.backClick.Play();
            }
        }
    }

    private void handleOptionsBackPress()
    {
        isOptionsMenuEnabled = false;
        SC_MenuLogic.isMenuEnabled = true;
        SC_MenuLogic.handleBackPress();
        initOptionsMenu();
    }

    private void UpdateOptionsMenu()
    {
        switch (currentSelection)
        {
            case 1:
                Text_Music.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                Text_Music.fontSize = 30;

                if (musicSliderValue == 0)
                    Text_Music.fontStyle = FontStyles.Strikethrough | FontStyles.Bold;
                else if (musicSliderValue > 0)
                    Text_Music.fontStyle = FontStyles.Bold;

                Slider_Music.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
                Slider_Music.image.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                Text_Sfx.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                Text_Sfx.fontSize = 28;

                if (sfxSliderValue == 0)
                    Text_Sfx.fontStyle = FontStyles.Strikethrough;
                else if (sfxSliderValue > 0)
                    Text_Sfx.fontStyle = FontStyles.Normal;

                Slider_Sfx.transform.localScale = new Vector3(1f, 1f, 1f);
                Slider_Sfx.image.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                break;

            case 2:
                Text_Music.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                Text_Music.fontSize = 28;

                if (musicSliderValue == 0)
                    Text_Music.fontStyle = FontStyles.Strikethrough;
                else if (musicSliderValue > 0)
                    Text_Music.fontStyle = FontStyles.Normal;

                Slider_Music.transform.localScale = new Vector3(1f, 1f, 1f);
                Slider_Music.image.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_REGULAR_COLOR);
                Text_Sfx.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                Text_Sfx.fontSize = 30;

                if (sfxSliderValue == 0)
                    Text_Sfx.fontStyle = FontStyles.Strikethrough | FontStyles.Bold;
                else if (sfxSliderValue > 0)
                    Text_Sfx.fontStyle = FontStyles.Bold;

                Slider_Sfx.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
                Slider_Sfx.image.color = Constants.GetColorFromHexString(Constants.MAIN_MENU_SELECTED_COLOR);
                break;
        }
    }
}

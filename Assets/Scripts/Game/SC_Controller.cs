using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Controller : MonoBehaviour
{
    public SC_MenuLogic SC_MenuLogic;
    public SC_GameLogic SC_GameLogic;

    public void ChangeScreen(string _NewScreen)
    {
        GlobalEnums.MenuScreens _newScreen = (GlobalEnums.MenuScreens)GlobalEnums.MenuScreens.Parse(typeof(GlobalEnums.MenuScreens), _NewScreen);
        if (SC_MenuLogic != null)
            SC_MenuLogic.ChangeScreen(_newScreen);
    }

    public void handleBackPress()
    {
        if (SC_MenuLogic != null)
            SC_MenuLogic.handleBackPress();
    }

    public void handleMusicSlider()
    {
        if (SC_MenuLogic != null)
            SC_MenuLogic.handleMusicSlider();
    }

    public void EnterBattle()
    {
        if (SC_MenuLogic != null)
            SC_GameLogic.EnterBattle();
    }
}

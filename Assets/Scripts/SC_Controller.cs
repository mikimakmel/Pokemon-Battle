using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Screens { Menu, Loading, Battle };

public class SC_Controller : MonoBehaviour
{
    public SC_GameLogic SC_GameLogic;

    public void EnterBattle()
    {
        SC_GameLogic.EnterBattle();
    }
}

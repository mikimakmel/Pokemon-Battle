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

public class SC_LoadingMenuLogic : MonoBehaviour
{
    public SC_DeckMenuLogic SC_DeckMenuLogic;
    public SC_MenuLogic SC_MenuLogic;
    public bool isLoadingMenuEnabled;
    public bool canUserCancel;
    private string RoomId;

    private void OnEnable()
    {
        Listener.OnJoinRoom += OnJoinRoom;
    }

    private void OnDisable()
    {
        Listener.OnJoinRoom -= OnJoinRoom;
    }

    private void Awake()
    {
        initLoadingMenu();
    }

    private void Update()
    {
        HandleLoadingMenu();
    }

    private void initLoadingMenu()
    {
        isLoadingMenuEnabled = false;
        canUserCancel = true;
    }

    private void HandleLoadingMenu()
    {
        if (isLoadingMenuEnabled)
            if (Input.GetKeyDown(KeyCode.X))
            {
                if (canUserCancel)
                    handleLoadingBackPress();

                SC_MenuLogic.backClick.Play();
            }
    }

    private void handleLoadingBackPress()
    {
        SC_MenuLogic.deckMusic.Play();
        WarpClient.GetInstance().LeaveRoom(RoomId);
        WarpClient.GetInstance().stopGame();
        initLoadingMenu();
        SC_DeckMenuLogic.isDeckMenuEnabled = true;
        SC_MenuLogic.handleBackPress();
    }

    private void OnJoinRoom(bool _IsSuccess, string _RoomId)
    {
        if (_IsSuccess)
            RoomId = _RoomId;
    }
}

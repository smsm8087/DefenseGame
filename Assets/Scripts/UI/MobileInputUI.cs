﻿using UnityEngine;
using UnityEngine.UI;

public class MobileInputUI : MonoBehaviour
{
    public FixedJoystick fixedJoystick;
    public Button jumpButton;
    public Button attackButton;
    
    private BasePlayer playerController;

    public static MobileInputUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void RegisterPlayer(BasePlayer player)
    {
        playerController = player;
        //플레이어 조인 이후에 세팅
        this.gameObject.SetActive(true);
    }
    
    void Start()
    {
        InputManager.joystick = fixedJoystick;

        jumpButton.onClick.AddListener(() => {
            if (playerController.GetCurrentState() != playerController.jumpState)
            {
                Debug.Log("Jump button pressed");
                playerController.ChangeState(playerController.jumpState);
            }
        });

        attackButton.onClick.AddListener(() => {
            if (playerController.GetCurrentState() != playerController.attackState)
            {
                Debug.Log("Attack button pressed");
                playerController.ChangeState(playerController.attackState);    
            }
        });
        //플레이어 조인 이후에 세팅
        this.gameObject.SetActive(false);
    }
}
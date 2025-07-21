using System;
using UnityEngine;
using System.Collections.Generic;
using UI;
using UnityEngine.SceneManagement;

public class InitialGameHandler : INetworkMessageHandler
{
    public string Type => "initial_game";
    private int _pendingWaveId = -1;
    public void Handle(NetMsg msg)
    {
        //룸 참가 완료
        _pendingWaveId = msg.wave_id;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadSceneAsync("InGameScene");
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "InGameScene")
        {
            GameManager.Instance.InitializeGame(_pendingWaveId);
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _pendingWaveId = -1;
        }
    }
}
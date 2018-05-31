﻿using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

    [SerializeField] private float gameTime;
    [SyncVar] private float gameTimer;
    [SerializeField] private InGameUI ui;
    public Transform spawnPoint;

    public override void OnStartServer()
    {
        gameTimer = gameTime;
    }

    private void Update()
    {
        if (!isServer) return;
        gameTimer -= Time.deltaTime;
        RpcUpdateTimer();
    }

    [ClientRpc]
    private void RpcUpdateTimer()
    {
        ui.UpdateTimer(gameTimer);
    }

    public float GameTimer
    {
        get { return gameTimer; }
    }
}

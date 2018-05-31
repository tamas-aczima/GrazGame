using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

    [SerializeField] private float gameTime;
    [SyncVar] private float gameTimer;
    [SerializeField] private InGameUI ui;
    public Transform spawnPoint;
    [SyncVar] public int score;

    public override void OnStartServer()
    {
        gameTimer = gameTime;
    }

    private void Start()
    {
        if (!isServer) return;
        RpcUpdateScore(0);
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

    [ClientRpc]
    public void RpcUpdateScore(int value)
    {
        Debug.Log("update score");
        ui.UpdateScore(value);
    }

    public float GameTimer
    {
        get { return gameTimer; }
    }
}

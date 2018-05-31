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
        if (gameTimer > 0)
        {
            gameTimer -= Time.deltaTime;
            RpcUpdateTimer();
        }
        else
        {
            RpcGameOver();
        }
    }

    [ClientRpc]
    private void RpcUpdateTimer()
    {
        ui.UpdateTimer(gameTimer);
    }

    [ClientRpc]
    public void RpcUpdateScore(int value)
    {
        ui.UpdateScore(value);
    }

    [ClientRpc]
    public void RpcGameOver()
    {
        ui.ShowGameOver();
    }

    public float GameTimer
    {
        get { return gameTimer; }
    }
}

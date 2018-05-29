using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

    [SerializeField] private float gameTime;
    private float gameTimer;

    private void Start()
    {
        gameTimer = gameTime;
    }

    private void Update()
    {
        gameTimer -= Time.deltaTime;
    }

    public float GameTimer
    {
        get { return gameTimer; }
    }
}

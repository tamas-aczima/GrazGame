using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour {

    [SerializeField] private Text scoreText;
    [SerializeField] private Text minuteText;
    [SerializeField] private Text secondText;
    [SerializeField] private Canvas gameOverCanvas;

    public void UpdateTimer(float time)
    {
        if (minuteText.text != ((int)time / 60).ToString())
        {
            minuteText.GetComponent<Animator>().Play("Animation");
        }

        minuteText.text = ((int)time / 60).ToString();

        if (((int)time / 60) < 1 && ((int)time % 60) <= 30 && secondText.text != ((int)time % 60).ToString())
        {
            secondText.GetComponent<Animator>().Play("Animation");
        }

        if (((int)time % 60).ToString().Length == 1)
        {
            secondText.text = "0" + ((int)time % 60).ToString();
        }
        else
        {
            secondText.text = ((int)time % 60).ToString();
        }
    }

    public void UpdateScore(int score)
    {
        scoreText.GetComponent<Animator>().Play("Animation");
        scoreText.text = score.ToString();
    }

    public void ShowGameOver()
    {
        gameOverCanvas.gameObject.SetActive(true);
        PlayerController[] players = FindObjectsOfType<PlayerController>();
        foreach (PlayerController player in players)
        {
            player.gameObject.GetComponent<CharacterController>().enabled = false;
        }
    }
}

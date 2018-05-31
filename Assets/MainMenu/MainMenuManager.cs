using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickPlay()
    {
        SceneManager.LoadScene("Bootstrap");

    }

    public void OnClickQuit()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}

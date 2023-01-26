using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject mainUI;

    void Start()
    {
        mainUI.transform.localScale = new Vector3(Screen.width / 1366f, Screen.height / 768f, 1f);
    }

    void Update()
    {
        mainUI.transform.localScale = new Vector3(Screen.width / 1366f, Screen.height / 768f, 1f);
    }   

    public void SingleBackBtnClick()
    {
        SceneManager.LoadScene("Main");
    }
}

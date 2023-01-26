using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class GameManager1 : MonoBehaviour
{
    public static GameManager1 Instance { set; get; }

    public GameObject clickLeaveButton;

    private void Start()
    {
        Instance = this;
        clickLeaveButton.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }

    public void LeaveButton()
    {
        clickLeaveButton.SetActive(true);
    }
    public void LeaveButton1()
    {
        SceneManager.LoadScene("Menu");
    }
    public void CancelButton()
    {
        clickLeaveButton.SetActive(false);
    }
}

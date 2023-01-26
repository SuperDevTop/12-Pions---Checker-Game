using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { set; get; }

    public GameObject mainMenu;
    public GameObject clickMultiPlayer;
    public GameObject clickCreateRoom;
    public GameObject clickJoinRoom;
    public GameObject clickCreateButton;

    public GameObject serverPrefab;
    public GameObject clientPrefab;
    
    public InputField nameInput;

    private void Start()
    {
        Instance = this;
        clickMultiPlayer.SetActive(false);
        clickJoinRoom.SetActive(false);
        clickCreateRoom.SetActive(false);
        clickCreateButton.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }
    public void ClassicButton()
    {
        mainMenu.SetActive(false);
        SceneManager.LoadScene("SampleScene");
    }

    public void MultiPlayerButton()
    {
        mainMenu.SetActive(false);
        clickMultiPlayer.SetActive(true);
    }
    public void ButtonCreate()
    {
        try
        {
            Server s = Instantiate(serverPrefab).GetComponent<Server>();
            s.Init();

            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = nameInput.text;
            c.isHost = true;

            if (c.clientName == "")
                c.clientName = "Host";
            c.ConnectToServer("127.0.0.1", 6321);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        clickCreateRoom.SetActive(false);
        clickCreateButton.SetActive(true);
    }
    public void JoinRoomBuuton()
    {
        clickMultiPlayer.SetActive(false);
        clickJoinRoom.SetActive(true);
    }
    public void CreateRoomButton()
    {
        clickMultiPlayer.SetActive(false);
        clickCreateRoom.SetActive(true);
    }
    public void QuittButton()
    {
        clickMultiPlayer.SetActive(false);
        mainMenu.SetActive(true);
    }
    public void BackButtonCreate()
    {
        clickCreateRoom.SetActive(false);
        clickMultiPlayer.SetActive(true);
    }
    public void BackButtonJoin()
    {
        clickJoinRoom.SetActive(false);
        clickMultiPlayer.SetActive(true);

        Server s = FindObjectOfType<Server>();
        if (s != null)
            Destroy(s.gameObject);

        Client c = FindObjectOfType<Client>();
        if (c != null)
            Destroy(c.gameObject);
    }
    public void ConnectButtonJoin()
    {
        string hostAddress = GameObject.Find("connectInput").GetComponent<InputField>().text;
        if (hostAddress == "")
            hostAddress = "127.0.0.1";

        try
        {
            Client c = Instantiate(clientPrefab).GetComponent<Client>();
            c.clientName = nameInput.text;
            if (c.clientName == "")
                c.clientName = "Client";
            c.ConnectToServer(hostAddress, 6321);
            clickJoinRoom.SetActive(false);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public void CancelButton()
    {
        clickCreateButton.SetActive(false);
        clickCreateRoom.SetActive(true);

        Server s = FindObjectOfType<Server>();
        if (s != null)
            Destroy(s.gameObject);

        Client c = FindObjectOfType<Client>();
        if (c != null)
            Destroy(c.gameObject);
    }
    

    public void StartGame()
    {
        SceneManager.LoadScene("Multiplayer");
    }
}

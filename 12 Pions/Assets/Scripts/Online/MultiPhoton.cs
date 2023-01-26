using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MultiPhoton : MonoBehaviourPunCallbacks, IOnEventCallback
{
    public GameObject mainUI;
    public GameObject[] availableRooms;
    public GameObject onlineUI;
    public GameObject loadingOnline;
    public GameObject roomUI;
    public GameObject joinUI;    
    public GameObject roomNameBack;
    public GameObject roomListBoard;
    public GameObject startBtn;
    public GameObject alert;

    public InputField usernameCreate;
    public InputField usernameJoin;

    public Text myName;
    public Text oppName;
    public Text roomName;
    public Text alertText;
    
    List<RoomInfo> rooms = new List<RoomInfo>();
    public static int singleBoard = 0;
    public static int multiBoard = 0;
    float time;
    bool isConnecting;
    bool isWarning;
    string gameVersion = "1";

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        PhotonNetwork.Disconnect();
    }

    void Update()
    {
        mainUI.transform.localScale = new Vector3(Screen.width / 1366f, Screen.height / 768f, 1f);

        if (isWarning)
        {
            time -= Time.deltaTime;
        }

        if (time < 0)
        {
            isWarning = false;
            alert.SetActive(false);
        }
    }

    #endregion

    #region Public Methods

    public void Connect()
    {
        isConnecting = true;
        ConnectToRegion();
        PhotonNetwork.GameVersion = this.gameVersion;         

        loadingOnline.SetActive(true);
    }

    void ConnectToRegion()
    {
        AppSettings regionSettings = new AppSettings();
        regionSettings.UseNameServer = true;
        regionSettings.FixedRegion = "cae";
        regionSettings.AppIdRealtime = "ec305016-cc17-4b85-bac3-6366fecdaf1a";
        regionSettings.AppVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings(regionSettings);
    }

    public void AlertShow(string str)
    {
        alertText.text = str;
        time = 2f;
        alert.SetActive(true);
        isWarning = true;
    }

    public void CreateRoom()
    {
        if(usernameCreate.text == "")
        {
            AlertShow("Please enter name.");
        }
        else
        {
            onlineUI.SetActive(false);
            roomUI.SetActive(true);            

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = (byte)2;

            PhotonNetwork.CreateRoom(usernameCreate.text, roomOptions, TypedLobby.Default);
            PhotonNetwork.NickName = usernameCreate.text;
            myName.text = usernameCreate.text;
        }                        
    }

    public void SingleBoardSelect()
    {
        singleBoard = int.Parse(EventSystem.current.currentSelectedGameObject.name);
    }

    public void MultiBoardSelect()
    {
        multiBoard = int.Parse(EventSystem.current.currentSelectedGameObject.name);
    }

    public void BackBtnClick()
    {
        PhotonNetwork.Disconnect();
    }

    public void LeaveRoomBtnClick()
    {
        roomUI.SetActive(false);
        onlineUI.SetActive(true);
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public void RoomListClick()
    {           
        if(usernameJoin.text == "")
        {
            AlertShow("Please enter name.");
        }
        else
        {
            PhotonNetwork.JoinRoom(EventSystem.current.currentSelectedGameObject.GetComponent<Text>().text);
            myName.text = EventSystem.current.currentSelectedGameObject.GetComponent<Text>().text;
            PhotonNetwork.NickName = usernameJoin.text;
        }        
    }

    public void StartGame()
    {
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            SceneManager.LoadScene("Multi");            
        }
        else
        {
            AlertShow("Not joined yet.");
        }
    }

    public void SingleGame()
    {
        SceneManager.LoadScene("Single");
    }

    public void JoinBtnClick()
    {
        for (int i = 0; i < availableRooms.Length; i++)
        {
            availableRooms[i].SetActive(false);
        }

        for(int i = 0; i < rooms.Count; i++)
        {
            if (i < 10)
            {
                availableRooms[i].SetActive(true);
                availableRooms[i].GetComponentInChildren<Text>().text = rooms[i].Name;                
            }           
        }
    }

    //public void RefreshBtnClick()
    //{
    //    if (PhotonNetwork.IsConnected)
    //    {
    //        PhotonNetwork.JoinLobby(TypedLobby.Default);
    //    }
    //    else
    //    {
    //        PhotonNetwork.ConnectUsingSettings();
    //    }
    //}

    public void ExitBtnClick()
    {
        Application.Quit();
    }

    #endregion

    #region MonoBehaviourPunCallbacks CallBacks

    public override void OnConnectedToMaster()
    {
        if (isConnecting)
        {
            loadingOnline.SetActive(false);
            onlineUI.SetActive(true);
            PhotonNetwork.JoinLobby(TypedLobby.Default);
        }        
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        print("Disconnected.");
    }

    public override void OnJoinedRoom()
    {
        print("Joined Room.");
        joinUI.SetActive(false);
        onlineUI.SetActive(false);
        roomUI.SetActive(true);
        SendJoinUsername();
    }

    public override void OnCreatedRoom()
    {
        
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        startBtn.SetActive(true);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        oppName.text = "Open";
        startBtn.SetActive(false);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {        
        LeaveRoomBtnClick();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        rooms.Clear();
        
        foreach (var item in roomList)
        {
            if (!item.IsVisible || !item.IsOpen || item.RemovedFromList)
                continue;
 
            rooms.Add(item);
        }         
    }

    public void SendJoinUsername()
    {
        string content = usernameJoin.text;
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All }; // You would have to set the Receivers to All in order to receive this event on the local client as well
        PhotonNetwork.RaiseEvent(1, content, raiseEventOptions, SendOptions.SendReliable);
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;

        if (eventCode == 1)
        {
            string data = (string)photonEvent.CustomData;

            if(data == "")
            {
                oppName.text = "Open";
            }
            else
            {
                oppName.text = data;
            }            
        }
    }

    #endregion
}

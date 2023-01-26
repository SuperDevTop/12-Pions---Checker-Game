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
    public GameObject playUI;
    public GameObject joinUI;
    public GameObject roomNameBack;
    public GameObject roomListBoard;
    public GameObject startBtn;

    public InputField usernameCreate;
    public InputField usernameJoin;

    public Text myName;
    public Text oppName;
    public Text roomName;
    
    List<RoomInfo> rooms = new List<RoomInfo>();
    bool isConnecting;
    string gameVersion = "1";

    #region MonoBehaviour CallBacks

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    
    void Update()
    {
        mainUI.transform.localScale = new Vector3(Screen.width / 1366f, Screen.height / 768f, 1f);
    }

    #endregion

    #region Public Methods

    public void Connect()
    {
        isConnecting = true;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = this.gameVersion;         

        loadingOnline.SetActive(true);
    }

    public void CreateRoom()
    {
        if(usernameCreate.text == "")
        {
            print("Enter name");
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
            myName.text = usernameCreate.text;
        }                        
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
            print("Enter name");
        }
        else
        {
            PhotonNetwork.JoinRoom(EventSystem.current.currentSelectedGameObject.GetComponent<Text>().text);
            myName.text = EventSystem.current.currentSelectedGameObject.GetComponent<Text>().text;
        }        
    }

    public void StartGame()
    {
        if (PhotonNetwork.PlayerList.Length == 2)
        {
            //PhotonNetwork.CurrentRoom.IsVisible = false;
            //PhotonNetwork.CurrentRoom.IsOpen = false;
            SceneManager.LoadScene("Multi");            
        }
        else
        {            
            print("Not yet..................");
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
        print("" + rooms.Count);

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
        startBtn.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
              
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        oppName.text = "Open";
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

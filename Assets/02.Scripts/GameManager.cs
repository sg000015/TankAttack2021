using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviourPunCallbacks
{
    [Header("Room Info")]
    public TMP_Text roomNameText;
    public TMP_Text connectInfoText;
    public TMP_Text msgText;
    public Button exitButton;

    [Header("Chatting UI")]
    public TMP_Text chatListText;
    public TMP_InputField msgIF;

    private PhotonView pv;

    public static GameManager instance = null;


    void Awake()

    {
        Vector3 pos = new Vector3(Random.Range(-200.0f,200.0f),20.0f,Random.Range(-200.0f,200.0f));
        PhotonNetwork.Instantiate("Tank",pos,Quaternion.identity,0);
        instance = this;
    }    



    
    // Start is called before the first frame update
    void Start()
    {
        pv = GetComponent<PhotonView>();
        // pv = photonView;
        SetRoomInfo();
    }

    void SetRoomInfo()
    {
        Room currentRoom = PhotonNetwork.CurrentRoom;
        roomNameText.text = currentRoom.Name;
        connectInfoText.text = $"{currentRoom.PlayerCount} / {currentRoom.MaxPlayers}";
    }

    public void onExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }

    //CleanUp 끝난 뒤에 호출되는 콜백
    public override void OnLeftRoom()
    {
        //Lobby로 돌아가기
        SceneManager.LoadScene("Lobby");
        
    }

    public override void OnJoinedRoom()
    {
        string msg = "Join Room successfully";
        msgText.text += msg;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetRoomInfo();
        string msg = $"\n<color=#00ff00>{newPlayer.NickName}</color> is joined the room";
        msgText.text += msg;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetRoomInfo();
        string msg = $"\n<color=#ff0000>{otherPlayer.NickName}</color> left the room";
        msgText.text += msg;
    }

    public void OnSendClick()
    {
        string _msg = $"<color=#00ff00>{PhotonNetwork.NickName}</color>:{msgIF.text}";        
        pv.RPC("SendChatMessage",RpcTarget.AllBufferedViaServer,_msg);
    }

    [PunRPC]
    void SendChatMessage(string msg)
    {
        chatListText.text+= $"{msg}\n";
    }





    // Update is called once per frame
    void Update()
    {
        
    }
}

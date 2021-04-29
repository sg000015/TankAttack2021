using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "v1.0.0";
    private string userId = "^[Ò_Ó]^";

    public TMP_InputField userIdText;
    public TMP_InputField roomNameText;

    //룸 목록 저장하기 위한 딕셔너리 자료형
    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();
    //룸을 표시할 프리팹
    public GameObject roomPrefab;
    //룸 프리팹이 차일드화 시킬 부모 객체
    public Transform scrollContent;


    void Awake()
    {
        //Scene전환
        PhotonNetwork.AutomaticallySyncScene = true;
        //게임 버전 지정
        PhotonNetwork.GameVersion = gameVersion;
        //유저명 지점
        //PhotonNetwork.NickName = userId;
        
        //서버 접속
        PhotonNetwork.ConnectUsingSettings();


    }

    void Start()
    {
        userId = PlayerPrefs.GetString("USER_ID",$"USER_{Random.Range(0,100):00}");
        userIdText.text = userId;
        PhotonNetwork.NickName = userId;
    }

    public override void OnConnectedToMaster()
    {
        
        Debug.Log("Connected to Photon Server");
        //PhotonNetwork.JoinRandomRoom(); //랜덤방 입장 시도

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("joined lobby");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)//입장실패
    {
        Debug.Log($"code={returnCode}, msg={message}");
        //룸 속성 설정
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 30;

        if(string.IsNullOrEmpty(roomNameText.text))
        roomNameText.text = $"ROOM_{Random.Range(0,100):000}";
        
        //룸 생성
        PhotonNetwork.CreateRoom(roomNameText.text,ro);

    }

    // 룸 생성 완료 콜백
    public override void OnCreatedRoom()
    {
        Debug.Log("Create Room successfully");
    }


    //룸 입장 완료 콜백
    public override void OnJoinedRoom()
    {
        Debug.Log("Join Room successfully");
        Debug.Log(PhotonNetwork.CurrentRoom.Name);

        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("BattleField");
        }
        // PhotonNetwork.Instantiate("Tank",new Vector3(0,30,0),Quaternion.identity,0);
    }

    //룸 목록 수신 -> 변경/갱신시 콜백
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;//지역변수는 초기화 필요

        foreach(RoomInfo room in roomList)
        {
            //Debug.Log($"room name={room.Name}, ({room.PlayerCount}/{room.MaxPlayers})");
            if(room.RemovedFromList ==true)
            {
                //딕셔너리에 삭제, roomItem 프리팹 삭제
                roomDict.TryGetValue(room.Name,out tempRoom);
                //RoomItem 프리팹 삭제
                Destroy(tempRoom);
                //딕셔너리에서 데이터를 삭제
                roomDict.Remove(room.Name);
            }
            else   //룸 정보 갱신
            {
                //처음 생성된 경우 딕셔너리에 데이터추가, roomItem 생성
                if(roomDict.ContainsKey(room.Name)==false)
                {
                    GameObject _room = Instantiate(roomPrefab,scrollContent);
                    //_room.GetComponentInChildren<TMP_Text>().text = room.Name;
                    _room.GetComponent<RoomData>().RoomInfo=room;
                    roomDict.Add(room.Name,_room);
                }
                else
                {
                    //룸 정보를 갱신
                    roomDict.TryGetValue(room.Name,out tempRoom);
                    tempRoom.GetComponent<RoomData>().RoomInfo=room;
                }
            }
        }       
        
    }









#region UI_BUTTON_CALLBACK

    public void OnLoginClick()
    {
        if(string.IsNullOrEmpty(userIdText.text))
        {
            userId = $"USER_{Random.Range(0,100):00}";
            userIdText.text = userId;
        }
        PlayerPrefs.SetString("USER_ID",userIdText.text);
        PhotonNetwork.NickName = userIdText.text;
        PhotonNetwork.JoinRandomRoom();
        //PhotonNetwork.JoinRoom(GetComponent<RoomData>().RoomInfo.Name);
    }
    
    public void OnMakeRoomClick()
    {
        //룸 속성 설정
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = 30;

        if(string.IsNullOrEmpty(roomNameText.text))
        {
            roomNameText.text = $"ROOM_{Random.Range(0,100):000}";
        }
        //룸 생성
        PhotonNetwork.CreateRoom(roomNameText.text,ro);
    }

#endregion
    


}

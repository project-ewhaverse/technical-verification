using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    public Text text;

    string gameVersion = "1";   //버전
    string currevent;   //이벤트 발생 시 해당 오브젝트 이름 저장할 변수

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //룸 입장 버튼 클릭 시 실행
    public void Connect()
    {
        currevent = EventSystem.current.currentSelectedGameObject.name;   // 현재 발생한 이벤트의 게임 오브젝트 이름 가져오기
        //마스터 서버에 접속 중이라면
        if (PhotonNetwork.IsConnected)
        {

            PhotonNetwork.JoinRoom(currevent);

            //PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    //마스터 서버 연결 성공시 자동 실행
    public override void OnConnectedToMaster()
    {
        text.text = "서버 연결 성공! 방에 입장하세요!";
    }

    //마스터 서버 연결 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        text.text = "연결 중...";
        PhotonNetwork.ConnectUsingSettings();
    }

    // 룸에 들어갈 때 호출
    public override void OnJoinedRoom()
    {
        Debug.Log("We load a " + currevent);
        PhotonNetwork.LoadLevel(currevent);
    }

    // 룸 입장 실패 시(
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        text.text = "방 입장 중...잠시만 기다려 주세요";
        PhotonNetwork.CreateRoom(currevent, new RoomOptions { MaxPlayers = 10 });
    }
}

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.EventSystems;

public class Launcher : MonoBehaviourPunCallbacks
{

    string gameVersion = "1";   //����
    string currevent;   //�̺�Ʈ �߻� �� �ش� ������Ʈ �̸� ������ ����

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    //�� ���� ��ư Ŭ�� �� ����
    public void Connect()
    {
        currevent = EventSystem.current.currentSelectedGameObject.name;   // ���� �߻��� �̺�Ʈ�� ���� ������Ʈ �̸� ��������
        //������ ������ ���� ���̶��
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

    //������ ���� ���� ������ �ڵ� ����
    public override void OnConnectedToMaster()
    {
        
        
    }

    //������ ���� ���� ���н� �ڵ� ����
    public override void OnDisconnected(DisconnectCause cause)
    {
        
    }

    // �뿡 �� �� ȣ��
    public override void OnJoinedRoom()
    {
        Debug.Log("We load a " + currevent);
        PhotonNetwork.LoadLevel(currevent);
    }

    // �� ���� ���� ��(
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        PhotonNetwork.CreateRoom(currevent, new RoomOptions { MaxPlayers = 10 });
    }
}

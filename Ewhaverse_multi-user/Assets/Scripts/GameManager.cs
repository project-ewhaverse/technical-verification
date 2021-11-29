using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;


public class GameManager : MonoBehaviourPunCallbacks
{
    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;

    void Start()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("< Color = Red >< a > Missing </ a ></ Color > playerPrefab Reference.Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            //�� �ȿ��� �÷��̾� �ν��Ͻ� ��Ÿ��
            PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);  //0�� ��(�κ�) �ε�
    }

    //���� ������ �޼ҵ�
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
}

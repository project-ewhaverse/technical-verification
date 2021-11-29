using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
public class player : MonoBehaviourPun
{
    float Mspeed = 5.0f;
    float Rspeed = 120.0f;

    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        transform.Translate(Vector3.forward * v * Time.deltaTime * Mspeed);
        transform.Rotate(Vector3.up * h * Time.deltaTime * Rspeed);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AnimationController : MonoBehaviourPun
{
    Animator animator;
    int isWalkingHash;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }


        JumpingUpdate();
        WalkingUpdate();
    }
    void WalkingUpdate()
    {
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");
        bool isWalking = animator.GetBool(isWalkingHash);
        if (hAxis != 0 || vAxis != 0)
        {
            if (!isWalking) animator.SetBool("isWalking", true);
        }
        else
        {
            if (isWalking) animator.SetBool("isWalking", false);
        }
    }

    void JumpingUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }
    }
}

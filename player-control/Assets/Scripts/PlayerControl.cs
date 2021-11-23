using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour    
{
    //private Rigidbody rigid;
    [HideInInspector] public CharacterController controller;
    
    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private float speed = 4f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Camera")]
    [SerializeField] private Transform camera;
    [SerializeField] private Transform cameraArm;
    [SerializeField] private float camSens = 2f;

    [Header("Jump Settings")]
    [SerializeField] float Gravity = 9.81f;
    [SerializeField] float jumpHeight = 1f;
    [SerializeField] float jumpForwardAppliedForce = .5f;
    [SerializeField] float airControl = 5f;
    [SerializeField] float stepDown = .2f;

    bool isJumping= false;
    Vector3 velocity;



    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        CameraLookAt();
        Translate();        
    }

    void Translate()
    {
        float hAxis = Input.GetAxis("Horizontal");
        float vAxis = Input.GetAxis("Vertical");

        if (isJumping)
		{
            velocity.y -= Gravity * Time.deltaTime;
            if (hAxis != 0 || vAxis != 0)
			{
                Vector3 forward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
                Vector3 right = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
                controller.Move(velocity * Time.deltaTime +  (forward * vAxis + right * hAxis) * 0.5f * speed * Time.deltaTime);
                //player.LookAt(player.position + forward * vAxis + right * hAxis);
                Quaternion LookAt = Quaternion.LookRotation(forward * vAxis + right * hAxis);
                player.rotation = Quaternion.Slerp(player.rotation, LookAt, Time.deltaTime * rotationSpeed);
            }
            isJumping = !controller.isGrounded;
        }

		else
		{
            if (hAxis != 0 || vAxis != 0)
            {
                Vector3 forward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
                Vector3 right = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;  
                controller.Move((forward * vAxis + right * hAxis) * speed * Time.deltaTime);
                Quaternion LookAt = Quaternion.LookRotation(forward * vAxis + right * hAxis);
                player.rotation = Quaternion.Slerp(player.rotation, LookAt, Time.deltaTime * rotationSpeed);
            }
            if (!controller.isGrounded)
            {
                isJumping = true;
                velocity = controller.velocity * jumpForwardAppliedForce;
                velocity.y = 0f;
            }

        }
    }

    void CameraLookAt()
	{
		if (Input.GetMouseButton(0)) {
            Vector2 mouseDelta = new Vector2(camSens * Input.GetAxis("Mouse X"), camSens * Input.GetAxis("Mouse Y"));
            Vector3 camAngle = cameraArm.rotation.eulerAngles;
          
            cameraArm.rotation = Quaternion.Euler(camAngle.x - mouseDelta.y, camAngle.y + mouseDelta.x, camAngle.z);
        }
        
    } 

    void Jump()
	{
        if (Input.GetKeyDown(KeyCode.Space)){
			//rigid.AddForce(Vector3.up * 5f, ForceMode.Impulse);
			if(!isJumping){
                isJumping = true;
                velocity= controller.velocity * jumpForwardAppliedForce;
                velocity.y = Mathf.Sqrt(2 * Gravity * jumpHeight);
            }
        }
    }


}

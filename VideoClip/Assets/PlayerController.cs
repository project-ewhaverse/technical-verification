using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform camera;
    [SerializeField] private Transform cameraArm;
    [SerializeField] private float camSens = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CameraLookAt();
    }

    void CameraLookAt()
    {
        if (Input.GetMouseButton(0))
        {
            float mouseDeltaX = camSens * Input.GetAxis("Mouse X");
            float mouseDeltaY = camSens * Input.GetAxis("Mouse Y");

            Vector3 camAngle = cameraArm.rotation.eulerAngles;
            cameraArm.rotation = Quaternion.Euler(camAngle.x - mouseDeltaY, camAngle.y + mouseDeltaX, camAngle.z);
        }

    }
}

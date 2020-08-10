using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostPlayerViewController : MonoBehaviour
{
    private float lookSensitivity = 100f;

    [SerializeField] private Camera playerCamera = null;
    [SerializeField] private Transform playerBody = null;

    private float yAxisRotation = 0f; //The rotation of left and right.
    private float xAxisRotation = 0f; //The rotation of up and down.
 
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        ReadMouseInputs();
    }

    private void ReadMouseInputs()
    {
        float mouseXPos = Input.GetAxis("Mouse X") * lookSensitivity * Time.deltaTime;
        float mouseYPos = Input.GetAxis("Mouse Y") * lookSensitivity * Time.deltaTime;

        xAxisRotation -= mouseYPos;
        xAxisRotation = Mathf.Clamp(xAxisRotation, -90f, 90f); //Restricts the head movement to straight up and straight down.

        yAxisRotation += mouseXPos;

        playerCamera.transform.localRotation = Quaternion.Euler(xAxisRotation, 0f, 0f);
        playerBody.transform.localRotation = Quaternion.Euler(0f, yAxisRotation, 0f);
    }
}

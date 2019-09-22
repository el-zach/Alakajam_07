using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControls : MonoBehaviour
{
    public Cinemachine.CinemachineFreeLook virtualCam;
    public float sensitivity = 2f;
    public string mouseButton = "Fire1";
    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis(mouseButton) > 0f)
        {
            virtualCam.m_XAxis.m_InputAxisValue = Input.GetAxis("Mouse X")* sensitivity;
            virtualCam.m_YAxis.m_InputAxisValue = Input.GetAxis("Mouse Y")* sensitivity;
        }
        else
        {
            virtualCam.m_XAxis.m_InputAxisValue = 0f;
            virtualCam.m_YAxis.m_InputAxisValue = 0f;
        }
    }
}

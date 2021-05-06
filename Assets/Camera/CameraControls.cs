using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControls : MonoBehaviour {
    CinemachineFreeLook cameraFreeLook;

    void Start() {
        cameraFreeLook = GetComponent<CinemachineFreeLook>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        if ( Input.mouseScrollDelta.y != 0 ) {
            cameraFreeLook.m_Orbits[0].m_Height = Mathf.Clamp( cameraFreeLook.m_Orbits[0].m_Height -Input.mouseScrollDelta.y/2, 1, 11 );
            cameraFreeLook.m_Orbits[1].m_Radius = Mathf.Clamp( cameraFreeLook.m_Orbits[1].m_Radius -Input.mouseScrollDelta.y/2, 1, 11 );
            cameraFreeLook.m_Orbits[2].m_Height = Mathf.Clamp( cameraFreeLook.m_Orbits[2].m_Height +Input.mouseScrollDelta.y/2, -11, -1 );
        }
    }
}

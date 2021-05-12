using UnityEngine;
using Cinemachine;

public class CameraControls : MonoBehaviour {
    private CinemachineFreeLook _cinemachineFreeLook;

    private void Start() {
        _cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        if ( Input.mouseScrollDelta.y != 0 )
        {
            _cinemachineFreeLook.m_Orbits[0].m_Height =
                Mathf.Clamp(_cinemachineFreeLook.m_Orbits[0].m_Height - Input.mouseScrollDelta.y / 2, 1, 11);
            _cinemachineFreeLook.m_Orbits[1].m_Radius =
                Mathf.Clamp(_cinemachineFreeLook.m_Orbits[1].m_Radius - Input.mouseScrollDelta.y / 2, 1, 11);
            _cinemachineFreeLook.m_Orbits[2].m_Height =
                Mathf.Clamp(_cinemachineFreeLook.m_Orbits[2].m_Height + Input.mouseScrollDelta.y / 2, -11, -1);
        }
    }
}

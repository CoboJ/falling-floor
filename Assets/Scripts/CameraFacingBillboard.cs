using UnityEngine;

public class CameraFacingBillboard : MonoBehaviour
{
    private Camera m_Camera;

    void Start() 
    {
        m_Camera = Camera.main;
    }
 
    // Orient the camera after all movement is completed this frame to avoid jittering.
    void LateUpdate()
    {
        if(m_Camera == null) { m_Camera = Camera.main; return; }

        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);    
    }
}

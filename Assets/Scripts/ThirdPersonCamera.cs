using UnityEngine;
using Rewired;
using DG.Tweening;

public class ThirdPersonCamera : MonoBehaviour
{
    public float mouseSensitivity = 10;
    [HideInInspector] public Transform target;
    public float distFromTarget = 2;
    public Vector2 pitchMinMax = new Vector2(-45, 90);
    public float movePitch;

    public float rotSmoothTime = .2f;
    Vector3 rotSmoothVelocity;
    Vector3 currentRotation;

    [SerializeField] private float smoothMove = 2;
    [SerializeField] private Vector3 endPos;
    [HideInInspector] public bool playerInactive;

    [HideInInspector] public Player player;
    float yaw;
    float pitch;
    [HideInInspector] public FallingFloorAgent agentScript;
    [HideInInspector] public Transform focusPoint;

    private void LateUpdate() 
    {
        if(GameManager.Instance.gameState.Equals(GameManager.GameState.GameOver) || target == null || playerInactive) { return; }

        yaw += (agentScript == null ? player.GetAxis("Rotate Y") : agentScript.horizontal) * mouseSensitivity;
        pitch -= movePitch;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        currentRotation = Vector3.SmoothDamp (
            currentRotation, 
            new Vector3(pitch, yaw), 
            ref rotSmoothVelocity,
            rotSmoothTime
        );

        transform.eulerAngles = currentRotation;

        transform.position = target.position - transform.forward * distFromTarget;
    }

    public void FocusCharacter()
    {
        if(focusPoint == null) return;

        transform.DOMove(focusPoint.position, 1f).SetEase(Ease.InOutSine);
        transform.DORotateQuaternion(focusPoint.rotation, 1f).SetEase(Ease.InOutSine);
    }

    public void CenterInCharacter()
    {   
        transform.DOMove((target.position - transform.forward * distFromTarget), 1f).SetEase(Ease.OutSine)
            .OnComplete(()=>{
                playerInactive = false;
            });
    }
}

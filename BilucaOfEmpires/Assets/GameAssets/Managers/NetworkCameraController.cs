using Assets.UnityFoundation.Code.CameraScripts.NewInputSystem;
using Mirror;
using UnityEngine;

public class NetworkCameraController : NetworkBehaviour
{
    [SerializeField] private float cameraSpeed = 20f;
    [SerializeField] private float edgeOffset = 10f;
    [SerializeField] private Vector2 moveLimitsX;
    [SerializeField] private Vector2 moveLimitsZ;

    public Transform cameraTransform;

    private CameraMovementXZ cameraMovement;

    private void Start()
    {
        cameraTransform = transform.Find("main_virtual_camera");
        cameraMovement = new CameraMovementXZ(cameraTransform) {
            CameraSpeed = cameraSpeed,
            EdgeOffset = edgeOffset,
            MoveLimitsX = moveLimitsX,
            MoveLimitsZ = moveLimitsZ
        };
    }

    [ClientCallback]
    private void Update()
    {
        if(!hasAuthority || !Application.isFocused)
            return;

        if(cameraTransform == null)
        {
            cameraTransform = transform.Find("main_virtual_camera");
            cameraMovement.SetTargetTransform(cameraTransform);
        }
        else
        {
            cameraMovement.OnUpdate();
        }
        
    }

}

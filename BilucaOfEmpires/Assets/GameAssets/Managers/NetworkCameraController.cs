using Assets.UnityFoundation.Code.CameraScripts.NewInputSystem;
using Mirror;
using UnityEngine;

public class NetworkCameraController : NetworkBehaviour
{
    [SerializeField] private float cameraSpeed = 20f;
    [SerializeField] private float edgeOffset = 10f;
    [SerializeField] private Vector2 moveLimitsX;
    [SerializeField] private Vector2 moveLimitsZ;

    private CameraMovementXZ cameraMovement;

    public override void OnStartServer()
    {
        cameraMovement = new CameraMovementXZ(transform.Find("main_virutal_camera")) {
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

        cameraMovement.OnUpdate();
    }

}

using Assets.UnityFoundation.CameraScripts;
using Mirror;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class UnitMovement : NetworkBehaviour {
    private UnitMovementServer server;
    private Camera mainCamera;

    public void Awake() {
        mainCamera = Camera.main;

        var agent = GetComponent<NavMeshAgent>();
        server = GetComponent<UnitMovementServer>()
            .Setup(agent);
    }

    [ClientCallback]
    private void Update() {
        if(!hasAuthority) return;

        if(!Mouse.current.rightButton.wasPressedThisFrame) return;

        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return;

        server.CmdMove(hit.point);
    }
}

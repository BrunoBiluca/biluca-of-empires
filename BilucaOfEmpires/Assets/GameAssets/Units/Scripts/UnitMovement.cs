using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour {
    private UnitMovementServer server;

    public void Awake() {
        var agent = GetComponent<NavMeshAgent>();
        server = GetComponent<UnitMovementServer>()
            .Setup(agent);
    }

    [ClientCallback]
    public void Move(Vector3 point) {
        server.CmdMove(point);
    }
}

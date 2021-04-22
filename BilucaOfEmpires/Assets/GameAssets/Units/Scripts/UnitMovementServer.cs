using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovementServer : NetworkBehaviour {
    private NavMeshAgent agent;

    public UnitMovementServer Setup(NavMeshAgent agent) {
        this.agent = agent;
        return this;
    }

    [Command]
    public void CmdMove(Vector3 position) {
        Debug.Log(position);
        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;

        agent.SetDestination(hit.position);
    }
}

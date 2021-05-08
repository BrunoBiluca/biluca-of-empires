using Assets.UnityFoundation.Code;
using Mirror;
using System;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovementServer : NetworkBehaviour
{
    [SerializeField] float chaseRange;
    private Unit unit;
    private NavMeshAgent agent;

    public UnitMovementServer Setup(NavMeshAgent agent)
    {
        unit = GetComponent<Unit>();
        this.agent = agent;
        return this;
    }

    [ServerCallback]
    private void Update()
    {
        if(unit.TargetHandler.Target != null)
        {
            ChaseUnit();
            return;
        }

        if(!agent.hasPath) return;

        if(agent.remainingDistance > agent.stoppingDistance) return;

        agent.ResetPath();
    }

    private void ChaseUnit()
    {
        if(!TransformUtils.IsInRange(unit.TargetHandler.Target.transform, transform, chaseRange))
        {
            agent.SetDestination(unit.TargetHandler.Target.transform.position);
            return;
        }

        if(agent.hasPath)
        {
            agent.ResetPath();
        }
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;

        agent.SetDestination(hit.position);
    }
}

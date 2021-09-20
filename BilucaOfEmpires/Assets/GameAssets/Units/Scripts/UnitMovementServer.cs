using Assets.UnityFoundation.Code;
using Mirror;
using System;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovementServer : NetworkBehaviour
{
    [SerializeField] float chaseRange = 15f;
    private Unit unit;
    private NavMeshAgent agent;

    public UnitMovementServer Setup(NavMeshAgent agent)
    {
        unit = GetComponent<Unit>();
        this.agent = agent;
        return this;
    }
    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [Server]
    private void ServerHandleGameOver()
    {
        agent.ResetPath();
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

    [Server]
    public void Move(Vector3 position)
    {
        unit.TargetHandler.ClearTarget();

        if(!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;

        agent.SetDestination(hit.position);
    }
}

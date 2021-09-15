using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    public static event Action<int> ClientOnGameOver;

    public static event Action ServerOnGameOver;

    private readonly List<UnitHeadQuarters> headQuarters = new List<UnitHeadQuarters>();

    #region Server

    public override void OnStartServer()
    {
        UnitHeadQuarters.ServerOnBaseSpawned += ServerHandleBaseSpawned;
        UnitHeadQuarters.ServerOnBaseDespawned += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        UnitHeadQuarters.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
        UnitHeadQuarters.ServerOnBaseDespawned -= ServerHandleBaseDespawned;
    }

    [Server]
    private void ServerHandleBaseSpawned(UnitHeadQuarters unitBase)
    {
        headQuarters.Add(unitBase);
    }

    [Server]
    private void ServerHandleBaseDespawned(UnitHeadQuarters unitBase)
    {
        headQuarters.Remove(unitBase);

        if(headQuarters.Count != 1) { return; }

        int playerId = headQuarters[0].connectionToClient.connectionId;

        RpcGameOver(playerId);

        ServerOnGameOver?.Invoke();
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(int winnerId)
    {
        ClientOnGameOver?.Invoke(winnerId);
    }

    #endregion

}

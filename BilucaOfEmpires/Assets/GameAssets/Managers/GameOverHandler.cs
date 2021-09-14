using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameOverHandler : NetworkBehaviour
{
    [SerializeField] private GameObject gameOverMenuPrefab;

    private GameOverMenu gameOverMenu;

    private readonly List<UnitHeadQuarters> headQuarters = new List<UnitHeadQuarters>();

    private void Awake()
    {
        var go = Instantiate(gameOverMenuPrefab);
        gameOverMenu = go.GetComponent<GameOverMenu>();
        gameOverMenu.Setup("Leave Game", LeaveGame);
    }

    private void LeaveGame()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }

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

        RpcGameOver($"Player {playerId}");
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        gameOverMenu.Show($"Player {winner} won!");
    }

    #endregion

}

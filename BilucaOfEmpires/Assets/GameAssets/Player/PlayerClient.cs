using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClient : NetworkBehaviour
{
    public List<Unit> Units {
        get {
            if(isClientOnly) return clientUnits;

            return playerServer.Units;
        }
    }

    public int Resources => playerServer.Resources;

    public event Action<int> ClientOnResourcesUpdated;

    [SerializeField] private List<Unit> clientUnits = new List<Unit>();

    private PlayerServer playerServer;

    private void Start()
    {
        playerServer = gameObject.GetComponent<PlayerServer>();
        playerServer.ServerOnResourcesUpdated
            += (resources) => ClientOnResourcesUpdated?.Invoke(Resources);
    }

    public override void OnStartAuthority()
    {
        if(NetworkServer.active) { return; }

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }

    public override void OnStartClient()
    {
        if(NetworkServer.active) 
            return;

        DontDestroyOnLoad(gameObject);

        PlayerServer player = gameObject.GetComponent<PlayerServer>();
        ((GameNetworkManager)NetworkManager.singleton)
            .Players
            .Add(player);
    }


    public override void OnStopClient()
    {
        PlayerServer player = gameObject.GetComponent<PlayerServer>();
        player.PlayerDisconnected();

        if(!isClientOnly) { return; }

        ((GameNetworkManager)NetworkManager.singleton)
            .Players
            .Remove(gameObject.GetComponent<PlayerServer>());

        if(!hasAuthority) { return; }

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
    }

    [Command]
    public void IncreaseResources(int addResources)
    {
        playerServer.IncreaseResources(addResources);
        ClientOnResourcesUpdated?.Invoke(Resources);
    }

    private void AuthorityHandleUnitSpawned(Unit unit)
    {
        if(!hasAuthority) { return; }

        clientUnits.Add(unit);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        if(!hasAuthority) { return; }

        clientUnits.Remove(unit);
    }

    [Command]
    public void CmdTryPlaceBuilding(int id, Vector3 point)
    {
        playerServer.TryPlaceBuilding(id, point);
    }

    public bool CanBuild(Building building, Vector3 point)
        => playerServer.CanBuild(building, point);
}

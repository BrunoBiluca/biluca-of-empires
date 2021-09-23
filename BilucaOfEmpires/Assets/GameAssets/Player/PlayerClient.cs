using Assets.UnityFoundation.UI.Minimap;
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

    private MinimapController minimapController;

    private void Start()
    {
        playerServer = GetComponent<PlayerServer>();
        playerServer.ServerOnResourcesUpdated 
            += (resources) => ClientOnResourcesUpdated?.Invoke(Resources);

        minimapController = FindObjectOfType<MinimapController>();
        minimapController.TargetTransform = transform.Find("main_virutal_camera");
    }

    public override void OnStartAuthority()
    {
        if(NetworkServer.active) { return; }

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }

    public override void OnStopClient()
    {
        if(!isClientOnly && !hasAuthority) { return; }

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

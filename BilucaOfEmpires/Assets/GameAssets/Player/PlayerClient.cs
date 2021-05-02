using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerClient : NetworkBehaviour
{
    public List<Unit> Units {
        get {
            if(isClientOnly) return clientUnits;

            return playerServer.Units;
        }
    }

    [SerializeField] private List<Unit> clientUnits = new List<Unit>();

    private PlayerServer playerServer;

    private void Start()
    {
        playerServer = GetComponent<PlayerServer>();
    }

    public override void OnStartClient()
    {
        if(!isClientOnly) { return; }

        Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
    }

    public override void OnStopClient()
    {
        if(!isClientOnly) { return; }

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
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
}
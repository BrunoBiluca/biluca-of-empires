using Mirror;
using System;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{

    private UnitSpawnerServer server;

    private void Awake()
    {
        server = GetComponent<UnitSpawnerServer>();

        var healthSystem = GetComponent<HealthSystemServer>();
        healthSystem.ServerOnDie += HandleServerOnDie;
    }

    [Server]
    private void HandleServerOnDie()
    {
        //NetworkServer.Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if(eventData.button != PointerEventData.InputButton.Left) return;

        if(!hasAuthority) return;

        server.CmdSpawnUnit();
    }
}

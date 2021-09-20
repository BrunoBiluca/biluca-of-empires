using Mirror;
using System;
using UnityEngine;

public class Unit : NetworkBehaviour
{
    [SerializeField] private int resourcesCost = 20;

    public int ResourcesCost => resourcesCost;

    public UnitMovement Movement { get; private set; }
    public TargetHandlerServer TargetHandler { get; private set; }

    public EventHandler OnUnitSelected;
    public EventHandler OnUnitDeselected;

    public static event Action<Unit> AuthorityOnUnitSpawned;
    public static event Action<Unit> AuthorityOnUnitDespawned;
    public static event Action<Unit> ServerOnUnitSpawned;
    public static event Action<Unit> ServerOnUnitDespawned;

    private void Awake()
    {
        Movement = GetComponent<UnitMovement>();
        TargetHandler = GetComponent<TargetHandlerServer>();

        var healthSystem = GetComponent<HealthSystemServer>();
        healthSystem.ServerOnDie += HandleServerOnDie;
    }

    [Client]
    public void Select()
    {
        if(!hasAuthority) return;

        OnUnitSelected?.Invoke(this, EventArgs.Empty);
    }

    [Client]
    public void Deselect()
    {
        if(!hasAuthority) return;

        OnUnitDeselected?.Invoke(this, EventArgs.Empty);
    }

    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if(!hasAuthority) return;

        AuthorityOnUnitDespawned?.Invoke(this);
    }

    public override void OnStartServer()
    {
        ServerOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        ServerOnUnitDespawned?.Invoke(this);
    }

    [Server]
    private void HandleServerOnDie()
    {
        NetworkServer.Destroy(gameObject);
    }

}

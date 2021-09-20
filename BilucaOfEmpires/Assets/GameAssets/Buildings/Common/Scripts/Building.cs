using Mirror;
using System;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField] private GameObject preview;
    [SerializeField] private Sprite icon;
    [SerializeField] private int id;
    [SerializeField] private int price;

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    public static event Action<Building> AuthorityOnBuildingSpawned;
    public static event Action<Building> AuthorityOnBuildingDespawned;

    public Sprite Icon => icon;

    public int Id =>  id;

    public int Price => price;

    public GameObject Preview => preview;

    // On Server
    public override void OnStartServer()
    {
        ServerOnBuildingSpawned?.Invoke(this);
    }

    // On Server
    public override void OnStopServer()
    {
        ServerOnBuildingDespawned?.Invoke(this);
    }

    // On Client
    public override void OnStartAuthority()
    {
        AuthorityOnBuildingSpawned?.Invoke(this);
    }

    // On Client
    public override void OnStopClient()
    {
        if (!hasAuthority) { return; }

        AuthorityOnBuildingDespawned?.Invoke(this);
    }
}

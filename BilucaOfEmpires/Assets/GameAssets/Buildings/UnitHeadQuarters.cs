using Mirror;
using System;

public class UnitHeadQuarters : NetworkBehaviour
{
    public static event Action<int> ServerOnPlayerDie;
    public static event Action<UnitHeadQuarters> ServerOnBaseSpawned;
    public static event Action<UnitHeadQuarters> ServerOnBaseDespawned;

    private HealthSystemServer healthSystem;

    private void Awake()
    {
        healthSystem = GetComponent<HealthSystemServer>();
    }

    public override void OnStartServer()
    {
        healthSystem.ServerOnDie += ServerHandleDie;

        ServerOnBaseSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        healthSystem.ServerOnDie -= ServerHandleDie;

        ServerOnBaseDespawned?.Invoke(this);
    }

    [Server]
    private void ServerHandleDie()
    {
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }

}

using Assets.UnityFoundation.UI.ProgressElements.ProgressCircle;
using Mirror;
using TMPro;
using UnityEngine;

public class UnitSpawnerServer : NetworkBehaviour
{
    [SerializeField] private Unit unitPrefab;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private int maxUnitQueue = 5;
    [SerializeField] private float spawnMoveRange = 7f;
    [SerializeField] private float unitSpawnDuration = 5f;

    private ProgressCircle progressCircle;

    private void Awake()
    {
        progressCircle = transform.Find("progress_circle")
            .GetComponent<ProgressCircle>()
            .Setup(unitSpawnDuration);
    }

    [Command]
    public void CmdSpawnUnit()
    {
        if(queuedUnits == maxUnitQueue) 
            return;

        var player = connectionToClient.identity.GetComponent<PlayerClient>();

        if(player.Resources < unitPrefab.ResourcesCost) 
            return;

        queuedUnits++;

        player.IncreaseResources(-unitPrefab.ResourcesCost);
    }

    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;
    [SyncVar]
    private float unitTimer;

    private void Update()
    {
        if(isServer)
        {
            ProduceUnits();
        }

        if(isClient)
        {
            if(queuedUnits == 0) 
                progressCircle.Hide();
            else 
                progressCircle.Display(unitTimer);
        }
    }

    [Server]
    private void ProduceUnits()
    {
        if(queuedUnits == 0) 
            return;

        unitTimer += Time.deltaTime;

        if(unitTimer < unitSpawnDuration) 
            return;

        var unitInstance = Instantiate(
            unitPrefab.gameObject,
            spawnPosition.position,
            spawnPosition.rotation
        );

        NetworkServer.Spawn(unitInstance, connectionToClient);

        var spawnOffset = Random.insideUnitSphere * spawnMoveRange;
        spawnOffset.y = spawnPosition.position.y;

        var unitMovement = unitInstance.GetComponent<UnitMovementServer>();
        unitMovement.Move(spawnPosition.position + spawnOffset);

        queuedUnits--;
        unitTimer = 0f;
    }

    private void ClientHandleQueuedUnitsUpdated(int oldUnits, int newUnits)
    {
        progressCircle.DisplayCounter(newUnits);
    }
}

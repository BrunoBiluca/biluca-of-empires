using Mirror;
using UnityEngine;

public class UnitSpawnerServer : NetworkBehaviour {

    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Transform spawnPosition;

    [Command]
    public void CmdSpawnUnit() {
        var go = Instantiate(
            unitPrefab,
            spawnPosition.position,
            spawnPosition.rotation
        );

        NetworkServer.Spawn(go, connectionToClient);
    }
}

using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerServer : NetworkBehaviour
{
    [SerializeField] private Building[] buildings;

    private readonly List<Unit> serverUnits = new List<Unit>();
    private readonly List<Building> serverBuildings = new List<Building>();

    public List<Unit> Units => serverUnits;

    public List<Building> Buildings => serverBuildings;

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

        Building.ServerOnBuildingSpawned += ServerBuildingSpawnHandler;
        Building.ServerOnBuildingSpawned += ServerBuildingDespawnHandler;
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;

        Building.ServerOnBuildingSpawned += ServerBuildingSpawnHandler;
        Building.ServerOnBuildingSpawned += ServerBuildingDespawnHandler;
    }

    private void ServerHandleUnitSpawned(Unit unit)
    {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        serverUnits.Add(unit);
    }

    private void ServerHandleUnitDespawned(Unit unit)
    {
        if(unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

        serverUnits.Remove(unit);
    }

    private void ServerBuildingDespawnHandler(Building building)
    {
        if(building.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        serverBuildings.Add(building);
    }

    private void ServerBuildingSpawnHandler(Building building)
    {
        if(building.connectionToClient.connectionId != connectionToClient.connectionId)
            return;

        serverBuildings.Remove(building);
    }

    public void TryPaceBuilding(int id, Vector3 point)
    {
        Building buildingToPlace = null;

        foreach(var building in buildings)
        {
            if(building.Id == id)
            {
                buildingToPlace = building;
                break;
            }
        }

        if(buildingToPlace == null) { return; }

        var buildingInstance = Instantiate(
            buildingToPlace.gameObject, 
            point, 
            buildingToPlace.transform.rotation
        );

        NetworkServer.Spawn(buildingInstance, connectionToClient);
    }


}
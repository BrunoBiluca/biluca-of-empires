using Assets.UnityFoundation.UI.Minimap;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerServer : NetworkBehaviour
{
    [SerializeField] private LayerMask buildingBlockLayer;
    [SerializeField] private float buildingRangeLimit = 5f;
    [SerializeField] private Building[] buildings;

    private readonly List<Unit> serverUnits = new List<Unit>();
    private readonly List<Building> serverBuildings = new List<Building>();

    public event Action<int> ServerOnResourcesUpdated;
    public static event Action OnPlayerInfoUpdated;
    public static event Action OnServerPlayerInfoUpdated;

    [SyncVar]
    private int resources = 500;
    public int Resources => resources;

    public List<Unit> Units => serverUnits;

    public List<Building> Buildings => serverBuildings;

    public Color TeamColor { get; set; }

    [SyncVar]
    private bool isPartyOwner;
    public bool IsPartyOwner {
        get { return isPartyOwner; }
        set {
            if(!hasAuthority) return;

            isPartyOwner = value;
            OnPlayerInfoUpdated?.Invoke();
        }
    }

    [SyncVar]
    public string playerName;
    public string PlayerName {
        get => playerName;
        set {
            playerName = value;
            OnPlayerInfoUpdated?.Invoke();
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += (scene, mode) => SetupGameplay();
    }

    public void SetupGameplay()
    {
        if(!SceneManager.GetActiveScene().name.StartsWith("desert"))
            return;

        FindObjectOfType<MinimapController>()
            .TargetTransform = transform.Find("main_virtual_camera");
    }

    public override void OnStartServer()
    {
        Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;

        Building.ServerOnBuildingSpawned += ServerBuildingSpawnHandler;
        Building.ServerOnBuildingSpawned += ServerBuildingDespawnHandler;

        OnPlayerInfoUpdated += TriggerLobbyMenuUpdate;

        SetupGameplay();

        DontDestroyOnLoad(gameObject);
    }

    public override void OnStopServer()
    {
        Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
        Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;

        Building.ServerOnBuildingSpawned += ServerBuildingSpawnHandler;
        Building.ServerOnBuildingSpawned += ServerBuildingDespawnHandler;
    }

    public void PlayerDisconnected()
    {
        OnPlayerInfoUpdated?.Invoke();
    }

    [Server]
    public void TriggerLobbyMenuUpdate()
    {
        RPCLobbyMenuUpdateOnClient();
    }

    [ClientRpc]
    private void RPCLobbyMenuUpdateOnClient()
    {
        OnServerPlayerInfoUpdated?.Invoke();
    }

    [Server]
    public void IncreaseResources(int addResources)
    {
        resources += addResources;
        ServerOnResourcesUpdated?.Invoke(resources);
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

    public void TryPlaceBuilding(int id, Vector3 point)
    {
        var buildingToPlace = SelectBuilding(id);

        if(buildingToPlace == null)
            return;

        if(!CanBuild(buildingToPlace, point))
            return;

        IncreaseResources(-buildingToPlace.Price);

        var buildingInstance = Instantiate(
            buildingToPlace.gameObject,
            point,
            buildingToPlace.transform.rotation
        );

        NetworkServer.Spawn(buildingInstance, connectionToClient);
    }

    private Building SelectBuilding(int id)
    {
        foreach(var building in buildings)
        {
            if(building.Id == id)
            {
                return building;
            }
        }

        return null;
    }

    public bool CanBuild(Building buildingToPlace, Vector3 point)
    {
        var buildingCollider = buildingToPlace.GetComponent<BoxCollider>();
        var isOverlapping = Physics.CheckBox(
            point + buildingCollider.center,
            buildingCollider.size / 2,
            Quaternion.identity,
            buildingBlockLayer
        );

        if(isOverlapping)
            return false;

        foreach(Building building in Buildings)
        {
            var buildingDistance = (point - building.transform.position).sqrMagnitude;
            if(buildingDistance <= buildingRangeLimit * buildingRangeLimit)
                return true;
        }

        return false;

    }

    [Command]
    public void CmdStartGame()
    {
        if(!isPartyOwner) { return; }

        ((GameNetworkManager)NetworkManager.singleton).StartGame();
    }
}
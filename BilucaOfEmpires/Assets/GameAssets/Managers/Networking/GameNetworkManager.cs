using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameNetworkManager : NetworkManager
{
    [SerializeField] private GameObject headQuartersPrefab;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab;

    private bool isGameInProgress = false;

    public List<PlayerServer> Players { get; } = new List<PlayerServer>();

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if(!isGameInProgress) return;

        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        var player = conn.identity.GetComponent<PlayerServer>();
        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Players.Clear();

        isGameInProgress = false;
    }

    public void StartGame()
    {
        //if(Players.Count < 2) return;

        isGameInProgress = true;

        ServerChangeScene("desert_map_scene");
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        var player = conn.identity.GetComponent<PlayerServer>();
        Players.Add(player);

        player.PlayerName = $"Player {Players.Count}";

        player.TeamColor = new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)
        );

        player.IsPartyOwner = Players.Count == 1;

        player.TriggerLobbyMenuUpdate();
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if(!SceneManager.GetActiveScene().name.StartsWith("desert"))
            return;

        var gameOverHandlerInstance = Instantiate(gameOverHandlerPrefab);

        NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

        foreach(var player in Players)
        {
            var baseInstance = Instantiate(
                headQuartersPrefab,
                GetStartPosition().position,
                Quaternion.identity
            );

            player.SetupGameplay();
            NetworkServer.Spawn(baseInstance, player.connectionToClient);
        }

    }
}

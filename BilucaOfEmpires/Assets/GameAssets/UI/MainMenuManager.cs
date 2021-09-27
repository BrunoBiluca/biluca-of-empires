using Assets.UnityFoundation.UI.Menus.MultiplayerLobbyMenu;
using Mirror;
using System.Linq;

public class MainMenuManager : LobbyMenuManager
{
    protected override void OnStart()
    {
        GameNetworkManager.ClientOnConnected += HandleClientConnected;
        GameNetworkManager.ClientOnDisconnected += HandleClientDisconnected;

        PlayerServer.OnPlayerInfoUpdated += PlayerInfoUpdatedHandler;
        PlayerServer.OnServerPlayerInfoUpdated += PlayerInfoUpdatedHandler;
    }

    private void OnDestroy()
    {
        GameNetworkManager.ClientOnConnected -= HandleClientConnected;
        GameNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
    }

    protected override void OnHostLobby()
    {
        base.OnHostLobby();
        NetworkManager.singleton.StartHost();
    }

    protected override void OnJoinLobby(string address)
    {
        NetworkManager.singleton.networkAddress = address;
        NetworkManager.singleton.StartClient();
    }

    protected override void OnLeaveLobby()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }

    protected override void OnStartGame()
    {
        NetworkClient.connection.identity.GetComponent<PlayerServer>().CmdStartGame();
    }

    private void HandleClientConnected()
    {
        landingPanel.Hide();
        enterAddressPanel.Hide();
        lobbyWattingPanel.Show();

        InvokeClientConnected();
    }

    private void HandleClientDisconnected()
    {
        InvokeClientDisconnected();
    }

    private void PlayerInfoUpdatedHandler()
    {
        var playersNames = ((GameNetworkManager)NetworkManager.singleton)
            .Players
            .Select(player => player.PlayerName)
            .ToList();

        var isPartyOwner = false;
        if(NetworkClient.connection != null)
            isPartyOwner = NetworkClient
                .connection
                .identity
                .GetComponent<PlayerServer>()
                .IsPartyOwner;

        InvokePlayerInfoUpdated(playersNames, isPartyOwner);
    }
}

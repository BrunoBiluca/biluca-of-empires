using Mirror;

public class NetworkGameOverMenu : GameOverMenu
{
    protected override void OnAwake()
    {
        Setup("Leave Game", LeaveGame);
    }

    private void LeaveGame()
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

    protected override void OnStart()
    {
        GameOverHandler.ClientOnGameOver += OnClientDied;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= OnClientDied;
    }

    private void OnClientDied(int winnerId)
    {
        Show($"Player {winnerId} won!");
    }
}

using Mirror;
using UnityEngine;

public class ResourceGenerator : NetworkBehaviour
{
    
    [SerializeField] private int resourcesPerInterval = 10;
    [SerializeField] private float interval = 2f;

    private float timer;
    private PlayerClient player;
    private HealthSystemServer health;

    public override void OnStartServer()
    {
        timer = interval;
        player = connectionToClient.identity.GetComponent<PlayerClient>();

        health = GetComponent<HealthSystemServer>();
        health.ServerOnDie += ServerHandleDie;
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            player.IncreaseResources(resourcesPerInterval);

            timer += interval;
        }
    }

    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    private void ServerHandleGameOver()
    {
        enabled = false;
    }
}


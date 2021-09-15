using Mirror;
using System;
using UnityEngine;

public class HealthSystemServer : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth;

    public event Action ServerOnDie;

    private IHealthBar healthBar;

    public void Awake()
    {
        healthBar = transform.Find("health_bar")
            .GetComponent<IHealthBar>();
        healthBar.Setup(maxHealth);       
    }

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        UnitHeadQuarters.ServerOnPlayerDie += ServerHandlePlayerDie;
    }

    public override void OnStopServer()
    {
        UnitHeadQuarters.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    [Server]
    private void ServerHandlePlayerDie(int connectionId)
    {
        if (connectionToClient.connectionId != connectionId) { return; }

        Damage(currentHealth);
    }


    [Server]
    public void Damage(float amount)
    {
        Debug.Log("Damage");

        if(currentHealth == 0) return;

        currentHealth = Mathf.Clamp(currentHealth - (int)amount, 0, maxHealth);

        Debug.Log(currentHealth);

        if(currentHealth > 0) return;

        ServerOnDie?.Invoke();

        Debug.Log("We Died");
    }

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        healthBar.SetCurrentHealth(newHealth);
    }

}
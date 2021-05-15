using Mirror;
using System;
using UnityEngine;

public class HealthSystemServer : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    public event Action ServerOnDie;

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
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
}
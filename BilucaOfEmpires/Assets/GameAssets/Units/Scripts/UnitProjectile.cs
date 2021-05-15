using Assets.UnityFoundation.HealthSystem;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float destroyAfterSeconds = 5f;
    [SerializeField] private float launchForce = 10f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), destroyAfterSeconds);
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(
            other.TryGetComponent(out NetworkIdentity networkIdentity)
            && networkIdentity.connectionToClient == connectionToClient
        )
        { return; }

        if(other.TryGetComponent(out HealthSystemServer health))
        {
            health.Damage(damageAmount);
        }
        DestroySelf();
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }
}


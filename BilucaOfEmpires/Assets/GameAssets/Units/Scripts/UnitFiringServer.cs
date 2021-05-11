using Assets.UnityFoundation.Code;
using Assets.UnityFoundation.TimeUtils;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFiringServer : NetworkBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float fireRange = 5f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float rotationSpeed = 20f;

    private Unit unit;
    private float lastFireTime;

    private void Start()
    {
        unit = GetComponent<Unit>();
    }

    [ServerCallback]
    private void Update()
    {
        var target = unit.TargetHandler.Target;
        if(!CanFireAtTarget()) return;

        Quaternion targetRotation =
            Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (Time.time > (1 / fireRate) + lastFireTime)
        {
            Quaternion projectileRotation = Quaternion.LookRotation(
                target.GetAimPoint().position - projectileSpawnPoint.position
            );

            GameObject projectileInstance = Instantiate(
                projectilePrefab, projectileSpawnPoint.position, projectileRotation);

            NetworkServer.Spawn(projectileInstance, connectionToClient);

            lastFireTime = Time.time;
        }

    }

    [Server]
    private bool CanFireAtTarget()
    {
        var target = unit.TargetHandler.Target;
        if(
            target == null
            || !TransformUtils.IsInRange(target.transform, transform, fireRange)
        )
        {
            return false;
        }

        return true;
    }

}

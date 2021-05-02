using Mirror;
using UnityEngine;

public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform aimPoint;

    public Transform GetAimPoint()
    {
        if(aimPoint == null) 
            return transform;

        return aimPoint;
    }
}

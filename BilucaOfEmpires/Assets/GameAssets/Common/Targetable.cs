using Mirror;
using UnityEngine;

public class Targetable : NetworkBehaviour
{
    [SerializeField] private Transform aimPoint;

    private void Start()
    {
        if(aimPoint == null)
        {
            aimPoint = transform.Find("aim_point");
        }
    }

    public Transform GetAimPoint()
    {
        if(aimPoint == null)
            return transform;

        return aimPoint;
    }
}

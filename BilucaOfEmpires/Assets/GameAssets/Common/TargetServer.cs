using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetServer : NetworkBehaviour
{
    [SerializeField] private Targetable target;

    [Command]
    public void CmdSetTarget(Targetable newTarget)
    {
        target = newTarget;
    }

    [Server]
    public void ClearTarget()
    {
        target = null;
    }
}

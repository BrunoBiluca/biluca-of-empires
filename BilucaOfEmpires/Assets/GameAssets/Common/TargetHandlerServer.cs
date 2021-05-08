using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandlerServer : NetworkBehaviour
{
    [SerializeField] public Targetable Target { get; private set; }

    [Command]
    public void CmdSetTarget(Targetable newTarget)
    {
        Target = newTarget;
    }

    [Server]
    public void ClearTarget()
    {
        Target = null;
    }
}

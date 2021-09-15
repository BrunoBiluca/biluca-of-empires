using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetHandlerServer : NetworkBehaviour
{
    public Targetable Target { get; private set; }

    private void ServerHandleGameOver()
    {
        ClearTarget();
    }

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

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

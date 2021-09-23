using Assets.UnityFoundation.Code.Common;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitController : Singleton<UnitController>
{
    [SerializeField] private LayerMask layerMask;

    private void Start()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void ClientHandleGameOver(int winnerId)
    {
        enabled = false;
    }


    void Update()
    {
        if(!Mouse.current.rightButton.wasPressedThisFrame) return;

        var position = Mouse.current.position.ReadValue();

        if(TryTarget(position)) { return; }

        TryMove(position);
    }

    private bool TryTarget(Vector2 position)
    {
        if(!PhysicsUtils.RaycastType(position, out Targetable target, layerMask))
            return false;

        if(target.hasAuthority) return false;

        UnitSelectorHandler.Instance
            .SelectedUnits
            .ForEach(unit => unit.TargetHandler.CmdSetTarget(target));

        return true;
    }

    private void TryMove(Vector2 position)
    {
        if(!PhysicsUtils.Raycast(position, out RaycastHit hit, layerMask))
            return;

        UnitSelectorHandler.Instance
            .SelectedUnits
            .ForEach(unit => {
                unit.Movement.Move(hit.point);
            });
    }
}

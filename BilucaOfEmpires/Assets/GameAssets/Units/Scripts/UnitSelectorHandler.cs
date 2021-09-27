using Assets.UnityFoundation.Code;
using Assets.UnityFoundation.Code.Common;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectorHandler : Singleton<UnitSelectorHandler>
{

    [SerializeField] private LayerMask layerMask;
    [SerializeField] private RectTransform selectionArea;
    public List<Unit> SelectedUnits { get; } = new List<Unit>();

    private PlayerClient player;
    private Camera mainCamera;
    private Vector2 startPosition;

    private void Start()
    {
        mainCamera = Camera.main;
        selectionArea.gameObject.SetActive(false);

        Unit.AuthorityOnUnitDespawned += HandleAuthorityUnitDespawned;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;

        player = NetworkClient.connection?.identity.GetComponent<PlayerClient>();
    }

    private void OnDestroy()
    {
        Unit.AuthorityOnUnitDespawned -= HandleAuthorityUnitDespawned;
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void HandleAuthorityUnitDespawned(Unit unit)
    {
        SelectedUnits.Remove(unit);
    }

    private void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            if(!Keyboard.current.leftShiftKey.isPressed)
            {
                DeselectUnits();
            }
            StartSelectionBox();
        }

        if(Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionBox();
        }

        if(Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionBox();
        }
    }
    private void DeselectUnits()
    {
        foreach(var previousSelectedUnit in SelectedUnits)
            previousSelectedUnit.Deselect();
        SelectedUnits.Clear();
    }

    private void StartSelectionBox()
    {
        selectionArea.gameObject.SetActive(true);
        startPosition = Mouse.current.position.ReadValue();
        UpdateSelectionBox();
    }

    private void UpdateSelectionBox()
    {
        var mousePosition = Mouse.current.position.ReadValue();

        var area = mousePosition - startPosition;

        selectionArea.sizeDelta = Vector2Utils.Abs(area);
        selectionArea.anchoredPosition = startPosition + Vector2Utils.Center(area);
    }

    private void ClearSelectionBox()
    {
        selectionArea.gameObject.SetActive(false);

        if(selectionArea.sizeDelta.magnitude == 0)
        {
            SelectOneUnit();
            return;
        }
        SelectMultipleUnits();
    }

    private void SelectOneUnit()
    {
        var ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

        if(!hit.collider.TryGetComponent(out Unit unit)) return;

        if(!unit.hasAuthority) return;

        SelectedUnits.Add(unit);

        foreach(var currentSelectedUnit in SelectedUnits)
        {
            currentSelectedUnit.Select();
        }
    }

    private void SelectMultipleUnits()
    {
        Vector2 min = selectionArea.anchoredPosition - (selectionArea.sizeDelta / 2);
        Vector2 max = selectionArea.anchoredPosition + (selectionArea.sizeDelta / 2);

        foreach(var unit in player.Units)
        {
            Vector3 unitOnScreenPosition = mainCamera.WorldToScreenPoint(unit.transform.position);

            if(
                unitOnScreenPosition.x > min.x
                && unitOnScreenPosition.x < max.x
                && unitOnScreenPosition.y > min.y
                && unitOnScreenPosition.y < max.y)
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }
    }

    private void ClientHandleGameOver(int winnerId)
    {
        enabled = false;
    }

}

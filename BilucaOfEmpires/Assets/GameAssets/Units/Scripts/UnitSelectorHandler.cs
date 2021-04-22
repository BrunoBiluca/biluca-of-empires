using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectorHandler : MonoBehaviour {

    [SerializeField] private LayerMask layerMask;

    private Camera mainCamera;
    private readonly List<Unit> selectedUnits = new List<Unit>();

    private void Start() {
        mainCamera = Camera.main;
    }

    private void Update() {
        if(Mouse.current.leftButton.wasPressedThisFrame) {
            foreach(var previousSelectedUnit in selectedUnits) {
                previousSelectedUnit.Deselect();
            }
            selectedUnits.Clear();
        }

        if(Mouse.current.leftButton.wasReleasedThisFrame) {
            ClearSelectionBox();
        }
    }

    private void ClearSelectionBox() {
        var ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if(!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

        if(!hit.collider.TryGetComponent(out Unit unit)) return;

        if(!unit.hasAuthority) return;

        selectedUnits.Add(unit);

        foreach(var currentSelectedUnit in selectedUnits) {
            currentSelectedUnit.Select();
        }
    }

}

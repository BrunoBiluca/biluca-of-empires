using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionMark : MonoBehaviour {

    private SpriteRenderer sprite;

    private void Start() {
        var unit = GetComponentInParent<Unit>();
        sprite = GetComponent<SpriteRenderer>();

        unit.OnUnitSelected += (sender, args) => {
            sprite.enabled = true;
        };

        unit.OnUnitDeselected += (sender, args) => {
            sprite.enabled = false;
        };
    }
}

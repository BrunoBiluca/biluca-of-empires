using UnityEngine;

public class UnitSelectionMark : MonoBehaviour
{
    private void Start()
    {
        var unit = GetComponentInParent<Unit>();

        unit.OnUnitSelected += (sender, args) => {
            gameObject.SetActive(true);
        };

        unit.OnUnitDeselected += (sender, args) => {
            gameObject.SetActive(false);
        };

        gameObject.SetActive(false);
    }
}

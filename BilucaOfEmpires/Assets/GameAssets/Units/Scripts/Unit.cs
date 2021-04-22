using Mirror;
using System;

public class Unit : NetworkBehaviour {

    public EventHandler OnUnitSelected;
    public EventHandler OnUnitDeselected;

    [Client]
    public void Select() {
        if(!hasAuthority) return;

        OnUnitSelected?.Invoke(this, EventArgs.Empty);
    }

    [Client]
    public void Deselect() {
        if(!hasAuthority) return;

        OnUnitDeselected?.Invoke(this, EventArgs.Empty);
    }

}

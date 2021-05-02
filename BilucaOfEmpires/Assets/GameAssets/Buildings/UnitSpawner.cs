using Mirror;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{

    private UnitSpawnerServer server;

    private void Awake()
    {
        server = GetComponent<UnitSpawnerServer>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if(eventData.button != PointerEventData.InputButton.Left) return;

        if(!hasAuthority) return;

        server.CmdSpawnUnit();
    }
}

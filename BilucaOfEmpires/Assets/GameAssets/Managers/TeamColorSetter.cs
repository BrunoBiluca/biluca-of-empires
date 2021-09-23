using Mirror;
using UnityEngine;

public class TeamColorSetter : NetworkBehaviour
{
    [SerializeField] private Renderer[] colorRenderers = new Renderer[0];

    [SyncVar(hook = nameof(HandleTeamColorUpdated))]
    private Color teamColor = new Color();

    public override void OnStartServer()
    {
        var player = connectionToClient.identity.GetComponent<PlayerServer>();

        teamColor = player.TeamColor;
    }

    private void HandleTeamColorUpdated(Color oldColor, Color newColor)
    {
        foreach(Renderer renderer in colorRenderers)
        {
            renderer.material.SetColor("_BaseColor", newColor);
        }
    }

}

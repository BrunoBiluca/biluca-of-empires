using Mirror;
using TMPro;
using UnityEngine;

public class ResourcesUI : MonoBehaviour
{
    private TextMeshProUGUI resourcesText;

    private PlayerClient player;

    private void Awake()
    {
        resourcesText = transform.Find("text").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        player = NetworkClient.connection.identity.GetComponent<PlayerClient>();

        ClientHandleResourcesUpdated(player.Resources);

        player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
    }

    private void OnDestroy()
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int resources)
    {
        resourcesText.text = $"Resources: {resources}";
    }

}

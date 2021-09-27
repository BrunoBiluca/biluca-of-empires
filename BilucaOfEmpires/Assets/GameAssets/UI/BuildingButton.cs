using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building;
    [SerializeField] private LayerMask floorMask = new LayerMask();

    private PlayerClient player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;

    private void Start()
    {
        transform.Find("icon")
            .GetComponent<Image>().sprite = building.Icon;
        transform.Find("text")
            .GetComponent<TextMeshProUGUI>().text = building.Price.ToString();

        player = NetworkClient.connection.identity.GetComponent<PlayerClient>();
    }

    private void Update()
    {
        if(buildingPreviewInstance == null) 
            return;

        UpdateBuildingPreview();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) { return; }
        buildingPreviewInstance = Instantiate(building.Preview);
        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();

        buildingPreviewInstance.SetActive(false);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if(buildingPreviewInstance == null) { return; }

        if(PhysicsUtils.RaycastMousePosition(out RaycastHit hit, floorMask))
        {
            player.CmdTryPlaceBuilding(building.Id, hit.point);
        }

        Destroy(buildingPreviewInstance);
    }

    private void UpdateBuildingPreview()
    {
        if(!PhysicsUtils.RaycastMousePosition(out RaycastHit hit, floorMask))
            return;

        buildingPreviewInstance.transform.position = hit.point;

        if(!buildingPreviewInstance.activeSelf)
            buildingPreviewInstance.SetActive(true);

        var color = player.CanBuild(building, hit.point) ? Color.green : Color.red;

        buildingRendererInstance.material.color = color;

    }
}

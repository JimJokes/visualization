using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Overlays : MonoBehaviour
{

    private BackendSocket backend;

    [Header("Highlighted Vehicle")]
    public GameObject highlightedVehicleUIElement;
    public bool showHighlightedVehicleOverlay = true;
    private List<GameObject> highlightedVehicles = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backend = GameObject.Find("Websocket Data").GetComponent<BackendSocket>();
    }

    void HandleHighlightedVehicles()
    {
        if (backend.world.highlights.vehicles == null)
        {
            return;
        }

        while (highlightedVehicles.Count < backend.world.highlights.vehicles.Length)
        {
            GameObject newHighlightedVehicle = Instantiate(highlightedVehicleUIElement, transform);
            highlightedVehicles.Add(newHighlightedVehicle);
        }
        while (highlightedVehicles.Count > backend.world.highlights.vehicles.Length)
        {
            Destroy(highlightedVehicles[highlightedVehicles.Count - 1]);
            highlightedVehicles.RemoveAt(highlightedVehicles.Count - 1);
        }

        for (int i = 0; i < backend.world.highlights.vehicles.Length; i++)
        {
            HighlightedVehicle highlightedVehicle = highlightedVehicles[i].GetComponent<HighlightedVehicle>();
            highlightedVehicle.target_uid = backend.world.highlights.vehicles[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(showHighlightedVehicleOverlay)
        {
            HandleHighlightedVehicles();
        }
    
    }
}

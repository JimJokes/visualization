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


    [Header("Traffic Lights")]
    public GameObject trafficLightUIElement;
    public bool showTrafficLightOverlay = true;
    private List<GameObject> trafficLightsOverlays = new List<GameObject>();
    private TrafficLightBuilder trafficLightBuilder;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backend = GameObject.Find("Websocket Data").GetComponent<BackendSocket>();
        trafficLightBuilder = GameObject.Find("TrafficLights").GetComponent<TrafficLightBuilder>();
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

    void HandleTrafficLightOverlays()
    {
        if(backend.world.traffic_lights == null)
        {
            return;
        }

        while(trafficLightsOverlays.Count < backend.world.traffic_lights.Length)
        {
            GameObject newTrafficLightOverlay = Instantiate(trafficLightUIElement, transform);
            trafficLightsOverlays.Add(newTrafficLightOverlay);
        }

        while(trafficLightsOverlays.Count > backend.world.traffic_lights.Length)
        {
            Destroy(trafficLightsOverlays[trafficLightsOverlays.Count - 1]);
            trafficLightsOverlays.RemoveAt(trafficLightsOverlays.Count - 1);
        }

        for(int i = 0; i < backend.world.traffic_lights.Length; i++)
        {
            TrafficLightOverlay trafficLightOverlay = trafficLightsOverlays[i].GetComponent<TrafficLightOverlay>();
            trafficLightOverlay.target_index = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(showHighlightedVehicleOverlay)
        {
            HandleHighlightedVehicles();
        }

        if (showTrafficLightOverlay)
        {
            HandleTrafficLightOverlays();
        }
    }
}

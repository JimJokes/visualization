using UnityEngine;
using DG.Tweening;
using System;
using TMPro;
using Unity.VisualScripting;

public class TrafficLightOverlay : MonoBehaviour
{

    public TMP_Text time;
    public TMP_Text to;
    public TMP_Text current;
    public BackendSocket backend;
    public TrafficLightBuilder traffic_light_builder;
    public int target_index;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backend = GameObject.Find("Websocket Data").GetComponent<BackendSocket>();
        traffic_light_builder = GameObject.Find("TrafficLights").GetComponent<TrafficLightBuilder>();
    }

    Color state_text_to_color(string text)
    {
        switch (text)
        {
            case "Red":
                return new Color(255/255, 209/255, 117/255);
            case "Yellow":
                return new Color(255/255, 255/255, 117/255);
            case "Green":
                return new Color(47/255, 255/255, 117/255);
            default:
                return new Color(1, 1, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (backend.world == null)
        {
            return;
        }
        if (backend.world.traffic == null)
        {
            return;
        }

        Vector3 target_position = new Vector3(
            traffic_light_builder.traffic_lights[target_index].transform.position.x, 
            traffic_light_builder.traffic_lights[target_index].transform.position.y, 
            traffic_light_builder.traffic_lights[target_index].transform.position.z
        );

        Vector3 truck_position = new Vector3(
            backend.truck.transform.z - backend.truck.transform.sector_y, 
            backend.truck.transform.y, 
            backend.truck.transform.x - backend.truck.transform.sector_x
        );

        float distance = Vector3.Distance(truck_position, target_position);

        if(distance > 50)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            return;
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }

        string uid = traffic_light_builder.traffic_lights[target_index].gameObject.name.Replace("Traffic Light ", "");
        TrafficLight traffic_light;

        try
        {
            traffic_light = backend.world.traffic_lights[Array.FindIndex(backend.world.traffic_lights, x => x.uid == uid)];
        }
        catch
        {
            transform.GetChild(0).gameObject.SetActive(false);
            return;
        }

        float time_left = MathF.Round(traffic_light.time_left, 1);
        string state_text = traffic_light.state_text;
        int state = traffic_light.state;

        string next_state_text = "";
        switch (state)
        {
            case 0: // OFF
                next_state_text = "Off";
                break;
            case 1: // ORANGE_TO_RED
                next_state_text = "Red";
                break;
            case 2: // RED
                next_state_text = "Yellow";
                break;
            case 4: // ORANGE_TO_GREEN
                next_state_text = "Green";
                break;
            case 8: // GREEN
                next_state_text = "Yellow";
                break;
            case 32: // SLEEP
                next_state_text = "Disabled";
                break;
        }

        Color current_color = state_text_to_color(state_text);
        Color next_color = state_text_to_color(next_state_text);

        string time_text = time_left.ToString();
        if (time_left % 1 == 0)
        {
            time_text += ".0";
        }
        time_text += "s";

        time.text = time_text;
        to.text = "> " + next_state_text.ToUpper();
        to.color = next_color;
        current.text = state_text.ToUpper();
        current.color = current_color;

        transform.DOMove(Camera.main.WorldToScreenPoint(target_position), 0.2f);

        // Scale is 0.8 at 20m distance
        float scale = 0.8f / (distance / 20);
        transform.DOScale(new Vector3(scale, scale, scale), 0.2f);
    }
}

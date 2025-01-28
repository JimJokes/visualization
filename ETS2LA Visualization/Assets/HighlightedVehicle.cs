using UnityEngine;
using TMPro;
using DG.Tweening;

public class HighlightedVehicle : MonoBehaviour
{

    public TMP_Text speed;
    public TMP_Text difference;
    public BackendSocket backend;
    public int target_uid;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backend = GameObject.Find("Websocket Data").GetComponent<BackendSocket>();
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

        if(target_uid == 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            return;
        }

        for (int i = 0; i < backend.world.traffic.Length; i++)
        {
            if (backend.world.traffic[i].id == target_uid)
            {

                Vector3 target_position = new Vector3(backend.world.traffic[i].position.z, backend.world.traffic[i].position.y + backend.world.traffic[i].size.height / 2, backend.world.traffic[i].position.x);
                Vector3 truck_position = new Vector3(backend.truck.transform.z, backend.truck.transform.y, backend.truck.transform.x);
                float distance = Vector3.Distance(truck_position, target_position);

                if(distance > 100)
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                    return;
                }
                else
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                }

                float target_speed = backend.world.traffic[i].speed * 3.6f;
                float target_speed_offset = (backend.world.traffic[i].speed - backend.truck.state.speed) * 3.6f;

                target_speed = (float)System.Math.Round(target_speed, 0);
                target_speed_offset = (float)System.Math.Round(target_speed_offset, 0);

                speed.text = target_speed.ToString() + " km/h";
                if (target_speed_offset > 0)
                {
                    difference.text = "+ " + target_speed_offset.ToString();
                    difference.enabled = true;
                    difference.color = new Color(0, 1, 0);
                }
                else if (target_speed_offset < 0)
                {
                    difference.text = "- " + System.Math.Abs(target_speed_offset).ToString();
                    difference.enabled = true;
                    difference.color = new Color(1, 0, 0);
                }
                else
                {
                    difference.enabled = false;
                    difference.color = new Color(1, 1, 1);
                }

                transform.DOMove(Camera.main.WorldToScreenPoint(target_position), 0.2f);

                // Scale is 1 at 20m distance
                float scale = 1 / (distance / 20);
                transform.DOScale(new Vector3(scale, scale, scale), 0.2f);
            }
        }
    }
}

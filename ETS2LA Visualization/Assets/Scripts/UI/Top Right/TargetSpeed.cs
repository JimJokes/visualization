using UnityEngine;
using TMPro;

public class TargetSpeed : MonoBehaviour
{
    private BackendSocket backend;
    private TMP_Text targetspeed;
    private TMP_Text set;

    void Start()
    {
        targetspeed = GetComponent<TMP_Text>();
        set = transform.GetChild(0).GetComponent<TMP_Text>();
        backend = GameObject.Find("Websocket Data").GetComponent<BackendSocket>();
    }

    // Update is called once per frame
    void Update()
    {
        if(backend.truck == null)
        {
            return;
        }
        if(backend.truck.state == null)
        {
            return;
        }

        targetspeed.text = ((int)(backend.truck.state.target_speed * 3.6f)).ToString();
    }
}

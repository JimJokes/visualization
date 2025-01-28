using UnityEngine;
using TMPro;

public class SpeedLimit : MonoBehaviour
{
    private BackendSocket backend;
    private TMP_Text speedlimit;

    void Start()
    {
        speedlimit = GetComponent<TMP_Text>();
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

        speedlimit.text = ((int)(backend.truck.state.speed_limit * 3.6f)).ToString();
    }
}

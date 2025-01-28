using UnityEngine;
using TMPro;

public class Speed : MonoBehaviour
{
    private BackendSocket backend;
    private TMP_Text speed;

    void Start()
    {
        speed = GetComponent<TMP_Text>();
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

        speed.text = ((int)(backend.truck.state.speed * 3.6f)).ToString();
    }
}

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class Acceleration : MonoBehaviour
{
    private BackendSocket backend;
    private Slider acceleration;

    void Start()
    {
        acceleration = GetComponent<Slider>();
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

        acceleration.DOValue(backend.truck.state.throttle, 0.2f);
    }
}
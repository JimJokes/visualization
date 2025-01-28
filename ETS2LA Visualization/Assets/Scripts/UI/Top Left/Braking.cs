using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Braking : MonoBehaviour
{
    private BackendSocket backend;
    private Slider braking;

    void Start()
    {
        braking = GetComponent<Slider>();
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

        braking.DOValue(backend.truck.state.brake, 0.2f);     
    }
}
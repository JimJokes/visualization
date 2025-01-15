using System.ComponentModel;
using Baracuda.Monitoring;
using DG.Tweening;
using UnityEngine;

public class CameraOffsetHandler : MonitoredBehaviour
{

    [Header("Settings")]
    public float lerp_time = 0.5f;
    public BackendSocket backend;
    [Header("Normal Offsets")]
    public Vector3 offset = new Vector3(0, 0, 0);
    public Vector3 offset_rotation = new Vector3(0, 0, 0);
    [Header("Stopped Offsets")]
    public Vector3 stopped_offset = new Vector3(0, 0, 0);
    public Vector3 stopped_offset_rotation = new Vector3(0, 0, 0);
    [Header("Reverse Offsets")]
    public Vector3 reverse_offset = new Vector3(0, 0, 0);
    public Vector3 reverse_offset_rotation = new Vector3(0, 0, 0);

    [Monitor]
    string state = "normal";

    // Update is called once per frame
    void Update()
    {
        if (backend.truck == null)
        {
            return;
        }
        if (backend.truck.state == null)
        {
            return;
        }

        bool is_stationary = backend.truck.state.speed < 0.5f && backend.truck.state.speed > -0.5f;
        bool is_reversing = backend.truck.state.speed < -0.5f;

        if(is_stationary)
        {
            state = "stationary";
            transform.DOLocalMove(stopped_offset, lerp_time);
            transform.DOLocalRotate(stopped_offset_rotation, lerp_time);
        }
        else if (is_reversing)
        {
            state = "reversing";
            transform.DOLocalMove(reverse_offset, lerp_time);
            transform.DOLocalRotate(reverse_offset_rotation, lerp_time);
        }
        else
        {
            state = "normal";
            transform.DOLocalMove(offset, lerp_time);
            transform.DOLocalRotate(offset_rotation, lerp_time);
        }
    }
}

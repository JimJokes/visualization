using Baracuda.Monitoring;
using DG.Tweening;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class CameraHandler : MonitoredBehaviour
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
    [Header("FOV")]
    public float at_0_speed = 55;
    public float at_80_speed = 75;

    [Monitor]
    string state = "normal";
    [Monitor]
    Vector3 additional_rotation_offset = new Vector3(0, 0, 0);
    GameObject truck;

    void Start()
    {
        truck = GameObject.Find("Truck");
    }

    Vector3 GetAverageSteeringPoint(){
        Vector3[] steering = backend.truck.steering;
        Vector3 average = new Vector3(0, 0, 0);
        for (int i = 0; i < steering.Length; i++)
        {
            average += steering[i];
        }
        return average /= steering.Length;
    }

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

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
        if (backend.truck.steering == null)
        {
            return;
        }

        Vector3 additional_offset = new Vector3(0, 0, 0);
        Vector3 average_point = GetAverageSteeringPoint();
        
        // Add the offset to point towards the average steering point
        Vector3 temp_additional_rotation_offset = new Vector3(0, Quaternion.LookRotation((average_point - transform.parent.position).normalized, Vector3.up).eulerAngles.y - truck.transform.rotation.eulerAngles.y, 0) / 2;

        if(temp_additional_rotation_offset.y > 180)
        {
            temp_additional_rotation_offset.y -= 360;
        }
        else if(temp_additional_rotation_offset.y < -180)
        {
            temp_additional_rotation_offset.y += 360;
        }

        if (temp_additional_rotation_offset.y > 90)
        {
            temp_additional_rotation_offset.y -= 180;
        }
        else if (temp_additional_rotation_offset.y < -90)
        {
            temp_additional_rotation_offset.y += 180;
        }

        if (!float.IsNaN(Vector3.Distance(average_point, transform.parent.position)))
        {
            additional_rotation_offset = Vector3.Lerp(additional_rotation_offset, temp_additional_rotation_offset, Time.deltaTime * 0.01f * Vector3.Distance(average_point, transform.parent.position));
        }

        bool is_stationary = backend.truck.state.speed < 0.5f && backend.truck.state.speed > -0.5f;
        bool is_reversing = backend.truck.state.speed < -0.5f;

        if(is_stationary)
        {
            state = "stationary";
            // Rotate the stopped offset to match the new added rotation
            additional_offset += RotatePointAroundPivot(stopped_offset, Vector3.zero, additional_rotation_offset) - stopped_offset;

            transform.DOLocalMove(stopped_offset + additional_offset, lerp_time);
            transform.DOLocalRotate(stopped_offset_rotation + additional_rotation_offset, lerp_time);
        }
        else if (is_reversing)
        {
            state = "reversing";
            // Rotate the stopped offset to match the new added rotation
            additional_offset += RotatePointAroundPivot(reverse_offset, Vector3.zero, additional_rotation_offset) - reverse_offset;

            transform.DOLocalMove(reverse_offset + additional_offset, lerp_time);
            transform.DOLocalRotate(reverse_offset_rotation + additional_rotation_offset, lerp_time);
        }
        else
        {
            state = "normal";
            // Rotate the stopped offset to match the new added rotation
            additional_offset += RotatePointAroundPivot(offset, Vector3.zero, additional_rotation_offset) - offset;
            
            transform.DOLocalMove(offset + additional_offset, lerp_time);
            transform.DOLocalRotate(offset_rotation + additional_rotation_offset, lerp_time);
        }

        float speed = backend.truck.state.speed * 3.6f;
        Camera.main.fieldOfView = Mathf.Lerp(at_0_speed, at_80_speed, speed / 80);
    }
}

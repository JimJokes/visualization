using UnityEngine;

[System.Serializable]
public class VehiclePosition
{
    public float x;
    public float y;
    public float z;
}

[System.Serializable]
public class VehicleQuaternion
{
    public float x;
    public float y;
    public float z;
    public float w;
    public float yaw;
    public float pitch;
    public float roll;
}

[System.Serializable]
public class VehicleSize
{
    public float width;
    public float height;
    public float length;
}

[System.Serializable]
public class TrailerClass
{
    public VehiclePosition position;
    public VehicleQuaternion rotation;
    public VehicleSize size;
}

[System.Serializable]
public class VehicleClass
{
    public VehiclePosition position;
    public VehicleQuaternion rotation;
    public VehicleSize size;
    public float speed;
    public float acceleration;
    public int trailer_count;
    public int id;
    public TrailerClass[] trailers;
}
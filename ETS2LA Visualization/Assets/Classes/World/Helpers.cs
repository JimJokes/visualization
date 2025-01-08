using UnityEngine;

[System.Serializable]
public class Point
{
    public float x;
    public float y;
    public float z;

    public Vector3 ToVector3()
    {
        return new Vector3(z, y, x);
    }
}

[System.Serializable]
public class BoundingBox
{
    public float min_x;
    public float min_y;
    public float max_x;
    public float max_y;
}
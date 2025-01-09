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

[System.Serializable]
public enum Side
{
    LEFT,
    RIGHT
}

[System.Serializable]
public class Node
{
    public string uid;
    public float x;
    public float y;
    public float z;
    public float rotation;
    public Quaternion rotationQuat;
    public string forward_item_uid;
    public string backward_item_uid;
    public int sector_x;
    public int sector_y;
    public string forward_country_id;
    public string backward_country_id;

    public Vector3 PositionTuple()
    {
        return new Vector3(x, z, y);
    }
}
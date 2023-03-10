using UnityEngine;

public class Coordinates
{
    private float _x;
    private float _y;

    public Coordinates(float x, float y)
    {
        _x = x;
        _y = y;
    }

    public static void Draw(Transform parent, Coordinates originPosition, Coordinates endPosition)
    {
        GameObject line = new GameObject();
        LineRenderer lineRenderer = line.AddComponent<LineRenderer>();

        lineRenderer.transform.parent = parent.transform;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.white;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        lineRenderer.SetPosition(0, new Vector3(originPosition._x, originPosition._y, 0));
        lineRenderer.SetPosition(1, new Vector3(endPosition._x, endPosition._y, 0));
    }
}
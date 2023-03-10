using UnityEngine;

public class DrawGrid : MonoBehaviour
{
    private int _xMax = 17;
    private int _yMax = 20;
    private int _size = 1;

    private void Start()
    {
        var mainCamera = Camera.main;
        var cameraHeight = 2f * mainCamera.orthographicSize;
        var cameraWidth = cameraHeight * mainCamera.aspect;
        _xMax = (int)cameraWidth / 2;
        _yMax = (int)cameraHeight / 2;

        Draw();
    }

    private void Draw()
    {
        for (int x = -_xMax; x <= _xMax; x += _size)
        {
            Coordinates.Draw(gameObject.transform, new Coordinates(x, -_yMax),
                new Coordinates(x, _yMax));
        }

        for (int y = -_yMax; y <= _yMax; y += _size)
        {
            Coordinates.Draw(gameObject.transform, new Coordinates(-_xMax, y), new Coordinates(_xMax, y));
        }
    }
}
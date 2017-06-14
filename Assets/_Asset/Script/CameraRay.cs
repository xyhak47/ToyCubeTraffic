using UnityEngine;
using System.Collections;
using Enums;
using System.Collections.Generic;

public class CameraRay : MonoBehaviour
{
    private AI_TYPE type;

    private float MaxX = 640.0f;
    private float MaxY = 480.0f;

    private bool OpenRandomCubesTest = false;

    private void RayPointTo()
    {
        Vector3 p = new Vector3();
        Camera c = Camera.main;
        Vector2 mousePos = new Vector2();

        mousePos.x = Input.mousePosition.x;
        mousePos.y = c.pixelHeight - Input.mousePosition.y;

        float offset = GetLayerOffset(type);

        p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, offset));

        p.y = offset;
        p.x *= -1;

        StationContorller.Instance.BuildNewStation(type, p);
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            RayPointTo();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            type = AI_TYPE.TRAIN;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            type = AI_TYPE.CAR;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            type = AI_TYPE.AIRPLANE;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            type = AI_TYPE.SHIP;
        }
    }

    private const float Offset_Y = -700.0f;
    private const float LayerHeight = 20.0f;
    public float GetLayerOffset(AI_TYPE InType)
    {
        if (InType == AI_TYPE.AIRPLANE) return LayerHeight * 4 + Offset_Y;
        else if (InType == AI_TYPE.CAR) return LayerHeight * 3 + Offset_Y;
        else if (InType == AI_TYPE.TRAIN) return LayerHeight * 2 + Offset_Y;
        else if (InType == AI_TYPE.SHIP) return LayerHeight + Offset_Y;
        else return Offset_Y;
    }

    //void OnGUI()
    //{
    //    Vector3 p = new Vector3();
    //    Camera c = Camera.main;
    //    Event e = Event.current;
    //    Vector2 mousePos = new Vector2();

    //    // Get the mouse position from Event.
    //    // Note that the y position from Event is inverted.
    //    mousePos.x = e.mousePosition.x;
    //    mousePos.y = c.pixelHeight - e.mousePosition.y;

    //    p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, c.nearClipPlane));

    //    GUILayout.BeginArea(new Rect(20, 20, 250, 120));
    //    GUILayout.Label("Screen pixels: " + c.pixelWidth + ":" + c.pixelHeight);
    //    GUILayout.Label("Mouse position: " + mousePos);
    //    GUILayout.Label("World position: " + p.ToString("F3"));
    //    GUILayout.EndArea();
    //}
}

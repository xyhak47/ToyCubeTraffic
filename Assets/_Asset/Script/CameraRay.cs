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

        p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, -700));

        p.y = -700;
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

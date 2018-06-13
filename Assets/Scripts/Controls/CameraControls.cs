using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles controlling the camera
/// </summary>
public class CameraControls : MonoBehaviour {

    Transform swivel, stick;

    //Zoom parameters
    public static float zoom = 1f;
    public float zoomMax, zoomMin;
    public float minZoomAngle, maxZoomAngle;
    public float moveSpeedMinZoom, moveSpeedMaxZoom;
    public static bool fixedCamera;

    //Rotation parameters
    public float rotationSpeed;

    /// <summary>
    /// Initializes camera.
    /// </summary>
    void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
        fixedCamera = false;
        CameraOver();
    }

    public void CameraOver()
    {
        transform.localPosition = new Vector3(PropertiesKeeper.mapWidth * HexMetrics.innerRadius, 0, PropertiesKeeper.mapHeight * HexMetrics.outerRadius / 1.5f);
        Transform swivel = transform.GetChild(0);
        swivel.rotation = Quaternion.Euler(90, 0, 0);
        Transform stick = swivel.transform.GetChild(0);
        stick.localPosition = new Vector3(0, 0, -250);
        zoom = 0f;
    }
    /// <summary>
    /// Checking for input.
    /// </summary>
    void Update()
    {
        if (fixedCamera) return;
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if(zoomDelta != 0)
        {
            AdjustZoom(zoomDelta);
        }

        float deltaX = Input.GetAxis("Horizontal");
        float deltaZ = Input.GetAxis("Vertical");
        if(deltaX != 0 || deltaZ != 0)
        {
            AdjustPosition(deltaX, deltaZ);
        }

        float deltaTurn = Input.GetAxis("Rotation");
        if(deltaTurn != 0)
        {
            AdjustRotation(deltaTurn);
        }        
    }

    public static void fixCamera(bool fix)
    {
        fixedCamera = fix;
    }

    /// <summary>
    /// This method asjusts camera zoom by said delta
    /// </summary>
    void AdjustZoom(float delta)
    {
        //Making sure zoom stays within 0-1 range
        zoom = Mathf.Clamp01(zoom + delta);

        //Linearly interpolating camera's Z coordinate
        float distance = Mathf.Lerp(zoomMax, zoomMin, zoom);
        stick.localPosition = new Vector3(0, 0, distance);

        //Changing camera's angle from sideview to top down
        float angle = Mathf.Lerp(maxZoomAngle, minZoomAngle, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    /// <summary>
    /// This method adjusts camere position by said deltaX and deltaZ
    /// </summary>
    void AdjustPosition(float deltaX, float deltaZ)
    {
        //Direction of moving camera is based on where it's looking multiplied with delta
        //Vector with deltaX and Z is normalized to prevent double speed when moving camera diagonally
        Vector3 direction = transform.localRotation * new Vector3(deltaX, 0, deltaZ).normalized;

        //This allows correct smooth moving of camera and basicaly ensures that we use 
        //largest delta co calculate distance for camera to travel
        float damping = Mathf.Max(Mathf.Abs(deltaX), Mathf.Abs(deltaZ));

        //Distance for camera to travel is based on it's damping, zoom modificator and time delta 
        //(ensures it's framerate independent)
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;

        //Applying new camera position
        Vector3 position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = ClampPosition(position);
    }

    /// <summary>
    /// This method is used to make sure that camera doesn't go out of bounds
    /// </summary>
    Vector3 ClampPosition(Vector3 position)
    {       
        float maxX = (PropertiesKeeper.mapWidth - 0.5f) * (HexMetrics.innerRadius * 2f);
        position.x = Mathf.Clamp(position.x, 0f, maxX);

        float maxZ = (PropertiesKeeper.mapHeight - 1f) * (HexMetrics.outerRadius * 1.5f);
        position.z = Mathf.Clamp(position.z, 0f, maxZ);

        return position;
    }

    /// <summary>
    /// This method is used to rotate the camera by said delta
    /// </summary>
    /// <param name="delta"></param>
    void AdjustRotation(float delta)
    {
        float rotation = transform.localRotation.eulerAngles.y + (delta * rotationSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(0f, rotation, 0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour {

    Transform swivel, stick;

    float zoom = 1f;
    public float zoomMax, zoomMin;
    public float minZoomAngle, maxZoomAngle;

    public float moveSpeedMinZoom, moveSpeedMaxZoom;

    public float rotationSpeed;
    float rotationAngle;

    void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }

    void Update()
    {
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

    void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);

        float distance = Mathf.Lerp(zoomMax, zoomMin, zoom);
        stick.localPosition = new Vector3(0, 0, distance);

        float angle = Mathf.Lerp(maxZoomAngle, minZoomAngle, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    void AdjustPosition(float deltaX, float deltaZ)
    {
        Vector3 direction = transform.localRotation * new Vector3(deltaX, 0, deltaZ).normalized;
        float damping = Mathf.Max(Mathf.Abs(deltaX), Mathf.Abs(deltaZ));
        float distance = Mathf.Lerp(moveSpeedMinZoom, moveSpeedMaxZoom, zoom) * damping * Time.deltaTime;

        Vector3 position = transform.localPosition;
        position += direction * distance;
        transform.localPosition = ClampPosition(position);
    }

    Vector3 ClampPosition(Vector3 position)
    {
        GameMaster master = GetComponentInParent<GameMaster>();

        float maxX = (master.mapWidth - 0.5f) * (HexMetrics.innerRadius * 2f);
        position.x = Mathf.Clamp(position.x, 0f, maxX);

        float maxZ = (master.mapHeight - 1f) * (HexMetrics.outerRadius * 1.5f);
        position.z = Mathf.Clamp(position.z, 0f, maxZ);

        return position;
    }

    void AdjustRotation(float delta)
    {
        rotationAngle += delta * rotationSpeed * Time.deltaTime;
        if (rotationAngle < 0f)
        {
            rotationAngle += 360f;
        }
        else if (rotationAngle >= 360f)
        {
            rotationAngle -= 360f;
        }
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);
    }
}

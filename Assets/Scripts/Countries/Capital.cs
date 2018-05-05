using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to work with planes
/// </summary>
public class Capital : MonoBehaviour {

    public void ValidatePosition()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, new Vector3(0f, -1f, 0f), out hit);

        Vector3 position = hit.point;
        position.y += 0.5f;

        this.transform.position = position;
        this.transform.localRotation = Quaternion.Euler(hit.normal);
    }
}

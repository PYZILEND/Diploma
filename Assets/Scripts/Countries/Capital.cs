using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Country's capital city
/// </summary>
public class Capital : MonoBehaviour {

    /// <summary>
    /// How much planes can a capital house
    /// </summary>
    public int remainingCapacity;

    /// <summary>
    /// Used to reference planes
    /// </summary>
    public Plane[] planes;

    public bool hasPlanes
    {
        get
        {
            for(int i = 0; i<planes.Length; i++)
            {
                if (planes[i])
                {
                    return true;
                }
            }
            return false;
        }
    }

    public int countPlanes
    {
        get
        {
            return planes.Length - remainingCapacity;
        }
    }

    /// <summary>
    /// Used to initialize capital
    /// </summary>
    /// <param name="allegiance"></param>
    public void Initialize(Allegiance allegiance)
    {
        remainingCapacity = 3;        
        planes = new Plane[3];

        ValidatePosition();
        GetComponentInChildren<MeshRenderer>().material.color = AllegianceExtentions.AllegianceToColor(allegiance);
    }

    /// <summary>
    /// Validates capital's position on terrain
    /// </summary>
    public void ValidatePosition()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, new Vector3(0f, -1f, 0f), out hit);

        Vector3 position = hit.point;
        position.y += 0.5f;

        transform.position = position;
        transform.localRotation = Quaternion.Euler(hit.normal);
    }

    public Vector3 GUIPosition()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, new Vector3(0f, -1f, 0f), out hit);

        Vector3 position = hit.point;
        position.y += 0.5f;

        //transform.position = position;
        // transform.localRotation = Quaternion.Euler(hit.normal);
        return position;
    }


    /// <summary>
    /// When capital is captured, this must be called
    /// to destroy it's planes
    /// </summary>
    public void DestroyPlanes()
    {
        for (int i = 0; i < planes.Length; i++)
        {
            if (planes[i])
            {
                planes[i].DestroyVisually();
            }
        }
        
        remainingCapacity = 3;
        planes = new Plane[3];
    }

    /// <summary>
    /// Method for taking plane aboard
    /// </summary>
    /// <param name="unit"></param>
    public void TakeAboard(Plane plane)
    {
        remainingCapacity--;
        planes[FindFreeSpace()] = plane;
    }

    /// <summary>
    /// Method for removing plane from carrier
    /// </summary>
    /// <param name="unit"></param>
    public void RemoveFromCapital(Plane plane)
    {
        remainingCapacity++;
        planes[FindPlaneIndex(plane)] = null;
    }

    /// <summary>
    /// Finds index of first free space
    /// returns -1 if no free space left
    /// </summary>
    /// <returns></returns>
    public int FindFreeSpace()
    {
        int i = 0;
        while (planes[i] != null)
        {
            i++;
            if (i > planes.Length)
            {
                return -1;
            }
        }
        return i;
    }

    /// <summary>
    /// Finds index of a said plane on carrier
    /// returns -1 if no such plane is on carrier
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public int FindPlaneIndex(Plane plane)
    {
        int i = 0;
        while (planes[i] != plane)
        {
            i++;
            if (i > planes.Length)
            {
                return -1;
            }
        }
        return i;
    }

    public void ChangeAllegiance(Color color)
    {
        GetComponentInChildren<MeshRenderer>().material.color = color;
    }
}

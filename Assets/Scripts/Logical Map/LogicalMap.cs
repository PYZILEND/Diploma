using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogicalMap : MonoBehaviour {

    public LogicalMapCell cellPrefab;
    LogicalMapCell[] cells;

    public void CreateMap(int width, int height)
    {
        cells = new LogicalMapCell[width * height];

        int i = 0;
        for(int x = 0; x < height; x++)
        {
            for(int z = 0; z < width; z++)
            {
                CreateCell(x, z, i);
                i++;
            }
        }
    }

    void CreateCell(int x, int z, int i)
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (2f * HexMetrics.innerRadius);
        position.y = 0f;
        position.z = z * (1.5f * HexMetrics.outerRadius);

        LogicalMapCell cell = cells[i] = Instantiate(cellPrefab);
        cell.transform.SetParent(this.transform, false);
        cell.transform.localPosition = position;        
    }
}

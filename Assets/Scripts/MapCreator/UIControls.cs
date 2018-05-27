using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIControls : MonoBehaviour {

    bool panelOut=false;

    public static int newMapWidth ;
    public static int newMapHeight ;
    float newSeaHeight = 2;

    public static int choosenMode = -1;
    GameObject panel;
    Vector3 pos;

    public Text widthTextValue;
    public Text heightTextValue;

    public GameObject camera;

    public MapCreator mapCreator;

    LogicalMapCell selectedCell;

    string mapName;
    
    void Awake()
    {
        panel = GameObject.Find("ButtonPanel");
        pos = panel.transform.position;
        newMapWidth = MapCreator.mapWidth;
        newMapHeight = MapCreator.mapHeight;
        widthTextValue.text = newMapWidth.ToString();
        heightTextValue.text = newMapHeight.ToString();
        selectedCell = null;
    }

    public void ClickButton()
    {
        if (!panelOut)
        {
            pos.x = 123f;
            panel.transform.position = pos;
            panelOut = true;
        }
    }
    
    public void ClosePanel()
    {
        if (panelOut)
        {
            pos.x = 0f;
            panel.transform.position = pos;
            panelOut = false;
            choosenMode = -1;
            MapCreator.newMap.HideAllHighlights();
        }
    }

    public void StartButton(bool value)
    {
        if (value)
        {
            pos.x = 123f;
            panel.transform.position = pos;
            choosenMode = 0;
            MapCreator.newMap.HighlightTerrain();
            CameraOver();
        }
    }

    public void HexMapButton(bool value)
    {
        if (value)
        {
            pos.x = 123f;
            panel.transform.position = pos;
            choosenMode = 1;
            MapCreator.newMap.HighlightTerrain();
        }
    }

    public void CountryButton(bool value)
    {
        if (value)
        {
            pos.x = 248f;
            panel.transform.position = pos;
            choosenMode = 2;
            MapCreator.newMap.HighlightAllegiance();
        }
    }

    public void EndButton(bool value)
    {
        if (value)
        {
            pos.x = 123f;
            panel.transform.position = pos;
            choosenMode = 3;
            MapCreator.newMap.HighlightAllegiance();
        }
    }
    public void CameraOver()
    {
        camera.transform.localPosition = new Vector3(MapCreator.mapWidth * HexMetrics.innerRadius, 0, MapCreator.mapHeight * HexMetrics.outerRadius / 1.5f);           
        Transform swivel = camera.transform.GetChild(0);
        swivel.rotation = Quaternion.Euler(90, 0, 0);
        Transform stick = swivel.transform.GetChild(0);
        stick.localPosition = new Vector3(0, 0, -250);
        CameraControls.zoom = 0f;
    }

    public void GetMapWidth(float value)
    {
        newMapWidth = (int)value;
        widthTextValue.text = newMapWidth.ToString();
    }

    public void GetMapHeight(float value)
    {
        newMapHeight = (int)value;
        heightTextValue.text = newMapHeight.ToString();
    }

    public void GetSeaHeight(float value)
    {
        newSeaHeight = value;
    }

    public void GetMapName(string value)
    {
        mapName = value;
    }

    public void ConfirmButtonClick()
    {
        if (MapCreator.mapHeight != newMapHeight|| MapCreator.mapWidth != newMapWidth)
        {
            MapCreator.mapHeight = newMapHeight;
            MapCreator.mapWidth = newMapWidth;
            mapCreator.RedoMap();
            CameraOver();
        }
    }

    public void SaveButtonClick()
    {
        MapSaver.SaveMapCreator(mapName);
        mapCreator.SaveWater(mapName);
        mapCreator.SaveMap(mapName);
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButton(0))
            {
                int ind = GetHexInd();
                if (ind != -1)
                {
                    switch (choosenMode)
                    {
                        case 1:
                            if (ind != -1 && MapCreator.newMap.cells[ind].terrain != mapCreator.currentType)
                            {
                                MapCreator.newMap.cells[ind].terrain = mapCreator.currentType;
                                mapCreator.hexMap.RedoMapMesh(MapCreator.mapHeight, MapCreator.mapWidth);
                                MapCreator.newMap.cells[ind].ValidateUIPosition();
                                MapCreator.newMap.cells[ind].HighlightValidatedTerrain();
                            }
                            break;
                        case 2:
                            if (ind != -1 && selectedCell!= MapCreator.newMap.cells[ind])
                            {
                                if (selectedCell != null)
                                {
                                    selectedCell.ValidateHighlightWithAllegiance();
                                }
                                selectedCell = MapCreator.newMap.cells[ind];
                                MapEditor.selectedCell = selectedCell;
                                selectedCell.EnableHighlight(Color.black);
                            }
                            break;
                        default: break;
                    }
                }
            }
        }
    }


    Vector3 GetHexCoords()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            HexCoordinates coordinates = HexCoordinates.fromPosition(hit.point);
            return new Vector3(coordinates.X, coordinates.Y, coordinates.Z);
        }
        return new Vector3(-1, -1, -1);
    }

    public int GetHexInd()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            int index = HexCoordinates.ToIndex(hit.point, MapCreator.mapWidth);
            return index;
        }
        return -1;
    }
}

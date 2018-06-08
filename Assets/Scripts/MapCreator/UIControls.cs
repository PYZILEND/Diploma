using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIControls : MonoBehaviour {

    bool panelOut=true;

    public static int newMapWidth ;
    public static int newMapHeight ;
    float newSeaHeight = 2;

    public static int choosenMode = -1;
    GameObject panel;
    Vector3 pos;

    public Text widthTextValue;
    public Text heightTextValue;
    public Text brushTextValue;
    public Text warningTextValue;

    public GameObject newcamera;

    public MapCreator mapCreator;

    LogicalMapCell selectedCell;

    int brushSize = 1;

    string mapName;
    
    void Awake()
    {
        panel = GameObject.Find("ButtonPanel");
        pos = panel.transform.position;
        StartButton(true);
        newMapWidth = PropertiesKeeper.mapWidth;
        newMapHeight = PropertiesKeeper.mapHeight;
        widthTextValue.text = newMapWidth.ToString();
        heightTextValue.text = newMapHeight.ToString();
        SetBrushValue();
        selectedCell = null;
        Toggle button = GameObject.Find("StartButton").GetComponent<Toggle>();
        button.isOn = !button.isOn;
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
             PropertiesKeeper.logicalMap.HideAllHighlights();
        }
    }

    public void StartButton(bool value)
    {
        if (value)
        {
            pos.x = 123f;
            panel.transform.position = pos;
            choosenMode = 0;
             PropertiesKeeper.logicalMap.HighlightTerrain();
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
             PropertiesKeeper.logicalMap.HighlightTerrain();
        }
    }

    public void CountryButton(bool value)
    {
        if (value)
        {
            pos.x = 248f;
            panel.transform.position = pos;
            choosenMode = 2;
             PropertiesKeeper.logicalMap.HighlightAllegiance();
        }
    }

    public void EndButton(bool value)
    {
        if (value)
        {
            pos.x = 123f;
            panel.transform.position = pos;
            choosenMode = 3;
             PropertiesKeeper.logicalMap.HighlightAllegiance();
        }
    }
    public void CameraOver()
    {
        newcamera.transform.localPosition = new Vector3(PropertiesKeeper.mapWidth * HexMetrics.innerRadius, 0, PropertiesKeeper.mapHeight * HexMetrics.outerRadius / 1.5f);           
        Transform swivel = newcamera.transform.GetChild(0);
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

    public void GetBrushSize(float value)
    {
        brushSize = (int)value;
        SetBrushValue();
    }

    void SetBrushValue()
    {
        brushTextValue.text = (brushSize + 1).ToString();
    }

    public void ConfirmButtonClick()
    {
        if (PropertiesKeeper.mapHeight != newMapHeight|| PropertiesKeeper.mapWidth != newMapWidth)
        {
            PropertiesKeeper.mapHeight = newMapHeight;
            PropertiesKeeper.mapWidth = newMapWidth;
            mapCreator.RedoMap();
            CameraOver();
        }
    }

    public void SaveButtonClick()
    {
        if (mapName==null || mapName.Equals(""))
        {
            PropertiesKeeper.popUp.transform.GetChild(1).GetComponent<Text>().text = "Warning: no map name";
            PropertiesKeeper.popUp.SetActive(true);
            CameraControls.fixCamera(true);
           // warningTextValue.text = "Warning: map name wasn't inputed";
            return;
        }
        if (PropertiesKeeper.countries.Count == 0)
        {
            PropertiesKeeper.popUp.transform.GetChild(1).GetComponent<Text>().text = "Warning: no countries created";
            PropertiesKeeper.popUp.SetActive(true);
            CameraControls.fixCamera(true);
            //warningTextValue.text = "Warning: no countries created";
            return;
        }
        MapSaver.SaveMapCreator(mapName,mapName);
        //mapCreator.SaveWater(mapName);
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
                            if (brushSize == 0 )
                            {
                                if ( PropertiesKeeper.logicalMap.cells[ind]!=null&& PropertiesKeeper.logicalMap.cells[ind].terrain != mapCreator.currentType)
                                {
                                     PropertiesKeeper.logicalMap.cells[ind].terrain = mapCreator.currentType;
                                     mapCreator.hexMap.RedoMapMesh(PropertiesKeeper.mapHeight, PropertiesKeeper.mapWidth);
                                     PropertiesKeeper.logicalMap.cells[ind].ValidateUIPosition();
                                     PropertiesKeeper.logicalMap.cells[ind].HighlightValidatedTerrain();
                                }
                            }
                            else
                            {
                                EditCells( PropertiesKeeper.logicalMap.cells[ind]);
                            }

                            break;
                        case 2:
                            if (ind != -1 && selectedCell!=  PropertiesKeeper.logicalMap.cells[ind])
                            {
                                if (selectedCell != null)
                                {
                                    selectedCell.ValidateHighlightWithAllegiance();
                                }
                                selectedCell =  PropertiesKeeper.logicalMap.cells[ind];
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

    void EditCells(LogicalMapCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;
        List<int> changedCells = new List<int>();

        for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + brushSize; x++)
            {                
                int ind = HexCoordinates.ToIndex(new HexCoordinates(x, z));
                if (ind!=-1 &&  PropertiesKeeper.logicalMap.cells[ind]!=null &&  PropertiesKeeper.logicalMap.cells[ind].terrain != mapCreator.currentType)
                {
                     PropertiesKeeper.logicalMap.cells[ind].terrain = mapCreator.currentType;
                    changedCells.Add(ind);
                }
            }
        }

        for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - brushSize; x <= centerX + r; x++)
            {
                int ind = HexCoordinates.ToIndex(new HexCoordinates(x, z));
                if (ind != -1 &&  PropertiesKeeper.logicalMap.cells[ind] != null &&  PropertiesKeeper.logicalMap.cells[ind].terrain != mapCreator.currentType)
                {
                     PropertiesKeeper.logicalMap.cells[ind].terrain = mapCreator.currentType;
                    changedCells.Add(ind);
                }
            }
        }
        if (changedCells.Count > 0)
        {
            mapCreator.hexMap.RedoMapMesh(PropertiesKeeper.mapHeight, PropertiesKeeper.mapWidth);
        }
        for (int i = 0; i < changedCells.Count; i++)
        {
             PropertiesKeeper.logicalMap.cells[changedCells[i]].ValidateUIPosition();
             PropertiesKeeper.logicalMap.cells[changedCells[i]].HighlightValidatedTerrain();
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
            int index = HexCoordinates.ToIndex(hit.point);
            return index;
        }
        return -1;
    }

    public void RedoBorders()
    {
        mapCreator.hexMap.CreateBorders(PropertiesKeeper.mapHeight, PropertiesKeeper.mapWidth);
    }

    public void ClosePopUp()
    {
        PropertiesKeeper.popUp.SetActive(false);
        CameraControls.fixCamera(false);
    }

    public void ResetCountries()
    {
        foreach (Country country in PropertiesKeeper.countries.ToArray())
        {
            country.DeleteCountry();
        }
        PropertiesKeeper.logicalMap.HighlightAllegiance();
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuControls : MonoBehaviour {

    int mapIndex=0;
    bool gameMode=true;
    public Dropdown list;
    List<string> mapNames;
    int alliesCount = 1;

    public void StartButtonClick()
    {
        PropertiesKeeper.mapName = mapNames[mapIndex];
        PropertiesKeeper.defaultGameMode = gameMode;
        PropertiesKeeper.secretAlliesNum = (byte) alliesCount;
        SceneManager.LoadScene(1);
    }

    public void EditorButtonClick()
    {
        SceneManager.LoadScene(2);
    }

    public void ExitButtonClick()
    {
        Application.Quit();
    }    
    
    public void ChangeGameMode(bool value)
    {
        gameMode = value;
    }

    public void GetMapIndex(int value)
    {
        mapIndex = value;
    }

    public void GetAlliesCount(int value)
    {
        alliesCount = value + 1;
    }

    void Awake()
    {
        string targetDirectory =Application.dataPath.ToString() + Path.AltDirectorySeparatorChar + "Maps/";
        string[] subdirectoryEntries = Directory.GetDirectories(targetDirectory);
        mapNames = new List<string>();
        list.ClearOptions();
        for (int i = 0; i < subdirectoryEntries.Length; i++)
        {
            mapNames.Add( subdirectoryEntries[i].Substring(targetDirectory.Length));
        }
        list.AddOptions(mapNames);

    }
}

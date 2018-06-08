using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapLoader : MonoBehaviour {
        
	public static void LoadMap()
    {
        string path = Application.dataPath.ToString() +
            Path.AltDirectorySeparatorChar +
            "Maps" +
            Path.AltDirectorySeparatorChar + "test.map";

        using (
        BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            //Delete current map
            GameMaster.DropMap();

            PropertiesKeeper.mapWidth = reader.ReadInt32();
            PropertiesKeeper.mapWidth = reader.ReadInt32();

            GameMaster.BuildMap();

            for (int i = 0; i < (PropertiesKeeper.mapHeight * PropertiesKeeper.mapWidth); i++)
            {
                LoadCell(reader, PropertiesKeeper.logicalMap.cells[i]);
            }

            int countryCount = reader.ReadInt32();
            for (int i = 0; i < countryCount; i++)
            {
                LoadCountry(reader);
            }
        }
    }

    public static void LoadMap(string mapName, string folderName)
    {
        string path = Application.dataPath.ToString() +
            Path.AltDirectorySeparatorChar +
            "Maps" +
            Path.AltDirectorySeparatorChar + folderName +
             Path.AltDirectorySeparatorChar;

        if (!Directory.Exists(Application.dataPath.ToString() +
           Path.AltDirectorySeparatorChar +
           "Maps" +
           Path.AltDirectorySeparatorChar + folderName +
           Path.AltDirectorySeparatorChar))
        {
            return;
        }

        using (
        BinaryReader reader = new BinaryReader(File.Open((path + mapName + ".map"), FileMode.Open)))
        {
            //Delete current map
            GameMaster.DropMap();

            PropertiesKeeper.mapHeight = reader.ReadInt32();
            PropertiesKeeper.mapWidth = reader.ReadInt32();

            if (File.Exists(path + mapName + "_mesh.asset"))
            {
                GameMaster.BuildMap(mapName, folderName);
            }
            else
            {
                GameMaster.BuildMap();
            }

            for (int i = 0; i < (PropertiesKeeper.mapHeight * PropertiesKeeper.mapWidth); i++)
            {
                LoadCell(reader, PropertiesKeeper.logicalMap.cells[i]);
            }

            if (PropertiesKeeper.physicalMap.transform.Find("forestContainer") != null)
            {
                Destroy(PropertiesKeeper.physicalMap.transform.Find("forestContainer"));
            }
            GameObject forestContainer = new GameObject("forestContainer");
            forestContainer.transform.SetParent(PropertiesKeeper.physicalMap.transform);
            PropertiesKeeper.logicalMap.AddForest();

            int countryCount = reader.ReadInt32();
           
            if (PropertiesKeeper.defaultGameMode)
            {
                Allegiance[] allegiance = GenerateAllegiance(countryCount);
                for (int i = 0; i < countryCount; i++)
                {
                    LoadCountry(reader,allegiance[i]);
                }
            }
            else
            {
                for (int i = 0; i < countryCount; i++)
                {
                    LoadCountry(reader);
                }
            }

            if (File.Exists(path + mapName + "_border0.asset"))
            {
                for (int i = 0; i < countryCount; i++)
                {
                    GameObject border = Instantiate(PropertiesKeeper.borderPrefab,  PropertiesKeeper.countries[i].transform, true);
                    MeshSaver.LoadMesh(mapName, folderName, "border" + i, border.GetComponent<MeshFilter>().mesh);
                }
            }

        }
    }

    static void LoadCell(BinaryReader reader, LogicalMapCell cell)
    {
        cell.terrain = (TerrainType)reader.ReadByte();
    }

    static void LoadCountry(BinaryReader reader)
    {

        string countryName = reader.ReadString();

        string typeName = reader.ReadString();
        int prefabIndex;
        for(prefabIndex = 0; prefabIndex < PropertiesKeeper.countryTypes.Length; prefabIndex++)
        {
            if (PropertiesKeeper.countryPrefabs[prefabIndex].type.name == typeName)
            {
                break;
            }
        }

        Allegiance allegiance = (Allegiance)reader.ReadByte();
        int areaSize = reader.ReadInt32();

        LogicalMapCell cell = PropertiesKeeper.logicalMap.cells[reader.ReadInt32()];

        Country country = Instantiate(PropertiesKeeper.countryPrefabs[prefabIndex]);
        country.CreateCountry(countryName, allegiance, cell);

        for (int i = 1; i < areaSize; i++)
        {            
            cell = PropertiesKeeper.logicalMap.cells[reader.ReadInt32()];
            country.AddAreaToCountry(cell);            
        }
         PropertiesKeeper.countries.Add(country);
    }


    static void LoadCountry(BinaryReader reader, Allegiance alleg)
    {

        string countryName = reader.ReadString();

        string typeName = reader.ReadString();
        int prefabIndex;
        for (prefabIndex = 0; prefabIndex < PropertiesKeeper.countryTypes.Length; prefabIndex++)
        {
            if (PropertiesKeeper.countryPrefabs[prefabIndex].type.name == typeName)
            {
                break;
            }
        }

        Allegiance allegiance = (Allegiance)reader.ReadByte();
        int areaSize = reader.ReadInt32();

        LogicalMapCell cell = PropertiesKeeper.logicalMap.cells[reader.ReadInt32()];

        Country country = Instantiate(PropertiesKeeper.countryPrefabs[prefabIndex]);
        country.CreateCountry(countryName, alleg, cell);

        for (int i = 1; i < areaSize; i++)
        {
            cell = PropertiesKeeper.logicalMap.cells[reader.ReadInt32()];
            country.AddAreaToCountry(cell);
        }
        PropertiesKeeper.countries.Add(country);
    }

    static Allegiance[] GenerateAllegiance(int countryCount)
    {
        Allegiance[] result = new Allegiance[countryCount];
        for (int i = 0; i < countryCount; i++)
        {
            result[i] = Allegiance.Neutral;
        }

        if (countryCount == 2)
        {
            result[0] = (Allegiance)Random.Range(0, 2);
            result[1] = AllegianceExtentions.Opposite(result[0]);
            return result;

        }

        if (countryCount == 3)
        {
            int i = Random.Range(0, 3);
            int j;
            do
            {
               j = Random.Range(0, 3);
            } while (i == j);

            result[i] = (Allegiance) Random.Range(0,2);
            result[j] = AllegianceExtentions.Opposite(result[i]);
            return result;
        }

        int allies =Mathf.CeilToInt(countryCount * PropertiesKeeper.secretAlliesNum);
        if (allies == 1) allies++;
        if (allies % 2 != 0) allies--;

        int dAllies = allies / 2;
        int sAllies = dAllies;

        do
        {
            int i = Random.Range(0, countryCount);
            int j = Random.Range(0, countryCount);
            if (result[i] != Allegiance.Sentinels && result[i] != Allegiance.Sentinels)
            {
                result[i] = Allegiance.Dominion;
                dAllies--;
            }
            if (i !=j && result[j] != Allegiance.Sentinels && result[j] != Allegiance.Sentinels)
            {
                result[j] = Allegiance.Sentinels;
                sAllies--;
            }
        } while (dAllies > 0 && sAllies>0);

        return result;
    }
}

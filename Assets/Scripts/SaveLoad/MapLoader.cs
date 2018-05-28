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

            GameMaster.mapWidth = reader.ReadInt32();
            GameMaster.mapWidth = reader.ReadInt32();

            GameMaster.BuildMap();

            for (int i = 0; i < (GameMaster.mapHeight * GameMaster.mapWidth); i++)
            {
                LoadCell(reader, GameMaster.logicalMap.cells[i]);
            }

            int countryCount = reader.ReadInt32();
            for (int i = 0; i < countryCount; i++)
            {
                LoadCountry(reader);
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
        for(prefabIndex = 0; prefabIndex < GameMaster.countryTypes.Length; prefabIndex++)
        {
            if (GameMaster.countryPrefabs[prefabIndex].type.name == typeName)
            {
                break;
            }
        }

        Allegiance allegiance = (Allegiance)reader.ReadByte();
        int areaSize = reader.ReadInt32();

        LogicalMapCell cell = GameMaster.logicalMap.cells[reader.ReadInt32()];

        Country country = Instantiate(GameMaster.countryPrefabs[prefabIndex]);
        country.CreateCountry(countryName, allegiance, cell);

        for (int i = 1; i < areaSize; i++)
        {            
            cell = GameMaster.logicalMap.cells[reader.ReadInt32()];
            country.AddAreaToCountry(cell);            
        }
        GameMaster.countries.Add(country);
    }    
}

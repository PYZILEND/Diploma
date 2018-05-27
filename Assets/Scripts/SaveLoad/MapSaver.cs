using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapSaver : MonoBehaviour {

    public MapCreator mapCreator;

    public static void SaveMap()
    {
        string path = Application.dataPath.ToString() + 
            Path.AltDirectorySeparatorChar + 
            "Maps" + 
            Path.AltDirectorySeparatorChar + "test.map";

        using (
        BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(GameMaster.mapHeight);
            writer.Write(GameMaster.mapWidth);

            for(int i = 0; i < (GameMaster.mapHeight*GameMaster.mapWidth); i++)
            {
                SaveCell(writer, GameMaster.logicalMap.cells[i]);
            }

            int countryCount = GameMaster.countries.Count;
            writer.Write(countryCount);
            for(int i = 0; i < countryCount; i++)
            {
                SaveCountry(writer, GameMaster.countries[i], GameMaster.mapWidth);
            }
        }
    }

    public static void SaveMapCreator(string mapName)
    {
        string path = Application.dataPath.ToString() +
            Path.AltDirectorySeparatorChar +
            "Maps" +
            Path.AltDirectorySeparatorChar + mapName+ ".map";


        using (
        BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(MapCreator.mapHeight);
            writer.Write(MapCreator.mapWidth);

            for (int i = 0; i < (MapCreator.mapHeight * MapCreator.mapWidth); i++)
            {
                SaveCell(writer, MapCreator.newMap.cells[i]);
            }

            int countryCount = MapCreator.countries.Count;
            writer.Write(countryCount);
            for (int i = 0; i < countryCount; i++)
            {
                SaveCountry(writer, MapCreator.countries[i], MapCreator.mapWidth);
            }
        }
    }

    static void SaveCell(BinaryWriter writer, LogicalMapCell cell)
    {
        writer.Write((byte)cell.terrain);
    }

    static void SaveCountry(BinaryWriter writer, Country country,int mapWidth)
    {
        writer.Write(country.countryName);
        writer.Write((byte)country.type);
        writer.Write((byte)country.secretAllegiance);
        writer.Write(country.area.Count);
        for(int i = 0; i<country.area.Count; i++)
        {
            writer.Write(country.area[i].coordinates.ToIndex(mapWidth));
        }
    }
}

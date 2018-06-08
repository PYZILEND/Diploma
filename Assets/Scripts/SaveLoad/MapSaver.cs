using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapSaver : MonoBehaviour {

	public static void SaveMap()
    {
        string path = Application.dataPath.ToString() + 
            Path.AltDirectorySeparatorChar + 
            "Maps" + 
            Path.AltDirectorySeparatorChar + "test.map";

        using (
        BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(PropertiesKeeper.mapHeight);
            writer.Write(PropertiesKeeper.mapWidth);

            for(int i = 0; i < (PropertiesKeeper.mapHeight* PropertiesKeeper.mapWidth); i++)
            {
                SaveCell(writer, PropertiesKeeper.logicalMap.cells[i]);
            }

            int countryCount =  PropertiesKeeper.countries.Count;
            writer.Write(countryCount);
            for(int i = 0; i < countryCount; i++)
            {
                SaveCountry(writer,  PropertiesKeeper.countries[i]);
            }
        }
    }

    public static void SaveMapCreator(string mapName)
    {
        string path = Application.dataPath.ToString() +
            Path.AltDirectorySeparatorChar +
            "Maps" +
            Path.AltDirectorySeparatorChar + mapName + ".map";


        using (
        BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(PropertiesKeeper.mapHeight);
            writer.Write(PropertiesKeeper.mapWidth);

            for (int i = 0; i < (PropertiesKeeper.mapHeight * PropertiesKeeper.mapWidth); i++)
            {
                SaveCell(writer,  PropertiesKeeper.logicalMap.cells[i]);
            }

            int countryCount = PropertiesKeeper.countries.Count;
            writer.Write(countryCount);
            for (int i = 0; i < countryCount; i++)
            {
                SaveCountry(writer, PropertiesKeeper.countries[i]);
            }
        }
    }

    public static void SaveMapCreator(string mapName, string folderName)
    {
        string path = Application.dataPath.ToString() +
            Path.AltDirectorySeparatorChar +
            "Maps" +
            Path.AltDirectorySeparatorChar + folderName +
            Path.AltDirectorySeparatorChar + mapName + ".map";

        if (!Directory.Exists(Application.dataPath.ToString() +
           Path.AltDirectorySeparatorChar +
           "Maps" +
           Path.AltDirectorySeparatorChar + folderName +
           Path.AltDirectorySeparatorChar))
        {
            Directory.CreateDirectory(Application.dataPath.ToString() +
           Path.AltDirectorySeparatorChar +
           "Maps" +
           Path.AltDirectorySeparatorChar + folderName +
           Path.AltDirectorySeparatorChar);
        }
        using (
    BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(PropertiesKeeper.mapHeight);
            writer.Write(PropertiesKeeper.mapWidth);

            for (int i = 0; i < (PropertiesKeeper.mapHeight * PropertiesKeeper.mapWidth); i++)
            {
                SaveCell(writer,  PropertiesKeeper.logicalMap.cells[i]);
            }

            int countryCount = PropertiesKeeper.countries.Count;
            writer.Write(countryCount);
            for (int i = 0; i < countryCount; i++)
            {
                SaveCountry(writer, PropertiesKeeper.countries[i]);
            }
        }
    }

    static void SaveCell(BinaryWriter writer, LogicalMapCell cell)
    {
        writer.Write((byte)cell.terrain);
    }

    static void SaveCountry(BinaryWriter writer, Country country)
    {
        writer.Write(country.countryName);
        writer.Write(country.type.name);
        writer.Write((byte)country.secretAllegiance);
        writer.Write(country.area.Count);
        for(int i = 0; i<country.area.Count; i++)
        {
            writer.Write(country.area[i].coordinates.ToIndex());
        }
    }
}

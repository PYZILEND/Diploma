using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MeshSaver : MonoBehaviour {

	public static void SaveMesh(string mapName, string folderName, string meshName, Mesh mesh)
    {
        string map;
        if (mesh.name.Equals(""))
        {
            map="unnamed";
        }
        else
        {
            map = mesh.name;
        }
        
        string path = Application.dataPath.ToString() +
               Path.AltDirectorySeparatorChar +
               "Maps" +
               Path.AltDirectorySeparatorChar +folderName+ 
               Path.AltDirectorySeparatorChar + mapName+"_"+meshName+".asset";

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
            writer.Write(map);

            MeshSaving(writer, mesh);
        }
    }

    public static void SaveMesh(string mapName, string folderName, string meshName, int i, Mesh mesh)
    {
        string map;
        if (mesh.name.Equals(""))
        {
            map = "unnamed";
        }
        else
        {
            map = mesh.name;
        }

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

        string path = Application.dataPath.ToString() +
               Path.AltDirectorySeparatorChar +
               "Maps" +
               Path.AltDirectorySeparatorChar + mapName + "_" + meshName +""+i+ ".asset";

        using (
         BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
        {
            writer.Write(map);

            MeshSaving(writer, mesh);
        }
    }

    static void MeshSaving(BinaryWriter writer, Mesh mesh)
    {
        Vector3[] vertices = mesh.vertices;

        writer.Write(vertices.Length);
        for (int i = 0; i < vertices.Length; i++)
        {
            writer.Write(vertices[i].x);
            writer.Write(vertices[i].y);
            writer.Write(vertices[i].z);
        }

        int[] triangles = mesh.triangles;
        writer.Write(triangles.Length);

        for (int i = 0; i < triangles.Length; i++)
        {
            writer.Write(triangles[i]);
        }

        if (mesh.colors.Length > 0)
        {
            ColorSaving(writer, mesh.colors);
        }
        else
        {
            writer.Write(false);
        }

        if (mesh.uv.Length > 0)
        {
            UVSaving(writer, mesh.uv);
        }
        else
        {
            writer.Write(false);
        }

    }

    static void ColorSaving(BinaryWriter writer,Color[] colors)
    {
        writer.Write(true);

        for (int i = 0; i < colors.Length; i++)
        {
            writer.Write(colors[i].r);
            writer.Write(colors[i].g);
            writer.Write(colors[i].b);
            writer.Write(colors[i].a);
        }
    }

    static void UVSaving(BinaryWriter writer, Vector2[] uv)
    {
        writer.Write(true);

        for (int i = 0; i < uv.Length; i++)
        {
            writer.Write(uv[i].x);
            writer.Write(uv[i].y);
        }
    }

    public static void LoadMesh(string mapName, string folderName,string meshName,Mesh mesh)
    {
        string path = Application.dataPath.ToString() +
            Path.AltDirectorySeparatorChar +
            "Maps" +
            Path.AltDirectorySeparatorChar +folderName+
            Path.AltDirectorySeparatorChar + mapName +"_"+meshName+ ".asset";

        using (
        BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            string map = reader.ReadString();
            if (!map.Equals("unnamed"))
            {
                mesh.name = map;
            }
            else
            {
                mesh.name = mapName;
            }

            MeshLoading(reader, mesh);
        }
    }

    static void MeshLoading(BinaryReader reader, Mesh mesh)
    {
        int vertLength = reader.ReadInt32();
        Vector3[] vertices = new Vector3[vertLength];
        for (int i=0;i< vertLength; i++)
        {
            vertices[i].x = reader.ReadSingle();
            vertices[i].y = reader.ReadSingle();
            vertices[i].z = reader.ReadSingle();
        }

        int triagLength = reader.ReadInt32();
        int[] triangles = new int[triagLength];
        for (int i = 0; i < triagLength; i++)
        {
            triangles[i] = reader.ReadInt32();
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        bool colorsCheck = reader.ReadBoolean();
        if (colorsCheck)
        {
            mesh.colors = ColorsLoading(reader,vertices.Length);
        }

        bool uvCheck = reader.ReadBoolean();
        if (uvCheck)
        {
            mesh.uv = UVLoading(reader, vertices.Length);
        }

    }

    static Color[] ColorsLoading(BinaryReader reader,int length)
    {
        Color[] colors = new Color[length];

        for (int i = 0; i < length; i++)
        {
            colors[i].r= reader.ReadSingle();
            colors[i].g= reader.ReadSingle();
            colors[i].b= reader.ReadSingle();
            colors[i].a= reader.ReadSingle();
        }
        return colors;
    }

    static Vector2[] UVLoading(BinaryReader reader, int length)
    {
        Vector2[] uv = new Vector2[length];

        for (int i = 0; i < length; i++)
        {
            uv[i].x = reader.ReadSingle();
            uv[i].y = reader.ReadSingle();
        }
        return uv;
    }
}

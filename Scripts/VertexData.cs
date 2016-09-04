using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class VertexData : ScriptableObject {
    Mesh mesh;
    public int vertexCount;
    public int[] indices;
    public Vector3[] vertices;
    public Vector3[] normals;
    public Vector2[] uvs;    
    

    public ComputeBuffer indexBuffer;    
    public ComputeBuffer vertexBuffer;
    public ComputeBuffer normalBuffer;
    public ComputeBuffer uvBuffer;
    
        

    public void initBuffers()
    {
        indexBuffer = new ComputeBuffer(indices.Length, sizeof(int));
        indexBuffer.SetData(indices);
        vertexBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 3);
        vertexBuffer.SetData(vertices);
        normalBuffer = new ComputeBuffer(normals.Length, sizeof(float) * 3);
        normalBuffer.SetData(normals);
        uvBuffer = new ComputeBuffer(uvs.Length, sizeof(float) * 2);
        uvBuffer.SetData(uvs);
        

    }

    public void Release()
    {        
        indexBuffer.Release();        
        vertexBuffer.Release();        
        normalBuffer.Release();
        uvBuffer.Release();
    }

    
    public void CreateData(Mesh meshIn)
    {
        if (meshIn == null) return;

        mesh = meshIn;

        BuildData();
    }

    void BuildData()
    {
        vertexCount = mesh.vertexCount;
        indices = mesh.triangles;
        vertices = mesh.vertices;
        normals = mesh.normals;
        uvs = mesh.uv;
        string name = mesh.name;
        mesh = null;        


        //AssetDatabase.CreateAsset(this, "Assets/" + name + ".asset");
        //AssetDatabase.SaveAssets();
        //AssetDatabase.Refresh();
        //Selection.activeObject = this;
    }

}

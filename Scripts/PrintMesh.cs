using UnityEngine;
using System.Collections;

public class PrintMesh : MonoBehaviour {
    public Mesh mesh;

	// Use this for initialization
	void Start () {
        if (mesh == null) return;
        VertexData vData = ScriptableObject.CreateInstance<VertexData>();
        vData.CreateData(mesh);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

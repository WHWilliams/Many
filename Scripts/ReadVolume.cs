

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;




public class ReadVolume : MonoBehaviour {
    public string path = "C:/vol.qb";
    FileStream filestream;

    public uint version;
    public uint colorFormat;
    public uint zAxisOrientation;
    public uint compressed;
    public uint visibilityMaskEncoded;
    public uint numMatricies;
    public uint i;
    public uint j;
    public uint x;
    public uint y;
    public uint z;
    public uint sizeX;
    public uint sizeY;
    public uint sizeZ;
    uint posX;
    uint posY;
    uint posZ;
    uint[] matrix;
    List<uint[]> matrixList;
    uint index;
    uint data;
    uint count;
    const uint CODEFLAG = 2;
    const uint NEXTSLICEFLAG = 6;    

	// Use this for initialization
	void Start () {
        filestream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        BinaryReader binaryReader = new BinaryReader(filestream);
        version = binaryReader.ReadUInt32();
        colorFormat = binaryReader.ReadUInt32();
        zAxisOrientation = binaryReader.ReadUInt32();
        compressed = binaryReader.ReadUInt32();
        visibilityMaskEncoded = binaryReader.ReadUInt32();
        numMatricies = binaryReader.ReadUInt32();
        matrixList = new List<uint[]>();

        
        
        int nameLength = binaryReader.ReadByte();
        string name = binaryReader.ReadBytes(nameLength).ToString();

        sizeX = binaryReader.ReadUInt32();
        sizeY = binaryReader.ReadUInt32();
        sizeZ = binaryReader.ReadUInt32();

        posX = binaryReader.ReadUInt32();
        posY = binaryReader.ReadUInt32();
        posZ = binaryReader.ReadUInt32();

        matrix = new uint[sizeX * sizeY * sizeZ];
        matrixList.Add(matrix);

        if(compressed == 0)
        {
            for (z = 0; z < sizeZ; z++)
                for (y = 0; y < sizeY; y++)
                    for (x = 0; x < sizeX; x++)
                    {
                        uint mi = binaryReader.ReadUInt32();
                        matrix[x + y * sizeX + z * sizeX * sizeY] = mi;
                    }
        }



        filestream.Close();
    }

    private Color32 UIntToColor(uint color)
    {
        byte a = (byte)(color >> 24);
        byte b = (byte)(color >> 16);
        byte g = (byte)(color >> 8);
        byte r = (byte)(color >> 0);
        return new Color32(r, g, b, a);
    }

    public int getBufferSize()
    {
        return (int)sizeX * (int)sizeY * (int)sizeZ;
    }

    public ComputeBuffer bufferFromVolume()
    {
        ComputeBuffer computeBuffer = new ComputeBuffer(getBufferSize(), sizeof(uint));
        computeBuffer.SetData(matrix);
        return computeBuffer;
    }
}

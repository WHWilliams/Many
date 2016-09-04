using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class AgentWorld : MonoBehaviour {
    private GameObject player;
    public VertexData characterData;
    public ComputeShader agentComputeShader;
    public int maxAgents = 512;
    public int spawnerBlockCount = 1;
    public float unitScale = 1.0f;
    public CameraEvent renderWhen = CameraEvent.AfterGBuffer;
    public float maxAccel = 0.5f;
    int[] agentCountArgs;
    int BLOCK_SIZE = 64;
    

    private int agentCount;
    public int AgentCount { get { return agentCount; } }

    int KMain;
    int KInit;
    int KSpawn;
    int KInstantiate;
    int KDealDamage;

    ComputeBuffer agentBuffer0;
    ComputeBuffer agentBuffer1;
    ComputeBuffer freeBuffer;
    ComputeBuffer spawnBuffer;
    ComputeBuffer agentInstanceBuffer;
    ComputeBuffer agentCountArgBuffer;
    ComputeBuffer damageBuffer;

    private List<AgentData> spawningAgents;

    public Material instanceMaterial;

    void OnValidate()
    {
        int border = BLOCK_SIZE;
        while (maxAgents > border)
        {
            border *= 2;
        }
        maxAgents = border;
    }

    public void DamageWorld(float magnitude, float radius, Vector3 position)
    {
        agentComputeShader.SetBuffer(KDealDamage, "damageBuffer", damageBuffer);
        agentComputeShader.SetBuffer(KDealDamage, "agentBufferIn", agentBufferIn);
        agentComputeShader.SetVector("damagePos", position);
        agentComputeShader.SetFloat("damageMag", magnitude);
        agentComputeShader.SetFloat("damageRadius", radius);
        agentComputeShader.Dispatch(KDealDamage, maxAgents / BLOCK_SIZE, 1, 1);
    }

    // Use this for initialization
    void Start () {

        // get player gameobject
        player = GameObject.Find("FPSController");
        
        // init character data
        characterData.initBuffers();


        // Create Kernels and Material
        KMain = agentComputeShader.FindKernel("Main");
        KInit = agentComputeShader.FindKernel("Init");
        KSpawn = agentComputeShader.FindKernel("Spawn");
        KInstantiate = agentComputeShader.FindKernel("Instantiate");
        KDealDamage = agentComputeShader.FindKernel("DealDamage");


        ///
        // Create Buffers 
        // Agent Buffers
        agentBuffer0 = new ComputeBuffer(maxAgents, AgentData.size);
        agentBuffer1 = new ComputeBuffer(maxAgents, AgentData.size);

        // Spawn Buffer 
        spawnBuffer = new ComputeBuffer(BLOCK_SIZE * spawnerBlockCount, AgentData.size);
        spawningAgents = new List<AgentData>(BLOCK_SIZE * spawnerBlockCount);

        // Free Buffer
        freeBuffer = new ComputeBuffer(maxAgents, sizeof(int), ComputeBufferType.Append);
        freeBuffer.ClearAppendBuffer();

        // Instance Buffer
        agentInstanceBuffer = new ComputeBuffer(maxAgents, sizeof(int), ComputeBufferType.Append);
        agentInstanceBuffer.ClearAppendBuffer();

        // Damage Buffer
        damageBuffer = new ComputeBuffer(maxAgents, sizeof(float));
        
        // Args Buffer
        agentCountArgBuffer = new ComputeBuffer(4, sizeof(int), ComputeBufferType.IndirectArguments);
        agentCountArgs = new int[] { characterData.indexBuffer.count, 0, 0, 0 };
        agentCountArgBuffer.SetData(agentCountArgs);

        

        // Run init shader on initial buffer 
        agentComputeShader.SetBuffer(KInit, "appendFreeBuffer", freeBuffer);
        agentComputeShader.SetBuffer(KInit, "agentBufferOut", agentBuffer0);
        agentComputeShader.SetBuffer(KInit, "damageBuffer", damageBuffer);
        agentComputeShader.Dispatch(KInit, maxAgents / BLOCK_SIZE, 1, 1);


        // Init constant uniforms 
        agentComputeShader.SetInt("maxAgents", maxAgents);

        SetCommandBuffer();


    }
    CommandBuffer cb = null;
    void SetCommandBuffer()
    {
        if(cb != null)
        {
            Camera.main.RemoveCommandBuffer(renderWhen, cb);
        }
        cb = new CommandBuffer();
        cb.name = "instance";        
        cb.DrawProcedural(Matrix4x4.identity, instanceMaterial, 0, MeshTopology.Triangles, characterData.indexBuffer.count, agentCount);
        //cb.DrawProceduralIndirect(Matrix4x4.identity, instanceMaterial, 0, MeshTopology.Triangles, agentCountArgBuffer);
        Camera.main.AddCommandBuffer(renderWhen, cb);
    }

    bool isEvenInBuffer = true;
    ComputeBuffer agentBufferIn;
    ComputeBuffer agentBufferOut;
    void Update()
    {
        // Flip Buffers
        agentBufferIn = isEvenInBuffer ? agentBuffer0 : agentBuffer1;
        agentBufferOut = isEvenInBuffer ? agentBuffer1 : agentBuffer0;
        isEvenInBuffer = !isEvenInBuffer;

        ///
        // Dispatch Compute Shaders        
        // Set Compute Shader Buffers and uniforms for Main
        agentComputeShader.SetFloat("maxAccel", maxAccel);
        Vector3 groundedTarget = player.transform.position;
        groundedTarget.y = 0.0f;

        agentComputeShader.SetVector("targetPos", groundedTarget);
        agentComputeShader.SetBuffer(KMain, "agentBufferIn", agentBufferIn);
        agentComputeShader.SetBuffer(KMain, "agentBufferOut", agentBufferOut);
        agentComputeShader.SetBuffer(KMain, "damageBuffer", damageBuffer);
        agentComputeShader.SetBuffer(KMain, "appendFreeBuffer", freeBuffer);
        // Dispatch Main Kernel
        agentComputeShader.Dispatch(KMain, maxAgents / BLOCK_SIZE, 1, 1);



        // Set Compute Shader Buffers and uniforms for Spawn 
        spawnBuffer.SetData(spawningAgents.ToArray());
        agentComputeShader.SetInt("agentsToSpawn", spawningAgents.Count);
        agentComputeShader.SetBuffer(KSpawn, "spawnBuffer", spawnBuffer);
        agentComputeShader.SetBuffer(KSpawn, "consumeFreeBuffer", freeBuffer);
        agentComputeShader.SetBuffer(KSpawn, "agentBufferOut", agentBufferOut);

        // Dispatch Spawn Kernel
        agentComputeShader.Dispatch(KSpawn, spawnerBlockCount, 1, 1);
        spawningAgents.Clear();


        // Update agent count        
        ComputeBuffer.CopyCount(freeBuffer, agentCountArgBuffer, 4);        
        SetCommandBuffer();


        // Instantiate active particles for rendering
        agentInstanceBuffer.ClearAppendBuffer();
        agentComputeShader.SetBuffer(KInstantiate, "instanceAppendBuffer", agentInstanceBuffer);
        agentComputeShader.SetBuffer(KInstantiate, "agentBufferIn", agentBufferIn);
        agentComputeShader.Dispatch(KInstantiate, maxAgents / BLOCK_SIZE, 1, 1);
        
        /// Attach Buffers to material        
        instanceMaterial.SetBuffer("_AgentData", agentBufferIn);
        instanceMaterial.SetBuffer("_AgentIndirectBuffer", agentInstanceBuffer);
        instanceMaterial.SetBuffer("_Indices", characterData.indexBuffer);
        instanceMaterial.SetBuffer("_UV", characterData.uvBuffer);
        instanceMaterial.SetBuffer("_VertexData", characterData.vertexBuffer);
        instanceMaterial.SetBuffer("_NormalsData", characterData.normalBuffer);

        agentCountArgBuffer.GetData(agentCountArgs);
        agentCount = maxAgents - agentCountArgs[1];
    }


 

    void OnDisable()
    {
        // Release Buffers
        agentBuffer0.Release();
        agentBuffer1.Release();
        spawnBuffer.Release();
        freeBuffer.Release();
        agentInstanceBuffer.Release();
        damageBuffer.Release();

        characterData.Release();

        agentCountArgBuffer.Release();

        //Camera.main.RemoveAllCommandBuffers();
    }

    public bool Spawn(AgentData spawningAgent)
    {
        if (spawningAgents.Count >= BLOCK_SIZE * spawnerBlockCount || spawningAgents.Count >= (maxAgents - BLOCK_SIZE * spawnerBlockCount * 4) - agentCount) return false;

        if (spawningAgent.life < 0.1f) spawningAgent.life = 0.1f;
        spawningAgents.Add(spawningAgent);
        return true;
    }

}

public static class MyExtensions
{
    public static void ClearAppendBuffer(this ComputeBuffer appendBuffer)
    {
        // This resets the append buffer buffer to 0
        RenderTexture dummy1 = RenderTexture.GetTemporary(8, 8, 24, RenderTextureFormat.ARGB32);
        RenderTexture dummy2 = RenderTexture.GetTemporary(8, 8, 24, RenderTextureFormat.ARGB32);
        RenderTexture active = RenderTexture.active;

        Graphics.SetRandomWriteTarget(1, appendBuffer);
        Graphics.Blit(dummy1, dummy2);
        Graphics.ClearRandomWriteTargets();

        RenderTexture.active = active;

        dummy1.Release();
        dummy2.Release();
    }
}
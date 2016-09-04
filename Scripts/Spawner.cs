using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {



    public float spawnLife = 1.0f;
    public int spawnCount = 1;
    public float spawnRadius = 1.0f;
    public float targetRadius = 1.0f;
    public bool doSpawn = false;
    public float yMag = 0.0f;
    public bool isCharGrounded = false;
    public AgentWorld agentManager;

    // Use this for initialization
    


    // Update is called once per frame
    void Update()
    {

        if (doSpawn) Spawn(spawnCount);
    }

    public void Spawn(int id)
    {
        if (agentManager == null) return;

        for (int i = 0; i < spawnCount; i++)
        {
            Vector3 p = Random.insideUnitSphere * spawnRadius + transform.position;
            if(isCharGrounded) p.y = yMag;
            if (!agentManager.Spawn(new AgentData
            {
                pos = p
                ,
                life = spawnLife
                ,
                vel = Vector3.zero
            })) break;
        }
    }


}

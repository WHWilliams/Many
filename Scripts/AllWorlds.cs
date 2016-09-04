using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AllWorlds : MonoBehaviour {
    private AgentWorld[] worlds;
    public static AllWorlds instance;
	// Use this for initialization
	void Start () {
        instance = this;
        worlds = FindObjectsOfType<AgentWorld>();
	}

    public void DamageAllWorlds(float mag, float rad, Vector3 pos)
    {
        foreach(AgentWorld world in worlds)
        {
            world.DamageWorld(mag, rad, pos);
        }
    }

    
}

using UnityEngine;
using System.Collections;

public struct AgentData
{
    public const int size = sizeof(float) * 7;
    public Vector3 pos;
    public Vector3 vel;
    public float life;
}

public struct DeathData
{
    public const int size = sizeof(float) * 6;
    public Vector3 damagerPos;
    public Vector3 deathPos;
}

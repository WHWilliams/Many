using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float rad;
    public float visualScale = 1.0f;
    public float mag;
    public float life;
    public Vector3 vel;
    private AllWorlds worlds;

	// Use this for initialization
	void Start () {
        worlds = AllWorlds.instance;
	}
	
	// Update is called once per frame
	void Update () {       

        transform.localScale = new Vector3(rad*visualScale, rad * visualScale, rad * visualScale);
        transform.position += vel;
        worlds.DamageAllWorlds(mag, rad, transform.position);
        life--;
        if (life <0.0f)
        {
            GameObject.Destroy(gameObject);
        }

    }
}

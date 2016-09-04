using UnityEngine;
using System.Collections;



public class Shoot : MonoBehaviour {

    public GameObject bullet;
    public float bulletSpeed = 0.1f;
    public int bulletLife = 300;
    public float bulletDmg = 1000;
    public float bulletR = 1.0f;
    public float yOffset = -1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetButton("Fire1"))
        {
            Bullet b = ((GameObject)GameObject.Instantiate(bullet, transform.position+Vector3.up*yOffset, Quaternion.identity)).GetComponent<Bullet>();
            b.vel = transform.forward * bulletSpeed;
            b.life = bulletLife;
            b.mag = bulletDmg;
            b.rad = bulletR;
        }
	}
}

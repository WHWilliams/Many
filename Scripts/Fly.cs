using UnityEngine;
using System.Collections;

public class Fly : MonoBehaviour {

    public float sprintSpeed = 1.0f;
    public float walkSpeed = .1f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        if(Input.GetKey(KeyCode.Q))
        {
            transform.position += Vector3.up * speed;
        }
        else if(Input.GetKey(KeyCode.E))
        {
            transform.position += Vector3.down * speed;
        }

	}
}

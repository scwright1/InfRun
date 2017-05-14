using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingMovement : MonoBehaviour {

    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        rb.AddForce(Vector3.back * 9.81f);
	}
}

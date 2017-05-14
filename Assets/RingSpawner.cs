using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RingSpawner : MonoBehaviour {

    public GameObject ring;

    [Range(0.1f, 1.0f)]
    public float spawnRate = 0.1f;

	// Use this for initialization
	void Start () {
        StartCoroutine("GenerateRing");
    }
	
	// Update is called once per frame
	void Update () {
	}

    IEnumerator GenerateRing() {
        while(true) {
			Instantiate(ring,transform.localPosition,Quaternion.identity,transform);
            yield return new WaitForSeconds(spawnRate);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingCollider : MonoBehaviour {

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.tag == "RingCollector") {
            Destroy(gameObject);
        }

        if(collision.gameObject.tag == "Player") {
            Debug.Log("Collided the player with the ring");
        }
    }
}

using UnityEngine;
using System.Collections;

public class Deployer : MonoBehaviour {

    public Transform fishBoid;
    public Transform sharkBoid;
    float positionMagnitude = 10.0f;

	void Start () {
	    for (int i = 0; i < 15; i++) {
            Instantiate(fishBoid, new Vector3(Random.Range(-positionMagnitude, positionMagnitude),
                        Random.Range(5.0f, 15.0f), Random.Range(-positionMagnitude, positionMagnitude)), 
                        Random.rotation);
        }
        for (int i = 0; i < 5; i++) {
            Instantiate(sharkBoid, new Vector3(Random.Range(-positionMagnitude, positionMagnitude) * 3,
                        Random.Range(5.0f, 15.0f) * 3, Random.Range(-positionMagnitude, positionMagnitude) * 3), 
                        Random.rotation);
        }
	}
	
	void Update () {
	
	}
}

using UnityEngine;
using System.Collections;

public class Deployer : MonoBehaviour {

    public Transform fishBoid;
    public Transform sharkBoid;

    const int numFishes = 15;
    const int numSharks = 5;
    const float minRange = 5.0f;
    const float maxRange = 15.0f;
    const float magnitude = 10.0f;

	void Start() {
	    DeployFishBoids();
        DeploySharkBoids();
	}
	
	void Update() {
	
	}

    private void DeployFishBoids() {
        for (int i = 0; i < numFishes; i++) {
            Instantiate(fishBoid, new Vector3(Random.Range(-magnitude, magnitude),
                    Random.Range(minRange, maxRange), Random.Range(-magnitude, magnitude)), 
                    Random.rotation);
        }
    }

    private void DeploySharkBoids() {
        for (int i = 0; i < numSharks; i++) {
            Instantiate(sharkBoid, new Vector3(Random.Range(-magnitude, magnitude) * 3,
                    Random.Range(minRange, maxRange) * 3, Random.Range(-magnitude, magnitude) * 3), 
                    Random.rotation);
        }
    }
}

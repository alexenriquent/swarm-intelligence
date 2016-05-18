using UnityEngine;
using System.Collections;

public class Deployer : MonoBehaviour {

    public Transform fishBoid;
    public Transform sharkBoid;

    private const int numFishes = 15;
    private const int numSharks = 5;
    private const float minRange = 5.0f;
    private const float maxRange = 15.0f;
    private const float randRange = 10.0f;
    private const float fishRange = 10.0f;
    private const float sharkRange = 3.0f;

	void Start() {
	    DeployFishBoids();
        DeploySharkBoids();
	}

    private void DeployFishBoids() {
        for (int i = 0; i < numFishes; i++) {
            Instantiate(fishBoid, new Vector3(Random.value * fishRange,
                Random.value * fishRange + 1.0f, Random.value * fishRange), 
                Quaternion.identity);
        }
    }

    private void DeploySharkBoids() {
        for (int i = 0; i < numSharks; i++) {
            Instantiate(sharkBoid, new Vector3(Random.Range(-randRange, randRange) * sharkRange,
                Random.Range(minRange, maxRange) * sharkRange, Random.Range(-randRange, randRange) * sharkRange), 
                Random.rotation);
        }
    }
}

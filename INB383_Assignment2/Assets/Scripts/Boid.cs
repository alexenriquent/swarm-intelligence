using UnityEngine;
using System.Collections;

abstract public class Boid : MonoBehaviour {

    public Vector3 simulationCentre = new Vector3(0.0f, 10.0f, 0.0f);
    public float simulationRadius = 20.0f;
    public float maxSpeed = 3.0f;
    public float separationDistance = 3.0f;
    public float separationStrength = 50.0f;
    public float cohesionDistance = 6.0f;
    public float cohesionStrength = 1.0f;
    public Transform marker;

    protected GameObject[] boids;
    protected int boidIndex;
    protected Vector3 cohesionPosition;

    protected const float minRange = -360.0f;
    protected const float maxRange = 360.0f;
    protected const float maxDuration = 100000000.0f;
    protected const float maxDistance = 1000.0f;

    protected abstract void Initialise();
    protected abstract void BoidUpdate();

	void Start() {
	    Initialise();
	}
	
	void Update() {
	    BoidUpdate();
	}

    protected void SetNewDirection(float magnitude) {
        float step = magnitude * Time.deltaTime;
        Vector3 target = simulationCentre - transform.position;
        Vector3 direction = Vector3.RotateTowards(transform.forward, target, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(direction);
        transform.position = Vector3.MoveTowards(transform.position, simulationCentre, step);
    }

    protected void IncrementBoidIndex() {
        boidIndex++;
        if (boidIndex >= boids.Length) {
            Vector3 cohesiveForce = (cohesionStrength / Vector3.Distance(cohesionPosition, transform.position)) 
                                  * (cohesionPosition - transform.position);
            GetComponent<Rigidbody>().AddForce(cohesiveForce);
            boidIndex = 0;
            cohesionPosition = Vector3.zero;
        }
    }

    protected void MoveForward(float force) {
        Vector3 randomTarget = new Vector3(Random.Range(minRange, maxRange), 
                                           Random.Range(minRange, maxRange), 
                                           Random.Range(minRange, maxRange));
        Vector3 wanderVariation = randomTarget + marker.position;
        Quaternion wanderRotation = Quaternion.LookRotation(wanderVariation);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, wanderRotation, 2.0f);
        GetComponent<Rigidbody>().AddForce(transform.forward * force);
    }
}
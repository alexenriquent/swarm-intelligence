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
}

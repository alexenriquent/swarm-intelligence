using UnityEngine;
using System.Collections;

public class FishBoid : Boid {

    public float safeDistance;
    public float swimForce;
    private float moveForce;
    private float fleeForce;
    private GameObject[] sharkBoids;
    private int sharkIndex;
    private float inactiveDuration;

    private const string sharkTag = "SharkBoid";
    private const string activeFishTag = "FishBoid";
    private const string inactiveFishTag = "Inactive";
    private const float stepMagnitude = 10.0f;
    private const float extension = 10.0f;

    protected override void Initialise() {
        boids = null;
        boidIndex = 0;
        cohesionPosition = Vector3.zero;
        safeDistance = 10.0f;
        swimForce = 0.3f;
        moveForce = 1.0f;
        fleeForce = 7.0f;
        sharkBoids = null;
        sharkIndex = 0;
        inactiveDuration = maxDuration;
    }

    protected override void BoidUpdate() {
        if (NoBoid()) {
            GetBoids();
        } else {
            CheckBoidStatus();
            if (Escape()) {
                SetNewDirection(stepMagnitude);
                return;
            }
            IncrementSharkIndex();          
            IncrementBoidIndex();
            InvokeAction();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (gameObject.tag == activeFishTag && other.gameObject.tag == sharkTag) {
            inactiveDuration = Time.time;
        }
    }

    private bool NoBoid() {
        return  boids == null || sharkBoids == null;
    }

    private void GetBoids() {
        boids = GameObject.FindGameObjectsWithTag(activeFishTag);
        sharkBoids = GameObject.FindGameObjectsWithTag(sharkTag);
    }

    private void CheckBoidStatus() {
        if (gameObject.tag == inactiveFishTag) {
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            Color colour = gameObject.GetComponent<Renderer>().material.color;
            colour.a = 0.1f;
            if (Time.time > inactiveDuration + extension) {
                gameObject.tag = activeFishTag;
                colour.a = 1.0f;
            }
            gameObject.GetComponent<Renderer>().material.color = colour;
        }
    }

    private bool Escape() {
        return Vector3.Distance(simulationCentre, transform.position) > simulationRadius;
    }

    private void IncrementSharkIndex() {
        sharkIndex++;
        if (sharkIndex >= sharkBoids.Length) {
            sharkIndex = 0;
        }
    }

    private void InvokeAction() {
        Vector3 position = boids[boidIndex].transform.position;
        Quaternion rotation = boids[boidIndex].transform.rotation;
        float distance = Vector3.Distance(transform.position, position);
        Vector3 predatorPosition = simulationCentre;
        float predatorDistance = maxDistance;

        for (int i = 0; i < sharkBoids.Length; i++) {
            if (Vector3.Distance(sharkBoids[i].transform.position, transform.position) < predatorDistance) {
                predatorPosition = sharkBoids[i].transform.position;
                predatorDistance = Vector3.Distance(sharkBoids[i].transform.position, transform.position);
            }
        }

        if (gameObject.tag == activeFishTag && safeDistance / predatorDistance > 1) {
            Flee(predatorPosition, predatorDistance);
        } else if (gameObject.tag == activeFishTag && distance > 0.0f) {
            Swim(position, rotation, distance);
        }
    }

    private void Flee(Vector3 predatorPosition, float predatorDistance) {
        Vector3 sharkPosition;
        gameObject.GetComponent<Renderer>().material.color = Color.yellow;
        sharkPosition = predatorPosition + predatorDistance 
                        * sharkBoids[sharkIndex].GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().AddForce((transform.position - sharkPosition) * swimForce);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                             Quaternion.LookRotation(transform.position - sharkPosition), fleeForce);
    }

    private void Swim(Vector3 position, Quaternion rotation, float distance) {
        Color colour = new Color(0.482f, 0.624f, 1.0f, 1.0f);
        gameObject.GetComponent<Renderer>().material.color = colour;
        if (distance < separationDistance) {
            float scale = separationStrength / distance;
            GetComponent<Rigidbody>().AddForce(scale * Vector3.Normalize(transform.position - position));
        } else if (distance <= cohesionDistance && distance > separationDistance) {
            cohesionPosition = cohesionPosition + position * (1.0f / (float) boids.Length);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 1.0f);
            GetComponent<Rigidbody>().AddForce(transform.forward);
        } else {
            MoveForward(moveForce);
        }    
    }
}
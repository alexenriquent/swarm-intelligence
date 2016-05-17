using UnityEngine;
using System.Collections;

public class SharkBoid : Boid {

    public float swimForce;
    private float moveForce;
    private float preyForce;
    private GameObject[] fishBoids;
    private int fishIndex;
    private int appetite;
    private bool hungry;
    private float hungerDuration;

    private const string sharkTag = "SharkBoid";
    private const string activeFishTag = "FishBoid";
    private const string inactiveFishTag = "Inactive";
    private const float stepMagnitude = 5.0f;
    private const int maxAppetite = 3;
    private const int timeToHungry = 10;
    private const int decayValue = 1;
    private const float extension = 5.0f;

    protected override void Initialise() {
        boids = null;
        boidIndex = 0;
        cohesionPosition = Vector3.zero;
        swimForce = 0.8f;
        moveForce = 2.0f;
        preyForce = 7.0f;
        fishIndex = 0;
        hungry = true;
        appetite = 0;
        hungerDuration = maxDuration;
    }

    protected override void BoidUpdate() {
        if (NoBoid()) {
            GetBoids();
        } else {
            CheckHungerStatus();
            if (Escape()) {
                SetNewDirection(stepMagnitude);
                return;
            }
            IncrementFishIndex();
            IncrementBoidIndex();
            InvokeAction();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == activeFishTag && hungry) {
            TopUp();
            other.gameObject.tag = inactiveFishTag;
            hungerDuration = Time.time;
        }
    }

    private bool NoBoid() {
        return boids == null || fishBoids == null;
    }

    private void GetBoids() {
        boids = GameObject.FindGameObjectsWithTag(sharkTag);
        fishBoids = GameObject.FindGameObjectsWithTag(activeFishTag);
    } 

    private void CheckHungerStatus() {
        if (Time.time % timeToHungry == 0) {
            Decay(decayValue);
        }
        if (appetite >= maxAppetite) {
            hungry = false;
            Color colour = new Color(0.557f, 0.580f, 0.580f, 1.0f);
            gameObject.GetComponent<Renderer>().material.color = colour;
            if (Time.time > hungerDuration + extension) {
                Decay(decayValue + 1);
            }
        } else {
            hungry = true;
        }
    }

    private void TopUp() {
        appetite++;
    }

    private void Decay(int decayValue) {
        appetite -= decayValue;
    }

    private bool Escape() {
        return Vector3.Distance(simulationCentre, transform.position) > simulationRadius;
    }

    private void IncrementFishIndex() {
        fishIndex++;
        if (fishIndex >= fishBoids.Length) {
            fishIndex = 0;
        }
    }

    private void InvokeAction() {
        Vector3 position = boids[boidIndex].transform.position;
        Quaternion rotation = boids[boidIndex].transform.rotation;
        float distance = Vector3.Distance(transform.position, position);

        if (hungry) {
            Prey();
        } else if (hungry == false && distance > 0.0f) {
            Swim(position, rotation, distance);     
        } 
    }

    private void Prey() {
        float preyDistance = maxDistance;
        Vector3 preyPosition = simulationCentre;
        Vector3 fishPosition;

        for (int i = 0; i < fishBoids.Length; i++) {
            if (Vector3.Distance(fishBoids[i].transform.position, transform.position) < preyDistance &&
                fishBoids[i].tag == activeFishTag) {
                preyDistance = Vector3.Distance(fishBoids[i].transform.position, transform.position);
                preyPosition = fishBoids[i].transform.position;
            }
        }

        fishPosition = preyPosition + preyDistance * fishBoids[fishIndex].GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().AddForce((fishPosition - transform.position) * swimForce);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                             Quaternion.LookRotation(fishPosition - transform.position), preyForce);
        gameObject.GetComponent<Renderer>().material.color = Color.red;
    }

    private void Swim(Vector3 position, Quaternion rotation, float distance) {
        if (distance <= separationDistance) {
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
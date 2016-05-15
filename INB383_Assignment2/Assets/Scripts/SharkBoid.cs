using UnityEngine;
using System.Collections;

public class SharkBoid : Boid {

    public float swimFource;
    private GameObject[] fishBoids;
    private int fishIndex;
    private int hunger;
    private bool isHungry;
    private float hungerDuration;
    private float stepMagnitude;

    protected override void Initialise() {
        boids = null;
        boidIndex = 0;
        cohesionPosition = new Vector3(0.0f, 0.0f, 0.0f);
        swimFource = 0.8f;
        fishIndex = 0;
        isHungry = true;
        hunger = 0;
        hungerDuration = maxDuration;
        stepMagnitude = 5.0f;
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
        if (other.gameObject.tag == "FishBoid" && isHungry) {
            hunger = hunger + 1;
            other.gameObject.tag = "Inactive";
            hungerDuration = Time.time;
        }
    }

    private bool NoBoid() {
        return boids == null || fishBoids == null;
    }

    private void GetBoids() {
        boids = GameObject.FindGameObjectsWithTag("SharkBoid");
        fishBoids = GameObject.FindGameObjectsWithTag("FishBoid");
    } 

    private void CheckHungerStatus() {
        if (Time.time % 10 == 0) {
            hunger = hunger - 1;
        }
        if (hunger >= 3) {
            isHungry = false;
            Color colour = new Color(0.557f, 0.580f, 0.580f, 1.0f);
            gameObject.GetComponent<Renderer>().material.color = colour;
            if (Time.time > hungerDuration + 5.0f) {
                hunger = hunger - 2;
            }
        } else {
            isHungry = true;
        }
    }

    private bool Escape() {
        return Vector3.Distance(simulationCentre, transform.position) > simulationRadius;
    }


    private void SetNewDirection(float magnitude) {
        float step = magnitude * Time.deltaTime;
        Vector3 target = simulationCentre - transform.position;
        Vector3 direction = Vector3.RotateTowards(transform.forward, target, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(direction);
        transform.position = Vector3.MoveTowards(transform.position, simulationCentre, step);
    }

    private void IncrementFishIndex() {
        fishIndex++;
        if (fishIndex >= fishBoids.Length) {
            fishIndex = 0;
        }
    }

    private void IncrementBoidIndex() {
        boidIndex++;
        if (boidIndex >= boids.Length) {
            Vector3 cohesiveForce = (cohesionStrength / Vector3.Distance(cohesionPosition, transform.position)) 
                                  * (cohesionPosition - transform.position);
            GetComponent<Rigidbody>().AddForce(cohesiveForce);
            boidIndex = 0;
            cohesionPosition.Set(0.0f, 0.0f, 0.0f);
        }
    }

    private void InvokeAction() {
        Vector3 position = boids[boidIndex].transform.position;
        Quaternion rotation = boids[boidIndex].transform.rotation;
        float distance = Vector3.Distance(transform.position, position);

        if (isHungry) {
            Prey();
        } else if (isHungry == false && distance > 0.0f) {
            Swim(position, rotation, distance);     
        } 
    }

    private void Prey() {
        float preyDistance = 1000.0f;
        Vector3 preyPosition = simulationCentre;
        Vector3 fishPosition;

        for (int i = 0; i < fishBoids.Length; i++) {
            if (Vector3.Distance(fishBoids[i].transform.position, transform.position) < preyDistance &&
                fishBoids[i].tag == "FishBoid") {
                preyDistance = Vector3.Distance(fishBoids[i].transform.position, transform.position);
                preyPosition = fishBoids[i].transform.position;
            }
        }

        fishPosition = preyPosition + preyDistance * fishBoids[fishIndex].GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().AddForce((fishPosition - transform.position) * swimFource);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                             Quaternion.LookRotation(fishPosition - transform.position), 7.0f);
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
            Wander();
        }   
    }

    private void Wander() {
        Vector3 randomTarget = new Vector3(Random.Range(minRange, maxRange), 
                                           Random.Range(minRange, maxRange), 
                                           Random.Range(minRange, maxRange));
        Vector3 wanderVariation = randomTarget + marker.position;
        Quaternion wanderRotation = Quaternion.LookRotation(wanderVariation);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, wanderRotation, 2.0f);
        GetComponent<Rigidbody>().AddForce(transform.forward * 2.0f);
    }
}

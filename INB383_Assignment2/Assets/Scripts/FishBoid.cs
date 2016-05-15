using UnityEngine;
using System.Collections;

public class FishBoid : Boid {

    public float safeDistance;
    public float swimForce;
    private GameObject[] sharkBoids;
    private int sharkIndex;
    private float inactiveDuration;
    private float stepMagnitude;

    protected override void Initialise() {
        boids = null;
        boidIndex = 0;
        cohesionPosition = new Vector3(0.0f, 0.0f, 0.0f);
        safeDistance = 10.0f;
        swimForce = 0.3f;
        sharkBoids = null;
        sharkIndex = 0;
        inactiveDuration = 100000000.0f;
        stepMagnitude = 10.0f;
    }

    protected override void BoidUpdate() {
        if (boids == null || sharkBoids == null) {
            GetBoids();
        } else {
            CheckBoidStatus();
            if (Vector3.Distance(simulationCentre, transform.position) > simulationRadius) {
                SetNewDirection(stepMagnitude);
                return;
            }
            IncrementSharkIndex();          
            IncrementBoidIndex();
            InvokeAction();
        }
    }

    void OnTriggerEnter(Collider other) {
        if (gameObject.tag == "FishBoid" && other.gameObject.tag == "SharkBoid") {
            inactiveDuration = Time.time;
        }
    }

    private void GetBoids() {
        boids = GameObject.FindGameObjectsWithTag("FishBoid");
        sharkBoids = GameObject.FindGameObjectsWithTag("SharkBoid");
    }

    private void CheckBoidStatus() {
        if (gameObject.tag == "Inactive") {
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
            Color colour = gameObject.GetComponent<Renderer>().material.color;
            colour.a = 0.1f;
            if (Time.time > inactiveDuration + 10.0f) {
                gameObject.tag = "FishBoid";
                colour.a = 1.0f;
            }
            gameObject.GetComponent<Renderer>().material.color = colour;
        }
    }

    private void SetNewDirection(float magnitude) {
        float step = magnitude * Time.deltaTime;
        Vector3 target = simulationCentre - transform.position;
        Vector3 direction = Vector3.RotateTowards(transform.forward, target, step, 0.0f);
        transform.rotation = Quaternion.LookRotation(direction);
        transform.position = Vector3.MoveTowards(transform.position, simulationCentre, step);
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
        float predatorDistance = 1000.0f;

        for (int i = 0; i < sharkBoids.Length; i++) {
            if (Vector3.Distance(sharkBoids[i].transform.position, transform.position) < predatorDistance) {
                predatorPosition = sharkBoids[i].transform.position;
                predatorDistance = Vector3.Distance(sharkBoids[i].transform.position, transform.position);
            }
        }

        if (gameObject.tag == "FishBoid" && safeDistance / predatorDistance > 1) {
            Vector3 sharkPosition;
            gameObject.GetComponent<Renderer>().material.color = Color.yellow;
            sharkPosition = predatorPosition + predatorDistance 
                            * sharkBoids[sharkIndex].GetComponent<Rigidbody>().velocity;
            GetComponent<Rigidbody>().AddForce((transform.position - sharkPosition) * swimForce);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, 
                                Quaternion.LookRotation(transform.position - sharkPosition), 7.0f);
        } else if (gameObject.tag == "FishBoid" && distance > 0.0f) {
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
                Vector3 randomTarget = new Vector3(Random.Range(minRange, maxRange), 
                                                    Random.Range(minRange, maxRange), 
                                                    Random.Range(minRange, maxRange));
                Vector3 wanderVariation = randomTarget + marker.position;
                Quaternion wanderRotation = Quaternion.LookRotation(wanderVariation);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, wanderRotation, 2.0f);
                GetComponent<Rigidbody>().AddForce(transform.forward);
            }
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{

    public float maxSpeed;
    public float maxVision;
    public float vision;
    public int initialHealth;
    public float deathTime;
    public float goodFoodValue;
    public float badFoodValue;
    public float minAttraction;
    public float maxAttraction;
    public float completeMutationChance;
    public float maxSmallMutation;

    private Rigidbody rb;
    private Vector3 desiredVelocity;
    private Vector3 steeringForce;
    private bool placed = false;
    public Renderer rend;
    private float goodFoodAttraction;
    private float badFoodAttraction;

    public float health { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 min = MainManager.Instance.BottomCorner();
        Vector3 max = MainManager.Instance.TopCorner();
        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);
        float z = Random.Range(min.z, max.z);
        transform.position = new Vector3(x, y, z);

        float speed = Random.Range(maxSpeed, maxSpeed);
        rb = GetComponent<Rigidbody>();
        rb.velocity = Random.onUnitSphere * speed;

        placed = true;

        health = initialHealth;
    }

    public void ofRandom()
    {
        vision = Random.Range(0, maxVision);
        goodFoodAttraction = Random.Range(minAttraction, maxAttraction);
        badFoodAttraction = Random.Range(minAttraction, maxAttraction);
    }

    public void ofParent(Creature creature)
    {
        transform.position = creature.gameObject.transform.position;
        if (Random.Range(0f, 1f) < completeMutationChance)
        {
            ofRandom();
            return;
        }
        else
        {
            float attractionRange = maxAttraction - minAttraction;

            vision = creature.vision + maxVision * Random.Range(-maxSmallMutation, maxSmallMutation);
            goodFoodAttraction = creature.goodFoodAttraction + attractionRange * Random.Range(-maxSmallMutation, maxSmallMutation);
            badFoodAttraction = creature.badFoodAttraction + attractionRange * Random.Range(-maxSmallMutation, maxSmallMutation);

            vision = Mathf.Clamp(vision, 0, maxVision);
            goodFoodAttraction = Mathf.Clamp(goodFoodAttraction, minAttraction, maxAttraction);
            badFoodAttraction = Mathf.Clamp(badFoodAttraction, minAttraction, maxAttraction);
        }
    }

    // Update is called once per frame
    void Update()
    {
        seek();
        steer();
        debugDraw();
        transform.LookAt(transform.position + rb.velocity);
        transform.Rotate(0, -90, 0);
        health = Mathf.Clamp(health - initialHealth * (Time.deltaTime / deathTime), 0, initialHealth);
        rend.material.color = Color.Lerp(Color.red, Color.green, health / initialHealth);
        if (health <= 0)
        {
            MainManager.Instance.RemoveCreature(this.gameObject);
        }
    }

    private void debugDraw()
    {
        if (!MainManager.Instance.debug) return;
        Debug.DrawLine(transform.position, transform.position + (rb.velocity.normalized * (goodFoodAttraction * 5)), Color.green);
        Debug.DrawLine(transform.position, transform.position + (rb.velocity.normalized * (badFoodAttraction * 5)), Color.red);
        //Debug.DrawLine(transform.position, transform.position + rb.velocity, Color.white);
        //Debug.DrawLine(transform.position + rb.velocity, transform.position + rb.velocity + steeringForce, Color.yellow);
    }

    private void seek()
    {
        if (!inBounds())
        {
            desiredVelocity = Vector3.zero - transform.position;
            desiredVelocity = desiredVelocity.normalized * maxSpeed;
            return;
        }
        List<GameObject> food = MainManager.Instance.Foods();
        List<GameObject> visibleFood = new List<GameObject>();
        foreach (GameObject f in food)
        {
            float dist = Vector3.Distance(f.transform.position, transform.position);
            if (dist <= vision)
            {
                visibleFood.Add(f);
            }
        }
        if (visibleFood.Count == 0)
        {
            desiredVelocity = rb.velocity;
            return;
        }
        visibleFood.Sort((o1, o2) => Vector3.Distance(o1.transform.position, transform.position).CompareTo(Vector3.Distance(o2.transform.position, transform.position)));
        GameObject closest = visibleFood[0];
        float speed = maxSpeed;
        speed *= closest.GetComponent<Food>().good ? goodFoodAttraction : badFoodAttraction;
        desiredVelocity = (closest.transform.position - transform.position).normalized * speed;
    }

    private void steer()
    {
        steeringForce = desiredVelocity - rb.velocity;
        rb.velocity += steeringForce * Time.deltaTime;
    }

    private bool inBounds()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        Vector3 min = MainManager.Instance.BottomCorner();
        Vector3 max = MainManager.Instance.TopCorner();
        return x >= min.x && x <= max.x && y >= min.y && y <= max.y && z >= min.z && z <= max.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (placed && other.gameObject.CompareTag("Food"))
        {
            Food food = other.gameObject.GetComponent<Food>();
            float nutrition = food.good ? goodFoodValue : badFoodValue;
            health = Mathf.Clamp(health + initialHealth * nutrition, 0, initialHealth);
        }
    }

}

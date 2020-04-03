using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : Singleton<MainManager>
{
    public int numberOfCreatures;
    public int numberOfFood;
    public bool debug = true;
    public float maxFoodSpawnTimer;
    public float maxReproductionTime;
    public float minReproductionHealth;

    public GameObject arena;
    public GameObject creaturePrefab;
    public GameObject foodPrefab;

    private List<GameObject> creatures;
    private List<GameObject> food;

    private float spawnFoodTime;
    private float spawnFoodTimer;

    private float reproductionTime;
    private float reproductionTimer;

    // Start is called before the first frame update
    void Start()
    {
        creatures = new List<GameObject>();
        for (int i = 0; i < numberOfCreatures; i++)
        {
            GameObject creature = GameObject.Instantiate(creaturePrefab);
            creature.GetComponent<Creature>().ofRandom();
            creatures.Add(creature);
        }

        food = new List<GameObject>();
        for (int i = 0; i < numberOfFood; i++)
        {
            food.Add(GameObject.Instantiate(foodPrefab));
        }

        spawnFoodTimer = Random.Range(0, maxFoodSpawnTimer);
        spawnFoodTime = 0;

        reproductionTimer = Random.Range(0, maxReproductionTime);
        reproductionTime = 0;

    }

    // Update is called once per frame
    void Update()
    {
        spawnFoodTime += Time.deltaTime;
        if(spawnFoodTime >= spawnFoodTimer) 
        {
            spawnFoodTime = 0;
            spawnFoodTimer = Random.Range(0, maxFoodSpawnTimer);
            food.Add(GameObject.Instantiate(foodPrefab));
        }

        reproductionTime += Time.deltaTime;
        if (reproductionTime >= reproductionTimer)
        {
            reproductionTime = 0;
            reproductionTimer = Random.Range(0, maxReproductionTime);
            List<GameObject> children = new List<GameObject>();
            foreach(GameObject go in creatures)
            {
                Creature parentCreature = go.GetComponent<Creature>();
                if (parentCreature.health >= minReproductionHealth)
                {
                    GameObject child = GameObject.Instantiate(creaturePrefab);
                    child.GetComponent<Creature>().ofParent(parentCreature);
                    children.Add(child);
                }
            }
            creatures.AddRange(children);
        }

    }

    public Vector3 BottomCorner()
    {
        Renderer renderer = arena.GetComponent<Renderer>();
        return new Vector3(renderer.bounds.min.x, renderer.bounds.min.y, renderer.bounds.min.z);
    }

    public Vector3 TopCorner()
    {
        Renderer renderer = arena.GetComponent<Renderer>();
        return new Vector3(renderer.bounds.max.x, renderer.bounds.max.y, renderer.bounds.max.z);
    }

    public List<GameObject> Foods()
    {
        List<GameObject> clone = new List<GameObject>();
        foreach (GameObject f in food)
        {
            clone.Add(f);
        }
        return clone;
    }

    public void RemoveFood(GameObject gameObject)
    {
        int foodIndex = food.IndexOf(gameObject);
        if (foodIndex >= 0)
        {
            food.RemoveAt(foodIndex);
            Destroy(gameObject);
        }
    }

    public void RemoveCreature(GameObject gameObject)
    {
        int creatureIndex = creatures.IndexOf(gameObject);
        if (creatureIndex >= 0)
        {
            GameObject newFood = GameObject.Instantiate(foodPrefab);
            newFood.transform.position = gameObject.transform.position;
            food.Add(newFood);
            creatures.RemoveAt(creatureIndex);
            Destroy(gameObject);
        }
    }

}

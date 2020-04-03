using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{

    public float goodChance;

    private bool placed = false;
    public bool good { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 min = MainManager.Instance.BottomCorner();
        Vector3 max = MainManager.Instance.TopCorner();
        float x = Random.Range(min.x, max.x);
        float y = Random.Range(min.y, max.y);
        float z = Random.Range(min.z, max.z);
        transform.position = new Vector3(x, y, z);

        good = Random.Range(0f, 1f) < Mathf.Clamp(goodChance, 0f, 1f);

        GetComponent<Renderer>().material.color = good ? Color.green : Color.red;

        placed = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (placed && other.gameObject.CompareTag("Creature"))
        {
            MainManager.Instance.RemoveFood(this.gameObject);
        }
    }

}

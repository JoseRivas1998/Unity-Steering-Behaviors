using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{

    public float distance;
    public float orbitSpeed;

    private Camera camera;
    private float angle;

    // Start is called before the first frame update
    void Start()
    {
        camera = GetComponent<Camera>();
        angle = 0;
    }

    // Update is called once per frame
    void Update()
    {
        angle += orbitSpeed * Time.deltaTime;
        float x = distance * Mathf.Cos(angle);
        float z = distance * Mathf.Sin(angle);
        float y = transform.position.y;
        transform.position = new Vector3(x, y, z);
        camera.transform.LookAt(Vector3.zero);
    }
}

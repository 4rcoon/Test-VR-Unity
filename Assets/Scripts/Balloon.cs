using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] public float timeToBoom;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeToBoom -= Time.deltaTime;
        transform.Translate(Vector3.up*speed*Time.deltaTime);
        if (timeToBoom < 0)
        {
            Destroy(gameObject);
        }
    }
}

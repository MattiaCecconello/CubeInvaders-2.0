using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTrash : MonoBehaviour
{
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(gameObject.transform.rotation.x * speed, gameObject.transform.rotation.y * speed, gameObject.transform.rotation.z * speed);
    }

    private void FixedUpdate()
    {

    }
}

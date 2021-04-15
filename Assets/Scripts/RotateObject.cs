using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Cube Invaders/Rotate Object")]
public class RotateObject : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] Vector3 direction = Vector3.up;
    [SerializeField] float speed = 1;

    void Update()
    {
        transform.Rotate(direction * speed * Time.deltaTime);
    }
}

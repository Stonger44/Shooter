using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSprite : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _rotationRange = 200;

    // Start is called before the first frame update
    void Start()
    {
        _rotationSpeed = Random.Range(-_rotationRange, _rotationRange);
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
    }
}

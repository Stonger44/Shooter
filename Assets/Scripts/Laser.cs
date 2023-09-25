using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed;
    private Vector3 _laserDirection = Vector3.right;

    [SerializeField] private float _boundary = 11f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if (transform.position.x > _boundary)
        {
            Destroy(this.gameObject);
        }
    }

    private void Move()
    {
        transform.Translate(_laserDirection * _speed * Time.deltaTime);
    }
}

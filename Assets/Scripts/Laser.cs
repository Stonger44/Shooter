using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _speed;
    private Vector3 _laserDirection = Vector3.up;

    private const float _boundary = 11f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if (this.transform.position.x > _boundary)
        {
            Destroy(this.gameObject);
        }
    }

    private void Move()
    {
        this.transform.Translate(_laserDirection * _speed * Time.deltaTime);
    }
}

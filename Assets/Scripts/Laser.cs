﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _boundary = 11f;

    [SerializeField] private float _speed = 20f;
    private Vector2 _laserDirection = Vector2.right;

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

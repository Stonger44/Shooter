using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceBomb : MonoBehaviour
{
    // -Move
    // -Detonate if boundary reached
    // -Detonate triggers Wide Explosion Radius
    // -VFX?
    // -Sound?
    [SerializeField] private float _boundary = 10f;
    [SerializeField] private float _speed = 15f;
    [SerializeField] private GameObject[] _blastRadii;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Detonate()
    {

    }

    private void Move()
    {
        if (transform.position.x > _boundary)
        {
            Detonate();
        }
        else
        {
            transform.Translate(Vector3.right * _speed * Time.deltaTime);
        }
    }
}

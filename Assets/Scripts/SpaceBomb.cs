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
    private const string _enemyTag = "Enemy";

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
        Debug.Log("SpaceBomb Detonated!");
        Destroy(this.gameObject);
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == _enemyTag)
        {
            Detonate();
        }
    }
}

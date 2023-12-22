using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRadar : MonoBehaviour
{
    private const string _enemyTag = "Enemy";

    [SerializeField] private MissilePlayer _missilePlayer;

    // Start is called before the first frame update
    void Start()
    {
        if (_missilePlayer == null)
        {
            Debug.LogError("MissilePlayer is null!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == _enemyTag)
        {

        }
    }
}

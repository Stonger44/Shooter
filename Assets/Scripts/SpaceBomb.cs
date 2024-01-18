using System;
using System.Collections;
using UnityEngine;

public class SpaceBomb : MonoBehaviour
{
    private const string _enemyTag = "Enemy";
    private CircleCollider2D _collider;

    [SerializeField] private float _boundary = 10f;
    [SerializeField] private float _speed = 15f;
    [SerializeField] private GameObject _spaceBombSprite;
    [SerializeField] private GameObject[] _blastRadii;
    [SerializeField] private GameObject _blastZone;

    public static event Action onExplosion;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<CircleCollider2D>();
        if (_collider == null)
        {
            Debug.Log("SpaceBomb Collider is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == _enemyTag)
        {
            _speed = 0f;
            Detonate();
        }
    }

    private void Detonate()
    {
        _spaceBombSprite.SetActive(false);
        _collider.enabled = false;
        StartCoroutine(SpaceBombBlast());
        onExplosion?.Invoke();
        Destroy(this.gameObject, 0.25f);
    }

    private IEnumerator SpaceBombBlast()
    {
        for (int i = 0;  i < _blastRadii.Length; i++)
        {
            _blastRadii[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.02f);
        }
        _blastZone.SetActive(true);
    }
}

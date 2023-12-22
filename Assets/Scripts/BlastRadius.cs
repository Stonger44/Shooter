using System.Collections;
using UnityEngine;

public class BlastRadius : MonoBehaviour
{
    private const string _playerTag = "Player";
    private string _otherTag = string.Empty;

    private Player _player;
    private CircleCollider2D _collider;
    [SerializeField] private GameObject _missileExplosion;

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag(_playerTag).GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player is null!");
        }
        _collider = GetComponent<CircleCollider2D>();
        if (_collider == null)
        {
            Debug.LogError("Collider is null!");
        }
        if (_missileExplosion == null)
        {
            Debug.LogError("MissileExplosion is null!");
        }

        StartCoroutine(DissipateExplosion());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _otherTag = other.tag;

        if (_otherTag == _playerTag)
        {
            _player.Damage();
        }
    }

    private IEnumerator DissipateExplosion()
    {
        yield return new WaitForSeconds(0.5f);
        _collider.enabled = false;
        yield return new WaitForSeconds(2.2f);
        Destroy(this.gameObject);
    }

}

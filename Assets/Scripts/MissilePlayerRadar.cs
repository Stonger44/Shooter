using UnityEngine;

public class MissilePlayerRadar : MonoBehaviour
{
    private const string _enemyTag = "Enemy";
    private const string _enemyLeaderTag = "EnemyLeader";
    private const string _shieldGeneratorTag = "ShieldGenerator";

    [SerializeField] private MissilePlayer _missilePlayer;
    [SerializeField] private Collider2D _radarCollider;

    // Start is called before the first frame update
    void Start()
    {
        if (_missilePlayer == null)
        {
            Debug.LogError("MissilePlayer is null!");
        }
        if (_radarCollider == null)
        {
            Debug.LogError("Collider is null!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(_shieldGeneratorTag))
        {
            _missilePlayer.SetTarget(other.gameObject);
            _radarCollider.enabled = false;
        }
        else if (other.CompareTag(_enemyTag))
        {
            _missilePlayer.SetTarget(other.gameObject);
            _radarCollider.enabled = false;
        }
    }
}

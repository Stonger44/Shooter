using UnityEngine;

public class EnemyRadar : MonoBehaviour
{
    private const string _playerTag = "Player";
    private const string _laserTag = "Laser";
    private const string _tripleShotTag = "TripleShot";

    [SerializeField] private EnemyTrooper _enemyTrooper;

    // Start is called before the first frame update
    void Start()
    {
        if (_enemyTrooper == null)
        {
            Debug.LogError("EnemyTrooper is null!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == _playerTag)
        {
            _enemyTrooper.IsRamming(true);
        }
        else if (other.tag == _laserTag || other.tag == _tripleShotTag)
        {
            _enemyTrooper.ShouldJink();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == _playerTag)
        {
            _enemyTrooper.IsRamming(false);
        }
    }
}

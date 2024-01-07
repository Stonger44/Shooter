using UnityEngine;

public class Laser : MonoBehaviour
{
    // 0: Player Laser 
    // 1: Enemy Laser
    [SerializeField] private int _laserId;
    [SerializeField] private float _xBoundary;
    [SerializeField] private float _yBoundary;
    [SerializeField] private float _speed;
    private Vector2 _laserDirection;

    // Start is called before the first frame update
    void Start()
    {
        if (_laserId == 0)
        {
            _laserDirection = Vector2.right;
        }
        else if (_laserId == 1)
        {
            _laserDirection = Vector2.left;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(_laserDirection * _speed * Time.deltaTime);

        DestroyIfOutOfBounds();
    }

    private void DestroyIfOutOfBounds()
    {
        if (_laserId == 0)
        {
            if (transform.position.x > _xBoundary)
            {
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                Destroy(this.gameObject);
            }
        }
        else if (_laserId == 1)
        {
            if (transform.position.x < _xBoundary || IsLaserOutOf_Y_Bounds())
            {
                if (transform.parent != null)
                {
                    Destroy(transform.parent.gameObject);
                }
                Destroy(this.gameObject);
            }
        }
    }

    private bool IsLaserOutOf_Y_Bounds()
    {
        return transform.position.y > 0 && transform.position.y > _yBoundary || transform.position.y < 0 && transform.position.y < -_yBoundary;
    }
}

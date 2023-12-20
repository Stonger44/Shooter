using System.Collections;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private Vector3 _defaultPosition = new Vector3(0, 0, -10);
    [SerializeField] private float _cameraShakeSeconds = 0.075f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = _defaultPosition;
    }

    public IEnumerator CameraShake()
    {
        float xPosition;
        float yPosition;
        float zPosition = -9f;
        for (int i = 0; i < 4; i++)
        {
            xPosition = Random.Range(-0.5f, 0.5f);
            yPosition = Random.Range(-0.5f, 0.5f);
            transform.position = new Vector3(xPosition, yPosition, zPosition);

            yield return new WaitForSeconds(_cameraShakeSeconds);
        }
        transform.position = _defaultPosition;
    }

}

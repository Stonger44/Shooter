using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _score;
    [SerializeField] private Image _lives;
    [SerializeField] private Sprite[] _liveSpriteArray;

    // Start is called before the first frame update
    void Start()
    {
        _score.text = $"Score: 0";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateScore(int score)
    {
        _score.text = $"Score: {score}";
    }

    public void UpdateLives(int lives)
    {
        _lives.sprite = _liveSpriteArray[lives];
    }
}

using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfo : MonoBehaviour
{

    [SerializeField]
    public int _currentHealth;

    [SerializeField]
    public int _maxHealth;

    [SerializeField]
    Slider _HPSlider;

    [SerializeField]
    TextMeshProUGUI _HPText;

    void Start()
    {
        //kan også kaldes hvis en modstander på en eller anden måde får mere max liv
        UpdateMaxBarValues();
    }

    void Update()
    {
        _HPSlider.value = _currentHealth;
        _HPText.text = _currentHealth.ToString() + "/" + _maxHealth.ToString();
    }

    public void UpdateMaxBarValues()
    {
        _HPSlider.maxValue = _maxHealth;
    }
}

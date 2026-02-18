using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CombatChrInfo : MonoBehaviour
{
    //værdier som alle karakterne har
    [SerializeField]
    public int _currentHealth;
    [SerializeField]
    public int _maxHealth;
    [SerializeField]
    public int _currentAP;
    [SerializeField]
    public int _maxAP;

    [SerializeField]
    public int _position;

    //slide bars til at vise karakterens liv og AP
    [SerializeField]
    Slider _HPSlider;

    [SerializeField]
    Slider _APSlider;

    //texten inde i de tidligere slide bars
    [SerializeField]
    TextMeshProUGUI _HPText;

    [SerializeField]
    TextMeshProUGUI _APText;



    //hvilke angreb karakteren har på sig
    public List<Attack> _equipedAttacks;



    void Start()
    {
        UpdateMaxBarValues();
    }

   
    void Update()
    {
        //updatere HP og AP bars
        _HPSlider.value = _currentHealth;
        _APSlider.value = _currentAP;
        _HPText.text = _currentHealth.ToString() +"/" + _maxHealth.ToString();
        _APText.text = _currentAP.ToString() + "/" + _maxAP.ToString();
    }

    //updatere max HP og AP værdier, hvis nu i fremtiden der et angreb der påvirker dem
    public void UpdateMaxBarValues() 
    {
        _HPSlider.maxValue = _maxHealth;
        _APSlider.maxValue = _maxAP;
    }
}

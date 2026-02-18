using System;
using TMPro;
using UnityEngine;

public class NumberBehavior : MonoBehaviour
{
    int numberValue;

    TextMeshPro numberInGame;

    Enum Type;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        numberInGame = GetComponent<TextMeshPro>();
        numberInGame.text = numberValue.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        
    }
}

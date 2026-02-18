using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Chr", menuName = "Scriptable Objects/Chr")]
public class Chr : ScriptableObject
{
    public string _chrName;
    public int _positionInRow;
    public List<Attack> _attacks;
    public List<Attack> _equipedAttacks;
}

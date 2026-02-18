using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Attack", menuName = "Scriptable Objects/Attack")]
public class Attack : ScriptableObject
{
    public string _attackName;
    
    public Sprite _image;
    
    public int DMG;
    
    public int APCost;
    
    public int APGain;
    
    public int HPGain;
    
    //hvor langt tid animationen knyttet til angrebet tager
    public double Duration;

    //hvor langt inde i anmationen skaden/angrebet faktisk skal ske
    public double EffectAt;

    //til hvilken animation skal spille når angrebet er brugt, for eksempel et stort angreb eller heal (animationerne eksistere dog ikke i nu)
    public int Value;
}

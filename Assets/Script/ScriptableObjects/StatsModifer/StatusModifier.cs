using UnityEngine;

[CreateAssetMenu(fileName = "StatusModifier", menuName = "Scriptable Objects/StatusModifier")]
public class StatusModifier : ScriptableObject
{
    public StatusModifierName modifierName;
    public int turnLeft;
    public enum ModifierType{DEFENSIVE = 0, DOT = 1, BUFF = 2 ,DEBUFF = 3}
    public ModifierType modifierType;
    
    [Header("Stat Modifier")]
    [Range(0,100)]
    [Tooltip("Percentage")]
    public int damageReduction;
}

using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterStats", menuName = "Scriptable Objects/Stats")]
public class CharacterStats : ScriptableObject
{
    #pragma warning disable 0649
    #pragma warning disable 0414
    [Header("Character Sprite And Animation")]
    public Sprite characterSprite;
    public AnimatorController characterAnimationController;
    public EnemyBehavior enemyBehavior;

    [Header("Character Stats")]
    public string characterName;
    public int health = 100;
    public int SP = 10;
    public int ATK = 10;
    public int DEF = 10 ;
    public int AGI = 10;


}
    

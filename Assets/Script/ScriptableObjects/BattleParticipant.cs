using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleParticipant", menuName = "Scriptable Objects/BattleParticipant")]
public class BattleParticipant : ScriptableObject
{
    //PositionFromTopToBottom
    public List<CharacterStats> allyCharacter;
    public List<CharacterStats> enemyCharacter;

}

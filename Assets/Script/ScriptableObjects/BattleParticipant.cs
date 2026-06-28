using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleParticipant", menuName = "Scriptable Objects/BattleParticipant")]
public class BattleParticipant : ScriptableObject
{
    public List<CharacterStats> allyCharacter;
    public List<CharacterStats> enemyCharacter;

}

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyBehavior", menuName = "Scriptable Objects/EnemyBehavior")]
public class EnemyBehavior : ScriptableObject
{
    [Range(0,100)]
    [Tooltip("the more higher the more smart")]
    public int enemySmartness;
    public ActorHandler target;
    private BattleManager bm;

    public void ExecuteBehavior()
    {
        if(BattleManager.Instance == null){
            Debug.LogError("Missing Battle Manager");
            return;
        }

        bm = BattleManager.Instance;

        int rollDecision = UnityEngine.Random.Range(0,101);
        if (rollDecision <= enemySmartness)
        {
            ExecuteSmartTargeting();
           
        }
        else
        {
            ExectueNormalTargeting();
            
        }
        bm.currentTurn.Attack(target,()=>{});
    }

    public void ExecuteSmartTargeting()
    {
        List<ActorHandler> lowestAllyHealth = bm.allyList.OrderBy(actor => actor.actorStats.health).ToList();
        target = lowestAllyHealth[0];
    }

    public void ExectueNormalTargeting()
    {
        int randomIndex = UnityEngine.Random.Range(0,bm.allyList.Count);
        target = bm.allyList[randomIndex];
    }
}

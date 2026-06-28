using System.Collections.Generic;
using UnityEngine;

public class TrainingDummyBehavior : ActorBehavior
{
    // Select Target --> Decide What to do --> End Turn
    private List<ActorHandler> allyList;
    private ActorHandler currentActor;
    public ActorHandler target;
    public override void StartBehavior()
    
    {
        currentActor = BattleManager.Instance.currentTurn;
        SearchTarget();
        Attack(target,currentActor.actorStats.ATK);
        BattleManager.Instance.EndTurn();
    }


    public void SearchTarget()
    {
        for (int i = 0; i < BattleManager.Instance.remainingActor.Count; i++)
        {
            if (BattleManager.Instance.remainingActor[i].actorAligment == ActorAligment.Ally)
            {
                allyList.Add(BattleManager.Instance.remainingActor[i]);
            }
        }

        int maxWeightDecision = 100;
        int goodWeightDecision = 20;
        // 20% get good decision
        int decisionRoll = Random.Range(0,100);
        if (decisionRoll >= maxWeightDecision - goodWeightDecision)
        {
          allyList.Sort((a,b)=> a.actorStats.health.CompareTo(b.actorStats.health));
          target = allyList[0];
        }
        else
        {
            int randomIndex = Random.Range(0,allyList.Count);
            target = allyList[randomIndex];
        }
    }
    
}

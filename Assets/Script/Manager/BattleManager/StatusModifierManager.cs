using System.Collections.Generic;
using UnityEngine;

public enum StatusModifierName{DEFEND = 0}

public class StatusModifierManager : MonoBehaviour
{
    public static StatusModifierManager Instance{get;private set;}
    public List<StatusModifier> statusModifiers;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ApplyStatusModifier(ActorHandler target, StatusModifierName statusModifierList, int turns){
        if (statusModifierList == StatusModifierName.DEFEND)
        {
            StatusModifier targetModifier = CheckModifierIsPresent(target,StatusModifierName.DEFEND);
            if (targetModifier != null)
            {
                targetModifier.turnLeft += turns;
                return;
            }

            StatusModifier newInstance = ScriptableObject.Instantiate(statusModifiers[0]);
            newInstance.turnLeft += turns;
            target.statusModifierList.Add(newInstance);
        }
        
    }
    private StatusModifier CheckModifierIsPresent(ActorHandler target, StatusModifierName modifierName)
    {
        foreach (StatusModifier modifier in target.statusModifierList)
        {
            if(modifier.modifierName == modifierName)
            {
                return modifier;
            }
        }
        return null;
    }
    public void Test()
    {
        Debug.Log("man");
    }
}

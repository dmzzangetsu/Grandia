using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public enum ActorAligment
    {
        Ally = 0,
        Enemy = 1
    }
public class ActorHandler : MonoBehaviour
{
    [SerializeField] private GameObject actorVisual;
    [SerializeField] private GameObject selectedMarker;
    [SerializeField] private ActionUI actionUI;
    public ActorBehavior actorBehavior;
    public ActorAligment actorAligment;
    public CharacterStats actorDefaultStats;
    public CharacterStats actorStats;
    private Animator actorAnimator;
    private SpriteRenderer actorSprite;
    private int defaultTurnSpeed = 100;
    public int currentTurnSpeed = 100;
    private bool isSelected = false;

    [Header("StatusModifier")]
    public List<StatusModifier> statusModifierList;
    

    void Awake()
    {
        actorAnimator = actorVisual.GetComponent<Animator>();
        actorSprite = actorVisual.GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (actorAligment == ActorAligment.Ally){
        actionUI.attackButton.onClick.AddListener(OnAttackPressed);
        actionUI.defendButton.onClick.AddListener(OnDefendPressed);
        }
    }

    void OnAttackPressed()
    {
        BattleManager.Instance.AttackActor();
    }

    void OnDefendPressed()
    {
        BattleManager.Instance.DefendActor();
    }

    public void InitializeActors()
    {
        if (actorDefaultStats == null)
        {
            Debug.LogError(gameObject.name + "ActorDefault Not set and null");
        }
        else
        {
            actorStats = ScriptableObject.Instantiate(actorDefaultStats);
        }
        if (actorStats.characterAnimationController == null || actorStats.characterSprite == null)
        {
            Debug.LogError("maybe your character sprite or animation controller not set in:"+gameObject.name);
        }
        else
        {
            actorAnimator.runtimeAnimatorController = actorStats.characterAnimationController;
            actorSprite.sprite = actorStats.characterSprite;
        }

        //Simple Turn speed using Agi
        defaultTurnSpeed -= actorStats.AGI;
        currentTurnSpeed = defaultTurnSpeed;
    }
    
    public void ResetTurnSpeed()
    {
        currentTurnSpeed = defaultTurnSpeed;
    }

    public void ShowActionUI()
    {
        if (actorAligment == ActorAligment.Ally)
        {
        actionUI.gameObject.SetActive(true);
        }
    }

    public void HideActionUI()
    {
        if (actorAligment == ActorAligment.Ally)
        {
        actionUI.gameObject.SetActive(false);
        }
    }

    public void ActorSelected()
    {
        isSelected = true;
        selectedMarker.SetActive(true);
    }
    
    public void ActorDeselected()
    {
        isSelected = false;
        selectedMarker.SetActive(false);
    }

    public void GetHit(int damage)
    {
        int finalDamage = damage - actorStats.DEF;

        if(statusModifierList != null)
        {
            foreach (StatusModifier modifier in statusModifierList)
            {
                if (modifier.modifierName == StatusModifierName.DEFEND)
                {
                    finalDamage = finalDamage * modifier.damageReduction/100;
                    break;
                }
            }
        }
        if (finalDamage <= 0)
        {
            finalDamage = 1;
        }
        actorStats.health -= finalDamage;
        Debug.Log(actorStats.characterName + " Taken" + finalDamage +"Damage");
        if (actorStats.health <= 0)
        {
            Dead();
        }
    }
    public void Dead()
    {
         BattleManager.Instance.DisableActors(this);
    }
}

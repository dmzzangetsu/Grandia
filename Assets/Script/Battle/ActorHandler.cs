using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fungus;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem.Composites;
using Random = UnityEngine.Random;

public enum ActorAligmentEnum
    {
        Ally = 0,
        Enemy = 1
    }
public class ActorHandler : MonoBehaviour
{
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    public AudioClip slideSound;
    public AudioClip hitSound;
    public AudioSource sfx;
    private enum State{Idle,Sliding,Busy,FinishAct}
    public enum ChoosenAction{Attack,Defend}
    public enum ChooseTargetType{Ally,Enemy}
    public ChooseTargetType targetType;
    public ChoosenAction choosenAction;
    private State state;
    private Vector3 targetSlidePosition;
    private Action OnSlideComplete; 

    [SerializeField] private GameObject actorVisual;
    [SerializeField] private GameObject selectedMarker;
    [SerializeField] private GameObject attackedRange;
    [SerializeField] private ActionUI actionUI;
    [SerializeField] private HealthContainer healthBar;

    public ActorAligmentEnum actorAligment;
    public CharacterStats defaultStats;
    public CharacterStats actorStats;
    private Animator actorAnimator;
    private SpriteRenderer actorSprite;
    
    public int currentActionSpeed = 100;
    public EnemyBehavior enemyBehavior;

    [Header("StatusModifier")]
    public List<StatusModifier> statusModifierList;


    void Awake()
    {
        actorAnimator = actorVisual.GetComponent<Animator>();
        actorSprite = actorVisual.GetComponent<SpriteRenderer>();
        state = State.Idle;
    }
    void Start()
    {
        if (actionUI != null)
        {
        actionUI.attackButton.onClick.AddListener(OnAttackClick);
        actionUI.defendButton.onClick.AddListener(OnDefendClick);
        }

    }

    private void OnAttackClick (){
        if(state == State.Idle){
        SetUIVisibilitiy(false);
        BattleManager bm = BattleManager.Instance;
        choosenAction = ChoosenAction.Attack;
        targetType = ChooseTargetType.Enemy;
        bm.InitiateSelection();
        // Attack(bm.targetActor,()=>{bm.EndTurn();});
        }
    }
    
    private void OnDefendClick()
    {
        choosenAction = ChoosenAction.Defend;
        StatusModifierManager.Instance.ApplyStatusModifier(this,StatusModifierName.DEFEND,1);
        BattleManager.Instance.EndTurn();

    }

    void Update()
    {
        healthBar.UpdateActionBar(currentActionSpeed);
        healthBar.UpdateHealthBar(actorStats.health);
        switch (state)
        {
            
            case State.Idle:
                break;
            case State.Busy:
                break;
            case State.Sliding:
                float slideSpeed = 20f;
                actorVisual.transform.position += slideSpeed * Time.deltaTime * (targetSlidePosition - GetVisualPosition());
                float reachedDistance = 1f;
                if(Vector3.Distance(GetVisualPosition(),targetSlidePosition) < reachedDistance)
                {
                    actorVisual.transform.position = targetSlidePosition;

                    OnSlideComplete();
                }
             break;
                    
                }
    }
    public void SetupActor(CharacterStats newCharStats)
    {
        actorStats = ScriptableObject.Instantiate(newCharStats);
        actorAnimator.runtimeAnimatorController = actorStats.characterAnimationController;
        actorSprite.sprite = actorStats.characterSprite;
        healthBar.SetMaxHealth(actorStats.health);

        ResetCharacterSpeed();
        if(newCharStats.enemyBehavior != null)
        {
            PopulateEnemyBehavior(newCharStats);
        }
    }

    public void PopulateEnemyBehavior(CharacterStats newCharStats)
    {
        enemyBehavior = ScriptableObject.Instantiate(newCharStats.enemyBehavior);
        
    }
    public Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }

    public Vector3 GetVisualPosition()
    {
        return actorVisual.transform.position;
    }
    public Vector3 GetAttackedRangePosition()
    {
        return attackedRange.transform.position;
    }

    public void ResetCharacterSpeed()
    {
        currentActionSpeed = 0;
    }

    public void SetActionSpeed()
    {
        currentActionSpeed += actorStats.AGI + Random.Range(0,5);
    }
    
    public void Attack(ActorHandler target, Action OnAttackComplete)
    {
        Vector3 originalPos = GetPosition();
       
        SlideToPosition(target.GetAttackedRangePosition(), ()=>
        {
            state = State.Busy;
                PlayAttackAnimation(() =>
                {
                    GetHit(target,actorStats.ATK);
                SlideToPosition(originalPos, () =>
                {
                    state = State.Idle;
                });
            });

        });
        
        OnAttackComplete();
        
    }

    public IEnumerator BreakRest(float time)
    {
        yield return new WaitForSecondsRealtime(time);
    }
    public void PlayAudio(AudioClip newsfx)
    {
        sfx.clip = newsfx;
        sfx.Play();
    }
    public void SlideToPosition(Vector3 slideTarget, Action onSlideComplete)
    {
        PlayAudio(slideSound);
        this.targetSlidePosition = slideTarget;
        this.OnSlideComplete = onSlideComplete;
        state = State.Sliding;
    }

    private void PlayAttackAnimation(Action onAttackAnimationComplete)
    {
       actorAnimator.Play(AttackHash);
       PlayAudio(hitSound);
        StartCoroutine(WaitForAnimation("Attack", () =>
        {
            onAttackAnimationComplete();
        }));
    }
    private IEnumerator WaitForAnimation(string stateName, Action onAnimationComplete)
    {
        yield return null;

        AnimatorStateInfo stateInfo = actorAnimator.GetCurrentAnimatorStateInfo(0);

        while (stateInfo.IsName(stateName)&& stateInfo.normalizedTime < 1.0f )
        {
            stateInfo = actorAnimator.GetCurrentAnimatorStateInfo(0);
            yield return null;
            
        }
        onAnimationComplete();
        
    }

    public void GetHit(ActorHandler target, int damage)
    {
        foreach (StatusModifier statusModifier in statusModifierList)
        {
            if (statusModifier.modifierName == StatusModifierName.DEFEND)
            {
                damage -= damage * statusModifier.damageReduction / 100;
            }
        }

        float randomModifier = Random.Range(0.05f,0.1f);
        int finalDamage = (int)(damage + (damage * randomModifier));
        finalDamage -= target.actorStats.DEF;
        if (finalDamage <= 0)
        {
            finalDamage = 1;
        }
        target.actorStats.health -= finalDamage;
        
        if (target.actorStats.health <= 0)
        {
            target.healthBar.gameObject.SetActive(false);
            Dead(target);
        }
    
    }

    public void Dead(ActorHandler target)
    {
        BattleManager.Instance.OnActorDead(target);
    }

    public void SetMarkerVisibility(bool visibility)
    {
        selectedMarker.SetActive(visibility);
    }

    public void SetUIVisibilitiy(bool visibility)
    {
         if (actionUI != null)
        {
            actionUI.gameObject.SetActive(visibility);
        }
    }

}

using System;
using System.Collections.Generic;
using Fungus;
using MoonSharp.Interpreter;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using Random=UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    public enum ActionList{ATTACK=0,DEFEND=1,GIVEBUFF=2,GIVEDEBUFF=3}
    private ActionList actorAction;

    public static BattleManager Instance{get;private set;}
    [Header("InputSysyem")]
    public PlayerInput battleInput;
    public bool isChoosing = false;
    [SerializeField] private int selectionIndex = 0;
    public ActorHandler selectedEnemy;

    [Header("Actor Handler")]
    public List<ActorHandler> allyActorHandlers;
    public List<ActorHandler> enemyActorHandlers;
    public BattleParticipant battleParticipant;
    [Header("Remaining Participant")]
    public List<ActorHandler> remainingActor;
    public List<ActorHandler> actorTurns;

    public ActorHandler currentTurn;
    [Header("Battle UI")]
    public TextMeshProUGUI turnLabel;
    
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
    }
    void Start()
    {
        // battleInput.GetComponent<PlayerInput>();
        battleInput.actions.Disable();
        StartBattle();
    }

    public void StartBattle()
    {
        if(battleParticipant == null)
        {
            Debug.LogError("You Forgot to set battleParticipant");
            return;
        }        
        SetBattleActors();
        
        InitiateTurn();
    }

    public void InitiateSelectEnemy()
    {
        battleInput.actions.Enable();
        isChoosing = true;
        
        for (int i = 0; i < remainingActor.Count; i++)
        {
            if (remainingActor[i].actorAligment == ActorAligment.Enemy)
            {
                selectedEnemy = remainingActor[i];
                selectionIndex = i;
                selectedEnemy.ActorSelected();
                return;
            }
        }
    }


    public void AttackActor()
    {
        actorAction = ActionList.ATTACK;
        InitiateSelectEnemy();
    }

    public void DefendActor()
    {
        StatusModifierManager.Instance.ApplyStatusModifier(currentTurn,StatusModifierName.DEFEND,1);
        EndTurn();
    }

    void OnMoveUp()
    {
    
        selectionIndex--;
         if (selectionIndex < 0)
        {
            selectionIndex = remainingActor.Count - 1;
        }

        if (remainingActor[selectionIndex].actorAligment == ActorAligment.Ally)
        {
            OnMoveUp();
        }
        else
        {
            SetEnemy(selectionIndex);
        }
    }

    void OnMoveDown()
    {
        selectionIndex++;
        if (selectionIndex >= remainingActor.Count)
        {
            selectionIndex = 0;
        }
        if (remainingActor[selectionIndex].actorAligment == ActorAligment.Ally)
        {
            OnMoveDown();
        }
        else
        {
            SetEnemy(selectionIndex);
        }
        
    }

    void OnAcceptSelection()
    {
        if (isChoosing)
        {
        isChoosing = false;
        selectedEnemy.ActorDeselected();
        battleInput.actions.Disable();
        }

        if (actorAction == ActionList.ATTACK)
        {
            selectedEnemy.GetHit(currentTurn.actorStats.ATK);
        }
        EndTurn();
    }

    private void SetEnemy(int Enemyindex)
    {
        if (selectedEnemy == remainingActor[Enemyindex])
        {
            return;
        }
        selectedEnemy.ActorDeselected();
        selectedEnemy = remainingActor[Enemyindex];
        selectedEnemy.ActorSelected();
    }


    public void InitiateTurn()
    {   
        if (isChoosing){
            EndTurn();
        }
        currentTurn = actorTurns[0];
        turnLabel.SetText(currentTurn.actorStats.characterName + " Turn");
        currentTurn.ShowActionUI();
        if (currentTurn.actorAligment == ActorAligment.Enemy)
        {
            EnemyAttackTarget();
        }
        else
        {
            currentTurn.ShowActionUI();
        }
    }
    // public void SetInitialTurns()
    // {
    //     foreach (ActorHandler actor in allyActorHandlers)
    //     {
    //         actorTurns.Add(actor);
    //     }

    //     foreach (ActorHandler actor in enemyActorHandlers)
    //     {
    //         actorTurns.Add(actor);
    //     }
    //     SortTurnBySpeed();
    // }

    public void SortTurnBySpeed()
    {
        actorTurns.Sort((a,b) => a.currentTurnSpeed.CompareTo(b.currentTurnSpeed));
    }

    void SetBattleActors()
    {
        for (int i = 0; i < battleParticipant.allyCharacter.Count; i++)
        {
            allyActorHandlers[i].gameObject.SetActive(true);
            allyActorHandlers[i].actorDefaultStats = battleParticipant.allyCharacter[i];
            allyActorHandlers[i].InitializeActors();
            remainingActor.Add(allyActorHandlers[i]);
        }
        for (int i = 0; i < battleParticipant.enemyCharacter.Count; i++)
        {
            enemyActorHandlers[i].gameObject.SetActive(true);
            enemyActorHandlers[i].actorDefaultStats = battleParticipant.enemyCharacter[i];
            enemyActorHandlers[i].InitializeActors();
            remainingActor.Add(enemyActorHandlers[i]);
        }
        actorTurns = new List<ActorHandler>(remainingActor);
        SortTurnBySpeed();
    }

    public void DisableActors(ActorHandler actorHandler)
    {
        remainingActor.Remove(actorHandler);
        actorTurns.Remove(actorHandler);
        actorHandler.gameObject.SetActive(false);
    }

    public void EndTurn()
    {
        ActorHandler tempActorHandler = actorTurns[0];
        for (int i = 1; i < actorTurns.Count; i++)
        {
            actorTurns[i].currentTurnSpeed -= actorTurns[0].currentTurnSpeed;
        }
        currentTurn.HideActionUI();
        currentTurn.ResetTurnSpeed();
        UpdateStatModifier();
        actorTurns.Add(tempActorHandler);
        actorTurns.RemoveAt(0);
        SortTurnBySpeed();
        InitiateTurn();

    }
    // public void GetTurnsIndex()
    // {
    //     for (int i = 0; i < remainingActor.Count; i++)
    //     {
    //         if(actorTurns[0].name == remainingActor[i].name)
    //         {
    //             actorTurns.Add(remainingActor[i]);
    //             actorTurns.RemoveAt(0);
    //         }            
    //     }
    // }
    public void UpdateStatModifier()
    {
        if (currentTurn.statusModifierList == null)
        {
            return;
        }

        for (int i = 0; i < currentTurn.statusModifierList.Count; i++)
        {
            currentTurn.statusModifierList[i].turnLeft--;
            if (currentTurn.statusModifierList[i].turnLeft <= 0)
            {
                currentTurn.statusModifierList.RemoveAt(i);
            }
        }
    }
    public void DebugAddModifier()
    {
        StatusModifierManager.Instance.ApplyStatusModifier(allyActorHandlers[0],StatusModifierName.DEFEND,2);
    }

    public void EnemyAttackTarget()
    {
        // List<ActorHandler> allyActorList = new List<ActorHandler>();
        // ActorHandler target;

        // for (int i = 0; i < remainingActor.Count; i++)
        // {
        //     if (remainingActor[i].actorAligment == ActorAligment.Ally)
        //     {
        //         allyActorList.Add(remainingActor[i]);
        //     }
        // }
        
        // int maxWeightDecision = 100;
        // int goodWeightDecision = 20;
        // // 20% get good decision
        // int decisionRoll = Random.Range(0,100);
        // if (decisionRoll >= maxWeightDecision - goodWeightDecision)
        // {
        //   allyActorList.Sort((a,b)=> a.actorStats.health.CompareTo(b.actorStats.health));
        //   target = allyActorList[0];
        // }
        // else
        // {

        //     int randomIndex = Random.Range(0,allyActorList.Count);
        //     target = allyActorList[randomIndex];
        // }
        // target.GetHit(currentTurn.actorStats.ATK);
        EndTurn();
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public enum State{CHOOSEACTION,ENEMYTURN,BUSY,SETTURN,PAUSED,ENDTURN, WINSTATE}
    public static BattleManager Instance{get; private set;}
    
    private int selectionIndex = 0 ;
    private bool isCharacterSelected= true;
    private readonly int actionSpeedTreshold = 500;
    [SerializeField] private PlayerInput playerInput;
    [HideInInspector]public State state;
    public List<ActorHandler> allyList;
    public List<ActorHandler> enemyList;
    public List<ActorHandler> actors;
    public List<ActorHandler> actorsHolders;
    public BattleParticipant battleParticipant;
    public ActorHandler currentTurn;
    public ActorHandler currentTarget;
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
        playerInput.actions.Disable();
        SetupActor();
        actors = new List<ActorHandler>(actorsHolders);
        allyList = actors.Where<ActorHandler>(a => a.actorAligment==ActorAligmentEnum.Ally).ToList();
        enemyList = actors.Where<ActorHandler>(a => a.actorAligment==ActorAligmentEnum.Enemy).ToList();
        currentTarget = actors[3];
        state = State.SETTURN;
    }

    void Update()
    {
        Keyboard currentKeyboard = Keyboard.current;
        if (currentKeyboard == null) return;

        switch (state)
        {
            case State.SETTURN:

                for (int i = 0; i < actors.Count; i++)
                {
                    actors[i].SetActionSpeed();
                    if(actors[i].currentActionSpeed > actionSpeedTreshold)
                    {
                        state = State.PAUSED;
                        break;
                    }                   
                }

            break;

            case State.PAUSED:
                state = State.BUSY;
                List<ActorHandler> actorSpeedExceedThreshold = actors.Where(actors=>actors.currentActionSpeed > 500).ToList();
                actorSpeedExceedThreshold[0].ResetCharacterSpeed();
                SelectCurrentActor(actorSpeedExceedThreshold[0]);

                if (currentTurn.actorAligment == ActorAligmentEnum.Enemy)
                {
                    state = State.ENEMYTURN;
                }
                else{
                    state = State.CHOOSEACTION;
                }


            break;

            case State.ENEMYTURN:
                state = State.BUSY;
                StartCoroutine(ExecuteEnemyBehavior());
                
            break;




        }
    }
    


    public void InitiateSelection()
    {
        switch (currentTurn.targetType)
        {
            case ActorHandler.ChooseTargetType.Ally:
                SetTarget(allyList[0]);
            break;
            
            case ActorHandler.ChooseTargetType.Enemy:
                SetTarget(enemyList[0]);
            break;
            
        }
        playerInput.actions.Enable();
        
    }
    public IEnumerator executeAttack()
    {
        currentTurn.Attack(currentTarget,()=>{});
        yield return new WaitForSecondsRealtime(1);
        EndTurn();
    }
    public IEnumerator ExecuteEnemyBehavior()
    {
        yield return new WaitForSecondsRealtime(1);
        currentTurn.enemyBehavior.ExecuteBehavior();
        yield return new WaitForSecondsRealtime(1);
        EndTurn();
    }
    public void SetTarget(ActorHandler target)
    {
        //setMarkerVisibility previous Target to false
        currentTarget.SetMarkerVisibility(false);
        
        currentTarget = target;
        currentTarget.SetMarkerVisibility(true);
    }

    void OnSelectionUp()
    {
        selectionIndex--;
        if (selectionIndex < 0)
        {
            selectionIndex = allyList.Count - 1;
        }
        SetTarget(enemyList[selectionIndex]);
    }

    void OnSelectionDown()
    {
        selectionIndex++;
        if (selectionIndex >= allyList.Count)
        {
            selectionIndex = 0;
        }
        SetTarget(enemyList[selectionIndex]);
    }

    void OnAcceptSelection()
    {
        state = State.CHOOSEACTION;
        playerInput.actions.Disable();
        switch (currentTurn.choosenAction)
        {
            case ActorHandler.ChoosenAction.Attack:
            Debug.Log(currentTarget + " Has Been Attacked By" + currentTurn);
            StartCoroutine(executeAttack());
            break;
            
        }
    }

    public void OnActorDead(ActorHandler target)
    {
        if (target.actorAligment == ActorAligmentEnum.Ally)
        {
            allyList.Remove(target);
            actors.Remove(target);
        }
        else
        {
            enemyList.Remove(target);
            actors.Remove(target);
        }
        target.gameObject.SetActive(false);
        if (allyList == null)
        {
            Debug.Log("GameOver");
        }
        else if(enemyList == null)
        {
            Debug.Log("WINNNN");
            
        }
    }



    public void SetupActor()
    {   
        if (battleParticipant == null)
        {
            Debug.LogError("Battle Participant Not Assigned");
            return;
        }
        List<CharacterStats> combinedStatsType = battleParticipant.allyCharacter.Concat<CharacterStats>(battleParticipant.enemyCharacter).ToList<CharacterStats>(); 
        Debug.Log(combinedStatsType.Count);
        for (int i = 0; i < actorsHolders.Count; i++)
        {
            actorsHolders[i].SetupActor(combinedStatsType[i]);
            
        }
    }

    public void SelectCurrentActor(ActorHandler target)
    {
        currentTurn = target;
        currentTurn.SetUIVisibilitiy(true);        
    }

    public void EndTurn()
    {
        for (int i = 0; i < currentTurn.statusModifierList.Count; i++)
        {
            currentTurn.statusModifierList[i].turnLeft--;
            if (currentTurn.statusModifierList[i].turnLeft <= 0)
            {
               currentTurn.statusModifierList.RemoveAt(i);
            }
        }
        currentTurn.SetUIVisibilitiy(false);
        currentTarget.SetMarkerVisibility(false);
        state = State.SETTURN;
    }

}

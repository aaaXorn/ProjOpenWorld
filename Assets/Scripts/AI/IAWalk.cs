﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IAWalk : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject target;
    public Animator anim;
    public Vector3 patrolposition;
    public float stoppedTime;
    public float patrolDistance=10;
    public float timetowait = 3;
    public float distancetotrigger = 10;
    public float distancetoattack = 3;
    public enum IaState
    {
        Stopped,
        Berserk,
        Attack,
        Damage,
        Dying,
        Patrol,
    }
	public bool dead;
	
	public string creatureType;
	public float baseSpeed;
	public float berserkSpeed;
	
	public Vector3 runTo;

	public GameObject Drop;
	
    public IaState currentState;
    // Start is called before the first frame update
    void Start()
    {
        patrolposition = new Vector3(transform.position.x + Random.Range(-patrolDistance, patrolDistance), transform.position.y, transform.position.z + Random.Range(-patrolDistance, patrolDistance));
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case IaState.Stopped:
                Stopped();
                break;
            case IaState.Berserk:
                Berserk();
                break;
            case IaState.Attack:
                Attack();
                break;
            case IaState.Damage:
                Damage();
                break;
            case IaState.Dying:
                Dying();
                break;
            case IaState.Patrol:
                Patrol();
                break;
        }

        anim.SetFloat("Velocity", agent.velocity.magnitude);
    }

    void Patrol()
    {
        agent.isStopped = false;
        agent.SetDestination(patrolposition);
        anim.SetBool("Attack", false);
        //tempo parado
        if (agent.velocity.magnitude < 0.1f)
        {
            stoppedTime += Time.deltaTime;
        }
        //se for mais q timetowait segundos
        if (stoppedTime> timetowait)
        {
            stoppedTime = 0;
            patrolposition = new Vector3(transform.position.x + Random.Range(-patrolDistance, patrolDistance), transform.position.y, transform.position.z + Random.Range(-patrolDistance, patrolDistance));
        }
        //ditancia do jogador for menor q distancetotrigger
        if (Vector3.Distance(transform.position, target.transform.position) < distancetotrigger && creatureType != "NPC")
        {
			agent.speed = berserkSpeed;
            currentState = IaState.Berserk;
        }

    }

    void Stopped()
    {
        agent.isStopped = true;
        anim.SetBool("Attack", false);

        if (target && Vector3.Distance(transform.position, target.transform.position) > distancetotrigger)
        {
            currentState = IaState.Patrol;
        }
    }
    void Berserk()
    {
        agent.isStopped = false;
		if(creatureType == "Enemy")
			agent.SetDestination(target.transform.position);
		else if(creatureType == "Critter")
		{
			runTo = new Vector3((2*transform.position.x) - target.transform.position.x,
								(2*transform.position.y) - target.transform.position.y,
								(2*transform.position.z) - target.transform.position.z);
			agent.SetDestination(runTo);
		}
        anim.SetBool("Attack", false);
        //se a distancia dele for  menor q 3 ele ataca
        if (Vector3.Distance(transform.position, target.transform.position) < distancetoattack)
        {
            currentState = IaState.Attack;
        }

        //se a distancia dele for  maior q o trigger ele patrulha de novo 
        if (Vector3.Distance(transform.position, target.transform.position) > distancetotrigger)
        {
			agent.speed = baseSpeed;
            currentState = IaState.Patrol;
        }
    }

    void Attack()
    {
        agent.isStopped = true;
        anim.SetBool("Attack", true);
		
        //se o jogador se afastar ele volta a perseguir
        if (Vector3.Distance(transform.position, target.transform.position) > distancetoattack+2)
        {
            currentState = IaState.Berserk;
        }
    }
    void Damage()
    {
        agent.isStopped = true;
        anim.SetBool("Attack", false);
        anim.SetTrigger("Hit");
        currentState = IaState.Stopped;
    }
    void Dying()
    {
        agent.isStopped = true;
        anim.SetBool("Attack", false);
		if(!dead)
		{
			anim.SetTrigger("Die");
			Instantiate(Drop, transform.position, Quaternion.identity);
			dead = true;
		}
        Destroy(gameObject, 3);
    }
}
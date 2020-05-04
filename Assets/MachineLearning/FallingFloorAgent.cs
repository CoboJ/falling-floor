using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using ECM.Components;
using DG.Tweening;

public class FallingFloorAgent : Agent
{
    private CharacterMovement charMove;
    private float onGround = 0;
    public float spawnArea;

    [Header("Actions Variables")]
    [HideInInspector]
    public float horizontal;
    [HideInInspector]
    public float vertical;
    [HideInInspector]
    public bool jump;

    public override void InitializeAgent()
    {
        base.InitializeAgent();
        charMove = transform.GetComponent<CharacterMovement>();
    }

    public override void AgentReset()
    {
        //DOTween.RewindAll();
        //gameObject.transform.localPosition = Vector3.zero;

        // Finale Production Ver
            
        /*if(!CharactersManager.Instance.isPlaying)
            gameObject.transform.localPosition = new Vector3(Random.Range(0f, spawnArea) * (Random.value <= 0.5 ? 1 : -1), 0, Random.Range(0f, spawnArea) * (Random.value <= 0.5 ? 1 : -1));
        else 
        {
            CharactersManager.Instance.CharacterEliminated();
            gameObject.SetActive(false);
        }*/
    }

    public override void CollectObservations()
    {
        //Use this if it is checking if is on ground
        if(charMove.isGrounded == true)
        {
            onGround = 1;
        }
        else if(charMove.isGrounded == false)
        {
            onGround = 0;
        }
        AddVectorObs(onGround);
    }

    public override void AgentAction(float[] vectorAction)
    {
        horizontal = Mathf.Clamp(vectorAction[0], -1, 1);
        vertical = Mathf.RoundToInt(Mathf.Clamp(vectorAction[1], 0, 1));
        jump = Mathf.RoundToInt(Mathf.Clamp(vectorAction[2], 0, 1)) > 0 ? true : false;

        if(vertical > 0)
        {
            AddReward(0.1f);
        }
        else
        {
            AddReward(-0.2f);
        }

        if(charMove.isGrounded == true)
        {
            AddReward(0.1f);
        }
        
        if(transform.localPosition.y < -1)
        {
            SetReward(-20f);
            //Done();
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.transform.CompareTag("wall"))
        {
            SetReward(-20f);
            //Done();
        }
    }

    public override float[] Heuristic()
    {
        var action = new float[3];
        action[0] = Input.GetAxis("Mouse X");
        action[1] = Input.GetAxis("Vertical");
        action[2] = Input.GetAxis("Jump");
        return action;
    }
}

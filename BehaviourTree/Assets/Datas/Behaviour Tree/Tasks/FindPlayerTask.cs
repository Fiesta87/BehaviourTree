using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindPlayerTask : BehaviourTreeTask {

    public Vector3 out_playerPosition;

    private int a;

    // Use this methode to setup the object link for your task, this code will be executed only once when the BehaviourTreeAgent Start methode is called.
    // eg. this.myCustomMonoBehaviourScript = this.agent.GetComponent<CustomMonoBehaviourScript>();
    public override void InitOnStart () {
        a = 0;
    }
	
    // Use this methode to init your task, this code will be executed every time a parent start this task.
    public override void Begin () {
        
    }

    // Use this methode to perform your task
    public override BehaviourTree.Status Update () {
        Debug.Log(this.agent.ToString() + " : " + out_playerPosition);
        Debug.Log(a);
        a++;
        if(PlayerController.Instance == null) {
            return BehaviourTree.Status.FAILURE;
        }

        this.out_playerPosition = PlayerController.Instance.gameObject.transform.position;

        return BehaviourTree.Status.SUCCESS;
    }

    public override void FinishSuccess () {
		
    }

    public override void FinishFailure () {
		
    }

	public override void Kill () {
        
    }
}

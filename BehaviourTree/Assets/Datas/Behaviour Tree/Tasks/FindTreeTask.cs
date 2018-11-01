using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTreeTask : BehaviourTreeTask {

    public Tree out_closestTree;
    public Vector3 out_position;

    // Use this methode to setup the object link for your task, this code will be executed only once when the BehaviourTreeAgent Start methode is called.
    // eg. this.myCustomMonoBehaviourScript = this.agent.GetComponent<CustomMonoBehaviourScript>();
    public override void InitOnStart () {

    }
	
    // Use this methode to init your task, this code will be executed every time a parent start this task.
    public override void Begin () {
        
    }

    // Use this methode to perform your task
    public override BehaviourTree.Status Update () {

        GameObject[] treeList = GameObject.FindGameObjectsWithTag("Tree");

        if(treeList.Length == 0) {
            return BehaviourTree.Status.FAILURE;
        }

        this.out_closestTree = treeList[0].GetComponent<Tree>();

        this.out_position = treeList[0].transform.position;

        return BehaviourTree.Status.SUCCESS;
    }

    public override void FinishSuccess () {
		
    }

    public override void FinishFailure () {
		
    }

	public override void Kill () {
        
    }
}

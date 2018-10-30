using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BehaviourTreeExecutionNode : BehaviourTreeNode, ISerializationCallbackReceiver {

    [SerializeField]
    [HideInInspector]
    public BehaviourTreeTask task;

    [NonSerialized]
    public Dictionary<string, string> contextLink = new Dictionary<string, string>();

    [SerializeField]
    public List<string> contextLinkKeys = new List<string>();

    [SerializeField]
    public List<string> contextLinkValues = new List<string>();

    [NonSerialized]
    private bool firstTick;

    [NonSerialized]
    private BehaviourTreeAgent agent;

    public override void Init (BehaviourTreeAgent agent) {
        this.firstTick = true;
        this.agent = agent;
        this.task.Init(agent);
        this.task.InitOnStart();
    }
    
    public override BehaviourTree.Status Tick() {
        
        BehaviourTree.Status result;

        if(this.firstTick) {
            
            this.firstTick = false;
            
            this.task.SetInputContext(this);

            this.task.Begin();
        }

        result = this.task.Update();

        if(result == BehaviourTree.Status.SUCCESS) {
            this.task.FinishSuccess();
            this.task.SetOutputContext(this);
            this.firstTick = true;
        } else if(result == BehaviourTree.Status.FAILURE) {
            this.task.FinishFailure();
            this.task.SetOutputContext(this);
            this.firstTick = true;
        }

        return result;
    }

	public override void Kill () {
        if( ! this.firstTick) {
            this.task.Kill();
            this.firstTick = true;
        }
    }

    public override int ChildrenCount () {
        return 0;
    }

    public override List<BehaviourTreeNode> GetChildren () {
        return new List<BehaviourTreeNode>();
    }

    public override void RemoveChild (BehaviourTreeNode child) {
        
    }

	public override void AddChild (BehaviourTreeNode child) {
		
	}
	public override void ReplaceChild (BehaviourTreeNode oldChild, BehaviourTreeNode newChild) {
		
	}

    public void OnBeforeSerialize () {

        contextLinkKeys.Clear();
        contextLinkValues.Clear();

        foreach(System.Collections.Generic.KeyValuePair<string, string> record in contextLink) {

            contextLinkKeys.Add(record.Key);
            contextLinkValues.Add(record.Value);
        }
    }

    public void OnAfterDeserialize () {

        contextLink = new Dictionary<string, string>();

        for(int i=0; i != Mathf.Min(contextLinkKeys.Count, contextLinkValues.Count); i++) {
            contextLink.Add(contextLinkKeys[i], contextLinkValues[i]);
        } 
    }
}

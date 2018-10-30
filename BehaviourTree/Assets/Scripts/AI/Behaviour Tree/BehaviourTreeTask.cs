using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// This class is an executable task in a behaviour tree.
/// </summary>
public abstract class BehaviourTreeTask : ScriptableObject {

    [NonSerialized]
    protected BehaviourTreeAgent agent;

    public void Init (BehaviourTreeAgent agent) {
        this.agent = agent;
    }

    public void SetInputContext (BehaviourTreeExecutionNode node) {

        foreach(System.Collections.Generic.KeyValuePair<string, string> record in node.contextLink) {
            
            if(record.Key.StartsWith("in_")) {
                try {
                    PropertyReader.SetValue(this, record.Key, this.agent.GetContext()[record.Value]);
                } catch (KeyNotFoundException e) {
                    Debug.LogError("The variable " + record.Value + " doesn't exist in this context.");
                    Debug.LogException(e);
                }
                
            }
        }
    }

    public void SetOutputContext (BehaviourTreeExecutionNode node) {

        foreach(System.Collections.Generic.KeyValuePair<string, string> record in node.contextLink) {
            
            if(record.Key.StartsWith("out_")) {
                this.agent.GetContext()[record.Value] = PropertyReader.GetValue(this, record.Key);
            }
        }
    }

    public abstract void InitOnStart ();
	public abstract void Begin ();
	public abstract BehaviourTree.Status Update ();
	public abstract void FinishSuccess ();
	public abstract void FinishFailure ();
	public abstract void Kill ();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is an executable task in a behaviour tree.
/// </summary>
public abstract class BehaviourTreeTask : ScriptableObject {

    protected BehaviourTreeAgent agent;

    [SerializeField]

    public void Init (BehaviourTreeAgent agent) {
        this.agent = agent;
    }

    public void SetInputContext (BehaviourTreeExecutionNode node) {

        foreach(System.Collections.Generic.KeyValuePair<string, string> record in node.contextLink) {
            
            if(record.Key.StartsWith("in_")) {
                PropertyReader.setValue(this, record.Key, this.agent.GetContext()[record.Value]);
            }
        }
    }

    public void SetOutputContext (BehaviourTreeExecutionNode node) {

        foreach(System.Collections.Generic.KeyValuePair<string, string> record in node.contextLink) {
            
            if(record.Key.StartsWith("out_")) {
                this.agent.GetContext()[record.Value] = PropertyReader.getValue(this, record.Key);
            }
        }
    }

    public abstract void InitOnStart ();
	public abstract void Begin ();
	public abstract BehaviourTree.Status Update ();
	public abstract void FinishSuccess ();
	public abstract void FinishFailure ();
}

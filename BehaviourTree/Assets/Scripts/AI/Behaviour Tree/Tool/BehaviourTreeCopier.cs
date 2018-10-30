using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeCopier {

    public static BehaviourTree Copy (BehaviourTree original) {

        BehaviourTree copy = UnityEngine.ScriptableObject.Instantiate(original);

        copy.child = CopyNodesRecursively(original.child);

        return copy;
    }

    private static BehaviourTreeNode CopyNodesRecursively (BehaviourTreeNode original) {

        BehaviourTreeNode result = UnityEngine.ScriptableObject.Instantiate(original);

        for(int i=0; i<result.ChildrenCount(); i++) {
            result.ReplaceChild(result.GetChildren()[i], CopyNodesRecursively(original.GetChildren()[i]));
        }

        if(original is BehaviourTreeExecutionNode) {
            ((BehaviourTreeExecutionNode)result).task = UnityEngine.ScriptableObject.Instantiate(((BehaviourTreeExecutionNode)original).task);
        }

        if(original is BehaviourTreeSubTreeNode) {
            ((BehaviourTreeSubTreeNode)result).subTree = Copy(((BehaviourTreeSubTreeNode)original).subTree);
        }

        return result;
    }
}
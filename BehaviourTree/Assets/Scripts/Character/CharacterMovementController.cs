using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

///	<summary>
///	This class control where a character should go.
///	It trigger the right animation and move the character.
///	</summary>  
public class CharacterMovementController : MonoBehaviour {

	private Animator animator;
	private NavMeshAgent navMeshAgent;
	private new Rigidbody rigidbody;
	private SpriteRenderer spriteRenderer;

	void Awake () {
		this.animator = GetComponent<Animator>();
		this.navMeshAgent = GetComponent<NavMeshAgent>();
		this.rigidbody = GetComponent<Rigidbody>();
		this.spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}

	void Start () {
		this.navMeshAgent.destination = this.transform.position;
	}
	
	void Update () {
		this.animator.SetBool("Walk", !this.IsAtDestination());
		this.spriteRenderer.flipX = this.transform.forward.x >= 0.0f;
	}

	///	<summary>
	///	The character will walk to the indicated position.
	/// Return true if the destination is reachable, false otherwise.
	///	</summary>
	public bool WalkTo (Vector3 position) {
		return this.navMeshAgent.SetDestination(position);
	}

	///	<summary>
	///	Is this character currently arrived at his setted destination ?
	///	</summary>
	public bool IsAtDestination () {
		return (this.navMeshAgent.destination - this.transform.position).magnitude <= this.navMeshAgent.stoppingDistance;
	}
}

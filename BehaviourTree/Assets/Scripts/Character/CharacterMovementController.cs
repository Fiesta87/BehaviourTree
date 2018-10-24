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

		// Set the animation to Walk if the character is moving.
		this.animator.SetBool("Walk", !this.IsAtDestination() && !this.navMeshAgent.isStopped);

		// Set the sprite orientation according to the character forward/looking direction.
		this.spriteRenderer.flipX = this.transform.forward.x >= 0.0f;
	}

	///	<summary>
	///	The character will walk to the indicated position.
	/// Return true if the destination is reachable, false otherwise.
	///	</summary>
	public bool WalkTo (Vector3 position) {
		this.navMeshAgent.isStopped = false;
		return this.navMeshAgent.SetDestination(position);
	}

	///	<summary>
	///	Is this character currently arrived at his setted destination ?
	///	</summary>
	/// <param name="distance">The distance at witch the character is considered as arrived. Default value is 0.0f.</param>
	public bool IsAtDestination (float distance = 0.0f) {
		return (this.navMeshAgent.destination - this.transform.position).magnitude <= this.navMeshAgent.stoppingDistance + distance;
	}

	///	<summary>
	///	Return the distance between this character and the specified position.
	///	</summary>
	/// <param name="position">The position with which measure the distance.</param>
	public float DistanceTo (Vector3 position) {
		return (position - this.transform.position).magnitude;
	}

	///	<summary>
	///	Stop the movement of the character.
	///	</summary>
	public void Stop () {
		this.navMeshAgent.isStopped = true;
	}
}

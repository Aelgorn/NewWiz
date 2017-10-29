﻿/*
* A script to animate the player's movement.
* Use left, right, up, and down arrow keys to move the player around (please note that any lefties can also use the classic WASD).
* Note: The player animator is getting updated by this script and is displaying temp animations to indicate the player direction.
* Note2: Directions can be combined; i.e. you can go diagonally up-left and the animation will be specific to that key combination. I thought
* I'd get that implementation out of the way now, in case we decide to use it. All we need to do if we do want to use it is replace the
* animations contained in the states. Just drag 'em on in there.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//At least one of the participants of a collision (something like OnTriggerEnter, OnTriggerStay, or OnTriggerExit) must have a Rigidbody.
//It makes more sense to just make the player have one and disable gravity and all adjustments to position made from the rigidbody, rather
//than make one for every other object that might ever collider with the player.
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MobileCharacter {

	/**A string variable containing the string name of the Input Manager variable responsible for player leftward movement.*/
	private readonly string STRINGKEY_INPUT_LEFT = "Left";
	/**A string variable containing the string name of the Input Manager variable responsible for player rightward movement.*/
	private readonly string STRINGKEY_INPUT_RIGHT = "Right";
	/**A string variable containing the string name of the Input Manager variable responsible for player "upward" movement.
	*Note: Here, "upward" denotes motion into the scene, away from the camera.*/
	private readonly string STRINGKEY_INPUT_UP = "Up";
	/**A string variable containing the string name of the Input Manager variable responsible for player "downward" movement.
	*Note: Here, "downward" denotes motion out of the scene, toward the camera.*/
	private readonly string STRINGKEY_INPUT_DOWN = "Down";

	/**A string variable containing the string name of the Animator parameter corresponding to the player facing leftward.*/
	private readonly string STRINGKEY_PARAM_ISFACINGLEFT = "isFacingLeft";
	/**A string variable containing the string name of the Animator parameter corresponding to the player facing rightward.*/
	private readonly string STRINGKEY_PARAM_ISFACINGRIGHT = "isFacingRight";
	/**A string variable containing the string name of the Animator parameter corresponding to the player facing upward.*/
	private readonly string STRINGKEY_PARAM_ISFACINGUP = "isFacingUp";
	/**A string variable containing the string name of the Animator parameter corresponding to the player facing downward.*/
	private readonly string STRINGKEY_PARAM_ISFACINGDOWN = "isFacingDown";

	/**A bool to tell us whether or not the player can move*/
	private bool m_CanMove = true;

	void Start()
	{
		this.m_MaximalVelocity = 20.0f;
		this.SetAnimator (this.GetComponent<Animator> ());

		Rigidbody player_rigidbody = this.GetComponent<Rigidbody> ();
		//Disable gravity
		player_rigidbody.useGravity = false;
		//Freeze effects of physics on rotation and position
		player_rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		//Ensure we can use collisions properly.
		player_rigidbody.detectCollisions = true;

		this.m_Direction = Vector3.right;


	}//end f'n void Start()

	// Update is called once per frame
	void Update () {

		//if the player can't move...
		if (!this.m_CanMove) {
			//then forget about input
			return;
		}//end if

		float leftward_input = -(Input.GetAxisRaw (STRINGKEY_INPUT_LEFT));
		float rightward_input = Input.GetAxisRaw (STRINGKEY_INPUT_RIGHT);
		//Note: "downward" in this context means down the scene, toward the camera
		float downward_input = -(Input.GetAxisRaw (STRINGKEY_INPUT_DOWN));
		//Note: "upward" in this context means into the scene, away from the camera
		float upward_input = Input.GetAxisRaw (STRINGKEY_INPUT_UP);
		//All horizontal inputs
		float horizontal_movement_input = 0.0f;
		//All vertical inputs
		float vertical_movement_input = 0.0f;

		//If any leftward or rightward input was detected...
		if (leftward_input != 0 || rightward_input != 0) {
			//...then set the value of the total horizontal input to reflect that input.
			//Notice that leftward input will be negative and rightward input will be positive.
			horizontal_movement_input = leftward_input + rightward_input;

			//if the sum of rightward and leftward input is positive, we're moving rightward
			this.m_IsMovingRight = (horizontal_movement_input > 0) ? true : false;
			//if we're moving rightward, then we're not moving leftward.
			//Similarly, if we're not moving rightward but there was horizontal input detected, then we must be moving leftward.
			this.m_IsMovingLeft = (this.m_IsMovingRight) ? false : true;
		}//end if
		//else if no leftward or rightward input was detected...
		else {
			//...then we are neither moving left nor right
			this.m_IsMovingRight = false;
			this.m_IsMovingLeft = false;
		}//end else
		//If any downward or upward input was detected...
		if (upward_input != 0 || downward_input != 0) {
			//...then set the value of the total vertical input to reflect that input.
			//Notice that downward input will be negative and upward input will be positive.
			vertical_movement_input = downward_input + upward_input;

			//if the sum of upward and downward input is positive, we're moving upward
			this.m_IsMovingUp = (vertical_movement_input > 0) ? true : false;
			//if we're moving upward, then we're not moving downward.
			//Similarly, if we're not moving upward but there was vertical input detected, then we must be moving downward.
			this.m_IsMovingDown = (this.m_IsMovingUp) ? false : true;
		}//end if
		//else if no upwardward or downward input was detected...
		else {
			//...then we are neither moving up nor down
			this.m_IsMovingUp = false;
			this.m_IsMovingDown = false;
		}//end else
		//if either horizontal or vertical input was detected...
		if (horizontal_movement_input != 0 || vertical_movement_input != 0) {
			Vector3 player_current_position = this.transform.position;
			Vector3 translation = new Vector3 (horizontal_movement_input * this.m_MaximalVelocity, 0.0f, vertical_movement_input * this.m_MaximalVelocity);
			translation = Vector3.ClampMagnitude (translation, this.m_MaximalVelocity);
			//Update direction
			this.m_Direction = translation;
			//***Check for collision with scene objects

			//if there's no object with a component of type Obstructable ahead...
			if (!this.IsObstructableAhead ()) {
				//...then keep moving
				this.transform.position = player_current_position + (translation * Time.deltaTime);
			}
		}//end if
		//else if we received absolutely no movement input...
		else {
			//...then update which way we're facing, based on the last direction
			this.UpdateFacingParameters ();
		}
		//Update the animator parameters
		this.UpdateAnimatorParameters();

	}//end f'n void Update

	/**A function to tell the animator which direction the player is facing.*/
	private void UpdateFacingParameters()
	{
		/*
		* The direction we're facing in is defined as follows:
		* 	- If we're moving either upward or downward but still going rightward, face rightward on rest
		* 	- If we're moving either upward or downward but still going leftward, face leftward on rest
		* 	- If we're only going upward, face upward on rest
		* 	- If we're only going downward, face downward on rest
		*/
		this.m_Animator.SetBool (this.STRINGKEY_PARAM_ISFACINGRIGHT, this.m_Direction.x > 0);
		this.m_Animator.SetBool (this.STRINGKEY_PARAM_ISFACINGLEFT, this.m_Direction.x < 0);
		this.m_Animator.SetBool (this.STRINGKEY_PARAM_ISFACINGUP, this.m_Direction.z > 0 && this.m_Direction.x == 0);
		this.m_Animator.SetBool (this.STRINGKEY_PARAM_ISFACINGDOWN, this.m_Direction.z < 0 && this.m_Direction.x == 0);
	}//end f'n void UpdateFacingParameters()

	/**A function to disable player movement.*/
	public void DisableMovement()
	{
		this.m_CanMove = false;
	}

	/**A function to enable player movement*/
	public void EnableMovement()
	{
		this.m_CanMove = true;
	}

}//end class PlayerMovement

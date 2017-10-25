﻿/*
* A script for the player's magic casting mechanics.
* The way this is supposed to work is wherever the user clicks on the screen, provided they aren't clicking on a UI element, then
* a spell should be cast at that spot.
*/
//A macro for testing; comment out to remove testing functionalities
//#define TESTING_SPELLCAST
//#define TESTING_SPELLMOVEMENT
//#define TESTING_MOUSE_UP

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerCastSpell : MonoBehaviour {

	#if TESTING_SPELLCAST
	[SerializeField] private GameObject m_MagicCubePrefab;
	#endif
	/**A reference to our default spell prefab.*/
	[SerializeField] public GameObject m_SpellCube;
	/**A reference to an instantiated spell cube (instantiated from the reference to the default spell prefab), to have its Spell component set accordingly.*/
    public GameObject m_SpellCubeInstance;

	/**A manager for the spell animator controllers*/
	[SerializeField] private SpellAnimatorManager m_SpellAnimatorManager;

    private GameObject m_Target;
	/**A reference to our main camera.*/
	[SerializeField] private Camera m_MainCamera;
	/**A reference to the player, so we can affect his parameters as a result of magic usage.*/
	private Player m_Player;

	/**A string variable containing the string name of the Input Manager variable responsible for player firing off spells.*/
	private readonly string STRINGKEY_INPUT_CASTSPELL = "Cast Spell";
	/**A string variable containing the string name of the isCastingSpell parameter in the player animator.*/
	private readonly string STRINGKEY_PARAM_CASTSPELL = "isCastingSpell";
	/**A bool variable to let us know whether or not the player's in the process of casting a spell.*/
	private bool m_isCastingSpell;

	/**The player animator, including the bit about casting spells.*/
	private Animator m_Animator;
    
	/**The reference to the SpellClass which is fired by the player.*/
	public SpellClass m_SpellClassToFire;

	/**A variable for testing purposes*/
	public string m_SpellName;

	/**The number of seconds until we destroy the spell gameobject.*/
	private readonly float TIME_UNTIL_DESTROY = 1.25f;

	void Start()
	{
		this.m_Animator = this.GetComponent<Animator> ();
		this.m_Player = this.GetComponent<Player> ();
    }

	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown (STRINGKEY_INPUT_CASTSPELL)) {
			this.CheckChosenSpell ();
			//if the spell to fire exists...
			if (this.m_SpellClassToFire != null) {

				//Update [this.m_isCastingSpell] for the animator
				this.m_isCastingSpell = true;

				//if the spell is mobile (meaning it's to be cast somewhere away from the player)...
				if (this.m_SpellClassToFire.m_IsMobileSpell) {
					Ray ray = this.m_MainCamera.ScreenPointToRay (Input.mousePosition);
					RaycastHit[] targets_hit = Physics.RaycastAll (ray);
					//We need to find the raycast hit furthest from the camera in the event that none of the raycasthits are 
					//mobile character as the furthest raycast hit will be the ground.
					RaycastHit furthest = targets_hit [0];
					bool any_mobile_characters = false;
					foreach (RaycastHit hit in targets_hit) {
						//if the hit's distance is greater than that of the furthest...
						if (hit.distance > furthest.distance) {
							//...then update the furthest
							furthest = hit;
						}//end if

						//if the hit has a MobileCharacter component...
						if (hit.collider.gameObject.GetComponent<MobileCharacter> () != null) {
							m_Target = hit.collider.gameObject;
							this.m_SpellCubeInstance = GameObject.Instantiate (this.m_SpellCube);
							this.m_SpellCubeInstance.transform.position = this.transform.position;
							SpellMovement spell_movement = this.m_SpellCubeInstance.GetComponent<SpellMovement> ();
							spell_movement.m_IsMobileCharacter = true;
							spell_movement.SetTarget (hit);
							any_mobile_characters = true;
							spell_movement.SetSpellToCast (this.m_SpellClassToFire);

							this.m_SpellAnimatorManager.SetSpellAnimator (this.m_SpellCubeInstance);
							//return to ensure we only launch one spell
							return;
						}//end if
					}//end foreach

					//if none of the gameobjects found in the raycastall were mobile characters...
					if (!any_mobile_characters) {
						//...then send the spell to the furthest Raycast hit
						#if TESTING_SPELLMOVEMENT
						Debug.Log("PlayerCastSpell::Update\tNo mobile characters found\tRay hit\tx: " + furthest.point.x
							+ " y: " + furthest.point.y + " z: " + furthest.point.z);
						#endif
						this.m_SpellCubeInstance = GameObject.Instantiate (this.m_SpellCube);
						this.m_SpellCubeInstance.transform.position = this.transform.position;
						SpellMovement spell_movement = this.m_SpellCubeInstance.GetComponent<SpellMovement> ();
						spell_movement.m_IsMobileCharacter = false;
						spell_movement.SetTarget (furthest);

						spell_movement.SetSpellToCast (this.m_SpellClassToFire);

						this.m_SpellAnimatorManager.SetSpellAnimator (this.m_SpellCubeInstance);

						GameObject.Destroy (this.m_SpellCubeInstance, TIME_UNTIL_DESTROY);

					}//end if
				}//end if
				//else if the spell is not mobile (meaning it's cast at the player's location...)
				//we have no spells that fit this yet, so do nothing.

			}//end if
		}//end if
		//else if the user holds down the mouse...
		else if (Input.GetButton(STRINGKEY_INPUT_CASTSPELL)) {
			this.CheckChosenSpell ();

			//if the spell to fire exists...
			if (this.m_SpellClassToFire != null && !this.m_SpellClassToFire.m_IsMobileSpell) {
				//Update [this.m_isCastingSpell] for the animator
				this.m_isCastingSpell = true;

				//if the spell is not mobile (meaning it's to be cast at the player)
				//	AND if the spell cube instance is null (which can only mean the last spell cube was destroyed)...
				if (!this.m_SpellClassToFire.m_IsMobileSpell && this.m_SpellCubeInstance == null) {
					//...then create a new spell cube
					this.m_SpellCubeInstance = GameObject.Instantiate (this.m_SpellCube);
					this.m_SpellCubeInstance.transform.position = this.transform.position;
					SpellMovement spell_movement = this.m_SpellCubeInstance.GetComponent<SpellMovement> ();
					spell_movement.SetSpellToCast (this.m_SpellClassToFire);
					this.m_SpellAnimatorManager.SetSpellAnimator (this.m_SpellCubeInstance);

				}//end if
				//if the spell cube instance exists (meaning we're in the process of casting a spell)...
				if (this.m_SpellCubeInstance != null) {
					//...then ensure the spell's always at our position
					SpellMovement spell_movement = this.m_SpellCubeInstance.GetComponent<SpellMovement> ();
					spell_movement.MaintainPosition (this.transform.position);
				}//end if
				//else if the spell is not mobile (meaning it's cast at the player's location...)
				//we have no spells that fit this yet, so do nothing.

			}//end if

		}//end if
		//if the player lets go of the mouse and the spell wasn't a mobile spell (meaning that at this point they're probably still
			//holding down the mouse...
		else if (Input.GetButtonUp (STRINGKEY_INPUT_CASTSPELL) && !this.m_SpellClassToFire.m_IsMobileSpell) {
			#if TESTING_MOUSE_UP
			Debug.Log ("Mouse up");
			#endif
			//...then the spell is no longer being cast
			this.m_isCastingSpell = false;
			//...and we need to destroy the gameobject instance

			GameObject.Destroy (this.m_SpellCubeInstance);
		}//end if

		//else if the player doesn't click (doesn't fire a spell)...
		else {
			//Update [this.m_isCastingSpell] for the animator
			this.m_isCastingSpell = false;
		}//end else

		//USE THIS SPACE TO UPDATE ANY PLAYER ATTRIBUTES AS A RESULT OF MAGIC
		this.ApplyPlayerAttributesAsResultOfMagic();

		this.UpdateAnimatorParameters ();
	}//end f'n void Update()

	/**A function to neatly apply all player attributes as a result of a given magic.
	*For instance, we'll use this function to apply the [IsShielded] to the Player class.*/
	private void ApplyPlayerAttributesAsResultOfMagic()
	{
		//***SHIELD
		//if we're casting a spell and the spell we're firing is the shield then the player is shielded.
		this.m_Player.m_IsShielded = (this.m_isCastingSpell && 
										this.m_SpellClassToFire.m_SpellName == SpellName.Shield) ? true : false;
	}//end f'n void ApplyPlayerAttributesAsResultOfMagic()

//	/**A function to return the medium-most time for a clip in the animator.
//	*This is to help us know when to destroy the gameobject such that the fade-out animation can play.*/
//	private float TimeOfEndClip()
//	{
//		float shortest_clip_time = 1000.0f, medium_clip_time = 0.0f, longest_clip_time = 0.0f;
//		foreach(AnimationClip clip in this.m_Animator.runtimeAnimatorController.animationClips)
//		{
//			//...if a given clip's length is greater than that of the longest clip's length...
//			if (clip.length > longest_clip_time) {
//				//...then update the longest clip
//				longest_clip_time = clip.length;
//			}//end if
//			//...if a given clip's length is less than that of the shortest clip's length...
//			if (clip.length < shortest_clip_time) {
//				//...then update the shortest clip
//				shortest_clip_time = clip.length;
//			}//end if
//			//...if a given clip's length is greater-than or equal to the shortest clip length
//			//		AND that same given clip's length is less-than or equal to the longest clip length...
//			if (clip.length >= shortest_clip_time && clip.length <= longest_clip_time) {
//				//...then update the medium clip
//				medium_clip_time = clip.length;
//			}//end if
//		}//end foreach
//		Debug.Log(medium_clip_time);
//		return medium_clip_time + 0.1f;
//	}//end f'n float TimeOfEndClip()

	/**Used to retrieve the current spell from the inventory.*/
	private void CheckChosenSpell()
	{
		this.m_SpellClassToFire = this.gameObject.GetComponent<PlayerInventory>().m_ActiveSpellClass;
		this.m_SpellName = this.m_SpellClassToFire.m_SpellName.ToString ();

	}//end f'n void CheckChosenSpell()

	/**A function to update the player animator with regards to the player spell casting animations.*/
	private void UpdateAnimatorParameters()
	{
		this.m_Animator.SetBool (STRINGKEY_PARAM_CASTSPELL, this.m_isCastingSpell);

	}
}//end class PlayerCastSpell

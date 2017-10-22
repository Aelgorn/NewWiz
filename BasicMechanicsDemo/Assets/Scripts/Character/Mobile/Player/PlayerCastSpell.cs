﻿/*
* A script for the player's magic casting mechanics.
* The way this is supposed to work is wherever the user clicks on the screen, provided they aren't clicking on a UI element, then
* a spell should be cast at that spot.
*/
//A macro for testing; comment out to remove testing functionalities
//#define TESTING_SPELLCAST
#define TESTING_SPELLMOVEMENT


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerCastSpell : MonoBehaviour {

	#if TESTING_SPELLCAST
	[SerializeField] private GameObject m_MagicCubePrefab;
	#endif
	#if TESTING_SPELLMOVEMENT
	[SerializeField] private GameObject m_SpellCube;
    private GameObject m_SpellCubeInstance;
    #endif

    private GameObject m_Target;
	/**A reference to our main camera.*/
	[SerializeField] private Camera m_MainCamera;

	/**A string variable containing the string name of the Input Manager variable responsible for player firing off spells.*/
	private readonly string STRINGKEY_INPUT_CASTSPELL = "Cast Spell";
	/**A string variable containing the string name of the isCastingSpell parameter in the player animator.*/
	private readonly string STRINGKEY_PARAM_CASTSPELL = "isCastingSpell";
	/**A bool variable to let us know whether or not the player's in the process of casting a spell.*/
	private bool m_isCastingSpell;

    private bool m_hasCastHittingSpell;
	/**The player animator, including the bit about casting spells.*/
	private Animator m_Animator;
    /**The reference to the spell which is fired by the player.*/
	private Spell m_SpellToFire;

	/**The number of seconds until we destroy the spell gameobject.*/
	private readonly float TIME_UNTIL_DESTROY = 1.25f;

	void Start()
	{
		this.m_Animator = this.GetComponent<Animator> ();
    }

	// Update is called once per frame
	void Update () {
		
		if (Input.GetButtonDown (STRINGKEY_INPUT_CASTSPELL)) {
			this.CheckChosenSpell ();
			//
			if (this.m_SpellToFire != null) {
				//Update [this.m_isCastingSpell] for the animator
				this.m_isCastingSpell = true;

				Ray ray = this.m_MainCamera.ScreenPointToRay (Input.mousePosition);
				RaycastHit[] targets_hit = Physics.RaycastAll(ray);
				bool any_mobile_characters = false;
				foreach (RaycastHit hit in targets_hit) {
					if (hit.collider.gameObject.GetComponent<MobileCharacter> () != null) {
						#if TESTING_SPELLMOVEMENT
						m_Target = hit.collider.gameObject;
						this.m_SpellCubeInstance = GameObject.Instantiate(this.m_SpellCube);
						this.m_SpellCubeInstance.transform.position = this.transform.position;
						this.m_hasCastHittingSpell = true;
						SpellMovement spell_movement = this.m_SpellCubeInstance.GetComponent<SpellMovement>();
						spell_movement.m_IsMobileCharacter = true;
                        spell_movement.SetTarget(hit);
						any_mobile_characters = true;
//						GameObject.Destroy(this.m_SpellCubeInstance, TIME_UNTIL_DESTROY);
						#endif
					}
                }
				//if none of the gameobjects found in the raycastall were mobile characters...
				if (!any_mobile_characters) {
					RaycastHit target_hit;

					if (Physics.Raycast(ray, out target_hit))
					{
                        //If the target hit was an obstructable.
                        if (target_hit.collider.gameObject.GetComponent<Obstructable>() != null)
                        {
                            #if TESTING_SPELLMOVEMENT
                            m_Target = target_hit.collider.gameObject;
                            this.m_SpellCubeInstance = GameObject.Instantiate(this.m_SpellCube);
                            this.m_SpellCubeInstance.transform.position = this.transform.position;
                            this.m_hasCastHittingSpell = true;
                            SpellMovement spell_movement_obs = this.m_SpellCubeInstance.GetComponent<SpellMovement>();
                            spell_movement_obs.m_IsObstructable = true;
                            spell_movement_obs.SetTarget(target_hit);
//						    GameObject.Destroy(this.m_SpellCubeInstance, TIME_UNTIL_DESTROY);
                            #endif
                        }
                        else
                        {
                        #if TESTING_SPELLMOVEMENT
                            Debug.Log("PlayerCastSpell::Update\tNo characters found\tRay hit\tx: " + target_hit.point.x
                                + " y: " + target_hit.point.y + " z: " + target_hit.point.z);
                            this.m_SpellCubeInstance = GameObject.Instantiate(this.m_SpellCube);
                            this.m_SpellCubeInstance.transform.position = this.transform.position;
                            SpellMovement spell_movement = this.m_SpellCubeInstance.GetComponent<SpellMovement>();
//						    spell_movement.SetSpellVariables(m_Spell);
                            spell_movement.m_IsMobileCharacter = false;
                            spell_movement.SetTarget(target_hit);
                            GameObject.Destroy(this.m_SpellCubeInstance, TIME_UNTIL_DESTROY);
                        #endif
                        }


                    }//end if

				}//end if

			}//end if

		}//end if
		//else if the player doesn't click (doesn't fire a spell)...
		else {
			//Update [this.m_isCastingSpell] for the animator
			this.m_isCastingSpell = false;
		}//end else



		this.UpdateAnimatorParameters ();
	}//end f'n void Update()

    /**Used to retrieve the current spell from the inventory.*/
    private void CheckChosenSpell()
    {
		this.m_SpellToFire = this.gameObject.GetComponent<PlayerInventory>().m_ActiveSpell;
    }

	/**A function to update the player animator with regards to the player spell casting animations.*/
	private void UpdateAnimatorParameters()
	{
		this.m_Animator.SetBool (STRINGKEY_PARAM_CASTSPELL, this.m_isCastingSpell);

	}
}//end class PlayerCastSpell

﻿//#define TESTING_SPELLMOVEMENT
#define TESTING_SPELLCOLLISION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellMovement : MonoBehaviour {

	public float m_MaximalVelocity = 35.0f;
	/**A reference to the target the spell is being cast at. Set using SpellMovement::SetTarget(GameObject).*/
	public RaycastHit m_Target { get; private set; }
	/**A reference to the gameobject of [this.m_Target] - this will be for the spells to lock on in case the enemy or target is moving.*/
	private GameObject m_TargetedObj;
	/**The direction in which the spell is moving.*/
	private Vector3 m_Direction = new Vector3();
	/**A bool to let us know whether the target is a mobile character.*/
	public bool m_IsMobileCharacter { get; set;}
	/**The spell we're currently casting.*/
	public Spell m_SpellToCast;

	//For testing purposes
	public string m_SpellName;

	void Awake()
	{
        
	}

	// Update is called once per frame
	void Update () {
		if (this.m_IsMobileCharacter) {
			//Update direction
			this.SetDirection ();
		}

		#if TESTING_SPELLMOVEMENT
		string message = "SpellMovement::Update::Direction of spell:\tx:" + this.m_Direction.x + "\ty:" + this.m_Direction.y +
		"\tz:" + this.m_Direction.z;
		Debug.Log(message);
		#endif

		//get our current position (which will be at whoever cast the spell)
		Vector3 current_position = this.transform.position;
		Vector3 translation = Vector3.ClampMagnitude (this.m_Direction, this.m_MaximalVelocity) * Time.deltaTime;
		//and update the current position
		current_position += translation;
		this.transform.position = current_position;
	}//end f'n void Update()

	/**Set the target at which we're shooting the magic.*/
	public void SetTarget(RaycastHit spell_target)
	{
		//Set the target
		this.m_Target = spell_target;

		if (this.m_IsMobileCharacter) {
			//Set the target's gameobject, to be able to follow it around if it's moving
			this.m_TargetedObj = this.m_Target.collider.gameObject;
		} else {
			this.m_TargetedObj = null;
			//...then send the spell in the direction of wherever the cursor was clicked
			this.m_Direction = Vector3.Normalize(this.m_Target.point - this.transform.position) * this.m_MaximalVelocity;
			this.m_Direction.y = 0.0f;
		}
	}//end f'n void SetTarget(GameObject)

	/**A private function to set the direction to the given target [this.m_Target].*/
	private void SetDirection()
	{
		//Note on the following if: it's possible that we destroy the enemy while spells are still on their way to them
		//if the target is a mobile character and exists...
		if (this.m_IsMobileCharacter && this.m_TargetedObj != null) {
			//...then lock onto the mobile character's position
			Vector3 target_position = this.m_TargetedObj.transform.position;
			this.m_Direction = Vector3.Normalize(target_position - this.transform.position) * this.m_MaximalVelocity;
		}//end if 
		//else if the target is a mobile character but has since been destroyed...
		if (this.m_IsMobileCharacter && this.m_TargetedObj == null) {
			//...then change the [m_IsMobileCharacter] value.
			this.m_IsMobileCharacter = false;
		}//end if 
		//else if the target is not a mobile character...
			//...then make no change to the direction
	}//end f'n void SetDirection()

	/**A function to be called from PlayerCastSpell to set the spell that's being cast.*/
	public void SetSpellToCast(Spell spell)
	{
		this.m_SpellToCast = spell;
		this.m_SpellName = this.m_SpellToCast.m_SpellName.ToString ();
	}

    /**A function to be called whenever something enters a spellmovement collider; in terms of functionality, we'll use this function to destroy the spell object prefab after it strikes with something's collider.*/
    void OnTriggerEnter(Collider other)
    {
        //if we hit the target...
        if (other.gameObject == this.m_TargetedObj)
        {
            #if TESTING_SPELLCOLLISION
			string message = "SpellMovement::OnTriggerEnter(Collider)\tTarget " + this.m_Target.collider.gameObject.name + " hit!\n";

			if (other.gameObject.GetComponent<Enemy>() != null)
			{
				Enemy enemy = other.gameObject.GetComponent<Enemy>();
				enemy.ApplySpellEffects(this.m_SpellToCast.m_SpellName);
				message += "Subtracting enemy health...\n";
			}

			GameObject.Destroy(this.gameObject);
			message += "\nGameObject destroyed";

			Debug.Log(message);
            #endif
			return;
        }
		//if the spell hits a part of the scenery...
		if (other.gameObject.GetComponent<Obstructable> ()) {
			//...destroy it (we don't want stuff like spells going through trees
			GameObject.Destroy (this.gameObject);
		}

    }//end f'n void OnTriggerEnter(Collider)
}

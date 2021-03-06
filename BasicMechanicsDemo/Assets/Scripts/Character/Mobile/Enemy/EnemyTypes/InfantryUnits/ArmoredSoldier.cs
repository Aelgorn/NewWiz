﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoredSoldier : EnemyInfantry {
    /**The health of an armored soldier*/
    public float m_ArmoredSoldierHealth = 60.0f;
    /**The attack damage of an armored soldier*/
    public float m_ArmoredSoldierAttackDamage = 15.0f;
    /**The string value of the name of the sorting layer*/
    public string sortingLayerName;
    /**Length of time for which the soldier will chase the player if the player attacks the soldier without being detected first*/
    public float m_SoldierChasePlayerDuration = 5.0f;

    // Use this for initialization
    void Start () {
		this.SetHealth(this.m_ArmoredSoldierHealth);
        this.SetAttackDamageValue();
        this.SetChasePlayerSettings(this.m_SoldierChasePlayerDuration);
        this.gameObject.GetComponentInChildren<SpriteRenderer>().sortingLayerName = sortingLayerName;
		this.m_EnemyName = EnemyName.ARMORED_SOLDIER;
    }
	
	// Update is called once per frame
	void Update () {
		//Take care of movement and attack patterns
		base.Update ();
		//Manage death
		this.Die ();

        //This line of code is used to get the correct draw order.
        this.gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(this.GetComponent<Transform>().transform.position.z * 100f) * -1;
    }

    public override void SetAttackDamageValue()
    {
        this.m_AttackDamageValue = this.m_ArmoredSoldierAttackDamage;
    }

    public override float GetAttackDamageValue()
    {
        return this.m_AttackDamageValue;
    }

	public override void ApplySpellEffect (SpellClass spell)
	{
		base.ApplySpellEffect (spell);
	}
}

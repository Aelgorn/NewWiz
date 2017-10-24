﻿#define TESTING_SPELLMOVEMENT
#define TESTING_SPELLCOLLISION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Collider to ensure the spell can be picked up if it's in the world
[RequireComponent(typeof(Collider))]
public class Spell : MonoBehaviour {
	/**An enum variable for the spell name. See SpellName.cs for all spell names.*/
	public SpellName m_SpellName;
	/**An enum variable for the spell effect. See SpellEffecr.cs for all spell effects.*/
	public SpellEffect m_SpellEffect;
	/**A bool to tell us whether or not a spell is mobile.
	*A mobile spell is a spell that is cast by the player at an enemy (i.e. Fireball), that moves around.
	*An immobile spell is a spell that is cast by the player on the player (i.e. Heal, Shield), that stays fixed on the player.*/
	public bool m_IsMobileSpell;

	void Awake()
	{
		//ensure the box collider's isTrigger is set to true
		this.GetComponent<Collider> ().isTrigger = true;
	}//end f'n void Awake()

	/**Return a copy of [spell_to_copy].*/
	public Spell CopySpell(Spell spell_to_copy)
	{
		Spell new_spell = new Spell ();
		new_spell.m_IsMobileSpell = spell_to_copy.m_IsMobileSpell;
		new_spell.m_SpellEffect = spell_to_copy.m_SpellEffect;
		new_spell.m_SpellName = spell_to_copy.m_SpellName;
		return new_spell;
	}

	public void GenerateInstance_Fireball(bool is_mobile)
	{
		this.m_IsMobileSpell = is_mobile;
		this.m_SpellName = SpellName.Fireball;
		this.m_SpellEffect = SpellEffect.Fire_Damage;
	}

    public void GenerateInstance_IceBall(bool is_mobile)
    {
        this.m_IsMobileSpell = is_mobile;
        this.m_SpellName = SpellName.Iceball;
        this.m_SpellEffect = SpellEffect.Ice_Freeze;
    }
}

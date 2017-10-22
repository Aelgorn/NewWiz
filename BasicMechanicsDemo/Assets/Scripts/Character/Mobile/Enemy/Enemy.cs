﻿/**
* Unlike the Player class, it's very likely that the Enemy class will only wind up becoming a superclass.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float m_Health;

    public readonly float ENEMY_FULL_HEALTH = 100.0f;

    void Start()
    {
		m_Health = ENEMY_FULL_HEALTH;
    }
    void Update()
    {
        /**Should add animation for death here later.*/
        if(this.m_Health <= 0.0f)
        {
			Debug.Log ("Enemy dead!");
            GameObject.Destroy(this.gameObject);
        }
    }
    /**A function to add [effect] to the enemys's health.*/
    public void AffectHealth(float effect)
    {
        this.m_Health += effect;
    }//end f'n void AffectHealth(float)

//    /**Function which applies the effect of a spell on the enemy.
//     *Should make it abstract when we add a variety of enemies.*/
//    public void ApplySpellEffects(Spell hitSpell)
//    {
//		Debug.Log ("Applying spell " + hitSpell.m_SpellName.ToString() + " with damage " + hitSpell.m_SpellDamage);
//        if(hitSpell.m_SpellName == SpellName.Fireball)
//        {
//            /**Could add further spell effects here.*/
//            AffectHealth(-hitSpell.m_SpellDamage);
//        }
//    }

	/**Function which applies the effect of a spell on the enemy.
     *Should make it abstract when we add a variety of enemies.*/
	public void ApplySpellEffects(SpellName spell_name)
	{
		switch ((int)spell_name) {
		case (int)SpellName.Fireball:
			{
				this.AffectHealth (-10.0f);
				break;
			}
		default:
			{
				//Impossible, right now
				break;
			}
		}

		Debug.Log ("Applying spell " + spell_name.ToString() + " with damage " + 10.0f);

	}


}

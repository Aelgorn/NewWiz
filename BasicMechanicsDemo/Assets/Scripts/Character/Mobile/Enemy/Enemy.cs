﻿/**
* Unlike the Player class, it's very likely that the Enemy class will only wind up becoming a superclass.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public float m_Health;

    public readonly float ENEMY_FULL_HEALTH = 100.0f;

    /**Boolean to check if enemy can move. To be used for the IceBall effects*/
    public bool frozen { get; set; }
    /**Timer to be used for the freezing effect.*/
    private float freeze_Timer = 0.0f;

    void Start()
    {
		m_Health = ENEMY_FULL_HEALTH;
        frozen = false;
    }
    void Update()
    {
        /**Should add animation for death here later.*/
        if(this.m_Health <= 0.0f)
        {
			string message = "Enemy dead! ";
			//if the enemy has a parent with a detection area...
			if (this.gameObject.transform.GetComponentInParent<EnemyDetectionArea> () != null) {
				message += "Has enemy detection area; therefore destroying parent.";
				//...then destroy its parent, along with THIS gameobject
				GameObject.Destroy (this.gameObject.transform.parent.gameObject);
			} else {
				message += "Has no enemy detection area; therefore destroying THIS gameobject.";
				GameObject.Destroy(this.gameObject);
			}
			Debug.Log (message);
        }
        /**Checks if enemy is frozen.*/
        if (frozen)
        {
            this.gameObject.GetComponent<Animator>().enabled = false;
            freeze_Timer += Time.deltaTime;
            if(freeze_Timer >= 2.0f)
            {
                this.gameObject.GetComponent<Animator>().enabled = true;
                frozen = false;
            }
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
        case (int)SpellName.Iceball:
            {
                this.AffectHealth(-2.0f);
                this.frozen = true;
                freeze_Timer = 0.0f;
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

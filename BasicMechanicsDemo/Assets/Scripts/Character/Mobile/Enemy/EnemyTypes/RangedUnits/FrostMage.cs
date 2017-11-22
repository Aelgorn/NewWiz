﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostMage : RangedEnemy {
    /**The health of a FrostMage*/
    public float m_FrostMageHealth = 30.0f;

    public float m_FrostMageMeleeDamage = 2.0f;

    public float m_IntervalBetweenRangedAttacks = 2.0f;

    public float m_IntervalBetweenMeleeAttacks = 2.5f;

	// Use this for initialization
	void Start () {
        this.SetIntervalsBetweenAttacks();
        this.SetHealth();
        this.SetMeleeDamage();
        this.SetSpellToCast(this.m_AttackSpell);
        this.SetAttackDamageValue();

        //???
        this.m_Animator = this.GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        base.Update();
	}

    public override void SetSpellToCast(SpellName spell)
    {
        SpellClass spell_instance = new SpellClass();
        this.m_SpellToCast = spell_instance.GenerateInstance(spell);
        this.m_AttackPattern.m_SpellToCast = this.m_SpellToCast;
    }

    public override void SetHealth()
    {
        this.m_Health = this.m_FrostMageHealth;
    }

    public override void SetAttackDamageValue()
    {
        if (this.m_AttackPattern.m_AttackPatternState == AttackPatternState.RANGED)
        {
            this.m_AttackDamageValue = this.m_SpellToCast.m_SpellDamage;
        }
        else if (this.m_AttackPattern.m_AttackPatternState == AttackPatternState.MELEE)
        {
            this.m_AttackDamageValue = this.m_FrostMageMeleeDamage;
        }
    }


    protected override void SetMeleeDamage()
    {
        this.m_MeleeDamage = this.m_FrostMageMeleeDamage;
    }

    protected override void SetGeneratedSpellInstance()
    {
        this.m_GeneratedSpellCubeInstance = this.m_AttackPattern.m_GeneratedSpellInstance;
    }

    /**A function to set different intervals between each successive strike of a type of attack*/
    protected override void SetIntervalsBetweenAttacks()
    {
        this.m_MeleeAttackInterval = this.m_IntervalBetweenMeleeAttacks;
        this.m_RangedAttackInterval = this.m_IntervalBetweenRangedAttacks;
    }
}

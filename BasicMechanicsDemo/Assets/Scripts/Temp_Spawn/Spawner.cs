﻿/*
* A temp class for spawning items and whatever else could potentially be spawned, for testing purposes.
*/

#define TESTING_ITEM_PICKUP

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {
	#if TESTING_ITEM_PICKUP
	/**A reference to the default item prefab to be spawned, before it is set to represent a specific item.*/
	[SerializeField] private GameObject m_TempItemPrefab;
	/**A reference to the default spell prefab to be spawned, before it is set to represent a specific spell.*/
	[SerializeField] private GameObject m_TempSpellPrefab;
	/**A reference to the player, just so we can consistently spawn something close to the player's transform.position.*/
	[SerializeField] private GameObject m_Player;
	#endif

	/**A list of all Item instances we spawn.*/
	private List<Item> m_ItemInstances;
	/**A list of all Spell instances we spawn.*/
	private List<Spell> m_SpellInstances;

	void Start()
	{
		this.m_ItemInstances = new List<Item> ();
		this.m_SpellInstances = new List<Spell>();
	}

	// Update is called once per frame
	void Update () {
		#if TESTING_ITEM_PICKUP
		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			this.Spawn_Item_HealthPotion (this.m_Player.transform.position + Vector3.left * 2.0f);
		}
		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			this.Spawn_Spell_Fireball (this.m_Player.transform.position + Vector3.right * 2.0f);
		}
		#endif
	}

	/**Spawns a health potion at the given position.*/
	public void Spawn_Item_HealthPotion(Vector3 position)
	{
		GameObject health_potion = GameObject.Instantiate (this.m_TempItemPrefab);
		health_potion.transform.position = position;
		Item health_potion_item = health_potion.GetComponent<Item> ();
		health_potion_item = this.GenerateInstance_Item_HealthPotion ();
		this.m_ItemInstances.Add (health_potion_item);
	}//end f'n void Spawn_Item_HealthPotion(Vector3)

	public void Spawn_Spell_Fireball(Vector3 position)
	{
		GameObject fireball = GameObject.Instantiate (this.m_TempSpellPrefab);
		fireball.transform.position = position;
		Spell fireball_spell = fireball.GetComponent<Spell> ();
		fireball_spell = this.GenerateInstance_Spell_Fireball ();
		this.m_SpellInstances.Add (fireball_spell);
	}//end f'n void Spawn_Spell_Fireball(Vector3)

	/**A private function to spawn a default health potion item; returns the instance of the Item.*/
	private Item GenerateInstance_Item_HealthPotion()
	{
		Item health_potion = new Item ();
		health_potion.m_ItemEffect = ItemEffect.Gain_Health;
		health_potion.m_ItemName = ItemName.Health_Potion;
		return health_potion;
	}//end f'n Item GenerateInstance_Item_HealthPotion()

	/**A private function to spawn a default fireball spell; returns the instance of the Spell.*/
	private Spell GenerateInstance_Spell_Fireball()
	{
		Spell fireball = new Spell ();
		fireball.m_SpellName = SpellName.Fireball;
		fireball.m_SpellEffect = SpellEffect.Fire_Damage;
		fireball.m_HasBeenDiscovered = false;
		return fireball;
	}//end f'n Spell GenerateInstance_Spell_Fireball()
}

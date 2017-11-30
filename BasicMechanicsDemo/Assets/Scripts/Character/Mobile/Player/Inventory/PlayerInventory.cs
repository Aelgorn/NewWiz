#define TESTING_INVENTORY_CONTENTS_OUTPUT
//#define TESTING_INVENTORY_SPELLPICKUP
//#define TESTING_ACTIVE_SPELL
//#define TESTING_INVENTORY_ITEMPICKUP
//#define TESTING_ITEMPICKUP_HOTKEYUPDATE

//Spell macros

///*Activating this macro will enable the player to start the game with the Fireball spell.*/
#define START_WITH_FIREBALL
///*Activating this macro will enable the player to start the game with the Iceball spell.*/
//#define START_WITH_ICEBALL
///*Activating this macro will enable the player to start the game with the Shield spell.*/
//#define START_WITH_SHIELD
///*Activating this macro will enable the player to start the game with the Thunderball spell.*/
//#define START_WITH_THUNDERBALL
///*Activating this macro will enable the player to start the game with the Thunderstorm spell.*/
//#define START_WITH_THUNDERSTORM
///*Activating this macro will enable the player to start the game with the Heal spell*/
//#define START_WITH_HEAL
///*Activating this macro will enable the player to start the game with the Tornado spell*/
//#define START_WITH_TORNADO
///*Activating this macro will enable the player to start the game with the Waterbubble spell*/
//#define START_WITH_WATERBUBBLE

//Item macros

///*Activating this macro will enable the player to start the game with the health potion item.*/
//#define START_WITH_HEALTH_POTION
///*Activating this macro will enable the player to start the game with the mana potion item.*/
//#define START_WITH_MANA_POTION

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ensure the player's got his capsule collider
[RequireComponent(typeof(CapsuleCollider))]

public class PlayerInventory : MonoBehaviour {
	
	public Dictionary<ItemClass, int> m_ItemDictionary;

	/**A list of all SpellClass instances of spells in the inventory.*/
	public List<SpellClass> m_SpellClassList;
	/**A list of all quest items*/
	public List<QuestItem> m_QuestItems;

	/**A reference to a default spell prefab to contain all our active spell information.*/
	[SerializeField] public GameObject m_DefaultSpellPrefab;

	/**The active spell, of type SpellClass, used for firing spells.*/
	public SpellClass m_ActiveSpellClass = null;
	/**For testing purposes; the name of the active SpellClass instance.*/
	public string m_ActiveSpellName;

	/**A string variable containing the string name of the Input Manager variable responsible for the player switching their active spell.*/
	private readonly string STRINGKEY_INPUT_SWITCHSPELLS = "Switch Spells";

	/**A reference to the HotKey button gameobject representative of HotKey1*/
	[SerializeField] public UnityEngine.UI.Button m_HotKey1_Obj;
	/**A reference to the HotKey button gameobject representative of HotKey2*/
	[SerializeField] public UnityEngine.UI.Button m_HotKey2_Obj;
	/**A reference to the HotKey button gameobject representative of HotKey3*/
	[SerializeField] public UnityEngine.UI.Button m_HotKey3_Obj;

	private PlayerCastSpell m_PlayerCastSpell;


	public List<UnityEngine.UI.Button> m_HotKeyList = new List<UnityEngine.UI.Button>();

	[SerializeField] private ActiveSpellIcon m_ActiveSpellIcon;


    void Awake()
	{
		this.m_PlayerCastSpell = this.GetComponent<PlayerCastSpell> ();
		//Get a reference to each hotkey slot
//		this.m_HotKey1 = this.m_HotKey1_Obj.GetComponentInChildren<HotKeys> ();
		this.m_HotKeyList.Add (this.m_HotKey1_Obj);
//		this.m_HotKey2 = this.m_HotKey2_Obj.GetComponentInChildren<HotKeys> ();
		this.m_HotKeyList.Add (this.m_HotKey2_Obj);
//		this.m_HotKey3 = this.m_HotKey3_Obj.GetComponentInChildren<HotKeys> ();
		this.m_HotKeyList.Add (this.m_HotKey3_Obj);

		//Initialize Item Dictionary
		this.m_ItemDictionary = new Dictionary<ItemClass, int>();
		//Initialize spell list
		this.m_SpellClassList = new List<SpellClass>();
		//Initialize quest item list
		this.m_QuestItems = new List<QuestItem> ();
	}

	void Start()
	{


		SpellClass spell_class_instance = new SpellClass();
		#if START_WITH_FIREBALL
		this.AddSpell(spell_class_instance.GenerateInstance(SpellName.Fireball));
		this.AssignDefaultActiveSpell();
		#endif
		#if START_WITH_ICEBALL
		this.AddSpell (spell_class_instance.GenerateInstance(SpellName.Iceball));
		this.AssignDefaultActiveSpell();
		#endif
		#if START_WITH_SHIELD
		this.AddSpell(spell_class_instance.GenerateInstance(SpellName.Shield));
		this.AssignDefaultActiveSpell();
		#endif
		#if START_WITH_THUNDERBALL
		this.AddSpell(spell_class_instance.GenerateInstance(SpellName.Thunderball));
		this.AssignDefaultActiveSpell();
		#endif
		#if START_WITH_THUNDERSTORM
		this.AddSpell(spell_class_instance.GenerateInstance(SpellName.Thunderstorm));
		this.AssignDefaultActiveSpell();
		#endif
		#if START_WITH_HEAL
		this.AddSpell(spell_class_instance.GenerateInstance(SpellName.Heal));
		this.AssignDefaultActiveSpell();
        #endif
        #if START_WITH_TORNADO
        this.AddSpell(spell_class_instance.GenerateInstance(SpellName.Tornado));
		this.AssignDefaultActiveSpell();
        #endif
        #if START_WITH_WATERBUBBLE
        this.AddSpell(spell_class_instance.GenerateInstance(SpellName.WaterBubble));
		this.AssignDefaultActiveSpell();
        #endif


        #if START_WITH_HEALTH_POTION
        ItemClass health_potion = new ItemClass ();
		health_potion.GenerateInstance(ItemName.Health_Potion);
		this.AddItem(health_potion);
		#endif
		#if START_WITH_MANA_POTION
		ItemClass mana_potion = new ItemClass();
		mana_potion.GenerateInstance(ItemName.Mana_Potion);
		this.AddItem(mana_potion);
		#endif

		//In case we're loading in the scene
		if (this.m_ActiveSpellClass != null) {
			this.m_ActiveSpellIcon.UpdateActiveSpellSprite (this.m_ActiveSpellClass.m_SpellName);
		} else {
			this.m_ActiveSpellIcon.gameObject.transform.parent.gameObject.SetActive (false);
		}

	}//end f'n void Start()

	void Update()
	{

		#if TESTING_ITEMPICKUP_HOTKEYUPDATE
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (this.m_HotKeyList.Count > 0)
			{
				string message = "At least one hotkey; actual quantity: " + this.m_HotKeyList.Count +"\n";
				foreach(var hotkey in this.m_HotKeyList)
				{
					ItemClass item_in_hotkey = hotkey.GetComponentInChildren<HotKeys>().item;
					if (item_in_hotkey != null)
					{
						message += "Item in " + hotkey.name + ": " + item_in_hotkey.m_ItemName.ToString() + "\t";
					}
					else
					{
						message += "Hotkey " + hotkey.name + " empty\t";
					}
				}
				Debug.Log(message);
			}
			else
			{
				Debug.Log("No hotkeys??");
			}
		}
		#endif

//		this.m_DefaultSpellPrefab.GetComponent<SpellMovement> ().m_SpellClassToCast = this.m_ActiveSpellClass;
		//If there's more than one spell in the player's inventory...
		if (this.m_SpellClassList.Count > 1 && !PlayerCastSpell.m_MenuOpen) {
			//...then check for player input and switch SpellClass instance on command.
			this.UpdateActiveSpell ();
		} //end if
		//if there's only one spell in the player's inventory and an active SpellClass instance hasn't yet been set...
		else if (this.m_SpellClassList.Count == 1 && this.m_ActiveSpellClass == null)
		{
			//...then assign a default value for the active SpellClass
			this.AssignDefaultActiveSpell ();
			//Update sprite
			if (this.m_ActiveSpellClass != null) {
				this.m_ActiveSpellIcon.gameObject.transform.parent.gameObject.SetActive (true);
				this.m_ActiveSpellIcon.UpdateActiveSpellSprite (this.m_ActiveSpellClass.m_SpellName);
			}
		}
		#if TESTING_INVENTORY_CONTENTS_OUTPUT
		if (Input.GetKeyDown (KeyCode.Return)) {
			this.OutputInventoryContents();
		}
		#endif
	}//end f'n void Update()

	/**A function to update the player's active SpellClass instance with respect to mousewheel input.
	*Switches to next rightward spell if the input is forward; switches to next leftward spell if the input is backward.*/
	private void UpdateActiveSpell()
	{
		//the user needs to have at least two spells to be able to switch between them
		if (this.m_SpellClassList.Count > 1) {
			float input = Input.GetAxis (STRINGKEY_INPUT_SWITCHSPELLS);

			//If the user wants to switch their active spells...
			if (input != 0) {
				int next_index = 0;
				int index_of_active_spell = this.m_SpellClassList.IndexOf(this.m_ActiveSpellClass);
				//if the user scrolls forward...
				if (input > 0.0f) {
					//...then switch to the next rightward spell

					//if the index of the active spell is equal to that of the last spell in the list...
					if (index_of_active_spell == this.m_SpellClassList.Count - 1) {
						//...then the next rightward spell is the first one.
						next_index = 0;
					}//end if
					//else if the index of the active spell is not the last spell in the list...
					else {
						//...then the next index is simply one further on.
						next_index = index_of_active_spell + 1;
					}//end else
				}//end if
				//else if the user scrolls backward...
				else { //if input < 0.0f
					//...then switch to the next leftward spell

					//if the index of the active spell is equal to that of the first spell in the list...
					if (index_of_active_spell == 0) {
						//...then the next leftward spell is the last one.
						next_index = this.m_SpellClassList.Count - 1;
					}//end if
					//else if the index of the active spell is not that of the first spell in the list...
					else {
						//...then the next index is simply one preceding the one we're at.
						next_index = index_of_active_spell - 1;
					}//end else
				}//end else

				//Update active spell
				this.m_ActiveSpellClass = this.m_SpellClassList [next_index];
				this.m_ActiveSpellName = this.m_ActiveSpellClass.m_SpellName.ToString ();

				//Update active spell icon
				this.m_ActiveSpellIcon.UpdateActiveSpellSprite (this.m_ActiveSpellClass.m_SpellName);

				this.GetComponent<PlayerCastSpell> ().m_SpellClassToFire = this.m_ActiveSpellClass;

				#if TESTING_ACTIVE_SPELL
				Debug.Log ("UpdateActiveSpell::Active SpellClass: " + this.m_ActiveSpellClass.m_SpellName.ToString ());
				#endif
			}//end if
		}//end if
	}//end f'n void UpdateActiveSpell()

	/**A function to assign the default active SpellClass instance of the player.
	*Assumes the size of the spell class list is equal to 1.*/
	private void AssignDefaultActiveSpell()
	{
		this.m_ActiveSpellClass = this.m_SpellClassList [0];
		this.m_ActiveSpellName = this.m_ActiveSpellClass.m_SpellName.ToString ();

		this.GetComponent<PlayerCastSpell> ().m_SpellClassToFire = this.m_ActiveSpellClass;
//		Debug.Log ("Spell to fire " + this.GetComponent<PlayerCastSpell> ().m_SpellClassToFire.m_SpellName.ToString ());
	}//end f'n void AssignDefaultActiveSpell()

	/**A function to return the string containing all of the SpellClass instances of the [this.m_SpellClassList]*/
	private string OutputSpellClassList()
	{
		string all_messages = "";
		foreach (SpellClass spell_class in this.m_SpellClassList) {
			all_messages += spell_class.ReturnSpellInstanceInfo () + "\n";
		}
		return all_messages;
	}

	/**A function, for testing purposes, to print out the inventory contents.*/
	public void OutputInventoryContents()
	{
		string message = "Inventory Contents:\n";
		message += "Spells:\n";
		message += this.OutputSpellClassList ();

		message += "Items:\n";
		foreach (KeyValuePair<ItemClass, int> entry in this.m_ItemDictionary) {
			message += "Item name: " + entry.Key.m_ItemName.ToString () + "\tItem Effect: " + entry.Key.m_ItemEffect.ToString () + "\tItem Quantity:\t" + entry.Value + "\n";
		}//end foreach
		message += "Active spell:\t";
		if (this.m_ActiveSpellClass == null) {
			message += "None";
		} else {
			message += this.m_ActiveSpellClass.ReturnSpellInstanceInfo();
		}
		message += "\nQuest Items:\n";
		foreach (QuestItem item in this.m_QuestItems) {
			message += "\tItem:\t" + item.m_QuestItemName.ToString () + "\tCollected? " + item.m_IsCollected + "\n";
		}//end foreach
		Debug.Log(message);
	}//end f'n void OutputInventoryContents()

	/**A function to add the item to the inventory.*/
	public void AddItem(ItemClass item_to_add)
	{
		#if TESTING_INVENTORY_ITEMPICKUP
		string message = "";
		message += "PlayerInventory::Adding item " + item_to_add.m_ItemName.ToString () +"\t";
		#endif

		bool match_found = false;
		foreach (KeyValuePair<ItemClass, int> entry in this.m_ItemDictionary) {
			if (item_to_add.m_ItemName.ToString () == entry.Key.m_ItemName.ToString ()) {
				match_found = true;
				this.m_ItemDictionary [entry.Key]++;
				#if TESTING_INVENTORY_ITEMPICKUP
				message += "item " + item_to_add.m_ItemName.ToString () + " found in inventory; quantity: " + this.m_ItemDictionary[entry.Key];
				Debug.Log(message);
				#endif
				break;
			}//end if
		}//end foreach
		#if TESTING_INVENTORY_ITEMPICKUP
		message += "item " + item_to_add.m_ItemName.ToString () + " not found in inventory; adding...";
		Debug.Log(message);
		#endif
		if (!match_found) {
			this.m_ItemDictionary.Add(item_to_add, 1);
		}
		#if TESTING_ITEMPICKUP_HOTKEYUPDATE
		string message = "Foreach starting...\n";
		#endif
		foreach(var hotkey in this.m_HotKeyList)
		{
			ItemClass item_in_hotkey = hotkey.GetComponentInChildren<HotKeys>().item;
			if (item_in_hotkey != null)
			{
				#if TESTING_ITEMPICKUP_HOTKEYUPDATE
				message += "Item in " + hotkey.name + ": " + item_in_hotkey.m_ItemName.ToString();
				#endif

				UnityEngine.UI.Text text_element = hotkey.GetComponentInChildren<UnityEngine.UI.Text> ();

				#if TESTING_ITEMPICKUP_HOTKEYUPDATE
				message += " (" + text_element.text + ")\t";
				message += "Item ";
				#endif
				if ((int)item_to_add.m_ItemName == (int)item_in_hotkey.m_ItemName) {
					#if TESTING_ITEMPICKUP_HOTKEYUPDATE
					message += "found in hotkey " + hotkey.name + "\t";
					#endif

					int current_quantity = int.Parse (text_element.text);
					int new_quantity = current_quantity + 1;
					string new_text = new_quantity + "";
					text_element.text = new_text;
					#if TESTING_ITEMPICKUP_HOTKEYUPDATE
					message += "new quantity: " + new_text + "\n";
					#endif
				}

			}
			else
			{
				#if TESTING_ITEMPICKUP_HOTKEYUPDATE
				message += "Hotkey " + hotkey.name + " empty\n";
				#endif
			}
		}
		#if TESTING_ITEMPICKUP_HOTKEYUPDATE
		Debug.Log (message);
		#endif

	}//end f'n void AddItem(Item)


	/**A function to add a SpellClass object to the SpellClass list if the SpellClass is not already in the list.*/
	public void AddSpell(SpellClass spell_to_add)
	{
		#if TESTING_INVENTORY_SPELLPICKUP
		string message = "";
		message += "PlayerInventory::Adding spell " + spell_to_add.m_SpellName.ToString () +"\t";
		#endif
		foreach (SpellClass spell in this.m_SpellClassList) {
			if (spell.m_SpellName.ToString () == spell_to_add.m_SpellName.ToString ()) {

				#if TESTING_INVENTORY_SPELLPICKUP
				message += "spell " + spell_to_add.m_SpellName.ToString () +" already found in inventory; making no changes";
				Debug.Log(message);
				#endif
				return;
			}
		}
		#if TESTING_INVENTORY_SPELLPICKUP
		message += "spell " + spell_to_add.m_SpellName.ToString () +" not already found in inventory; adding...";
		Debug.Log(message);
		#endif
		this.m_SpellClassList.Add (spell_to_add);

	}//end f'n void AddSpell(SpellClass)

	/**A function to decrement the item dictionary entry with key [item]; returns false if the entry can't be decremented and elsewis returns true.*/
	public bool useItem(ItemClass item)
	{
		Debug.Log ("Is menu open? " + PlayerCastSpell.m_MenuOpen);
		if (PlayerCastSpell.m_MenuOpen) {
			return false;
		}
		foreach (KeyValuePair<ItemClass, int> entry in m_ItemDictionary) 
		{
			if (entry.Key.m_ItemName == item.m_ItemName) 
			{
				if (entry.Value > 0) 
				{
					m_ItemDictionary [entry.Key]--;
					return true;
				} 
				else
					return false;
			}
		}
		return false;
	}

	/**A function to apply the effects of consuming an item, after having checked if consumption of that item is possible, and then subtracting from the item quantity to show the item's been consumed.*/
	public void itemEffectUsage(ItemClass item)
	{
		
		if (useItem (item)) 
		{
			switch (item.m_ItemEffect) 
			{
			case ItemEffect.Gain_Health:
				Debug.Log ("Using item " + item.m_ItemName.ToString ());
				gameObject.GetComponent<Player> ().AffectHealth (item.effectAmount);
				break;
			case ItemEffect.Gain_Mana:
				gameObject.GetComponent<Player> ().AffectMana (item.effectAmount);
				break;
			}//end switch
		}//end if
	}

	/**A function to return the number of the given item in the item dictionary.
	*Returns -1 if the item is not found in the dictionary*/
	public int getNumberItem(ItemClass item)
	{
//		return m_ItemDictionary [item];
		foreach (KeyValuePair<ItemClass, int> entry in this.m_ItemDictionary) {
			if ((int)entry.Key.m_ItemName == (int)item.m_ItemName) {
				return entry.Value;
			}
		}

		return -1;
	}


}//end class PlayerInventory

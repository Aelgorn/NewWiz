﻿/*
* A script to manage the player's status.
*/

//#define TESTING_ZERO_HEALTH
#define TESTING_MANA_REGEN
#define TESTING_REGION
//#define TESTING_ENABLE_RESURRECTION
//#define TESTING_SET_INITIAL_REGION_TO_DEMO
//A macro to leave player position unmodified on scene load (to avoid going by the menu every time)
//#define TESTING_OVERRIDEPOSITIONING
#define TESTING_PRINT_ACTIVE_SCENE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour, ICanBeDamagedByMagic {
	[SerializeField] public GameObject healthMeter;
	[SerializeField] public GameObject manaMeter;

	/**A reference to the HotKey button gameobject representative of HotKey1*/
	[SerializeField] public UnityEngine.UI.Button m_HotKey1_Obj;
	/**A reference to the HotKey button gameobject representative of HotKey2*/
	[SerializeField] public UnityEngine.UI.Button m_HotKey2_Obj;
	/**A reference to the HotKey button gameobject representative of HotKey3*/
	[SerializeField] public UnityEngine.UI.Button m_HotKey3_Obj;

	/**A string value of the input axis name for HotKey1*/
	private readonly string STRINGKEY_INPUT_HOTKEY1 = "HotKey1";
	/**A string value of the input axis name for HotKey2*/
	private readonly string STRINGKEY_INPUT_HOTKEY2 = "HotKey2";
	/**A string value of the input axis name for HotKey3*/
	private readonly string STRINGKEY_INPUT_HOTKEY3 = "HotKey3";

	private HotKeys m_HotKey1;
	private HotKeys m_HotKey2;
	private HotKeys m_HotKey3;

	public AudioSource m_audioSource;

	[SerializeField] public PlayerAudio m_PlayerAudio;

	[SerializeField] public Serialization_Manager m_SerializationManager;

	/**A variable to keep track of the player's health.*/
	public float m_Health;
	/**A variable to keep track of the player's mana.*/
	public float m_Mana;
	/**A variable to help us know what the player's full health is; public for accessibility in the Item classes (think health potions).*/
	public float PLAYER_FULL_HEALTH = 100.0f;
	/**A variable to help us know what the player's full mana count is.*/
	public float PLAYER_FULL_MANA = 100.0f;
	/**A bool to tell us whether or not the player is capable of being damaged.*/
	public bool m_IsShielded = false;
	/**A bool to tell us whether or not the player is capable of casting spells, with respect to mana.*/
	public bool m_CanCastSpells = true;
	/**A multiplier to help with mana regeneration.*/
	public float m_ManaRegenMultiplier = 1.125f;
	/**A bool to let us know whether or not we're alive.*/
	public bool m_IsAlive = false;
	/**The player animator*/
	private Animator m_Animator;
	/**The string value of the player animator parameter isAlive*/
	public readonly string STRINGKEY_PARAM_ISALIVE = "isAlive";
	/**A vector to contain the player's respawn position*/
	public Vector3 m_PlayerRespawnPosition = new Vector3(-6.5f, 0.55f, -0.43f);
    /**The string value of the name of the sorting layer*/
    public string sortingLayerName;
	/**A bool to tell us whether or not the player is currently affected by a spell.*/
	public bool m_IsAffectedBySpell = false;
	/**A timer to keep track of the active spell effects. Should be made private in final iteration.*/
	public float m_ExtraEffectsTimer = 0.0f;
	/**The spell we need to apply to the player (the spell affecting the player if they get hit by a hostile spell)*/
	protected SpellClass m_SpellAffectingPlayer = new SpellClass ();
	/**A multiplier to influence magic damage as the player gets stronger.*/
	public float m_MagicAffinity = 1.0f;

//	public Scenes m_CurrentRegion = Scenes.DEMO_AREA;
	public static Scenes m_CurrentRegion = Scenes.DEMO_AREA;

	public RPGTalk rpgTalk;


	public bool IsAffectedByMagic()
	{
		return this.m_IsAffectedBySpell;
	}

	void Awake()
	{
		this.m_HotKey1 = this.m_HotKey1_Obj.GetComponentInChildren<HotKeys> ();
		this.m_HotKey2 = this.m_HotKey2_Obj.GetComponentInChildren<HotKeys> ();
		this.m_HotKey3 = this.m_HotKey3_Obj.GetComponentInChildren<HotKeys> ();

		this.m_MagicAffinity = 1.0f;
	}

	void Start()
	{
		this.m_Animator = this.GetComponent<Animator> ();
		this.m_audioSource = GetComponent<AudioSource> ();
//		//Start off with full health
//		this.m_Health = PLAYER_FULL_HEALTH;
//		setMaxMeter (healthMeter, this.m_Health);
//		setMeterValue (healthMeter, this.m_Health);
//		
//		//Start off with full mana
//		this.m_Mana = PLAYER_FULL_MANA;
//		setMaxMeter (manaMeter, this.m_Mana);
//		setMeterValue (manaMeter, this.m_Mana);
        //The sorting layer name is retrieved from unity
        this.gameObject.GetComponentInChildren<SpriteRenderer>().sortingLayerName = sortingLayerName;

		int user_menu_choice = UnityEngine.PlayerPrefs.GetInt (MainMenu_UIManager.STRINGKEY_PLAYERPREF_LOADGAME);
		#if TESTING_OVERRIDEPOSITIONING
		#else
		//if we're not starting a new game and nor are we loading, via a menu specifically (meaning we're loading during a scene transition)
		if (user_menu_choice == 0) {
			int current_scene_build_index = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().buildIndex;
			Debug.Log ("Loading scene from Player class on transition from scene " + (int)m_CurrentRegion + " to scene " + current_scene_build_index);
			//if the index of the current scene is not equal to the index of the player's current region on start,
			//in the context of our project, it means that we just went from one region to another.
			if (current_scene_build_index != (int)m_CurrentRegion) {
				//So at this point, we need to spawn the player at a given region entrance, with respect to the last region
				//			Debug.Log ("Going into region: " + current_scene_build_index);
				//Find new spawn position

				this.PositionPlayerAtEntrance((int)m_CurrentRegion, current_scene_build_index);
				m_CurrentRegion = ReturnSceneAtIndex (current_scene_build_index);

				this.m_SerializationManager.Save();
			}
			//Update playerpref
//			UnityEngine.PlayerPrefs.SetInt (MainMenu_UIManager.STRINGKEY_PLAYERPREF_LOADGAME, 0);
		}
		#endif
		//else if we're starting a new game
		if (user_menu_choice == 1) {
			//Start off with full health
			this.m_Health = PLAYER_FULL_HEALTH;
			setMaxMeter (healthMeter, this.m_Health);
			setMeterValue (healthMeter, this.m_Health);

			//Start off with full mana
			this.m_Mana = PLAYER_FULL_MANA;
			setMaxMeter (manaMeter, this.m_Mana);
			setMeterValue (manaMeter, this.m_Mana);

			#if TESTING_SET_INITIAL_REGION_TO_DEMO
			Debug.Log("Player::Starting new game; setting player test region to DEMO AREA and position to Respawn Position");
			m_CurrentRegion = Scenes.DEMO_AREA;
			this.transform.position = this.m_PlayerRespawnPosition;
			#else
			Player.m_CurrentRegion = Scenes.FOREST;
			Vector3 starting_position = new Vector3(-28.7f, 0.55f, 2.51f);//Where does the player start?
			this.transform.position = starting_position;
//			this.m_PlayerRespawnPosition = starting_position;
			#endif
			//Update playerpref
			UnityEngine.PlayerPrefs.SetInt (MainMenu_UIManager.STRINGKEY_PLAYERPREF_LOADGAME, 0);
		}


    }

	/**A function to return the Scenes enum variable with the corresponding [index] value*/
	public static Scenes ReturnSceneAtIndex(int index)
	{
		foreach (Scenes scene in System.Enum.GetValues(typeof(Scenes))) {
			if ((int)scene == index) {
				return scene;
			}
		}
		//Pretty much impossible, so return default value
		Debug.Log("Player::ReturnSceneAtIndex(int) impossible case reached. Program fucked.");
		return Scenes.DEMO_AREA;
	}

	/**We place the player gameobject in the scene on start, with respect to the scene we came from and the one in which we now find ourselves.
	*In this scenario [index_from_region] is the region we came from (so it'll still be considered the current region according to the player's variables. [index_to_region] is the build index of the sene in which we now find ourselves.)*/
	private void PositionPlayerAtEntrance (int index_from_region, int index_to_region)
	{
		Vector3 position_to_spawn_player = new Vector3 ();
		switch (index_from_region) {
		//if we're going from the demo area...
		case (int)Scenes.DEMO_AREA:
			{
				switch(index_to_region)
				{
				//...to the overworld
				case (int)Scenes.OVERWORLD:
					{
                        m_CurrentRegion = Scenes.OVERWORLD;
						//then the position to spawn at is as follows:
						position_to_spawn_player = TransitionPositions.Transition_Demo_To_Overworld;
						break;
					}//end case to OVERWORLD
				}
				break;
			}//end case from DEMO AREA
			//if we're going from the forest...
		case (int)Scenes.FOREST:
			{
				switch (index_to_region) {
				//...to the overworld
				case (int)Scenes.OVERWORLD:
					{
						m_CurrentRegion = Scenes.OVERWORLD;
						//then the position to spawn at is as follows:
						position_to_spawn_player = TransitionPositions.Transition_Forest_To_Overworld;
						break;
					}//end case to overworld
				}//end switch
				break;
			}//end case from forest
		//if we're going from the overworld...
		case (int)Scenes.OVERWORLD:
			{
				//...to the demo area
				switch (index_to_region) {
				case (int)Scenes.DEMO_AREA:
					{
                        m_CurrentRegion = Scenes.DEMO_AREA;
						//then the position to spawn at is as follows:
						position_to_spawn_player = TransitionPositions.Transition_Overworld_To_Demo;
						break;
					}//end case to DEMO AREA
					//...to Soirhbeach
                case (int)Scenes.SOIRHBEACH:
					{
                        m_CurrentRegion = Scenes.SOIRHBEACH;
						//then the position to spawn at is as follows:
						position_to_spawn_player = TransitionPositions.Transition_Overworld_To_Left_Soirhbeach;
						break;
					}//end case to soirhbeach
					//...to the castle
                case (int)Scenes.CASTLE:
					{
                        m_CurrentRegion = Scenes.CASTLE;
						//then the position to spawn at is as follows:
						position_to_spawn_player = TransitionPositions.Transition_Overworld_To_Castle;
						break;
					}//end case to castle
					//...to the forest
				case (int)Scenes.FOREST:
					{
						m_CurrentRegion = Scenes.FOREST;
						//then the position to spawn at is as follows:
						position_to_spawn_player = TransitionPositions.Transition_Overworld_To_Forest;
						break;
					}//end case to forest
				}//end switch
				break;
			}//end case from OVERWORLD
			//if we're going from soirhbeach...
        case(int)Scenes.SOIRHBEACH:
                {
                    switch (index_to_region)
                    {
                        //...to the overworld
                        case (int)Scenes.OVERWORLD:
                            {
                                m_CurrentRegion = Scenes.OVERWORLD;
                                //then the position to spawn at is as follows:
                                position_to_spawn_player = TransitionPositions.Transition_Soirhbeach_Left_To_Overworld;
                                break;
                            }//end case to OVERWORLD
                    }
                    break;
                }
        case(int)Scenes.CASTLE:
                {
                    
                    switch (index_to_region)
                    {
                        //...to the overworld
                        case (int)Scenes.OVERWORLD:
                            {
                                m_CurrentRegion = Scenes.OVERWORLD;
                                //then the position to spawn at is as follows:
                                position_to_spawn_player = TransitionPositions.Transition_Castle_To_Overworld;
                                break;
                            }//end case to OVERWORLD
                    }
                    break;
                }
		}//end switch
		this.transform.position = position_to_spawn_player;
	}

	void Update()
	{
		#if TESTING_PRINT_ACTIVE_SCENE
		if (Input.GetKeyDown (KeyCode.F)) {
			Debug.Log ("Active scene: " + ReturnSceneAtIndex (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().buildIndex).ToString ());
			Debug.Log("Player current region: " + m_CurrentRegion.ToString());
		}
		#endif

		if (PlayerInteraction.m_IsTalking) {
//			Player cast spell will not run if m_IsTalking is set to true
			//so all that's left to worry about is player movement
			this.GetComponent<PlayerMovement> ().DisableMovement ();
		} else {
			this.GetComponent<PlayerMovement> ().EnableMovement ();
		}

		if (this.m_IsAffectedBySpell) {
			this.ApplySpellEffect (this.m_SpellAffectingPlayer);
		}

		//check for input from the player for use of hotkeyed items
		this.CheckForHotKeyButtonInput ();
        
        //This line calculates the sorting order value for the sprite renderer while in play.
        this.gameObject.GetComponentInChildren<SpriteRenderer>().sortingOrder = Mathf.RoundToInt(this.GetComponent<Transform>().transform.position.z * 100f) * -1;
        if (this.m_Health <= 0)
        {
			#if TESTING_ZERO_HEALTH
			Debug.Log("Zero health; player dead\tResurrection time!");
			this.m_Health = PLAYER_FULL_HEALTH;
			#endif

			setMeterValue(healthMeter, this.m_Health);
			this.m_IsAlive = false;
		}//end if

		this.UpdateCanCastSpell ();

		if (this.m_Mana < PLAYER_FULL_MANA) {
			this.m_Mana += Time.deltaTime * this.m_ManaRegenMultiplier;
			if (this.m_Mana > PLAYER_FULL_MANA) {
				this.m_Mana = PLAYER_FULL_MANA;
			}//end if
			setMeterValue (manaMeter, this.m_Mana);
		}//end if

		this.UpdateAnimatorParameters ();

	}//end f'n void Update



	private void UpdateCanCastSpell()
	{
		SpellClass active_spell = this.GetComponent<PlayerInventory> ().m_ActiveSpellClass;
		if (active_spell != null) {
			if (active_spell.m_IsPersistent) {
				this.m_CanCastSpells = (this.m_Mana >= active_spell.m_ManaCost * Time.deltaTime);
			} else {
				this.m_CanCastSpells = this.m_Mana >= active_spell.m_ManaCost;
			}
		}
	}

	/**A function to increase the player's magic affinity*/
	public void AddToMagicAffinity(float addition)
	{
		this.m_MagicAffinity += addition;
	}

	/**A function to check for keyboard input for use of hotkeyed items.*/
	private void CheckForHotKeyButtonInput()
	{
		//if the user hits the input button for HotKey1 item consumption...
		if (Input.GetButtonDown (STRINGKEY_INPUT_HOTKEY1)) {
			this.m_HotKey1.useItem ();
		}//end if
		//else if the user hits the input button for HotKey2 item consumption...
		else if (Input.GetButtonDown (STRINGKEY_INPUT_HOTKEY2)) {
			this.m_HotKey2.useItem ();
		}//end else if
		//else if the user hits the input button for HotKey3 item consumption...
		else if (Input.GetButtonDown (STRINGKEY_INPUT_HOTKEY3)) {
			this.m_HotKey3.useItem ();
		}//end else if
		//else do nothing
	}//end f'n void CheckForHotKeyButtonInput()

	private void UpdateAnimatorParameters()
	{
		this.m_Animator.SetBool (STRINGKEY_PARAM_ISALIVE, this.m_IsAlive);
	}

	/**A function to resurrect the player to their respawn point*/
	private void Resurrect()
	{
		this.m_IsAffectedBySpell = false;
		this.m_IsAlive = true;
		this.m_Health = this.PLAYER_FULL_HEALTH;
		this.setMeterValue(healthMeter, this.m_Health);
		this.m_Mana = this.PLAYER_FULL_MANA;
		this.setMeterValue (manaMeter, this.m_Mana);
		this.transform.position = this.m_PlayerRespawnPosition;
	}

	/**A function to add [effect] to the player's health.*/
	public void AffectHealth(float effect)
	{
		//if the player's receiving damage...
		if (effect < 0.0f) {
			//...and if the player's shielded...
			if (this.m_IsShielded) {
				//...then nullify damage
				return;
			}
		}
		this.m_Health += effect;
		setMeterValue (healthMeter, this.m_Health);
		if (this.m_Health <= 0.0f) {
			#if TESTING_ENABLE_RESURRECTION
			this.Resurrect ();
			#else
			UnityEngine.SceneManagement.SceneManager.LoadScene((int)Scenes.GAME_OVER);
			#endif
		}

	}//end f'n void AffectHealth(float)

	public void AffectMana(float effect)
	{
		this.m_Mana += effect;
		setMeterValue (manaMeter, this.m_Mana);
	}
		
	public void setMaxMeter(GameObject meter, float value)
	{
		meter.GetComponent<Slider> ().maxValue = value;
	}

	public void setMeterValue(GameObject meter, float value)
	{
		meter.GetComponent<Slider> ().value = value;
	}

	public void ApplySpellEffect (SpellClass spell)
	{
		if (this.m_ExtraEffectsTimer == 0.0f) {
			this.m_IsAffectedBySpell = true;
			this.m_SpellAffectingPlayer = spell;
			if (!spell.m_IsPersistent) {
				this.AffectHealth (-spell.m_SpellDamage);
			}
			this.m_ExtraEffectsTimer += Time.deltaTime;

		} else if (0.0f < this.m_ExtraEffectsTimer && this.m_ExtraEffectsTimer < spell.m_EffectDuration) {
			//Apply whatever needs to happen here
			switch ((int)spell.m_SpellType) {
			case (int)SpellType.AOE_ON_TARGET:
				{
					if (spell.m_IsPersistent) {
						this.AffectHealth ((-spell.m_SpellDamage / spell.m_EffectDuration) * Time.deltaTime);
					}
					break;
				}
			}
			this.m_ExtraEffectsTimer += Time.deltaTime;
		} else if (this.m_ExtraEffectsTimer >= spell.m_EffectDuration) {
			this.m_IsAffectedBySpell = false;
			this.m_SpellAffectingPlayer = null;
			this.m_ExtraEffectsTimer = 0.0f;
		}

	}

	public SpellClass SpellAffectingCharacter ()
	{
		return m_SpellAffectingPlayer;
	}

	public void playSound(AudioClip clip)
	{
		Debug.Log ("Play pickup sound");
		m_audioSource.PlayOneShot (clip);
	}
}


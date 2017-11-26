﻿#define TESTING_SAVE_LOAD

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

public class Serialization_Manager : MonoBehaviour {
//	[SerializeField] GameObject m_PlayerPrefab;
	[SerializeField] public Player m_Player;
	[SerializeField] QuestManager m_QuestManager;

	public Serializable_Session m_SerializableSession = new Serializable_Session();

	private readonly string FILEPATH_EXTENSION = "/SavedGame.gd";

	//Executed every time the game scene is loaded.
	void Start()
	{
		Debug.Log ("Am I being called?");
		//If a saved game file exists...
		if (File.Exists (Application.persistentDataPath + FILEPATH_EXTENSION)) {
			//...and if the user chose load
			if (UnityEngine.PlayerPrefs.GetInt (MainMenu_UIManager.STRINGKEY_PLAYERPREF_LOADGAME) == 2) {
				this.Load ();
				float x = 0.0f, y = 0.0f, z = 0.0f;
				x = this.m_SerializableSession.m_SerializablePlayer.m_PlayerPositionInWorld_X;
				y = this.m_SerializableSession.m_SerializablePlayer.m_PlayerPositionInWorld_Y;
				z = this.m_SerializableSession.m_SerializablePlayer.m_PlayerPositionInWorld_Z;
				this.m_Player.transform.position = new Vector3(x, y, z);

				//Reset player pref for next interaction with main menu UI
				UnityEngine.PlayerPrefs.SetInt (MainMenu_UIManager.STRINGKEY_PLAYERPREF_LOADGAME, 0);
			}//end if
			//...or if the user chose new game
			//We should probably implement this in the spawner class.
			//i.e. if saved file exists and user chose new game, then spawn everything for the Coille forest region, 
			//setting all player variables to default values
		}
	}


	public void Save() {
//		Serializable_Player SP = new Serializable_Player ();
//		this.m_SerializablePlayer = this.m_SerializablePlayer.GenerateSerializableInstance(this.m_Player);
//		this.m_SerializableQuestManager.ParseAllQuestStates(m_QuestManager);
		this.m_SerializableSession.GatherSessionInformation(m_Player.gameObject, m_QuestManager);
		BinaryFormatter bf = new BinaryFormatter();
		Debug.Log (Application.persistentDataPath);
		FileStream file = File.Create (Application.persistentDataPath + FILEPATH_EXTENSION);
//		bf.Serialize(file, m_SavedSessions);
//		bf.Serialize(file, this.m_SerializablePlayer);
//		bf.Serialize(file, this.m_SerializableQuestManager);
		bf.Serialize(file, this.m_SerializableSession);
		file.Close();
	}	

	public void Load() {
		if(File.Exists(Application.persistentDataPath + FILEPATH_EXTENSION)) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + FILEPATH_EXTENSION, FileMode.Open);
//			this.m_SavedSessions = (List<Serializable_Player>)bf.Deserialize(file);
//			this.m_SerializablePlayer = (Serializable_Player)bf.Deserialize(file);
//			this.m_SerializableQuestManager = (Serializable_QuestManager)bf.Deserialize(file);
			this.m_SerializableSession = (Serializable_Session)bf.Deserialize(file);
			file.Close();

			//Set all player information
			this.m_SerializableSession.SetSessionInformation(this.m_Player.gameObject, this.m_QuestManager);
			//Spawn in quest objects
			this.SpawnAllQuestObjects();
		}
	}

	private void SpawnAllQuestObjects()
	{

		//for each quest in the quest list...
		for (int quest_index = 0; quest_index < System.Enum.GetValues (typeof(QuestName)).Length; quest_index++) {
			Quest current_quest = this.m_QuestManager.m_AllQuests[m_QuestManager.m_AllQuests.ElementAt(quest_index).Key];
			//We only want to spawn in the quests that are in the same region as us
			if ((int)current_quest.m_QuestRegion == (int)this.m_Player.m_CurrentRegion) {
				//... and we only care about the quest if it's in process
				if ((int)current_quest.m_QuestState == (int)QuestState.IN_PROCESS) {
					//...and if so, then spawn corresponding quest objects
					this.m_QuestManager.SpawnInQuestObjects (current_quest);
				}
			}
		}
	}

	
	// Update is called once per frame
	void Update () {
		#if TESTING_SAVE_LOAD
		if (Input.GetKeyDown (KeyCode.Alpha6)) {
			Save ();
		}
		if (Input.GetKeyDown (KeyCode.Alpha7)) {
			Load ();
		}
		#endif
	}
}

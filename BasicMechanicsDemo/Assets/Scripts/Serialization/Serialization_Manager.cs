﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Serialization_Manager : MonoBehaviour {
	[SerializeField] GameObject m_Player;
	[SerializeField] QuestManager m_QuestManager;

	public Serializable_Session m_SerializableSession = new Serializable_Session();

	private readonly string FILEPATH_EXTENSION = "/SavedGame.gd";

//	public Serializable_Player m_SerializablePlayer = new Serializable_Player();
//	public Serializable_QuestManager m_SerializableQuestManager = new Serializable_QuestManager();

	public void Save() {
//		Serializable_Player SP = new Serializable_Player ();
//		this.m_SerializablePlayer = this.m_SerializablePlayer.GenerateSerializableInstance(this.m_Player);
//		this.m_SerializableQuestManager.ParseAllQuestStates(m_QuestManager);
		this.m_SerializableSession.GatherSessionInformation(m_Player, m_QuestManager);
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
			this.m_SerializableSession.SetSessionInformation(this.m_Player, this.m_QuestManager);
		}
	}


	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha6)) {
			Save ();
		}
		if (Input.GetKeyDown (KeyCode.Alpha7)) {
			Load ();
		}
	}
}

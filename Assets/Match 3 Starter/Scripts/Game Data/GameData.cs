using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public class SaveData
{
    public bool[] isActive;
    public int[] highScores;
    public int[] stars;
    public bool[] isCompleted; 
}


public class GameData : MonoBehaviour
{
    public static GameData Instance;
    public SaveData saveData;

    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Load();
    }

    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/playerData", FileMode.Create);
        SaveData data = new SaveData();
        data = saveData;
        formatter.Serialize(file, data);
        file.Close();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnDisable()
    {
            Save();        
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/playerData"))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerData", FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
        }
        else
        {
            saveData = new SaveData();
            saveData.isActive = new bool[100];
            saveData.stars = new int[100];
            saveData.highScores = new int[100];
            //for (int i = 0; i < 8; i++) //Use this to unlock levels for build
            //{
            //    saveData.isActive[i] = true;
            //}
            saveData.isActive[0] = true;
            saveData.isCompleted = new bool[100];
        }
    }
}

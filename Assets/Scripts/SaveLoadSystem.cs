namespace redd096
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public enum SaveFolder
    {
        persistentDataPath, gameFolder, nothing
    }

    [System.Serializable]
    public class ClassToSave
    {
        public List<Coordinates> coordinates = new List<Coordinates>();
        public List<bool> isAlive = new List<bool>();

        public ClassToSave(List<Cell> cells)
        {
            //foreach cell, save start coordinates and values
            foreach(Cell cell in cells)
            {
                coordinates.Add(cell.startCoordinates);
                isAlive.Add(cell.IsAlive);
            }
        }
    }

    [AddComponentMenu("redd096/Singletons/Save and Load System")]
    public class SaveLoadSystem : Singleton<SaveLoadSystem>
    {
        [Header("Data Directory")]
        [SerializeField] SaveFolder saveFolder = SaveFolder.persistentDataPath;
        [SerializeField] string directoryName = "Saves";

        public string PathDirectory
        {
            get
            {
                if (saveFolder == SaveFolder.persistentDataPath)
                    return Path.Combine(Application.persistentDataPath, directoryName);     //return persistent data path + directory path
                else if (saveFolder == SaveFolder.gameFolder)
                    return Path.Combine(Application.dataPath, directoryName);               //return game folder path + directory path
                else
                    return directoryName;                                                   //return only directory path
            }
        }
    }

    public static class SaveLoadJSON
    {
        /// <summary>
        /// Get path to the file (directory path + name of the file (key) + format (.json))
        /// </summary>
        /// <param name="key">Name of the file</param>
        /// <returns></returns>
        public static string GetPathFile(string key)
        {
            //directory path + name of the file (key) + format (.json)
            return Path.Combine(SaveLoadSystem.instance.PathDirectory, key + ".json");
        }

        /// <summary>
        /// Save class in directory/key.json
        /// </summary>
        /// <param name="key">Name of the file</param>
        /// <param name="value">Value to save</param>
        public static void Save(string key, ClassToSave value)
        {
            //if there is no directory, create it
            if (Directory.Exists(SaveLoadSystem.instance.PathDirectory) == false)
            {
                Directory.CreateDirectory(SaveLoadSystem.instance.PathDirectory);
            }

            //value to json, then save file
            string jsonValue = JsonUtility.ToJson(value);
            File.WriteAllText(GetPathFile(key), jsonValue);
        }

        /// <summary>
        /// Load class from directory/key.json
        /// </summary>
        /// <param name="key">name of the file</param>
        public static ClassToSave Load(string key)
        {
            //if there is no file, return null
            if (File.Exists(GetPathFile(key)) == false)
            {
                Debug.Log("Save file not found: " + GetPathFile(key));
                return null;
            }

            //load file, then json to value
            string jsonValue = File.ReadAllText(GetPathFile(key));
            return JsonUtility.FromJson<ClassToSave>(jsonValue);
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="key">Name of the file</param>
        public static void DeleteData(string key)
        {
            //check there is a file
            if (File.Exists(GetPathFile(key)) == false)
            {
                Debug.Log("Save file not found: " + GetPathFile(key));
                return;
            }

            //delete file
            File.Delete(GetPathFile(key));
        }

        /// <summary>
        /// Delete directory with every file
        /// </summary>
        public static void DeleteAll()
        {
            //check there is a directory
            if (Directory.Exists(SaveLoadSystem.instance.PathDirectory) == false)
            {
                Debug.Log("Directory not found: " + SaveLoadSystem.instance.PathDirectory);
                return;
            }

            //delete directory
            Directory.Delete(SaveLoadSystem.instance.PathDirectory, true);
        }
    }

    public static class SaveLoadBinary
    {
        /// <summary>
        /// Get path to the file (directory path + name of the file (key) + format (.bin))
        /// </summary>
        /// <param name="key">Name of the file</param>
        /// <returns></returns>
        public static string GetPathFile(string key)
        {
            //directory path + name of the file (key) + format (.bin)
            return Path.Combine(SaveLoadSystem.instance.PathDirectory, key + ".bin");
        }

        /// <summary>
        /// Save class in directory/key.bin
        /// </summary>
        /// <param name="key">Name of the file</param>
        /// <param name="value">Value to save</param>
        public static void Save(string key, ClassToSave value)
        {
            //if there is no directory, create it
            if (Directory.Exists(SaveLoadSystem.instance.PathDirectory) == false)
            {
                Directory.CreateDirectory(SaveLoadSystem.instance.PathDirectory);
            }

            //create stream at file position
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(GetPathFile(key), FileMode.Create);

            //then save value to file position, and close stream
            formatter.Serialize(stream, value);
            stream.Close();
        }

        /// <summary>
        /// Load class from directory/key.bin
        /// </summary>
        /// <param name="key">name of the file</param>
        public static ClassToSave Load(string key)
        {
            //if there is no file, return null
            if (File.Exists(GetPathFile(key)) == false)
            {
                Debug.Log("Save file not found: " + GetPathFile(key));
                return null;
            }

            //create stream at file position
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(GetPathFile(key), FileMode.Open);

            //then load from file position as value, and close stream
            ClassToSave value = formatter.Deserialize(stream) as ClassToSave;
            stream.Close();

            return value;
        }

        /// <summary>
        /// Delete a file
        /// </summary>
        /// <param name="key">Name of the file</param>
        public static void DeleteData(string key)
        {
            //check there is a file
            if (File.Exists(GetPathFile(key)) == false)
            {
                Debug.Log("Save file not found: " + GetPathFile(key));
                return;
            }

            //delete file
            File.Delete(GetPathFile(key));
        }

        /// <summary>
        /// Delete directory with every file
        /// </summary>
        public static void DeleteAll()
        {
            //check there is a directory
            if (Directory.Exists(SaveLoadSystem.instance.PathDirectory) == false)
            {
                Debug.Log("Directory not found: " + SaveLoadSystem.instance.PathDirectory);
                return;
            }

            //delete directory
            Directory.Delete(SaveLoadSystem.instance.PathDirectory, true);
        }
    }
}
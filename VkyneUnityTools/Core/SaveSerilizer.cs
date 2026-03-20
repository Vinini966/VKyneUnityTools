using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using System;

namespace VkyneTools.Core
{
    public static class SaveSerilizer
    {
        static Dictionary<string, object> saveData = new Dictionary<string, object>();


        public static void addData(string key, object data)
        {
            saveData.Add(key, data);
        }

        public static void updateData(string key, object nData)
        {
            saveData[key] = nData;
        }

        public static bool getBool(string key)
        {
            return (bool)saveData[key];
        }

        public static int getInt(string key)
        {
            return (int)saveData[key];
        }

        public static string getString(string key)
        {
            return (string)saveData[key];
        }

        public static object getObject(string key)
        {
            return saveData[key];
        }

        public static void DeleteAll()
        {
            saveData = new Dictionary<string, object>();
        }

        public static bool KeyExists(string key)
        {
            Debug.Log("Looking for " + key);
            return saveData.ContainsKey(key);
        }

        public static Dictionary<string, object> getALLData()
        {
            return saveData;
        }

        // !----------------WARNING--------------------------------
        /* When Saving and adding variables to the Dictionary make 
           sure they can be serialized.*/

        public static byte[] Save()
        {
            DateTime currentDate = DateTime.Now;
            if (!KeyExists("LastPlayed"))
            {
                addData("LastPlayed", currentDate);
            }
            else
            {
                updateData("LastPlayed", currentDate);
            }
            try
            {

                //Serilizes saveData to file based on username.
                BinaryFormatter bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, saveData);
                    return ms.ToArray(); //Encript data to string
                }

            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            return null;
        }

        public static void Load(byte[] data)
        {
            try
            {
                using (var ms = new MemoryStream())
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    ms.Write(data, 0, data.Length);
                    ms.Seek(0, SeekOrigin.Begin);

                    DateTime currentDate = DateTime.Now;
                    if (!KeyExists("LastPlayed"))
                    {
                        addData("LastPlayed", currentDate);
                    }
                    else
                    {
                        updateData("LastPlayed", currentDate);
                    }

                    //saveData = (Dictionary<string, object>)bf.Deserialize(ms);
                    foreach (KeyValuePair<string, object> key in (Dictionary<string, object>)bf.Deserialize(ms))
                    {
                        if (saveData.ContainsKey(key.Key))
                        {
                            saveData[key.Key] = key.Value;
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Debug.Log(e);
            }



        }

    }
}



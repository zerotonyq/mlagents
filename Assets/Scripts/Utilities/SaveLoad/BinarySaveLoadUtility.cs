using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Utilities.SaveLoad
{
    [Serializable]
    public class Data
    {
        
    }

    public static class BinarySaveLoadUtility
    {
        public static void Save<T>(T data, string savePath) where T : Data
        {
            var formatter = new BinaryFormatter();

            using var fileStream = new FileStream(savePath, FileMode.OpenOrCreate);

            formatter.Serialize(fileStream, data);
        }

        public static T Load<T>(string loadPath) where T : Data
        {
            var formatter = new BinaryFormatter();

            using var fileStream = new FileStream(loadPath, FileMode.OpenOrCreate);

            T data = formatter.Deserialize(fileStream) as T;

            if (data == null)
                throw new NullReferenceException("There is no data at path " + loadPath);

            return data;
        }
    }
}
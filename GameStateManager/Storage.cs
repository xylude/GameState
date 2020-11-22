using System.IO;
using GameState;
using System.Runtime.Serialization.Formatters.Binary;

namespace GameState {
    public class Storage<TStore> {
        public void Save(string path, TStore store) {
            var file = File.Exists(path) ? File.OpenWrite(path) : File.Create(path);
 
            var bf = new BinaryFormatter();
            bf.Serialize(file, store);
            file.Close();
        }

        public TStore Load(string path) {                        
            if(File.Exists(path)) {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(path, FileMode.Open);
                var store = (TStore)bf.Deserialize(file);
                file.Close();

                return store;
            }
            
            throw new FileNotFoundException("Could not load save file.");
        }
    }
}
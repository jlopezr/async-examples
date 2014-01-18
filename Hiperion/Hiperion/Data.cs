using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiperion
{
    [Serializable]
    public class Metadata {
    }

    [Serializable]
    public class SpecificMetadata1 : Metadata
    {
        public int value;

        public SpecificMetadata1(int value)
        {
            this.value = value;
        }
    }

    [Serializable]
    public class SpecificMetadata2 : Metadata
    {
        public int X;
        public int Y;
        public int Z;

        public SpecificMetadata2(int X, int Y, int Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }

    [Serializable]
    class Data
    { 
        protected Dictionary<String, Object> metadata;

        public Data()
        {
            metadata = new Dictionary<string, object>();
        }

        public void Put(String key, Object value)
        {
            if(metadata.ContainsKey(key))
            {
                metadata[key] = value;
            }
            else
            {
                metadata.Add(key, value);
            }
        }

        public object Get(String key) 
        {
            object o;
            if (metadata.TryGetValue(key, out o))
            {
                return o;
            }
            else
            {
                return null;
            }
        }

        public IEnumerable Keys
        {
            get { return metadata.Keys; }
            
        }

        public void Put<T>(T value) where T : Metadata {
            Put(typeof(T).Name, value);
        }

        public T Get<T>() where T : Metadata
        {
            return (T)Get(typeof(T).Name);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiperion
{
    [Serializable]
    public class Metadata
    {
    }

    [Serializable]
    public class SpecificMetadata1 : Metadata
    {
        public int value;

        public SpecificMetadata1(int value)
        {
            this.value = value;
        }

        public override string ToString()
        {
            return value.ToString();
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

        public override string ToString()
        {
            return String.Format("[{0},{1},{2}]", X, Y, Z);
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
            if (metadata.ContainsKey(key))
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

        public void Put<T>(T value) where T : Metadata
        {
            Put(typeof(T).Name, value);
        }

        public T Get<T>() where T : Metadata
        {
            return (T)Get(typeof(T).Name);
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append('{');
            var result = from n in metadata orderby n.Key select new { n.Key, n.Value };
            foreach (var t in result)
            {
                b.Append(t.Key);
                b.Append(':');
                b.Append(t.Value);
                b.Append(' ');
            }
            b.Append('}');
            return b.ToString();
        }
    }
}

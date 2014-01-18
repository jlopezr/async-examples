using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hiperion
{
    public class StorageService : IDisposable
    {
        protected OODB db;

        public StorageService()
        {
            db = new PerstDB();
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public void Put(String id, String service, object data)
        {
            db.Put(id, service, data);
        }

        public IEnumerable<Record> Dates { get { return db.Dates; } }
        public IEnumerable<Record> Ids { get { return db.Ids; } }
        public IEnumerable<Record> Services { get { return db.Services; } }
    }

    public interface OODB : IDisposable
    {
        void Put(String id, String service, object data);

        IEnumerable<Record> Dates { get; }
        IEnumerable<Record> Ids { get; }
        IEnumerable<Record> Services { get; }
    }

    public class Record
    {
        public DateTime date;
        public String service;
        public String id;
        public object data;

        public override String ToString()
        {
            StringBuilder b = new StringBuilder();
            b.Append('[');
            b.Append(date.ToString());
            b.Append("] ");
            b.Append(service);
            b.Append(" (");
            b.Append(id);
            b.Append(") :");
            b.Append(data);
            return b.ToString();
        }
    }

}

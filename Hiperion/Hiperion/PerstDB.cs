using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perst;

namespace Hiperion
{
    public class PerstDB : OODB, IDisposable
    {
        const int PagePoolSize = 32 * 1024 * 1024; // database cache size

        internal Storage db;
        internal Root root;

       

        internal class Root : Persistent
        {
            internal FieldIndex<DateTime, Record> dates;
            internal FieldIndex<String, Record> services;
            internal FieldIndex<String, Record> ids;

            public Root()
            {
                //REQUIRED
            }

            public Root(Storage db)
                : base(db)
            {
                dates = db.CreateFieldIndex<DateTime, Record>("date", false);
                services = db.CreateFieldIndex<String, Record>("service", false);
                ids = db.CreateFieldIndex<String, Record>("id", false);
            }
        }

        public PerstDB()
        {
            db = StorageFactory.Instance.CreateStorage();
            db.Open("perst.dbs", PagePoolSize);

            root = (Root)db.Root; // get storage root
            if (root == null)
            {
                root = new Root(db); // create root object
                db.Root = root; // register root object
            }
        }

        public void Put(String id, String service, object data)
        {
            Record r = new Record { date = DateTime.Now, id = id, service = service, data = data };
            root.dates.Add(r);
            root.ids.Add(r);
            root.services.Add(r);
        }

        public IEnumerable<Record> Dates {
            get { return root.dates; }
        }

        public IEnumerable<Record> Ids
        {
            get { return root.ids; }
        }

        public IEnumerable<Record> Services
        {
            get { return root.services; }
        }

        public void Get()
        {
            foreach (Record r in root.dates)
            {
                Console.WriteLine(r.ToString());
            }
        }

        public void Dispose()
        {
            db.Commit();
            db.Close();
        }
    }
}

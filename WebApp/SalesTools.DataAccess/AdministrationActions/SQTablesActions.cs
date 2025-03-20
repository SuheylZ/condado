using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{    
    public class SQTablesActions
    {
        private DBEngine engine = null;

        internal SQTablesActions(DBEngine reng)
        {
            engine = reng;
        }

        public void Add(Models.SQTables nTable)
        {
            engine.adminEntities.SQTables.AddObject(nTable);
            engine.Save();

        }

        public void Change(Models.SQTables nTable)
        {
            engine.Save();
        }

        public void Delete(int tableKey)
        {
            var U = (from T in engine.adminEntities.SQTables.Where(x => x.Id==tableKey) select T).FirstOrDefault();
            engine.Save();
        }

        public IQueryable<Models.SQTables> GetAll()
        {
            return engine.adminEntities.SQTables;
        }

        public Models.SQTables Get(int tablekey)
        {
            return engine.adminEntities.SQTables.Where(x => x.Id==tablekey).FirstOrDefault();
        }
    }

}

using SalesTool.DataAccess.Models;
using System.Linq;
namespace SalesTool.DataAccess.AdministrationActions
{
    public class InContactDataAction:BaseActions
    {
        internal InContactDataAction(DBEngine engine) : base(engine) { }

        public void Add(inContact_data data)
        {
            E.leadEntities.inContact_data.AddObject(data);
            Engine.Save();
        }
        public void AddOrUpdate(inContact_data data)
        {
            var inContactData = E.leadEntities.inContact_data.FirstOrDefault(p => p.contact_id == data.contact_id);
            if (inContactData != null)
                E.leadEntities.inContact_data.ApplyCurrentValues(data);
            else
            {
                E.leadEntities.inContact_data.AddObject(data);
            }
            Engine.Save();
        }
    }
}
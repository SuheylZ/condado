using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess.AdministrationActions
{
    public class IndividualPrescriptionActions: BaseActions
    {
        internal IndividualPrescriptionActions(DBEngine engine) : base(engine) { }
        
        
        public void Add(Models.IndividualPrescriptions indvPrescription)
        {
            indvPrescription.AddedOn = DateTime.Now;
            E.leadEntities.individual_prescriptions.AddObject(indvPrescription);            
            E.Save();
        }

        public void AddWithIndividual(Models.Account account)
        {
            E.leadEntities.Accounts.AddObject(account);
            E.Save();
        }

        public bool Save(Models.Account account)
        {
            E.Save();
            return true;
        }
    }
}

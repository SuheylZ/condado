using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class BaseQueryDataActions
    {
        DBEngine E=null;

        internal BaseQueryDataActions(DBEngine reng)
        {
            E = reng;
        }
        public Models.BaseQueryData Get(int id)
        {
            return E.adminEntities.BaseQueryDatas.Where(x => x.Id == id).FirstOrDefault();
        }

        public int Add(Models.BaseQueryData nBaseData)
        {
            E.adminEntities.BaseQueryDatas.AddObject(nBaseData);
            E.Save();
            return nBaseData.Id;
        }

        public void Change(Models.BaseQueryData nBaseData)
        {
            E.Save();
        }

        public void Delete(int basedataKey)
        {
            var U = (from T in E.adminEntities.BaseQueryDatas.Where(x => x.Id == basedataKey) select T).FirstOrDefault();            
            E.Save();
        }
        public IQueryable<Models.BaseQueryData> GetAll()
        {
            return E.adminEntities.BaseQueryDatas;
        }
        public IQueryable<Models.BaseQueryData> GetAllByInsuranceType(short nInsuranceType)
        {
            /* YA[June 3, 2013] In web.config Insurance Type is defined as
             * Insurance Type setting 
            For Senior set value =0,
            For Auto and Home set value =1     
            <add key="InsuranceType" value="0"/>
             * Following are the report types
             * //SQAH = Auto Home and SQS = Senior
                //  For both SQAH and SQS
                //Account Dataset - Account, Primary Lead, Primary Ind, Secondary Ind
                //Lead History Dataset - Account, All Leads, Primary Ind, Secondary Ind
                //Account History - Account, Account History, Primary Ind, Secondary Ind
                //Carrier Issues Dataset - Account, Carrier Issues, Carrier Issue CSR, Carrier Issue History
        
                // Only for SQAH
                //Policy Dataset - Account, Primary Lead, Policy, Individual
                //Quote Dataset - Account, Primary Lead, Quote, Individual
        
                // Only for SQS
                //Medicare Supplement Dataset - Account, Primary Lead, Policy, Individual
                //MAPDP Dataset - Account, Primary Lead, Policy, Individual
                //Dental & Vision Dataset - Account, Primary Lead, Policy, Individual
             * In BaseQueryData Entity Types are set as follows
             * For both SQAH and SQS Type = 1
             * Only for SQAH Type = 2
             * Only for SQS Type = 3
             */
            IQueryable<Models.BaseQueryData> U = null;
            if (nInsuranceType == 0) //Senior
                U = E.adminEntities.BaseQueryDatas.Where(x => x.Type == 1 || x.Type == 3);
            if (nInsuranceType == 1) //Auto & Home
                U = E.adminEntities.BaseQueryDatas.Where(x => x.Type == 1 || x.Type == 2);
            if (nInsuranceType == 2) //Life
                U = E.adminEntities.BaseQueryDatas.Where(x => x.Type == 1 || x.Type == 4);
            return U;
        }
    }
}

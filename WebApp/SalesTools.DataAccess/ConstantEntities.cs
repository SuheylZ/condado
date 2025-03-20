// --------------------------------------------------------------------------
// Application:  SQ Sales Tool
// 
// Project:      SalesTool.DataAccess
// 
// Description: This application is created for Condado Group. the application 
//              is accessible from the condado-02 (QA site).
//              This file contains the database entities that are to be initilized when teh database is created.
//              
// Caution:     DO NOT perform Add, Edit or Delete functions on these entities
//              
// 
// Created By:   SZ
// Created On:   12/12/2012
// 
// --------------------------------------------------------------------------
// 

using System;

using System.Linq;


namespace SalesTool.DataAccess
{
    public class ConstantEntities
    {
        private DBEngine engine =null;

        internal ConstantEntities(DBEngine reng)
        {
            engine=reng;
        }

        public IQueryable<Models.State> States
        {
            get
            {
                return engine.adminEntities.States.AsQueryable();
            }
        }

        public IQueryable<Models.TimeZone> TimeZones
        {
            get
            {
                return engine.Admin.TimeZones.AsQueryable();
            }
        }

        public IQueryable<Models.Role> Roles
        {
            get
            {
                return engine.adminEntities.Roles.AsQueryable();
            }
        }

        public string GetStateName(byte id)
        {
                return engine.adminEntities.States.Where(x=>x.Id==id).FirstOrDefault().FullName;
        }
        public string GetStateCode(byte id)
        {
            return (engine.adminEntities.States.Count(x=>x.Id==id)>0)? 
                engine.adminEntities.States.Where(x => x.Id == id).FirstOrDefault().Abbreviation: 
                string.Empty;
        }

        public System.Collections.Generic.List<Models.State> StateList
        {
            get
            {
                return engine.adminEntities.States.AsQueryable().ToList();
            }
        }
        
    }
}

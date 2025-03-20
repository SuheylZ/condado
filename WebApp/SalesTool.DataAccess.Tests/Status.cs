using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SalesTool.DataAccess.Tests
{
    [TestClass]
    public class Status: EngineBase
    {
        [TestMethod]
        public void TestAssignment()
        {
            int id = E.StatusActions.All.FirstOrDefault().Id;

            var actions1 = E.StatusActions.GetActionTemplates(id, true).ToList();
            var actions2 = E.StatusActions.GetActionTemplates(id, false).ToList();



            //E.StatusActions.AssignActions(id, actions1.Select(x=>x.Id).ToArray<int>(), "unknown");

        }

        [TestMethod]
        public void AddSubStatus(){
            Models.Status S= null;

            List<int> childIds = new List<int>();
            int parentid = E.StatusActions.AllSubStatus1.FirstOrDefault().Id;
 
            
            //var x = E.StatusActions.Add("Sub Status 2", true"Test", " 2);
            //childIds.Add(x.Id);

            //x =E.StatusActions.Add("Sub Status 2", "Test", 2);
            //childIds.Add(x.Id);

            E.StatusActions.AssignSubStatuses(parentid, childIds.ToArray(), "Test", "Red");
            

            int iAll = E.StatusActions.AllSubStatus2.Count();
            int iFree = E.StatusActions.GetSubStatuses(parentid, true).Count();
            int iHold = E.StatusActions.GetSubStatuses(parentid, false).Count();

            if (iAll != iFree + iHold)
                throw new InternalTestFailureException();

            E.StatusActions.AssignSubStatuses(parentid, new int[] { }, "Test", string.Empty);
            foreach(int I in childIds)
                E.StatusActions.Delete(I);
        }


        [TestMethod]
        public void TagFields_Basic()
        {
            int statusId = E.StatusActions.AllSubStatus2.First().Id;
            int iAssigned=E.StatusActions.GetTagFields(statusId).Count();
            int iFree = E.StatusActions.GetTagFields(statusId, true).Count();
            int iTotal = E.TagFieldsActions.GetAll().Count();

            if (iAssigned + iFree != iTotal)
                throw new Exception("test failed, tag fields are not balanced");

            // now assign some
            List<int> list = new List<int>();
            list = E.TagFieldsActions.GetAll().Select(x => x.Id).ToList();
            list.RemoveRange(3, 4);
            E.StatusActions.AssignTagFields(statusId, list.ToArray());

            iAssigned = E.StatusActions.GetTagFields(statusId).Count();
            iFree = E.StatusActions.GetTagFields(statusId, true).Count();
            iTotal = E.TagFieldsActions.GetAll().Count();
            if (iAssigned + iFree != iTotal)
                throw new Exception("test failed, tag fields are not balanced");

            list.Clear();
            E.StatusActions.AssignTagFields(statusId, list.ToArray());
        }

        [TestMethod]
        public void TagFields_AssigntoStatus()
        {
            int statusId = E.StatusActions.All.First().Id;

            int iAssigned = E.StatusActions.GetTagFields(statusId).Count();
            int iFree = E.StatusActions.GetTagFields(statusId, true).Count();
            int iTotal = E.TagFieldsActions.GetAll().Count();
            if (iAssigned + iFree != iTotal)
                throw new Exception("test failed, tag fields are not balanced");

            // now assign some
            List<int> list = new List<int>();
            list = E.TagFieldsActions.GetAll().Select(x => x.Id).ToList();
            list.RemoveRange(3, 4);
            E.StatusActions.AssignTagFields(statusId, list.ToArray());

            iAssigned = E.StatusActions.GetTagFields(statusId).Count();
            iFree = E.StatusActions.GetTagFields(statusId, true).Count();
            iTotal = E.TagFieldsActions.GetAll().Count();
            if (iAssigned + iFree != iTotal)
                throw new Exception("test failed, tag fields are not balanced");

            list.Clear();
            E.StatusActions.AssignTagFields(statusId, list.ToArray());
        }

        [TestMethod]
        public void Test_NextSubStatus()
        {
            Models.Status S = null;
            foreach(var item in E.StatusActions.All){
                if(E.StatusActions.GetSubStatuses(item.Id, false).Count() > 1){
                    S= item;
                    break;
                }
            }


            Models.Status sub = E.StatusActions.NextSubStatus(S.Id);
            while (sub != null)
            {
                int? priority = sub.Priority;
                string title = sub.Title;
                sub = E.StatusActions.NextSubStatus(S.Id, sub.Id);
            }
        }

    }
}

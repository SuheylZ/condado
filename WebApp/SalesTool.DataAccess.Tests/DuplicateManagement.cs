using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using System.Linq;

namespace SalesTool.DataAccess.Tests
{
    [TestClass]
    public class DuplicateManagementTests : EngineBase
    {
        [TestMethod]
        public void Add()
        {
            string title = Title;
            int id = E.DuplicateRecordActions.Add(title, 0, 0, true, true, 1, "", 0, "", 0, null, "TEST");


            Assert.IsTrue(id > 0);

            Assert.IsTrue(E.DuplicateRecordActions.Exists(id));
            Assert.IsTrue(E.DuplicateRecordActions.Exists(title));

            E.DuplicateRecordActions.Delete(id);

            Assert.IsFalse(E.DuplicateRecordActions.Exists(id));
            Assert.IsFalse(E.DuplicateRecordActions.Exists(title));
        }


        [TestMethod]
        public void UnSupportedFunctions()
        {
            int[] ids = new int[3];
            ids[0] = AddRecord();
            ids[1] = AddRecord();
            ids[2] = AddRecord();


            int totaltags = E.TagFieldsActions.GetAll().Count();
            //These tag field ids must exist. otherwise change them
            int[] tags = new int[] {60, 61, 62, 63, 64, 65, 66, 67, 68};

            E.DuplicateRecordActions._AddTags(ids[0], tags);

            var x = E.DuplicateRecordActions.Get(ids[0]);
            
            Assert.IsTrue(x.FieldTagsRulesColumns.Count() == tags.Length);
            int unassigned =  E.DuplicateRecordActions._GetFreeTags(ids[0]).Length;

            Assert.IsTrue(totaltags - tags.Length == unassigned);

            E.DuplicateRecordActions._ClearTags(ids[0]);

            var y  = E.DuplicateRecordActions.Get(ids[0]);
            //Assert.IsTrue(y.FieldTags.Count() == 0);

            foreach(int i in ids)
                E.DuplicateRecordActions.Delete(i);
        }

        internal string Title
        {
            get
            {
                return "TEST_" + DateTime.Now.ToString();
            }
        }

        internal int AddRecord()
        {
            return E.DuplicateRecordActions.Add(Title, 0, 0,  true, true, 1, "", 0, "", 0, null, "TEST");
        }
    }
}

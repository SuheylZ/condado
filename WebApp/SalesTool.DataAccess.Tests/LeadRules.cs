using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SalesTool.DataAccess.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class LeadRules: EngineBase
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        private int Count { get { return E.LeadRetentionActions.All.Count(); } }


        [TestMethod]
        public void AddSingle()
        {
            int icount = Count;
            var T = E.LeadRetentionActions.Add("Sample",  "This is a test sample", true, Guid.NewGuid(), "AddSingleTest", 0, "");
            if(T==null)
                throw new InternalTestFailureException();

            if(Count!= (icount+1))
                throw new InternalTestFailureException();

            E.LeadRetentionActions.Delete(T.Id);
        }

        [TestMethod]
        public void AddMultiple(){
            Queue<int> ids = new Queue<int>();
            for(int i = 0; i<25; i++){
            var T  = E.LeadRetentionActions.Add(
                "Sample31241234 23452354 234523452345 234523452345"+i.ToString(), 
                 "This is a test sample This is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sample", 
                 true,                 
                Guid.NewGuid(), 
                "AddMultipleTest", 0, "");
                ids.Enqueue(T.Id);
            }
            while(ids.Count>0)
               E.LeadRetentionActions.Delete(ids.Dequeue());
        }


        [TestMethod]
        public void DeleteSingle()
        {
            var T = E.LeadRetentionActions.Add(
                "Sample31241234 23452354 234523452345 234523452345" ,
                 "This is a test sample This is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sample",
                true, 
                Guid.NewGuid(),
                "DeleteSingle", 
                0, 
                "");
            E.LeadRetentionActions.Delete(T.Id);
        }

        [TestMethod]
        public void DeleteAll()
        {
            Queue<int> ids = new Queue<int>();
            for (int i = 0; i < 40; i++)
            {
                var T = E.LeadRetentionActions.Add(
                    "Sample31241234 23452354 234523452345 234523452345" + i.ToString(),
                     "This is a test sample This is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sampleThis is a test sample",
                    true,
                    Guid.NewGuid(),
                    "AddMultipleTest", 
                    0, "");
                ids.Enqueue(T.Id);
            }
            while (ids.Count > 0)
                E.LeadRetentionActions.Delete(ids.Dequeue());
        }

        [TestMethod]
        public void ChangePriority()
        {
            Queue<int> ids = new Queue<int>();

            //Initialize
            for(int i=0;i<10;i++)
                ids.Enqueue(
                    E.LeadRetentionActions.Add(
                    "untitled" + i.ToString(), "This is a description", true, Guid.Empty, "Test", 0, "").Id
                    );
            

            //Change priority
            E.LeadRetentionActions.Move(ids.ElementAt(4), 57);
            E.LeadRetentionActions.Move(ids.ElementAt(9), -11);
            E.LeadRetentionActions.Move(ids.ElementAt(0), 25);



            //cleanup
            while (ids.Count > 0)
                E.LeadRetentionActions.Delete(ids.Dequeue());
        }

        [TestMethod]
        public void Copy()
        {
            //var source = engine.LeadPrioritizationActions.Add("test1", "this is a description", true,  "Test", 0, "");

            //Clean the copies if already there
            E.LeadPrioritizationActions.Delete(12, 2);
            var target = E.LeadPrioritizationActions.Copy(7, "Test");


            //engine.LeadPrioritizationActions.Delete(source.Id);
            E.LeadPrioritizationActions.Delete(target.Id,2);
        }

        [TestMethod]
        public void TestNextAccount()
        {
            var x = E.AccountActions.GetNextPriorityAccount(32, new Guid("{6d690286-a449-4c09-a694-1940df3b249f}"));
            if(x.Key != 122)
                throw new Exception("Invalid result");
        }

        [TestMethod]
        public void MergeRecord()
        {
            E.LeadsActions.Merge(880, 1000, 21, 21, "Test Case");


        }
    }
}

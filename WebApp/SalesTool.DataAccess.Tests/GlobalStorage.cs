using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalesTool.Common;

namespace SalesTool.DataAccess.Tests
{
    [TestClass]
    public class GlobalStorageTest
    {

        [TestInitialize]
        public void Initialize()
        {
            CGlobalStorage.Init();
        }

        internal CGlobalStorage Instance { get { return CGlobalStorage.Instance; } }
        string TestKey { get { return ("_Unit_Test_Key_" + DateTime.Now.Ticks.ToString()).Substring(0, 20); } }


        [TestMethod]
        public void StoreInteger()
        {
            string key = TestKey;

            Instance.Set<int>(key, 234);
            if (Instance.Get<int>(key) != 234)
                throw new System.InvalidOperationException("integer storage failed");

            Instance.Set<int>(key, 23455);
            if (Instance.Get<int>(key) != 23455)
                throw new System.InvalidOperationException("integer storage failed");

            Instance.Clear(key);
        }

        [TestMethod]
        public void StoreDouble()
        {
            string key = TestKey;

            Instance.Set<double>(key, 234.45);
            if (Instance.Get<double>(key) != 234.45)
                throw new System.InvalidOperationException("double storage failed");

            Instance.Set<double>(key, 2.3455);
            if (Instance.Get<double>(key) != 2.3455)
                throw new System.InvalidOperationException("double storage failed");

            Instance.Clear(key);
        }

        [TestMethod]
        public void StoreString()
        {
            string key = TestKey;

            Instance.Set<string>(key, "Sleepy");
            if (Instance.Get<string>(key) != "Sleepy")
                throw new System.InvalidOperationException("string storage failed");

            Instance.Set<string>(key, "Sleepy Hollow");
            if (Instance.Get<string>(key) != "Sleepy Hollow")
                throw new System.InvalidOperationException("string storage failed");

            Instance.Clear(key);
        }

        [TestMethod]
        public void StoreBoolean()
        {
            string key = TestKey;

            Instance.Set<bool>(key, true);
            if (Instance.Get<bool>(key) != true)
                throw new System.InvalidOperationException("bool storage failed");

            Instance.Set<bool>(key, false);
            if (Instance.Get<bool>(key) != false)
                throw new System.InvalidOperationException("bool storage failed");

            Instance.Clear(key);
        }

        [TestMethod]
        public void StoreDateTime()
        {
            string key = TestKey;

            Instance.Set<DateTime>(key, new DateTime(2012, 1, 1));
            if (Instance.Get<DateTime>(key) != new DateTime(2012, 1, 1))
                throw new System.InvalidOperationException("date storage failed");

            Instance.Set<DateTime>(key, new DateTime(2013, 1, 1));
            if (Instance.Get<DateTime>(key) != new DateTime(2013, 1, 1))
                throw new System.InvalidOperationException("date storage failed");

            Instance.Clear(key);
        }
    }
}

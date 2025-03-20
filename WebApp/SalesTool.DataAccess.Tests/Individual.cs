using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SalesTool.DataAccess.Tests
{
    [TestClass]
    public class Individual: EngineBase
    {
        [TestMethod]
        public void AddAndDeleteIndividual()
        {
            Models.Individual nIndividual = new Models.Individual { Key = 1, FirstName = "Test", LastName = "Test1" };

            Models.Individual recordAdded = E.IndividualsActions.Add(nIndividual);

            if (recordAdded == null)
                throw new InternalTestFailureException();

            E.IndividualsActions.Delete(recordAdded.Key);


        }
        [TestMethod]
        public void SetConsent()
        {
            var I = E.IndividualsActions.GetAll().Select(x=>x.Key).ToList<long>();
            long id = I[1];

            E.IndividualsActions.SetConsent(id, TCPAConsentType.Yes);
            if (E.IndividualsActions.GetConsent(id) != TCPAConsentType.Yes)
                throw new InvalidOperationException("failed to set consent");

            E.IndividualsActions.SetConsent(id, TCPAConsentType.No);
            if (E.IndividualsActions.GetConsent(id) != TCPAConsentType.No)
                throw new InvalidOperationException("failed to set consent");


            E.IndividualsActions.SetConsent(id,TCPAConsentType.Undefined);
            if (E.IndividualsActions.GetConsent(id) != TCPAConsentType.Undefined)
                throw new InvalidOperationException("failed to set consent");

            E.IndividualsActions.SetConsent(id, TCPAConsentType.Blank);
            if (E.IndividualsActions.GetConsent(id) != TCPAConsentType.Blank)
                throw new InvalidOperationException("failed to set consent");
        }
    }
}

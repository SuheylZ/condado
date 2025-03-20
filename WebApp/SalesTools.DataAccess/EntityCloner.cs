using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class ItemEventArgs<T> : EventArgs
    {
        public ItemEventArgs(T item)
        {
            Item = item;
        }

        public T Item { get; protected set; }
    }
    internal class EntityCloner
    {
        DBEngine E = null;
        internal EntityCloner(DBEngine rengine) { E = rengine; }

        internal Models.DentalVision CloneI(Models.DentalVision S, string By)
        {
            Models.DentalVision C = new Models.DentalVision
            {
                AddedBy = By,
                AddedOn = DateTime.Now,
                AnnualPremium = S.AnnualPremium,
                CommissionAmount = S.CommissionAmount,
                CommissionPaidDate = S.CommissionPaidDate,
                CompanyName = S.CompanyName,
                EffectiveDate = S.EffectiveDate,
                Enroller = S.Enroller,
                IsActive = S.IsActive,
                IsDeleted = S.IsDeleted,
                IsPaidFromCarrier = S.IsPaidFromCarrier,
                PolicyNumber = S.PolicyNumber,
                PolicyStatus = S.PolicyStatus,
                SubmissionDate = S.SubmissionDate
            };
            return C;
        }
        internal Models.Account Clone(Models.Account S, string By)
        {
            Models.Account C = new Models.Account
            {
                AddedBy = By,
                AddedOn = DateTime.Now,
                AssignedCsrKey = S.AssignedCsrKey,
                AssignedUserKey = S.AssignedUserKey,
                ChangedBy = null,
                ChangedOn = null,
                ExternalAgent = S.ExternalAgent,
                IsActive = S.IsActive,
                IsDeleted = S.IsDeleted,
                LifeInfo = S.LifeInfo,
                NextEvenDate = S.NextEvenDate,
                Notes = S.Notes,
                PolicyId = S.PolicyId,
                PolicyType = S.PolicyType,
                TransferUserKey = S.TransferUserKey
            };
            E.AccountActions.Add(C);

            for(int i=0;i<S.AccountDetails.Count;i++)
            {
                Models.AccountDetail AD = S.AccountDetails.ToArray()[i];
                Models.AccountDetail X = CloneI(AD, By);
                X.Accountkey = C.Key;
                E.leadEntities.AddToAccountDetails(X);
            }
            E.Save();

            for(int i=0;i<S.Attachments.Count;i++)
            {
                Models.AccountAttachment AA = S.Attachments.ToArray()[i];
                Models.AccountAttachment X = CloneI(AA, By);
                X.AccountId = C.Key;
                E.leadEntities.AddToAccountAttachments1(X);
            }
            E.Save();

            for (int i = 0; i < S.AutoHomeQuotes.Count;i++)
            {
                Models.AutoHomeQuote X = CloneI(S.AutoHomeQuotes.ToArray()[i], By);
                X.AccountKey = C.Key;
                E.leadEntities.AddToAutoHomeQuotes(X);
            }
            E.Save();

            for (int i = 0; i < S.CarrierIssues.Count;i++)
            {
                var X = CloneI(S.CarrierIssues.ToArray()[i], By);
                X.AccountId = C.Key;
                E.CarrierIssueActions.Add(X);
            }
            E.Save();

            for (int i = 0; i < S.MedicalSupplementsApps.Count;i++)
            {
                var X = CloneI(S.MedicalSupplementsApps.ToArray()[i], By);
                X.AccountId = C.Key;
                E.leadEntities.AddTomedsupApplications(X);
            }
            E.Save();

            for (int i = 0; i < S.Individuals.Count;i++)
            {
                var X = Clone(S.Individuals.ToArray()[i], By);
                X.AccountId = C.Key;
            }
            E.Save();

            return C;
        }
        internal Models.Lead Clone(Models.Lead S, string By)
        {
            Models.Lead C = new Models.Lead
            {
                AccountId = S.AccountId,
                ActionId = S.ActionId,
                AddedBy = By,
                AddedOn = DateTime.Now,
                AdVariation = S.AdVariation,
                CampaignId = S.CampaignId,
                Company = S.Company,
                EmailTrackingCode = S.EmailTrackingCode,
                FirstContactAppointment = S.FirstContactAppointment,
                Group = S.Group,
                IPAddress = S.IPAddress,
                IsActive = S.IsActive,
                IsDeleted = S.IsDeleted,
                IsDuplicate = S.IsDuplicate,
                LastActionDate = S.LastActionDate,
                LastCallDate = S.LastCallDate,
                //lea_additional_info
                //lead_notes
                PublisherID = S.PublisherID,
                PubSubId = S.PubSubId,
                SourceCode = S.SourceCode,
                SourceKey = S.SourceKey,
                StatusId = S.StatusId,
                SubStatusId = S.SubStatusId,
                TimeCreated = S.TimeCreated,
                TrackingCode = S.TrackingCode,
                TrackingInformation = S.TrackingInformation
            };
            E.leadEntities.AddToLeads(C);
            E.Save();

            for (int i = 0; i < S.lea_additional_info.Count;i++)
            {
                Models.LeadAdditionalInfo X = CloneI(S.lea_additional_info.ToArray()[i], By);
                X.LeadKey = C.Key;
                E.leadEntities.AddToLeaAdditionalInfos(X);
            }
            E.Save();

            for (int i = 0; i < S.lead_notes.Count;i++ )
            {
                Models.LeadNote X = CloneI(S.lead_notes.ToArray()[i], By);
                X.LeadKey = C.Key;
                E.leadEntities.AddToLeadNotes(X);
            }
            E.Save();

            return C;
        }
        internal Models.LeadNote CloneI(Models.LeadNote S, string By)
        {
            Models.LeadNote C = new Models.LeadNote { AddedBy = By, AddedOn = DateTime.Now, IsActive = S.IsActive, IsDeleted = S.IsDeleted, NoteLeadsKey = S.NoteLeadsKey, Text = S.Text };
            return C;
        }
        internal Models.LeadAdditionalInfo CloneI(SalesTool.DataAccess.Models.LeadAdditionalInfo S, string By)
        {
            Models.LeadAdditionalInfo C = new Models.LeadAdditionalInfo
            {
                AddedBy = By,
                AddedOn = DateTime.Now,
                CreditSelfRating = S.CreditSelfRating,
                IsActive = S.IsActive,
                IsDeleted = S.IsDeleted,
                Reposession = S.Reposession
            };
            return C;
        }
        internal Models.AccountDetail CloneI(Models.AccountDetail S, string By)
        {
            Models.AccountDetail C = new Models.AccountDetail
            {
                Accountkey = S.Accountkey,
                AddedBy = By,
                AddedOn = DateTime.Now,
                ChangedBy = string.Empty,
                ChangedOn = null,
                Id = 0,
                Key = S.Key,
                PolicyId = S.PolicyId,
                PolicyType = S.PolicyType
            };
            return C;
        }
        internal Models.AccountAttachment CloneI(Models.AccountAttachment S, string By)
        {
            Models.AccountAttachment C = new Models.AccountAttachment
            {
                AccountId = S.AccountId,
                AddedBy = By,
                AddedOn = DateTime.Now,
                Attachment = S.Attachment,
                ChangedBy = string.Empty,
                ChangedOn = null,
                FileDescription = S.FileDescription,
                FileName = S.FileName,
                IsDeleted = S.IsDeleted
            };
            return C;
        }
        internal Models.AutoHomeQuote CloneI(Models.AutoHomeQuote S, string by)
        {
            Models.AutoHomeQuote C = new Models.AutoHomeQuote
            {
                AccountKey = S.AccountKey,
                CompanyName = S.CompanyName,
                CurrentCarrierID = S.CurrentCarrierID,
                CurrentCarrierText = S.CurrentCarrierText,
                CurrentPremium = S.CurrentPremium,
                QuotedCarrierID = S.QuotedCarrierID,
                QuotedDate = S.QuotedDate,
                QuotedPremium = S.QuotedPremium,
                Saving = S.Saving,
                Type = S.Type,
                Umbrella = S.Umbrella
            };
            return C;
        }
        internal Models.medsupApplication CloneI(Models.medsupApplication S, string By)
        {
            Models.medsupApplication C = new Models.medsupApplication
            {
                ActualApplicationSentDate = S.ActualApplicationSentDate,
                AddedBy = By,
                AddedOn = DateTime.Now,
                ApplicationSentToCustomerMethod = S.ApplicationSentToCustomerMethod,
                ApplicationSentToSpouse = S.ApplicationSentToSpouse,
                ApplicationSentToSpouseMethod = S.ApplicationSentToSpouseMethod,
                CarrierStatusNote = S.CarrierStatusNote,
                CaseSpecialist = S.CaseSpecialist,
                CaseSpecialistNote = S.CaseSpecialistNote,
                CaseSpecialistNote2 = S.CaseSpecialistNote2,
                CaseSpecialistNote3 = S.CaseSpecialistNote3,
                PolicySubmitToCarrierDate = S.PolicySubmitToCarrierDate,
            };
            return C;
        }
        internal Models.AutoHomePolicy CloneI(Models.AutoHomePolicy S, string By)
        {
            Models.AutoHomePolicy C = new Models.AutoHomePolicy
            {
                AccountId = S.AccountId,
                Added = new Models.History1 { By1 = By, On1 = DateTime.Now },
                BoundOn = S.BoundOn,
                CarrierID = S.CarrierID,
                Changed = null,
                CompanyName = S.CompanyName,
                CurrentCarrierID = S.CurrentCarrierID,
                CurrentCarrierText = S.CurrentCarrierText,
                CurrentMonthlyPremium = S.CurrentMonthlyPremium,
                EffectiveDate = S.EffectiveDate,
                IndividualKey = S.IndividualKey,
                IsActive = S.IsActive,
                IsCoverageIncreased = S.IsCoverageIncreased,
                IsDeleted = S.IsDeleted,
                LapsedOn = S.LapsedOn,
                MonthlyPremium = S.MonthlyPremium,
                PolicyNumber = S.PolicyNumber,
                PolicyStatus = S.PolicyStatus,
                PolicyType = S.PolicyType,
                Term = S.Term,
                UmbrellaPolicy = S.UmbrellaPolicy,
                WritingAgent = S.WritingAgent
            };
            return C;
        }
        internal Models.CarrierIssue CloneI(Models.CarrierIssue S, string By)
        {
            Models.CarrierIssue C = new Models.CarrierIssue
            {
                AccountId = S.AccountId,
                AddedBy = By,
                AddedOn = DateTime.Now,
                CaseSpecialist = S.CaseSpecialist,
                ChangedBy = string.Empty,
                ChangedOn = null,
                ContactFax = S.ContactFax,
                ContactNumber = S.ContactNumber,
                ContactPerson = S.ContactPerson,
                DetailedNote = S.DetailedNote,
                DetailedNote2 = S.DetailedNote2,
                DetailedNote3 = S.DetailedNote3,
                DetailedNote4 = S.DetailedNote4,
                DetectDate = S.DetectDate,
                IsActive = S.IsActive,
                IsDeleted = S.IsDeleted,
                Issues = S.Issues,
                OpenResearchRequest = S.OpenResearchRequest,
                ResearchCaseSpecialist = S.ResearchCaseSpecialist,
                ResearchCloseDate = S.ResearchCloseDate,
                ResearchOpenDate = S.ResearchOpenDate,
                ResearchRequest = S.ResearchRequest,
                ResearchResponse = S.ResearchResponse,
                ResolveDate = S.ResolveDate,
                StatusNote = S.StatusNote
            };
            return C;
        }
        internal Models.Individual Clone(Models.Individual S, string By)
        {
            Models.Individual C = new Models.Individual
            {
                AccountId = S.AccountId,
                AddedBy = By,
                AddedOn = DateTime.Now,
                Address1 = S.Address1,
                Address2 = S.Address2,
                Age = S.Age,
                Birthday = S.Birthday,
                CellPhone = S.CellPhone,
                ChangedBy = string.Empty,
                ChangedOn = null,
                City = S.City,
                DayPhone = S.DayPhone,
                Email = S.Email,
                EveningPhone = S.EveningPhone,
                FaxNmbr = S.FaxNmbr,
                FirstName = S.FirstName,
                Gender = S.Gender,
                HRASubsidyAmount = S.HRASubsidyAmount,
                IndividualPDPStatusID = S.IndividualPDPStatusID,
                IndividualStatusID = S.IndividualStatusID,
                IsActive = S.IsActive,
                IsDeleted = S.IsDeleted,
                LastName = S.LastName,
                Notes = S.Notes,
                Relation = S.Relation,
                Smoking = S.Smoking,
                StateID = S.StateID,
                Zipcode = S.Zipcode
            };

            E.IndividualsActions.Add(C);

            for (int i = 0; i < S.AccountIndividualDetails.Count;i++ )
            {
                Models.AccountIndividualDetail X = CloneI(S.AccountIndividualDetails.ToArray()[i], By);
                X.Individualkey = C.Key;
                E.leadEntities.AddToAccountIndividualDetails(X);
            }
            E.Save();

            for (int i = 0; i<S.best_call.Count;i++ )
            {
                var X = CloneI(S.best_call.ToArray()[i], By);
                X.IndividualId = C.Key;
                E.leadEntities.AddToBestCalls(X);
            }
            E.Save();

            for (int i = 0; i < S.ContactInformations.Count;i++ )
            {
                Models.ContactInformation X = CloneI(S.ContactInformations.ToArray()[i], By);
                X.IndividualId = C.Key;
                X.Individualkey = C.Key;
                E.leadEntities.AddToContactInformations(X);
            }
            E.Save();

            for (int i = 0; i < S.driver_info.Count;i++ )
            {
                var X = Clone(S.driver_info.ToArray()[i], By);
                X.AccountId = C.AccountId;
                X.IndividualId = C.Key;
                E.DriverInfoActions.Update(X);
            }

            for (int i = 0; i < S.Homes.Count;i++)
            {
                Models.Home X = CloneI(S.Homes.ToArray()[i], By);
                X.AccountId = C.AccountId;
                X.IndvId = C.Key;
                X.Individualkey = C.Key;
                E.leadEntities.AddToHomes(X);
            }
            E.Save();

            for (int i = 0; i < S.IndividualDetails.Count;i++)
            {
                var X = CloneI(S.IndividualDetails.ToArray()[i], By);
                X.IndividualId = C.Key;
                E.leadEntities.AddToIndividualDetails(X);
            }
            E.Save();

            for (int i = 0; i < S.Vehicles.Count;i++ )
            {
                var X = CloneI(S.Vehicles.ToArray()[i], By);
                X.IndividualId = C.Key;
                X.Individualkey = C.Key;
                E.leadEntities.AddToVehicles(X);
            }
            E.Save();

            if (S.IndividualPDPStatuses != null)
            {
                //Models.IndividualPDPStatuses pdp = new Models.IndividualPDPStatuses { AddedBy = By, AddedOn = DateTime.Now, ChangedBy = null, ChangedOn = null, Title = S.IndividualPDPStatuses.Title };
                //E.leadEntities.AddToIndividualPDPStatuses(pdp);
                C.IndividualPDPStatusID = S.IndividualPDPStatusID;
                E.Save();
            }

            if (S.IndividualStatus != null)
            {
                //Models.IndividualStatus inds = new Models.IndividualStatus { AddedBy = By, AddedOn = DateTime.Now, ChangedBy = null, ChangedOn = null, Title = S.IndividualStatus.Title };
                //E.leadEntities.AddToIndividualStatus(inds);
                C.IndividualStatusID = S.IndividualStatusID;
                E.Save();
            }
            
            return C;
        }
        internal Models.Home CloneI(Models.Home S, string by)
        {
            Models.Home C = new Models.Home
            {
                AddedBy = S.AddedBy,
                AddedOn = S.AddedOn,
                Address1 = S.Address1,
                Address2 = S.Address2,
                ChangedBy = null,
                ChangedOn = null,
                City = S.City,
                CurrentCarrier = S.CurrentCarrier,
                CurrentXdateLeadInfo = S.CurrentXdateLeadInfo,
                DesignType = S.DesignType,
                DwellingType = S.DwellingType,
                ExteriorWallType = S.ExteriorWallType,
                FoundationType = S.FoundationType,
                HeatingType = S.HeatingType,
                IndvId = null,
                IsActive = S.IsActive,
                IsDeleted = S.IsDeleted,
                NumberOfBathrooms = S.NumberOfBathrooms,
                NumberOfBedrooms = S.NumberOfBedrooms,
                NumberOfClaims = S.NumberOfClaims,
                ReqCoverage = S.ReqCoverage,
                RoofAge = S.RoofAge,
                RoofType = S.RoofType,
                SqFootage = S.SqFootage,
                StateId = S.StateId,
                YearBuilt = S.YearBuilt,
                ZipCode = S.ZipCode
            };
            return C;
        }
        internal Models.ContactInformation CloneI(Models.ContactInformation S, string By)
        {
            Models.ContactInformation C = new Models.ContactInformation
            {
                IndividualId = S.IndividualId,
                Description = S.Description,
                Detail = S.Detail,
                IsActive = S.IsActive,
                IsDeleted = S.IsDeleted
            };
            return C;
        }
        internal Models.AccountIndividualDetail CloneI(Models.AccountIndividualDetail S, string By)
        {
            Models.AccountIndividualDetail C = new Models.AccountIndividualDetail
            {
                AddedBy = By,
                AddedOn = DateTime.Now,
                ChangedBy = string.Empty,
                ChangedOn = null,
                Individualkey = S.Individualkey,
                IsActive = S.IsActive,
                IsDeleted = S.IsDeleted,
                IsPrimary = S.IsPrimary,
            };
            return C;
        }
        internal Models.IndividualDetail CloneI(Models.IndividualDetail S, string By)
        {
            Models.IndividualDetail C = new Models.IndividualDetail
            {
                AccountId = S.AccountId,
                RelationshipDescription = S.RelationshipDescription
            };
            return C;
        }
        internal Models.Vehicle CloneI(Models.Vehicle S, string By)
        {
            Models.Vehicle C = new Models.Vehicle
            {
                AddedBy = By,
                AddedOn = DateTime.Now,
                AnnualMileage = S.AnnualMileage,
                ChangedBy = null,
                ChangedOn = null,
                CollisionDeductable = S.CollisionDeductable,
                ComprehensiveDeductable = S.ComprehensiveDeductable,
                IsActive = S.IsActive,
                IsDeleted = S.IsDeleted,
                Make = S.Make,
                Model = S.Model,
                PolicyId = S.PolicyId,
                PrimaryUse = S.PrimaryUse,
                SecuritySystem = S.SecuritySystem,
                Submodel = S.Submodel,
                WeeklyCommuteDays = S.WeeklyCommuteDays,
                WhereParked = S.WhereParked,
                Year = S.Year
            };

            return C;
        }
        internal Models.BestCall CloneI(Models.BestCall S, string By)
        {
            Models.BestCall C = new Models.BestCall
            {
                AddedBy = By,
                AddedOn = DateTime.Now,
                AppointmentSetDate = S.AppointmentSetDate,
                AppointmentSetHour = S.AppointmentSetHour,
                AppointSetMin = S.AppointSetMin,
                ChangedBy = null,
                ChangedOn = null,
                DateCall = S.DateCall,
                IsActive = S.IsActive,
                IsDeleted = S.IsDeleted,
                NumberCall = S.NumberCall,
                TimeCall = S.TimeCall
            };
            return C;
        }
        internal Models.DriverInfo Clone(Models.DriverInfo S, string By)
        {
            Models.DriverInfo C = new Models.DriverInfo
            {
                AddedBy = By,
                AddedOn = DateTime.Now,
                AgeLicensed = S.AgeLicensed,
                ChangedBy = null,
                ChangedOn = null,
                ClaimPaidAmount = S.ClaimPaidAmount,
                CurrentAutoXDate = S.CurrentAutoXDate,
                CurrentCarrier = S.CurrentCarrier,
                DlState = S.DlState,
                //DriverIncidences
                Education = S.Education,
                IncidentDate = S.IncidentDate,
                IncidentType = S.IncidentType,
                IncidentDescription = S.IncidentDescription,
                IsActive = S.IsActive,
                IsDeleted = S.IsDeleted,
                LiabilityLimit = S.LiabilityLimit,
                LicenseStatus = S.LicenseStatus,
                LisenceNumber = S.LisenceNumber,
                MaritalStatus = S.MaritalStatus,
                MedicalPayment = S.MedicalPayment,
                NmbrIncidents = S.NmbrIncidents,
                Occupation = S.Occupation,
                PolicyYears = S.PolicyYears,
                St22 = S.St22,
                TicketsAccidentsClaims = S.TicketsAccidentsClaims,
                YearsAtResidence = S.YearsAtResidence,
                YearsWithCompany = S.YearsWithCompany,
                YrsInField = S.YrsInField
            };

            E.leadEntities.AddToDriverInfos(C);
            E.Save();

            for (int i = 0; i < S.DriverIncidences.Count;i++)
            {
                Models.DriverIncidence X = CloneI(S.DriverIncidences.ToArray()[i], By);
                X.DriverInformationKey = C.Key;
                E.leadEntities.AddToDriverIncidences(X);
            }
            E.Save();
            return C;
        }
        internal Models.DriverIncidence CloneI(SalesTool.DataAccess.Models.DriverIncidence S, string by)
        {
            Models.DriverIncidence C = new Models.DriverIncidence { AddedBy = by, AddedOn = DateTime.Now, ClaimPaidAmount = S.ClaimPaidAmount, Date1 = S.Date1, IsActive = S.IsActive, IsDeleted = S.IsDeleted, Type = S.Type };
            return C;
        }
        internal Models.Medsup CloneI(Models.Medsup S, string By)
        {
            Models.Medsup C = new Models.Medsup
            {
                AddedBy = By,
                AddedOn = DateTime.Now,
                AnnualPremium = S.AnnualPremium,
                EffectiveDate = S.EffectiveDate,
                ExpirationDate = S.ExpirationDate,
                IssueDate = S.IssueDate,
                CarrierId = S.CarrierId,
                Plan = S.Plan,
                PolicyNumber = S.PolicyNumber
            };
            return C;
        }
        internal Models.Mapdp CloneI(Models.Mapdp S, string By)
        {
            Models.Mapdp C = new Models.Mapdp
            {
                AddedBy = By,
                AddedOn = DateTime.Now,
                Carrier = S.Carrier,
                CommissionAmount = S.CommissionAmount,
                CommissionPaidDate = S.CommissionPaidDate,
                CompanyName = S.CompanyName,
                CoventryPdpReferal = S.CoventryPdpReferal,
                DtePurchasedPdp = S.DtePurchasedPdp,
                EffectiveDate = S.EffectiveDate,
                Enroller = S.Enroller,
                EnrollmentDate = S.EnrollmentDate,
                IsActive = S.IsActive,
                IsDeleted = S.IsDeleted,
                LapseDate = S.LapseDate,
                MaIssueDate = S.MaIssueDate,
                MaritalStatus = S.MaritalStatus,
                PaidFromCarrier = S.PaidFromCarrier,
                PlanName = S.PlanName,
                PlanType = S.PlanType,
                PlanNumber = S.PlanNumber,
                PolicyIdNumber = S.PolicyIdNumber,
                PolicyStatus = S.PolicyStatus,
                SpecialNotes = S.SpecialNotes,
                SpouseCarrier = S.SpouseCarrier,
                SpouseEffectiveDate = S.SpouseEffectiveDate,
                SpouseEnrollmentDate = S.SpouseEnrollmentDate,
                SpousePlanName = S.SpousePlanName,
                SpousePlanNumber = S.SpousePlanNumber,
                SpousePlanType = S.SpousePlanType,
                SpouseType = S.SpouseType,
                SpousePolicyIdNumber = S.SpousePolicyIdNumber,
                Switcher = S.Switcher,
                Type = S.Type,
                VoiceSigSentDate = S.VoiceSigSentDate,
                WritingNumber = S.WritingNumber
            };
            return C;
        }

        internal void Clone(List<Models.Individual> lst, Models.Account target, long primaryId, long secondaryId, string By)
        {
            throw new NotImplementedException();
        }
        
        internal void CloneMedsupApp(Models.Account tAccount, Models.Account sAccount, string By)
        {
            var arrMSA = sAccount.MedicalSupplementsApps.Where(x => !(x.IsDeleted ?? false)).ToArray();
            for (int i = 0; i < arrMSA.Length; i++)
            {
                var X = CloneI(arrMSA[i], By);
                X.AccountId = tAccount.Key;
                E.leadEntities.AddTomedsupApplications(X);
            }
            E.Save();
        }
        internal void CloneMapDP(Models.Account tAccount, Models.Account sAccount, long leadID, string By)
        {
            var arr1 = E.MapdpActions.GetByAccount(sAccount.Key).ToArray();
            for (int i = 0; i < arr1.Length; i++)
            {
                Models.Mapdp x = CloneI(arr1[i], By);
                x.AccountId = tAccount.Key;
                x.LeadId = leadID;
                var Indv = E.IndividualsActions.Get(arr1[i].IndividualId ?? 0);
                if (Indv != null)
                {
                    var y = tAccount.Individuals.Where(z => z.FirstName == Indv.FirstName && z.LastName == Indv.LastName).FirstOrDefault();
                    if (y != null)
                        x.IndividualId = y.Key;
                }
                E.leadEntities.AddToMapdps(x);
            }
            E.Save();
        }
        internal void CloneDentalVision(Models.Account tAccount, Models.Account sAccount, string By)
        {
            var arr2 = E.DentalVisionActions.GetAllByAccountID(sAccount.Key).ToArray();
            for (int i = 0; i < arr2.Length; i++)
            {
                Models.DentalVision x = CloneI(arr2[i], By);
                x.AccountId = tAccount.Key;
                E.leadEntities.AddToDentalVisions(x);
            }
            E.Save();
        }
        internal void CloneMedsUp(Models.Account tAccount, Models.Account sAccount, string By)
        {
            var arr0 = E.MedsupActions.GetAllByAccount(sAccount.Key).ToArray();
            for (int i = 0; i < arr0.Length; i++)
            {
                Models.Medsup X = CloneI(arr0[i], By);
                X.AccountId = tAccount.Key;
                E.leadEntities.AddToMedsups(X);
            }
            E.Save();
        }
        internal void CloneCarrierIssues(Models.Account tAccount, Models.Account sAccount, string By)
        {
            var arrCar = sAccount.CarrierIssues.Where(x => !(x.IsDeleted ?? false)).ToArray();
            for (int i = 0; i < arrCar.Length;i++ )
            {
                var X = CloneI(arrCar[i], By);
                X.AccountId = tAccount.Key;
                E.leadEntities.AddToCarrierIssues(X);
            }
            E.Save();
        }
        internal void CloneIndividuals(Models.Account tAccount, Models.Account sAccount, long primaryId, long secondaryId, string By,List<Int64> lstRemovedIDs = null)
        {
            bool bPrimary = false, bSecondary = false;
            var arrIdnv = sAccount.Individuals.Where(x => !(x.IsDeleted ?? false)).ToArray();
            if (lstRemovedIDs != null && lstRemovedIDs.Count() > 0)
            {
                List<Models.Individual> lstIndv = arrIdnv.ToList();
                lstRemovedIDs.ForEach(delegate(Int64 i) { lstIndv.RemoveAll(delegate(Models.Individual indv) { return indv.Key == i; }); });
                arrIdnv = lstIndv.ToArray();
            }
            for (int i = 0; i < arrIdnv.Length; i++)
            {
                Models.Individual Indv = arrIdnv[i];
                var X = Clone(Indv, By);
                X.AccountId = tAccount.Key;

                if (Indv.Key == primaryId)
                {
                    E.AccountActions.SetIndividual(tAccount.Key, IndividualType.Primary, X.Key);
                    bPrimary = true;
                }
                else if (Indv.Key == secondaryId)
                {
                    E.AccountActions.SetIndividual(tAccount.Key, IndividualType.Secondary, X.Key);
                    bSecondary = true;
                }
            }
            E.Save();

            if (!bPrimary)
                E.AccountActions.SetIndividual(tAccount.Key, IndividualType.Primary, primaryId);
            if (!bSecondary)
                E.AccountActions.SetIndividual(tAccount.Key, IndividualType.Secondary, secondaryId);
        }
    };
}

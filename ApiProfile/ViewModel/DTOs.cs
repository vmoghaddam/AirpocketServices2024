using ApiProfile.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ApiProfile.ViewModels
{
    public class Person
    {
        public int PersonId { get; set; }
        public DateTime? DateCreate { get; set; }
        public int MarriageId { get; set; }
        public string NID { get; set; }
        public int SexId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateBirth { get; set; }
        public string Email { get; set; }
        public string EmailPassword { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Mobile { get; set; }
        public string FaxTelNumber { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? DatePassportIssue { get; set; }
        public DateTime? DatePassportExpire { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; }
        public bool OtherAirline { get; set; }
        public DateTime? DateJoinAvation { get; set; }
        public DateTime? DateLastCheckUP { get; set; }
        public DateTime? DateNextCheckUP { get; set; }
        public DateTime? DateYearOfExperience { get; set; }
        public string CaoCardNumber { get; set; }
        public DateTime? DateCaoCardIssue { get; set; }
        public DateTime? DateCaoCardExpire { get; set; }
        public string CompetencyNo { get; set; }
        public int? CaoInterval { get; set; }
        public int? CaoIntervalCalanderTypeId { get; set; }
        public bool IsDeleted { get; set; }
        public string Remark { get; set; }
        public string StampNumber { get; set; }
        public string StampUrl { get; set; }
        public string TechLogNo { get; set; }
        public DateTime? DateIssueNDT { get; set; }
        public int? IntervalNDT { get; set; }
        public string NDTNumber { get; set; }
        public int? NDTIntervalCalanderTypeId { get; set; }
        public bool? IsAuditor { get; set; }
        public bool? IsAuditee { get; set; }
        public string Nickname { get; set; }
        public int? CityId { get; set; }
        public string FatherName { get; set; }
        public string IDNo { get; set; }
        public Nullable<System.Guid> RowId { get; set; }
        public string UserId { get; set; }
        public string ImageUrl { get; set; }
        public Nullable<int> CustomerCreatorId { get; set; }
        public Nullable<System.DateTime> DateExpireNDT { get; set; }


        public DateTime? ProficiencyExpireDate { get; set; }
        public DateTime? CrewMemberCertificateExpireDate { get; set; }
        public DateTime? LicenceExpireDate { get; set; }
        public DateTime? LicenceIRExpireDate { get; set; }
        public DateTime? SimulatorLastCheck { get; set; }
        public DateTime? SimulatorNextCheck { get; set; }
        public string RampPassNo { get; set; }
        public DateTime? RampPassExpireDate { get; set; }
        public DateTime? LanguageCourseExpireDate { get; set; }
        public string LicenceTitle { get; set; }
        public DateTime? LicenceInitialIssue { get; set; }
        public string RaitingCertificates { get; set; }
        public DateTime? LicenceIssueDate { get; set; }
        public string LicenceDescription { get; set; }
        public int? ProficiencyCheckType { get; set; }
        public DateTime? ProficiencyCheckDate { get; set; }
        public DateTime? ProficiencyValidUntil { get; set; }
        public int? ICAOLPRLevel { get; set; }
        public DateTime? ICAOLPRValidUntil { get; set; }
        public int? MedicalClass { get; set; }
        public string CMCEmployedBy { get; set; }
        public string CMCOccupation { get; set; }
        public string PostalCode { get; set; }
        public bool? ProficiencyIPC { get; set; }
        public bool? ProficiencyOPC { get; set; }

        public string MedicalLimitation { get; set; }
        public string ProficiencyDescription { get; set; }

        public DateTime? VisaExpireDate { get; set; }
        public DateTime? SEPTIssueDate { get; set; }
        public DateTime? SEPTExpireDate { get; set; }
        public DateTime? SEPTPIssueDate { get; set; }
        public DateTime? SEPTPExpireDate { get; set; }
        public DateTime? DangerousGoodsIssueDate { get; set; }
        public DateTime? DangerousGoodsExpireDate { get; set; }
        public DateTime? CCRMIssueDate { get; set; }
        public DateTime? CCRMExpireDate { get; set; }
        public DateTime? CRMIssueDate { get; set; }
        public DateTime? CRMExpireDate { get; set; }
        public DateTime? SMSIssueDate { get; set; }
        public DateTime? SMSExpireDate { get; set; }
        public DateTime? AviationSecurityIssueDate { get; set; }
        public DateTime? AviationSecurityExpireDate { get; set; }
        public DateTime? EGPWSIssueDate { get; set; }
        public DateTime? EGPWSExpireDate { get; set; }
        public DateTime? UpsetRecoveryTrainingIssueDate { get; set; }
        public DateTime? UpsetRecoveryTrainingExpireDate { get; set; }
        public DateTime? ColdWeatherOperationIssueDate { get; set; }
        public DateTime? HotWeatherOperationIssueDate { get; set; }

        public DateTime? ColdWeatherOperationExpireDate { get; set; }
        public DateTime? HotWeatherOperationExpireDate { get; set; }


        public DateTime? PBNRNAVIssueDate { get; set; }
        public DateTime? PBNRNAVExpireDate { get; set; }

        public DateTime? FirstAidIssueDate { get; set; }
        public DateTime? FirstAidExpireDate { get; set; }

        public string ScheduleName { get; set; }
        public string Code { get; set; }

        public DateTime? DateTypeIssue { get; set; }
        public DateTime? DateTypeExpire { get; set; }
        public int? AircraftTypeId { get; set; }

        public string ProficiencyDescriptionOPC { get; set; }
        public DateTime? ProficiencyCheckDateOPC { get; set; }
        public DateTime? ProficiencyValidUntilOPC { get; set; }
        public DateTime? DateTRIExpired { get; set; }
        public DateTime? DateTREExpired { get; set; }

        public DateTime? LineIssueDate { get; set; }
        public DateTime? LineExpireDate { get; set; }

        public DateTime? RecurrentIssueDate { get; set; }
        public DateTime? RecurrentExpireDate { get; set; }

        public DateTime? TypeMDIssueDate { get; set; }
        public DateTime? TypeMDExpireDate { get; set; }
        public DateTime? Type737IssueDate { get; set; }
        public DateTime? Type737ExpireDate { get; set; }
        public DateTime? TypeAirbusIssueDate { get; set; }
        public DateTime? TypeAirbusExpireDate { get; set; }
        public DateTime? TypeMDConversionIssueDate { get; set; }
        public DateTime? Type737ConversionIssueDate { get; set; }
        public DateTime? TypeAirbusConversionIssueDate { get; set; }

        public DateTime? LRCIssueDate { get; set; }
        public DateTime? LRCExpireDate { get; set; }
        public DateTime? RSPIssueDate { get; set; }
        public DateTime? RSPExpireDate { get; set; }
        public DateTime? CTUIssueDate { get; set; }
        public DateTime? CTUExpireDate { get; set; }
        public DateTime? SAIssueDate { get; set; }
        public DateTime? SAExpireDate { get; set; }

        public DateTime? HFIssueDate { get; set; }
        public DateTime? HFExpireDate { get; set; }
        public DateTime? ASDIssueDate { get; set; }
        public DateTime? ASDExpireDate { get; set; }
        public DateTime? GOMIssueDate { get; set; }
        public DateTime? GOMExpireDate { get; set; }
        public DateTime? ASFIssueDate { get; set; }
        public DateTime? ASFExpireDate { get; set; }
        public DateTime? CCIssueDate { get; set; }
        public DateTime? CCExpireDate { get; set; }
        public DateTime? ERPIssueDate { get; set; }
        public DateTime? ERPExpireDate { get; set; }
        public DateTime? MBIssueDate { get; set; }
        public DateTime? MBExpireDate { get; set; }
        public DateTime? PSIssueDate { get; set; }
        public DateTime? PSExpireDate { get; set; }
        public DateTime? ANNEXIssueDate { get; set; }
        public DateTime? ANNEXExpireDate { get; set; }
        public DateTime? DRMIssueDate { get; set; }
        public DateTime? DRMExpireDate { get; set; }
        public DateTime? FMTDIssueDate { get; set; }
        public DateTime? FMTDExpireDate { get; set; }

        public DateTime? FMTIssueDate { get; set; }
        public DateTime? FMTExpireDate { get; set; }

        public DateTime? MELExpireDate { get; set; }
        public DateTime? MELIssueDate { get; set; }
        public DateTime? METIssueDate { get; set; }
        public DateTime? METExpireDate { get; set; }
        public DateTime? PERIssueDate { get; set; }
        public DateTime? PERExpireDate { get; set; }


        public DateTime? LPCC1ExpireDate { get; set; }
        public DateTime? LPCC2ExpireDate { get; set; }
        public DateTime? LPCC3ExpireDate { get; set; }
        public DateTime? LPCC1IssueDate { get; set; }
        public DateTime? LPCC2IssueDate { get; set; }
        public DateTime? LPCC3IssueDate { get; set; }
        public DateTime? OPCC1IssueDate { get; set; }
        public DateTime? OPCC2IssueDate { get; set; }
        public DateTime? OPCC3IssueDate { get; set; }
        public DateTime? LineC1IssueDate { get; set; }
        public DateTime? LineC2IssueDate { get; set; }
        public DateTime? LineC3IssueDate { get; set; }
        public DateTime? LineC1ExpireDate { get; set; }
        public DateTime? LineC2ExpireDate { get; set; }
        public DateTime? LineC3ExpireDate { get; set; }
        public DateTime? RampIssueDate { get; set; }
        public DateTime? RampExpireDate { get; set; }
        public DateTime? ACIssueDate { get; set; }
        public DateTime? ACExpireDate { get; set; }
        public DateTime? UPRTIssueDate { get; set; }
        public DateTime? UPRTExpireDate { get; set; }
        public DateTime? SFIIssueDate { get; set; }
        public DateTime? SFIExpireDate { get; set; }
        public DateTime? SFEIssueDate { get; set; }
        public DateTime? SFEExpireDate { get; set; }
        public DateTime? TRI2IssueDate { get; set; }
        public DateTime? TRI2ExpireDate { get; set; }
        public DateTime? TRE2IssueDate { get; set; }
        public DateTime? TRE2ExpireDate { get; set; }
        public DateTime? IRIIssueDate { get; set; }
        public DateTime? IRIExpireDate { get; set; }
        public DateTime? IREIssueDate { get; set; }
        public DateTime? IREExpireDate { get; set; }
        public DateTime? CRIIssueDate { get; set; }
        public DateTime? CRIExpireDate { get; set; }
        public DateTime? CREIssueDate { get; set; }
        public DateTime? CREExpireDate { get; set; }
        public DateTime? SFI2IssueDate { get; set; }
        public DateTime? SFI2ExpireDate { get; set; }
        public DateTime? SFE2IssueDate { get; set; }
        public DateTime? SFE2ExpireDate { get; set; }
        public DateTime? AirCrewIssueDate { get; set; }
        public DateTime? AirCrewExpireDate { get; set; }
        public DateTime? AirOpsIssueDate { get; set; }
        public DateTime? AirOpsExpireDate { get; set; }
        public DateTime? SOPIssueDate { get; set; }
        public DateTime? SOPExpireDate { get; set; }
        public DateTime? Diff31IssueDate { get; set; }
        public DateTime? Diff31ExpireDate { get; set; }
        public DateTime? Diff34IssueDate { get; set; }
        public DateTime? Diff34ExpireDate { get; set; }
        public DateTime? OMA1IssueDate { get; set; }
        public DateTime? OMA1ExpireDate { get; set; }
        public DateTime? OMB1IssueDate { get; set; }
        public DateTime? OMB1ExpireDate { get; set; }
        public DateTime? OMC1IssueDate { get; set; }
        public DateTime? OMC1ExpireDate { get; set; }
        public DateTime? OMA2IssueDate { get; set; }
        public DateTime? OMA2ExpireDate { get; set; }
        public DateTime? OMB2IssueDate { get; set; }
        public DateTime? OMB2ExpireDate { get; set; }
        public DateTime? OMC2IssueDate { get; set; }
        public DateTime? OMC2ExpireDate { get; set; }
        public DateTime? OMA3IssueDate { get; set; }
        public DateTime? OMA3ExpireDate { get; set; }
        public DateTime? OMB3IssueDate { get; set; }
        public DateTime? OMB3ExpireDate { get; set; }
        public DateTime? OMC3IssueDate { get; set; }
        public DateTime? OMC3ExpireDate { get; set; }
        public DateTime? MapIssueDate { get; set; }
        public DateTime? MapExpireDate { get; set; }
        public DateTime? ComResIssueDate { get; set; }
        public DateTime? ComResExpireDate { get; set; }
        public DateTime? OPCC1ExpireDate { get; set; }
        public DateTime? OPCC2ExpireDate { get; set; }
        public DateTime? OPCC3ExpireDate { get; set; }

        public DateTime? IssueDate1 { get; set; }
        public DateTime? ExpireDate1 { get; set; }
        public DateTime? IssueDate2 { get; set; }
        public DateTime? ExpireDate2 { get; set; }
        public DateTime? IssueDate3 { get; set; }
        public DateTime? ExpireDate3 { get; set; }
        public DateTime? IssueDate4 { get; set; }
        public DateTime? ExpireDate4 { get; set; }
        public DateTime? IssueDate5 { get; set; }
        public DateTime? ExpireDate5 { get; set; }
        public DateTime? IssueDate6 { get; set; }
        public DateTime? ExpireDate6 { get; set; }
        public DateTime? IssueDate7 { get; set; }
        public DateTime? ExpireDate7 { get; set; }
        public DateTime? IssueDate8 { get; set; }
        public DateTime? ExpireDate8 { get; set; }
        public DateTime? IssueDate9 { get; set; }
        public DateTime? ExpireDate9 { get; set; }
        public DateTime? IssueDate10 { get; set; }
        public DateTime? ExpireDate10 { get; set; }

        public Nullable<System.DateTime> IssueDate11 { get; set; }
        public Nullable<System.DateTime> ExpireDate11 { get; set; }
        public Nullable<System.DateTime> IssueDate12 { get; set; }
        public Nullable<System.DateTime> ExpireDate12 { get; set; }
        public Nullable<System.DateTime> IssueDate13 { get; set; }
        public Nullable<System.DateTime> ExpireDate13 { get; set; }
        public Nullable<System.DateTime> IssueDate14 { get; set; }
        public Nullable<System.DateTime> ExpireDate14 { get; set; }
        public Nullable<System.DateTime> IssueDate15 { get; set; }
        public Nullable<System.DateTime> ExpireDate15 { get; set; }
        public Nullable<System.DateTime> IssueDate16 { get; set; }
        public Nullable<System.DateTime> ExpireDate16 { get; set; }
        public Nullable<System.DateTime> IssueDate17 { get; set; }
        public Nullable<System.DateTime> ExpireDate17 { get; set; }
        public Nullable<System.DateTime> IssueDate18 { get; set; }
        public Nullable<System.DateTime> ExpireDate18 { get; set; }
        public Nullable<System.DateTime> IssueDate19 { get; set; }
        public Nullable<System.DateTime> ExpireDate19 { get; set; }
        public Nullable<System.DateTime> IssueDate20 { get; set; }
        public Nullable<System.DateTime> ExpireDate20 { get; set; }
        public Nullable<System.DateTime> IssueDate21 { get; set; }
        public Nullable<System.DateTime> ExpireDate21 { get; set; }
        public Nullable<System.DateTime> IssueDate22 { get; set; }
        public Nullable<System.DateTime> ExpireDate22 { get; set; }
        public Nullable<System.DateTime> IssueDate23 { get; set; }
        public Nullable<System.DateTime> ExpireDate23 { get; set; }
        public Nullable<System.DateTime> IssueDate24 { get; set; }
        public Nullable<System.DateTime> ExpireDate24 { get; set; }
        public Nullable<System.DateTime> IssueDate25 { get; set; }
        public Nullable<System.DateTime> ExpireDate25 { get; set; }
        public Nullable<System.DateTime> IssueDate26 { get; set; }
        public Nullable<System.DateTime> ExpireDate26 { get; set; }
        public Nullable<System.DateTime> IssueDate27 { get; set; }
        public Nullable<System.DateTime> ExpireDate27 { get; set; }
        public Nullable<System.DateTime> IssueDate28 { get; set; }
        public Nullable<System.DateTime> ExpireDate28 { get; set; }
        public Nullable<System.DateTime> IssueDate29 { get; set; }
        public Nullable<System.DateTime> ExpireDate29 { get; set; }
        public Nullable<System.DateTime> IssueDate30 { get; set; }
        public Nullable<System.DateTime> ExpireDate30 { get; set; }
        public Nullable<System.DateTime> IssueDate31 { get; set; }
        public Nullable<System.DateTime> ExpireDate31 { get; set; }
        public Nullable<System.DateTime> IssueDate32 { get; set; }
        public Nullable<System.DateTime> ExpireDate32 { get; set; }
        public Nullable<System.DateTime> IssueDate33 { get; set; }
        public Nullable<System.DateTime> ExpireDate33 { get; set; }
        public Nullable<System.DateTime> IssueDate34 { get; set; }
        public Nullable<System.DateTime> ExpireDate34 { get; set; }
        public Nullable<System.DateTime> IssueDate35 { get; set; }
        public Nullable<System.DateTime> ExpireDate35 { get; set; }


        public Nullable<System.DateTime> IssueDateTRG02 { get; set; }
        public Nullable<System.DateTime> ExpireDateTRG02 { get; set; }
        public Nullable<System.DateTime> IssueDate36 { get; set; }
        public Nullable<System.DateTime> ExpireDate36 { get; set; }
        public Nullable<System.DateTime> IssueDate37 { get; set; }
        public Nullable<System.DateTime> ExpireDate37 { get; set; }
        public Nullable<System.DateTime> IssueDate38 { get; set; }
        public Nullable<System.DateTime> ExpireDate38 { get; set; }
        public Nullable<System.DateTime> IssueDate39 { get; set; }
        public Nullable<System.DateTime> ExpireDate39 { get; set; }
        public Nullable<System.DateTime> IssueDate40 { get; set; }
        public Nullable<System.DateTime> ExpireDate40 { get; set; }
        public Nullable<System.DateTime> IssueDate41 { get; set; }
        public Nullable<System.DateTime> IssueDate42 { get; set; }
        public Nullable<System.DateTime> IssueDate43 { get; set; }
        public Nullable<System.DateTime> IssueDate44 { get; set; }
        public Nullable<System.DateTime> IssueDate45 { get; set; }
        public Nullable<System.DateTime> IssueDate46 { get; set; }
        public Nullable<System.DateTime> IssueDate47 { get; set; }
        public Nullable<System.DateTime> IssueDate48 { get; set; }
        public Nullable<System.DateTime> IssueDate49 { get; set; }
        public Nullable<System.DateTime> IssueDate50 { get; set; }
        public Nullable<System.DateTime> ExpireDate41 { get; set; }
        public Nullable<System.DateTime> ExpireDate42 { get; set; }
        public Nullable<System.DateTime> ExpireDate43 { get; set; }
        public Nullable<System.DateTime> ExpireDate44 { get; set; }
        public Nullable<System.DateTime> ExpireDate45 { get; set; }
        public Nullable<System.DateTime> ExpireDate46 { get; set; }
        public Nullable<System.DateTime> ExpireDate47 { get; set; }
        public Nullable<System.DateTime> ExpireDate48 { get; set; }
        public Nullable<System.DateTime> ExpireDate49 { get; set; }
        public Nullable<System.DateTime> ExpireDate50 { get; set; }
        public Nullable<System.DateTime> ExpireDate51 { get; set; }
        public Nullable<System.DateTime> ExpireDate52 { get; set; }
        public Nullable<System.DateTime> ExpireDate53 { get; set; }
        public Nullable<System.DateTime> ExpireDate54 { get; set; }
        public Nullable<System.DateTime> ExpireDate55 { get; set; }
        public Nullable<System.DateTime> ExpireDate56 { get; set; }
        public Nullable<System.DateTime> ExpireDate57 { get; set; }
        public Nullable<System.DateTime> ExpireDate58 { get; set; }
        public Nullable<System.DateTime> ExpireDate59 { get; set; }
        public Nullable<System.DateTime> ExpireDate60 { get; set; }
        public Nullable<System.DateTime> IssueDate51 { get; set; }
        public Nullable<System.DateTime> IssueDate52 { get; set; }
        public Nullable<System.DateTime> IssueDate53 { get; set; }
        public Nullable<System.DateTime> IssueDate54 { get; set; }
        public Nullable<System.DateTime> IssueDate55 { get; set; }
        public Nullable<System.DateTime> IssueDate56 { get; set; }
        public Nullable<System.DateTime> IssueDate57 { get; set; }
        public Nullable<System.DateTime> IssueDate58 { get; set; }
        public Nullable<System.DateTime> IssueDate59 { get; set; }
        public Nullable<System.DateTime> IssueDate60 { get; set; }
        public Nullable<System.DateTime> IssueDate61 { get; set; }
        public Nullable<System.DateTime> IssueDate62 { get; set; }
        public Nullable<System.DateTime> ExpireDate61 { get; set; }
        public Nullable<System.DateTime> ExpireDate62 { get; set; }
        public Nullable<System.DateTime> IssueDate63 { get; set; }
        public Nullable<System.DateTime> IssueDate64 { get; set; }
        public Nullable<System.DateTime> IssueDate65 { get; set; }
        public Nullable<System.DateTime> ExpireDate63 { get; set; }
        public Nullable<System.DateTime> ExpireDate64 { get; set; }
        public Nullable<System.DateTime> ExpireDate65 { get; set; }
        public Nullable<System.DateTime> IssueDate66 { get; set; }
        public Nullable<System.DateTime> IssueDate67 { get; set; }
        public Nullable<System.DateTime> IssueDate68 { get; set; }
        public Nullable<System.DateTime> IssueDate69 { get; set; }
        public Nullable<System.DateTime> IssueDate70 { get; set; }
        public Nullable<System.DateTime> ExpireDate66 { get; set; }
        public Nullable<System.DateTime> ExpireDate67 { get; set; }
        public Nullable<System.DateTime> ExpireDate68 { get; set; }
        public Nullable<System.DateTime> ExpireDate69 { get; set; }
        public Nullable<System.DateTime> ExpireDate70 { get; set; }
        public Nullable<System.DateTime> IssueDate71 { get; set; }
        public Nullable<System.DateTime> ExpireDate71 { get; set; }
        public Nullable<System.DateTime> IssueDate72 { get; set; }
        public Nullable<System.DateTime> ExpireDate72 { get; set; }
        public Nullable<System.DateTime> IssueDate73 { get; set; }
        public Nullable<System.DateTime> ExpireDate73 { get; set; }
        public Nullable<System.DateTime> IssueDate74 { get; set; }
        public Nullable<System.DateTime> ExpireDate74 { get; set; }
        public Nullable<System.DateTime> IssueDate75 { get; set; }
        public Nullable<System.DateTime> IssueDate76 { get; set; }
        public Nullable<System.DateTime> IssueDate77 { get; set; }
        public Nullable<System.DateTime> ExpireDate75 { get; set; }
        public Nullable<System.DateTime> ExpireDate76 { get; set; }
        public Nullable<System.DateTime> ExpireDate77 { get; set; }


        public Nullable<System.DateTime> EFBIssueDate { get; set; }
        public Nullable<System.DateTime> EFBExpireDate { get; set; }
        public Nullable<System.DateTime> RIGHT_SEAT_QUALIFICATION_IssueDate { get; set; }
        public Nullable<System.DateTime> RIGHT_SEAT_QUALIFICATION_ExpireDate { get; set; }
        public Nullable<System.DateTime> ELTIssueDate { get; set; }
        public Nullable<System.DateTime> ELTExpireDate { get; set; }
        public Nullable<System.DateTime> RVSMIssueDate { get; set; }
        public Nullable<System.DateTime> RVSMExpireDate { get; set; }
        public Nullable<System.DateTime> PILOT_INCAPACITATION_IssueDate { get; set; }
        public Nullable<System.DateTime> PILOT_INCAPACITATION_ExpireDate { get; set; }

        public Nullable<System.DateTime> SafetyPilotIssueDate { get; set; }
        public Nullable<System.DateTime> SafetyPilotExpireDate { get; set; }

        public Nullable<System.DateTime> RouteCheckIssueDate { get; set; }
        public Nullable<System.DateTime> RouteCheckExpireDate { get; set; }

        public string Category { get; set; }
        public Nullable<System.DateTime> PROFICIENCY_ASSESSMENT_IsuueDate { get; set; }
        public Nullable<System.DateTime> PROFICIENCY_ASSESSMENT_ExpireDate { get; set; }
        public Nullable<System.DateTime> MPIssueDate { get; set; }
        public Nullable<System.DateTime> MPExpireDate { get; set; }
        public Nullable<System.DateTime> CALRIssueDate { get; set; }
        public Nullable<System.DateTime> CALRExpireDate { get; set; }
        public Nullable<System.DateTime> SpecialApprovalIssueDate { get; set; }
        public Nullable<System.DateTime> SpecialApprovalExpireDate { get; set; }
        public Nullable<System.DateTime> TRG01IssueDate { get; set; }
        public Nullable<System.DateTime> TRG01ExpireDate { get; set; }
        public Nullable<System.DateTime> TRG16IssueDate { get; set; }
        public Nullable<System.DateTime> TRG16ExpireDate { get; set; }

        public Nullable<System.DateTime> LAR_IssueDate { get; set; }
        public Nullable<System.DateTime> LOAD_CONTROL_IssueDate { get; set; }
        public Nullable<System.DateTime> LAR_ExpireDate { get; set; }
        public Nullable<System.DateTime> LOAD_CONTROL_ExpireDate { get; set; }

        public Nullable<System.DateTime> Phase1IssueDate { get; set; }
        public Nullable<System.DateTime> Phase1ExpireDate { get; set; }
        public Nullable<System.DateTime> Phase2IssueDate { get; set; }
        public Nullable<System.DateTime> Phase2ExpireDate { get; set; }
        public Nullable<System.DateTime> Phase3IssueDate { get; set; }
        public Nullable<System.DateTime> Phase3ExpireDate { get; set; }

        public Nullable<System.DateTime> TRIIssueDate { get; set; }
        public Nullable<System.DateTime> TREIssueDate { get; set; }
        public Nullable<System.DateTime> GroundIssueDate { get; set; }
        public Nullable<System.DateTime> GroundExpireDate { get; set; }
        public Nullable<System.DateTime> FirstAidCockpitIssueDate { get; set; }
        public Nullable<System.DateTime> FirstAidCockpitExpireDate { get; set; }

        public Nullable<System.DateTime> CONVERSION_IssueDate { get; set; }
        public Nullable<System.DateTime> CONVERSION_ExpireDate { get; set; }

        public Nullable<System.DateTime> TypeFoker50IssueDate { get; set; }
        public Nullable<System.DateTime> TypeFoker100IssueDate { get; set; }
        public Nullable<System.DateTime> TypeFoker50ExpireDate { get; set; }
        public Nullable<System.DateTime> TypeFoker100ExpireDate { get; set; }

        public Nullable<System.DateTime> TRG07AIssueDate { get; set; }
        public Nullable<System.DateTime> TRG07AExpireDate { get; set; }

        //DISPATCH_MANUAL_FAM_ExpireDate
        public Nullable<System.DateTime> DISPATCH_MANUAL_FAM_ExpireDate { get; set; }
        public Nullable<System.DateTime> DISPATCH_MANUAL_FAM_IssueDate { get; set; }

        public string BaseAirline { get; set; }

        public Nullable<bool> IsType737 { get; set; }
        public Nullable<bool> IsTypeMD { get; set; }
        public Nullable<bool> IsTypeAirbus { get; set; }
        public Nullable<bool> IsTypeFokker { get; set; }



        List<PersonAircraftType> aircraftTypes = null;
        public List<PersonAircraftType> AircraftTypes
        {
            get
            {
                if (aircraftTypes == null)
                    aircraftTypes = new List<PersonAircraftType>();
                return aircraftTypes;

            }
            set { aircraftTypes = value; }
        }

        List<PersonAircraftType2> aircraftTypes2 = null;
        public List<PersonAircraftType2> AircraftTypes2
        {
            get
            {
                if (aircraftTypes2 == null)
                    aircraftTypes2 = new List<PersonAircraftType2>();
                return aircraftTypes2;

            }
            set { aircraftTypes2 = value; }
        }


        //List<ViewCertificate> certificates = null;
        //public List<ViewCertificate> Certificates
        //{
        //    get
        //    {
        //        if (certificates == null)
        //            certificates = new List<ViewCertificate>();
        //        return certificates;

        //    }
        //    set { certificates = value; }
        //}


        //List<ViewPersonActiveCourse> courses = null;
        //public List<ViewPersonActiveCourse> Courses
        //{
        //    get
        //    {
        //        if (courses == null)
        //            courses = new List<ViewPersonActiveCourse>();
        //        return courses;

        //    }
        //    set { courses = value; }
        //}




        List<PersonDocument> documents = null;
        public List<PersonDocument> Documents
        {
            get
            {
                if (documents == null)
                    documents = new List<PersonDocument>();
                return documents;

            }
            set { documents = value; }
        }

        //List<PersonRating> ratings = null;
        //public List<PersonRating> Ratings
        //{
        //    get
        //    {
        //        if (ratings == null)
        //            ratings = new List<PersonRating>();
        //        return ratings;

        //    }
        //    set { ratings = value; }
        //}

        public static void Fill(Models.Person entity, ViewModels.Person person)
        {
            if (person.DateTypeIssue != null)
            {
                person.DateTypeIssue = ((DateTime)person.DateTypeIssue).ToLocalTime();
            }
            if (person.DateTypeExpire != null)
            {

                person.DateTypeExpire = ((DateTime)person.DateTypeExpire).ToLocalTime();
            }
            entity.Id = person.PersonId;
            entity.AircraftTypeId = person.AircraftTypeId;
            entity.DateTypeIssue = person.DateTypeIssue;
            entity.DateTypeExpire = person.DateTypeExpire;

            //    entity.DateCreate = person.DateCreate;
            entity.MarriageId = person.MarriageId;
            entity.NID = person.NID;
            entity.SexId = person.SexId;
            entity.FirstName = person.FirstName;
            entity.LastName = person.LastName;
            entity.DateBirth = person.DateBirth;
            entity.Email = person.Email;
            entity.EmailPassword = person.EmailPassword;
            entity.Phone1 = person.Phone1;
            entity.Phone2 = person.Phone2;
            entity.Mobile = person.Mobile;
            entity.FaxTelNumber = person.FaxTelNumber;
            entity.PassportNumber = person.PassportNumber;
            entity.DatePassportIssue = person.DatePassportIssue;
            entity.DatePassportExpire = person.DatePassportExpire;
            entity.Address = person.Address;
            entity.IsActive = person.IsActive;
            entity.DateJoinAvation = person.DateJoinAvation;
            entity.DateLastCheckUP = person.DateLastCheckUP == null ? null : (Nullable<DateTime>)((DateTime)person.DateLastCheckUP).Date;
            entity.DateNextCheckUP = person.DateNextCheckUP == null ? null : (Nullable<DateTime>)((DateTime)person.DateNextCheckUP).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
            entity.DateYearOfExperience = person.DateYearOfExperience;
            entity.CaoCardNumber = person.CaoCardNumber;
            entity.DateCaoCardIssue = person.DateCaoCardIssue;
            entity.DateCaoCardExpire = person.DateCaoCardExpire;
            entity.CompetencyNo = person.CompetencyNo;
            entity.CaoInterval = person.CaoInterval;
            entity.CaoIntervalCalanderTypeId = person.CaoIntervalCalanderTypeId;
            entity.IsDeleted = person.IsDeleted;
            entity.Remark = person.Remark;
            entity.StampNumber = person.StampNumber;
            entity.StampUrl = person.StampUrl;
            entity.TechLogNo = person.TechLogNo;
            entity.DateIssueNDT = person.DateIssueNDT;
            entity.IntervalNDT = person.IntervalNDT;
            entity.NDTNumber = person.NDTNumber;
            entity.NDTIntervalCalanderTypeId = person.NDTIntervalCalanderTypeId;
            entity.IsAuditor = person.IsAuditor;
            entity.IsAuditee = person.IsAuditee;
            entity.Nickname = person.Nickname;
            entity.CityId = person.CityId;
            entity.FatherName = person.FatherName;
            entity.IDNo = person.IDNo;
            entity.ImageUrl = person.ImageUrl;

            entity.ProficiencyExpireDate = person.ProficiencyExpireDate;
            entity.CrewMemberCertificateExpireDate = person.CrewMemberCertificateExpireDate;
            entity.LicenceExpireDate = person.LicenceExpireDate;
            entity.LicenceIRExpireDate = person.LicenceIRExpireDate;
            entity.SimulatorLastCheck = person.SimulatorLastCheck;
            entity.SimulatorNextCheck = person.SimulatorNextCheck;
            entity.RampPassNo = person.RampPassNo;
            entity.RampPassExpireDate = person.RampPassExpireDate;
            entity.LanguageCourseExpireDate = person.LanguageCourseExpireDate;
            entity.LicenceTitle = person.LicenceTitle;
            entity.LicenceInitialIssue = person.LicenceInitialIssue;
            entity.RaitingCertificates = person.RaitingCertificates;
            entity.LicenceIssueDate = person.LicenceIssueDate;
            entity.LicenceDescription = person.LicenceDescription;
            entity.ProficiencyCheckType = person.ProficiencyCheckType;
            entity.ProficiencyCheckDate = person.ProficiencyCheckDate;
            entity.ProficiencyValidUntil = person.ProficiencyValidUntil;
            entity.ICAOLPRLevel = person.ICAOLPRLevel;
            entity.ICAOLPRValidUntil = person.ICAOLPRValidUntil;
            entity.MedicalClass = person.MedicalClass;
            entity.CMCEmployedBy = person.CMCEmployedBy;
            entity.CMCOccupation = person.CMCOccupation;
            entity.PostalCode = person.PostalCode;
            entity.ProficiencyIPC = person.ProficiencyIPC;
            entity.ProficiencyOPC = person.ProficiencyOPC;
            entity.VisaExpireDate = person.VisaExpireDate;
            entity.MedicalLimitation = person.MedicalLimitation;
            entity.ProficiencyExpireDate = person.ProficiencyExpireDate;
            entity.SEPTIssueDate = person.SEPTIssueDate;
            entity.SEPTExpireDate = person.SEPTExpireDate;
            entity.SEPTPIssueDate = person.SEPTPIssueDate;
            entity.SEPTPExpireDate = person.SEPTPExpireDate;
            entity.DangerousGoodsIssueDate = person.DangerousGoodsIssueDate;
            entity.DangerousGoodsExpireDate = person.DangerousGoodsExpireDate;
            entity.CCRMIssueDate = person.CCRMIssueDate;
            entity.CCRMExpireDate = person.CCRMExpireDate;
            entity.CRMIssueDate = person.CRMIssueDate;
            entity.CRMExpireDate = person.CRMExpireDate;
            entity.SMSIssueDate = person.SMSIssueDate;
            entity.SMSExpireDate = person.SMSExpireDate;
            entity.AviationSecurityIssueDate = person.AviationSecurityIssueDate;
            entity.AviationSecurityExpireDate = person.AviationSecurityExpireDate;
            entity.EGPWSIssueDate = person.EGPWSIssueDate;
            entity.EGPWSExpireDate = person.EGPWSExpireDate;
            entity.UpsetRecoveryTrainingIssueDate = person.UpsetRecoveryTrainingIssueDate;
            entity.UpsetRecoveryTrainingExpireDate = person.UpsetRecoveryTrainingExpireDate;
            entity.ColdWeatherOperationIssueDate = person.ColdWeatherOperationIssueDate;
            entity.HotWeatherOperationIssueDate = person.HotWeatherOperationIssueDate;
            entity.ColdWeatherOperationExpireDate = person.ColdWeatherOperationExpireDate;
            entity.HotWeatherOperationExpireDate = person.HotWeatherOperationExpireDate;
            entity.PBNRNAVIssueDate = person.PBNRNAVIssueDate;
            entity.PBNRNAVExpireDate = person.PBNRNAVExpireDate;

            entity.FirstAidExpireDate = person.FirstAidExpireDate;
            entity.FirstAidIssueDate = person.FirstAidIssueDate;
            entity.ScheduleName = person.ScheduleName;
           
            if (string.IsNullOrEmpty(entity.ScheduleName))
                entity.ScheduleName = entity.FirstName + entity.LastName;


            entity.Code = person.Code;

            entity.DateTREExpired = person.DateTREExpired;
            entity.DateTRIExpired = person.DateTRIExpired;
            entity.ProficiencyCheckDateOPC = person.ProficiencyCheckDateOPC;
            entity.ProficiencyDescriptionOPC = person.ProficiencyDescriptionOPC;
            entity.ProficiencyValidUntilOPC = person.ProficiencyValidUntilOPC;

            entity.LineIssueDate = person.LineIssueDate;
            entity.LineExpireDate = person.LineExpireDate;

            entity.RecurrentIssueDate = person.RecurrentIssueDate;
            entity.RecurrentExpireDate = person.RecurrentExpireDate;


            entity.TypeMDIssueDate = person.TypeMDIssueDate;
            entity.TypeMDExpireDate = person.TypeMDExpireDate;
            entity.Type737IssueDate = person.Type737IssueDate;
            entity.Type737ExpireDate = person.Type737ExpireDate;
            entity.TypeAirbusIssueDate = person.TypeAirbusIssueDate;
            entity.TypeAirbusExpireDate = person.TypeAirbusExpireDate;
            entity.TypeMDConversionIssueDate = person.TypeMDConversionIssueDate;
            entity.Type737ConversionIssueDate = person.Type737ConversionIssueDate;
            entity.TypeAirbusConversionIssueDate = person.TypeAirbusConversionIssueDate;

            entity.LRCIssueDate = person.LRCIssueDate;
            entity.LRCExpireDate = person.LRCExpireDate;
            entity.RSPIssueDate = person.RSPIssueDate;
            entity.RSPExpireDate = person.RSPExpireDate;
            entity.CTUIssueDate = person.CTUIssueDate;
            entity.CTUExpireDate = person.CTUExpireDate;
            entity.SAIssueDate = person.SAIssueDate;
            entity.SAExpireDate = person.SAExpireDate;

            entity.HFIssueDate = person.HFIssueDate;
            entity.HFExpireDate = person.HFExpireDate;
            entity.ASDIssueDate = person.ASDIssueDate;
            entity.ASDExpireDate = person.ASDExpireDate;
            entity.GOMIssueDate = person.GOMIssueDate;
            entity.GOMExpireDate = person.GOMExpireDate;
            entity.ASFIssueDate = person.ASFIssueDate;
            entity.ASFExpireDate = person.ASFExpireDate;
            entity.CCIssueDate = person.CCIssueDate;
            entity.CCExpireDate = person.CCExpireDate;
            entity.ERPIssueDate = person.ERPIssueDate;
            entity.ERPExpireDate = person.ERPExpireDate;
            entity.MBIssueDate = person.MBIssueDate;
            entity.MBExpireDate = person.MBExpireDate;
            entity.PSIssueDate = person.PSIssueDate;
            entity.PSExpireDate = person.PSExpireDate;
            entity.ANNEXIssueDate = person.ANNEXIssueDate;
            entity.ANNEXExpireDate = person.ANNEXExpireDate;
            entity.DRMIssueDate = person.DRMIssueDate;
            entity.DRMExpireDate = person.DRMExpireDate;
            entity.FMTDIssueDate = person.FMTDIssueDate;
            entity.FMTDExpireDate = person.FMTDExpireDate;
            entity.FMTIssueDate = person.FMTIssueDate;
            entity.FMTExpireDate = person.FMTExpireDate;
            entity.MELExpireDate = person.MELExpireDate;
            entity.MELIssueDate = person.MELIssueDate;
            entity.METIssueDate = person.METIssueDate;
            entity.METExpireDate = person.METExpireDate;
            entity.PERIssueDate = person.PERIssueDate;
            entity.PERExpireDate = person.PERExpireDate;


            entity.LPCC1ExpireDate = person.LPCC1ExpireDate;
            entity.LPCC2ExpireDate = person.LPCC2ExpireDate;
            entity.LPCC3ExpireDate = person.LPCC3ExpireDate;
            entity.LPCC1IssueDate = person.LPCC1IssueDate;
            entity.LPCC2IssueDate = person.LPCC2IssueDate;
            entity.LPCC3IssueDate = person.LPCC3IssueDate;
            entity.OPCC1IssueDate = person.OPCC1IssueDate;
            entity.OPCC2IssueDate = person.OPCC2IssueDate;
            entity.OPCC3IssueDate = person.OPCC3IssueDate;
            entity.LineC1IssueDate = person.LineC1IssueDate;
            entity.LineC2IssueDate = person.LineC2IssueDate;
            entity.LineC3IssueDate = person.LineC3IssueDate;
            entity.LineC1ExpireDate = person.LineC1ExpireDate;
            entity.LineC2ExpireDate = person.LineC2ExpireDate;
            entity.LineC3ExpireDate = person.LineC3ExpireDate;
            entity.RampIssueDate = person.RampIssueDate;
            entity.RampExpireDate = person.RampExpireDate;
            entity.ACIssueDate = person.ACIssueDate;
            entity.ACExpireDate = person.ACExpireDate;
            entity.UPRTIssueDate = person.UPRTIssueDate;
            entity.UPRTExpireDate = person.UPRTExpireDate;
            entity.SFIIssueDate = person.SFIIssueDate;
            entity.SFIExpireDate = person.SFIExpireDate;
            entity.SFEIssueDate = person.SFEIssueDate;
            entity.SFEExpireDate = person.SFEExpireDate;
            entity.TRI2IssueDate = person.TRI2IssueDate;
            entity.TRI2ExpireDate = person.TRI2ExpireDate;
            entity.TRE2IssueDate = person.TRE2IssueDate;
            entity.TRE2ExpireDate = person.TRE2ExpireDate;
            entity.IRIIssueDate = person.IRIIssueDate;
            entity.IRIExpireDate = person.IRIExpireDate;
            entity.IREIssueDate = person.IREIssueDate;
            entity.IREExpireDate = person.IREExpireDate;
            entity.CRIIssueDate = person.CRIIssueDate;
            entity.CRIExpireDate = person.CRIExpireDate;
            entity.CREIssueDate = person.CREIssueDate;
            entity.CREExpireDate = person.CREExpireDate;
            entity.SFI2IssueDate = person.SFI2IssueDate;
            entity.SFI2ExpireDate = person.SFI2ExpireDate;
            entity.SFE2IssueDate = person.SFE2IssueDate;
            entity.SFE2ExpireDate = person.SFE2ExpireDate;
            entity.AirCrewIssueDate = person.AirCrewIssueDate;
            entity.AirCrewExpireDate = person.AirCrewExpireDate;
            entity.AirOpsIssueDate = person.AirOpsIssueDate;
            entity.AirOpsExpireDate = person.AirOpsExpireDate;
            entity.SOPIssueDate = person.SOPIssueDate;
            entity.SOPExpireDate = person.SOPExpireDate;
            entity.Diff31IssueDate = person.Diff31IssueDate;
            entity.Diff31ExpireDate = person.Diff31ExpireDate;
            entity.Diff34IssueDate = person.Diff34IssueDate;
            entity.Diff34ExpireDate = person.Diff34ExpireDate;
            entity.OMA1IssueDate = person.OMA1IssueDate;
            entity.OMA1ExpireDate = person.OMA1ExpireDate;
            entity.OMB1IssueDate = person.OMB1IssueDate;
            entity.OMB1ExpireDate = person.OMB1ExpireDate;
            entity.OMC1IssueDate = person.OMC1IssueDate;
            entity.OMC1ExpireDate = person.OMC1ExpireDate;
            entity.OMA2IssueDate = person.OMA2IssueDate;
            entity.OMA2ExpireDate = person.OMA2ExpireDate;
            entity.OMB2IssueDate = person.OMB2IssueDate;
            entity.OMB2ExpireDate = person.OMB2ExpireDate;
            entity.OMC2IssueDate = person.OMC2IssueDate;
            entity.OMC2ExpireDate = person.OMC2ExpireDate;
            entity.OMA3IssueDate = person.OMA3IssueDate;
            entity.OMA3ExpireDate = person.OMA3ExpireDate;
            entity.OMB3IssueDate = person.OMB3IssueDate;
            entity.OMB3ExpireDate = person.OMB3ExpireDate;
            entity.OMC3IssueDate = person.OMC3IssueDate;
            entity.OMC3ExpireDate = person.OMC3ExpireDate;
            entity.MapIssueDate = person.MapIssueDate;
            entity.MapExpireDate = person.MapExpireDate;
            entity.ComResIssueDate = person.ComResIssueDate;
            entity.ComResExpireDate = person.ComResExpireDate;
            entity.OPCC1ExpireDate = person.OPCC1ExpireDate;
            entity.OPCC2ExpireDate = person.OPCC2ExpireDate;
            entity.OPCC3ExpireDate = person.OPCC3ExpireDate;

            entity.IssueDate1 = person.IssueDate1;
            entity.ExpireDate1 = person.ExpireDate1;
            entity.IssueDate2 = person.IssueDate2;
            entity.ExpireDate2 = person.ExpireDate2;
            entity.IssueDate3 = person.IssueDate3;
            entity.ExpireDate3 = person.ExpireDate3;
            entity.IssueDate4 = person.IssueDate4;
            entity.ExpireDate4 = person.ExpireDate4;
            entity.IssueDate5 = person.IssueDate5;
            entity.ExpireDate5 = person.ExpireDate5;
            entity.IssueDate6 = person.IssueDate6;
            entity.ExpireDate6 = person.ExpireDate6;
            entity.IssueDate7 = person.IssueDate7;
            entity.ExpireDate7 = person.ExpireDate7;
            entity.IssueDate8 = person.IssueDate8;
            entity.ExpireDate8 = person.ExpireDate8;
            entity.IssueDate9 = person.IssueDate9;
            entity.ExpireDate9 = person.ExpireDate9;
            entity.IssueDate10 = person.IssueDate10;
            entity.ExpireDate10 = person.ExpireDate10;

            entity.IssueDate11 = person.IssueDate11;
            entity.ExpireDate11 = person.ExpireDate11;

            entity.IssueDate12 = person.IssueDate12;
            entity.ExpireDate12 = person.ExpireDate12;

            entity.IssueDate13 = person.IssueDate13;
            entity.ExpireDate13 = person.ExpireDate13;

            entity.IssueDate14 = person.IssueDate14;
            entity.ExpireDate14 = person.ExpireDate14;

            entity.IssueDate15 = person.IssueDate15;
            entity.ExpireDate15 = person.ExpireDate15;

            entity.IssueDate16 = person.IssueDate16;
            entity.ExpireDate16 = person.ExpireDate16;

            entity.IssueDate17 = person.IssueDate17;
            entity.ExpireDate17 = person.ExpireDate17;

            entity.IssueDate18 = person.IssueDate18;
            entity.ExpireDate18 = person.ExpireDate18;

            entity.IssueDate19 = person.IssueDate19;
            entity.ExpireDate19 = person.ExpireDate19;

            entity.IssueDate20 = person.IssueDate20;
            entity.ExpireDate20 = person.ExpireDate20;

            entity.IssueDate21 = person.IssueDate21;
            entity.ExpireDate21 = person.ExpireDate21;

            entity.IssueDate22 = person.IssueDate22;
            entity.ExpireDate22 = person.ExpireDate22;

            entity.IssueDate23 = person.IssueDate23;
            entity.ExpireDate23 = person.ExpireDate23;

            entity.IssueDate24 = person.IssueDate24;
            entity.ExpireDate24 = person.ExpireDate24;

            entity.IssueDate25 = person.IssueDate25;
            entity.ExpireDate25 = person.ExpireDate25;

            entity.IssueDate26 = person.IssueDate26;
            entity.ExpireDate26 = person.ExpireDate26;
            entity.IssueDate27 = person.IssueDate27;
            entity.ExpireDate27 = person.ExpireDate27;

            entity.IsType737 = person.IsType737;
            entity.IsTypeAirbus = person.IsTypeAirbus;
            entity.IsTypeFokker = person.IsTypeFokker;
            entity.IsTypeMD = person.IsTypeMD;

            entity.OtherAirline = person.OtherAirline;

            entity.ExpireDate28 = person.ExpireDate28;
            entity.ExpireDate29 = person.ExpireDate29;
            entity.ExpireDate30 = person.ExpireDate30;
            entity.ExpireDate31 = person.ExpireDate31;
            entity.ExpireDate32 = person.ExpireDate32;
            entity.ExpireDate33 = person.ExpireDate33;
            entity.ExpireDate34 = person.ExpireDate34;
            entity.ExpireDate35 = person.ExpireDate35;

            entity.ExpireDate36 = person.ExpireDate36;
            entity.ExpireDate37 = person.ExpireDate37;
            entity.ExpireDate38 = person.ExpireDate38;
            entity.ExpireDate39 = person.ExpireDate39;
            entity.ExpireDate40 = person.ExpireDate40;
            entity.ExpireDate41 = person.ExpireDate41;
            entity.ExpireDate42 = person.ExpireDate42;
            entity.ExpireDate43 = person.ExpireDate43;
            entity.ExpireDate44 = person.ExpireDate44;
            entity.ExpireDate45 = person.ExpireDate45;
            entity.ExpireDate46 = person.ExpireDate46;
            entity.ExpireDate47 = person.ExpireDate47;
            entity.ExpireDate48 = person.ExpireDate48;
            entity.ExpireDate49 = person.ExpireDate49;
            entity.ExpireDate50 = person.ExpireDate50;

            entity.IssueDate28 = person.IssueDate28;
            entity.IssueDate29 = person.IssueDate29;
            entity.IssueDate30 = person.IssueDate30;
            entity.IssueDate31 = person.IssueDate31;
            entity.IssueDate32 = person.IssueDate32;
            entity.IssueDate33 = person.IssueDate33;
            entity.IssueDate34 = person.IssueDate34;
            entity.IssueDate35 = person.IssueDate35;


            entity.IssueDate36 = person.IssueDate36;
            entity.IssueDate37 = person.IssueDate37;
            entity.IssueDate38 = person.IssueDate38;
            entity.IssueDate39 = person.IssueDate39;
            entity.IssueDate40 = person.IssueDate40;
            entity.IssueDate41 = person.IssueDate41;
            entity.IssueDate42 = person.IssueDate42;
            entity.IssueDate43 = person.IssueDate43;
            entity.IssueDate44 = person.IssueDate44;
            entity.IssueDate45 = person.IssueDate45;
            entity.IssueDate46 = person.IssueDate46;
            entity.IssueDate47 = person.IssueDate47;
            entity.IssueDate48 = person.IssueDate48;
            entity.IssueDate49 = person.IssueDate49;
            entity.IssueDate50 = person.IssueDate50;
            entity.ExpireDate51 = person.ExpireDate51;
            entity.ExpireDate52 = person.ExpireDate52;
            entity.ExpireDate53 = person.ExpireDate53;
            entity.ExpireDate54 = person.ExpireDate54;
            entity.ExpireDate55 = person.ExpireDate55;
            entity.ExpireDate56 = person.ExpireDate56;
            entity.ExpireDate57 = person.ExpireDate57;
            entity.ExpireDate58 = person.ExpireDate58;
            entity.ExpireDate59 = person.ExpireDate59;
            entity.ExpireDate60 = person.ExpireDate60;
            entity.IssueDate51 = person.IssueDate51;
            entity.IssueDate52 = person.IssueDate52;
            entity.IssueDate53 = person.IssueDate53;
            entity.IssueDate54 = person.IssueDate54;
            entity.IssueDate55 = person.IssueDate55;
            entity.IssueDate56 = person.IssueDate56;
            entity.IssueDate57 = person.IssueDate57;
            entity.IssueDate58 = person.IssueDate58;
            entity.IssueDate59 = person.IssueDate59;
            entity.IssueDate60 = person.IssueDate60;
            entity.IssueDate61 = person.IssueDate61;
            entity.IssueDate62 = person.IssueDate62;
            entity.ExpireDate61 = person.ExpireDate61;
            entity.ExpireDate62 = person.ExpireDate62;
            entity.IssueDate63 = person.IssueDate63;
            entity.IssueDate64 = person.IssueDate64;
            entity.IssueDate65 = person.IssueDate65;
            entity.ExpireDate63 = person.ExpireDate63;
            entity.ExpireDate64 = person.ExpireDate64;
            entity.ExpireDate65 = person.ExpireDate65;
            entity.IssueDate66 = person.IssueDate66;
            entity.IssueDate67 = person.IssueDate67;
            entity.IssueDate68 = person.IssueDate68;
            entity.IssueDate69 = person.IssueDate69;
            entity.IssueDate70 = person.IssueDate70;
            entity.ExpireDate66 = person.ExpireDate66;
            entity.ExpireDate67 = person.ExpireDate67;
            entity.ExpireDate68 = person.ExpireDate68;
            entity.ExpireDate69 = person.ExpireDate69;
            entity.ExpireDate70 = person.ExpireDate70;
            entity.IssueDate71 = person.IssueDate71;
            entity.ExpireDate71 = person.ExpireDate71;
            entity.IssueDate72 = person.IssueDate72;
            entity.ExpireDate72 = person.ExpireDate72;
            entity.IssueDate73 = person.IssueDate73;
            entity.ExpireDate73 = person.ExpireDate73;
            entity.IssueDate74 = person.IssueDate74;
            entity.ExpireDate74 = person.ExpireDate74;
            entity.IssueDate75 = person.IssueDate75;
            entity.IssueDate76 = person.IssueDate76;
            entity.IssueDate77 = person.IssueDate77;
            entity.ExpireDate75 = person.ExpireDate75;
            entity.ExpireDate76 = person.ExpireDate76;
            entity.ExpireDate77 = person.ExpireDate77;

            entity.ExpireDateTRG02 = person.ExpireDateTRG02;
            entity.IssueDateTRG02 = person.IssueDateTRG02;

            entity.EFBIssueDate = person.EFBIssueDate;
            entity.EFBExpireDate = person.EFBExpireDate;
            entity.RIGHT_SEAT_QUALIFICATION_IssueDate = person.RIGHT_SEAT_QUALIFICATION_IssueDate;
            entity.RIGHT_SEAT_QUALIFICATION_ExpireDate = person.RIGHT_SEAT_QUALIFICATION_ExpireDate;
            entity.ELTIssueDate = person.ELTIssueDate;
            entity.ELTExpireDate = person.ELTExpireDate;
            entity.RVSMIssueDate = person.RVSMIssueDate;
            entity.RVSMExpireDate = person.RVSMExpireDate;
            entity.PILOT_INCAPACITATION_IssueDate = person.PILOT_INCAPACITATION_IssueDate;
            entity.PILOT_INCAPACITATION_ExpireDate = person.PILOT_INCAPACITATION_ExpireDate;

            entity.SafetyPilotIssueDate = person.SafetyPilotIssueDate;
            entity.SafetyPilotExpireDate = person.SafetyPilotExpireDate;

            entity.RouteCheckIssueDate = person.RouteCheckIssueDate;
            entity.RouteCheckExpireDate = person.RouteCheckExpireDate;

            entity.LAR_IssueDate = person.LAR_IssueDate;
            entity.LAR_ExpireDate = person.LAR_ExpireDate;
            entity.LOAD_CONTROL_ExpireDate = person.LOAD_CONTROL_ExpireDate;
            entity.LOAD_CONTROL_IssueDate = person.LOAD_CONTROL_IssueDate;


            entity.Category = person.Category;
            entity.PROFICIENCY_ASSESSMENT_IsuueDate = person.PROFICIENCY_ASSESSMENT_IsuueDate;
            entity.PROFICIENCY_ASSESSMENT_ExpireDate = person.PROFICIENCY_ASSESSMENT_ExpireDate;
            entity.MPIssueDate = person.MPIssueDate;
            entity.MPExpireDate = person.MPExpireDate;
            entity.CALRIssueDate = person.CALRIssueDate;
            entity.CALRExpireDate = person.CALRExpireDate;
            entity.SpecialApprovalIssueDate = person.SpecialApprovalIssueDate;
            entity.SpecialApprovalExpireDate = person.SpecialApprovalExpireDate;
            entity.TRG01IssueDate = person.TRG01IssueDate;
            entity.TRG01ExpireDate = person.TRG01ExpireDate;
            entity.TRG16IssueDate = person.TRG16IssueDate;
            entity.TRG16ExpireDate = person.TRG16ExpireDate;

            entity.TRG07AExpireDate = person.TRG07AExpireDate;
            entity.TRG07AIssueDate = person.TRG07AIssueDate;

            entity.DISPATCH_MANUAL_FAM_ExpireDate = person.DISPATCH_MANUAL_FAM_ExpireDate;
            entity.DISPATCH_MANUAL_FAM_IssueDate = person.DISPATCH_MANUAL_FAM_IssueDate;


            entity.Phase1IssueDate = person.Phase1IssueDate;
            entity.Phase1ExpireDate = person.Phase1ExpireDate;

            entity.Phase2IssueDate = person.Phase2IssueDate;
            entity.Phase2ExpireDate = person.Phase2ExpireDate;

            entity.Phase3IssueDate = person.Phase3IssueDate;
            entity.Phase3ExpireDate = person.Phase3ExpireDate;


            entity.TRIIssueDate = person.TRIIssueDate;
            entity.TREIssueDate = person.TREIssueDate;
            entity.GroundIssueDate = person.GroundIssueDate;
            entity.GroundExpireDate = person.GroundExpireDate;
            entity.FirstAidCockpitIssueDate = person.FirstAidCockpitIssueDate;
            entity.FirstAidCockpitExpireDate = person.FirstAidCockpitExpireDate;
            entity.CONVERSION_IssueDate = person.CONVERSION_IssueDate;
            entity.CONVERSION_ExpireDate = person.CONVERSION_ExpireDate;

            entity.TypeFoker100ExpireDate = person.TypeFoker100ExpireDate;
            entity.TypeFoker100IssueDate = person.TypeFoker100IssueDate;
            entity.TypeFoker50ExpireDate = person.TypeFoker50ExpireDate;
            entity.TypeFoker50IssueDate = person.TypeFoker50IssueDate;




            entity.BaseAirline = person.BaseAirline;


        }
        public static void FillDto(Models.Person entity, ViewModels.Person person)
        {
            person.PersonId = entity.Id;
            person.DateCreate = entity.DateCreate;
            person.MarriageId = entity.MarriageId;
            person.NID = entity.NID;
            person.SexId = entity.SexId;
            person.FirstName = entity.FirstName;
            person.LastName = entity.LastName;
            person.DateBirth = entity.DateBirth;
            person.Email = entity.Email;
            person.EmailPassword = entity.EmailPassword;
            person.Phone1 = entity.Phone1;
            person.Phone2 = entity.Phone2;
            person.Mobile = entity.Mobile;
            person.FaxTelNumber = entity.FaxTelNumber;
            person.PassportNumber = entity.PassportNumber;
            person.DatePassportIssue = entity.DatePassportIssue;
            person.DatePassportExpire = entity.DatePassportExpire;
            person.Address = entity.Address;
            person.IsActive = entity.IsActive;
            person.DateJoinAvation = entity.DateJoinAvation;
            person.DateLastCheckUP = entity.DateLastCheckUP;
            person.DateNextCheckUP = entity.DateNextCheckUP;
            person.DateYearOfExperience = entity.DateYearOfExperience;
            person.CaoCardNumber = entity.CaoCardNumber;
            person.DateCaoCardIssue = entity.DateCaoCardIssue;
            person.DateCaoCardExpire = entity.DateCaoCardExpire;
            person.CompetencyNo = entity.CompetencyNo;
            person.CaoInterval = entity.CaoInterval;
            person.CaoIntervalCalanderTypeId = entity.CaoIntervalCalanderTypeId;
            person.IsDeleted = entity.IsDeleted;
            person.Remark = entity.Remark;
            person.StampNumber = entity.StampNumber;
            person.StampUrl = entity.StampUrl;
            person.TechLogNo = entity.TechLogNo;
            person.DateIssueNDT = entity.DateIssueNDT;
            person.IntervalNDT = entity.IntervalNDT;
            person.NDTNumber = entity.NDTNumber;
            person.NDTIntervalCalanderTypeId = entity.NDTIntervalCalanderTypeId;
            person.IsAuditor = entity.IsAuditor;
            person.IsAuditee = entity.IsAuditee;
            person.Nickname = entity.Nickname;
            person.CityId = entity.CityId;
            person.FatherName = entity.FatherName;
            person.IDNo = entity.IDNo;
            person.RowId = entity.RowId;
            person.UserId = entity.UserId;
            person.ImageUrl = entity.ImageUrl;
            person.CustomerCreatorId = entity.CustomerCreatorId;
            person.DateExpireNDT = entity.DateExpireNDT;

            person.ProficiencyExpireDate = entity.ProficiencyExpireDate;
            person.CrewMemberCertificateExpireDate = entity.CrewMemberCertificateExpireDate;
            person.LicenceExpireDate = entity.LicenceExpireDate;
            person.LicenceIRExpireDate = entity.LicenceIRExpireDate;
            person.SimulatorLastCheck = entity.SimulatorLastCheck;
            person.SimulatorNextCheck = entity.SimulatorNextCheck;
            person.RampPassNo = entity.RampPassNo;
            person.RampPassExpireDate = entity.RampPassExpireDate;
            person.LanguageCourseExpireDate = entity.LanguageCourseExpireDate;
            person.LicenceTitle = entity.LicenceTitle;
            person.LicenceInitialIssue = entity.LicenceInitialIssue;
            person.RaitingCertificates = entity.RaitingCertificates;
            person.LicenceIssueDate = entity.LicenceIssueDate;
            person.LicenceDescription = entity.LicenceDescription;
            person.ProficiencyCheckType = entity.ProficiencyCheckType;
            person.ProficiencyCheckDate = entity.ProficiencyCheckDate;
            person.ProficiencyValidUntil = entity.ProficiencyValidUntil;
            person.ICAOLPRLevel = entity.ICAOLPRLevel;
            person.ICAOLPRValidUntil = entity.ICAOLPRValidUntil;
            person.MedicalClass = entity.MedicalClass;
            person.CMCEmployedBy = entity.CMCEmployedBy;
            person.CMCOccupation = entity.CMCOccupation;
            person.PostalCode = entity.PostalCode;
            person.ProficiencyIPC = entity.ProficiencyIPC;
            person.ProficiencyOPC = entity.ProficiencyOPC;

            person.MedicalLimitation = entity.MedicalLimitation;
            person.ProficiencyDescription = entity.ProficiencyDescription;
            person.VisaExpireDate = entity.VisaExpireDate;

            person.SEPTIssueDate = entity.SEPTIssueDate;
            person.SEPTExpireDate = entity.SEPTExpireDate;
            person.SEPTPIssueDate = entity.SEPTPIssueDate;
            person.SEPTPExpireDate = entity.SEPTPExpireDate;
            person.DangerousGoodsIssueDate = entity.DangerousGoodsIssueDate;
            person.DangerousGoodsExpireDate = entity.DangerousGoodsExpireDate;
            person.CCRMIssueDate = entity.CCRMIssueDate;
            person.CCRMExpireDate = entity.CCRMExpireDate;
            person.CRMIssueDate = entity.CRMIssueDate;
            person.CRMExpireDate = entity.CRMExpireDate;
            person.SMSIssueDate = entity.SMSIssueDate;
            person.SMSExpireDate = entity.SMSExpireDate;
            person.AviationSecurityIssueDate = entity.AviationSecurityIssueDate;
            person.AviationSecurityExpireDate = entity.AviationSecurityExpireDate;
            person.EGPWSIssueDate = entity.EGPWSIssueDate;
            person.EGPWSExpireDate = entity.EGPWSExpireDate;
            person.UpsetRecoveryTrainingIssueDate = entity.UpsetRecoveryTrainingIssueDate;
            person.UpsetRecoveryTrainingExpireDate = entity.UpsetRecoveryTrainingExpireDate;
            person.ColdWeatherOperationIssueDate = entity.ColdWeatherOperationIssueDate;
            person.HotWeatherOperationIssueDate = entity.HotWeatherOperationIssueDate;
            person.ColdWeatherOperationExpireDate = entity.ColdWeatherOperationExpireDate;
            person.HotWeatherOperationExpireDate = entity.HotWeatherOperationExpireDate;
            person.PBNRNAVIssueDate = entity.PBNRNAVIssueDate;
            person.PBNRNAVExpireDate = entity.PBNRNAVExpireDate;
            person.FirstAidExpireDate = entity.FirstAidExpireDate;
            person.FirstAidIssueDate = entity.FirstAidIssueDate;
            person.ScheduleName = entity.ScheduleName;
            person.Code = entity.Code;
            person.AircraftTypeId = entity.AircraftTypeId;
            person.DateTypeExpire = entity.DateTypeExpire;
            person.DateTypeIssue = entity.DateTypeIssue;

            person.ProficiencyCheckDateOPC = entity.ProficiencyCheckDateOPC;
            person.ProficiencyValidUntilOPC = entity.ProficiencyValidUntilOPC;
            person.ProficiencyDescriptionOPC = entity.ProficiencyDescriptionOPC;

            person.DateTRIExpired = entity.DateTRIExpired;
            person.DateTREExpired = entity.DateTREExpired;

            person.LineExpireDate = entity.LineExpireDate;
            person.LineIssueDate = entity.LineIssueDate;
            person.RecurrentExpireDate = entity.RecurrentExpireDate;
            person.RecurrentIssueDate = entity.RecurrentIssueDate;



            person.TypeMDIssueDate = entity.TypeMDIssueDate;
            person.TypeMDExpireDate = entity.TypeMDExpireDate;
            person.Type737IssueDate = entity.Type737IssueDate;
            person.Type737ExpireDate = entity.Type737ExpireDate;
            person.TypeAirbusIssueDate = entity.TypeAirbusIssueDate;
            person.TypeAirbusExpireDate = entity.TypeAirbusExpireDate;
            person.TypeMDConversionIssueDate = entity.TypeMDConversionIssueDate;
            person.Type737ConversionIssueDate = entity.Type737ConversionIssueDate;
            person.TypeAirbusConversionIssueDate = entity.TypeAirbusConversionIssueDate;

            person.LRCIssueDate = entity.LRCIssueDate;
            person.LRCExpireDate = entity.LRCExpireDate;
            person.RSPIssueDate = entity.RSPIssueDate;
            person.RSPExpireDate = entity.RSPExpireDate;
            person.CTUIssueDate = entity.CTUIssueDate;
            person.CTUExpireDate = entity.CTUExpireDate;
            person.SAIssueDate = entity.SAIssueDate;
            person.SAExpireDate = entity.SAExpireDate;

            person.HFIssueDate = entity.HFIssueDate;
            person.HFExpireDate = entity.HFExpireDate;
            person.ASDIssueDate = entity.ASDIssueDate;
            person.ASDExpireDate = entity.ASDExpireDate;
            person.GOMIssueDate = entity.GOMIssueDate;
            person.GOMExpireDate = entity.GOMExpireDate;
            person.ASFIssueDate = entity.ASFIssueDate;
            person.ASFExpireDate = entity.ASFExpireDate;
            person.CCIssueDate = entity.CCIssueDate;
            person.CCExpireDate = entity.CCExpireDate;
            person.ERPIssueDate = entity.ERPIssueDate;
            person.ERPExpireDate = entity.ERPExpireDate;
            person.MBIssueDate = entity.MBIssueDate;
            person.MBExpireDate = entity.MBExpireDate;
            person.PSIssueDate = entity.PSIssueDate;
            person.PSExpireDate = entity.PSExpireDate;
            person.ANNEXIssueDate = entity.ANNEXIssueDate;
            person.ANNEXExpireDate = entity.ANNEXExpireDate;
            person.DRMIssueDate = entity.DRMIssueDate;
            person.DRMExpireDate = entity.DRMExpireDate;
            person.FMTDIssueDate = entity.FMTDIssueDate;
            person.FMTDExpireDate = entity.FMTDExpireDate;
            person.FMTIssueDate = entity.FMTIssueDate;
            person.FMTExpireDate = entity.FMTExpireDate;
            person.MELExpireDate = entity.MELExpireDate;
            person.MELIssueDate = entity.MELIssueDate;
            person.METIssueDate = entity.METIssueDate;
            person.METExpireDate = entity.METExpireDate;
            person.PERIssueDate = entity.PERIssueDate;
            person.PERExpireDate = entity.PERExpireDate;



            person.LPCC1ExpireDate = entity.LPCC1ExpireDate;
            person.LPCC2ExpireDate = entity.LPCC2ExpireDate;
            person.LPCC3ExpireDate = entity.LPCC3ExpireDate;
            person.LPCC1IssueDate = entity.LPCC1IssueDate;
            person.LPCC2IssueDate = entity.LPCC2IssueDate;
            person.LPCC3IssueDate = entity.LPCC3IssueDate;
            person.OPCC1IssueDate = entity.OPCC1IssueDate;
            person.OPCC2IssueDate = entity.OPCC2IssueDate;
            person.OPCC3IssueDate = entity.OPCC3IssueDate;
            person.LineC1IssueDate = entity.LineC1IssueDate;
            person.LineC2IssueDate = entity.LineC2IssueDate;
            person.LineC3IssueDate = entity.LineC3IssueDate;
            person.LineC1ExpireDate = entity.LineC1ExpireDate;
            person.LineC2ExpireDate = entity.LineC2ExpireDate;
            person.LineC3ExpireDate = entity.LineC3ExpireDate;
            person.RampIssueDate = entity.RampIssueDate;
            person.RampExpireDate = entity.RampExpireDate;
            person.ACIssueDate = entity.ACIssueDate;
            person.ACExpireDate = entity.ACExpireDate;
            person.UPRTIssueDate = entity.UPRTIssueDate;
            person.UPRTExpireDate = entity.UPRTExpireDate;
            person.SFIIssueDate = entity.SFIIssueDate;
            person.SFIExpireDate = entity.SFIExpireDate;
            person.SFEIssueDate = entity.SFEIssueDate;
            person.SFEExpireDate = entity.SFEExpireDate;
            person.TRI2IssueDate = entity.TRI2IssueDate;
            person.TRI2ExpireDate = entity.TRI2ExpireDate;
            person.TRE2IssueDate = entity.TRE2IssueDate;
            person.TRE2ExpireDate = entity.TRE2ExpireDate;
            person.IRIIssueDate = entity.IRIIssueDate;
            person.IRIExpireDate = entity.IRIExpireDate;
            person.IREIssueDate = entity.IREIssueDate;
            person.IREExpireDate = entity.IREExpireDate;
            person.CRIIssueDate = entity.CRIIssueDate;
            person.CRIExpireDate = entity.CRIExpireDate;
            person.CREIssueDate = entity.CREIssueDate;
            person.CREExpireDate = entity.CREExpireDate;
            person.SFI2IssueDate = entity.SFI2IssueDate;
            person.SFI2ExpireDate = entity.SFI2ExpireDate;
            person.SFE2IssueDate = entity.SFE2IssueDate;
            person.SFE2ExpireDate = entity.SFE2ExpireDate;
            person.AirCrewIssueDate = entity.AirCrewIssueDate;
            person.AirCrewExpireDate = entity.AirCrewExpireDate;
            person.AirOpsIssueDate = entity.AirOpsIssueDate;
            person.AirOpsExpireDate = entity.AirOpsExpireDate;
            person.SOPIssueDate = entity.SOPIssueDate;
            person.SOPExpireDate = entity.SOPExpireDate;
            person.Diff31IssueDate = entity.Diff31IssueDate;
            person.Diff31ExpireDate = entity.Diff31ExpireDate;
            person.Diff34IssueDate = entity.Diff34IssueDate;
            person.Diff34ExpireDate = entity.Diff34ExpireDate;
            person.OMA1IssueDate = entity.OMA1IssueDate;
            person.OMA1ExpireDate = entity.OMA1ExpireDate;
            person.OMB1IssueDate = entity.OMB1IssueDate;
            person.OMB1ExpireDate = entity.OMB1ExpireDate;
            person.OMC1IssueDate = entity.OMC1IssueDate;
            person.OMC1ExpireDate = entity.OMC1ExpireDate;
            person.OMA2IssueDate = entity.OMA2IssueDate;
            person.OMA2ExpireDate = entity.OMA2ExpireDate;
            person.OMB2IssueDate = entity.OMB2IssueDate;
            person.OMB2ExpireDate = entity.OMB2ExpireDate;
            person.OMC2IssueDate = entity.OMC2IssueDate;
            person.OMC2ExpireDate = entity.OMC2ExpireDate;
            person.OMA3IssueDate = entity.OMA3IssueDate;
            person.OMA3ExpireDate = entity.OMA3ExpireDate;
            person.OMB3IssueDate = entity.OMB3IssueDate;
            person.OMB3ExpireDate = entity.OMB3ExpireDate;
            person.OMC3IssueDate = entity.OMC3IssueDate;
            person.OMC3ExpireDate = entity.OMC3ExpireDate;
            person.MapIssueDate = entity.MapIssueDate;
            person.MapExpireDate = entity.MapExpireDate;
            person.ComResIssueDate = entity.ComResIssueDate;
            person.ComResExpireDate = entity.ComResExpireDate;
            person.OPCC1ExpireDate = entity.OPCC1ExpireDate;
            person.OPCC2ExpireDate = entity.OPCC2ExpireDate;
            person.OPCC3ExpireDate = entity.OPCC3ExpireDate;


            person.IssueDate1 = entity.IssueDate1;
            person.ExpireDate1 = entity.ExpireDate1;
            person.IssueDate2 = entity.IssueDate2;
            person.ExpireDate2 = entity.ExpireDate2;
            person.IssueDate3 = entity.IssueDate3;
            person.ExpireDate3 = entity.ExpireDate3;
            person.IssueDate4 = entity.IssueDate4;
            person.ExpireDate4 = entity.ExpireDate4;
            person.IssueDate5 = entity.IssueDate5;
            person.ExpireDate5 = entity.ExpireDate5;
            person.IssueDate6 = entity.IssueDate6;
            person.ExpireDate6 = entity.ExpireDate6;
            person.IssueDate7 = entity.IssueDate7;
            person.ExpireDate7 = entity.ExpireDate7;
            person.IssueDate8 = entity.IssueDate8;
            person.ExpireDate8 = entity.ExpireDate8;
            person.IssueDate9 = entity.IssueDate9;
            person.ExpireDate9 = entity.ExpireDate9;
            person.IssueDate10 = entity.IssueDate10;
            person.ExpireDate10 = entity.ExpireDate10;

            person.IssueDate11 = entity.IssueDate11;
            person.ExpireDate11 = entity.ExpireDate11;

            person.IssueDate12 = entity.IssueDate12;
            person.ExpireDate12 = entity.ExpireDate12;

            person.IssueDate13 = entity.IssueDate13;
            person.ExpireDate13 = entity.ExpireDate13;

            person.IssueDate14 = entity.IssueDate14;
            person.ExpireDate14 = entity.ExpireDate14;

            person.IssueDate15 = entity.IssueDate15;
            person.ExpireDate15 = entity.ExpireDate15;

            person.IssueDate16 = entity.IssueDate16;
            person.ExpireDate16 = entity.ExpireDate16;

            person.IssueDate17 = entity.IssueDate17;
            person.ExpireDate17 = entity.ExpireDate17;

            person.IssueDate18 = entity.IssueDate18;
            person.ExpireDate18 = entity.ExpireDate18;

            person.IssueDate19 = entity.IssueDate19;
            person.ExpireDate19 = entity.ExpireDate19;

            person.IssueDate20 = entity.IssueDate20;
            person.ExpireDate20 = entity.ExpireDate20;

            person.IssueDate21 = entity.IssueDate21;
            person.ExpireDate21 = entity.ExpireDate21;

            person.IssueDate22 = entity.IssueDate22;
            person.ExpireDate22 = entity.ExpireDate22;

            person.IssueDate23 = entity.IssueDate23;
            person.ExpireDate23 = entity.ExpireDate23;

            person.IssueDate24 = entity.IssueDate24;
            person.ExpireDate24 = entity.ExpireDate24;

            person.IssueDate25 = entity.IssueDate25;
            person.ExpireDate25 = entity.ExpireDate25;

            person.IssueDate26 = entity.IssueDate26;
            person.ExpireDate26 = entity.ExpireDate26;

            person.IssueDate27 = entity.IssueDate27;
            person.ExpireDate27 = entity.ExpireDate27;
        
            person.IssueDate28 = entity.IssueDate28;
            person.ExpireDate28 = entity.ExpireDate28;

            person.IssueDate29 = entity.IssueDate29;
            person.ExpireDate29 = entity.ExpireDate29;

            person.IssueDate30 = entity.IssueDate30;
            person.ExpireDate30 = entity.ExpireDate30;

            person.IssueDate31 = entity.IssueDate31;
            person.ExpireDate31 = entity.ExpireDate31;

            person.IssueDate32 = entity.IssueDate32;
            person.ExpireDate32 = entity.ExpireDate32;

            person.IssueDate33 = entity.IssueDate33;
            person.ExpireDate33 = entity.ExpireDate33;

            person.IssueDate34 = entity.IssueDate34;
            person.ExpireDate34 = entity.ExpireDate34;

            person.IssueDate35 = entity.IssueDate35;
            person.ExpireDate35 = entity.ExpireDate35;


            person.IssueDate36 = entity.IssueDate36;
            person.IssueDate37 = entity.IssueDate37;
            person.IssueDate38 = entity.IssueDate38;
            person.IssueDate39 = entity.IssueDate39;
            person.IssueDate40 = entity.IssueDate40;
            person.IssueDate41 = entity.IssueDate41;
            person.IssueDate42 = entity.IssueDate42;
            person.IssueDate43 = entity.IssueDate43;
            person.IssueDate44 = entity.IssueDate44;
            person.IssueDate45 = entity.IssueDate45;
            person.IssueDate46 = entity.IssueDate46;
            person.IssueDate47 = entity.IssueDate47;
            person.IssueDate48 = entity.IssueDate48;
            person.IssueDate49 = entity.IssueDate49;
            person.IssueDate50 = entity.IssueDate50;
            person.ExpireDate51 = entity.ExpireDate51;
            person.ExpireDate52 = entity.ExpireDate52;
            person.ExpireDate53 = entity.ExpireDate53;
            person.ExpireDate54 = entity.ExpireDate54;
            person.ExpireDate55 = entity.ExpireDate55;
            person.ExpireDate56 = entity.ExpireDate56;
            person.ExpireDate57 = entity.ExpireDate57;
            person.ExpireDate58 = entity.ExpireDate58;
            person.ExpireDate59 = entity.ExpireDate59;
            person.ExpireDate60 = entity.ExpireDate60;
            person.IssueDate51 = entity.IssueDate51;
            person.IssueDate52 = entity.IssueDate52;
            person.IssueDate53 = entity.IssueDate53;
            person.IssueDate54 = entity.IssueDate54;
            person.IssueDate55 = entity.IssueDate55;
            person.IssueDate56 = entity.IssueDate56;
            person.IssueDate57 = entity.IssueDate57;
            person.IssueDate58 = entity.IssueDate58;
            person.IssueDate59 = entity.IssueDate59;
            person.IssueDate60 = entity.IssueDate60;
            person.IssueDate61 = entity.IssueDate61;
            person.IssueDate62 = entity.IssueDate62;
            person.ExpireDate61 = entity.ExpireDate61;
            person.ExpireDate62 = entity.ExpireDate62;
            person.IssueDate63 = entity.IssueDate63;
            person.IssueDate64 = entity.IssueDate64;
            person.IssueDate65 = entity.IssueDate65;
            person.ExpireDate63 = entity.ExpireDate63;
            person.ExpireDate64 = entity.ExpireDate64;
            person.ExpireDate65 = entity.ExpireDate65;
            person.IssueDate66 = entity.IssueDate66;
            person.IssueDate67 = entity.IssueDate67;
            person.IssueDate68 = entity.IssueDate68;
            person.IssueDate69 = entity.IssueDate69;
            person.IssueDate70 = entity.IssueDate70;
            person.ExpireDate66 = entity.ExpireDate66;
            person.ExpireDate67 = entity.ExpireDate67;
            person.ExpireDate68 = entity.ExpireDate68;
            person.ExpireDate69 = entity.ExpireDate69;
            person.ExpireDate70 = entity.ExpireDate70;
            person.IssueDate71 = entity.IssueDate71;
            person.ExpireDate71 = entity.ExpireDate71;
            person.IssueDate72 = entity.IssueDate72;
            person.ExpireDate72 = entity.ExpireDate72;
            person.IssueDate73 = entity.IssueDate73;
            person.ExpireDate73 = entity.ExpireDate73;
            person.IssueDate74 = entity.IssueDate74;
            person.ExpireDate74 = entity.ExpireDate74;
            person.IssueDate75 = entity.IssueDate75;
            person.IssueDate76 = entity.IssueDate76;
            person.IssueDate77 = entity.IssueDate77;
            person.ExpireDate75 = entity.ExpireDate75;
            person.ExpireDate76 = entity.ExpireDate76;
            person.ExpireDate77 = entity.ExpireDate77;

            person.ExpireDateTRG02 = entity.ExpireDateTRG02;
            person.IssueDateTRG02 = entity.IssueDateTRG02;


            person.EFBIssueDate = entity.EFBIssueDate;
            person.EFBExpireDate = entity.EFBExpireDate;
            person.RIGHT_SEAT_QUALIFICATION_IssueDate = entity.RIGHT_SEAT_QUALIFICATION_IssueDate;
            person.RIGHT_SEAT_QUALIFICATION_ExpireDate = entity.RIGHT_SEAT_QUALIFICATION_ExpireDate;
            person.ELTIssueDate = entity.ELTIssueDate;
            person.ELTExpireDate = entity.ELTExpireDate;
            person.RVSMIssueDate = entity.RVSMIssueDate;
            person.RVSMExpireDate = entity.RVSMExpireDate;
            person.PILOT_INCAPACITATION_IssueDate = entity.PILOT_INCAPACITATION_IssueDate;
            person.PILOT_INCAPACITATION_ExpireDate = entity.PILOT_INCAPACITATION_ExpireDate;

            person.SafetyPilotIssueDate = entity.SafetyPilotIssueDate;
            person.SafetyPilotExpireDate = entity.SafetyPilotExpireDate;

            person.RouteCheckIssueDate = entity.RouteCheckIssueDate;
            person.RouteCheckExpireDate = entity.RouteCheckExpireDate;

            person.LAR_IssueDate = entity.LAR_IssueDate;
            person.LAR_ExpireDate = entity.LAR_ExpireDate;
            person.LOAD_CONTROL_ExpireDate = entity.LOAD_CONTROL_ExpireDate;
            person.LOAD_CONTROL_IssueDate = entity.LOAD_CONTROL_IssueDate;


            person.Category = entity.Category;
            person.PROFICIENCY_ASSESSMENT_IsuueDate = entity.PROFICIENCY_ASSESSMENT_IsuueDate;
            person.PROFICIENCY_ASSESSMENT_ExpireDate = entity.PROFICIENCY_ASSESSMENT_ExpireDate;
            person.MPIssueDate = entity.MPIssueDate;
            person.MPExpireDate = entity.MPExpireDate;
            person.CALRIssueDate = entity.CALRIssueDate;
            person.CALRExpireDate = entity.CALRExpireDate;
            person.SpecialApprovalIssueDate = entity.SpecialApprovalIssueDate;
            person.SpecialApprovalExpireDate = entity.SpecialApprovalExpireDate;
            person.TRG01IssueDate = entity.TRG01IssueDate;
            person.TRG01ExpireDate = entity.TRG01ExpireDate;
            person.TRG16IssueDate = entity.TRG16IssueDate;
            person.TRG16ExpireDate = entity.TRG16ExpireDate;
            person.TRG07AIssueDate = entity.TRG07AIssueDate;
            person.TRG07AExpireDate = entity.TRG07AExpireDate;

            person.DISPATCH_MANUAL_FAM_IssueDate = entity.DISPATCH_MANUAL_FAM_IssueDate;
            person.DISPATCH_MANUAL_FAM_ExpireDate = entity.DISPATCH_MANUAL_FAM_ExpireDate;


            person.Phase1IssueDate = entity.Phase1IssueDate;
            person.Phase1ExpireDate = entity.Phase1ExpireDate;

            person.Phase2IssueDate = entity.Phase2IssueDate;
            person.Phase2ExpireDate = entity.Phase2ExpireDate;

            person.Phase3IssueDate = entity.Phase3IssueDate;
            person.Phase3ExpireDate = entity.Phase3ExpireDate;

            person.TRIIssueDate = entity.TRIIssueDate;
            person.TREIssueDate = entity.TREIssueDate;
            person.GroundIssueDate = entity.GroundIssueDate;
            person.GroundExpireDate = entity.GroundExpireDate;
            person.FirstAidCockpitIssueDate = entity.FirstAidCockpitIssueDate;
            person.FirstAidCockpitExpireDate = entity.FirstAidCockpitExpireDate;

            person.CONVERSION_IssueDate = entity.CONVERSION_IssueDate;
            person.CONVERSION_ExpireDate = entity.CONVERSION_ExpireDate;

            person.TypeFoker100ExpireDate = entity.TypeFoker100ExpireDate;
            person.TypeFoker100IssueDate = entity.TypeFoker100IssueDate;
            person.TypeFoker50ExpireDate = entity.TypeFoker50ExpireDate;
            person.TypeFoker50IssueDate = entity.TypeFoker50IssueDate;

            person.BaseAirline = entity.BaseAirline;

            person.IsType737 = entity.IsType737;
            person.IsTypeAirbus = entity.IsTypeAirbus;
            person.IsTypeFokker = entity.IsTypeFokker;
            person.IsTypeMD = entity.IsTypeMD;


            person.OtherAirline = entity.OtherAirline == null ? false : (bool)entity.OtherAirline;

        }
    }
    public class PersonCustomer
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public DateTime? DateJoinCompany { get; set; }
        public decimal? DateJoinCompanyP { get; set; }
        public bool IsActive { get; set; }
        public decimal? DateRegisterP { get; set; }
        public decimal? DateConfirmedP { get; set; }
        public DateTime? DateRegister { get; set; }
        public DateTime? DateConfirmed { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DateActiveStart { get; set; }
        public DateTime? DateActiveEnd { get; set; }
        public decimal? DateLastLoginP { get; set; }
        public DateTime? DateLastLogin { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int? CustomerId { get; set; }
        public Nullable<int> GroupId { get; set; }

        public Nullable<int> C1GroupId { get; set; }
        public Nullable<int> C2GroupId { get; set; }
        public Nullable<int> C3GroupId { get; set; }

        public Person Person { get; set; }
        public static void Fill(Models.PersonCustomer entity, ViewModels.Employee personcustomer)
        {
            entity.Id = personcustomer.Id;
            entity.PersonId = personcustomer.PersonId;
            entity.DateJoinCompany = personcustomer.DateJoinCompany;
          //  if (personcustomer.DateJoinCompany != null)
           //     entity.DateJoinCompanyP = Convert.ToDecimal(Utils.DateTimeUtil.GetPersianDateDigital((DateTime)personcustomer.DateJoinCompany));
            entity.IsActive = personcustomer.IsActive;

            if (entity.Id == -1)
            {
                entity.DateRegister = DateTime.Now;
               // entity.DateRegisterP = Convert.ToDecimal(Utils.DateTimeUtil.GetPersianDateTimeDigital((DateTime)entity.DateRegister));
                entity.DateConfirmed = personcustomer.DateRegister;
                entity.DateConfirmedP = entity.DateRegisterP;
            }


            entity.IsDeleted = personcustomer.IsDeleted;
            entity.DateActiveStart = personcustomer.DateActiveStart;
            entity.DateActiveEnd = personcustomer.DateActiveEnd;
            // entity.DateLastLoginP = personcustomer.DateLastLoginP;
            //entity.DateLastLogin = personcustomer.DateLastLogin;
            entity.Username = personcustomer.PID;
            entity.Password = personcustomer.PID;
            entity.CustomerId = personcustomer.CustomerId;
            entity.GroupId = personcustomer.GroupId;

            entity.C1GroupId = personcustomer.C1GroupId;
            entity.C2GroupId = personcustomer.C2GroupId;
            entity.C3GroupId = personcustomer.C3GroupId;
        }
    }

    public partial class PersonAircraftType
    {
        public string AircraftType { get; set; }
        public int ManufacturerId { get; set; }
        public string Manufacturer { get; set; }
        public int Id { get; set; }
        public int AircraftTypeId { get; set; }
        public int PersonId { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> DateLimitBegin { get; set; }
        public Nullable<System.DateTime> DateLimitEnd { get; set; }
        public string Remark { get; set; }
        public static void Fill(Models.PersonAircraftType entity, ViewModels.PersonAircraftType personaircrafttype)
        {
            entity.Id = personaircrafttype.Id;
            entity.AircraftTypeId = personaircrafttype.AircraftTypeId;
            entity.PersonId = personaircrafttype.PersonId;
            entity.IsActive = personaircrafttype.IsActive;
            entity.DateLimitBegin = personaircrafttype.DateLimitBegin;
            entity.DateLimitEnd = personaircrafttype.DateLimitEnd;
            entity.Remark = personaircrafttype.Remark;
        }
        public static void FillDto(Models.ViewPersonAircraftType entity, ViewModels.PersonAircraftType viewpersonaircrafttype)
        {
            viewpersonaircrafttype.AircraftType = entity.AircraftType;
            viewpersonaircrafttype.ManufacturerId = entity.ManufacturerId;
            viewpersonaircrafttype.Manufacturer = entity.Manufacturer;
            viewpersonaircrafttype.Id = entity.Id;
            viewpersonaircrafttype.AircraftTypeId = entity.AircraftTypeId;
            viewpersonaircrafttype.PersonId = entity.PersonId;
            viewpersonaircrafttype.IsActive = entity.IsActive;
            viewpersonaircrafttype.DateLimitBegin = entity.DateLimitBegin;
            viewpersonaircrafttype.DateLimitEnd = entity.DateLimitEnd;
            viewpersonaircrafttype.Remark = entity.Remark;
        }
        public static ViewModels.PersonAircraftType GetDto(Models.ViewPersonAircraftType entity)
        {
            var result = new ViewModels.PersonAircraftType();
            FillDto(entity, result);
            return result;
        }
        public static List<ViewModels.PersonAircraftType> GetDtos(List<Models.ViewPersonAircraftType> entities)
        {
            var result = new List<ViewModels.PersonAircraftType>();
            foreach (var x in entities)
                result.Add(GetDto(x));
            return result;

        }
    }


    public partial class PersonAircraftType2
    {
        public string AircraftType { get; set; }
         
        public int AircraftTypeId { get; set; }
        public static void FillDto(Models.ViewPersonAircraftType entity, ViewModels.PersonAircraftType2 viewpersonaircrafttype)
        {
            viewpersonaircrafttype.AircraftType = entity.AircraftType;
          
            viewpersonaircrafttype.AircraftTypeId = entity.AircraftTypeId;
           
        }
        public static ViewModels.PersonAircraftType2 GetDto(Models.ViewPersonAircraftType entity)
        {
            var result = new ViewModels.PersonAircraftType2();
            FillDto(entity, result);
            return result;
        }
        public static List<ViewModels.PersonAircraftType2> GetDtos(List<Models.ViewPersonAircraftType> entities)
        {
            var result = new List<ViewModels.PersonAircraftType2>();
            foreach (var x in entities)
                result.Add(GetDto(x));
            return result;

        }

    }

    public partial class PersonDocument
    {
        public int PersonId { get; set; }
        public string Title { get; set; }
        public string Remark { get; set; }
        public int DocumentTypeId { get; set; }
        public int Id { get; set; }
        public string DocumentType { get; set; }
        public DateTime? DateIssue { get; set; }
        public DateTime? DateExpire { get; set; }

        List<Document> documents = null;
        public List<Document> Documents
        {
            get
            {
                if (documents == null)
                    documents = new List<Document>();
                return documents;

            }
            set { documents = value; }
        }
        public static void Fill(Models.ViewPersonDocument entity, ViewModels.PersonDocument viewpersondocument)
        {


            entity.PersonId = viewpersondocument.PersonId;
            entity.Title = viewpersondocument.Title;
            entity.Remark = viewpersondocument.Remark;
            entity.DocumentTypeId = viewpersondocument.DocumentTypeId;
            entity.Id = viewpersondocument.Id;
            entity.DocumentType = viewpersondocument.DocumentType;
        }
        public static void FillDto(Models.ViewPersonDocument entity, ViewModels.PersonDocument viewpersondocument)
        {
            viewpersondocument.PersonId = entity.PersonId;
            viewpersondocument.Title = entity.Title;
            viewpersondocument.Remark = entity.Remark;
            viewpersondocument.DocumentTypeId = (int)entity.DocumentTypeId;
            viewpersondocument.Id = entity.Id;
            viewpersondocument.DocumentType = entity.DocumentType;
            viewpersondocument.DateExpire = entity.DateExpire;
            viewpersondocument.DateIssue = entity.DateIssue;
        }

        public static ViewModels.PersonDocument GetDto(Models.ViewPersonDocument entity, List<Models.ViewPersonDocumentFile> files)
        {
            var result = new ViewModels.PersonDocument();
            FillDto(entity, result);
            var docfiles = files.Where(q => q.PersonDocumentId == entity.Id).ToList();
            foreach (var x in docfiles)
            {
                result.Documents.Add(new Document()
                {
                    DocumentTypeId = x.DocumentTypeId,
                    FileType = x.FileType,
                    FileTypeId = x.FileTypeId,
                    FileUrl = x.FileUrl,
                    Id = x.Id,
                    SysUrl = x.SysUrl,
                    Title = x.Title,

                });
            }
            return result;
        }
        public static List<ViewModels.PersonDocument> GetDtos(List<Models.ViewPersonDocument> entities, List<Models.ViewPersonDocumentFile> files)
        {
            var result = new List<ViewModels.PersonDocument>();
            foreach (var x in entities)
                result.Add(GetDto(x, files));
            return result;

        }
    }

    public class EmployeeAbs
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string Phone1 { get; set; }
        public int? BaseAirportId { get; set; }
        public Nullable<int> GroupId { get; set; }
        public string NID { get; set; }
        public int SexId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateBirth { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string PassportNumber { get; set; }
        public DateTime? DatePassportIssue { get; set; }
        public DateTime? DatePassportExpire { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }

        public string LinkedIn { get; set; }
        public string WhatsApp { get; set; }
        public string Telegram { get; set; }
        public string PostalCode { get; set; }

    }
    public class Employee : PersonCustomer
    {

        public string PID { get; set; }
        public string Phone { get; set; }
        public int? BaseAirportId { get; set; }
        public DateTime? DateInactiveBegin { get; set; }

        public DateTime? DateInactiveEnd { get; set; }
        public bool? InActive { get; set; }

        List<EmployeeLocation> locations = null;
        public List<EmployeeLocation> Locations
        {
            get
            {
                if (locations == null)
                    locations = new List<EmployeeLocation>();
                return locations;

            }
            set { locations = value; }
        }
        public static void Fill(Models.Employee entity, ViewModels.Employee employee)
        {
            entity.Id = employee.Id;
            entity.InActive = employee.InActive;
            entity.PID = employee.PID;
            entity.Phone = employee.Phone;
            entity.BaseAirportId = employee.BaseAirportId;
            entity.DateInactiveBegin = employee.DateInactiveBegin;
            if (employee.DateInactiveEnd != null)
                entity.DateInactiveEnd = ((DateTime)employee.DateInactiveEnd).Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        public List<view_trn_certificate_history_last> Certificates { get; set; }
       
    }

    public partial class EmployeeLocation
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public int LocationId { get; set; }
        public bool IsMainLocation { get; set; }
        public Nullable<int> OrgRoleId { get; set; }
        public Nullable<decimal> DateActiveStartP { get; set; }
        public Nullable<decimal> DateActiveEndP { get; set; }
        public Nullable<System.DateTime> DateActiveStart { get; set; }
        public Nullable<System.DateTime> DateActiveEnd { get; set; }
        public string Remark { get; set; }
        public string Phone { get; set; }
        public string OrgRole { get; set; }
        public string Title { get; set; }
        public string FullCode { get; set; }
        public static void Fill(Models.EmployeeLocation entity, ViewModels.EmployeeLocation employeelocation)
        {
            entity.Id = employeelocation.Id;
            entity.EmployeeId = employeelocation.EmployeeId;
            entity.LocationId = employeelocation.LocationId;
            entity.IsMainLocation = employeelocation.IsMainLocation;
            entity.OrgRoleId = employeelocation.OrgRoleId;
            entity.DateActiveStartP = employeelocation.DateActiveStartP;
            entity.DateActiveEndP = employeelocation.DateActiveEndP;
            entity.DateActiveStart = employeelocation.DateActiveStart;
            entity.DateActiveEnd = employeelocation.DateActiveEnd;
            entity.Remark = employeelocation.Remark;
            entity.Phone = employeelocation.Phone;
        }
        public static void FillDto(Models.ViewEmployeeLocation entity, ViewModels.EmployeeLocation viewemployeelocation)
        {
            viewemployeelocation.Id = entity.Id;
            viewemployeelocation.EmployeeId = entity.EmployeeId;
            viewemployeelocation.LocationId = entity.LocationId;
            viewemployeelocation.IsMainLocation = entity.IsMainLocation;
            viewemployeelocation.OrgRoleId = entity.OrgRoleId;
            viewemployeelocation.DateActiveStartP = entity.DateActiveStartP;
            viewemployeelocation.DateActiveEndP = entity.DateActiveEndP;
            viewemployeelocation.DateActiveStart = entity.DateActiveStart;
            viewemployeelocation.DateActiveEnd = entity.DateActiveEnd;
            viewemployeelocation.Remark = entity.Remark;
            viewemployeelocation.Phone = entity.Phone;
            viewemployeelocation.OrgRole = entity.OrgRole;
            viewemployeelocation.Title = entity.Title;
            viewemployeelocation.FullCode = entity.FullCode;
        }
        public static ViewModels.EmployeeLocation GetDto(Models.ViewEmployeeLocation entity)
        {
            var result = new ViewModels.EmployeeLocation();
            FillDto(entity, result);
            return result;
        }
        public static List<ViewModels.EmployeeLocation> GetDtos(List<Models.ViewEmployeeLocation> entities)
        {
            var result = new List<ViewModels.EmployeeLocation>();
            foreach (var x in entities)
                result.Add(GetDto(x));
            return result;

        }
    }


}
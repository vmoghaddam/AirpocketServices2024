// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Xml.Serialization;

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class pps_Flight
{
    private int idField;

   // private OverflightCost overflightCostField;
    private List<pps_FIROverflightCost> overflightCost_CostField;
    private string overflightCost_CurrencyField;
    private double? overflightCost_TotalOverflightCostField;
    private  double? overflightCost_TotalTerminalCostField;

    private string flightLogIDField;

    private string xmlIDField;

    private string pPSNameField;

    private string aCFTAILField;

    private string dEPField;

    private string dESTField;

    private string aLT1Field;

    private string aLT2Field;

    private System.DateTime? sTDField;

    private  double? pAXField;

    private  double? fUELField;

    private double? lOADField;

    private double? validHrsField;

    private double? minFLField;

    private double? maxFLField;

    private List<pps_String> stringsField;

    //private List<pps_String> eROPSAltAptsField;

    //private List<pps_String> adequateAptField;

    //private List<pps_String> fIRField;

    //private List<pps_String> altAptsField;

    private string tOAField;

    private string fMDIDField;

    private string dESTSTDALTField;

    private double? fUELCOMPField;

    private string tIMECOMPField;

    private double? fUELCONTField;

    private string tIMECONTField;

    private double? pCTCONTField;

    private double? fUELMINField;

    private string tIMEMINField;

    private double? fUELTAXIField;

    private double? tIMETAXIField;

    private double? fUELEXTRAField;

    private string tIMEEXTRAField;

    private double? fUELLDGField;

    private string tIMELDGField;

    private double? zFMField;

    private double? gCDField;

    private double? eSADField;

    private string glField;

    private double? fUELBIASField;

    private System.DateTime? sTAField;

    private System.DateTime? eTAField;

    //private LocalTime localTimeField;

    // ---------- [Fields migrated from LocalTime] ----------
    private DateTime? localTime_Departure_STDField;
    private DateTime? localTime_Departure_ETDField;
    private DateTime? localTime_Departure_SunriseField;
    private DateTime? localTime_Departure_SunsetField;
    private DateTime? localTime_Destination_STAField;
    private DateTime? localTime_Destination_ETAField;
    private DateTime? localTime_Destination_SunriseField;
    private DateTime? localTime_Destination_SunsetField;

    private double? sCHBLOCKTIMEField;

    private string dISPField;

    private System.DateTime? lastEditDateField;

    private System.DateTime? latestFlightPlanDateField;

    private System.DateTime? latestDocumentUploadDateField;

    private double? fUELMINTOField;

    private double? tIMEMINTOField;

    private double? aRAMPField;

    private double? tIMEACTField;

    private double? fUELACTField;

    private string destERAField;

    private double? trafficLoadField;

    private string weightUnitField;

    private double? windComponentField;

    private string customerDataPPSField;

    private string customerDataScheduledField;

    private double? flField;

    private double? routeDistNMField;

    private string routeNameField;

    private string routeRemarkField;

    private double? emptyWeightField;

    private double? totalDistanceField;

    private double? altDistField;

    private double? destTimeField;

    private double? altTimeField;

    private double? altFuelField;

    private double? holdTimeField;

    private double? reserveTimeField;

    private double? cargoField;

    private double? actTOWField;

    private double? tripFuelField;

    private double? holdFuelField;

    //private Holding holdingField;
    // ---------- [Fields migrated from Holding] ----------
    private double? holding_FuelField;
    private double? holding_MinutesField;
    private string holding_ProfileField;
    private string holding_SpecificationField;
    private string holding_FuelFlowTypeField;

    private double? elwField;

    private string fuelPolicyField;

    private double? alt2TimeField;

    private double? alt2FuelField;

    private double? maxTOMField;

    private double? maxLMField;

    private double? maxZFMField;

    private System.DateTime? weatherObsTimeField;

    private System.DateTime? weatherPlanTimeField;

    private string mFCIField;

    private string cruiseProfileField;

    private double? tempTopOfClimbField;

    private string climbField;

    private string descendField;

    private string fuelPLField;

    private string descendWindField;

    private string climbProfileField;

    private string descendProfileField;

    private string holdProfileField;

    private string stepClimbProfileField;

    private string fuelContDefField;

    private string fuelAltDefField;

    private string amexsyStatusField;

    private double? avgTrackField;

    //private DEPTAF dEPTAFField;

    private string dEPMetarField;

    //private FlightNotam[] dEPNotamField;
    // ---------- [Consolidated Notam List] ----------
    private List<pps_Notam> NotamsField = new List<pps_Notam>();

    //private DESTTAF dESTTAFField;

    private string dESTMetarField;

    //private ALT1TAF aLT1TAFField;

    //private ALT2TAF aLT2TAFField;
    // ---------- [TAF Fields] ----------
    private List<pps_TAF> tAFsField;
    //private pps_TAF depTAFField;
    //private pps_TAF destTAFField;
    //private pps_TAF alt1TAFField;
    //private pps_TAF alt2TAFField;


    private string aLT1MetarField;

    private string aLT2MetarField;

    //private ALT1Notam aLT1NotamField;

    private List<pps_RoutePoint> routePointsField;

    private List<pps_Crew> crewsField;//= new List<pps_Crew>();

    private bool responce_SucceedField;

    //private FlightATCData aTCDataField;
    private string aTCData_ATCIDField;
    private string aTCData_ATCTOAField;
    private string aTCData_ATCRuleField;
    private string aTCData_ATCTypeField;
    private string aTCData_ATCNumField;
    private string aTCData_ATCWakeField;
    private string aTCData_ATCEquiField;
    private string aTCData_ATCSSRField;
    private string aTCData_ATCDepField;
    private string aTCData_ATCTimeField;
    private string aTCData_ATCSpeedField;
    private string aTCData_ATCFLField;
    private string aTCData_ATCRouteField;
    private string aTCData_ATCDestField;
    private string aTCData_ATCEETField;
    private string aTCData_ATCAlt1Field;
    private string aTCData_ATCAlt2Field;
    private string aTCData_ATCInfoField;
    private string aTCData_ATCEnduField;
    private string aTCData_ATCPersField;
    private string aTCData_ATCRadiField;
    private string aTCData_ATCSurvField;
    private string aTCData_ATCJackField;
    private string aTCData_ATCDingField;
    private string aTCData_ATCCapField;
    private string aTCData_ATCCoverField;
    private string aTCData_ATCColoField;
    private string aTCData_ATCAccoField;
    private string aTCData_ATCRemField;
    private string aTCData_ATCPICField;
    private string aTCData_ATCCtotField;


    private List<pps_FlightLevel> optFlightLevelsField;

    //private FlightNotam1[] adequateNotamField;

    //private FlightNotam2[] fIRNotamField;

    private List<pps_AltAirport> airportsField;

    private string enrouteAlternatesField;

    //private pps_RoutePoint[] alt1PointsField;

    //private RoutePoint2[] alt2PointsField;

    private string stdAlternatesField;

    private string tOALTField;

    //private FlightRouteStrings routeStringsField;
    private string routeStrings_ToDestField;
    private string routeStrings_ToAlt1Field;
    private string routeStrings_ToAlt2Field;
    private string routeStrings_TOAltField;

    private string dEPIATAField;

    private string dESTIATAField;

    private double? finalReserveMinutesField;

    private double? finalReserveFuelField;

    private double? addFuelMinutesField;

    private double? addFuelField;

    private string flightSummaryField;

    private string passThroughValuesField;

    //private FlightEtopsInformation etopsInformationField;
    private double? etopsInformation_RuleTimeUsedField;
    private double? etopsInformation_IcingPercentageField;


    private double? fuelINCRBurnField;

    private List<pps_CorrectionTable> correctionTablesField;

    private string externalFlightIdField;

    private string gUFIField;

    //private FlightSidAndStarProcedures sidAndStarProceduresField;
    private string sidProcedures_NameField;
    private string sidProcedures_InfoField;
    private string starProcedures_NameField;
    private string starProcedures_InfoField;

    private double? fMRIField;

    //private FlightLoad loadField;
    private double? load_Fuel_ActTotalField;
    private double? load_Fuel_Section_ActMassField;
    private string load_Fuel_Section_IDField;
    private double? load_Cargo_ActTotalField;
    private double? load_Cargo_Section_ActMassField;
    private string load_Cargo_Section_IDField;
    private double? load_Pax_TotalField;
    private double? load_Pax_Data_MaxPaxField;
    private double? load_Pax_Data_ActPaxField;
    private double? load_Pax_Data_ActMassField;
    private double? load_Pax_Data_PaxAmountField;
    private double? load_Pax_Data_MaleField;
    private double? load_Pax_Data_FemaleField;
    private double? load_Pax_Data_ChildrenField;
    private double? load_Pax_Data_InfantField;
    private string load_Pax_Data_CustMassField;
    private List<pps_PaxSection> load_Pax_SectionsField;
    private double? load_MassBalance_Takeoff_ForwardLimitField;
    private double? load_MassBalance_Takeoff_ActualPositionField;
    private double? load_MassBalance_Takeoff_AftLimitField;
    private double? load_MassBalance_Landing_ForwardLimitField;
    private double? load_MassBalance_Landing_ActualPositionField;
    private double? load_MassBalance_Landing_AftLimitField;
    private double? load_MassBalance_ZeroFuel_ForwardLimitField;
    private double? load_MassBalance_ZeroFuel_ActualPositionField;
    private double? load_MassBalance_ZeroFuel_AftLimitField;
    private double? load_MassBalance_Index_DryOperatingIndexField;
    private double? load_MassBalance_Index_ZeroFuelForwardLimitField;
    private double? load_MassBalance_Index_ZeroFuelWeightIndexField;
    private double? load_MassBalance_Index_ZeroFuelAftLimitField;
    private double? load_Payload_MaxPayloadField;
    private double? load_Payload_MzfmField;
    private double? load_Payload_MtomField;
    private double? load_Payload_MlmField;
    private double? load_Payload_MrmpField;
    private double? load_Payload_MaxCargoField;
    private double? load_DryOperating_BasicEmptyArmField;
    private double? load_DryOperating_BasicEmptyWeightField;
    private double? load_DryOperating_DryOperatingMassArmField;
    private double? load_DryOperating_DryOperatingWeightField;

    //private FlightAircraftConfiguration aircraftConfigurationField;
    private string aircraftConfiguration_NameField;

    private List<pps_Crew> aircraftConfiguration_CrewsField;

    private bool isRecalcField;

    private double? maxRampWeightField;

    private string underloadFactorField;

    private double? avgISAField;

    private string hWCorrection20KtsTimeField;

    private double? hWCorrection20KtsFuelField;

    private double? correction1TONField;

    private double? correction2TONField;

    private string rcfHeaderField;

    private bool aLT2AsInfoOnlyField;

    //private FlightPpsVersionInformation ppsVersionInformationField;
    private string ppsApplicationVersionField;
    private string ppsExeVersionField;

    //private FlightCustomReferences customReferencesField;
    private string customReferences_RefID;
    private string customReferences_MilID;

    private string cFMUStatusField;

    private double? structuralTOMField;

    private double? fW1Field;

    private double? fW2Field;

    private double? fW3Field;

    private double? fW4Field;

    private double? fW5Field;

    private double? fW6Field;

    private double? fW7Field;

    private double? fW8Field;

    private double? fW9Field;

    private double? tOTALPAXWEIGHTField;

    private double? alt2DistField;

    private string fMSIdentField;

    private List<pps_ExtraFuel> extraFuelsField;

    private double? aircraftFuelBiasField;

    private double? melFuelBiasField;

    private List<pps_AirportWeatherData> planningEnRouteAlternateAirportsField;

    /// <remarks/>
    public int Id { get => this.idField; set => this.idField = value; }
    
    /// <remarks/>

    [System.Xml.Serialization.XmlArrayItemAttribute("FIROverflightCost", IsNullable = false)]
    public List<pps_FIROverflightCost> OverflightCost_Cost { get { return this.overflightCost_CostField; } set { this.overflightCost_CostField = value; } }

    // ---------- [Properties migrated from LocalTime] ----------
    public DateTime? LocalTime_Departure_STD { get => this.localTime_Departure_STDField; set => this.localTime_Departure_STDField = value; }
    public DateTime? LocalTime_Departure_ETD { get => this.localTime_Departure_ETDField; set => this.localTime_Departure_ETDField = value; }
    public DateTime? LocalTime_Departure_Sunrise { get => this.localTime_Departure_SunriseField; set => this.localTime_Departure_SunriseField = value; }
    public DateTime? LocalTime_Departure_Sunset { get => this.localTime_Departure_SunsetField; set => this.localTime_Departure_SunsetField = value; }
    public DateTime? LocalTime_Destination_STA { get => this.localTime_Destination_STAField; set => this.localTime_Departure_STDField = value; }
    public DateTime? LocalTime_Destination_ETA { get => this.localTime_Destination_ETAField; set => this.localTime_Departure_ETDField = value; }
    public DateTime? LocalTime_Destination_Sunrise { get => this.localTime_Destination_SunriseField; set => this.localTime_Departure_SunriseField = value; }
    public DateTime? LocalTime_Destination_Sunset { get => this.localTime_Destination_SunsetField; set => this.localTime_Departure_SunsetField = value; }


    public string OverflightCost_Currency { get => this.overflightCost_CurrencyField; set => this.overflightCost_CurrencyField = value; }
    public double? OverflightCost_TotalOverflightCost { get => this.overflightCost_TotalOverflightCostField; set => this.overflightCost_TotalOverflightCostField = value; }
    public double? OverflightCost_TotalTerminalCost { get => this.overflightCost_TotalTerminalCostField; set => this.overflightCost_TotalTerminalCostField = value; }

    // ---------- [Properties migrated from Holding] ----------
    public double? Holding_Fuel { get => this.holding_FuelField; set => this.holding_FuelField = value; }
    public double? Holding_Minutes { get => this.holding_MinutesField; set => this.holding_MinutesField = value; }
    public string Holding_Profile { get => this.holding_ProfileField; set => this.holding_ProfileField = value; }
    public string Holding_Specification { get => this.holding_SpecificationField; set => this.holding_SpecificationField = value; }
    public string Holding_FuelFlowType { get => this.holding_FuelFlowTypeField; set => this.holding_FuelFlowTypeField = value; }

    // ---------- [TAF Properties] ----------
    public List<pps_TAF> TAFs { get => this.tAFsField; set => this.tAFsField = value; }
  
    // NOTAM---------------------------------
    [XmlArray("Notams")]
    [XmlArrayItem("Notam", typeof(pps_Notam), IsNullable = false)]
    public List<pps_Notam> Notams { get => NotamsField; set => NotamsField = value; }
    public string FlightLogID { get => this.flightLogIDField; set => this.flightLogIDField = value; }
    public string XmlID { get => this.xmlIDField; set => this.xmlIDField = value; }
    public string PPSName { get => this.pPSNameField; set => this.pPSNameField = value; }
    public string ACFTAIL { get => this.aCFTAILField; set => this.aCFTAILField = value; }
    public string DEP { get => this.dEPField; set => this.dEPField = value; }
    public string DEST { get => this.dESTField; set => this.dESTField = value; }
    public string ALT1 { get => this.aLT1Field; set => this.aLT1Field = value; }
    public string ALT2 { get => this.aLT2Field; set => this.aLT2Field = value; }
    public System.DateTime? STD { get => this.sTDField; set => this.sTDField = value; }
    public double? PAX { get => this.pAXField; set => this.pAXField = value; }
    public double? FUEL { get => this.fUELField; set => this.fUELField = value; }
    public double? LOAD { get => this.lOADField; set => this.lOADField = value; }
    public double? ValidHrs { get => this.validHrsField; set => this.validHrsField = value; }
    public double? MinFL { get => this.minFLField; set => this.minFLField = value; }
    public double? MaxFL { get => this.maxFLField; set => this.maxFLField = value; }
    public List<pps_String> Strings { get => this.stringsField; set => this.stringsField = value; }
    public string TOA { get => this.tOAField; set => this.tOAField = value; }
    public string FMDID { get => this.fMDIDField; set => this.fMDIDField = value; }
    public string DESTSTDALT { get => this.dESTSTDALTField; set => this.dESTSTDALTField = value; }
    public double? FUELCOMP { get => this.fUELCOMPField; set => this.fUELCOMPField = value; }
    public string TIMECOMP { get => this.tIMECOMPField; set => this.tIMECOMPField = value; }
    public double? FUELCONT { get => this.fUELCONTField; set => this.fUELCONTField = value; }
    public string TIMECONT { get => this.tIMECONTField; set => this.tIMECONTField = value; }
    public double? PCTCONT { get => this.pCTCONTField; set => this.pCTCONTField = value; }
    public double? FUELMIN { get => this.fUELMINField; set => this.fUELMINField = value; }
    public string TIMEMIN { get => this.tIMEMINField; set => this.tIMEMINField = value; }
    public double? FUELTAXI { get => this.fUELTAXIField; set => this.fUELTAXIField = value; }
    public double? TIMETAXI { get => this.tIMETAXIField; set => this.tIMETAXIField = value; }
    public double? FUELEXTRA { get => this.fUELEXTRAField; set => this.fUELEXTRAField = value; }
    public string TIMEEXTRA { get => this.tIMEEXTRAField; set => this.tIMEEXTRAField = value; }
    public double? FUELLDG { get => this.fUELLDGField; set => this.fUELLDGField = value; }
    public string TIMELDG { get => this.tIMELDGField; set => this.tIMELDGField = value; }
    public double? ZFM { get => this.zFMField; set => this.zFMField = value; }
    public double? GCD { get => this.gCDField; set => this.gCDField = value; }

    public double? ESAD { get => this.eSADField; set => this.eSADField = value; }
    public string GL { get => this.glField; set => this.glField = value; }
    public double? FUELBIAS { get => this.fUELBIASField; set => this.fUELBIASField = value; }
    public DateTime? STA { get => this.sTAField; set => this.sTAField = value; }
    public DateTime? ETA { get => this.eTAField; set => this.eTAField = value; }
    public double? SCHBLOCKTIME { get => this.sCHBLOCKTIMEField; set => this.sCHBLOCKTIMEField = value; }
    public string DISP { get => this.dISPField; set => this.dISPField = value; }
    public DateTime? LastEditDate { get => this.lastEditDateField; set => this.lastEditDateField = value; }
    public DateTime? LatestFlightPlanDate { get => this.latestFlightPlanDateField; set => this.latestFlightPlanDateField = value; }
    public DateTime? LatestDocumentUploadDate { get => this.latestDocumentUploadDateField; set => this.latestDocumentUploadDateField = value; }
    public double? FUELMINTO { get => this.fUELMINTOField; set => this.fUELMINTOField = value; }
    public double? TIMEMINTO { get => this.tIMEMINTOField; set => this.tIMEMINTOField = value; }
    public double? ARAMP { get => this.aRAMPField; set => this.aRAMPField = value; }
    public double? TIMEACT { get => this.tIMEACTField; set => this.tIMEACTField = value; }
    public double? FUELACT { get => this.fUELACTField; set => this.fUELACTField = value; }
    public string DestERA { get => this.destERAField; set => this.destERAField = value; }
    public double? TrafficLoad { get => this.trafficLoadField; set => this.trafficLoadField = value; }
    public string WeightUnit { get => this.weightUnitField; set => this.weightUnitField = value; }
    public double? WindComponent { get => this.windComponentField; set => this.windComponentField = value; }
    public string CustomerDataPPS { get => this.customerDataPPSField; set => this.customerDataPPSField = value; }
    public string CustomerDataScheduled { get => this.customerDataScheduledField; set => this.customerDataScheduledField = value; }
    public double? Fl { get => this.flField; set => this.flField = value; }
    public double? RouteDistNM { get => this.routeDistNMField; set => this.routeDistNMField = value; }
    public string RouteName { get => this.routeNameField; set => this.routeNameField = value; }
    public string RouteRemark { get => this.routeRemarkField; set => this.routeRemarkField = value; }
    public double? EmptyWeight { get => this.emptyWeightField; set => this.emptyWeightField = value; }
    public double? TotalDistance { get => this.totalDistanceField; set => this.totalDistanceField = value; }
    public double? AltDist { get => this.altDistField; set => this.altDistField = value; }
    public double? DestTime { get => this.destTimeField; set => this.destTimeField = value; }
    public double? AltTime { get => this.altTimeField; set => this.altTimeField = value; }
    public double? AltFuel { get => this.altFuelField; set => this.altFuelField = value; }
    public double? HoldTime { get => this.holdTimeField; set => this.holdTimeField = value; }
    public double? ReserveTime { get => this.reserveTimeField; set => this.reserveTimeField = value; }
    public double? Cargo { get => this.cargoField; set => this.cargoField = value; }
    public double? ActTOW { get => this.actTOWField; set => this.actTOWField = value; }
    public double? TripFuel { get => this.tripFuelField; set => this.tripFuelField = value; }
    public double? HoldFuel { get => this.holdFuelField; set => this.holdFuelField = value; }
    public double? Elw { get => this.elwField; set => this.elwField = value; }
    public string FuelPolicy { get => this.fuelPolicyField; set => this.fuelPolicyField = value; }
    public double? Alt2Time { get => this.alt2TimeField; set => this.alt2TimeField = value; }
    public double? Alt2Fuel { get => this.alt2FuelField; set => this.alt2FuelField = value; }

    public double? MaxTOM { get { return this.maxTOMField; } set { this.maxTOMField = value; } }
    public double? MaxLM { get { return this.maxLMField; } set { this.maxLMField = value; } }
    public double? MaxZFM { get { return this.maxZFMField; } set { this.maxZFMField = value; } }
    public System.DateTime? WeatherObsTime { get { return this.weatherObsTimeField; } set { this.weatherObsTimeField = value; } }
    public System.DateTime? WeatherPlanTime { get { return this.weatherPlanTimeField; } set { this.weatherPlanTimeField = value; } }
    public string MFCI { get { return this.mFCIField; } set { this.mFCIField = value; } }
    public string CruiseProfile { get { return this.cruiseProfileField; } set { this.cruiseProfileField = value; } }
    public double? TempTopOfClimb { get { return this.tempTopOfClimbField; } set { this.tempTopOfClimbField = value; } }
    public string Climb { get { return this.climbField; } set { this.climbField = value; } }
    public string Descend { get { return this.descendField; } set { this.descendField = value; } }
    public string FuelPL { get { return this.fuelPLField; } set { this.fuelPLField = value; } }
    public string DescendWind { get { return this.descendWindField; } set { this.descendWindField = value; } }
    public string ClimbProfile { get { return this.climbProfileField; } set { this.climbProfileField = value; } }
    public string DescendProfile { get { return this.descendProfileField; } set { this.descendProfileField = value; } }
    public string HoldProfile { get { return this.holdProfileField; } set { this.holdProfileField = value; } }
    public string StepClimbProfile { get { return this.stepClimbProfileField; } set { this.stepClimbProfileField = value; } }
    public string FuelContDef { get { return this.fuelContDefField; } set { this.fuelContDefField = value; } }
    public string FuelAltDef { get { return this.fuelAltDefField; } set { this.fuelAltDefField = value; } }
    public string AmexsyStatus { get { return this.amexsyStatusField; } set { this.amexsyStatusField = value; } }
    public double? AvgTrack { get { return this.avgTrackField; } set { this.avgTrackField = value; } }
    public string DEPMetar { get { return this.dEPMetarField; } set { this.dEPMetarField = value; } }
    public string DESTMetar { get { return this.dESTMetarField; } set { this.dESTMetarField = value; } }
    public string ALT1Metar { get { return this.aLT1MetarField; } set { this.aLT1MetarField = value; } }
    public string ALT2Metar { get { return this.aLT2MetarField; } set { this.aLT2MetarField = value; } }
    [System.Xml.Serialization.XmlArrayItemAttribute("RoutePoint", IsNullable = false)]
    public List<pps_RoutePoint> RoutePoints { get { return this.routePointsField; } set { this.routePointsField = value; } }

    /// <remarks/>
    public List<pps_Crew> Crews   {   get => crewsField;  set => crewsField = value; }
    /// <remarks/>
    public bool Responce_Succeed { get => responce_SucceedField; set => responce_SucceedField = value; }

    [XmlElement("ATCID")] public string ATCData_ATCID { get => aTCData_ATCIDField; set => aTCData_ATCIDField = value; }
    [XmlElement("ATCTOA")] public string ATCData_ATCTOA { get => aTCData_ATCTOAField; set => aTCData_ATCTOAField = value; }
    [XmlElement("ATCRule")] public string ATCData_ATCRule { get => aTCData_ATCRuleField; set => aTCData_ATCRuleField = value; }
    [XmlElement("ATCType")] public string ATCData_ATCType { get => aTCData_ATCTypeField; set => aTCData_ATCTypeField = value; }
    [XmlElement("ATCNum")] public string ATCData_ATCNum { get => aTCData_ATCNumField; set => aTCData_ATCNumField = value; }
    [XmlElement("ATCWake")] public string ATCData_ATCWake { get => aTCData_ATCWakeField; set => aTCData_ATCWakeField = value; }
    [XmlElement("ATCEqui")] public string ATCData_ATCEqui { get => aTCData_ATCEquiField; set => aTCData_ATCEquiField = value; }
    [XmlElement("ATCSSR")] public string ATCData_ATCSSR { get => aTCData_ATCSSRField; set => aTCData_ATCSSRField = value; }
    [XmlElement("ATCDep")] public string ATCData_ATCDep { get => aTCData_ATCDepField; set => aTCData_ATCDepField = value; }
    [XmlElement("ATCTime")] public string ATCData_ATCTime { get => aTCData_ATCTimeField; set => aTCData_ATCTimeField = value; }
    [XmlElement("ATCSpeed")] public string ATCData_ATCSpeed { get => aTCData_ATCSpeedField; set => aTCData_ATCSpeedField = value; }
    [XmlElement("ATCFL")] public string ATCData_ATCFL { get => aTCData_ATCFLField; set => aTCData_ATCFLField = value; }
    [XmlElement("ATCRoute")] public string ATCData_ATCRoute { get => aTCData_ATCRouteField; set => aTCData_ATCRouteField = value; }
    [XmlElement("ATCDest")] public string ATCData_ATCDest { get => aTCData_ATCDestField; set => aTCData_ATCDestField = value; }
    [XmlElement("ATCEET")] public string ATCData_ATCEET { get => aTCData_ATCEETField; set => aTCData_ATCEETField = value; }
    [XmlElement("ATCAlt1")] public string ATCData_ATCAlt1 { get => aTCData_ATCAlt1Field; set => aTCData_ATCAlt1Field = value; }
    [XmlElement("ATCAlt2")] public string ATCData_ATCAlt2 { get => aTCData_ATCAlt2Field; set => aTCData_ATCAlt2Field = value; }
    [XmlElement("ATCInfo")] public string ATCData_ATCInfo { get => aTCData_ATCInfoField; set => aTCData_ATCInfoField = value; }
    [XmlElement("ATCEndu")] public string ATCData_ATCEndu { get => aTCData_ATCEnduField; set => aTCData_ATCEnduField = value; }
    [XmlElement("ATCPers")] public string ATCData_ATCPers { get => aTCData_ATCPersField; set => aTCData_ATCPersField = value; }
    [XmlElement("ATCRadi")] public string ATCData_ATCRadi { get => aTCData_ATCRadiField; set => aTCData_ATCRadiField = value; }
    [XmlElement("ATCSurv")] public string ATCData_ATCSurv { get => aTCData_ATCSurvField; set => aTCData_ATCSurvField = value; }
    [XmlElement("ATCJack")] public string ATCData_ATCJack { get => aTCData_ATCJackField; set => aTCData_ATCJackField = value; }
    [XmlElement("ATCDing")] public string ATCData_ATCDing { get => aTCData_ATCDingField; set => aTCData_ATCDingField = value; }
    [XmlElement("ATCCap")] public string ATCData_ATCCap { get => aTCData_ATCCapField; set => aTCData_ATCCapField = value; }
    [XmlElement("ATCCover")] public string ATCData_ATCCover { get => aTCData_ATCCoverField; set => aTCData_ATCCoverField = value; }
    [XmlElement("ATCColo")] public string ATCData_ATCColo { get => aTCData_ATCColoField; set => aTCData_ATCColoField = value; }
    [XmlElement("ATCAcco")] public string ATCData_ATCAcco { get => aTCData_ATCAccoField; set => aTCData_ATCAccoField = value; }
    [XmlElement("ATCRem")] public string ATCData_ATCRem { get => aTCData_ATCRemField; set => aTCData_ATCRemField = value; }
    [XmlElement("ATCPIC")] public string ATCData_ATCPIC { get => aTCData_ATCPICField; set => aTCData_ATCPICField = value; }
    [XmlElement("ATCCtot")] public string ATCData_ATCCtot { get => aTCData_ATCCtotField; set => aTCData_ATCCtotField = value; }

        /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("FlightLevel", IsNullable = false)]
    public List<pps_FlightLevel> OptFlightLevels { get => optFlightLevelsField; set => optFlightLevelsField = value; }
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("AltAirport", IsNullable = false)]
    public List<pps_AltAirport> Airports { get => airportsField; set => airportsField = value; }
    /// <remarks/>
    public string EnrouteAlternates { get { return this.enrouteAlternatesField; } set { this.enrouteAlternatesField = value; } }
    public string StdAlternates { get { return this.stdAlternatesField; } set { this.stdAlternatesField = value; } }
    public string TOALT { get { return this.tOALTField; } set { this.tOALTField = value; } }

    [System.Xml.Serialization.XmlElement("ToDest")] 
    public string RouteStrings_ToDest { get => routeStrings_ToDestField; set => routeStrings_ToDestField = value; }
    [System.Xml.Serialization.XmlElement("ToAlt1")] 
    public string RouteStrings_ToAlt1 { get => routeStrings_ToAlt1Field; set => routeStrings_ToAlt1Field = value; }
    [System.Xml.Serialization.XmlElement("ToAlt2")] 
    public string RouteStrings_ToAlt2 { get => routeStrings_ToAlt2Field; set => routeStrings_ToAlt2Field = value; }
    [System.Xml.Serialization.XmlElement("TOAlt")] 
    public string RouteStrings_TOAlt { get => routeStrings_TOAltField; set => routeStrings_TOAltField = value; }

    public string DEPIATA { get { return this.dEPIATAField; } set { this.dEPIATAField = value; } }
    public string DESTIATA { get { return this.dESTIATAField; } set { this.dESTIATAField = value; } }
    public double? FinalReserveMinutes { get { return this.finalReserveMinutesField; } set { this.finalReserveMinutesField = value; } }
    public double? FinalReserveFuel { get { return this.finalReserveFuelField; } set { this.finalReserveFuelField = value; } }
    public double? AddFuelMinutes { get { return this.addFuelMinutesField; } set { this.addFuelMinutesField = value; } }
    public double? AddFuel { get { return this.addFuelField; } set { this.addFuelField = value; } }
    public string FlightSummary { get { return this.flightSummaryField; } set { this.flightSummaryField = value; } }
    public string PassThroughValues { get { return this.passThroughValuesField; } set { this.passThroughValuesField = value; } }

    /// <remarks/>
    [System.Xml.Serialization.XmlElement("RuleTimeUsed")] 
    public double? EtopsInformation_RuleTimeUsed { get { return this.etopsInformation_RuleTimeUsedField; } set { this.etopsInformation_RuleTimeUsedField = value; } }
    [System.Xml.Serialization.XmlElement("IcingPercentage")] 
    public double? EtopsInformation_IcingPercentage { get { return this.etopsInformation_IcingPercentageField; } set { this.etopsInformation_IcingPercentageField = value; } }
    /// <remarks/>
    public double? FuelINCRBurn { get { return this.fuelINCRBurnField; } set { this.fuelINCRBurnField = value; } }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("CorrectionTable", IsNullable = false)]
    public List<pps_CorrectionTable> CorrectionTables { get { return this.correctionTablesField; } set { this.correctionTablesField = value; } }

    public string ExternalFlightId { get { return this.externalFlightIdField; } set { this.externalFlightIdField = value; } }
    public string GUFI { get { return this.gUFIField; } set { this.gUFIField = value; } }
    public string SidProcedures_Name { get { return this.sidProcedures_NameField; } set { this.sidProcedures_NameField = value; } }
    public string SidProcedures_Info { get { return this.sidProcedures_InfoField; } set { this.sidProcedures_InfoField = value; } }
    public string StarProcedures_Name { get { return this.starProcedures_NameField; } set { this.starProcedures_NameField = value; } }
    public string StarProcedures_Info { get { return this.starProcedures_InfoField; } set { this.starProcedures_InfoField = value; } }

    public double? FMRI { get { return this.fMRIField; } set { this.fMRIField = value; } }
    public double? Load_Fuel_ActTotal { get { return this.load_Fuel_ActTotalField; } set { this.load_Fuel_ActTotalField = value; } }
    public double? Load_Fuel_Section_ActMass { get { return this.load_Fuel_Section_ActMassField; } set { this.load_Fuel_Section_ActMassField = value; } }
    public string Load_Fuel_Section_ID { get { return this.load_Fuel_Section_IDField; } set { this.load_Fuel_Section_IDField = value; } }
    public double? Load_Cargo_ActTotal { get { return this.load_Cargo_ActTotalField; } set { this.load_Cargo_ActTotalField = value; } }
    public double? Load_Cargo_Section_ActMass { get { return this.load_Cargo_Section_ActMassField; } set { this.load_Cargo_Section_ActMassField = value; } }
    public string Load_Cargo_Section_ID { get { return this.load_Cargo_Section_IDField; } set { this.load_Cargo_Section_IDField = value; } }
    public double? Load_Pax_Total { get { return this.load_Pax_TotalField; } set { this.load_Pax_TotalField = value; } }
    public double? Load_Pax_Data_MaxPax { get { return this.load_Pax_Data_MaxPaxField; } set { this.load_Pax_Data_MaxPaxField = value; } }
    public double? Load_Pax_Data_ActPax { get { return this.load_Pax_Data_ActPaxField; } set { this.load_Pax_Data_ActPaxField = value; } }
    public double? Load_Pax_Data_ActMass { get { return this.load_Pax_Data_ActMassField; } set { this.load_Pax_Data_ActMassField = value; } }
    public double? Load_Pax_Data_PaxAmount { get { return this.load_Pax_Data_PaxAmountField; } set { this.load_Pax_Data_PaxAmountField = value; } }
    public double? Load_Pax_Data_Male { get { return this.load_Pax_Data_MaleField; } set { this.load_Pax_Data_MaleField = value; } }
    public double? Load_Pax_Data_Female { get { return this.load_Pax_Data_FemaleField; } set { this.load_Pax_Data_FemaleField = value; } }
    public double? Load_Pax_Data_Children { get { return this.load_Pax_Data_ChildrenField; } set { this.load_Pax_Data_ChildrenField = value; } }
    public double? Load_Pax_Data_Infant { get { return this.load_Pax_Data_InfantField; } set { this.load_Pax_Data_InfantField = value; } }
    public string Load_Pax_Data_CustMass { get { return this.load_Pax_Data_CustMassField; } set { this.load_Pax_Data_CustMassField = value; } }
    public List<pps_PaxSection> Load_Pax_Sections { get { return this.load_Pax_SectionsField; } set { this.load_Pax_SectionsField = value; } }
    public double? Load_MassBalance_Takeoff_ForwardLimit { get { return this.load_MassBalance_Takeoff_ForwardLimitField; } set { this.load_MassBalance_Takeoff_ForwardLimitField = value; } }
    public double? Load_MassBalance_Takeoff_ActualPosition { get { return this.load_MassBalance_Takeoff_ActualPositionField; } set { this.load_MassBalance_Takeoff_ActualPositionField = value; } }
    public double? Load_MassBalance_Takeoff_AftLimit { get { return this.load_MassBalance_Takeoff_AftLimitField; } set { this.load_MassBalance_Takeoff_AftLimitField = value; } }
    public double? Load_MassBalance_Landing_ForwardLimit { get { return this.load_MassBalance_Landing_ForwardLimitField; } set { this.load_MassBalance_Landing_ForwardLimitField = value; } }
    public double? Load_MassBalance_Landing_ActualPosition { get { return this.load_MassBalance_Landing_ActualPositionField; } set { this.load_MassBalance_Landing_ActualPositionField = value; } }
    public double? Load_MassBalance_Landing_AftLimit { get { return this.load_MassBalance_Landing_AftLimitField; } set { this.load_MassBalance_Landing_AftLimitField = value; } }
    public double? Load_MassBalance_ZeroFuel_ForwardLimit { get { return this.load_MassBalance_ZeroFuel_ForwardLimitField; } set { this.load_MassBalance_ZeroFuel_ForwardLimitField = value; } }
    public double? Load_MassBalance_ZeroFuel_ActualPosition { get { return this.load_MassBalance_ZeroFuel_ActualPositionField; } set { this.load_MassBalance_ZeroFuel_ActualPositionField = value; } }
    public double? Load_MassBalance_ZeroFuel_AftLimit { get { return this.load_MassBalance_ZeroFuel_AftLimitField; } set { this.load_MassBalance_ZeroFuel_AftLimitField = value; } }
    public double? Load_MassBalance_Index_DryOperatingIndex { get { return this.load_MassBalance_Index_DryOperatingIndexField; } set { this.load_MassBalance_Index_DryOperatingIndexField = value; } }
    public double? Load_MassBalance_Index_ZeroFuelForwardLimit { get { return this.load_MassBalance_Index_ZeroFuelForwardLimitField; } set { this.load_MassBalance_Index_ZeroFuelForwardLimitField = value; } }
    public double? Load_MassBalance_Index_ZeroFuelWeightIndex { get { return this.load_MassBalance_Index_ZeroFuelWeightIndexField; } set { this.load_MassBalance_Index_ZeroFuelWeightIndexField = value; } }
    public double? Load_MassBalance_Index_ZeroFuelAftLimit { get { return this.load_MassBalance_Index_ZeroFuelAftLimitField; } set { this.load_MassBalance_Index_ZeroFuelAftLimitField = value; } }
    public double? Load_Payload_MaxPayload { get { return this.load_Payload_MaxPayloadField; } set { this.load_Payload_MaxPayloadField = value; } }
    public double? Load_Payload_Mzfm { get { return this.load_Payload_MzfmField; } set { this.load_Payload_MzfmField = value; } }
    public double? Load_Payload_Mtom { get { return this.load_Payload_MtomField; } set { this.load_Payload_MtomField = value; } }
    public double? Load_Payload_Mlm { get { return this.load_Payload_MlmField; } set { this.load_Payload_MlmField = value; } }
    public double? Load_Payload_Mrmp { get { return this.load_Payload_MrmpField; } set { this.load_Payload_MrmpField = value; } }
    public double? Load_Payload_MaxCargo { get { return this.load_Payload_MaxCargoField; } set { this.load_Payload_MaxCargoField = value; } }
    public double? Load_DryOperating_BasicEmptyArm { get { return this.load_DryOperating_BasicEmptyArmField; } set { this.load_DryOperating_BasicEmptyArmField = value; } }
    public double? Load_DryOperating_BasicEmptyWeight { get { return this.load_DryOperating_BasicEmptyWeightField; } set { this.load_DryOperating_BasicEmptyWeightField = value; } }
    public double? Load_DryOperating_DryOperatingMassArm { get { return this.load_DryOperating_DryOperatingMassArmField; } set { this.load_DryOperating_DryOperatingMassArmField = value; } }
    public double? Load_DryOperating_DryOperatingWeight { get { return this.load_DryOperating_DryOperatingWeightField; } set { this.load_DryOperating_DryOperatingWeightField = value; } }

    /// <remarks/>
    public string AircraftConfiguration_Name { get { return this.aircraftConfiguration_NameField; } set { this.aircraftConfiguration_NameField = value; } }
    public List<pps_Crew> AircraftConfiguration_Crews { get { return this.aircraftConfiguration_CrewsField; } set { this.aircraftConfiguration_CrewsField = value; } }

    public bool IsRecalc { get { return this.isRecalcField; } set { this.isRecalcField = value; } }
    public double? MaxRampWeight { get { return this.maxRampWeightField; } set { this.maxRampWeightField = value; } }
    public string UnderloadFactor { get { return this.underloadFactorField; } set { this.underloadFactorField = value; } }
    public double? AvgISA { get { return this.avgISAField; } set { this.avgISAField = value; } }
    public string HWCorrection20KtsTime { get { return this.hWCorrection20KtsTimeField; } set { this.hWCorrection20KtsTimeField = value; } }
    public double? HWCorrection20KtsFuel { get { return this.hWCorrection20KtsFuelField; } set { this.hWCorrection20KtsFuelField = value; } }
    public double? Correction1TON { get { return this.correction1TONField; } set { this.correction1TONField = value; } }
    public double? Correction2TON { get { return this.correction2TONField; } set { this.correction2TONField = value; } }
    public string RcfHeader { get { return this.rcfHeaderField; } set { this.rcfHeaderField = value; } }
    public bool ALT2AsInfoOnly { get { return this.aLT2AsInfoOnlyField; } set { this.aLT2AsInfoOnlyField = value; } }
    public string PpsApplicationVersion { get { return this.ppsApplicationVersionField; } set { this.ppsApplicationVersionField = value; } }
    public string PpsExeVersion { get { return this.ppsExeVersionField; } set { this.ppsExeVersionField = value; } }

    /// <remarks/>
    public string CustomReferences_RefID { get { return this.customReferences_RefID; } set { this.customReferences_RefID = value; } }
    public string CustomReferences_MilID { get { return this.customReferences_MilID; } set { this.customReferences_MilID = value; } }

    /// <remarks/>
    public string CFMUStatus { get { return this.cFMUStatusField; } set { this.cFMUStatusField = value; } }
    public double? StructuralTOM { get { return this.structuralTOMField; } set { this.structuralTOMField = value; } }
    public double? FW1 { get { return this.fW1Field; } set { this.fW1Field = value; } }
    public double? FW2 { get { return this.fW2Field; } set { this.fW2Field = value; } }
    public double? FW3 { get { return this.fW3Field; } set { this.fW3Field = value; } }
    public double? FW4 { get { return this.fW4Field; } set { this.fW4Field = value; } }
    public double? FW5 { get { return this.fW5Field; } set { this.fW5Field = value; } }
    public double? FW6 { get { return this.fW6Field; } set { this.fW6Field = value; } }
    public double? FW7 { get { return this.fW7Field; } set { this.fW7Field = value; } }
    public double? FW8 { get { return this.fW8Field; } set { this.fW8Field = value; } }
    public double? FW9 { get { return this.fW9Field; } set { this.fW9Field = value; } }
    public double? TOTALPAXWEIGHT { get { return this.tOTALPAXWEIGHTField; } set { this.tOTALPAXWEIGHTField = value; } }
    public double? Alt2Dist { get { return this.alt2DistField; } set { this.alt2DistField = value; } }
    public string FMSIdent { get { return this.fMSIdentField; } set { this.fMSIdentField = value; } }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("ExtraFuel", IsNullable = false)]
    public List<pps_ExtraFuel> ExtraFuels { get { return this.extraFuelsField; } set { this.extraFuelsField = value; } }

    /// <remarks/>
    public double? AircraftFuelBias { get { return this.aircraftFuelBiasField; } set { this.aircraftFuelBiasField = value; } }
    public double? MelFuelBias { get { return this.melFuelBiasField; } set { this.melFuelBiasField = value; } }

    [System.Xml.Serialization.XmlArrayItemAttribute("AirportWeatherData", IsNullable = false)]
    public List<pps_AirportWeatherData> PlanningEnRouteAlternateAirports { get { return this.planningEnRouteAlternateAirportsField; } set { this.planningEnRouteAlternateAirportsField = value; } }
}

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class pps_String
{

    private int idField;
    private string stringTypeField;
    private string txtField;

    /// <remarks/>
    public int Id { get { return this.idField; } set { this.idField = value; } }
    public string StringType { get { return this.stringTypeField; } set { this.stringTypeField = value; } }
    public string Txt { get { return this.txtField; } set { this.txtField = value; } }
  
}


[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class pps_FIROverflightCost
{

    private int idField;
    private string fIRField;

    private double? distanceField;

    private double? costField;

    /// <remarks/>
    public int Id { get { return this.idField; } set { this.idField = value; } }
    public string FIR { get { return this.fIRField; } set { this.fIRField = value; } }
    public double? Distance { get { return this.distanceField; } set { this.distanceField = value; } }
    public double? Cost { get { return this.costField; } set { this.costField = value; } }
 
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class pps_TAF
{
    private int idField;
    private string typeField;
    private string xmlTypeField;
    private string textField;
    private string iCAOField;
    private System.DateTime? forecastTimeField;
    private System.DateTime? forecastStartTimeField;
    private System.DateTime? forecastEndTimeField;

    public int Id { get => this.idField; set => this.idField = value; }
    public string Type { get => this.typeField; set => this.typeField = value; }
    public string XmlType { get => this.xmlTypeField; set => this.xmlTypeField = value; }
    public string Text { get => this.textField; set => this.textField = value; }
    public string ICAO { get => this.iCAOField; set => this.iCAOField = value; }
    public System.DateTime? ForecastTime { get => this.forecastTimeField; set => this.forecastTimeField = value; }
    public System.DateTime? ForecastStartTime { get => this.forecastStartTimeField; set => this.forecastStartTimeField = value; }
    public System.DateTime? ForecastEndTime { get => this.forecastEndTimeField; set => this.forecastEndTimeField = value; }
}

[System.Serializable]
[System.ComponentModel.DesignerCategory("code")]
[System.Xml.Serialization.XmlType(AnonymousType = true)]
public class pps_Notam
{
    // ---------- [New Field] ----------
    private int idField;
    private string notamTypeField; // فیلد جدید Type
    // فیلدهای پایه
    private string numberField;
    private string textField;
    private DateTime? fromDateField;
    private DateTime? toDateField;
    private double? fromLevelField;
    private double? toLevelField;
    private string firField;
    private string qCodeField;
    private string eCodeField;
    private string iCAOField;
    private string uniformAbbreviationField;
    private int yearField;
    private List<int> partsField;
    private int totalPartsField;
    private string routePartField;
    private string providerField;


    // پراپرتی‌های پایه
    public string Number { get => numberField; set => numberField = value; }
    public string Text { get => textField; set => textField = value; }
    public DateTime? FromDate { get => fromDateField; set => fromDateField = value; }
    public DateTime? ToDate { get => toDateField; set => toDateField = value; }
    public double? FromLevel { get => fromLevelField; set => fromLevelField = value; }
    public double? ToLevel { get => toLevelField; set => toLevelField = value; }
    public string Fir { get => firField; set => firField = value; }
    public string QCode { get => qCodeField; set => qCodeField = value; }
    public string ECode { get => eCodeField; set => eCodeField = value; }
    public string ICAO { get => iCAOField; set => iCAOField = value; }
    public string UniformAbbreviation { get => uniformAbbreviationField; set => uniformAbbreviationField = value; }
    public int Year { get => yearField; set => yearField = value; }

    // پراپرتی‌های Part به صورت آرایه
    [XmlArray("Parts")]
    [XmlArrayItem("Part")]
    public List<int> Parts { get => partsField; set => partsField = value; }

    public int TotalParts { get => totalPartsField; set => totalPartsField = value; }
    public string RoutePart { get => routePartField; set => routePartField = value; }
    public string Provider { get => providerField; set => providerField = value; }

    // ---------- [New Property] ----------
    public string Notam_Type {  get => notamTypeField; set => notamTypeField = value; }
    public int Id { get => idField; set => idField = value; }

}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class pps_RoutePoint
{

    private int idField;

    private string xmlIDField;

    private string routePointTypeField;

    private string iDENTField;

    private double? flField;

    private double? windField;

    private double? volField;

    private double? iSAField;

    private double? legTimeField;

    private double? legCourseField;

    private double? legDistanceField;

    private double? legCATField;

    private string legNameField;

    private string legAWYField;

    private double? fuelUsedField;

    private double? fuelFlowField;

    private double? lATField;

    private double? lONField;

    private double? vARIATIONField;

    private double? aCCDISTField;

    private double? aCCTIMEField;

    private double? magCourseField;

    private double? trueAirSpeedField;

    private double? groundSpeedField;

    private double? fuelRemainingField;

    private double? distRemainingField;

    private double? timeRemainingField;

    private double? minReqFuelField;

    private double? fuelFlowPerEngField;

    private double? temperatureField;

    private double? mORAField;

    private double? frequencyField;

    private double? windComponentField;

    private double? minimumEnrouteAltitudeField;

    //private RoutePointEco ecoField;
    // ---------- [Fields migrated from Eco] ----------
    private double? eco_OptSpeedFLField;
    private double? eco_SpeedGainField;
    private double? eco_OptEcoFLField;
    private double? eco_MoneyGainField;
    private double? eco_OptFuelFLField;
    private double? eco_FuelGainField;

    private double? magneticHeadingField;

    private double? trueHeadingField;

    private double? magneticTrackField;

    private double? trueTrackField;

    private string hLAEntryExitField;

    private string fIRField;

    private string climbDescentField;

    private double? legFuelField;

    /// <remarks/>
    public int Id  {  get  {  return this.idField; }  set   {  this.idField = value;     } }
    public string XmlID  {  get  {  return this.xmlIDField; }  set   {  this.xmlIDField = value;     } }
    public string RoutePointType { get { return this.routePointTypeField; } set { this.routePointTypeField = value; } }
    public string IDENT { get { return this.iDENTField; } set { this.iDENTField = value; } }
    public double? FL { get { return this.flField; } set { this.flField = value; } }
    public double? Wind { get { return this.windField; } set { this.windField = value; } }
    public double? Vol { get { return this.volField; } set { this.volField = value; } }
    public double? ISA { get { return this.iSAField; } set { this.iSAField = value; } }
    public double? LegTime { get { return this.legTimeField; } set { this.legTimeField = value; } }
    public double? LegCourse { get { return this.legCourseField; } set { this.legCourseField = value; } }
    public double? LegDistance { get { return this.legDistanceField; } set { this.legDistanceField = value; } }
    public double? LegCAT { get { return this.legCATField; } set { this.legCATField = value; } }
    public string LegName { get { return this.legNameField; } set { this.legNameField = value; } }
    public string LegAWY { get { return this.legAWYField; } set { this.legAWYField = value; } }
    public double? FuelUsed { get { return this.fuelUsedField; } set { this.fuelUsedField = value; } }
    public double? FuelFlow { get { return this.fuelFlowField; } set { this.fuelFlowField = value; } }
    public double? LAT { get { return this.lATField; } set { this.lATField = value; } }
    public double? LON { get { return this.lONField; } set { this.lONField = value; } }
    public double? VARIATION { get { return this.vARIATIONField; } set { this.vARIATIONField = value; } }
    public double? ACCDIST { get { return this.aCCDISTField; } set { this.aCCDISTField = value; } }
    public double? ACCTIME { get { return this.aCCTIMEField; } set { this.aCCTIMEField = value; } }
    public double? MagCourse { get { return this.magCourseField; } set { this.magCourseField = value; } }
    public double? TrueAirSpeed { get { return this.trueAirSpeedField; } set { this.trueAirSpeedField = value; } }
    public double? GroundSpeed { get { return this.groundSpeedField; } set { this.groundSpeedField = value; } }
    public double? FuelRemaining { get { return this.fuelRemainingField; } set { this.fuelRemainingField = value; } }
    public double? DistRemaining { get { return this.distRemainingField; } set { this.distRemainingField = value; } }
    public double? TimeRemaining { get { return this.timeRemainingField; } set { this.timeRemainingField = value; } }
    public double? MinReqFuel { get { return this.minReqFuelField; } set { this.minReqFuelField = value; } }
    public double? FuelFlowPerEng { get { return this.fuelFlowPerEngField; } set { this.fuelFlowPerEngField = value; } }
    public double? Temperature { get { return this.temperatureField; } set { this.temperatureField = value; } }
    public double? MORA { get { return this.mORAField; } set { this.mORAField = value; } }
    public double? Frequency { get { return this.frequencyField; } set { this.frequencyField = value; } }
    public double? WindComponent { get { return this.windComponentField; } set { this.windComponentField = value; } }
    public double? MinimumEnrouteAltitude { get { return this.minimumEnrouteAltitudeField; } set { this.minimumEnrouteAltitudeField = value; } }
   // public RoutePointEco Eco { get { return this.ecoField; } set { this.ecoField = value; } }
    public double? MagneticHeading { get { return this.magneticHeadingField; } set { this.magneticHeadingField = value; } }
    public double? TrueHeading { get { return this.trueHeadingField; } set { this.trueHeadingField = value; } }
    public double? MagneticTrack { get { return this.magneticTrackField; } set { this.magneticTrackField = value; } }
    public double? TrueTrack { get { return this.trueTrackField; } set { this.trueTrackField = value; } }
    public string HLAEntryExit { get { return this.hLAEntryExitField; } set { this.hLAEntryExitField = value; } }
    public string FIR { get { return this.fIRField; } set { this.fIRField = value; } }
    public string ClimbDescent { get { return this.climbDescentField; } set { this.climbDescentField = value; } }
    public double? LegFuel { get { return this.legFuelField; } set { this.legFuelField = value; } }
    // ---------- [Properties migrated from Eco] ----------
    public double? Eco_OptSpeedFL { get => this.eco_OptSpeedFLField; set => this.eco_OptSpeedFLField = value; }
    public double? Eco_SpeedGain { get => this.eco_SpeedGainField; set => this.eco_SpeedGainField = value; }
    public double? Eco_OptEcoFL { get => this.eco_OptEcoFLField; set => this.eco_OptEcoFLField = value; }
    public double? Eco_MoneyGain { get => this.eco_MoneyGainField; set => this.eco_MoneyGainField = value; }
    public double? Eco_OptFuelFL { get => this.eco_OptFuelFLField; set => this.eco_OptFuelFLField = value; }
    public double? Eco_FuelGain { get => this.eco_FuelGainField; set => this.eco_FuelGainField = value; }


}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class pps_Crew
{
    private int idField;

    private string xmlIDField;

    private string xmlTypeField;

    private string crewTypeField;

    private string crewNameField;

    private string initialsField;

    private string gSMField;

    private string massField;

    /// <remarks/>
    public int Id { get => idField; set => idField = value; }
    public string XmlID { get => xmlIDField; set => xmlIDField = value; }
    public string XmlType { get => xmlTypeField; set => xmlTypeField = value; }
    public string CrewType { get => crewTypeField; set => crewTypeField = value; }
    public string CrewName { get => crewNameField; set => crewNameField = value; }
    public string Initials { get => initialsField; set => initialsField = value; }
    public string GSM { get => gSMField; set => gSMField = value; }

    [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
    public string Mass { get => massField; set => massField = value; }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class pps_FlightLevel
{
    private int idField;

    private double? levelField;

    private double? costField;

    private double? wcField;

    private double? timeNCruiseField;

    private double? fuelNCruiseField;

    private double? timeProfile2Field;

    private double? fuelProfile2Field;

    private double? timeProfile3Field;

    private double? fuelProfile3Field;

    private double? fuelLowerField;

    private double? costDiffField;

    /// <remarks/>
    public int Id { get => idField; set => idField = value; }
    public double? Level { get => levelField; set => levelField = value; }
    public double? Cost { get => costField; set => costField = value; }
    public double? WC { get => wcField; set => wcField = value; }
    public double? TimeNCruise { get => timeNCruiseField; set => timeNCruiseField = value; }
    public double? FuelNCruise { get => fuelNCruiseField; set => fuelNCruiseField = value; }
    public double? TimeProfile2 { get => timeProfile2Field; set => timeProfile2Field = value; }
    public double? FuelProfile2 { get => fuelProfile2Field; set => fuelProfile2Field = value; }
    public double? TimeProfile3 { get => timeProfile3Field; set => timeProfile3Field = value; }
    public double? FuelProfile3 { get => fuelProfile3Field; set => fuelProfile3Field = value; }
    public double? FuelLower { get => fuelLowerField; set => fuelLowerField = value; }
    public double? CostDiff { get => costDiffField; set => costDiffField = value; }

}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class pps_AltAirport
{
    private int idField;
    
    private string typeField;

    private string icaoField;

    private double? distField;

    private double? timeField;

    private double? fuelField;

    private double? mAGCURSField;

    private string aTCField;

    private double? latField;

    private double? longField;

    private double? rwylField;

    private double? elevationField;

    private string nameField;

    private string iataField;

    private string categoryField;

    private string frequenciesField;

    private string frequencies2Field;

    /// <remarks/>
    public int Id { get => idField; set => idField = value; }
    public string Type { get => typeField; set => typeField = value; }
    public string Icao { get => icaoField; set => icaoField = value; }
    public double? Dist { get => distField; set => distField = value; }
    public double? Time { get => timeField; set => timeField = value; }
    public double? Fuel { get => fuelField; set => fuelField = value; }
    public double? MAGCURS { get => mAGCURSField; set => mAGCURSField = value; }
    public string ATC { get => aTCField; set => aTCField = value; }
    public double? Lat { get => latField; set => latField = value; }
    public double? Long { get => longField; set => longField = value; }
    public double? Rwyl { get => rwylField; set => rwylField = value; }
    public double? Elevation { get => elevationField; set => elevationField = value; }
    public string Name { get => nameField; set => nameField = value; }
    public string Iata { get => iataField; set => iataField = value; }
    public string Category { get => categoryField; set => categoryField = value; }
    public string Frequencies { get => frequenciesField; set => frequenciesField = value; }
    public string Frequencies2 { get => frequencies2Field; set => frequencies2Field = value; }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class pps_CorrectionTable
{
    private int idField;

    private string ctIDField;

    private double? flightlevelField;

    private double? windCorrectionField;

    private double? timeInMinutesForCruiseProfileField;

    private double? timeInHoursMinutesForCruiseProfileField;

    private string timeInHoursMinutesForAltCruiseProfileField;

    private string timeInMinutesForAltCruiseProfileField;

    private double? timeInMinutesForXProfileField;

    private string timeInHoursMinutesForXProfileField;

    private double? fuelForSelectedProfileField;

    private double? fuelForSecondProfileField;

    private double? fuelForXProfileField;

    private double? differentialCostField;

    private double? totalFuelIncreaseWith10ktWindField;

    public int Id { get => idField; set => idField = value; }
    public string CtID { get { return this.ctIDField; } set { this.ctIDField = value; } }
    public double? Flightlevel { get { return this.flightlevelField; } set { this.flightlevelField = value; } }
    public double? WindCorrection { get { return this.windCorrectionField; } set { this.windCorrectionField = value; } }
    public double? TimeInMinutesForCruiseProfile { get { return this.timeInMinutesForCruiseProfileField; } set { this.timeInMinutesForCruiseProfileField = value; } }
    public double? TimeInHoursMinutesForCruiseProfile { get { return this.timeInHoursMinutesForCruiseProfileField; } set { this.timeInHoursMinutesForCruiseProfileField = value; } }
    public string TimeInHoursMinutesForAltCruiseProfile { get { return this.timeInHoursMinutesForAltCruiseProfileField; } set { this.timeInHoursMinutesForAltCruiseProfileField = value; } }
    public string TimeInMinutesForAltCruiseProfile { get { return this.timeInMinutesForAltCruiseProfileField; } set { this.timeInMinutesForAltCruiseProfileField = value; } }
    public double? TimeInMinutesForXProfile { get { return this.timeInMinutesForXProfileField; } set { this.timeInMinutesForXProfileField = value; } }
    public string TimeInHoursMinutesForXProfile { get { return this.timeInHoursMinutesForXProfileField; } set { this.timeInHoursMinutesForXProfileField = value; } }
    public double? FuelForSelectedProfile { get { return this.fuelForSelectedProfileField; } set { this.fuelForSelectedProfileField = value; } }
    public double? FuelForSecondProfile { get { return this.fuelForSecondProfileField; } set { this.fuelForSecondProfileField = value; } }
    public double? FuelForXProfile { get { return this.fuelForXProfileField; } set { this.fuelForXProfileField = value; } }
    public double? DifferentialCost { get { return this.differentialCostField; } set { this.differentialCostField = value; } }
    public double? TotalFuelIncreaseWith10ktWind { get { return this.totalFuelIncreaseWith10ktWindField; } set { this.totalFuelIncreaseWith10ktWindField = value; } }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class pps_PaxSection
{
    private int idField;

    private string paxSectionTypeField;

    private string rowField;

    private double? actPaxField;

    private double? actMassField;

    private double? maleField;

    private double? femaleField;

    private double? childrenField;

    private double? infantField;

    private string cutsMassField;

    /// <remarks/>
    public int Id { get => idField; set => idField = value; }
    public string PaxSectionType { get { return this.paxSectionTypeField; } set { this.paxSectionTypeField = value; } }
    public string Row { get { return this.rowField; } set { this.rowField = value; } }
    public double? ActPax { get { return this.actPaxField; } set { this.actPaxField = value; } }
    public double? ActMass { get { return this.actMassField; } set { this.actMassField = value; } }
    public double? Male { get { return this.maleField; } set { this.maleField = value; } }
    public double? Female { get { return this.femaleField; } set { this.femaleField = value; } }
    public double? Children { get { return this.childrenField; } set { this.childrenField = value; } }
    public double? Infant { get { return this.infantField; } set { this.infantField = value; } }
    public string CutsMass { get { return this.cutsMassField; } set { this.cutsMassField = value; } }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class pps_ExtraFuel
{
    private int idField;

    private string typeField;

    private double? fuelField;

    private string timeField;

    /// <remarks/>
    public int Id { get => idField; set => idField = value; }
    public string Type { get { return this.typeField; } set { this.typeField = value; } }
    public double? Fuel { get { return this.fuelField; } set { this.fuelField = value; } }
    public string Time { get { return this.timeField; } set { this.timeField = value; } }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class pps_AirportWeatherData
{
    private int idField;
    private string iCAOField;
    private string taf_TypeField;
    private string taf_TextField;
    private string taf_ICAOField;
    private DateTime? taf_ForecastTimeField;
    private DateTime? taf_ForecastStartTimeField;
    private DateTime? taf_ForecastEndTimeField;
    private string metar_TextField;
    private string metar_ICAOField;
    private DateTime? metar_ObservationTimeField;
    private string metar_ObservationTypeField;


    /// <remarks/>
    public int Id { get => idField; set => idField = value; }
    public string ICAO { get { return this.iCAOField; } set { this.iCAOField = value; } }
    public string Taf_Type { get { return this.taf_TypeField; } set { this.taf_TypeField = value; } }
    public string Taf_Text { get { return this.taf_TextField; } set { this.taf_TextField = value; } }
    public string Taf_ICAO { get { return this.taf_ICAOField; } set { this.taf_ICAOField = value; } }
    public DateTime? Taf_ForecastTime { get { return this.taf_ForecastTimeField; } set { this.taf_ForecastTimeField = value; } }
    public DateTime? Taf_ForecastStartTime { get { return this.taf_ForecastStartTimeField; } set { this.taf_ForecastStartTimeField = value; } }
    public DateTime? Taf_ForecastEndTime { get { return this.taf_ForecastEndTimeField; } set { this.taf_ForecastEndTimeField = value; } }
    public string Metar_Text { get { return this.metar_TextField; } set { this.metar_TextField = value; } }
    public string Metar_ICAO { get { return this.metar_ICAOField; } set { this.metar_ICAOField = value; } }
    public DateTime? Metar_ObservationTime { get { return this.metar_ObservationTimeField; } set { this.metar_ObservationTimeField = value; } }
    public string Metar_ObservationType { get { return this.metar_ObservationTypeField; } set { this.metar_ObservationTypeField = value; } }

}

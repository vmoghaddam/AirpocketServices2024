
// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class Flight
{

    private object messagesField;

    private FlightOverflightCost overflightCostField;

    private string flightLogIDField;

    private uint idField;

    private string pPSNameField;

    private string aCFTAILField;

    private string dEPField;

    private string dESTField;

    private string aLT1Field;

    private string aLT2Field;

    private System.DateTime sTDField;

    private byte pAXField;

    private ushort fUELField;

    private byte lOADField;

    private byte validHrsField;

    private ushort minFLField;

    private ushort maxFLField;

    private object eROPSAltAptsField;

    private string[] adequateAptField;

    private string[] fIRField;

    private string[] altAptsField;

    private string tOAField;

    private object fMDIDField;

    private object dESTSTDALTField;

    private byte fUELCOMPField;

    private string tIMECOMPField;

    private ushort fUELCONTField;

    private string tIMECONTField;

    private byte pCTCONTField;

    private ushort fUELMINField;

    private string tIMEMINField;

    private ushort fUELTAXIField;

    private byte tIMETAXIField;

    private ushort fUELEXTRAField;

    private string tIMEEXTRAField;

    private ushort fUELLDGField;

    private object tIMELDGField;

    private uint zFMField;

    private ushort gCDField;

    private ushort eSADField;

    private string glField;

    private decimal fUELBIASField;

    private System.DateTime sTAField;

    private System.DateTime eTAField;

    private FlightLocalTime localTimeField;

    private int sCHBLOCKTIMEField;

    private string dISPField;

    private System.DateTime lastEditDateField;

    private System.DateTime latestFlightPlanDateField;

    private System.DateTime latestDocumentUploadDateField;

    private ushort fUELMINTOField;

    private byte tIMEMINTOField;

    private decimal aRAMPField;

    private ushort tIMEACTField;

    private ushort fUELACTField;

    private object destERAField;

    private ushort trafficLoadField;

    private string weightUnitField;

    private sbyte windComponentField;

    private object customerDataPPSField;

    private object customerDataScheduledField;

    private ushort flField;

    private byte routeDistNMField;

    private string routeNameField;

    private string routeRemarkField;

    private uint emptyWeightField;

    private ushort totalDistanceField;

    private byte altDistField;

    private byte destTimeField;

    private byte altTimeField;

    private ushort altFuelField;

    private byte holdTimeField;

    private byte reserveTimeField;

    private ushort cargoField;

    private decimal actTOWField;

    private ushort tripFuelField;

    private ushort holdFuelField;

    private FlightHolding holdingField;

    private uint elwField;

    private string fuelPolicyField;

    private byte alt2TimeField;

    private ushort alt2FuelField;

    private decimal maxTOMField;

    private decimal maxLMField;

    private decimal maxZFMField;

    private System.DateTime weatherObsTimeField;

    private System.DateTime weatherPlanTimeField;

    private string mFCIField;

    private string cruiseProfileField;

    private byte tempTopOfClimbField;

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

    private object amexsyStatusField;

    private ushort avgTrackField;

    private FlightDEPTAF dEPTAFField;

    private string dEPMetarField;

    private FlightNotam[] dEPNotamField;

    private FlightDESTTAF dESTTAFField;

    private string dESTMetarField;

    private FlightALT1TAF aLT1TAFField;

    private FlightALT2TAF aLT2TAFField;

    private string aLT1MetarField;

    private string aLT2MetarField;

    private FlightALT1Notam aLT1NotamField;

    private FlightRoutePoint[] routePointsField;

    private FlightCrews crewsField;

    private FlightResponce responceField;

    private FlightATCData aTCDataField;

    private FlightFlightLevel[] optFlightLevelsField;

    private FlightNotam1[] adequateNotamField;

    private FlightNotam2[] fIRNotamField;

    private FlightAltAirport[] airportsField;

    private object enrouteAlternatesField;

    private FlightRoutePoint1[] alt1PointsField;

    private FlightRoutePoint2[] alt2PointsField;

    private object stdAlternatesField;

    private object tOALTField;

    private FlightRouteStrings routeStringsField;

    private string dEPIATAField;

    private string dESTIATAField;

    private byte finalReserveMinutesField;

    private ushort finalReserveFuelField;

    private byte addFuelMinutesField;

    private byte addFuelField;

    private string flightSummaryField;

    private object passThroughValuesField;

    private FlightEtopsInformation etopsInformationField;

    private byte fuelINCRBurnField;

    private FlightCorrectionTable[] correctionTableField;

    private object externalFlightIdField;

    private string gUFIField;

    private FlightSidAndStarProcedures sidAndStarProceduresField;

    private ushort fMRIField;

    private FlightLoad loadField;

    private FlightAircraftConfiguration aircraftConfigurationField;

    private bool isRecalcField;

    private decimal maxRampWeightField;

    private string underloadFactorField;

    private byte avgISAField;

    private string hWCorrection20KtsTimeField;

    private decimal hWCorrection20KtsFuelField;

    private byte correction1TONField;

    private byte correction2TONField;

    private object rcfHeaderField;

    private bool aLT2AsInfoOnlyField;

    private FlightPpsVersionInformation ppsVersionInformationField;

    private FlightCustomReferences customReferencesField;

    private object cFMUStatusField;

    private decimal structuralTOMField;

    private ushort fW1Field;

    private byte fW2Field;

    private byte fW3Field;

    private byte fW4Field;

    private byte fW5Field;

    private byte fW6Field;

    private byte fW7Field;

    private byte fW8Field;

    private byte fW9Field;

    private byte tOTALPAXWEIGHTField;

    private byte alt2DistField;

    private object fMSIdentField;

    private FlightExtraFuel[] extraFuelsField;

    private decimal aircraftFuelBiasField;

    private decimal melFuelBiasField;

    private FlightAirportWeatherData[] planningEnRouteAlternateAirportsField;

    /// <remarks/>
    public object Messages
    {
        get
        {
            return this.messagesField;
        }
        set
        {
            this.messagesField = value;
        }
    }

    /// <remarks/>
    public FlightOverflightCost OverflightCost
    {
        get
        {
            return this.overflightCostField;
        }
        set
        {
            this.overflightCostField = value;
        }
    }

    /// <remarks/>
    public string FlightLogID
    {
        get
        {
            return this.flightLogIDField;
        }
        set
        {
            this.flightLogIDField = value;
        }
    }

    /// <remarks/>
    public uint ID
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }

    /// <remarks/>
    public string PPSName
    {
        get
        {
            return this.pPSNameField;
        }
        set
        {
            this.pPSNameField = value;
        }
    }

    /// <remarks/>
    public string ACFTAIL
    {
        get
        {
            return this.aCFTAILField;
        }
        set
        {
            this.aCFTAILField = value;
        }
    }

    /// <remarks/>
    public string DEP
    {
        get
        {
            return this.dEPField;
        }
        set
        {
            this.dEPField = value;
        }
    }

    /// <remarks/>
    public string DEST
    {
        get
        {
            return this.dESTField;
        }
        set
        {
            this.dESTField = value;
        }
    }

    /// <remarks/>
    public string ALT1
    {
        get
        {
            return this.aLT1Field;
        }
        set
        {
            this.aLT1Field = value;
        }
    }

    /// <remarks/>
    public string ALT2
    {
        get
        {
            return this.aLT2Field;
        }
        set
        {
            this.aLT2Field = value;
        }
    }

    /// <remarks/>
    public System.DateTime STD
    {
        get
        {
            return this.sTDField;
        }
        set
        {
            this.sTDField = value;
        }
    }

    /// <remarks/>
    public byte PAX
    {
        get
        {
            return this.pAXField;
        }
        set
        {
            this.pAXField = value;
        }
    }

    /// <remarks/>
    public ushort FUEL
    {
        get
        {
            return this.fUELField;
        }
        set
        {
            this.fUELField = value;
        }
    }

    /// <remarks/>
    public byte LOAD
    {
        get
        {
            return this.lOADField;
        }
        set
        {
            this.lOADField = value;
        }
    }

    /// <remarks/>
    public byte ValidHrs
    {
        get
        {
            return this.validHrsField;
        }
        set
        {
            this.validHrsField = value;
        }
    }

    /// <remarks/>
    public ushort MinFL
    {
        get
        {
            return this.minFLField;
        }
        set
        {
            this.minFLField = value;
        }
    }

    /// <remarks/>
    public ushort MaxFL
    {
        get
        {
            return this.maxFLField;
        }
        set
        {
            this.maxFLField = value;
        }
    }

    /// <remarks/>
    public object EROPSAltApts
    {
        get
        {
            return this.eROPSAltAptsField;
        }
        set
        {
            this.eROPSAltAptsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
    public string[] AdequateApt
    {
        get
        {
            return this.adequateAptField;
        }
        set
        {
            this.adequateAptField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
    public string[] FIR
    {
        get
        {
            return this.fIRField;
        }
        set
        {
            this.fIRField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
    public string[] AltApts
    {
        get
        {
            return this.altAptsField;
        }
        set
        {
            this.altAptsField = value;
        }
    }

    /// <remarks/>
    public string TOA
    {
        get
        {
            return this.tOAField;
        }
        set
        {
            this.tOAField = value;
        }
    }

    /// <remarks/>
    public object FMDID
    {
        get
        {
            return this.fMDIDField;
        }
        set
        {
            this.fMDIDField = value;
        }
    }

    /// <remarks/>
    public object DESTSTDALT
    {
        get
        {
            return this.dESTSTDALTField;
        }
        set
        {
            this.dESTSTDALTField = value;
        }
    }

    /// <remarks/>
    public byte FUELCOMP
    {
        get
        {
            return this.fUELCOMPField;
        }
        set
        {
            this.fUELCOMPField = value;
        }
    }

    /// <remarks/>
    public string TIMECOMP
    {
        get
        {
            return this.tIMECOMPField;
        }
        set
        {
            this.tIMECOMPField = value;
        }
    }

    /// <remarks/>
    public ushort FUELCONT
    {
        get
        {
            return this.fUELCONTField;
        }
        set
        {
            this.fUELCONTField = value;
        }
    }

    /// <remarks/>
    public string TIMECONT
    {
        get
        {
            return this.tIMECONTField;
        }
        set
        {
            this.tIMECONTField = value;
        }
    }

    /// <remarks/>
    public byte PCTCONT
    {
        get
        {
            return this.pCTCONTField;
        }
        set
        {
            this.pCTCONTField = value;
        }
    }

    /// <remarks/>
    public ushort FUELMIN
    {
        get
        {
            return this.fUELMINField;
        }
        set
        {
            this.fUELMINField = value;
        }
    }

    /// <remarks/>
    public string TIMEMIN
    {
        get
        {
            return this.tIMEMINField;
        }
        set
        {
            this.tIMEMINField = value;
        }
    }

    /// <remarks/>
    public ushort FUELTAXI
    {
        get
        {
            return this.fUELTAXIField;
        }
        set
        {
            this.fUELTAXIField = value;
        }
    }

    /// <remarks/>
    public byte TIMETAXI
    {
        get
        {
            return this.tIMETAXIField;
        }
        set
        {
            this.tIMETAXIField = value;
        }
    }

    /// <remarks/>
    public ushort FUELEXTRA
    {
        get
        {
            return this.fUELEXTRAField;
        }
        set
        {
            this.fUELEXTRAField = value;
        }
    }

    /// <remarks/>
    public string TIMEEXTRA
    {
        get
        {
            return this.tIMEEXTRAField;
        }
        set
        {
            this.tIMEEXTRAField = value;
        }
    }

    /// <remarks/>
    public ushort FUELLDG
    {
        get
        {
            return this.fUELLDGField;
        }
        set
        {
            this.fUELLDGField = value;
        }
    }

    /// <remarks/>
    public object TIMELDG
    {
        get
        {
            return this.tIMELDGField;
        }
        set
        {
            this.tIMELDGField = value;
        }
    }

    /// <remarks/>
    public uint ZFM
    {
        get
        {
            return this.zFMField;
        }
        set
        {
            this.zFMField = value;
        }
    }

    /// <remarks/>
    public ushort GCD
    {
        get
        {
            return this.gCDField;
        }
        set
        {
            this.gCDField = value;
        }
    }

    /// <remarks/>
    public ushort ESAD
    {
        get
        {
            return this.eSADField;
        }
        set
        {
            this.eSADField = value;
        }
    }

    /// <remarks/>
    public string GL
    {
        get
        {
            return this.glField;
        }
        set
        {
            this.glField = value;
        }
    }

    /// <remarks/>
    public decimal FUELBIAS
    {
        get
        {
            return this.fUELBIASField;
        }
        set
        {
            this.fUELBIASField = value;
        }
    }

    /// <remarks/>
    public System.DateTime STA
    {
        get
        {
            return this.sTAField;
        }
        set
        {
            this.sTAField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ETA
    {
        get
        {
            return this.eTAField;
        }
        set
        {
            this.eTAField = value;
        }
    }

    /// <remarks/>
    public FlightLocalTime LocalTime
    {
        get
        {
            return this.localTimeField;
        }
        set
        {
            this.localTimeField = value;
        }
    }

    /// <remarks/>
    public int SCHBLOCKTIME
    {
        get
        {
            return this.sCHBLOCKTIMEField;
        }
        set
        {
            this.sCHBLOCKTIMEField = value;
        }
    }

    /// <remarks/>
    public string DISP
    {
        get
        {
            return this.dISPField;
        }
        set
        {
            this.dISPField = value;
        }
    }

    /// <remarks/>
    public System.DateTime LastEditDate
    {
        get
        {
            return this.lastEditDateField;
        }
        set
        {
            this.lastEditDateField = value;
        }
    }

    /// <remarks/>
    public System.DateTime LatestFlightPlanDate
    {
        get
        {
            return this.latestFlightPlanDateField;
        }
        set
        {
            this.latestFlightPlanDateField = value;
        }
    }

    /// <remarks/>
    public System.DateTime LatestDocumentUploadDate
    {
        get
        {
            return this.latestDocumentUploadDateField;
        }
        set
        {
            this.latestDocumentUploadDateField = value;
        }
    }

    /// <remarks/>
    public ushort FUELMINTO
    {
        get
        {
            return this.fUELMINTOField;
        }
        set
        {
            this.fUELMINTOField = value;
        }
    }

    /// <remarks/>
    public byte TIMEMINTO
    {
        get
        {
            return this.tIMEMINTOField;
        }
        set
        {
            this.tIMEMINTOField = value;
        }
    }

    /// <remarks/>
    public decimal ARAMP
    {
        get
        {
            return this.aRAMPField;
        }
        set
        {
            this.aRAMPField = value;
        }
    }

    /// <remarks/>
    public ushort TIMEACT
    {
        get
        {
            return this.tIMEACTField;
        }
        set
        {
            this.tIMEACTField = value;
        }
    }

    /// <remarks/>
    public ushort FUELACT
    {
        get
        {
            return this.fUELACTField;
        }
        set
        {
            this.fUELACTField = value;
        }
    }

    /// <remarks/>
    public object DestERA
    {
        get
        {
            return this.destERAField;
        }
        set
        {
            this.destERAField = value;
        }
    }

    /// <remarks/>
    public ushort TrafficLoad
    {
        get
        {
            return this.trafficLoadField;
        }
        set
        {
            this.trafficLoadField = value;
        }
    }

    /// <remarks/>
    public string WeightUnit
    {
        get
        {
            return this.weightUnitField;
        }
        set
        {
            this.weightUnitField = value;
        }
    }

    /// <remarks/>
    public sbyte WindComponent
    {
        get
        {
            return this.windComponentField;
        }
        set
        {
            this.windComponentField = value;
        }
    }

    /// <remarks/>
    public object CustomerDataPPS
    {
        get
        {
            return this.customerDataPPSField;
        }
        set
        {
            this.customerDataPPSField = value;
        }
    }

    /// <remarks/>
    public object CustomerDataScheduled
    {
        get
        {
            return this.customerDataScheduledField;
        }
        set
        {
            this.customerDataScheduledField = value;
        }
    }

    /// <remarks/>
    public ushort Fl
    {
        get
        {
            return this.flField;
        }
        set
        {
            this.flField = value;
        }
    }

    /// <remarks/>
    public byte RouteDistNM
    {
        get
        {
            return this.routeDistNMField;
        }
        set
        {
            this.routeDistNMField = value;
        }
    }

    /// <remarks/>
    public string RouteName
    {
        get
        {
            return this.routeNameField;
        }
        set
        {
            this.routeNameField = value;
        }
    }

    /// <remarks/>
    public string RouteRemark
    {
        get
        {
            return this.routeRemarkField;
        }
        set
        {
            this.routeRemarkField = value;
        }
    }

    /// <remarks/>
    public uint EmptyWeight
    {
        get
        {
            return this.emptyWeightField;
        }
        set
        {
            this.emptyWeightField = value;
        }
    }

    /// <remarks/>
    public ushort TotalDistance
    {
        get
        {
            return this.totalDistanceField;
        }
        set
        {
            this.totalDistanceField = value;
        }
    }

    /// <remarks/>
    public byte AltDist
    {
        get
        {
            return this.altDistField;
        }
        set
        {
            this.altDistField = value;
        }
    }

    /// <remarks/>
    public byte DestTime
    {
        get
        {
            return this.destTimeField;
        }
        set
        {
            this.destTimeField = value;
        }
    }

    /// <remarks/>
    public byte AltTime
    {
        get
        {
            return this.altTimeField;
        }
        set
        {
            this.altTimeField = value;
        }
    }

    /// <remarks/>
    public ushort AltFuel
    {
        get
        {
            return this.altFuelField;
        }
        set
        {
            this.altFuelField = value;
        }
    }

    /// <remarks/>
    public byte HoldTime
    {
        get
        {
            return this.holdTimeField;
        }
        set
        {
            this.holdTimeField = value;
        }
    }

    /// <remarks/>
    public byte ReserveTime
    {
        get
        {
            return this.reserveTimeField;
        }
        set
        {
            this.reserveTimeField = value;
        }
    }

    /// <remarks/>
    public ushort Cargo
    {
        get
        {
            return this.cargoField;
        }
        set
        {
            this.cargoField = value;
        }
    }

    /// <remarks/>
    public decimal ActTOW
    {
        get
        {
            return this.actTOWField;
        }
        set
        {
            this.actTOWField = value;
        }
    }

    /// <remarks/>
    public ushort TripFuel
    {
        get
        {
            return this.tripFuelField;
        }
        set
        {
            this.tripFuelField = value;
        }
    }

    /// <remarks/>
    public ushort HoldFuel
    {
        get
        {
            return this.holdFuelField;
        }
        set
        {
            this.holdFuelField = value;
        }
    }

    /// <remarks/>
    public FlightHolding Holding
    {
        get
        {
            return this.holdingField;
        }
        set
        {
            this.holdingField = value;
        }
    }

    /// <remarks/>
    public uint Elw
    {
        get
        {
            return this.elwField;
        }
        set
        {
            this.elwField = value;
        }
    }

    /// <remarks/>
    public string FuelPolicy
    {
        get
        {
            return this.fuelPolicyField;
        }
        set
        {
            this.fuelPolicyField = value;
        }
    }

    /// <remarks/>
    public byte Alt2Time
    {
        get
        {
            return this.alt2TimeField;
        }
        set
        {
            this.alt2TimeField = value;
        }
    }

    /// <remarks/>
    public ushort Alt2Fuel
    {
        get
        {
            return this.alt2FuelField;
        }
        set
        {
            this.alt2FuelField = value;
        }
    }

    /// <remarks/>
    public decimal MaxTOM
    {
        get
        {
            return this.maxTOMField;
        }
        set
        {
            this.maxTOMField = value;
        }
    }

    /// <remarks/>
    public decimal MaxLM
    {
        get
        {
            return this.maxLMField;
        }
        set
        {
            this.maxLMField = value;
        }
    }

    /// <remarks/>
    public decimal MaxZFM
    {
        get
        {
            return this.maxZFMField;
        }
        set
        {
            this.maxZFMField = value;
        }
    }

    /// <remarks/>
    public System.DateTime WeatherObsTime
    {
        get
        {
            return this.weatherObsTimeField;
        }
        set
        {
            this.weatherObsTimeField = value;
        }
    }

    /// <remarks/>
    public System.DateTime WeatherPlanTime
    {
        get
        {
            return this.weatherPlanTimeField;
        }
        set
        {
            this.weatherPlanTimeField = value;
        }
    }

    /// <remarks/>
    public string MFCI
    {
        get
        {
            return this.mFCIField;
        }
        set
        {
            this.mFCIField = value;
        }
    }

    /// <remarks/>
    public string CruiseProfile
    {
        get
        {
            return this.cruiseProfileField;
        }
        set
        {
            this.cruiseProfileField = value;
        }
    }

    /// <remarks/>
    public byte TempTopOfClimb
    {
        get
        {
            return this.tempTopOfClimbField;
        }
        set
        {
            this.tempTopOfClimbField = value;
        }
    }

    /// <remarks/>
    public string Climb
    {
        get
        {
            return this.climbField;
        }
        set
        {
            this.climbField = value;
        }
    }

    /// <remarks/>
    public string Descend
    {
        get
        {
            return this.descendField;
        }
        set
        {
            this.descendField = value;
        }
    }

    /// <remarks/>
    public string FuelPL
    {
        get
        {
            return this.fuelPLField;
        }
        set
        {
            this.fuelPLField = value;
        }
    }

    /// <remarks/>
    public string DescendWind
    {
        get
        {
            return this.descendWindField;
        }
        set
        {
            this.descendWindField = value;
        }
    }

    /// <remarks/>
    public string ClimbProfile
    {
        get
        {
            return this.climbProfileField;
        }
        set
        {
            this.climbProfileField = value;
        }
    }

    /// <remarks/>
    public string DescendProfile
    {
        get
        {
            return this.descendProfileField;
        }
        set
        {
            this.descendProfileField = value;
        }
    }

    /// <remarks/>
    public string HoldProfile
    {
        get
        {
            return this.holdProfileField;
        }
        set
        {
            this.holdProfileField = value;
        }
    }

    /// <remarks/>
    public string StepClimbProfile
    {
        get
        {
            return this.stepClimbProfileField;
        }
        set
        {
            this.stepClimbProfileField = value;
        }
    }

    /// <remarks/>
    public string FuelContDef
    {
        get
        {
            return this.fuelContDefField;
        }
        set
        {
            this.fuelContDefField = value;
        }
    }

    /// <remarks/>
    public string FuelAltDef
    {
        get
        {
            return this.fuelAltDefField;
        }
        set
        {
            this.fuelAltDefField = value;
        }
    }

    /// <remarks/>
    public object AmexsyStatus
    {
        get
        {
            return this.amexsyStatusField;
        }
        set
        {
            this.amexsyStatusField = value;
        }
    }

    /// <remarks/>
    public ushort AvgTrack
    {
        get
        {
            return this.avgTrackField;
        }
        set
        {
            this.avgTrackField = value;
        }
    }

    /// <remarks/>
    public FlightDEPTAF DEPTAF
    {
        get
        {
            return this.dEPTAFField;
        }
        set
        {
            this.dEPTAFField = value;
        }
    }

    /// <remarks/>
    public string DEPMetar
    {
        get
        {
            return this.dEPMetarField;
        }
        set
        {
            this.dEPMetarField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Notam", IsNullable = false)]
    public FlightNotam[] DEPNotam
    {
        get
        {
            return this.dEPNotamField;
        }
        set
        {
            this.dEPNotamField = value;
        }
    }

    /// <remarks/>
    public FlightDESTTAF DESTTAF
    {
        get
        {
            return this.dESTTAFField;
        }
        set
        {
            this.dESTTAFField = value;
        }
    }

    /// <remarks/>
    public string DESTMetar
    {
        get
        {
            return this.dESTMetarField;
        }
        set
        {
            this.dESTMetarField = value;
        }
    }

    /// <remarks/>
    public FlightALT1TAF ALT1TAF
    {
        get
        {
            return this.aLT1TAFField;
        }
        set
        {
            this.aLT1TAFField = value;
        }
    }

    /// <remarks/>
    public FlightALT2TAF ALT2TAF
    {
        get
        {
            return this.aLT2TAFField;
        }
        set
        {
            this.aLT2TAFField = value;
        }
    }

    /// <remarks/>
    public string ALT1Metar
    {
        get
        {
            return this.aLT1MetarField;
        }
        set
        {
            this.aLT1MetarField = value;
        }
    }

    /// <remarks/>
    public string ALT2Metar
    {
        get
        {
            return this.aLT2MetarField;
        }
        set
        {
            this.aLT2MetarField = value;
        }
    }

    /// <remarks/>
    public FlightALT1Notam ALT1Notam
    {
        get
        {
            return this.aLT1NotamField;
        }
        set
        {
            this.aLT1NotamField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("RoutePoint", IsNullable = false)]
    public FlightRoutePoint[] RoutePoints
    {
        get
        {
            return this.routePointsField;
        }
        set
        {
            this.routePointsField = value;
        }
    }

    /// <remarks/>
    public FlightCrews Crews
    {
        get
        {
            return this.crewsField;
        }
        set
        {
            this.crewsField = value;
        }
    }

    /// <remarks/>
    public FlightResponce Responce
    {
        get
        {
            return this.responceField;
        }
        set
        {
            this.responceField = value;
        }
    }

    /// <remarks/>
    public FlightATCData ATCData
    {
        get
        {
            return this.aTCDataField;
        }
        set
        {
            this.aTCDataField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("FlightLevel", IsNullable = false)]
    public FlightFlightLevel[] OptFlightLevels
    {
        get
        {
            return this.optFlightLevelsField;
        }
        set
        {
            this.optFlightLevelsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Notam", IsNullable = false)]
    public FlightNotam1[] AdequateNotam
    {
        get
        {
            return this.adequateNotamField;
        }
        set
        {
            this.adequateNotamField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Notam", IsNullable = false)]
    public FlightNotam2[] FIRNotam
    {
        get
        {
            return this.fIRNotamField;
        }
        set
        {
            this.fIRNotamField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("AltAirport", IsNullable = false)]
    public FlightAltAirport[] Airports
    {
        get
        {
            return this.airportsField;
        }
        set
        {
            this.airportsField = value;
        }
    }

    /// <remarks/>
    public object EnrouteAlternates
    {
        get
        {
            return this.enrouteAlternatesField;
        }
        set
        {
            this.enrouteAlternatesField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("RoutePoint", IsNullable = false)]
    public FlightRoutePoint1[] Alt1Points
    {
        get
        {
            return this.alt1PointsField;
        }
        set
        {
            this.alt1PointsField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("RoutePoint", IsNullable = false)]
    public FlightRoutePoint2[] Alt2Points
    {
        get
        {
            return this.alt2PointsField;
        }
        set
        {
            this.alt2PointsField = value;
        }
    }

    /// <remarks/>
    public object StdAlternates
    {
        get
        {
            return this.stdAlternatesField;
        }
        set
        {
            this.stdAlternatesField = value;
        }
    }

    /// <remarks/>
    public object TOALT
    {
        get
        {
            return this.tOALTField;
        }
        set
        {
            this.tOALTField = value;
        }
    }

    /// <remarks/>
    public FlightRouteStrings RouteStrings
    {
        get
        {
            return this.routeStringsField;
        }
        set
        {
            this.routeStringsField = value;
        }
    }

    /// <remarks/>
    public string DEPIATA
    {
        get
        {
            return this.dEPIATAField;
        }
        set
        {
            this.dEPIATAField = value;
        }
    }

    /// <remarks/>
    public string DESTIATA
    {
        get
        {
            return this.dESTIATAField;
        }
        set
        {
            this.dESTIATAField = value;
        }
    }

    /// <remarks/>
    public byte FinalReserveMinutes
    {
        get
        {
            return this.finalReserveMinutesField;
        }
        set
        {
            this.finalReserveMinutesField = value;
        }
    }

    /// <remarks/>
    public ushort FinalReserveFuel
    {
        get
        {
            return this.finalReserveFuelField;
        }
        set
        {
            this.finalReserveFuelField = value;
        }
    }

    /// <remarks/>
    public byte AddFuelMinutes
    {
        get
        {
            return this.addFuelMinutesField;
        }
        set
        {
            this.addFuelMinutesField = value;
        }
    }

    /// <remarks/>
    public byte AddFuel
    {
        get
        {
            return this.addFuelField;
        }
        set
        {
            this.addFuelField = value;
        }
    }

    /// <remarks/>
    public string FlightSummary
    {
        get
        {
            return this.flightSummaryField;
        }
        set
        {
            this.flightSummaryField = value;
        }
    }

    /// <remarks/>
    public object PassThroughValues
    {
        get
        {
            return this.passThroughValuesField;
        }
        set
        {
            this.passThroughValuesField = value;
        }
    }

    /// <remarks/>
    public FlightEtopsInformation EtopsInformation
    {
        get
        {
            return this.etopsInformationField;
        }
        set
        {
            this.etopsInformationField = value;
        }
    }

    /// <remarks/>
    public byte FuelINCRBurn
    {
        get
        {
            return this.fuelINCRBurnField;
        }
        set
        {
            this.fuelINCRBurnField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("CorrectionTable", IsNullable = false)]
    public FlightCorrectionTable[] CorrectionTable
    {
        get
        {
            return this.correctionTableField;
        }
        set
        {
            this.correctionTableField = value;
        }
    }

    /// <remarks/>
    public object ExternalFlightId
    {
        get
        {
            return this.externalFlightIdField;
        }
        set
        {
            this.externalFlightIdField = value;
        }
    }

    /// <remarks/>
    public string GUFI
    {
        get
        {
            return this.gUFIField;
        }
        set
        {
            this.gUFIField = value;
        }
    }

    /// <remarks/>
    public FlightSidAndStarProcedures SidAndStarProcedures
    {
        get
        {
            return this.sidAndStarProceduresField;
        }
        set
        {
            this.sidAndStarProceduresField = value;
        }
    }

    /// <remarks/>
    public ushort FMRI
    {
        get
        {
            return this.fMRIField;
        }
        set
        {
            this.fMRIField = value;
        }
    }

    /// <remarks/>
    public FlightLoad Load
    {
        get
        {
            return this.loadField;
        }
        set
        {
            this.loadField = value;
        }
    }

    /// <remarks/>
    public FlightAircraftConfiguration AircraftConfiguration
    {
        get
        {
            return this.aircraftConfigurationField;
        }
        set
        {
            this.aircraftConfigurationField = value;
        }
    }

    /// <remarks/>
    public bool IsRecalc
    {
        get
        {
            return this.isRecalcField;
        }
        set
        {
            this.isRecalcField = value;
        }
    }

    /// <remarks/>
    public decimal MaxRampWeight
    {
        get
        {
            return this.maxRampWeightField;
        }
        set
        {
            this.maxRampWeightField = value;
        }
    }

    /// <remarks/>
    public string UnderloadFactor
    {
        get
        {
            return this.underloadFactorField;
        }
        set
        {
            this.underloadFactorField = value;
        }
    }

    /// <remarks/>
    public byte AvgISA
    {
        get
        {
            return this.avgISAField;
        }
        set
        {
            this.avgISAField = value;
        }
    }

    /// <remarks/>
    public string HWCorrection20KtsTime
    {
        get
        {
            return this.hWCorrection20KtsTimeField;
        }
        set
        {
            this.hWCorrection20KtsTimeField = value;
        }
    }

    /// <remarks/>
    public decimal HWCorrection20KtsFuel
    {
        get
        {
            return this.hWCorrection20KtsFuelField;
        }
        set
        {
            this.hWCorrection20KtsFuelField = value;
        }
    }

    /// <remarks/>
    public byte Correction1TON
    {
        get
        {
            return this.correction1TONField;
        }
        set
        {
            this.correction1TONField = value;
        }
    }

    /// <remarks/>
    public byte Correction2TON
    {
        get
        {
            return this.correction2TONField;
        }
        set
        {
            this.correction2TONField = value;
        }
    }

    /// <remarks/>
    public object RcfHeader
    {
        get
        {
            return this.rcfHeaderField;
        }
        set
        {
            this.rcfHeaderField = value;
        }
    }

    /// <remarks/>
    public bool ALT2AsInfoOnly
    {
        get
        {
            return this.aLT2AsInfoOnlyField;
        }
        set
        {
            this.aLT2AsInfoOnlyField = value;
        }
    }

    /// <remarks/>
    public FlightPpsVersionInformation PpsVersionInformation
    {
        get
        {
            return this.ppsVersionInformationField;
        }
        set
        {
            this.ppsVersionInformationField = value;
        }
    }

    /// <remarks/>
    public FlightCustomReferences CustomReferences
    {
        get
        {
            return this.customReferencesField;
        }
        set
        {
            this.customReferencesField = value;
        }
    }

    /// <remarks/>
    public object CFMUStatus
    {
        get
        {
            return this.cFMUStatusField;
        }
        set
        {
            this.cFMUStatusField = value;
        }
    }

    /// <remarks/>
    public decimal StructuralTOM
    {
        get
        {
            return this.structuralTOMField;
        }
        set
        {
            this.structuralTOMField = value;
        }
    }

    /// <remarks/>
    public ushort FW1
    {
        get
        {
            return this.fW1Field;
        }
        set
        {
            this.fW1Field = value;
        }
    }

    /// <remarks/>
    public byte FW2
    {
        get
        {
            return this.fW2Field;
        }
        set
        {
            this.fW2Field = value;
        }
    }

    /// <remarks/>
    public byte FW3
    {
        get
        {
            return this.fW3Field;
        }
        set
        {
            this.fW3Field = value;
        }
    }

    /// <remarks/>
    public byte FW4
    {
        get
        {
            return this.fW4Field;
        }
        set
        {
            this.fW4Field = value;
        }
    }

    /// <remarks/>
    public byte FW5
    {
        get
        {
            return this.fW5Field;
        }
        set
        {
            this.fW5Field = value;
        }
    }

    /// <remarks/>
    public byte FW6
    {
        get
        {
            return this.fW6Field;
        }
        set
        {
            this.fW6Field = value;
        }
    }

    /// <remarks/>
    public byte FW7
    {
        get
        {
            return this.fW7Field;
        }
        set
        {
            this.fW7Field = value;
        }
    }

    /// <remarks/>
    public byte FW8
    {
        get
        {
            return this.fW8Field;
        }
        set
        {
            this.fW8Field = value;
        }
    }

    /// <remarks/>
    public byte FW9
    {
        get
        {
            return this.fW9Field;
        }
        set
        {
            this.fW9Field = value;
        }
    }

    /// <remarks/>
    public byte TOTALPAXWEIGHT
    {
        get
        {
            return this.tOTALPAXWEIGHTField;
        }
        set
        {
            this.tOTALPAXWEIGHTField = value;
        }
    }

    /// <remarks/>
    public byte Alt2Dist
    {
        get
        {
            return this.alt2DistField;
        }
        set
        {
            this.alt2DistField = value;
        }
    }

    /// <remarks/>
    public object FMSIdent
    {
        get
        {
            return this.fMSIdentField;
        }
        set
        {
            this.fMSIdentField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("ExtraFuel", IsNullable = false)]
    public FlightExtraFuel[] ExtraFuels
    {
        get
        {
            return this.extraFuelsField;
        }
        set
        {
            this.extraFuelsField = value;
        }
    }

    /// <remarks/>
    public decimal AircraftFuelBias
    {
        get
        {
            return this.aircraftFuelBiasField;
        }
        set
        {
            this.aircraftFuelBiasField = value;
        }
    }

    /// <remarks/>
    public decimal MelFuelBias
    {
        get
        {
            return this.melFuelBiasField;
        }
        set
        {
            this.melFuelBiasField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("AirportWeatherData", IsNullable = false)]
    public FlightAirportWeatherData[] PlanningEnRouteAlternateAirports
    {
        get
        {
            return this.planningEnRouteAlternateAirportsField;
        }
        set
        {
            this.planningEnRouteAlternateAirportsField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightOverflightCost
{

    private FlightOverflightCostFIROverflightCost[] costField;

    private string currencyField;

    private ushort totalOverflightCostField;

    private byte totalTerminalCostField;

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("FIROverflightCost", IsNullable = false)]
    public FlightOverflightCostFIROverflightCost[] Cost
    {
        get
        {
            return this.costField;
        }
        set
        {
            this.costField = value;
        }
    }

    /// <remarks/>
    public string Currency
    {
        get
        {
            return this.currencyField;
        }
        set
        {
            this.currencyField = value;
        }
    }

    /// <remarks/>
    public ushort TotalOverflightCost
    {
        get
        {
            return this.totalOverflightCostField;
        }
        set
        {
            this.totalOverflightCostField = value;
        }
    }

    /// <remarks/>
    public byte TotalTerminalCost
    {
        get
        {
            return this.totalTerminalCostField;
        }
        set
        {
            this.totalTerminalCostField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightOverflightCostFIROverflightCost
{

    private string fIRField;

    private ushort distanceField;

    private byte costField;

    /// <remarks/>
    public string FIR
    {
        get
        {
            return this.fIRField;
        }
        set
        {
            this.fIRField = value;
        }
    }

    /// <remarks/>
    public ushort Distance
    {
        get
        {
            return this.distanceField;
        }
        set
        {
            this.distanceField = value;
        }
    }

    /// <remarks/>
    public byte Cost
    {
        get
        {
            return this.costField;
        }
        set
        {
            this.costField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLocalTime
{

    private FlightLocalTimeDeparture departureField;

    private FlightLocalTimeDestination destinationField;

    /// <remarks/>
    public FlightLocalTimeDeparture Departure
    {
        get
        {
            return this.departureField;
        }
        set
        {
            this.departureField = value;
        }
    }

    /// <remarks/>
    public FlightLocalTimeDestination Destination
    {
        get
        {
            return this.destinationField;
        }
        set
        {
            this.destinationField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLocalTimeDeparture
{

    private System.DateTime sTDField;

    private System.DateTime eTDField;

    private System.DateTime sunriseField;

    private System.DateTime sunsetField;

    /// <remarks/>
    public System.DateTime STD
    {
        get
        {
            return this.sTDField;
        }
        set
        {
            this.sTDField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ETD
    {
        get
        {
            return this.eTDField;
        }
        set
        {
            this.eTDField = value;
        }
    }

    /// <remarks/>
    public System.DateTime Sunrise
    {
        get
        {
            return this.sunriseField;
        }
        set
        {
            this.sunriseField = value;
        }
    }

    /// <remarks/>
    public System.DateTime Sunset
    {
        get
        {
            return this.sunsetField;
        }
        set
        {
            this.sunsetField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLocalTimeDestination
{

    private System.DateTime sTAField;

    private System.DateTime eTAField;

    private System.DateTime sunriseField;

    private System.DateTime sunsetField;

    /// <remarks/>
    public System.DateTime STA
    {
        get
        {
            return this.sTAField;
        }
        set
        {
            this.sTAField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ETA
    {
        get
        {
            return this.eTAField;
        }
        set
        {
            this.eTAField = value;
        }
    }

    /// <remarks/>
    public System.DateTime Sunrise
    {
        get
        {
            return this.sunriseField;
        }
        set
        {
            this.sunriseField = value;
        }
    }

    /// <remarks/>
    public System.DateTime Sunset
    {
        get
        {
            return this.sunsetField;
        }
        set
        {
            this.sunsetField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightHolding
{

    private byte fuelField;

    private byte minutesField;

    private object profileField;

    private object specificationField;

    private string fuelFlowTypeField;

    /// <remarks/>
    public byte Fuel
    {
        get
        {
            return this.fuelField;
        }
        set
        {
            this.fuelField = value;
        }
    }

    /// <remarks/>
    public byte Minutes
    {
        get
        {
            return this.minutesField;
        }
        set
        {
            this.minutesField = value;
        }
    }

    /// <remarks/>
    public object Profile
    {
        get
        {
            return this.profileField;
        }
        set
        {
            this.profileField = value;
        }
    }

    /// <remarks/>
    public object Specification
    {
        get
        {
            return this.specificationField;
        }
        set
        {
            this.specificationField = value;
        }
    }

    /// <remarks/>
    public string FuelFlowType
    {
        get
        {
            return this.fuelFlowTypeField;
        }
        set
        {
            this.fuelFlowTypeField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightDEPTAF
{

    private string typeField;

    private string textField;

    private string iCAOField;

    private System.DateTime forecastTimeField;

    private System.DateTime forecastStartTimeField;

    private System.DateTime forecastEndTimeField;

    /// <remarks/>
    public string Type
    {
        get
        {
            return this.typeField;
        }
        set
        {
            this.typeField = value;
        }
    }

    /// <remarks/>
    public string Text
    {
        get
        {
            return this.textField;
        }
        set
        {
            this.textField = value;
        }
    }

    /// <remarks/>
    public string ICAO
    {
        get
        {
            return this.iCAOField;
        }
        set
        {
            this.iCAOField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ForecastTime
    {
        get
        {
            return this.forecastTimeField;
        }
        set
        {
            this.forecastTimeField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ForecastStartTime
    {
        get
        {
            return this.forecastStartTimeField;
        }
        set
        {
            this.forecastStartTimeField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ForecastEndTime
    {
        get
        {
            return this.forecastEndTimeField;
        }
        set
        {
            this.forecastEndTimeField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightNotam
{

    private string numberField;

    private string textField;

    private System.DateTime fromDateField;

    private System.DateTime toDateField;

    private byte fromLevelField;

    private ushort toLevelField;

    private string firField;

    private string qCodeField;

    private string eCodeField;

    private string iCAOField;

    private string uniformAbbreviationField;

    private byte yearField;

    private FlightNotamPartInformation partInformationField;

    private string routePartField;

    private string providerField;

    /// <remarks/>
    public string Number
    {
        get
        {
            return this.numberField;
        }
        set
        {
            this.numberField = value;
        }
    }

    /// <remarks/>
    public string Text
    {
        get
        {
            return this.textField;
        }
        set
        {
            this.textField = value;
        }
    }

    /// <remarks/>
    public System.DateTime FromDate
    {
        get
        {
            return this.fromDateField;
        }
        set
        {
            this.fromDateField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ToDate
    {
        get
        {
            return this.toDateField;
        }
        set
        {
            this.toDateField = value;
        }
    }

    /// <remarks/>
    public byte FromLevel
    {
        get
        {
            return this.fromLevelField;
        }
        set
        {
            this.fromLevelField = value;
        }
    }

    /// <remarks/>
    public ushort ToLevel
    {
        get
        {
            return this.toLevelField;
        }
        set
        {
            this.toLevelField = value;
        }
    }

    /// <remarks/>
    public string Fir
    {
        get
        {
            return this.firField;
        }
        set
        {
            this.firField = value;
        }
    }

    /// <remarks/>
    public string QCode
    {
        get
        {
            return this.qCodeField;
        }
        set
        {
            this.qCodeField = value;
        }
    }

    /// <remarks/>
    public string ECode
    {
        get
        {
            return this.eCodeField;
        }
        set
        {
            this.eCodeField = value;
        }
    }

    /// <remarks/>
    public string ICAO
    {
        get
        {
            return this.iCAOField;
        }
        set
        {
            this.iCAOField = value;
        }
    }

    /// <remarks/>
    public string UniformAbbreviation
    {
        get
        {
            return this.uniformAbbreviationField;
        }
        set
        {
            this.uniformAbbreviationField = value;
        }
    }

    /// <remarks/>
    public byte Year
    {
        get
        {
            return this.yearField;
        }
        set
        {
            this.yearField = value;
        }
    }

    /// <remarks/>
    public FlightNotamPartInformation PartInformation
    {
        get
        {
            return this.partInformationField;
        }
        set
        {
            this.partInformationField = value;
        }
    }

    /// <remarks/>
    public string RoutePart
    {
        get
        {
            return this.routePartField;
        }
        set
        {
            this.routePartField = value;
        }
    }

    /// <remarks/>
    public string Provider
    {
        get
        {
            return this.providerField;
        }
        set
        {
            this.providerField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightNotamPartInformation
{

    private byte partField;

    private byte totalPartsField;

    /// <remarks/>
    public byte Part
    {
        get
        {
            return this.partField;
        }
        set
        {
            this.partField = value;
        }
    }

    /// <remarks/>
    public byte TotalParts
    {
        get
        {
            return this.totalPartsField;
        }
        set
        {
            this.totalPartsField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightDESTTAF
{

    private string typeField;

    private string textField;

    private string iCAOField;

    private System.DateTime forecastTimeField;

    private System.DateTime forecastStartTimeField;

    private System.DateTime forecastEndTimeField;

    /// <remarks/>
    public string Type
    {
        get
        {
            return this.typeField;
        }
        set
        {
            this.typeField = value;
        }
    }

    /// <remarks/>
    public string Text
    {
        get
        {
            return this.textField;
        }
        set
        {
            this.textField = value;
        }
    }

    /// <remarks/>
    public string ICAO
    {
        get
        {
            return this.iCAOField;
        }
        set
        {
            this.iCAOField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ForecastTime
    {
        get
        {
            return this.forecastTimeField;
        }
        set
        {
            this.forecastTimeField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ForecastStartTime
    {
        get
        {
            return this.forecastStartTimeField;
        }
        set
        {
            this.forecastStartTimeField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ForecastEndTime
    {
        get
        {
            return this.forecastEndTimeField;
        }
        set
        {
            this.forecastEndTimeField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightALT1TAF
{

    private string typeField;

    private string textField;

    private string iCAOField;

    private System.DateTime forecastTimeField;

    private System.DateTime forecastStartTimeField;

    private System.DateTime forecastEndTimeField;

    /// <remarks/>
    public string Type
    {
        get
        {
            return this.typeField;
        }
        set
        {
            this.typeField = value;
        }
    }

    /// <remarks/>
    public string Text
    {
        get
        {
            return this.textField;
        }
        set
        {
            this.textField = value;
        }
    }

    /// <remarks/>
    public string ICAO
    {
        get
        {
            return this.iCAOField;
        }
        set
        {
            this.iCAOField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ForecastTime
    {
        get
        {
            return this.forecastTimeField;
        }
        set
        {
            this.forecastTimeField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ForecastStartTime
    {
        get
        {
            return this.forecastStartTimeField;
        }
        set
        {
            this.forecastStartTimeField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ForecastEndTime
    {
        get
        {
            return this.forecastEndTimeField;
        }
        set
        {
            this.forecastEndTimeField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightALT2TAF
{

    private string typeField;

    private string textField;

    private string iCAOField;

    private System.DateTime forecastTimeField;

    private System.DateTime forecastStartTimeField;

    private System.DateTime forecastEndTimeField;

    /// <remarks/>
    public string Type
    {
        get
        {
            return this.typeField;
        }
        set
        {
            this.typeField = value;
        }
    }

    /// <remarks/>
    public string Text
    {
        get
        {
            return this.textField;
        }
        set
        {
            this.textField = value;
        }
    }

    /// <remarks/>
    public string ICAO
    {
        get
        {
            return this.iCAOField;
        }
        set
        {
            this.iCAOField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ForecastTime
    {
        get
        {
            return this.forecastTimeField;
        }
        set
        {
            this.forecastTimeField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ForecastStartTime
    {
        get
        {
            return this.forecastStartTimeField;
        }
        set
        {
            this.forecastStartTimeField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ForecastEndTime
    {
        get
        {
            return this.forecastEndTimeField;
        }
        set
        {
            this.forecastEndTimeField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightALT1Notam
{

    private FlightALT1NotamNotam notamField;

    /// <remarks/>
    public FlightALT1NotamNotam Notam
    {
        get
        {
            return this.notamField;
        }
        set
        {
            this.notamField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightALT1NotamNotam
{

    private string numberField;

    private string textField;

    private System.DateTime fromDateField;

    private System.DateTime toDateField;

    private byte fromLevelField;

    private ushort toLevelField;

    private string firField;

    private string qCodeField;

    private string eCodeField;

    private string iCAOField;

    private string uniformAbbreviationField;

    private byte yearField;

    private FlightALT1NotamNotamPartInformation partInformationField;

    private string routePartField;

    private string providerField;

    /// <remarks/>
    public string Number
    {
        get
        {
            return this.numberField;
        }
        set
        {
            this.numberField = value;
        }
    }

    /// <remarks/>
    public string Text
    {
        get
        {
            return this.textField;
        }
        set
        {
            this.textField = value;
        }
    }

    /// <remarks/>
    public System.DateTime FromDate
    {
        get
        {
            return this.fromDateField;
        }
        set
        {
            this.fromDateField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ToDate
    {
        get
        {
            return this.toDateField;
        }
        set
        {
            this.toDateField = value;
        }
    }

    /// <remarks/>
    public byte FromLevel
    {
        get
        {
            return this.fromLevelField;
        }
        set
        {
            this.fromLevelField = value;
        }
    }

    /// <remarks/>
    public ushort ToLevel
    {
        get
        {
            return this.toLevelField;
        }
        set
        {
            this.toLevelField = value;
        }
    }

    /// <remarks/>
    public string Fir
    {
        get
        {
            return this.firField;
        }
        set
        {
            this.firField = value;
        }
    }

    /// <remarks/>
    public string QCode
    {
        get
        {
            return this.qCodeField;
        }
        set
        {
            this.qCodeField = value;
        }
    }

    /// <remarks/>
    public string ECode
    {
        get
        {
            return this.eCodeField;
        }
        set
        {
            this.eCodeField = value;
        }
    }

    /// <remarks/>
    public string ICAO
    {
        get
        {
            return this.iCAOField;
        }
        set
        {
            this.iCAOField = value;
        }
    }

    /// <remarks/>
    public string UniformAbbreviation
    {
        get
        {
            return this.uniformAbbreviationField;
        }
        set
        {
            this.uniformAbbreviationField = value;
        }
    }

    /// <remarks/>
    public byte Year
    {
        get
        {
            return this.yearField;
        }
        set
        {
            this.yearField = value;
        }
    }

    /// <remarks/>
    public FlightALT1NotamNotamPartInformation PartInformation
    {
        get
        {
            return this.partInformationField;
        }
        set
        {
            this.partInformationField = value;
        }
    }

    /// <remarks/>
    public string RoutePart
    {
        get
        {
            return this.routePartField;
        }
        set
        {
            this.routePartField = value;
        }
    }

    /// <remarks/>
    public string Provider
    {
        get
        {
            return this.providerField;
        }
        set
        {
            this.providerField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightALT1NotamNotamPartInformation
{

    private byte partField;

    private byte totalPartsField;

    /// <remarks/>
    public byte Part
    {
        get
        {
            return this.partField;
        }
        set
        {
            this.partField = value;
        }
    }

    /// <remarks/>
    public byte TotalParts
    {
        get
        {
            return this.totalPartsField;
        }
        set
        {
            this.totalPartsField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightRoutePoint
{

    private byte idField;

    private string iDENTField;

    private ushort flField;

    private ushort windField;

    private byte volField;

    private byte iSAField;

    private byte legTimeField;

    private ushort legCourseField;

    private byte legDistanceField;

    private byte legCATField;

    private string legNameField;

    private string legAWYField;

    private ushort fuelUsedField;

    private ushort fuelFlowField;

    private decimal lATField;

    private decimal lONField;

    private byte vARIATIONField;

    private ushort aCCDISTField;

    private byte aCCTIMEField;

    private ushort magCourseField;

    private ushort trueAirSpeedField;

    private ushort groundSpeedField;

    private ushort fuelRemainingField;

    private ushort distRemainingField;

    private byte timeRemainingField;

    private ushort minReqFuelField;

    private decimal fuelFlowPerEngField;

    private sbyte temperatureField;

    private byte mORAField;

    private decimal frequencyField;

    private sbyte windComponentField;

    private byte minimumEnrouteAltitudeField;

    private FlightRoutePointEco ecoField;

    private ushort magneticHeadingField;

    private ushort trueHeadingField;

    private ushort magneticTrackField;

    private ushort trueTrackField;

    private object hLAEntryExitField;

    private string fIRField;

    private string climbDescentField;

    private decimal legFuelField;

    /// <remarks/>
    public byte ID
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }

    /// <remarks/>
    public string IDENT
    {
        get
        {
            return this.iDENTField;
        }
        set
        {
            this.iDENTField = value;
        }
    }

    /// <remarks/>
    public ushort FL
    {
        get
        {
            return this.flField;
        }
        set
        {
            this.flField = value;
        }
    }

    /// <remarks/>
    public ushort Wind
    {
        get
        {
            return this.windField;
        }
        set
        {
            this.windField = value;
        }
    }

    /// <remarks/>
    public byte Vol
    {
        get
        {
            return this.volField;
        }
        set
        {
            this.volField = value;
        }
    }

    /// <remarks/>
    public byte ISA
    {
        get
        {
            return this.iSAField;
        }
        set
        {
            this.iSAField = value;
        }
    }

    /// <remarks/>
    public byte LegTime
    {
        get
        {
            return this.legTimeField;
        }
        set
        {
            this.legTimeField = value;
        }
    }

    /// <remarks/>
    public ushort LegCourse
    {
        get
        {
            return this.legCourseField;
        }
        set
        {
            this.legCourseField = value;
        }
    }

    /// <remarks/>
    public byte LegDistance
    {
        get
        {
            return this.legDistanceField;
        }
        set
        {
            this.legDistanceField = value;
        }
    }

    /// <remarks/>
    public byte LegCAT
    {
        get
        {
            return this.legCATField;
        }
        set
        {
            this.legCATField = value;
        }
    }

    /// <remarks/>
    public string LegName
    {
        get
        {
            return this.legNameField;
        }
        set
        {
            this.legNameField = value;
        }
    }

    /// <remarks/>
    public string LegAWY
    {
        get
        {
            return this.legAWYField;
        }
        set
        {
            this.legAWYField = value;
        }
    }

    /// <remarks/>
    public ushort FuelUsed
    {
        get
        {
            return this.fuelUsedField;
        }
        set
        {
            this.fuelUsedField = value;
        }
    }

    /// <remarks/>
    public ushort FuelFlow
    {
        get
        {
            return this.fuelFlowField;
        }
        set
        {
            this.fuelFlowField = value;
        }
    }

    /// <remarks/>
    public decimal LAT
    {
        get
        {
            return this.lATField;
        }
        set
        {
            this.lATField = value;
        }
    }

    /// <remarks/>
    public decimal LON
    {
        get
        {
            return this.lONField;
        }
        set
        {
            this.lONField = value;
        }
    }

    /// <remarks/>
    public byte VARIATION
    {
        get
        {
            return this.vARIATIONField;
        }
        set
        {
            this.vARIATIONField = value;
        }
    }

    /// <remarks/>
    public ushort ACCDIST
    {
        get
        {
            return this.aCCDISTField;
        }
        set
        {
            this.aCCDISTField = value;
        }
    }

    /// <remarks/>
    public byte ACCTIME
    {
        get
        {
            return this.aCCTIMEField;
        }
        set
        {
            this.aCCTIMEField = value;
        }
    }

    /// <remarks/>
    public ushort MagCourse
    {
        get
        {
            return this.magCourseField;
        }
        set
        {
            this.magCourseField = value;
        }
    }

    /// <remarks/>
    public ushort TrueAirSpeed
    {
        get
        {
            return this.trueAirSpeedField;
        }
        set
        {
            this.trueAirSpeedField = value;
        }
    }

    /// <remarks/>
    public ushort GroundSpeed
    {
        get
        {
            return this.groundSpeedField;
        }
        set
        {
            this.groundSpeedField = value;
        }
    }

    /// <remarks/>
    public ushort FuelRemaining
    {
        get
        {
            return this.fuelRemainingField;
        }
        set
        {
            this.fuelRemainingField = value;
        }
    }

    /// <remarks/>
    public ushort DistRemaining
    {
        get
        {
            return this.distRemainingField;
        }
        set
        {
            this.distRemainingField = value;
        }
    }

    /// <remarks/>
    public byte TimeRemaining
    {
        get
        {
            return this.timeRemainingField;
        }
        set
        {
            this.timeRemainingField = value;
        }
    }

    /// <remarks/>
    public ushort MinReqFuel
    {
        get
        {
            return this.minReqFuelField;
        }
        set
        {
            this.minReqFuelField = value;
        }
    }

    /// <remarks/>
    public decimal FuelFlowPerEng
    {
        get
        {
            return this.fuelFlowPerEngField;
        }
        set
        {
            this.fuelFlowPerEngField = value;
        }
    }

    /// <remarks/>
    public sbyte Temperature
    {
        get
        {
            return this.temperatureField;
        }
        set
        {
            this.temperatureField = value;
        }
    }

    /// <remarks/>
    public byte MORA
    {
        get
        {
            return this.mORAField;
        }
        set
        {
            this.mORAField = value;
        }
    }

    /// <remarks/>
    public decimal Frequency
    {
        get
        {
            return this.frequencyField;
        }
        set
        {
            this.frequencyField = value;
        }
    }

    /// <remarks/>
    public sbyte WindComponent
    {
        get
        {
            return this.windComponentField;
        }
        set
        {
            this.windComponentField = value;
        }
    }

    /// <remarks/>
    public byte MinimumEnrouteAltitude
    {
        get
        {
            return this.minimumEnrouteAltitudeField;
        }
        set
        {
            this.minimumEnrouteAltitudeField = value;
        }
    }

    /// <remarks/>
    public FlightRoutePointEco Eco
    {
        get
        {
            return this.ecoField;
        }
        set
        {
            this.ecoField = value;
        }
    }

    /// <remarks/>
    public ushort MagneticHeading
    {
        get
        {
            return this.magneticHeadingField;
        }
        set
        {
            this.magneticHeadingField = value;
        }
    }

    /// <remarks/>
    public ushort TrueHeading
    {
        get
        {
            return this.trueHeadingField;
        }
        set
        {
            this.trueHeadingField = value;
        }
    }

    /// <remarks/>
    public ushort MagneticTrack
    {
        get
        {
            return this.magneticTrackField;
        }
        set
        {
            this.magneticTrackField = value;
        }
    }

    /// <remarks/>
    public ushort TrueTrack
    {
        get
        {
            return this.trueTrackField;
        }
        set
        {
            this.trueTrackField = value;
        }
    }

    /// <remarks/>
    public object HLAEntryExit
    {
        get
        {
            return this.hLAEntryExitField;
        }
        set
        {
            this.hLAEntryExitField = value;
        }
    }

    /// <remarks/>
    public string FIR
    {
        get
        {
            return this.fIRField;
        }
        set
        {
            this.fIRField = value;
        }
    }

    /// <remarks/>
    public string ClimbDescent
    {
        get
        {
            return this.climbDescentField;
        }
        set
        {
            this.climbDescentField = value;
        }
    }

    /// <remarks/>
    public decimal LegFuel
    {
        get
        {
            return this.legFuelField;
        }
        set
        {
            this.legFuelField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightRoutePointEco
{

    private ushort optSpeedFLField;

    private byte speedGainField;

    private ushort optEcoFLField;

    private ushort moneyGainField;

    private ushort optFuelFLField;

    private byte fuelGainField;

    /// <remarks/>
    public ushort OptSpeedFL
    {
        get
        {
            return this.optSpeedFLField;
        }
        set
        {
            this.optSpeedFLField = value;
        }
    }

    /// <remarks/>
    public byte SpeedGain
    {
        get
        {
            return this.speedGainField;
        }
        set
        {
            this.speedGainField = value;
        }
    }

    /// <remarks/>
    public ushort OptEcoFL
    {
        get
        {
            return this.optEcoFLField;
        }
        set
        {
            this.optEcoFLField = value;
        }
    }

    /// <remarks/>
    public ushort MoneyGain
    {
        get
        {
            return this.moneyGainField;
        }
        set
        {
            this.moneyGainField = value;
        }
    }

    /// <remarks/>
    public ushort OptFuelFL
    {
        get
        {
            return this.optFuelFLField;
        }
        set
        {
            this.optFuelFLField = value;
        }
    }

    /// <remarks/>
    public byte FuelGain
    {
        get
        {
            return this.fuelGainField;
        }
        set
        {
            this.fuelGainField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightCrews
{

    private FlightCrewsCrew crewField;

    /// <remarks/>
    public FlightCrewsCrew Crew
    {
        get
        {
            return this.crewField;
        }
        set
        {
            this.crewField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightCrewsCrew
{

    private object idField;

    private string crewTypeField;

    private string crewNameField;

    private string initialsField;

    private object gSMField;

    private object massField;

    /// <remarks/>
    public object ID
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }

    /// <remarks/>
    public string CrewType
    {
        get
        {
            return this.crewTypeField;
        }
        set
        {
            this.crewTypeField = value;
        }
    }

    /// <remarks/>
    public string CrewName
    {
        get
        {
            return this.crewNameField;
        }
        set
        {
            this.crewNameField = value;
        }
    }

    /// <remarks/>
    public string Initials
    {
        get
        {
            return this.initialsField;
        }
        set
        {
            this.initialsField = value;
        }
    }

    /// <remarks/>
    public object GSM
    {
        get
        {
            return this.gSMField;
        }
        set
        {
            this.gSMField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
    public object Mass
    {
        get
        {
            return this.massField;
        }
        set
        {
            this.massField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightResponce
{

    private bool succeedField;

    /// <remarks/>
    public bool Succeed
    {
        get
        {
            return this.succeedField;
        }
        set
        {
            this.succeedField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightATCData
{

    private string aTCIDField;

    private string aTCTOAField;

    private string aTCRuleField;

    private string aTCTypeField;

    private object aTCNumField;

    private string aTCWakeField;

    private string aTCEquiField;

    private string aTCSSRField;

    private string aTCDepField;

    private ushort aTCTimeField;

    private string aTCSpeedField;

    private string aTCFLField;

    private string aTCRouteField;

    private string aTCDestField;

    private byte aTCEETField;

    private string aTCAlt1Field;

    private string aTCAlt2Field;

    private string aTCInfoField;

    private ushort aTCEnduField;

    private byte aTCPersField;

    private string aTCRadiField;

    private object aTCSurvField;

    private string aTCJackField;

    private object aTCDingField;

    private object aTCCapField;

    private object aTCCoverField;

    private object aTCColoField;

    private string aTCAccoField;

    private object aTCRemField;

    private string aTCPICField;

    private object aTCCtotField;

    /// <remarks/>
    public string ATCID
    {
        get
        {
            return this.aTCIDField;
        }
        set
        {
            this.aTCIDField = value;
        }
    }

    /// <remarks/>
    public string ATCTOA
    {
        get
        {
            return this.aTCTOAField;
        }
        set
        {
            this.aTCTOAField = value;
        }
    }

    /// <remarks/>
    public string ATCRule
    {
        get
        {
            return this.aTCRuleField;
        }
        set
        {
            this.aTCRuleField = value;
        }
    }

    /// <remarks/>
    public string ATCType
    {
        get
        {
            return this.aTCTypeField;
        }
        set
        {
            this.aTCTypeField = value;
        }
    }

    /// <remarks/>
    public object ATCNum
    {
        get
        {
            return this.aTCNumField;
        }
        set
        {
            this.aTCNumField = value;
        }
    }

    /// <remarks/>
    public string ATCWake
    {
        get
        {
            return this.aTCWakeField;
        }
        set
        {
            this.aTCWakeField = value;
        }
    }

    /// <remarks/>
    public string ATCEqui
    {
        get
        {
            return this.aTCEquiField;
        }
        set
        {
            this.aTCEquiField = value;
        }
    }

    /// <remarks/>
    public string ATCSSR
    {
        get
        {
            return this.aTCSSRField;
        }
        set
        {
            this.aTCSSRField = value;
        }
    }

    /// <remarks/>
    public string ATCDep
    {
        get
        {
            return this.aTCDepField;
        }
        set
        {
            this.aTCDepField = value;
        }
    }

    /// <remarks/>
    public ushort ATCTime
    {
        get
        {
            return this.aTCTimeField;
        }
        set
        {
            this.aTCTimeField = value;
        }
    }

    /// <remarks/>
    public string ATCSpeed
    {
        get
        {
            return this.aTCSpeedField;
        }
        set
        {
            this.aTCSpeedField = value;
        }
    }

    /// <remarks/>
    public string ATCFL
    {
        get
        {
            return this.aTCFLField;
        }
        set
        {
            this.aTCFLField = value;
        }
    }

    /// <remarks/>
    public string ATCRoute
    {
        get
        {
            return this.aTCRouteField;
        }
        set
        {
            this.aTCRouteField = value;
        }
    }

    /// <remarks/>
    public string ATCDest
    {
        get
        {
            return this.aTCDestField;
        }
        set
        {
            this.aTCDestField = value;
        }
    }

    /// <remarks/>
    public byte ATCEET
    {
        get
        {
            return this.aTCEETField;
        }
        set
        {
            this.aTCEETField = value;
        }
    }

    /// <remarks/>
    public string ATCAlt1
    {
        get
        {
            return this.aTCAlt1Field;
        }
        set
        {
            this.aTCAlt1Field = value;
        }
    }

    /// <remarks/>
    public string ATCAlt2
    {
        get
        {
            return this.aTCAlt2Field;
        }
        set
        {
            this.aTCAlt2Field = value;
        }
    }

    /// <remarks/>
    public string ATCInfo
    {
        get
        {
            return this.aTCInfoField;
        }
        set
        {
            this.aTCInfoField = value;
        }
    }

    /// <remarks/>
    public ushort ATCEndu
    {
        get
        {
            return this.aTCEnduField;
        }
        set
        {
            this.aTCEnduField = value;
        }
    }

    /// <remarks/>
    public byte ATCPers
    {
        get
        {
            return this.aTCPersField;
        }
        set
        {
            this.aTCPersField = value;
        }
    }

    /// <remarks/>
    public string ATCRadi
    {
        get
        {
            return this.aTCRadiField;
        }
        set
        {
            this.aTCRadiField = value;
        }
    }

    /// <remarks/>
    public object ATCSurv
    {
        get
        {
            return this.aTCSurvField;
        }
        set
        {
            this.aTCSurvField = value;
        }
    }

    /// <remarks/>
    public string ATCJack
    {
        get
        {
            return this.aTCJackField;
        }
        set
        {
            this.aTCJackField = value;
        }
    }

    /// <remarks/>
    public object ATCDing
    {
        get
        {
            return this.aTCDingField;
        }
        set
        {
            this.aTCDingField = value;
        }
    }

    /// <remarks/>
    public object ATCCap
    {
        get
        {
            return this.aTCCapField;
        }
        set
        {
            this.aTCCapField = value;
        }
    }

    /// <remarks/>
    public object ATCCover
    {
        get
        {
            return this.aTCCoverField;
        }
        set
        {
            this.aTCCoverField = value;
        }
    }

    /// <remarks/>
    public object ATCColo
    {
        get
        {
            return this.aTCColoField;
        }
        set
        {
            this.aTCColoField = value;
        }
    }

    /// <remarks/>
    public string ATCAcco
    {
        get
        {
            return this.aTCAccoField;
        }
        set
        {
            this.aTCAccoField = value;
        }
    }

    /// <remarks/>
    public object ATCRem
    {
        get
        {
            return this.aTCRemField;
        }
        set
        {
            this.aTCRemField = value;
        }
    }

    /// <remarks/>
    public string ATCPIC
    {
        get
        {
            return this.aTCPICField;
        }
        set
        {
            this.aTCPICField = value;
        }
    }

    /// <remarks/>
    public object ATCCtot
    {
        get
        {
            return this.aTCCtotField;
        }
        set
        {
            this.aTCCtotField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightFlightLevel
{

    private ushort levelField;

    private ushort costField;

    private sbyte wcField;

    private byte timeNCruiseField;

    private ushort fuelNCruiseField;

    private byte timeProfile2Field;

    private ushort fuelProfile2Field;

    private byte timeProfile3Field;

    private ushort fuelProfile3Field;

    private byte fuelLowerField;

    private byte costDiffField;

    /// <remarks/>
    public ushort Level
    {
        get
        {
            return this.levelField;
        }
        set
        {
            this.levelField = value;
        }
    }

    /// <remarks/>
    public ushort Cost
    {
        get
        {
            return this.costField;
        }
        set
        {
            this.costField = value;
        }
    }

    /// <remarks/>
    public sbyte WC
    {
        get
        {
            return this.wcField;
        }
        set
        {
            this.wcField = value;
        }
    }

    /// <remarks/>
    public byte TimeNCruise
    {
        get
        {
            return this.timeNCruiseField;
        }
        set
        {
            this.timeNCruiseField = value;
        }
    }

    /// <remarks/>
    public ushort FuelNCruise
    {
        get
        {
            return this.fuelNCruiseField;
        }
        set
        {
            this.fuelNCruiseField = value;
        }
    }

    /// <remarks/>
    public byte TimeProfile2
    {
        get
        {
            return this.timeProfile2Field;
        }
        set
        {
            this.timeProfile2Field = value;
        }
    }

    /// <remarks/>
    public ushort FuelProfile2
    {
        get
        {
            return this.fuelProfile2Field;
        }
        set
        {
            this.fuelProfile2Field = value;
        }
    }

    /// <remarks/>
    public byte TimeProfile3
    {
        get
        {
            return this.timeProfile3Field;
        }
        set
        {
            this.timeProfile3Field = value;
        }
    }

    /// <remarks/>
    public ushort FuelProfile3
    {
        get
        {
            return this.fuelProfile3Field;
        }
        set
        {
            this.fuelProfile3Field = value;
        }
    }

    /// <remarks/>
    public byte FuelLower
    {
        get
        {
            return this.fuelLowerField;
        }
        set
        {
            this.fuelLowerField = value;
        }
    }

    /// <remarks/>
    public byte CostDiff
    {
        get
        {
            return this.costDiffField;
        }
        set
        {
            this.costDiffField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightNotam1
{

    private string numberField;

    private string textField;

    private System.DateTime fromDateField;

    private System.DateTime toDateField;

    private byte fromLevelField;

    private ushort toLevelField;

    private string firField;

    private string qCodeField;

    private string eCodeField;

    private string iCAOField;

    private string uniformAbbreviationField;

    private byte yearField;

    private FlightNotamPartInformation1 partInformationField;

    private string routePartField;

    private string providerField;

    /// <remarks/>
    public string Number
    {
        get
        {
            return this.numberField;
        }
        set
        {
            this.numberField = value;
        }
    }

    /// <remarks/>
    public string Text
    {
        get
        {
            return this.textField;
        }
        set
        {
            this.textField = value;
        }
    }

    /// <remarks/>
    public System.DateTime FromDate
    {
        get
        {
            return this.fromDateField;
        }
        set
        {
            this.fromDateField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ToDate
    {
        get
        {
            return this.toDateField;
        }
        set
        {
            this.toDateField = value;
        }
    }

    /// <remarks/>
    public byte FromLevel
    {
        get
        {
            return this.fromLevelField;
        }
        set
        {
            this.fromLevelField = value;
        }
    }

    /// <remarks/>
    public ushort ToLevel
    {
        get
        {
            return this.toLevelField;
        }
        set
        {
            this.toLevelField = value;
        }
    }

    /// <remarks/>
    public string Fir
    {
        get
        {
            return this.firField;
        }
        set
        {
            this.firField = value;
        }
    }

    /// <remarks/>
    public string QCode
    {
        get
        {
            return this.qCodeField;
        }
        set
        {
            this.qCodeField = value;
        }
    }

    /// <remarks/>
    public string ECode
    {
        get
        {
            return this.eCodeField;
        }
        set
        {
            this.eCodeField = value;
        }
    }

    /// <remarks/>
    public string ICAO
    {
        get
        {
            return this.iCAOField;
        }
        set
        {
            this.iCAOField = value;
        }
    }

    /// <remarks/>
    public string UniformAbbreviation
    {
        get
        {
            return this.uniformAbbreviationField;
        }
        set
        {
            this.uniformAbbreviationField = value;
        }
    }

    /// <remarks/>
    public byte Year
    {
        get
        {
            return this.yearField;
        }
        set
        {
            this.yearField = value;
        }
    }

    /// <remarks/>
    public FlightNotamPartInformation1 PartInformation
    {
        get
        {
            return this.partInformationField;
        }
        set
        {
            this.partInformationField = value;
        }
    }

    /// <remarks/>
    public string RoutePart
    {
        get
        {
            return this.routePartField;
        }
        set
        {
            this.routePartField = value;
        }
    }

    /// <remarks/>
    public string Provider
    {
        get
        {
            return this.providerField;
        }
        set
        {
            this.providerField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightNotamPartInformation1
{

    private byte partField;

    private byte totalPartsField;

    /// <remarks/>
    public byte Part
    {
        get
        {
            return this.partField;
        }
        set
        {
            this.partField = value;
        }
    }

    /// <remarks/>
    public byte TotalParts
    {
        get
        {
            return this.totalPartsField;
        }
        set
        {
            this.totalPartsField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightNotam2
{

    private string numberField;

    private string textField;

    private System.DateTime fromDateField;

    private System.DateTime toDateField;

    private ushort fromLevelField;

    private ushort toLevelField;

    private string firField;

    private string qCodeField;

    private string eCodeField;

    private string iCAOField;

    private string uniformAbbreviationField;

    private byte yearField;

    private FlightNotamPartInformation2 partInformationField;

    private string routePartField;

    private string providerField;

    /// <remarks/>
    public string Number
    {
        get
        {
            return this.numberField;
        }
        set
        {
            this.numberField = value;
        }
    }

    /// <remarks/>
    public string Text
    {
        get
        {
            return this.textField;
        }
        set
        {
            this.textField = value;
        }
    }

    /// <remarks/>
    public System.DateTime FromDate
    {
        get
        {
            return this.fromDateField;
        }
        set
        {
            this.fromDateField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ToDate
    {
        get
        {
            return this.toDateField;
        }
        set
        {
            this.toDateField = value;
        }
    }

    /// <remarks/>
    public ushort FromLevel
    {
        get
        {
            return this.fromLevelField;
        }
        set
        {
            this.fromLevelField = value;
        }
    }

    /// <remarks/>
    public ushort ToLevel
    {
        get
        {
            return this.toLevelField;
        }
        set
        {
            this.toLevelField = value;
        }
    }

    /// <remarks/>
    public string Fir
    {
        get
        {
            return this.firField;
        }
        set
        {
            this.firField = value;
        }
    }

    /// <remarks/>
    public string QCode
    {
        get
        {
            return this.qCodeField;
        }
        set
        {
            this.qCodeField = value;
        }
    }

    /// <remarks/>
    public string ECode
    {
        get
        {
            return this.eCodeField;
        }
        set
        {
            this.eCodeField = value;
        }
    }

    /// <remarks/>
    public string ICAO
    {
        get
        {
            return this.iCAOField;
        }
        set
        {
            this.iCAOField = value;
        }
    }

    /// <remarks/>
    public string UniformAbbreviation
    {
        get
        {
            return this.uniformAbbreviationField;
        }
        set
        {
            this.uniformAbbreviationField = value;
        }
    }

    /// <remarks/>
    public byte Year
    {
        get
        {
            return this.yearField;
        }
        set
        {
            this.yearField = value;
        }
    }

    /// <remarks/>
    public FlightNotamPartInformation2 PartInformation
    {
        get
        {
            return this.partInformationField;
        }
        set
        {
            this.partInformationField = value;
        }
    }

    /// <remarks/>
    public string RoutePart
    {
        get
        {
            return this.routePartField;
        }
        set
        {
            this.routePartField = value;
        }
    }

    /// <remarks/>
    public string Provider
    {
        get
        {
            return this.providerField;
        }
        set
        {
            this.providerField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightNotamPartInformation2
{

    private byte partField;

    private byte totalPartsField;

    /// <remarks/>
    public byte Part
    {
        get
        {
            return this.partField;
        }
        set
        {
            this.partField = value;
        }
    }

    /// <remarks/>
    public byte TotalParts
    {
        get
        {
            return this.totalPartsField;
        }
        set
        {
            this.totalPartsField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightAltAirport
{

    private string typeField;

    private string icaoField;

    private ushort distField;

    private byte timeField;

    private ushort fuelField;

    private ushort mAGCURSField;

    private string aTCField;

    private decimal latField;

    private decimal longField;

    private ushort rwylField;

    private ushort elevationField;

    private string nameField;

    private string iataField;

    private object categoryField;

    private object frequenciesField;

    private object frequencies2Field;

    /// <remarks/>
    public string Type
    {
        get
        {
            return this.typeField;
        }
        set
        {
            this.typeField = value;
        }
    }

    /// <remarks/>
    public string Icao
    {
        get
        {
            return this.icaoField;
        }
        set
        {
            this.icaoField = value;
        }
    }

    /// <remarks/>
    public ushort Dist
    {
        get
        {
            return this.distField;
        }
        set
        {
            this.distField = value;
        }
    }

    /// <remarks/>
    public byte Time
    {
        get
        {
            return this.timeField;
        }
        set
        {
            this.timeField = value;
        }
    }

    /// <remarks/>
    public ushort Fuel
    {
        get
        {
            return this.fuelField;
        }
        set
        {
            this.fuelField = value;
        }
    }

    /// <remarks/>
    public ushort MAGCURS
    {
        get
        {
            return this.mAGCURSField;
        }
        set
        {
            this.mAGCURSField = value;
        }
    }

    /// <remarks/>
    public string ATC
    {
        get
        {
            return this.aTCField;
        }
        set
        {
            this.aTCField = value;
        }
    }

    /// <remarks/>
    public decimal Lat
    {
        get
        {
            return this.latField;
        }
        set
        {
            this.latField = value;
        }
    }

    /// <remarks/>
    public decimal Long
    {
        get
        {
            return this.longField;
        }
        set
        {
            this.longField = value;
        }
    }

    /// <remarks/>
    public ushort Rwyl
    {
        get
        {
            return this.rwylField;
        }
        set
        {
            this.rwylField = value;
        }
    }

    /// <remarks/>
    public ushort Elevation
    {
        get
        {
            return this.elevationField;
        }
        set
        {
            this.elevationField = value;
        }
    }

    /// <remarks/>
    public string Name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    public string Iata
    {
        get
        {
            return this.iataField;
        }
        set
        {
            this.iataField = value;
        }
    }

    /// <remarks/>
    public object Category
    {
        get
        {
            return this.categoryField;
        }
        set
        {
            this.categoryField = value;
        }
    }

    /// <remarks/>
    public object Frequencies
    {
        get
        {
            return this.frequenciesField;
        }
        set
        {
            this.frequenciesField = value;
        }
    }

    /// <remarks/>
    public object Frequencies2
    {
        get
        {
            return this.frequencies2Field;
        }
        set
        {
            this.frequencies2Field = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightRoutePoint1
{

    private byte idField;

    private string iDENTField;

    private ushort flField;

    private ushort windField;

    private byte volField;

    private byte iSAField;

    private byte legTimeField;

    private ushort legCourseField;

    private byte legDistanceField;

    private byte legCATField;

    private string legNameField;

    private string legAWYField;

    private decimal fuelUsedField;

    private ushort fuelFlowField;

    private decimal lATField;

    private decimal lONField;

    private byte vARIATIONField;

    private ushort aCCDISTField;

    private byte aCCTIMEField;

    private ushort magCourseField;

    private ushort trueAirSpeedField;

    private ushort groundSpeedField;

    private decimal fuelRemainingField;

    private byte distRemainingField;

    private byte timeRemainingField;

    private decimal minReqFuelField;

    private byte fuelFlowPerEngField;

    private sbyte temperatureField;

    private byte mORAField;

    private byte frequencyField;

    private byte windComponentField;

    private byte minimumEnrouteAltitudeField;

    private ushort magneticHeadingField;

    private ushort trueHeadingField;

    private ushort magneticTrackField;

    private ushort trueTrackField;

    private string climbDescentField;

    private decimal legFuelField;

    /// <remarks/>
    public byte ID
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }

    /// <remarks/>
    public string IDENT
    {
        get
        {
            return this.iDENTField;
        }
        set
        {
            this.iDENTField = value;
        }
    }

    /// <remarks/>
    public ushort FL
    {
        get
        {
            return this.flField;
        }
        set
        {
            this.flField = value;
        }
    }

    /// <remarks/>
    public ushort Wind
    {
        get
        {
            return this.windField;
        }
        set
        {
            this.windField = value;
        }
    }

    /// <remarks/>
    public byte Vol
    {
        get
        {
            return this.volField;
        }
        set
        {
            this.volField = value;
        }
    }

    /// <remarks/>
    public byte ISA
    {
        get
        {
            return this.iSAField;
        }
        set
        {
            this.iSAField = value;
        }
    }

    /// <remarks/>
    public byte LegTime
    {
        get
        {
            return this.legTimeField;
        }
        set
        {
            this.legTimeField = value;
        }
    }

    /// <remarks/>
    public ushort LegCourse
    {
        get
        {
            return this.legCourseField;
        }
        set
        {
            this.legCourseField = value;
        }
    }

    /// <remarks/>
    public byte LegDistance
    {
        get
        {
            return this.legDistanceField;
        }
        set
        {
            this.legDistanceField = value;
        }
    }

    /// <remarks/>
    public byte LegCAT
    {
        get
        {
            return this.legCATField;
        }
        set
        {
            this.legCATField = value;
        }
    }

    /// <remarks/>
    public string LegName
    {
        get
        {
            return this.legNameField;
        }
        set
        {
            this.legNameField = value;
        }
    }

    /// <remarks/>
    public string LegAWY
    {
        get
        {
            return this.legAWYField;
        }
        set
        {
            this.legAWYField = value;
        }
    }

    /// <remarks/>
    public decimal FuelUsed
    {
        get
        {
            return this.fuelUsedField;
        }
        set
        {
            this.fuelUsedField = value;
        }
    }

    /// <remarks/>
    public ushort FuelFlow
    {
        get
        {
            return this.fuelFlowField;
        }
        set
        {
            this.fuelFlowField = value;
        }
    }

    /// <remarks/>
    public decimal LAT
    {
        get
        {
            return this.lATField;
        }
        set
        {
            this.lATField = value;
        }
    }

    /// <remarks/>
    public decimal LON
    {
        get
        {
            return this.lONField;
        }
        set
        {
            this.lONField = value;
        }
    }

    /// <remarks/>
    public byte VARIATION
    {
        get
        {
            return this.vARIATIONField;
        }
        set
        {
            this.vARIATIONField = value;
        }
    }

    /// <remarks/>
    public ushort ACCDIST
    {
        get
        {
            return this.aCCDISTField;
        }
        set
        {
            this.aCCDISTField = value;
        }
    }

    /// <remarks/>
    public byte ACCTIME
    {
        get
        {
            return this.aCCTIMEField;
        }
        set
        {
            this.aCCTIMEField = value;
        }
    }

    /// <remarks/>
    public ushort MagCourse
    {
        get
        {
            return this.magCourseField;
        }
        set
        {
            this.magCourseField = value;
        }
    }

    /// <remarks/>
    public ushort TrueAirSpeed
    {
        get
        {
            return this.trueAirSpeedField;
        }
        set
        {
            this.trueAirSpeedField = value;
        }
    }

    /// <remarks/>
    public ushort GroundSpeed
    {
        get
        {
            return this.groundSpeedField;
        }
        set
        {
            this.groundSpeedField = value;
        }
    }

    /// <remarks/>
    public decimal FuelRemaining
    {
        get
        {
            return this.fuelRemainingField;
        }
        set
        {
            this.fuelRemainingField = value;
        }
    }

    /// <remarks/>
    public byte DistRemaining
    {
        get
        {
            return this.distRemainingField;
        }
        set
        {
            this.distRemainingField = value;
        }
    }

    /// <remarks/>
    public byte TimeRemaining
    {
        get
        {
            return this.timeRemainingField;
        }
        set
        {
            this.timeRemainingField = value;
        }
    }

    /// <remarks/>
    public decimal MinReqFuel
    {
        get
        {
            return this.minReqFuelField;
        }
        set
        {
            this.minReqFuelField = value;
        }
    }

    /// <remarks/>
    public byte FuelFlowPerEng
    {
        get
        {
            return this.fuelFlowPerEngField;
        }
        set
        {
            this.fuelFlowPerEngField = value;
        }
    }

    /// <remarks/>
    public sbyte Temperature
    {
        get
        {
            return this.temperatureField;
        }
        set
        {
            this.temperatureField = value;
        }
    }

    /// <remarks/>
    public byte MORA
    {
        get
        {
            return this.mORAField;
        }
        set
        {
            this.mORAField = value;
        }
    }

    /// <remarks/>
    public byte Frequency
    {
        get
        {
            return this.frequencyField;
        }
        set
        {
            this.frequencyField = value;
        }
    }

    /// <remarks/>
    public byte WindComponent
    {
        get
        {
            return this.windComponentField;
        }
        set
        {
            this.windComponentField = value;
        }
    }

    /// <remarks/>
    public byte MinimumEnrouteAltitude
    {
        get
        {
            return this.minimumEnrouteAltitudeField;
        }
        set
        {
            this.minimumEnrouteAltitudeField = value;
        }
    }

    /// <remarks/>
    public ushort MagneticHeading
    {
        get
        {
            return this.magneticHeadingField;
        }
        set
        {
            this.magneticHeadingField = value;
        }
    }

    /// <remarks/>
    public ushort TrueHeading
    {
        get
        {
            return this.trueHeadingField;
        }
        set
        {
            this.trueHeadingField = value;
        }
    }

    /// <remarks/>
    public ushort MagneticTrack
    {
        get
        {
            return this.magneticTrackField;
        }
        set
        {
            this.magneticTrackField = value;
        }
    }

    /// <remarks/>
    public ushort TrueTrack
    {
        get
        {
            return this.trueTrackField;
        }
        set
        {
            this.trueTrackField = value;
        }
    }

    /// <remarks/>
    public string ClimbDescent
    {
        get
        {
            return this.climbDescentField;
        }
        set
        {
            this.climbDescentField = value;
        }
    }

    /// <remarks/>
    public decimal LegFuel
    {
        get
        {
            return this.legFuelField;
        }
        set
        {
            this.legFuelField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightRoutePoint2
{

    private byte idField;

    private string iDENTField;

    private byte flField;

    private ushort windField;

    private byte volField;

    private byte iSAField;

    private byte legTimeField;

    private ushort legCourseField;

    private byte legDistanceField;

    private byte legCATField;

    private string legNameField;

    private string legAWYField;

    private decimal fuelUsedField;

    private ushort fuelFlowField;

    private decimal lATField;

    private decimal lONField;

    private byte vARIATIONField;

    private ushort aCCDISTField;

    private byte aCCTIMEField;

    private ushort magCourseField;

    private ushort trueAirSpeedField;

    private ushort groundSpeedField;

    private decimal fuelRemainingField;

    private byte distRemainingField;

    private byte timeRemainingField;

    private decimal minReqFuelField;

    private byte fuelFlowPerEngField;

    private sbyte temperatureField;

    private byte mORAField;

    private byte frequencyField;

    private sbyte windComponentField;

    private byte minimumEnrouteAltitudeField;

    private ushort magneticHeadingField;

    private ushort trueHeadingField;

    private ushort magneticTrackField;

    private ushort trueTrackField;

    private string climbDescentField;

    private decimal legFuelField;

    /// <remarks/>
    public byte ID
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }

    /// <remarks/>
    public string IDENT
    {
        get
        {
            return this.iDENTField;
        }
        set
        {
            this.iDENTField = value;
        }
    }

    /// <remarks/>
    public byte FL
    {
        get
        {
            return this.flField;
        }
        set
        {
            this.flField = value;
        }
    }

    /// <remarks/>
    public ushort Wind
    {
        get
        {
            return this.windField;
        }
        set
        {
            this.windField = value;
        }
    }

    /// <remarks/>
    public byte Vol
    {
        get
        {
            return this.volField;
        }
        set
        {
            this.volField = value;
        }
    }

    /// <remarks/>
    public byte ISA
    {
        get
        {
            return this.iSAField;
        }
        set
        {
            this.iSAField = value;
        }
    }

    /// <remarks/>
    public byte LegTime
    {
        get
        {
            return this.legTimeField;
        }
        set
        {
            this.legTimeField = value;
        }
    }

    /// <remarks/>
    public ushort LegCourse
    {
        get
        {
            return this.legCourseField;
        }
        set
        {
            this.legCourseField = value;
        }
    }

    /// <remarks/>
    public byte LegDistance
    {
        get
        {
            return this.legDistanceField;
        }
        set
        {
            this.legDistanceField = value;
        }
    }

    /// <remarks/>
    public byte LegCAT
    {
        get
        {
            return this.legCATField;
        }
        set
        {
            this.legCATField = value;
        }
    }

    /// <remarks/>
    public string LegName
    {
        get
        {
            return this.legNameField;
        }
        set
        {
            this.legNameField = value;
        }
    }

    /// <remarks/>
    public string LegAWY
    {
        get
        {
            return this.legAWYField;
        }
        set
        {
            this.legAWYField = value;
        }
    }

    /// <remarks/>
    public decimal FuelUsed
    {
        get
        {
            return this.fuelUsedField;
        }
        set
        {
            this.fuelUsedField = value;
        }
    }

    /// <remarks/>
    public ushort FuelFlow
    {
        get
        {
            return this.fuelFlowField;
        }
        set
        {
            this.fuelFlowField = value;
        }
    }

    /// <remarks/>
    public decimal LAT
    {
        get
        {
            return this.lATField;
        }
        set
        {
            this.lATField = value;
        }
    }

    /// <remarks/>
    public decimal LON
    {
        get
        {
            return this.lONField;
        }
        set
        {
            this.lONField = value;
        }
    }

    /// <remarks/>
    public byte VARIATION
    {
        get
        {
            return this.vARIATIONField;
        }
        set
        {
            this.vARIATIONField = value;
        }
    }

    /// <remarks/>
    public ushort ACCDIST
    {
        get
        {
            return this.aCCDISTField;
        }
        set
        {
            this.aCCDISTField = value;
        }
    }

    /// <remarks/>
    public byte ACCTIME
    {
        get
        {
            return this.aCCTIMEField;
        }
        set
        {
            this.aCCTIMEField = value;
        }
    }

    /// <remarks/>
    public ushort MagCourse
    {
        get
        {
            return this.magCourseField;
        }
        set
        {
            this.magCourseField = value;
        }
    }

    /// <remarks/>
    public ushort TrueAirSpeed
    {
        get
        {
            return this.trueAirSpeedField;
        }
        set
        {
            this.trueAirSpeedField = value;
        }
    }

    /// <remarks/>
    public ushort GroundSpeed
    {
        get
        {
            return this.groundSpeedField;
        }
        set
        {
            this.groundSpeedField = value;
        }
    }

    /// <remarks/>
    public decimal FuelRemaining
    {
        get
        {
            return this.fuelRemainingField;
        }
        set
        {
            this.fuelRemainingField = value;
        }
    }

    /// <remarks/>
    public byte DistRemaining
    {
        get
        {
            return this.distRemainingField;
        }
        set
        {
            this.distRemainingField = value;
        }
    }

    /// <remarks/>
    public byte TimeRemaining
    {
        get
        {
            return this.timeRemainingField;
        }
        set
        {
            this.timeRemainingField = value;
        }
    }

    /// <remarks/>
    public decimal MinReqFuel
    {
        get
        {
            return this.minReqFuelField;
        }
        set
        {
            this.minReqFuelField = value;
        }
    }

    /// <remarks/>
    public byte FuelFlowPerEng
    {
        get
        {
            return this.fuelFlowPerEngField;
        }
        set
        {
            this.fuelFlowPerEngField = value;
        }
    }

    /// <remarks/>
    public sbyte Temperature
    {
        get
        {
            return this.temperatureField;
        }
        set
        {
            this.temperatureField = value;
        }
    }

    /// <remarks/>
    public byte MORA
    {
        get
        {
            return this.mORAField;
        }
        set
        {
            this.mORAField = value;
        }
    }

    /// <remarks/>
    public byte Frequency
    {
        get
        {
            return this.frequencyField;
        }
        set
        {
            this.frequencyField = value;
        }
    }

    /// <remarks/>
    public sbyte WindComponent
    {
        get
        {
            return this.windComponentField;
        }
        set
        {
            this.windComponentField = value;
        }
    }

    /// <remarks/>
    public byte MinimumEnrouteAltitude
    {
        get
        {
            return this.minimumEnrouteAltitudeField;
        }
        set
        {
            this.minimumEnrouteAltitudeField = value;
        }
    }

    /// <remarks/>
    public ushort MagneticHeading
    {
        get
        {
            return this.magneticHeadingField;
        }
        set
        {
            this.magneticHeadingField = value;
        }
    }

    /// <remarks/>
    public ushort TrueHeading
    {
        get
        {
            return this.trueHeadingField;
        }
        set
        {
            this.trueHeadingField = value;
        }
    }

    /// <remarks/>
    public ushort MagneticTrack
    {
        get
        {
            return this.magneticTrackField;
        }
        set
        {
            this.magneticTrackField = value;
        }
    }

    /// <remarks/>
    public ushort TrueTrack
    {
        get
        {
            return this.trueTrackField;
        }
        set
        {
            this.trueTrackField = value;
        }
    }

    /// <remarks/>
    public string ClimbDescent
    {
        get
        {
            return this.climbDescentField;
        }
        set
        {
            this.climbDescentField = value;
        }
    }

    /// <remarks/>
    public decimal LegFuel
    {
        get
        {
            return this.legFuelField;
        }
        set
        {
            this.legFuelField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightRouteStrings
{

    private string toDestField;

    private string toAlt1Field;

    private string toAlt2Field;

    private object tOAltField;

    /// <remarks/>
    public string ToDest
    {
        get
        {
            return this.toDestField;
        }
        set
        {
            this.toDestField = value;
        }
    }

    /// <remarks/>
    public string ToAlt1
    {
        get
        {
            return this.toAlt1Field;
        }
        set
        {
            this.toAlt1Field = value;
        }
    }

    /// <remarks/>
    public string ToAlt2
    {
        get
        {
            return this.toAlt2Field;
        }
        set
        {
            this.toAlt2Field = value;
        }
    }

    /// <remarks/>
    public object TOAlt
    {
        get
        {
            return this.tOAltField;
        }
        set
        {
            this.tOAltField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightEtopsInformation
{

    private byte ruleTimeUsedField;

    private byte icingPercentageField;

    /// <remarks/>
    public byte RuleTimeUsed
    {
        get
        {
            return this.ruleTimeUsedField;
        }
        set
        {
            this.ruleTimeUsedField = value;
        }
    }

    /// <remarks/>
    public byte IcingPercentage
    {
        get
        {
            return this.icingPercentageField;
        }
        set
        {
            this.icingPercentageField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightCorrectionTable
{

    private sbyte ctIDField;

    private ushort flightlevelField;

    private sbyte windCorrectionField;

    private byte timeInMinutesForCruiseProfileField;

    private decimal timeInHoursMinutesForCruiseProfileField;

    private object timeInHoursMinutesForAltCruiseProfileField;

    private object timeInMinutesForAltCruiseProfileField;

    private byte timeInMinutesForXProfileField;

    private object timeInHoursMinutesForXProfileField;

    private decimal fuelForSelectedProfileField;

    private decimal fuelForSecondProfileField;

    private decimal fuelForXProfileField;

    private byte differentialCostField;

    private decimal totalFuelIncreaseWith10ktWindField;

    /// <remarks/>
    public sbyte CtID
    {
        get
        {
            return this.ctIDField;
        }
        set
        {
            this.ctIDField = value;
        }
    }

    /// <remarks/>
    public ushort Flightlevel
    {
        get
        {
            return this.flightlevelField;
        }
        set
        {
            this.flightlevelField = value;
        }
    }

    /// <remarks/>
    public sbyte WindCorrection
    {
        get
        {
            return this.windCorrectionField;
        }
        set
        {
            this.windCorrectionField = value;
        }
    }

    /// <remarks/>
    public byte TimeInMinutesForCruiseProfile
    {
        get
        {
            return this.timeInMinutesForCruiseProfileField;
        }
        set
        {
            this.timeInMinutesForCruiseProfileField = value;
        }
    }

    /// <remarks/>
    public decimal TimeInHoursMinutesForCruiseProfile
    {
        get
        {
            return this.timeInHoursMinutesForCruiseProfileField;
        }
        set
        {
            this.timeInHoursMinutesForCruiseProfileField = value;
        }
    }

    /// <remarks/>
    public object TimeInHoursMinutesForAltCruiseProfile
    {
        get
        {
            return this.timeInHoursMinutesForAltCruiseProfileField;
        }
        set
        {
            this.timeInHoursMinutesForAltCruiseProfileField = value;
        }
    }

    /// <remarks/>
    public object TimeInMinutesForAltCruiseProfile
    {
        get
        {
            return this.timeInMinutesForAltCruiseProfileField;
        }
        set
        {
            this.timeInMinutesForAltCruiseProfileField = value;
        }
    }

    /// <remarks/>
    public byte TimeInMinutesForXProfile
    {
        get
        {
            return this.timeInMinutesForXProfileField;
        }
        set
        {
            this.timeInMinutesForXProfileField = value;
        }
    }

    /// <remarks/>
    public object TimeInHoursMinutesForXProfile
    {
        get
        {
            return this.timeInHoursMinutesForXProfileField;
        }
        set
        {
            this.timeInHoursMinutesForXProfileField = value;
        }
    }

    /// <remarks/>
    public decimal FuelForSelectedProfile
    {
        get
        {
            return this.fuelForSelectedProfileField;
        }
        set
        {
            this.fuelForSelectedProfileField = value;
        }
    }

    /// <remarks/>
    public decimal FuelForSecondProfile
    {
        get
        {
            return this.fuelForSecondProfileField;
        }
        set
        {
            this.fuelForSecondProfileField = value;
        }
    }

    /// <remarks/>
    public decimal FuelForXProfile
    {
        get
        {
            return this.fuelForXProfileField;
        }
        set
        {
            this.fuelForXProfileField = value;
        }
    }

    /// <remarks/>
    public byte DifferentialCost
    {
        get
        {
            return this.differentialCostField;
        }
        set
        {
            this.differentialCostField = value;
        }
    }

    /// <remarks/>
    public decimal TotalFuelIncreaseWith10ktWind
    {
        get
        {
            return this.totalFuelIncreaseWith10ktWindField;
        }
        set
        {
            this.totalFuelIncreaseWith10ktWindField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightSidAndStarProcedures
{

    private FlightSidAndStarProceduresSid sidField;

    private FlightSidAndStarProceduresStar starField;

    /// <remarks/>
    public FlightSidAndStarProceduresSid Sid
    {
        get
        {
            return this.sidField;
        }
        set
        {
            this.sidField = value;
        }
    }

    /// <remarks/>
    public FlightSidAndStarProceduresStar Star
    {
        get
        {
            return this.starField;
        }
        set
        {
            this.starField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightSidAndStarProceduresSid
{

    private string nameField;

    private string infoField;

    /// <remarks/>
    public string Name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    public string Info
    {
        get
        {
            return this.infoField;
        }
        set
        {
            this.infoField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightSidAndStarProceduresStar
{

    private string nameField;

    private string infoField;

    /// <remarks/>
    public string Name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    public string Info
    {
        get
        {
            return this.infoField;
        }
        set
        {
            this.infoField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoad
{

    private FlightLoadFuel fuelField;

    private FlightLoadCargo cargoField;

    private FlightLoadPax paxField;

    private FlightLoadMassBalance massBalanceField;

    private FlightLoadPayload payloadField;

    private FlightLoadDryOperating dryOperatingField;

    /// <remarks/>
    public FlightLoadFuel Fuel
    {
        get
        {
            return this.fuelField;
        }
        set
        {
            this.fuelField = value;
        }
    }

    /// <remarks/>
    public FlightLoadCargo Cargo
    {
        get
        {
            return this.cargoField;
        }
        set
        {
            this.cargoField = value;
        }
    }

    /// <remarks/>
    public FlightLoadPax Pax
    {
        get
        {
            return this.paxField;
        }
        set
        {
            this.paxField = value;
        }
    }

    /// <remarks/>
    public FlightLoadMassBalance MassBalance
    {
        get
        {
            return this.massBalanceField;
        }
        set
        {
            this.massBalanceField = value;
        }
    }

    /// <remarks/>
    public FlightLoadPayload Payload
    {
        get
        {
            return this.payloadField;
        }
        set
        {
            this.payloadField = value;
        }
    }

    /// <remarks/>
    public FlightLoadDryOperating DryOperating
    {
        get
        {
            return this.dryOperatingField;
        }
        set
        {
            this.dryOperatingField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadFuel
{

    private ushort actTotalField;

    private FlightLoadFuelLoadFuel loadFuelField;

    /// <remarks/>
    public ushort ActTotal
    {
        get
        {
            return this.actTotalField;
        }
        set
        {
            this.actTotalField = value;
        }
    }

    /// <remarks/>
    public FlightLoadFuelLoadFuel LoadFuel
    {
        get
        {
            return this.loadFuelField;
        }
        set
        {
            this.loadFuelField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadFuelLoadFuel
{

    private FlightLoadFuelLoadFuelLoadFuelSection loadFuelSectionField;

    /// <remarks/>
    public FlightLoadFuelLoadFuelLoadFuelSection LoadFuelSection
    {
        get
        {
            return this.loadFuelSectionField;
        }
        set
        {
            this.loadFuelSectionField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadFuelLoadFuelLoadFuelSection
{

    private ushort actMassField;

    private string idField;

    /// <remarks/>
    public ushort ActMass
    {
        get
        {
            return this.actMassField;
        }
        set
        {
            this.actMassField = value;
        }
    }

    /// <remarks/>
    public string ID
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadCargo
{

    private ushort actTotalField;

    private FlightLoadCargoLoadCargo loadCargoField;

    /// <remarks/>
    public ushort ActTotal
    {
        get
        {
            return this.actTotalField;
        }
        set
        {
            this.actTotalField = value;
        }
    }

    /// <remarks/>
    public FlightLoadCargoLoadCargo LoadCargo
    {
        get
        {
            return this.loadCargoField;
        }
        set
        {
            this.loadCargoField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadCargoLoadCargo
{

    private FlightLoadCargoLoadCargoLoadCargoSection loadCargoSectionField;

    /// <remarks/>
    public FlightLoadCargoLoadCargoLoadCargoSection LoadCargoSection
    {
        get
        {
            return this.loadCargoSectionField;
        }
        set
        {
            this.loadCargoSectionField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadCargoLoadCargoLoadCargoSection
{

    private ushort actMassField;

    private string idField;

    /// <remarks/>
    public ushort ActMass
    {
        get
        {
            return this.actMassField;
        }
        set
        {
            this.actMassField = value;
        }
    }

    /// <remarks/>
    public string ID
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadPax
{

    private byte totalField;

    private FlightLoadPaxPaxData paxDataField;

    private FlightLoadPaxPaxSections paxSectionsField;

    /// <remarks/>
    public byte Total
    {
        get
        {
            return this.totalField;
        }
        set
        {
            this.totalField = value;
        }
    }

    /// <remarks/>
    public FlightLoadPaxPaxData PaxData
    {
        get
        {
            return this.paxDataField;
        }
        set
        {
            this.paxDataField = value;
        }
    }

    /// <remarks/>
    public FlightLoadPaxPaxSections PaxSections
    {
        get
        {
            return this.paxSectionsField;
        }
        set
        {
            this.paxSectionsField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadPaxPaxData
{

    private byte maxPaxField;

    private byte actPaxField;

    private byte actMassField;

    private byte paxAmountField;

    private byte maleField;

    private byte femaleField;

    private byte childrenField;

    private byte infantField;

    private byte custMassField;

    /// <remarks/>
    public byte MaxPax
    {
        get
        {
            return this.maxPaxField;
        }
        set
        {
            this.maxPaxField = value;
        }
    }

    /// <remarks/>
    public byte ActPax
    {
        get
        {
            return this.actPaxField;
        }
        set
        {
            this.actPaxField = value;
        }
    }

    /// <remarks/>
    public byte ActMass
    {
        get
        {
            return this.actMassField;
        }
        set
        {
            this.actMassField = value;
        }
    }

    /// <remarks/>
    public byte PaxAmount
    {
        get
        {
            return this.paxAmountField;
        }
        set
        {
            this.paxAmountField = value;
        }
    }

    /// <remarks/>
    public byte Male
    {
        get
        {
            return this.maleField;
        }
        set
        {
            this.maleField = value;
        }
    }

    /// <remarks/>
    public byte Female
    {
        get
        {
            return this.femaleField;
        }
        set
        {
            this.femaleField = value;
        }
    }

    /// <remarks/>
    public byte Children
    {
        get
        {
            return this.childrenField;
        }
        set
        {
            this.childrenField = value;
        }
    }

    /// <remarks/>
    public byte Infant
    {
        get
        {
            return this.infantField;
        }
        set
        {
            this.infantField = value;
        }
    }

    /// <remarks/>
    public byte CustMass
    {
        get
        {
            return this.custMassField;
        }
        set
        {
            this.custMassField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadPaxPaxSections
{

    private FlightLoadPaxPaxSectionsSimplePaxSection simplePaxSectionField;

    /// <remarks/>
    public FlightLoadPaxPaxSectionsSimplePaxSection SimplePaxSection
    {
        get
        {
            return this.simplePaxSectionField;
        }
        set
        {
            this.simplePaxSectionField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadPaxPaxSectionsSimplePaxSection
{

    private string rowField;

    private byte actPaxField;

    private byte actMassField;

    private byte maleField;

    private byte femaleField;

    private byte childrenField;

    private byte infantField;

    private object custMassField;

    /// <remarks/>
    public string Row
    {
        get
        {
            return this.rowField;
        }
        set
        {
            this.rowField = value;
        }
    }

    /// <remarks/>
    public byte ActPax
    {
        get
        {
            return this.actPaxField;
        }
        set
        {
            this.actPaxField = value;
        }
    }

    /// <remarks/>
    public byte ActMass
    {
        get
        {
            return this.actMassField;
        }
        set
        {
            this.actMassField = value;
        }
    }

    /// <remarks/>
    public byte Male
    {
        get
        {
            return this.maleField;
        }
        set
        {
            this.maleField = value;
        }
    }

    /// <remarks/>
    public byte Female
    {
        get
        {
            return this.femaleField;
        }
        set
        {
            this.femaleField = value;
        }
    }

    /// <remarks/>
    public byte Children
    {
        get
        {
            return this.childrenField;
        }
        set
        {
            this.childrenField = value;
        }
    }

    /// <remarks/>
    public byte Infant
    {
        get
        {
            return this.infantField;
        }
        set
        {
            this.infantField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
    public object CustMass
    {
        get
        {
            return this.custMassField;
        }
        set
        {
            this.custMassField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadMassBalance
{

    private FlightLoadMassBalanceTakeoff takeoffField;

    private FlightLoadMassBalanceLanding landingField;

    private FlightLoadMassBalanceZeroFuel zeroFuelField;

    private FlightLoadMassBalanceIndex indexField;

    /// <remarks/>
    public FlightLoadMassBalanceTakeoff Takeoff
    {
        get
        {
            return this.takeoffField;
        }
        set
        {
            this.takeoffField = value;
        }
    }

    /// <remarks/>
    public FlightLoadMassBalanceLanding Landing
    {
        get
        {
            return this.landingField;
        }
        set
        {
            this.landingField = value;
        }
    }

    /// <remarks/>
    public FlightLoadMassBalanceZeroFuel ZeroFuel
    {
        get
        {
            return this.zeroFuelField;
        }
        set
        {
            this.zeroFuelField = value;
        }
    }

    /// <remarks/>
    public FlightLoadMassBalanceIndex Index
    {
        get
        {
            return this.indexField;
        }
        set
        {
            this.indexField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadMassBalanceTakeoff
{

    private byte forwardLimitField;

    private byte actualPositionField;

    private byte aftLimitField;

    /// <remarks/>
    public byte ForwardLimit
    {
        get
        {
            return this.forwardLimitField;
        }
        set
        {
            this.forwardLimitField = value;
        }
    }

    /// <remarks/>
    public byte ActualPosition
    {
        get
        {
            return this.actualPositionField;
        }
        set
        {
            this.actualPositionField = value;
        }
    }

    /// <remarks/>
    public byte AftLimit
    {
        get
        {
            return this.aftLimitField;
        }
        set
        {
            this.aftLimitField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadMassBalanceLanding
{

    private byte forwardLimitField;

    private byte actualPositionField;

    private byte aftLimitField;

    /// <remarks/>
    public byte ForwardLimit
    {
        get
        {
            return this.forwardLimitField;
        }
        set
        {
            this.forwardLimitField = value;
        }
    }

    /// <remarks/>
    public byte ActualPosition
    {
        get
        {
            return this.actualPositionField;
        }
        set
        {
            this.actualPositionField = value;
        }
    }

    /// <remarks/>
    public byte AftLimit
    {
        get
        {
            return this.aftLimitField;
        }
        set
        {
            this.aftLimitField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadMassBalanceZeroFuel
{

    private byte forwardLimitField;

    private byte actualPositionField;

    private byte aftLimitField;

    /// <remarks/>
    public byte ForwardLimit
    {
        get
        {
            return this.forwardLimitField;
        }
        set
        {
            this.forwardLimitField = value;
        }
    }

    /// <remarks/>
    public byte ActualPosition
    {
        get
        {
            return this.actualPositionField;
        }
        set
        {
            this.actualPositionField = value;
        }
    }

    /// <remarks/>
    public byte AftLimit
    {
        get
        {
            return this.aftLimitField;
        }
        set
        {
            this.aftLimitField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadMassBalanceIndex
{

    private byte dryOperatingIndexField;

    private byte zeroFuelForwardLimitField;

    private byte zeroFuelWeightIndexField;

    private byte zeroFuelAftLimitField;

    /// <remarks/>
    public byte DryOperatingIndex
    {
        get
        {
            return this.dryOperatingIndexField;
        }
        set
        {
            this.dryOperatingIndexField = value;
        }
    }

    /// <remarks/>
    public byte ZeroFuelForwardLimit
    {
        get
        {
            return this.zeroFuelForwardLimitField;
        }
        set
        {
            this.zeroFuelForwardLimitField = value;
        }
    }

    /// <remarks/>
    public byte ZeroFuelWeightIndex
    {
        get
        {
            return this.zeroFuelWeightIndexField;
        }
        set
        {
            this.zeroFuelWeightIndexField = value;
        }
    }

    /// <remarks/>
    public byte ZeroFuelAftLimit
    {
        get
        {
            return this.zeroFuelAftLimitField;
        }
        set
        {
            this.zeroFuelAftLimitField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadPayload
{

    private ushort maxPayloadField;

    private ushort mzfmField;

    private ushort mtomField;

    private ushort mlmField;

    private ushort mrmpField;

    private ushort maxCargoField;

    /// <remarks/>
    public ushort MaxPayload
    {
        get
        {
            return this.maxPayloadField;
        }
        set
        {
            this.maxPayloadField = value;
        }
    }

    /// <remarks/>
    public ushort Mzfm
    {
        get
        {
            return this.mzfmField;
        }
        set
        {
            this.mzfmField = value;
        }
    }

    /// <remarks/>
    public ushort Mtom
    {
        get
        {
            return this.mtomField;
        }
        set
        {
            this.mtomField = value;
        }
    }

    /// <remarks/>
    public ushort Mlm
    {
        get
        {
            return this.mlmField;
        }
        set
        {
            this.mlmField = value;
        }
    }

    /// <remarks/>
    public ushort Mrmp
    {
        get
        {
            return this.mrmpField;
        }
        set
        {
            this.mrmpField = value;
        }
    }

    /// <remarks/>
    public ushort MaxCargo
    {
        get
        {
            return this.maxCargoField;
        }
        set
        {
            this.maxCargoField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightLoadDryOperating
{

    private byte basicEmptyArmField;

    private ushort basicEmptyWeightField;

    private byte dryOperatingMassArmField;

    private uint dryOperatingWeightField;

    /// <remarks/>
    public byte BasicEmptyArm
    {
        get
        {
            return this.basicEmptyArmField;
        }
        set
        {
            this.basicEmptyArmField = value;
        }
    }

    /// <remarks/>
    public ushort BasicEmptyWeight
    {
        get
        {
            return this.basicEmptyWeightField;
        }
        set
        {
            this.basicEmptyWeightField = value;
        }
    }

    /// <remarks/>
    public byte DryOperatingMassArm
    {
        get
        {
            return this.dryOperatingMassArmField;
        }
        set
        {
            this.dryOperatingMassArmField = value;
        }
    }

    /// <remarks/>
    public uint DryOperatingWeight
    {
        get
        {
            return this.dryOperatingWeightField;
        }
        set
        {
            this.dryOperatingWeightField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightAircraftConfiguration
{

    private string nameField;

    private FlightAircraftConfigurationCrew crewField;

    /// <remarks/>
    public string Name
    {
        get
        {
            return this.nameField;
        }
        set
        {
            this.nameField = value;
        }
    }

    /// <remarks/>
    public FlightAircraftConfigurationCrew Crew
    {
        get
        {
            return this.crewField;
        }
        set
        {
            this.crewField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightAircraftConfigurationCrew
{

    private FlightAircraftConfigurationCrewCrew crewField;

    /// <remarks/>
    public FlightAircraftConfigurationCrewCrew Crew
    {
        get
        {
            return this.crewField;
        }
        set
        {
            this.crewField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightAircraftConfigurationCrewCrew
{

    private object idField;

    private string crewTypeField;

    private string crewNameField;

    private string initialsField;

    private object gSMField;

    private object massField;

    /// <remarks/>
    public object ID
    {
        get
        {
            return this.idField;
        }
        set
        {
            this.idField = value;
        }
    }

    /// <remarks/>
    public string CrewType
    {
        get
        {
            return this.crewTypeField;
        }
        set
        {
            this.crewTypeField = value;
        }
    }

    /// <remarks/>
    public string CrewName
    {
        get
        {
            return this.crewNameField;
        }
        set
        {
            this.crewNameField = value;
        }
    }

    /// <remarks/>
    public string Initials
    {
        get
        {
            return this.initialsField;
        }
        set
        {
            this.initialsField = value;
        }
    }

    /// <remarks/>
    public object GSM
    {
        get
        {
            return this.gSMField;
        }
        set
        {
            this.gSMField = value;
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
    public object Mass
    {
        get
        {
            return this.massField;
        }
        set
        {
            this.massField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightPpsVersionInformation
{

    private string ppsApplicationVersionField;

    private string ppsExeVersionField;

    /// <remarks/>
    public string PpsApplicationVersion
    {
        get
        {
            return this.ppsApplicationVersionField;
        }
        set
        {
            this.ppsApplicationVersionField = value;
        }
    }

    /// <remarks/>
    public string PpsExeVersion
    {
        get
        {
            return this.ppsExeVersionField;
        }
        set
        {
            this.ppsExeVersionField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightCustomReferences
{

    private object refIDField;

    private object milIDField;

    /// <remarks/>
    public object RefID
    {
        get
        {
            return this.refIDField;
        }
        set
        {
            this.refIDField = value;
        }
    }

    /// <remarks/>
    public object MilID
    {
        get
        {
            return this.milIDField;
        }
        set
        {
            this.milIDField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightExtraFuel
{

    private string typeField;

    private ushort fuelField;

    private string timeField;

    /// <remarks/>
    public string Type
    {
        get
        {
            return this.typeField;
        }
        set
        {
            this.typeField = value;
        }
    }

    /// <remarks/>
    public ushort Fuel
    {
        get
        {
            return this.fuelField;
        }
        set
        {
            this.fuelField = value;
        }
    }

    /// <remarks/>
    public string Time
    {
        get
        {
            return this.timeField;
        }
        set
        {
            this.timeField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightAirportWeatherData
{

    private string iCAOField;

    private FlightAirportWeatherDataTAF tAFField;

    private FlightAirportWeatherDataMetar metarField;

    /// <remarks/>
    public string ICAO
    {
        get
        {
            return this.iCAOField;
        }
        set
        {
            this.iCAOField = value;
        }
    }

    /// <remarks/>
    public FlightAirportWeatherDataTAF TAF
    {
        get
        {
            return this.tAFField;
        }
        set
        {
            this.tAFField = value;
        }
    }

    /// <remarks/>
    public FlightAirportWeatherDataMetar Metar
    {
        get
        {
            return this.metarField;
        }
        set
        {
            this.metarField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightAirportWeatherDataTAF
{

    private string typeField;

    private string textField;

    private string iCAOField;

    private System.DateTime forecastTimeField;

    private System.DateTime forecastStartTimeField;

    private System.DateTime forecastEndTimeField;

    /// <remarks/>
    public string Type
    {
        get
        {
            return this.typeField;
        }
        set
        {
            this.typeField = value;
        }
    }

    /// <remarks/>
    public string Text
    {
        get
        {
            return this.textField;
        }
        set
        {
            this.textField = value;
        }
    }

    /// <remarks/>
    public string ICAO
    {
        get
        {
            return this.iCAOField;
        }
        set
        {
            this.iCAOField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ForecastTime
    {
        get
        {
            return this.forecastTimeField;
        }
        set
        {
            this.forecastTimeField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ForecastStartTime
    {
        get
        {
            return this.forecastStartTimeField;
        }
        set
        {
            this.forecastStartTimeField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ForecastEndTime
    {
        get
        {
            return this.forecastEndTimeField;
        }
        set
        {
            this.forecastEndTimeField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class FlightAirportWeatherDataMetar
{

    private string textField;

    private string iCAOField;

    private System.DateTime observationTimeField;

    private string observationTypeField;

    /// <remarks/>
    public string Text
    {
        get
        {
            return this.textField;
        }
        set
        {
            this.textField = value;
        }
    }

    /// <remarks/>
    public string ICAO
    {
        get
        {
            return this.iCAOField;
        }
        set
        {
            this.iCAOField = value;
        }
    }

    /// <remarks/>
    public System.DateTime ObservationTime
    {
        get
        {
            return this.observationTimeField;
        }
        set
        {
            this.observationTimeField = value;
        }
    }

    /// <remarks/>
    public string ObservationType
    {
        get
        {
            return this.observationTypeField;
        }
        set
        {
            this.observationTypeField = value;
        }
    }
}


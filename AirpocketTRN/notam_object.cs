using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AirpocketTRN
{
    public class Notam
    {
        public string facilityDesignator { get; set; }
        public string notamNumber { get; set; }
        public string featureName { get; set; }
        public string issueDate { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string source { get; set; }
        public string sourceType { get; set; }
        public string icaoMessage { get; set; }
        public string traditionalMessage { get; set; }
        public string plainLanguageMessage { get; set; }
        public string traditionalMessageFrom4thWord { get; set; }
        public string icaoId { get; set; }
        public string accountId { get; set; }
        public string airportName { get; set; }
        public bool procedure { get; set; }
        public int userID { get; set; }
        public int transactionID { get; set; }
        public bool cancelledOrExpired { get; set; }
        public bool digitalTppLink { get; set; }
        public string status { get; set; }
        public bool contractionsExpandedForPlainLanguage { get; set; }
        public string keyword { get; set; }
        public bool snowtam { get; set; }
        public string geometry { get; set; }
        public bool digitallyTransformed { get; set; }
        public string messageDisplayed { get; set; }
        public bool hasHistory { get; set; }
        public bool moreThan300Chars { get; set; }
        public bool showingFullText { get; set; }
        public int locID { get; set; }
        public bool defaultIcao { get; set; }
        public int crossoverTransactionID { get; set; }
        public string crossoverAccountID { get; set; }
        public string mapPointer { get; set; }
        public int requestID { get; set; }
    }

    public class NotamResponse
    {
        public List<Notam> notamList { get; set; }
        public int startRecordCount { get; set; }
        public int endRecordCount { get; set; }
        public int totalNotamCount { get; set; }
        public int filteredResultCount { get; set; }
        public string criteriaCaption { get; set; }
        public string searchDateTime { get; set; }
        public string linkedLocationCaption { get; set; }
        public string error { get; set; }
        public List<CountByType> countsByType { get; set; }
        public int requestID { get; set; }
    }

    public class CountByType
    {
        public string name { get; set; }
        public int value { get; set; }
    }

}
using System.Collections.Generic;
using System;

public class Magfa
{
    string username = "flypersia_48000"; //"caspianline";// "varesh_85972"; //; //"atlas_82173";// "varesh_85972"; //ConfigurationManager.AppSettings["magfa_user"];/* "taban";*/ //ConfigurationManager.AppSettings["magfa_user"]; //"caspianline"; //"flypersia_48000";
    string password = "YYDWMU5BAJQQHCuG"; //"oJTyaWoLnQycrfdX"; //"ZQMihTmdLqCbnbrW";// "Ux4D3tBnYvOlZGd6";// //"YYDWMU5BAJQQHCuG"; //"Ot9V0CWKbH1HUQGK";// "oJTyaWoLnQycrfdX"; //ConfigurationManager.AppSettings["magfa_pass"];/*"ZIECXHgRSJT1QLMy";*/ //ConfigurationManager.AppSettings["magfa_pass"]; // "ZQMihTmdLqCbnbrW"; //"YYDWMU5BAJQQHCuG";
    string domain = "magfa"; //"varesh"; //"http://atlasairplus.com/";// "magfa";// "tabanair"; /*"tabanair";*/
    string senderNumber = "300048000"; //"3000748907"; //"300085972";//"300048000"; ////"300085972";// ConfigurationManager.AppSettings["magfa_no"]; /*"30006327";*/ // ConfigurationManager.AppSettings["magfa_no"]; // "3000748907"; //"300048000";
    public List<string> getStatus(List<Int64> refIds)
    {

        ApiAPSB.com.magfa.sms.SoapSmsQueuableImplementationService sq = new ApiAPSB.com.magfa.sms.SoapSmsQueuableImplementationService();
        sq.Credentials = new System.Net.NetworkCredential(username, password);
        sq.PreAuthenticate = true;

        //List<string> result = new List<string>();
        //foreach (var x in refIds)
        //{
        //    var str = "Unknown";
        //    var response = sq.getMessageStatus(x);
        //    switch (response)
        //    {
        //        case 1:
        //            str = "Sending";
        //            break;
        //        case 2:
        //            str = "Delivered";
        //            break;
        //        case 3:
        //            str = "Not Delivered";
        //            break;


        //        default:
        //            break;
        //    }
        //    result.Add(str);
        //}



        var response = sq.getRealMessageStatuses(refIds.ToArray());
        List<string> result = new List<string>();
        foreach (var x in response)
        {
            var str = "Unknown";
            switch (x)
            {
                case 1:
                    str = "Delivered";
                    break;
                case 2:
                    str = "Not Delivered To Phone";
                    break;
                case 8:
                    str = "Delivered To ICT";
                    break;
                case 16:
                    str = "Not Delivered To ICT";
                    break;
                case 0:
                    str = "Sending Queue";
                    break;
                default:
                    break;
            }
            result.Add(str);
        }


        return result;
    }
    public string getStatus(Int64 refid)
    {
        try
        {
            ApiAPSB.com.magfa.sms.SoapSmsQueuableImplementationService sq = new ApiAPSB.com.magfa.sms.SoapSmsQueuableImplementationService();
            sq.Credentials = new System.Net.NetworkCredential(username, password);
            sq.PreAuthenticate = true;

            var response = sq.getMessageStatus(refid);


            var str = "Unknown";
            switch (response)
            {
                case 1:
                    str = "Delivered";
                    break;
                case 2:
                    str = "Not Delivered To Phone";
                    break;
                case 8:
                    str = "Delivered To ICT";
                    break;
                case 16:
                    str = "Not Delivered To ICT";
                    break;
                case 0:
                    str = "Sending Queue";
                    break;
                default:
                    break;
            }




            return str;
        }
        catch (Exception ex)
        {
            return "UNKNOWN-ERROR";
        }

    }
    public long[] enqueue(int count, String recipientNumber, String text)
    {
        try
        {
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            //com.

            ApiAPSB.com.magfa.sms.SoapSmsQueuableImplementationService sq = new ApiAPSB.com.magfa.sms.SoapSmsQueuableImplementationService();
            //if (useProxy)
            //{
            //    WebProxy proxy;
            //    proxy = new WebProxy(proxyAddress);
            //    proxy.Credentials = new NetworkCredential(proxyUsername, proxyPassword);
            //    sq.Proxy = proxy;
            //}
            sq.Credentials = new System.Net.NetworkCredential(username, password);
            sq.PreAuthenticate = true;
            long[] results;

            string[] messages;
            string[] mobiles;
            string[] origs;

            int[] encodings;
            string[] UDH;
            int[] mclass;
            int[] priorities;
            long[] checkingIds;

            messages = new string[count];
            mobiles = new string[count];
            origs = new string[count];

            encodings = new int[count];
            UDH = new string[count];
            mclass = new int[count];
            priorities = new int[count];
            checkingIds = new long[count];

            /*
            encodings = null;
            UDH = null;
            mclass = null;
            priorities = null;
            checkingIds = null;
            */
            for (int i = 0; i < count; i++)
            {
                messages[i] = text;
                mobiles[i] = recipientNumber;
                origs[i] = senderNumber;

                encodings[i] = -1;
                UDH[i] = "";
                mclass[i] = -1;
                priorities[i] = -1;
                checkingIds[i] = 200 + i;
            }
            var xxx = sq.Url;
            return sq.enqueue(domain, messages, mobiles, origs, encodings, UDH, mclass, priorities, checkingIds);


            ////////////////////////////////
            /////kakoli
            //// Credentials


            //// Service (Add a Web Reference)
            //com.magfa.sms.SoapSmsQueuableImplementationService service = new com.magfa.sms.SoapSmsQueuableImplementationService();

            //// Basic Auth
            //NetworkCredential netCredential = new NetworkCredential(username, password);
            //Uri uri = new Uri(service.Url);
            //ICredentials credentials = netCredential.GetCredential(uri, "Basic");

            //service.Credentials = credentials;
            //service.AllowAutoRedirect = true;

            //// Call
            //long[] resp = service.enqueue(domain,
            //    new string[] { "تست ارسال پيامک. Sample Text for test.", "Hi!" },
            //    new string[] { "09124449584", "09306678047" },
            //    new string[] { senderNumber },
            //    new int[] { 0 },
            //    new string[] { "" },
            //    new int[] { 0 },
            //    new int[] { 0 },
            //    new long[] { 198981, 123032 }
            //);
            //foreach (long r in resp)
            //{
            //    Console.WriteLine("send: " + r);
            //}
            //return resp;
            //////////////////////////////////////////
        }
        catch (Exception ex)
        {
            return new long[] { -1 };
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ApiXLS.Controllers
{
    public class IrimoController : ApiController
    {
        private static readonly HttpClient client = new HttpClient();

        public class TAF
        {
            public string Station { get; set; }
            public string Taf { get; set; }
        }

        public class METAR
        {
            public string Station { get; set; }
            public string Metar { get; set; }
        }

        [Route("api/get/irimo")]
        [HttpGet]
        public async Task<IHttpActionResult> GetIrimoTafs()
        {
            string url = "https://irimo.ir/far/index.php?module=pmk&type=user&func=showBlock&bid=588&standalone=1";
            List<TAF> tafList = new List<TAF>();

            try
            {
                // Send an HTTP GET request to the URL
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseData = await response.Content.ReadAsStringAsync();

                // Regular expression to find the sections containing station and TAF data
                var stationRegex = new Regex(@"<div[^>]*?>(?<station>O[A-Z]{3}.*?)<\/div>", RegexOptions.Singleline);
                var tafRegex = new Regex(@"<div[^>]*?>\s*TAF\s(?<taf>O[A-Z]{3}.*?)<\/div>", RegexOptions.Singleline);

                // Extract matches for stations and TAFs
                MatchCollection stationMatches = stationRegex.Matches(responseData);
                MatchCollection tafMatches = tafRegex.Matches(responseData);

                for (int i = 0; i < stationMatches.Count; i++)
                {
                    // Check if corresponding TAF exists to avoid index out-of-bounds exceptions
                    if (i < tafMatches.Count)
                    {
                        TAF tafEntry = new TAF
                        {
                            Station = stationMatches[i].Groups["station"].Value.Trim(),
                            Taf = tafMatches[i].Groups["taf"].Value.Trim()
                        };
                        tafList.Add(tafEntry);
                    }
                }

                return Ok(tafList);
            }
            catch (HttpRequestException ex)
            {
                // Log the error and return an appropriate error message
                Console.WriteLine($"HTTP Request Error: {ex.Message}");
                return InternalServerError(ex);
            }
            catch (Exception ex)
            {
                // Log the error and return an appropriate error message
                Console.WriteLine($"Error occurred while fetching TAF data: {ex.Message}");
                return InternalServerError(ex);
            }
        }


        [Route("api/get/irimo/metar")]
        [HttpGet]
        public async Task<IHttpActionResult> GetIrimoMetar()
        {
            string url = "https://irimo.ir/far/index.php?module=pmk&type=user&func=showBlock&bid=601&standalone=1";
            List<METAR> metarList = new List<METAR>();

            try
            {
                // Send an HTTP GET request to the URL
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseData = await response.Content.ReadAsStringAsync();

                // Regular expression to find the sections containing station and TAF data
                var stationRegex = new Regex(@"<div[^>]*?>(?<station>O[A-Z]{3}.*?)<\/div>", RegexOptions.Singleline);
                var metarRegex = new Regex(@"<div[^>]*?>\s*METAR\s(?<metar>O[A-Z]{3}.*?)<\/div>", RegexOptions.Singleline);

                // Extract matches for stations and TAFs
                MatchCollection stationMatches = stationRegex.Matches(responseData);
                MatchCollection metarMatches = metarRegex.Matches(responseData);

                for (int i = 0; i < stationMatches.Count; i++)
                {
                    // Check if corresponding TAF exists to avoid index out-of-bounds exceptions
                    if (i < metarMatches.Count)
                    {
                        METAR metarEntry = new METAR
                        {
                            Station = stationMatches[i].Groups["station"].Value.Trim(),
                            Metar = metarMatches[i].Groups["metar"].Value.Trim()
                        };
                        metarList.Add(metarEntry);
                    }
                }

                return Ok(metarList);
            }
            catch (HttpRequestException ex)
            {
                // Log the error and return an appropriate error message
                Console.WriteLine($"HTTP Request Error: {ex.Message}");
                return InternalServerError(ex);
            }
            catch (Exception ex)
            {
                // Log the error and return an appropriate error message
                Console.WriteLine($"Error occurred while fetching TAF data: {ex.Message}");
                return InternalServerError(ex);
            }
        }
    }
}

using ApiLibrary.Models;
using ApiLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Validation;
using static ApiLibrary.Controllers.LibraryController;

namespace ApiLibrary.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PIFController : ApiController
    {
        [Route("api/acn/new/{id}")]
        public async Task<IHttpActionResult> GetNewAcn(int id)
        {

            ppa_entities context = new ppa_entities();
            var query = from x in context.ViewBookApplicableEmployees
                        where x.IsVisited==false && x.EmployeeId==id && (x.CategoryId== 10007)
                        select x;

            var result = await query.OrderByDescending(q => q.DateCreate).ToListAsync();


            return Ok(result);
        }
        [Route("api/acn/new/count/{id}")]
        public async Task<IHttpActionResult> GetNewAcnCount(int id)
        {

            ppa_entities context = new ppa_entities();
            var query = from x in context.ViewBookApplicableEmployees
                        where x.IsVisited == false && x.EmployeeId == id && (x.CategoryId == 10007)
                        select x;

            var result = await query.CountAsync();


            return Ok(result);
        }



        //return unitOfWork.BookRepository.GetViewBookApplicableEmployee()
        //           .Where(q => q.IsExposed == 1 && q.EmployeeId == id && q.Category == str)
        //           .OrderBy(q => q.IsVisited)
        //           .ThenBy(q => q.IsSigned)
        //           .ThenBy(q => q.RemainingValid == null ? 10000000 : (q.RemainingValid< 0 ? 20000000 : q.RemainingValid))
        //           .ThenByDescending(q => q.DateRelease);

        [Route("api/acn/all/{id}")]
        public async Task<IHttpActionResult> GetAllAcn(int id)
        {

            ppa_entities context = new ppa_entities();
            var query = from x in context.ViewBookApplicableEmployees
                        where   x.EmployeeId == id && (x.CategoryId == 10007)
                        select x;

            var result = await query.OrderBy (q => q.IsVisited).ThenBy(q=>q.IsSigned).ThenByDescending(q=>q.DateRelease).ToListAsync();


            return Ok(result);
        }







    }
}

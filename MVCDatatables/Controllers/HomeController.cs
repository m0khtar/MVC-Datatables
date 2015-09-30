using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCDatatables.Models;
using System.Text.RegularExpressions;
using System.Collections.Specialized;

namespace MVCDatatables.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCustomers()
        {
            var searchParams = new DataTableParams(HttpContext);

            using (var ctx = new NORTHWNDEntities())
            {
                var allCustomers = (from c in ctx.Customers
                                    select c).ToList();

                var filterdCustomers = allCustomers;


                #region filters

                //name
                if (!string.IsNullOrEmpty(searchParams.customerNameFilter))
                {
                    filterdCustomers = filterdCustomers.Where(c => c.CompanyName.ToUpper().Contains(searchParams.customerNameFilter.ToUpper())).ToList();
                }

                //address

                if (!string.IsNullOrEmpty(searchParams.addressFilter))
                {
                    filterdCustomers = filterdCustomers.Where(c =>
                        (!string.IsNullOrEmpty(c.Address) && c.Address.ToUpper().Contains(searchParams.addressFilter.ToUpper())) ||
                        (!string.IsNullOrEmpty(c.PostalCode) && c.PostalCode.ToUpper().Contains(searchParams.addressFilter.ToUpper())) ||
                        (!string.IsNullOrEmpty(c.City) && c.City.ToUpper().Contains(searchParams.addressFilter.ToUpper())) ||
                        (!string.IsNullOrEmpty(c.Country) && c.Country.ToUpper().Contains(searchParams.addressFilter.ToUpper()))
                        ).ToList();
                }
                //phone
                if (!string.IsNullOrEmpty(searchParams.phoneNumberFilter))
                {
                    filterdCustomers = filterdCustomers.Where(c => c.Phone.ToUpper().Contains(searchParams.phoneNumberFilter.ToUpper())).ToList();
                }
                #endregion

                var displayedCustomers = filterdCustomers.Skip(searchParams.start).Take(searchParams.length);

                var result = from r in displayedCustomers
                             select new
                             {
                                 ID = r.CustomerID,
                                 Name = r.CompanyName,
                                 Address = r.Address + ", " + r.PostalCode + "," + r.City + "," + r.Country,
                                 PhoneNumber = r.Phone
                             };

                Object o = new
                {
                    recordsTotal = allCustomers.Count(),
                    recordsFiltered = filterdCustomers.Count(),
                    aaData = result
                };

                return Json(o);
            }

        }

        public class DataTableParams
        {
            public int start { get; set; }
            public int length { get; set; }
            public string customerNameFilter { get; set; }
            public string addressFilter { get; set; }
            public string phoneNumberFilter { get; set; }


            public DataTableParams(HttpContextBase ctx)
            {
                NameValueCollection nvc = ctx.Request.Form;
                this.start = int.Parse(nvc["start"]);
                this.length = int.Parse(nvc["length"]);
                this.customerNameFilter = nvc["columns[1][search][value]"];
                this.addressFilter = nvc["columns[2][search][value]"];
                this.phoneNumberFilter = nvc["columns[3][search][value]"];
            }
        }
    }
}

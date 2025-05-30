﻿using CrystalDecisions.CrystalReports.Engine;
using SymOrdinary;
using SymRepository.VMS;
using VATViewModel.DTOs;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading.Tasks;
using System.Net.Http;
using Excel.Log.Logger;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SymVATWebUI.Filters;
using VATServer.Library;
using VATServer.Ordinary;
using SymphonySofttech.Utilities;
using System.Text;
using Elmah;


namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class MPLSalesInvSA4Controller : Controller
    {
        ShampanIdentity identity = null;

        MPLSalesInvSA4Repo _repo = null;

        public MPLSalesInvSA4Controller()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new MPLSalesInvSA4Repo(identity);
            }
            catch
            {
                //
            }
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Menu()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult HomeIndex()
        {
            return View();
        }

        [UserFilter]
        public ActionResult Index(MPLSaleInvoiceSA4VM paramVM)
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    //
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }

            paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());

            #region BranchList

            int userId = Convert.ToInt32(identity.UserId);
            var list = new SymRepository.VMS.BranchRepo(identity).UserDropDownBranchProfile(userId);

            var listBranch = new SymRepository.VMS.BranchRepo(identity).SelectAll();

            if (list.Count() == listBranch.Count())
            {
                list.Add(new BranchProfileVM() { BranchID = -1, BranchName = "All" });
            }

            paramVM.BranchList = list;

            #endregion

            return View(paramVM);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamVM param, MPLSaleInvoiceSA4VM paramVM)
        {
            _repo = new MPLSalesInvSA4Repo(identity, Session);

            List<MPLSaleInvoiceSA4VM> getAllData = new List<MPLSaleInvoiceSA4VM>();

            #region Access Controll

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {

                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }

            #endregion


            #region Data Call

            string[] conditionFields;
            string[] conditionValues;

            paramVM.SelectTop = paramVM.SelectTop == null ? "100" : paramVM.SelectTop;
            if (paramVM.BranchId == 0)
            {
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            }

            if (paramVM.BranchId == -1)
            {
                paramVM.BranchId = 0;
            }

            conditionFields = new string[] { "T.TankCode", "C.CustomerName", "TD.SalesInvoiceNo", "TD.BranchId" };
            conditionValues = new string[] { paramVM.TankCode, paramVM.CustomerName, paramVM.SalesInvoiceNo, paramVM.BranchId.ToString() };

            getAllData = _repo.SelectAll(0, conditionFields, conditionValues, null, null,"", paramVM.SelectTop);


            #endregion

            #region Search and Filter Data
            IEnumerable<MPLSaleInvoiceSA4VM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
               
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);


                filteredData = getAllData
                   .Where(c => isSearchable1 && c.CustomerName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.SalesInvoiceNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<MPLSaleInvoiceSA4VM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.CustomerName : sortColumnIndex == 2 && isSortable_2 ? c.SalesInvoiceNo.ToString() :

                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                select new[] { 
                    c.Id+"~"+ c.Post+"~"+ c.SalesInvoiceNo
                    , c.SalesInvoiceNo
                    , c.InvoiceDateTime
                    , c.CustomerName
                    , c.TankCode
                    , c.Status
                };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Create(string tType)
        {
            CommonRepo commonRepo = new CommonRepo(identity, Session);
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            string FormNumeric = commonRepo.settings("DecimalPlace", "FormNumeric");
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    //
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }

            MPLSaleInvoiceSA4VM vm = new MPLSaleInvoiceSA4VM();
            vm.InvoiceDateTime = Session["SessionDate"].ToString();
            vm.Operation = "add";

            return View(vm);
        }
        
        [HttpPost]
        [Authorize]
        public ActionResult CreateEdit(MPLSaleInvoiceSA4VM vm)
        {
            try
            {
                _repo = new MPLSalesInvSA4Repo(identity, Session);
                string[] result = new string[6];

                try
                {
                    string UserId = identity.UserId;
                    int currentBranch = Convert.ToInt32(Session["BranchId"]);
                    vm.BranchId = currentBranch;
                    if (vm.Operation.ToLower() == "add")
                    {
                        vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                        vm.CreatedBy = identity.Name;
                        vm.Post = "N";

                        result = _repo.MPLSalesInvSA4Insert(vm, null, null);

                        if (result[0] == "Success")
                        {
                            Session["result"] = result[0] + "~" + result[1];
                            return RedirectToAction("Edit", new { id = result[4]});
                        }
                        else
                        {
                            string msg = result[1].Split('\r').FirstOrDefault();

                            Session["result"] = result[0] + "~" + msg;

                            return View("Create", vm);
                        }
                    }
                    else if (vm.Operation.ToLower() == "update")
                    {
                        vm.LastModifiedBy = identity.Name;
                        vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");

                        result = _repo.MPLSalesInvSA4Update(vm, null, null);

                        if (result[0] == "Success")
                        {
                            Session["result"] = result[0] + "~" + result[1];
                            return RedirectToAction("Edit", new { id = vm.Id });
                        }
                        else
                        {
                            string msg = result[1].Split('\r').FirstOrDefault();
                            Session["result"] = result[0] + "~" + msg;

                            return View("Create", vm);
                        }
                    }
                    else
                    {
                        return View("Create", vm);
                    }
                }
                catch (Exception ex)
                {
                    string msg = ex.Message.Split('\r').FirstOrDefault();
                    Session["result"] = "Fail~" + msg;
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    return RedirectToAction("Edit", new { id = vm.Id});
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        [ShampanAuthorize]
        [HttpGet]
        public ActionResult Edit(string id)
        {
            MPLSaleInvoiceSA4VM vm = new MPLSaleInvoiceSA4VM();

            try
            {
                _repo = new MPLSalesInvSA4Repo(identity, Session);
                
                vm = _repo.SelectAll(Convert.ToInt32(id), null, null, null, null, null, "100").FirstOrDefault();

                if (vm == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                
                vm.Operation = "update";
                
                return View("Create", vm);
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        [Authorize]
        public ActionResult Post(MPLSaleInvoiceSA4VM vm)
        {
            try
            {
                if (vm.IDs == null)
                {
                    return Json("Already Posted!", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    _repo = new MPLSalesInvSA4Repo(identity, Session);
                    string[] result = new string[6];

                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.LastModifiedBy = identity.Name;

                    result = _repo.SaleInvoiceSA4Post(vm, null, null);
                    return Json(result[1], JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index");
            }
        }

        [ShampanAuthorize]
        [HttpGet]
        public ActionResult Report(string id)
        {
            MPLSaleInvoiceSA4VM vm = new MPLSaleInvoiceSA4VM();

            try
            {
                _repo = new MPLSalesInvSA4Repo(identity, Session);

                vm = _repo.SelectAll(Convert.ToInt32(id), null, null, null, null, null, "100").FirstOrDefault();

                if (vm == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                return View("Report", vm);
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index");
            }
        }
        

    }
}

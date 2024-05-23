using CrystalDecisions.CrystalReports.Engine;
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
    public class MPLTradeChallanController : Controller
    {
        ShampanIdentity identity = null;

        MPLTradeChallanRepo _repo = null;

        public MPLTradeChallanController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new MPLTradeChallanRepo(identity);
            }
            catch
            {

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
        public ActionResult Index(MPLTradeChallanVM paramVM)
        {

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
        public ActionResult _index(JQueryDataTableParamVM param, MPLTradeChallanVM paramVM)
        {
            _repo = new MPLTradeChallanRepo(identity, Session);

            List<MPLTradeChallanVM> getAllData = new List<MPLTradeChallanVM>();

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

            #region SeachParameters

            string dtFrom = null;
            string dtTo = null;
            paramVM.SelectTop = paramVM.SelectTop ==  null ? "100" : paramVM.SelectTop;
            if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
            {
                dtFrom = Convert.ToDateTime(paramVM.FromDate).ToString("yyyyMMdd");
            }
            if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
            {
                dtTo = Convert.ToDateTime(paramVM.ToDate).AddDays(1).ToString("yyyyMMdd");
            }

            if (paramVM.BranchId == 0)
            {
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            }

            if (paramVM.BranchId == -1)
            {
                paramVM.BranchId = 0;
            }

            #endregion SeachParameters

            #region Data Call

            string[] conditionFields;
            string[] conditionValues;

            conditionFields = new string[] { "TRC.TransactionDateTime>=", "TRC.TransactionDateTime<=", "TRC.BranchId" };
            conditionValues = new string[] { dtFrom, dtTo,paramVM.BranchId.ToString() };

            getAllData = _repo.SelectAll(0, conditionFields, conditionValues, null, null, paramVM.SelectTop);


            #endregion

            #region Search and Filter Data
            IEnumerable<MPLTradeChallanVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
               
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);


                filteredData = getAllData
                   .Where(c => isSearchable1 && c.TransactionDateTime.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.Consignee.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<MPLTradeChallanVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.TransactionDateTime : sortColumnIndex == 2 && isSortable_2 ? c.Consignee.ToString() :

                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.Id+"~"+ c.Code+"~"+ c.BranchId
                , c.Code
                , c.TransactionDateTime.ToString()               
                , c.CustomerCode.ToString()               
                , c.CustomerName.ToString()               
                , c.Consignee.ToString()               

             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },
                        JsonRequestBehavior.AllowGet);
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

            MPLTradeChallanVM vm = new MPLTradeChallanVM();
            List<MPLTradeChallanDetilsVM> detailVMs = new List<MPLTradeChallanDetilsVM>();
            vm.MPLTradeChallanDetilsVMs = detailVMs;

            vm.Operation = "add";
            vm.TransactionDateTime = Session["SessionDate"].ToString();
            vm.AgreementDate = Session["SessionDate"].ToString();

            return View(vm);
        }
        
        [HttpPost]
        [Authorize]
        public ActionResult CreateEdit(MPLTradeChallanVM vm)
        {
            try
            {
                _repo = new MPLTradeChallanRepo(identity, Session);
                string[] result = new string[6];
                vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                if (!string.IsNullOrEmpty(vm.TransactionDateTime))
                { vm.TransactionDateTime = Convert.ToDateTime(vm.TransactionDateTime).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"); }
               
                try
                {
                    string UserId = identity.UserId;

                    if (vm.Operation.ToLower() == "add")
                    {
                        vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                        vm.CreatedBy = identity.Name;

                        result = _repo.InsertToMPLTradeChallan(vm, null, null);

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

                        result = _repo.UpdateMPLTradeChallan(vm, null, null);

                        if (result[0] == "Success")
                        {
                            Session["result"] = result[0] + "~" + result[1];
                            return RedirectToAction("Edit", new { id = vm.Id});
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
                    return RedirectToAction("Edit", new { id = vm.Id });
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
            MPLTradeChallanVM vm = new MPLTradeChallanVM();

            try
            {
                _repo = new MPLTradeChallanRepo(identity, Session);
               

                vm = _repo.SelectAll(Convert.ToInt32(id), null, null, null, null, "100").FirstOrDefault();
                if (vm == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                vm.PreviousSalesInvoiceRefId = vm.SalesInvoiceRefId;

                List<MPLTradeChallanDetilsVM> detailVMs = new List<MPLTradeChallanDetilsVM>();
                MPLTradeChallanVM TradeChallanVM = new MPLTradeChallanVM();
                TradeChallanVM.Id = vm.SalesInvoiceRefId;
                TradeChallanVM.SalesInvoiceRefId = vm.SalesInvoiceRefId;
                detailVMs = _repo.GetMPLCreditItemList(TradeChallanVM);
                vm.MPLTradeChallanDetilsVMs = detailVMs;

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
        
        [Authorize(Roles = "Admin")]
        public ActionResult BlankItem(MPLTradeChallanDetilsVM vm)
        {
            MPLTradeChallanVM TradeChallanVM = new MPLTradeChallanVM();
            TradeChallanVM.Id=vm.Id;
            List<MPLTradeChallanDetilsVM>detailVMs=new List<MPLTradeChallanDetilsVM>();
            if(vm.Id>0)
            {
             detailVMs = _repo.GetMPLCreditItemList(TradeChallanVM, null, null);
            }
           
           return PartialView("_detail", detailVMs);
        }


        [Authorize]
        [HttpGet]
        public ActionResult GetMPLCreditInvoiceListPopUp(MPLTradeChallanVM paramVM)
        {
            try
            {
                string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

                string dtFrom = null;
                string dtTo = null;
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    dtFrom = Convert.ToDateTime(paramVM.FromDate).ToString("yyyy-MM-dd");
                }
                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    dtTo = Convert.ToDateTime(paramVM.ToDate).AddDays(1).ToString("yyyy-MM-dd");
                }

                string[] conditionFields;
                string[] conditionValues;

                conditionFields = new string[] { "SH.InvoiceDateTime>=", "SH.InvoiceDateTime<=", "c.CustomerName", "sh.SalesInvoiceNo", "sh.BranchId" };
                conditionValues = new string[] { dtFrom, dtTo, paramVM.CustomerName, paramVM.SalesInvoiceNo, paramVM.BranchId.ToString()};

                MPLTradeChallanVM vm = new MPLTradeChallanVM();
                List<MPLTradeChallanDetilsVM> detailVMs = new List<MPLTradeChallanDetilsVM>();
                detailVMs = _repo.GetMPLCreditInvoiceList(paramVM, conditionFields, conditionValues);
                vm.MPLTradeChallanDetilsVMs = detailVMs;
                return PartialView("_tradeChallanDetails", vm);
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult SearchMPLCreditInvoiceListPopUp(MPLTradeChallanVM paramVM)
        {
            try
            {
                string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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

                string dtFrom = null;
                string dtTo = null;
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
                if (!string.IsNullOrWhiteSpace(paramVM.FromDate))
                {
                    dtFrom = Convert.ToDateTime(paramVM.FromDate).ToString("yyyy-MM-dd");
                }
                if (!string.IsNullOrWhiteSpace(paramVM.ToDate))
                {
                    dtTo = Convert.ToDateTime(paramVM.ToDate).AddDays(1).ToString("yyyy-MM-dd");
                }

                string[] conditionFields;
                string[] conditionValues;

                if (paramVM.SearchField != null)
                {
                    conditionFields = new string[] { paramVM.SearchField + " like", "SH.InvoiceDateTime>=", "SH.InvoiceDateTime<=", "SH.BranchId", "c.CustomerName", "", "", "" };
                    conditionValues = new string[] { paramVM.SearchValue, dtFrom, dtTo, paramVM.BranchId.ToString(), paramVM.CustomerName, "", "", "" };
                }
                else
                {
                    //conditionFields = new string[] { "bp.InstrumentDate>=", "bp.InstrumentDate<=", "bb.BankName", "sh.SalesInvoiceNo", "bp.InstrumentNo", "bp.ModeOfPayment", "bp.BranchId" };
                    conditionFields = new string[] { "SH.InvoiceDateTime>=", "SH.InvoiceDateTime<=", "c.CustomerName", "d.SHSalesInvoiceNo", "SH.BranchId" };
                    conditionValues = new string[] { dtFrom, dtTo, paramVM.CustomerName, paramVM.SalesInvoiceNo,paramVM.BranchId.ToString() };
                }

                List<MPLTradeChallanDetilsVM> detailVMs = new List<MPLTradeChallanDetilsVM>();
                detailVMs = _repo.GetMPLCreditInvoiceList(paramVM, conditionFields, conditionValues);

                return PartialView("_filteredDepositSlipDetail", detailVMs);
            }
            catch (Exception e)
            {
                List<MPLTradeChallanDetilsVM> detailVMs = new List<MPLTradeChallanDetilsVM>();

                string msg = e.Message.Split('\r').FirstOrDefault();
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return PartialView("_filteredDepositSlipDetail", detailVMs);
            }
        }

    }
}

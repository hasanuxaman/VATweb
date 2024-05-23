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
    public class MPLIN89Controller : Controller
    {
        ShampanIdentity identity = null;

        MPLIN89Repo _repo = null;

        public MPLIN89Controller()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new MPLIN89Repo(identity);
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
        public ActionResult Index(MPLIN89VM paramVM)
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

            paramVM.TransactionType = "Other";

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
        public ActionResult _index(JQueryDataTableParamVM param, MPLIN89VM paramVM)
        {
            _repo = new MPLIN89Repo(identity, Session);

            List<MPLIN89VM> getAllData = new List<MPLIN89VM>();

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

            if (string.IsNullOrEmpty(paramVM.SearchField))
            {
                paramVM.SearchValue = "";
            }
            else
            {
                paramVM.SearchField = "H." + paramVM.SearchField + " like";
            }

            string[] conditionFields;
            string[] conditionValues;

            conditionFields = new string[] { "H.TransactionDateTime>=", "H.TransactionDateTime<=", "H.Post", "H.BranchId", paramVM.SearchField };
            conditionValues = new string[] { dtFrom, dtTo, paramVM.Post, paramVM.BranchId.ToString(), paramVM.SearchValue };

            getAllData = _repo.SelectAll(0, conditionFields, conditionValues, null, null,paramVM.TransactionType, "Y",paramVM.SelectTop);


            #endregion

            #region Search and Filter Data
            IEnumerable<MPLIN89VM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
               
                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);


                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.TransactionDateTime.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.IssueNaturalQuantity.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.ReceiveNaturalQuantity.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<MPLIN89VM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code : sortColumnIndex == 2 && isSortable_2 ? c.TransactionDateTime.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.IssueNaturalQuantity.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.ReceiveNaturalQuantity.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Post.ToString() :

                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] { 
                  c.Id+"~"+ c.Post+"~"+ c.BranchId
                , c.Code
                , c.TransactionDateTime.ToString()             
                , c.IssueNaturalQuantity.ToString()               
                , c.ReceiveNaturalQuantity.ToString()     
                , c.GainNaturalQuantity.ToString()     
                , c.Status
                , c.TransactionType
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
        public ActionResult Create(MPLIN89VM vm)
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

            string issueIds = "";
            string receiveIds = "";

            for (int i = 0; i < vm.IDs.Count; i++)
            {

                string[] splitIds = vm.IDs[i].Split('~'); 
                if (splitIds.Length < 3)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(splitIds[1]))
                {
                    continue;
                }
                if (string.IsNullOrEmpty(receiveIds))
                {
                    receiveIds = splitIds[1];
                }
                else
                {
                    receiveIds += "," + splitIds[1];
                }

                if (string.IsNullOrEmpty(splitIds[2]))
                {
                    continue;
                }

                if (string.IsNullOrEmpty(issueIds))
                {
                    issueIds = splitIds[2];
                }
                else
                {
                    issueIds += "," + splitIds[2];
                }

            }


            List<MPLIN89DetailsVM> detailVMs = new List<MPLIN89DetailsVM>();
            List<MPLIN89IssueDetailsVM> issueDetailVMs = new List<MPLIN89IssueDetailsVM>();

            issueDetailVMs = _repo.SearchTransferMPLIssuesList(issueIds);
            vm.MPLIN89IssueDetailsVMs = issueDetailVMs;

            detailVMs = _repo.SearchTransferMPLReceivesList(receiveIds);
            vm.MPLIN89DetailsVMs = detailVMs;

            foreach (var item in detailVMs.Take(1))
            {
                vm.ItemNo = item.ItemNo;
                vm.ProductCode = item.ProductCode;
                vm.ProductName = item.ProductName;
            }
           
            vm.Operation = "add";
            vm.TransactionType = "Other";
            vm.TransactionDateTime = Session["SessionDate"].ToString();

            return View(vm);
        }
        
        [HttpPost]
        [Authorize]
        public ActionResult CreateEdit(MPLIN89VM vm)
        {
            try
            {
                _repo = new MPLIN89Repo(identity, Session);
                string[] result = new string[6];

                if (!string.IsNullOrEmpty(vm.TransactionDateTime))
                { vm.TransactionDateTime = Convert.ToDateTime(vm.TransactionDateTime).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss"); }
                
                try
                {
                    string UserId = identity.UserId;

                    if (vm.Operation.ToLower() == "add")
                    {
                        vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                        vm.CreatedBy = identity.Name;
                        vm.BranchId = Convert.ToInt32(Session["BranchId"]);

                        result = _repo.MPLIN89Insert(vm, null, null);

                        if (result[0] == "Success")
                        {
                            Session["result"] = result[0] + "~" + result[1];
                            return RedirectToAction("Edit", new { id = result[4], TransactionType = vm.TransactionType });
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
                        vm.BranchId = Convert.ToInt32(Session["BranchId"]);

                        result = _repo.MPLIN89Update(vm, null, null);

                        if (result[0] == "Success")
                        {
                            Session["result"] = result[0] + "~" + result[1];
                            return RedirectToAction("Edit", new { id = vm.Id, TransactionType = vm.TransactionType });
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
                    return RedirectToAction("Edit", new { id = vm.Id, TransactionType = vm.TransactionType });
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
        public ActionResult Edit(string id, string TransactionType)
        {
            MPLIN89VM vm = new MPLIN89VM();

            try
            {
                _repo = new MPLIN89Repo(identity, Session);
                if (TransactionType == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                vm = _repo.SelectAll(Convert.ToInt32(id), null, null, null, null, null, TransactionType).FirstOrDefault();

                if (vm == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                List<MPLIN89DetailsVM> detailVMs = new List<MPLIN89DetailsVM>();
                List<MPLIN89IssueDetailsVM> issueDetailVMs = new List<MPLIN89IssueDetailsVM>();

                detailVMs = _repo.SearchMPLIN89DetailsList(id);
                vm.MPLIN89DetailsVMs = detailVMs;

                issueDetailVMs = _repo.SearchMPLIN89IssueDetailsList(id);
                vm.MPLIN89IssueDetailsVMs = issueDetailVMs;

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
        public ActionResult MPLIN89Post(MPLIN89VM vm)
        {
            try
            {
                if (vm.IDs == null)
                {
                    return Json("Already Posted!", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    _repo = new MPLIN89Repo(identity, Session);
                    string[] result = new string[6];

                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.LastModifiedBy = identity.Name;

                    result = _repo.MPLIN89Post(vm, null, null);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                string msg = e.Message.Split('\r').FirstOrDefault();
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return RedirectToAction("Index");
            }
        }



        [UserFilter]
        public ActionResult TransferReceiveIndex(MPLIN89VM paramVM)
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

            paramVM.TransactionType = "Other";

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
        public ActionResult _receiveIndex(JQueryDataTableParamVM param, MPLIN89VM paramVM)
        {
            _repo = new MPLIN89Repo(identity, Session);

            List<MPLIN89VM> getAllData = new List<MPLIN89VM>();

            #region Access Controll

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

            #endregion

            #region SeachParameters

            string dtFrom = null;
            string dtTo = null;
            paramVM.SelectTop = paramVM.SelectTop == null ? "100" : paramVM.SelectTop;
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

            if (string.IsNullOrEmpty(paramVM.SearchField))
            {
                paramVM.SearchValue = "";
            }
            else
            {
                paramVM.SearchField = "P." + paramVM.SearchField + " like";
            }

            string[] conditionFields;
            string[] conditionValues;

            conditionFields = new string[] { "D.ReceiveDateTime>=", "D.ReceiveDateTime<=", "D.Post", "D.BranchId", paramVM.SearchField };
            conditionValues = new string[] { dtFrom, dtTo, paramVM.Post, paramVM.BranchId.ToString(), paramVM.SearchValue };

            getAllData = _repo.TransReceiveIndex(0, conditionFields, conditionValues, null, null, paramVM.TransactionType, "Y", paramVM.SelectTop);


            #endregion

            #region Search and Filter Data
            IEnumerable<MPLIN89VM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                //Optionally check whether the columns are searchable at all 
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);


                filteredData = getAllData
                   .Where(c => isSearchable1 && c.TransferReceiveNo.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.ProductName.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable3 && c.ProductCode.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable4 && c.WagonNo.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable5 && c.Post.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredData = getAllData;
            }

            #endregion Search and Filter Data

            var isSortable_1 = Convert.ToBoolean(Request["bSortable_1"]);
            var isSortable_2 = Convert.ToBoolean(Request["bSortable_2"]);
            var isSortable_3 = Convert.ToBoolean(Request["bSortable_3"]);
            var isSortable_4 = Convert.ToBoolean(Request["bSortable_4"]);
            var isSortable_5 = Convert.ToBoolean(Request["bSortable_5"]);

            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<MPLIN89VM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.TransferReceiveNo : sortColumnIndex == 2 && isSortable_2 ? c.ProductName.ToString() :
                                                           sortColumnIndex == 3 && isSortable_3 ? c.ProductCode.ToString() :
                                                           sortColumnIndex == 4 && isSortable_4 ? c.WagonNo.ToString() :
                                                           sortColumnIndex == 5 && isSortable_5 ? c.Post.ToString() :

                                                           "");

            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);

            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in getAllData
                         select new[] { 
                  c.ItemNo+"~"+ c.TransferIssueMasterRefId +"~"+c.TransferIssueDetailsRefId
                , c.TransferReceiveNo.ToString()             
                , c.ReceiveDateTime.ToString()     
                , c.TransferFrom.ToString()    
                , c.ProductCode.ToString()    
                , c.ProductName.ToString()  
                , c.WagonNo.ToString()   
                , c.Quantity.ToString() 
                , c.TransactionType

             };

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = getAllData.Count(),
                iTotalDisplayRecords = filteredData.Count(),
                aaData = result
            },JsonRequestBehavior.AllowGet);
        }


    }
}

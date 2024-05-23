using SymOrdinary;
using SymRepository.VMS;
using SymVATWebUI.Filters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using VATServer.Ordinary;
using VATViewModel.DTOs;

namespace SymVATWebUI.Areas.VMS.Controllers
{
    [ShampanAuthorize]
    public class TollProductionConsumptionController : Controller
    {
        //
        // GET: /VMS/TollProductionConsumption/


        TollProductionConsumptionRepo _repo = null;
        ShampanIdentity identity = null;

        public TollProductionConsumptionController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new TollProductionConsumptionRepo(identity);
            }
            catch
            {

            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index(TollProductionConsumptionVM paramVM)
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


            if (string.IsNullOrWhiteSpace(paramVM.TransactionType))
            {
                paramVM.TransactionType = "Other";
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
        public ActionResult _index(JQueryDataTableParamVM param, TollProductionConsumptionVM paramVM)
        {
            _repo = new TollProductionConsumptionRepo(identity, Session);

            List<TollProductionConsumptionVM> getAllData = new List<TollProductionConsumptionVM>();

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

            #region SeachParameters

            string dtFrom = DateTime.Now.ToString("yyyyMMdd");
            string dtTo = DateTime.Now.AddDays(1).ToString("yyyyMMdd");

            if (!string.IsNullOrWhiteSpace(paramVM.DateTimeFrom))
            {
                dtFrom = Convert.ToDateTime(paramVM.DateTimeFrom).ToString("yyyyMMdd");
            }
            if (!string.IsNullOrWhiteSpace(paramVM.DateTimeTo))
            {

                dtTo = Convert.ToDateTime(paramVM.DateTimeTo).AddDays(1).ToString("yyyyMMdd");
            }

            if (string.IsNullOrWhiteSpace(paramVM.SelectTop))
            {
                paramVM.SelectTop = "All";
            }

            if (paramVM.BranchId == 0)
            {
                paramVM.BranchId = Convert.ToInt32(Session["BranchId"].ToString());
            }

            if (paramVM.BranchId == -1)
            {
                paramVM.BranchId = 0;
            }
            if (!string.IsNullOrWhiteSpace(paramVM.Code) || !string.IsNullOrWhiteSpace(paramVM.ImportID))
            {
                dtFrom = "";
                dtTo = "";
            }

            #endregion SeachParameters

            string[] conditionFields = { "DateTime>=", "DateTime<=", "ImportID", "TransactionType like", "Code", "BranchId", "SelectTop", "ImportIDExcel like" };
            string[] conditionValues = { dtFrom, dtTo, paramVM.ImportID, paramVM.TransactionType, paramVM.Code, paramVM.BranchId.ToString(), paramVM.SelectTop, paramVM.ImportID };

            getAllData = _repo.SelectAll(0, conditionFields, conditionValues);

            #endregion
            #region Search and Filter Data
            IEnumerable<TollProductionConsumptionVM> filteredData;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {

                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);
                var isSearchable6 = Convert.ToBoolean(Request["bSearchable_6"]);


                filteredData = getAllData
                   .Where(c => isSearchable1 && c.Code.ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable2 && c.DateTime.ToString().ToLower().Contains(param.sSearch.ToLower())
                       //|| isSearchable3 && c.TotalVATAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                       //|| isSearchable4 && c.TotalAmount.ToString().ToLower().Contains(param.sSearch.ToLower())
                       //|| isSearchable5 && c.SerialNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                               || isSearchable6 && c.Post.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            var isSortable_6 = Convert.ToBoolean(Request["bSortable_6"]);
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            Func<TollProductionConsumptionVM, string> orderingFunction = (c => sortColumnIndex == 1 && isSortable_1 ? c.Code :
                                                           sortColumnIndex == 2 && isSortable_2 ? Ordinary.DateToString(c.DateTime) :
                //sortColumnIndex == 3 && isSortable_3 ? c.TotalVATAmount.ToString() :
                //sortColumnIndex == 4 && isSortable_4 ? c.TotalAmount.ToString() :
                //sortColumnIndex == 5 && isSortable_5 ? c.SerialNo.ToString() :
                                                           sortColumnIndex == 6 && isSortable_6 ? c.Post.ToString() :
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
                , c.DateTime
                //, c.TotalAmount.ToString()
                , c.Comments.ToString()             
                , c.RefNo.ToString()               
                , c.Post=="Y" ? "Posted" : "Not Posted"
                ,c.ImportID              
                ,c.TransactionType              
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
            TollProductionConsumptionVM vm = new TollProductionConsumptionVM();

            List<TollProductionConsumptionDetailVM> DetailVMs = new List<TollProductionConsumptionDetailVM>();
            vm.Details = DetailVMs;
            vm.Operation = "add";
            vm.TransactionType = tType;
            vm.DateTime = Session["SessionDate"].ToString();
            vm.ProductType = "Raw";
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult CreateEdit(TollProductionConsumptionVM vm)
        {
            _repo = new TollProductionConsumptionRepo(identity, Session);
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

            //string[] result = new string[6];

            try
            {
                if (vm.Operation.ToLower() == "add")
                {
                    vm.CreatedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.CreatedBy = identity.Name;
                    vm.Post = "N";
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);
                    vm.PeriodID = "-";
                    if (vm.RefNo == null)
                    {
                        vm.RefNo = "-";
                    }
                    if (vm.Comments == null)
                    {
                        vm.Comments = "-";
                    }
                    if (vm.ImportID == null)
                    {
                        vm.ImportID = "-";
                    }

                    ResultVM rVM = _repo.SaveData(vm);

                    rVM.Message = rVM.Message.Split('\r').FirstOrDefault();

                    if (rVM.Status == "Success")
                    {
                       
                        Session["result"] = rVM.Status + "~" + rVM.Message;
                        return RedirectToAction("Edit", new { id = rVM.Id, TransactionType = vm.TransactionType });

                    }
                    else
                    {
                        Session["result"] = rVM.Status + "~" + rVM.Message;
                        return View("Create", vm);
                    }

                    return View("Create", vm);

                }
                else if (vm.Operation.ToLower() == "update")
                {
                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss");
                    vm.BranchId = Convert.ToInt32(Session["BranchId"]);

                    #region Branch Check

                    string[] conditionFields;
                    string[] conditionValues;

                    conditionFields = new string[] { "Code" };
                    conditionValues = new string[] { vm.Code };

                    int OldBranchId = _repo.SelectAll(0, conditionFields, conditionValues).FirstOrDefault().BranchId;

                    if (OldBranchId != Convert.ToInt32(Session["BranchId"]) && Convert.ToInt32(vm.BranchId) != 0)
                    {

                        throw new ArgumentNullException("", "This Information not for this Branch");

                    }

                    #endregion

                    var check = vm.Id;

                    ResultVM rVM = _repo.UpdateData(vm, identity.UserId);

                    rVM.Message = rVM.Message.Split('\r').FirstOrDefault();

                    if (rVM.Status == "Success")
                    {
                        Session["result"] = rVM.Status + "~" + rVM.Message;
                        return RedirectToAction("Edit", new { id = rVM.Id, TransactionType = vm.TransactionType });
                    }
                    else
                    {
                        Session["result"] = rVM.Status + "~" + rVM.Message;
                        return View("Create", vm);

                    }

                    return View("Create", vm);

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
                return View("Create", vm);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string id, string TransactionType)
        {
            _repo = new TollProductionConsumptionRepo(identity, Session);
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

            if (TransactionType == null)
            {
                return RedirectToAction("Index", "Home");
            }

            TollProductionConsumptionVM vm = new TollProductionConsumptionVM();

            string[] conditionFields = new string[] { "TransactionType" };
            string[] conditionValues = new string[] { TransactionType };

            vm = _repo.SelectAll(Convert.ToInt32(id), conditionFields, conditionValues).FirstOrDefault();

            if (vm == null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<TollProductionConsumptionDetailVM> DetailVMs = new List<TollProductionConsumptionDetailVM>();

            DetailVMs = _repo.SearchDetailList(id);

            vm.Details = DetailVMs;
            vm.Operation = "update";
            vm.ProductType = "Raw";

            return View("Create", vm);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult BlankItem(TollProductionConsumptionDetailVM vm)
        {

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                //
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/Vms/Home");
            }

            ProductRepo PRepo = new ProductRepo(identity, Session);
            string BomId = null;

            var dtbom = PRepo.SelectBOMRaw(vm.ItemNo, DateTime.Parse(vm.TollDateTime).ToString("yyyy-MMM-dd"));

            if (dtbom != null && dtbom.Rows.Count > 0)
            {
                BomId = dtbom.Rows[0]["BOMId"].ToString();
            }
            if (string.IsNullOrEmpty(BomId))
            {
                vm.BomId = "0";
            }
            else
            {
                vm.BomId = BomId;
            }

            return PartialView("_detail", vm);
        }

        public JsonResult ProductDetails(string productCode, string IssueDate)
        {
            var _repo = new ProductRepo(identity, Session);

            var product = _repo.SelectAll(productCode).FirstOrDefault();


            string UserId = identity.UserId;

            #region businessLogic
            OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);
            var issueDatetime = DateTime.Parse(IssueDate).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
            DataTable priceData = _repo.AvgPriceNew(product.ItemNo, issueDatetime, null, null, true, true, true, true, UserId);
            decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
            decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

            if (quan > 0)
            {
                product.CostPrice = (amount / quan);
            }
            else
            {
                product.CostPrice = 0;
            }
            #endregion businessLogic
            product.Stock = quan;

            #region BOM Set

            //DataTable dt = new DataTable();

            //dt = _repo.SelectBOMRaw(product.ProductCode, Convert.ToDateTime(issueDatetime).ToString("yyyy-MMM-dd"));

            //int BOMId = 0;

            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    DataRow dr = dt.Rows[0];
            //    string tempBOMId = dr["BOMId"].ToString();
            //    if (!string.IsNullOrWhiteSpace(tempBOMId))
            //    {
            //        BOMId = Convert.ToInt32(tempBOMId);
            //    }
            //}

            //txtBOMId.Text = BOMId.ToString();
            #endregion

            return Json(product, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Post(string ids)
        {
            _repo = new TollProductionConsumptionRepo(identity, Session);
            string[] a = ids.Split('~');
            var id = a[0];
            TollProductionConsumptionVM vm = new TollProductionConsumptionVM();
            vm = _repo.SelectAll(Convert.ToInt32(id)).FirstOrDefault();
            List<TollProductionConsumptionDetailVM> TollProductionDetailVMs = new List<TollProductionConsumptionDetailVM>();

            TollProductionDetailVMs = _repo.SearchDetailList(vm.Id.ToString());

            vm.Details = TollProductionDetailVMs;

            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            vm.Post = "Y";
            OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);

            ResultVM result = _repo.PostData(vm, identity.UserId);

            return Json(result.Message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUomOption(string uomFrom)
        {
            try
            {
                var _repo = new UOMRepo(identity, Session);
                string[] conditionalFields = new string[] { "UOMFrom" };
                string[] conditionalValues = new string[] { uomFrom };
                var uoms = _repo.SelectAll(0, conditionalFields, conditionalValues);

                return Json(uoms, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                List<UOMConversionVM> VMs = new List<UOMConversionVM>();
                UOMConversionVM vm = new UOMConversionVM();
                vm.UOMFrom = uomFrom.ToString();
                vm.UOMTo = uomFrom.ToString();
                vm.Convertion = 1;
                VMs.Add(vm);

                return Json(VMs, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetConvFactor(string uomFrom, string UomTo)
        {
            var _repo = new UOMRepo(identity, Session);
            string[] conditionalFields = new string[] { "UOMFrom", "UOMTo" };
            string[] conditionalValues = new string[] { uomFrom, UomTo };
            var uom = _repo.SelectAll(0, conditionalFields, conditionalValues).FirstOrDefault();
            var uomFactor = uom.Convertion;
            return Json(uomFactor, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductDetailsSearch(string productCode, string tollDate)
        {
            var _repo = new ProductRepo(identity, Session);

            string[] conditionalFields = new string[] { "Pr.ProductCode" };
            string[] conditionalValues = new string[] { productCode };

            var product = _repo.SelectAll("0", conditionalFields, conditionalValues).FirstOrDefault();

            string UserId = identity.UserId;

            #region businessLogic
            OrdinaryVATDesktop.BranchId = Convert.ToInt32(Session["BranchId"]);
            var issueDatetime = DateTime.Parse(tollDate).ToString("yyyy-MMM-dd") + DateTime.Now.ToString(" HH:mm:ss");
            DataTable priceData = _repo.AvgPriceNew(product.ItemNo, issueDatetime, null, null, true, true, true, true, UserId);
            decimal amount = Convert.ToDecimal(priceData.Rows[0]["Amount"].ToString());
            decimal quan = Convert.ToDecimal(priceData.Rows[0]["Quantity"].ToString());

            if (quan > 0)
            {
                product.CostPrice = (amount / quan);
            }
            else
            {
                product.CostPrice = 0;
            }
            #endregion businessLogic
            product.Stock = quan;

            return Json(product, JsonRequestBehavior.AllowGet);

        }



    }
}

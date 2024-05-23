﻿//using JQueryDataTables.Models;
using SymOrdinary;
using SymphonySofttech.Utilities;
using SymphonySofttech.Reports.Report;
//using SymRepository.Common;
using SymRepository.VMS;
using SymVATWebUI.Areas.VMS.Models;
//using SymViewModel.Common;
using VATViewModel.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymVATWebUI.Filters;
using System.Configuration;
using VATServer.Ordinary;
using System.Text.RegularExpressions;

namespace SymVATWebUI.Areas.vms.Controllers
{
    [ShampanAuthorize]
    public class UserInformationController : Controller
    {
        //
        // GET: /vms/Branch/
        ShampanIdentity identity = null;
        UserInformationRepo _repo = null;

        public UserInformationController()
        {
            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

                _repo = new UserInformationRepo(identity);

            }
            catch
            {

            }
        }
        //////ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;

        ////UserInformationRepo _repo = new UserInformationRepo(identity);

        [Authorize(Roles = "Admin")]
        public ActionResult HomeIndex()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/vms/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/vms/Home");
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        public ActionResult _index(JQueryDataTableParamModel param)
        {
            _repo = new UserInformationRepo(identity, Session);

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/vms/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/vms/Home");
            }
            //00     //UserID 
            //01     //UserName
            //02     //GroupName
            //03     //LastLoginDateTime
            //04     //LastModifiedBy
            //05     //LastModifiedOn


            #region Search and Filter Data
            var getAllData = _repo.SelectAll();
            IEnumerable<UserInformationVM> filteredData;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                var isSearchable1 = Convert.ToBoolean(Request["bSearchable_1"]);
                var isSearchable2 = Convert.ToBoolean(Request["bSearchable_2"]);
                var isSearchable3 = Convert.ToBoolean(Request["bSearchable_3"]);
                var isSearchable4 = Convert.ToBoolean(Request["bSearchable_4"]);
                var isSearchable5 = Convert.ToBoolean(Request["bSearchable_5"]);

                filteredData = getAllData.Where(c =>
                       isSearchable1 && c.UserName.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable2 && c.FullName.ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable3 && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable4 && c.Mobile.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || isSearchable5 && c.Email.ToString().ToLower().Contains(param.sSearch.ToLower())
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
            Func<UserInformationVM, string> orderingFunction = (c =>
                sortColumnIndex == 1 && isSortable_1 ? c.UserName.ToString() :
                sortColumnIndex == 2 && isSortable_2 ? c.FullName.ToString() :
                sortColumnIndex == 3 && isSortable_3 ? c.Designation.ToString() :
                sortColumnIndex == 4 && isSortable_4 ? c.Mobile.ToString() :
                sortColumnIndex == 5 && isSortable_5 ? c.Email.ToString() :
                "");
            var sortDirection = Request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filteredData = filteredData.OrderBy(orderingFunction);
            else
                filteredData = filteredData.OrderByDescending(orderingFunction);
            var displayedCompanies = filteredData.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new[] { 
                  c.UserID 
                , c.UserName
                , c.FullName
                , c.Designation
                , c.Mobile
                , c.Email
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
        public ActionResult Create()
        {
            ////string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            ////if (project.ToLower() == "vms")
            ////{
            ////    if (!identity.IsAdmin)
            ////    {
            ////        Session["rollPermission"] = "deny";
            ////        return Redirect("/vms/Home");
            ////    }
            ////}
            ////else
            ////{
            ////    Session["rollPermission"] = "deny";
            ////    return Redirect("/vms/Home");
            ////}
            ////UserInformationVM vm = new UserInformationVM();
            ////vm.Operation = "add";
            ////vm.ActiveStatus = true;
            //////////return PartialView(vm);


            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/vms/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/vms/Home");
            }

            UserInformationVM vm = new UserInformationVM();
            vm.Operation = "add";
            vm.ActiveStatus = true;

            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult CreateEdit(UserInformationVM vm)
        {
            _repo = new UserInformationRepo(identity, Session);

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/vms/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/vms/Home");
            }
            string[] result = new string[6];

            try
            {
                vm.IsMainSettings = vm.MainSetting ? "Y" : "N";

                #region Check

                
                if (!IsValidEmail(vm.Email))
                {
                    throw new ArgumentNullException("Email", "Email Address Not Valid.");
                }
                if (!OrdinaryVATDesktop.IsNumber(vm.NationalId))
                {
                    throw new ArgumentNullException("NationalID", "Please Enter National ID only number.");
                }
                if (vm.NationalId.Length >= 18)
                {
                    throw new ArgumentNullException("NationalID", "Please Enter Valid National ID No/Not more than 17 digits.");
                }


                #endregion

                if (vm.Operation.ToLower() == "add")
                {
                    var regexItem = new Regex("[^A-Za-z0-9]");

                    if (regexItem.IsMatch(vm.UserName))
                    {
                        throw new ArgumentNullException("UserName", "User name only allows Alphabets & Numeric. Space not allowed.");
                    }
                    if (vm.UserPassword.Length <= 5)
                    {
                        ModelState.AddModelError("UserPassword", "Please Enter Password At Least 6 Digits.");
                    }
                    if (vm.UserPassword != vm.NewPassword)
                    {
                        throw new ArgumentNullException("Password not Matched Please Confirm Again");
                    }

                    vm.CreatedOn = DateTime.Now.ToString();
                    vm.CreatedBy = identity.Name;
                    vm.ActiveStatus = true;
                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString();
                    var constRepo = new DBConstantRepo();
                    string passPhrase = DBConstant.PassPhrase;
                    string enckey = DBConstant.EnKey;
                    vm.UserPassword = Converter.DESEncrypt(passPhrase, enckey, vm.UserPassword);

                    result = _repo.InsertUserInformation(vm);

                    Session["result"] = result[0] + "~" + result[1];

                    if (result[0] == "Success")
                    {
                        return RedirectToAction("Edit", new { UserId = result[2] });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1].Replace("\r", "").Replace("\n", "");
                        return View("Create", vm);
                    }

                }
                else if (vm.Operation.ToLower() == "update")
                {
                    if (string.IsNullOrWhiteSpace(vm.UserID))
                    {
                        throw new ArgumentNullException("No data updated. Please select existing information first");
                    }

                    vm.LastModifiedBy = identity.Name;
                    vm.LastModifiedOn = DateTime.Now.ToString();

                    result = _repo.UpdateUserInformation(vm);

                    Session["result"] = result[0] + "~" + result[1];

                    if (result[0] == "Success")
                    {
                        return RedirectToAction("Edit", new { UserId = vm.UserID });
                    }
                    else
                    {
                        Session["result"] = result[0] + "~" + result[1].Replace("\r", "").Replace("\n", "");
                        return View("Create", vm);
                    }

                }
                else
                {
                    return RedirectToAction("Index");
                }


            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                Session["result"] = "Fail~" + msg;

                FileLogger.Log("UserInformationController", "CreateEdit", ex.ToString());

                return View("Create", vm);
            }
        }

        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult Edit(string UserId)
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new UserInformationRepo(identity, Session);
                CommonRepo _cRepo = new CommonRepo(identity, Session);

                string project = new AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
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


                UserInformationVM vm = new UserInformationVM();

                //string[] conditionFields = new string[] { "pih.TransactionType" };
                //string[] conditionValues = new string[] { TransactionType };


                vm = _repo.SelectAll(Convert.ToInt32(UserId)).FirstOrDefault();

                vm.MainSetting = vm.IsMainSettings == "Y";

                vm.Operation = "update";

                return View("Create", vm);
            }
            catch (Exception e)
            {
                ////SymOrdinary.FileLogger.Log("SaleInvoiceController", "CreateEdit", "-----" + "\n" + e.StackTrace);

                return RedirectToAction("Index");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult ChangePassword()
        {
            _repo = new UserInformationRepo(identity, Session);

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();

            UserInformationVM vm = new UserInformationVM();
            vm.IsAdmin = identity.IsAdmin;
            vm.UserName = identity.Name;
            vm.UserPassword = null;

            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/vms/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/vms/Home");
            }
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult ChangePassword(UserInformationVM vm)
        {
            try
            {
                _repo = new UserInformationRepo(identity, Session);

                string[] result = new string[6];
                string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
                if (project.ToLower() == "vms")
                {
                    if (!identity.IsAdmin)
                    {
                        Session["rollPermission"] = "deny";
                        return Redirect("/vms/Home");
                    }
                }
                else
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/vms/Home");
                }
                if (identity.Name.ToLower() != "admin")
                {
                    bool passMatch = matchPassword(vm.UserName, vm.UserPassword);
                    if (passMatch == false)
                    {
                        Session["result"] = "Fail~Current Password didn't match";
                        return View("ChangePassword", vm);
                    }
                }

                string newPassword = Converter.DESEncrypt(DBConstant.PassPhrase, DBConstant.EnKey, vm.NewPassword);
                result = _repo.UpdateUserPasswordNew(vm.UserName, newPassword, identity.Name, DateTime.Now.ToString(), "");
                Session["result"] = result[0] + "~" + result[1];
                vm.UserName = identity.Name;
                //return RedirectToAction("Index");
                return View("ChangePassword", vm);
            }
            catch (Exception ex)
            {
                string msg = ex.Message.Split('\r').FirstOrDefault();
                msg = msg.Replace("SQL:~", "");
                Session["result"] = "Fail" + "~" + msg;
                return RedirectToAction("ChangePassword");

            }


        }

        public bool matchPassword(string userName, string userPassword)
        {
            _repo = new UserInformationRepo(identity, Session);

            string[] cFields = new string[] { "ui.UserName" };
            string[] cVals = new string[] { userName };
            UserInformationVM user = _repo.SelectAll(0, cFields, cVals, null, null).FirstOrDefault();
            if (user != null)
            {
                string currentPassword = Converter.DESDecrypt(DBConstant.PassPhrase, DBConstant.EnKey, user.UserPassword);
                if (currentPassword == userPassword)
                {
                    return true;
                }
            }
            return false;
        }

    }
}

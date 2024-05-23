//using JQueryDataTables.Models;
using SymOrdinary;
using SymRepository.VMS;
using VATViewModel.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using SymVATWebUI.Filters;
using Microsoft.Office.Interop.Excel;
using System.Data;

namespace SymVATWebUI.Areas.Vms.Controllers
{
    [ShampanAuthorize]
    public class SettingController : Controller
    {
        //
        // GET: /VMS/Branch/

        // 
        ShampanIdentity identity = null;

        SettingRepo _repo = null;

        public SettingController()
        {

            try
            {
                identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                _repo = new SettingRepo(identity);

            }
            catch
            {

            }
        }
        // SettingRepo _repo = new SettingRepo();
        // ShampanIdentity identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
        [Authorize(Roles = "Admin")]
        public ActionResult Index(string group, string IsSettingRol = "")
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new SettingRepo(identity, Session);

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/VMS/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/VMS/Home");
            }

            _repo.SettingChange();

            List<SettingsVM> vms = new List<SettingsVM>();
            SettingsVM vm = new SettingsVM();

            System.Data.DataTable dt = new System.Data.DataTable();

            try
            {

                ////////vms = _repo.SearchSettingsRoleList(identity.UserId, identity.Name);

                vms = _repo.SelectAll();

                if (group != null && group != "AllGroup")
                {
                    ViewBag.groupName = group;
                    var filteredList = vms.Where(m => m.SettingGroup == group).ToList();
                    return View(filteredList);
                }
                ViewBag.groupName = "AllGroup";

                vms = new List<SettingsVM>();
                
                return View(vms);
            }
            catch (Exception)
            {
                return View(vms);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult IndexUser(string group, string UserId, string UserName)
        {
            identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
            _repo = new SettingRepo(identity, Session);

            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/VMS/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/VMS/Home");
            }

            _repo.SettingChange();

            List<SettingsVM> vms = new List<SettingsVM>();
            SettingsVM vm = new SettingsVM();

            SettingModel sModel = new SettingModel();

            try
            {
                sModel.UserID = UserId;
                sModel.UserName = UserName;
                sModel.groupName = group;

                if (string.IsNullOrWhiteSpace(sModel.UserID))
                {
                    sModel.UserID = identity.UserId;
                }
                if (string.IsNullOrWhiteSpace(sModel.UserName))
                {
                    sModel.UserName = identity.Name;                    
                }

                vms = _repo.SearchSettingsRoleList(sModel.UserID, sModel.UserName);

                if (sModel.groupName != null && sModel.groupName != "AllGroup")
                {
                    sModel.vms = vms.Where(m => m.SettingGroup == sModel.groupName).ToList();
                    return View(sModel);
                }
                sModel.groupName = "AllGroup";

                sModel.vms = new List<SettingsVM>();

                return View(sModel);
            }
            catch (Exception)
            {
                return View(sModel);
            }
        }

        [HttpPost]
        public JsonResult UpdateUserSetting(SettingsVM vm)
        {
          

            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();

            List<SettingsVM> vms = new List<SettingsVM>();
            vms.Add(vm);

            if (identity.Name.ToLower() == "admin" && identity.UserId == "10")
            {
                result = new SettingRepo(identity, Session).SettingsUpdate(vms,vm.UserName,vm.UserID);
            }
            else
            {
                result = new SettingRepo(identity, Session).SettingsUpdate(vms);

            }

            //result = new SettingRepo(identity, Session).settingsDataUpdate(vm);
            var retResult = result[0] + "~" + result[1];
            return Json(retResult, JsonRequestBehavior.AllowGet);
        }

        
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(SettingsVM vm)
        {
            string project = new System.Configuration.AppSettingsReader().GetValue("CompanyName", typeof(string)).ToString();
            if (project.ToLower() == "vms")
            {
                if (!identity.IsAdmin)
                {
                    Session["rollPermission"] = "deny";
                    return Redirect("/VMS/Home");
                }
            }
            else
            {
                Session["rollPermission"] = "deny";
                return Redirect("/VMS/Home");
            }
            string[] result = new string[6];
            try
            {
                ShampanIdentity Identity = (ShampanIdentity)Thread.CurrentPrincipal.Identity;
                vm.LastModifiedBy = Identity.Name;
                vm.LastModifiedOn = DateTime.Now.ToString();
                result = new SettingRepo(identity, Session).settingsDataUpdate(vm);
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                Session["result"] = result[0] + "~" + result[1];
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public JsonResult Update(SettingsVM vm)
        {
            string[] result = new string[6];
            vm.LastModifiedBy = identity.Name;
            vm.LastModifiedOn = DateTime.Now.ToString();
            result = new SettingRepo(identity, Session).settingsDataUpdate(vm);
            var retResult = result[0] + "~" + result[1];
            return Json(retResult, JsonRequestBehavior.AllowGet);
        }




        public ActionResult DBUpdate()
        {
            ResultVM rVM = new ResultVM();
            rVM = new SettingRepo(identity, Session).DBUpdate("");

            Session["result"] = rVM.Status + "~" + rVM.Message;
            return Redirect("/VMS/Home");
        }





    }
}

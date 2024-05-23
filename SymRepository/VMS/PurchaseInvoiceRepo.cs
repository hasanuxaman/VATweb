using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Linq;
using VATViewModel.DTOs;
using VATServer.Library;
using VMSAPI;
using SymOrdinary;

namespace SymRepository.VMS
{
    public class PurchaseInvoiceRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public PurchaseInvoiceRepo()
        {
            connVM = null;
        }
        public PurchaseInvoiceRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public PurchaseInvoiceRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }


        public ResultVM ImportPurchaseBollore(string xml)
        {
            try
            {
                PurchaseAPI api = new PurchaseAPI();

                ResultVM rVM = api.ImportPurchaseBollore(xml, "", connVM);

                return rVM;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


    }
}

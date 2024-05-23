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
    public class MPLSalesInvSA4Repo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public MPLSalesInvSA4Repo()
        {
            connVM = null;
        }
        public MPLSalesInvSA4Repo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public MPLSalesInvSA4Repo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }


        public string[] MPLSalesInvSA4Insert(MPLSaleInvoiceSA4VM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new MPLSalesInvSA4DAL().MPLSalesInvSA4Insert(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] MPLSalesInvSA4Update(MPLSaleInvoiceSA4VM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new MPLSalesInvSA4DAL().MPLSalesInvSA4Update(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<MPLSaleInvoiceSA4VM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string ActiveStatus = "", string SelectTop = "100")
        {
            try
            {
                return new MPLSalesInvSA4DAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, null, ActiveStatus, SelectTop);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public List<MPLSaleInvoiceSA4VM> DropDown(int branchId, string ItemNo)
        {
            try
            {
                return new MPLSalesInvSA4DAL().DropDown(branchId, ItemNo, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] SaleInvoiceSA4Post(MPLSaleInvoiceSA4VM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new MPLSalesInvSA4DAL().SaleInvoiceSA4Post(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
}

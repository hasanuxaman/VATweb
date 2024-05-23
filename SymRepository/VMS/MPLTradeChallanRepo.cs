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
    public class MPLTradeChallanRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public MPLTradeChallanRepo()
        {
            connVM = null;
        }
        public MPLTradeChallanRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public MPLTradeChallanRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }


        public string[] InsertToMPLTradeChallan(MPLTradeChallanVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new MPLTradeChallanDAL().InsertToMPLTradeChallan(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] UpdateMPLTradeChallan(MPLTradeChallanVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new MPLTradeChallanDAL().UpdateMPLTradeChallan(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<MPLTradeChallanVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string SelectTop = "100")
        {
            try
            {
                return new MPLTradeChallanDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, SelectTop);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

      
        public List<MPLTradeChallanDetilsVM> GetMPLCreditInvoiceList(MPLTradeChallanVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new MPLTradeChallanDAL().GetMPLCreditInvoiceList(vm, conditionFields, conditionValues, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<MPLTradeChallanDetilsVM> GetMPLCreditItemList(MPLTradeChallanVM vm, string[] conditionFields = null, string[] conditionValues = null)
        {
            try
            {
                return new MPLTradeChallanDAL().GetMPLCreditItemList(vm, conditionFields, conditionValues, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
       

    }
}

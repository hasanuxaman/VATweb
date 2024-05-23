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
    public class MPLDayEndClosingRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public MPLDayEndClosingRepo()
        {
            connVM = null;
        }
        public MPLDayEndClosingRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public MPLDayEndClosingRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }



        public string[] InsertToMPLDayEndClosing(MPLDayEndClosingVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new MPLDayEndClosingDAL().InsertToMPLDayEndClosing(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public string[] UpdateMPLDayEndData(MPLDayEndClosingVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new MPLDayEndClosingDAL().UpdateMPLDayEndData(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<MPLDayEndClosingVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
        {
            try
            {
                return new MPLDayEndClosingDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, null, transactionType, Orderby, SelectTop);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string[] DayEndClosingPost(MPLDayEndClosingVM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new MPLDayEndClosingDAL().Post(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }



    }
}

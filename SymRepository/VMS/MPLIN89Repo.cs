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
    public class MPLIN89Repo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public MPLIN89Repo()
        {
            connVM = null;
        }
        public MPLIN89Repo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public MPLIN89Repo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        public IEnumerable<object> ReceiveIN89ColumnSearch()
        {
            try
            {
                string[] columnName = new string[] { "Code", "Item No"};
                IEnumerable<object> enumList = from e in columnName select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
                return enumList;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] MPLIN89Insert(MPLIN89VM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new MPLIN89DAL().MPLIN89Insert(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] MPLIN89Update(MPLIN89VM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new MPLIN89DAL().MPLIN89Update(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<MPLIN89VM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
        {
            try
            {
                return new MPLIN89DAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, null, transactionType, Orderby, SelectTop);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MPLIN89DetailsVM> SearchMPLIN89DetailsList(string mplIN89HeaderId)
        {
            try
            {
                return new MPLIN89DAL().SearchMPLIN89DetailsList(mplIN89HeaderId, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MPLIN89IssueDetailsVM> SearchMPLIN89IssueDetailsList(string mplIN89HeaderId)
        {
            try
            {
                return new MPLIN89DAL().SearchMPLIN89IssueDetailsList(mplIN89HeaderId, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] MPLIN89Post(MPLIN89VM Master, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new MPLIN89DAL().MPLIN89Post(Master, transaction, currConn, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public List<MPLIN89VM> TransReceiveIndex(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string transactionType = null, string Orderby = "Y", string SelectTop = "100")
        {
            try
            {
                return new MPLIN89DAL().TransReceiveIndex(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM, null, transactionType, Orderby, SelectTop);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MPLIN89IssueDetailsVM> SearchTransferMPLIssuesList(string id)
        {
            try
            {
                return new MPLIN89DAL().SearchTransferMPLIssuesList(id, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MPLIN89DetailsVM> SearchTransferMPLReceivesList(string id)
        {
            try
            {
                return new MPLIN89DAL().SearchTransferMPLReceivesList(id, null, null, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}

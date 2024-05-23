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
    public class DivisionRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public DivisionRepo()
        {
            connVM = null;
        }
        public DivisionRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public DivisionRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }


        public string[] InsertDivision(DivisionVM vm, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new DivisionDAL().InsertDivision(vm, connVM, currConn, transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] UpdateDivision(DivisionVM vm, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new DivisionDAL().UpdateDivision(vm, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<DivisionVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, string ActiveStatus = "", string SelectTop = "100")
        {
            try
            {
                return new DivisionDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DivisionVM> DropDown()
        {
            try
            {
                return new DivisionDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}

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
    public class DistrictRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public DistrictRepo()
        {
            connVM = null;
        }
        public DistrictRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }
        public DistrictRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }


        public string[] InsertDistrict(DistrictVM vm, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new DistrictDAL().InsertDistrict(vm, connVM, currConn, transaction);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string[] UpdateDistrict(DistrictVM vm, SqlTransaction transaction, SqlConnection currConn)
        {
            try
            {
                return new DistrictDAL().UpdateDistrict(vm,connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<DistrictVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new DistrictDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DistrictVM> DropDown(int? divisionId)
        {
            try
            {
                return new DistrictDAL().DropDown(divisionId, connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<DistrictVM> DropDownAll()
        {
            try
            {
                return new DistrictDAL().DropDownAll(connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}

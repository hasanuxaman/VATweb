using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using VATServer.Library;
using VATViewModel.DTOs;
using SymOrdinary;
using System.Web;


namespace SymRepository.VMS
{
    public class SectorsRepo
    {
        private SymOrdinary.ShampanIdentity identity;
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public SectorsRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public SectorsRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }


        public List<SectorsVM> DropDown() 
        {
            try
            {
                return new SectorsDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string[] InsertSectors(SectorsVM vm)
        {
            try
            {
                return new SectorsDAL().InsertSectors(vm, connVM,null,null);
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public string[] UpdateSectors(SectorsVM vm)
        {
            try
            {
                return new SectorsDAL().UpdateSectors(vm, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<SectorsVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new SectorsDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] Delete(SectorsVM vm, string[] ids, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new SectorsDAL().Delete(vm, ids, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}

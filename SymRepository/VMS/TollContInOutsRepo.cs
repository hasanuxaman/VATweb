using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
    public class TollContInOutsRepo
    {


        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public TollContInOutsRepo()
        {
            connVM = null;
        }
        public TollContInOutsRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public TollContInOutsRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        public ResultVM SaveData(TollContInOutVM Master, SqlTransaction transaction = null, SqlConnection currConn = null)
        {
            return new TollContInOutDAL().SaveData(Master, Master.Details, transaction, currConn, connVM);
           
        }

        public ResultVM UpdateData(TollContInOutVM Master, string UserId = "")
        {
            return new TollContInOutDAL().UpdateData(Master, Master.Details, connVM, UserId);
            
        }

        public ResultVM PostData(TollContInOutVM Master, string UserId = "")
        {
            return new TollContInOutDAL().PostData(Master, Master.Details, null, null, connVM, UserId);
           
        }

        public List<TollContInOutVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null)
        {
            return new TollContInOutDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, likeVM, connVM);
           
        }

        public List<TollContInOutDetailVM> SearchDetailList(string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            return new TollContInOutDAL().SearchDetailList(Id, VcurrConn, Vtransaction, connVM);
           
        }



    }
}

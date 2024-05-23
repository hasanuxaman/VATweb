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
    public class TollClientInOutRepo
    {
          
          private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public TollClientInOutRepo()
        {
            connVM = null;
        }
        public TollClientInOutRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public TollClientInOutRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        public ResultVM SaveData(TollClientInOutVM Master, SqlTransaction transaction = null, SqlConnection currConn = null)
        {
            return new TollClientInOutDAL().SaveData(Master, Master.Details, transaction, currConn, connVM);
        }

        public ResultVM UpdateData(TollClientInOutVM Master, string UserId = "")
        {
            return new TollClientInOutDAL().UpdateData(Master, Master.Details, connVM, UserId);
        }

        public ResultVM PostData(TollClientInOutVM Master, string UserId = "")
        {
            return new TollClientInOutDAL().PostData(Master, Master.Details, null, null, connVM, UserId);
        }

        public List<TollClientInOutVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null)
        {
            return new TollClientInOutDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, likeVM, connVM);
        }
     
        public List<TollClientInOutDetailVM> SearchDetailList(string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            return new TollClientInOutDAL().SearchDetailList(Id, VcurrConn, Vtransaction, connVM);
        }


    }
}

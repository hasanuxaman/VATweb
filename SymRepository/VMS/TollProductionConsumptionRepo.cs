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
    public class TollProductionConsumptionRepo
    {

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public TollProductionConsumptionRepo()
        {
            connVM = null;
        }
        public TollProductionConsumptionRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public TollProductionConsumptionRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        public ResultVM SaveData(TollProductionConsumptionVM Master, SqlTransaction transaction = null, SqlConnection currConn = null)
        {
            return new TollProductionConsumptionDAL().SaveData(Master, Master.Details, transaction, currConn, connVM);
        }

        //public string[] TollProductionConsumptionUpdate(TollProductionConsumptionVM Master, string UserId = "")
        public ResultVM UpdateData(TollProductionConsumptionVM Master, string UserId = "")
        {
            //return new TollProductionConsumptionDAL().TollProductionConsumptionUpdate(Master, Master.Details, connVM, UserId);
             return new TollProductionConsumptionDAL().UpdateData(Master, Master.Details, connVM, UserId);
        }

        //public string[] TollProductionConsumptionPost(TollProductionConsumptionVM Master, string UserId = "")
        public ResultVM PostData(TollProductionConsumptionVM Master, string UserId = "")
        {
            //return new TollProductionConsumptionDAL().TollProductionConsumptionPost(Master, Master.Details, null, null, connVM, UserId);
            return new TollProductionConsumptionDAL().PostData(Master, Master.Details, null, null, connVM, UserId);
        }

        public List<TollProductionConsumptionVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, IssueMasterVM likeVM = null)
        {
            return new TollProductionConsumptionDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, likeVM, connVM);
        }

        //public List<TollProductionConsumptionDetailVM> SelectTollProductionConsumptionDetail(string TollNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        //{
        //    return new TollProductionConsumptionDAL().SelectTollProductionConsumptionDetails(TollNo, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
        //}

        public List<TollProductionConsumptionDetailVM> SearchDetailList(string Id, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, SysDBInfoVMTemp connVM = null)
        {
            return new TollProductionConsumptionDAL().SearchDetailList(Id, VcurrConn, Vtransaction, connVM);
        }


    }
}

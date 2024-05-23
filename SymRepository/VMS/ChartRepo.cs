using SymOrdinary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VATServer.Library;
using VATViewModel.DTOs;

namespace SymRepository.VMS
{
    public class ChartRepo
    {

        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();

        public ChartRepo()
        {
            connVM = null;
        }

        public ChartRepo(SymOrdinary.ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public ChartRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        _9_1_VATReturnDAL _DAL = new _9_1_VATReturnDAL();

        public DataSet vat9_1forChartBar(VATReturnVM vm)
        {
            DataSet dataSet = new DataSet();

            try
            {
                dataSet = _DAL.SelectAllvat9_1forChartBar(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataSet;
        }

        public List<VATReturnVM> SelectAllvat9_1forChartPie(VATReturnVM vm)
        {
            List<VATReturnVM> dataSet = new List<VATReturnVM>();
            try
            {
                dataSet = _DAL.SelectAllvat9_1forChartPie(vm);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dataSet;
        }

        public DataTable GetDateWiseProduct(string fromDate, string toDate)
        {
            try
            {
                return new ChartDAL().GetDateWiseProduct(fromDate, toDate, connVM);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}

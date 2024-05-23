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
    public class PurchaseMPLRepo
    {
        private SysDBInfoVMTemp connVM = new SysDBInfoVMTemp();
        public PurchaseMPLRepo()
        {
             connVM = null;
        }
        public PurchaseMPLRepo(ShampanIdentity identity)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;
        }

        public PurchaseMPLRepo(ShampanIdentity identity, HttpSessionStateBase session)
        {
            connVM.SysDatabaseName = identity.InitialCatalog;
            connVM.SysUserName = SysDBInfoVM.SysUserName;
            connVM.SysPassword = SysDBInfoVM.SysPassword;
            connVM.SysdataSource = SysDBInfoVM.SysdataSource;

            connVM = Ordinary.StaticValueReAssign(identity, session);
        }

        public string[] PurchaseInsert(PurchaseInvoiceMPLHeadersVM Master, List<PurchaseInvoiceMPLDetailVM> Details, List<PurchaseDutiesVM> Duties, List<TrackingVM> Trackings, SqlTransaction transaction = null, SqlConnection currConn = null)
        {
            return new PurchaseMPLDAL().PurchaseInsert(Master, Details, Duties, Trackings, transaction, currConn,0,connVM);
        }

        public TDSCalcVM TDSCalculation(TDSCalcVM vm)
        {
            return new PurchaseMPLDAL().TDSCalculation(vm,connVM);
        }

        public List<PurchaseInvoiceMPLHeadersVM> SelectAll(int Id = 0, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null, PurchaseInvoiceMPLHeadersVM likeVm = null, string ItemNo = null, bool IsDisposeRawSearch = false)
        {
            try
            {
                return new PurchaseMPLDAL().SelectAllList(Id, conditionFields, conditionValues, VcurrConn, Vtransaction, likeVm, connVM,ItemNo,IsDisposeRawSearch);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<PurchaseInvoiceMPLDetailVM> SelectPurchaseDetail(string PurchaseInvoiceNo, string[] conditionFields = null, string[] conditionValues = null, SqlConnection VcurrConn = null, SqlTransaction Vtransaction = null)
        {
            try
            {
                return new PurchaseMPLDAL().SelectPurchaseDetailList(PurchaseInvoiceNo, conditionFields, conditionValues, VcurrConn, Vtransaction, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<TrackingVM> SelectTrakingsDetail(List<PurchaseInvoiceMPLDetailVM> DetailVMs, string ID, SysDBInfoVMTemp connVM = null)
        {
            try
            {
                return new PurchaseMPLDAL().GetTrackingsWeb(DetailVMs,ID,connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] PurchaseUpdate(PurchaseInvoiceMPLHeadersVM Master, List<PurchaseInvoiceMPLDetailVM> Details, List<PurchaseDutiesVM> Duties, List<TrackingVM> Trackings, string UserId = "")
        {
            return new PurchaseMPLDAL().PurchaseUpdate(Master, Details, Duties, Trackings, connVM,UserId);
        }

        public List<PurchaseInvoiceMPLHeadersVM> DropDown()
        {
            try
            {
                return new PurchaseMPLDAL().DropDown(connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string[] PurchasePost(PurchaseInvoiceMPLHeadersVM Master, List<PurchaseInvoiceMPLDetailVM> Details, List<PurchaseDutiesVM> Duties, List<TrackingVM> Trackings)
        {
            try
            {
                return new PurchaseMPLDAL().PurchasePost(Master, Details, Duties, Trackings, null, null, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public string[] ImportExcelFile(PurchaseInvoiceMPLHeadersVM vm)
        {
            try
            {

                return new PurchaseMPLDAL().ImportExcelFile(vm, connVM);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<PurchaseInvoiceMPLDetailVM> RateCheck(List<PurchaseInvoiceMPLDetailVM> VMs)
        {
            try
            {
                return new PurchaseMPLDAL().RateCheck(VMs, connVM);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<object> GetPurchaseColumn()
        {
            try
            {
                string[] columnName = new string[] { "Purchase Invoice No", "Serial No", "BE Number", "Vendor Name", "Import ID" };
                IEnumerable<object> enumList = from e in columnName select new { Id = e.ToString().Replace(" ", ""), Name = e.ToString() };
                return enumList;
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public string[] MultiplePost(string[] Ids)
        {
            try
            {
                return new PurchaseMPLDAL().MultiplePost(Ids, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable GetExcelDataWeb(List<string> Ids)
        {
            try
            {
                return new PurchaseMPLDAL().GetExcelDataWeb(Ids, true, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public MISExcelVM DownloadMIS_PurchaseReport(MISExcelVM vm)
        {
            try
            {
                PurchaseMPLDAL purchaseObj = new PurchaseMPLDAL();
                return purchaseObj.PurchaseMISExcelDownload(vm);
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public string[] MultipleRebate(List<string> IDs, string PeriodName)
        {
            try
            {
                return new PurchaseMPLDAL().MultipleRebateData(IDs, PeriodName, null, null, connVM);

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        #region API

        public string SavePurchase(string xml)
        {
            try
            {
                PurchaseAPI api = new PurchaseAPI();

                var result = api.SavePurchase(xml);

                return result;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        public ResultVM SaveBCPurchase(string xml)
        {
            try
            {
                PurchaseAPI api = new PurchaseAPI();

                ResultVM rVM = api.SavePurchaseBritishCouncil(xml);

                return rVM;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion
    }
}

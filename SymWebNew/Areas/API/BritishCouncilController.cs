using SymRepository.VMS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
using SymOrdinary;
using VATServer.Ordinary;
using VATViewModel.DTOs;
using System.IO;
using System.Web.Hosting;
using System.Security.Cryptography.X509Certificates;

namespace SymVATWebUI.Areas.API
{
    public class BritishCouncilController : ApiController
    {
        private AuthHelper _authHelper = new AuthHelper();

        [HttpPost]
        public HttpResponseMessage CreateSale([FromBody]XElement xml, [FromUri]string BIN)
        {
            string fileName = "Sales-" + DateTime.Now.ToString("yyyyMMdd HHmmss");

            FileLogger.WriteToFileTempForBC(xml.ToString() + "  " + BIN.ToString(), fileName, "BritishCouncilController", "CreateSale", false, true, false);

            var table = new DataTable();
            var resultSet = new DataSet();

            table.Columns.Add("Result");
            table.Columns.Add("Message");
            table.Rows.Add("fail", "Sale Import Fail");
            resultSet.Tables.Add(table);

            SaleAPIVM vm = new SaleAPIVM
            {
                Xml = xml.ToString(),
                Bin = BIN
            };

            bool CheckAuthorized = true;

            try
            {

                HttpContext httpContext = HttpContext.Current;

                ////////FileLogger.WriteToFileTempForBC("Before authHeader ", fileName + " (Authorization-01)", "BritishCouncilController", "CreateSale");

                string authHeader = httpContext.Request.Headers["Authorization"];

                ////////FileLogger.WriteToFileTempForBC("After authHeader ", fileName + " (Authorization-02)", "BritishCouncilController", "CreateSale");

                #region Comments

                //////try
                //////{

                //////    //////string path = HostingEnvironment.MapPath("~/Logs/Regular/Temp/");
                //////    //////path += fileName + " (New Authorization)" + ".txt";

                //////    //////FileLogger.WriteToFileTempForBC("Before authorizationHeader ", fileName + " (authorizationHeader-01)", "BritishCouncilController", "CreateSale");

                //////    //////var authorizationHeader = httpContext.Request.Headers["Authorization"];

                //////    //////FileLogger.WriteToFileTempForBC("After authorizationHeader ", fileName + " (authorizationHeader-02)", "BritishCouncilController", "CreateSale");

                //////    //////if (!string.IsNullOrEmpty(authorizationHeader))
                //////    //////{
                //////    //////    FileLogger.WriteToFileTempForBC("authorization Header string IsNullOrEmpty check ", fileName + " (authorizationHeader-03)", "BritishCouncilController", "CreateSale");

                //////    //////    try
                //////    //////    {
                //////    //////        using (StreamWriter writer = new StreamWriter(path))
                //////    //////        {
                //////    //////            writer.Write(authorizationHeader);
                //////    //////        }
                //////    //////    }
                //////    //////    catch (Exception ex)
                //////    //////    {
                //////    //////        FileLogger.WriteToFileTempForBC("StreamWriter error : " + ex.Message.ToString(), fileName + " (StreamWriterAuthorizationHeader-05)", "BritishCouncilController", "CreateSale");
                //////    //////    }

                //////    //////}
                //////    //////else
                //////    //////{

                //////    //////    ////FileLogger.WriteToFileTempForBC("authorization Header is empty ", fileName + " (EmptyAuthorizationHeader-04)", "BritishCouncilController", "CreateSale");

                //////    //////    throw new ArgumentNullException("", "The authorization header is empty.");

                //////    //////}
                //////}
                //////catch (Exception ex)
                //////{

                //////    //////FileLogger.WriteToFileTempForBC(ex.Message, fileName + " (EmptyAuthorizationHeader-04)", "BritishCouncilController", "CreateSale");

                //////    //////FileLogger.Log("BritishCouncilController", "CreateSale", ex.Message);

                //////    //////table.Rows[0]["Message"] = ex.Message;

                //////    //////if (CheckAuthorized)
                //////    //////{
                //////    //////    var response01 = new HttpResponseMessage() { Content = new StringContent(OrdinaryVATDesktop.GetXMLFromDataSet(resultSet)), StatusCode = HttpStatusCode.Unauthorized };
                //////    //////    response01.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                //////    //////    return response01;
                //////    //////}

                //////}

                #endregion

                try
                {

                    if (authHeader != null && authHeader.StartsWith("Basic"))
                    {
                        string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                        Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                        string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                        int seperatorIndex = usernamePassword.IndexOf(':');

                        vm.UserName = usernamePassword.Substring(0, seperatorIndex);
                        vm.Password = usernamePassword.Substring(seperatorIndex + 1);
                    }
                    else
                    {
                        throw new ArgumentNullException("", "The authorization header is either empty or isn't Basic.");
                    }

                    if (string.IsNullOrWhiteSpace(vm.Bin))
                    {
                        throw new ArgumentNullException("", "Bin number can not be empty");
                    }
                    if (string.IsNullOrWhiteSpace(vm.UserName) || string.IsNullOrWhiteSpace(vm.Password))
                    {
                        throw new ArgumentNullException("", "UserName or Password can not be empty");
                    }

                }
                catch (Exception ex)
                {
                    FileLogger.WriteToFileTempForBC(ex.Message, fileName + " (Authorization Exception-05)", "BritishCouncilController", "CreateSale", false, true, true);

                    FileLogger.Log("BritishCouncilController", "CreateSale", ex.Message);

                    table.Rows[0]["Message"] = ex.Message;

                    if (CheckAuthorized)
                    {
                        var response02 = new HttpResponseMessage() { Content = new StringContent(OrdinaryVATDesktop.GetXMLFromDataSet(resultSet)), StatusCode = HttpStatusCode.Unauthorized };
                        response02.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                        return response02;
                    }

                }

                if (string.IsNullOrWhiteSpace(vm.Xml))
                {
                    throw new ArgumentNullException("", "Xml can not be empty");
                }

                if (!CheckAuthorized)
                {
                    //////vm.UserName = "SAPUATDEV";
                    //////vm.Password = "BDvat@uat05";

                    //////if (string.IsNullOrWhiteSpace(vm.Bin))
                    //////{
                    //////    vm.Bin = "004681628-0000";
                    //////}
                }

                try
                {
                    _authHelper.Authenticate(vm);

                }
                catch (Exception ex)
                {
                    FileLogger.WriteToFileTempForBC(ex.Message, fileName + " (Authorization Exception-06)", "BritishCouncilController", "CreateSale", false, true, true);

                    FileLogger.Log("BritishCouncilController", "CreateSale", ex.Message);

                    table.Rows[0]["Message"] = ex.Message;

                    var response03 = new HttpResponseMessage() { Content = new StringContent(OrdinaryVATDesktop.GetXMLFromDataSet(resultSet)), StatusCode = HttpStatusCode.Unauthorized };
                    response03.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                    return response03;
                }

                SaleInvoiceRepo api = new SaleInvoiceRepo();

                vm.Xml = vm.Xml.Replace("&", "and");
                string pathRoot = AppDomain.CurrentDomain.BaseDirectory;

                ResultVM rVM = api.ImportSaleBritishCouncil(vm.Xml, pathRoot);

                #region Remove FileName and Log

                CommonRepo settingRepo = new CommonRepo();
                string flag = settingRepo.settings("Sale", "LogXML");
                //DataSet ds = OrdinaryVATDesktop.GetDataSetFromXML(result);

                if (flag == "Y")
                {
                    FileLogger.LogRegularSale("BritishCouncil", "CreateSale", vm.Xml, rVM.FileName);
                }

                table.Rows[0]["Result"] = rVM.Status;
                table.Rows[0]["Message"] = rVM.Message;

                ////ds.Tables[0].Columns.Remove("FileName");
                ////result = OrdinaryVATDesktop.GetXMLFromDataSet(ds);

                #endregion

                FileLogger.WriteToFileTempForBC(rVM.XML.ToString(), rVM.ResponseFileName, "BritishCouncilController", "CreateSale", true, true, false);

                var response = new HttpResponseMessage() { Content = new StringContent(rVM.XML), StatusCode = HttpStatusCode.Created };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

                return response;
                //return result;
            }
            catch (Exception e)
            {
                FileLogger.WriteToFileTempForBC(e.Message, fileName + " (Exception)", "BritishCouncilController", "CreateSale", false, true, true);

                FileLogger.Log("Sale", "SaveSale", e.Message + "\n" + e.StackTrace + "\n" + vm.Xml + "\n" + vm.Bin + "\n" + vm.Password + "\n" + vm.UserName);

                table.Rows[0]["Message"] = e.Message;

                //return  OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);
                var response = new HttpResponseMessage() { Content = new StringContent(OrdinaryVATDesktop.GetXMLFromDataSet(resultSet)), StatusCode = HttpStatusCode.BadRequest };
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
                return response;

            }
        }

        ////[HttpPost]
        ////public HttpResponseMessage CreateSaleBackup([FromBody]XElement xml, [FromUri]string BIN)
        ////{
        ////    string fileName = "Sales-" + DateTime.Now.ToString("yyyyMMdd HHmmss");

        ////    FileLogger.WriteToFileTempForBC(xml.ToString() + "  " + BIN.ToString(), fileName, "BritishCouncilController", "CreateSale");

        ////    var table = new DataTable();
        ////    var resultSet = new DataSet();

        ////    table.Columns.Add("Result");
        ////    table.Columns.Add("Message");
        ////    table.Rows.Add("fail", "Sale Import Fail");
        ////    resultSet.Tables.Add(table);

        ////    SaleAPIVM vm = new SaleAPIVM
        ////    {
        ////        Xml = xml.ToString(),
        ////        Bin = BIN
        ////    };

        ////    try
        ////    {

        ////        HttpContext httpContext = HttpContext.Current;

        ////        string authHeader = httpContext.Request.Headers["Authorization"];

        ////        if (authHeader != null && authHeader.StartsWith("Basic"))
        ////        {
        ////            string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
        ////            Encoding encoding = Encoding.GetEncoding("iso-8859-1");
        ////            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

        ////            int seperatorIndex = usernamePassword.IndexOf(':');

        ////            vm.UserName = usernamePassword.Substring(0, seperatorIndex);
        ////            vm.Password = usernamePassword.Substring(seperatorIndex + 1);
        ////        }
        ////        else
        ////        {
        ////            //Handle what happens if that isn't the case
        ////            throw new Exception("The authorization header is either empty or isn't Basic.");
        ////        }

        ////        if (string.IsNullOrWhiteSpace(vm.Bin))
        ////        {
        ////            throw new ArgumentNullException("Bin number can not be empty");
        ////        }
        ////        if (string.IsNullOrWhiteSpace(vm.UserName) || string.IsNullOrWhiteSpace(vm.Password))
        ////        {
        ////            throw new ArgumentNullException("UserName or Password can not be empty");
        ////        }
        ////        if (string.IsNullOrWhiteSpace(vm.Xml))
        ////        {
        ////            throw new ArgumentNullException("Xml can not be empty");
        ////        }

        ////        _authHelper.Authenticate(vm);

        ////        SaleInvoiceRepo api = new SaleInvoiceRepo();

        ////        vm.Xml = vm.Xml.Replace("&", "and");
        ////        string pathRoot = AppDomain.CurrentDomain.BaseDirectory;

        ////        ResultVM rVM = api.ImportSaleBritishCouncil(vm.Xml, pathRoot);

        ////        #region Remove FileName and Log

        ////        CommonRepo settingRepo = new CommonRepo();
        ////        string flag = settingRepo.settings("Sale", "LogXML");
        ////        //DataSet ds = OrdinaryVATDesktop.GetDataSetFromXML(result);

        ////        if (flag == "Y")
        ////        {
        ////            FileLogger.LogRegularSale("BritishCouncil", "CreateSale", vm.Xml, rVM.FileName);
        ////        }

        ////        table.Rows[0]["Result"] = rVM.Status;
        ////        table.Rows[0]["Message"] = rVM.Message;

        ////        ////ds.Tables[0].Columns.Remove("FileName");
        ////        ////result = OrdinaryVATDesktop.GetXMLFromDataSet(ds);

        ////        #endregion

        ////        var response = new HttpResponseMessage() { Content = new StringContent(rVM.XML), StatusCode = HttpStatusCode.Created };
        ////        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");

        ////        return response;
        ////        //return result;
        ////    }
        ////    catch (Exception e)
        ////    {
        ////        FileLogger.WriteToFileTempForBC(e.Message, fileName + " (Exception)", "BritishCouncilController", "CreateSale");


        ////        FileLogger.Log("Sale", "SaveSale", e.Message + "\n" + e.StackTrace + "\n" + vm.Xml + "\n" + vm.Bin + "\n" + vm.Password + "\n" + vm.UserName);

        ////        table.Rows[0]["Message"] = e.Message;

        ////        //return  OrdinaryVATDesktop.GetXMLFromDataSet(resultSet);
        ////        var response = new HttpResponseMessage() { Content = new StringContent(OrdinaryVATDesktop.GetXMLFromDataSet(resultSet)), StatusCode = HttpStatusCode.Created };
        ////        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/xml");
        ////        return response;

        ////    }
        ////}


    }
}

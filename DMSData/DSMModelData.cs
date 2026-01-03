using DSMData.Model;
using System;
using System.IO;

using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Configuration;

namespace DSMData
{
    public class DSMModelData
    {
        public DSMdbEntities dsmcontext;

        public DSMModelData()
        {
            dsmcontext = new DSMdbEntities();
        }

        public string CreateUser(UserDisplayModel userData)
        {
            string res = string.Empty;
            using (var context = new DSMdbEntities())
            {
                var user = context.Users.Where(x => x.Username.ToLower() == userData.UserName.ToLower()).FirstOrDefault();
                if (user == null)
                {
                    User entUser = new User();
                    entUser.Username = userData.UserName;
                    entUser.Password = userData.Password;
                    entUser.UserType = userData.UserType;
                    context.Users.Add(entUser);
                    context.SaveChanges();
                    res = "Success";
                }
                else
                {
                    res = "Exist";
                }
            }
            return res;
        }

        public List<DigitalSignDetail> GetDSdetailbyClientId(string ClientName)
        {
            List<DigitalSignDetail> listDS = new List<DigitalSignDetail>();
            var client = dsmcontext.ClientMasters.Where(x => x.ClientName == ClientName).FirstOrDefault();
            if (client != null)
            {
                listDS = dsmcontext.DigitalSignDetails.Where(x => x.ClientId == client.ClientId).ToList();
            }
            return listDS;
        }

        public List<DSMasterDisplayModel> GetDSdetailbyClientLineId(int ClientLineId)
        {
            List<DSMasterDisplayModel> listDS = new List<DSMasterDisplayModel>();
            var client = dsmcontext.ClientMasters.Where(x => x.ClientLineId == ClientLineId && x.IsActive == true).FirstOrDefault();
            if (client != null)
            {
                listDS = dsmcontext.DigitalSignDetails.Where(x => x.ClientId == client.ClientId && x.IsActive == true).Select(x => new DSMasterDisplayModel()
                {
                    KeyName = x.KeyName,
                    KeyPinNo = x.KeyPinNo
                    //ClientId = x.ClientId
                }).ToList();
            }
            return listDS;
        }

        public List<DSMasterDisplayModel> GetDSdetailbyKeyName(int ClientLineId)
        {
            List<DSMasterDisplayModel> listDS = new List<DSMasterDisplayModel>();
            var client = dsmcontext.ClientMasters.Where(x => x.ClientLineId == ClientLineId && x.IsActive == true).FirstOrDefault();
            if (client != null)
            {
                listDS = dsmcontext.DigitalSignDetails.Where(x => x.ClientId == client.ClientId && x.IsActive == true).Select(x => new DSMasterDisplayModel()
                {
                    KeyName = x.KeyName,
                    KeyPinNo = x.KeyPinNo
                    //ClientId = x.ClientId
                }).ToList();
            }
            return listDS;
        }

        //public List<DigitalSignDetailDisplayModel> GetDSdetailbyClientLineId(int ClientLineId)
        //{
        //    List<DigitalSignDetailDisplayModel> listDS = new List<DigitalSignDetailDisplayModel>();
        //    var client = dsmcontext.ClientMasters.Where(x => x.ClientLineId == ClientLineId && x.IsActive == true).FirstOrDefault();
        //    if (client != null)
        //    {
        //        listDS = dsmcontext.DigitalSignDetails.Where(x => x.ClientId == client.ClientId && x.IsActive == true).Select(x => new DigitalSignDetailDisplayModel()
        //        {
        //            KeyName = x.KeyName,
        //            KeyPinNo = x.KeyPinNo,
        //            ClientId =x.ClientId
        //        }).ToList();
        //    }
        //    return listDS;
        //}

        public DSMaster GetDSmasterByKeyName(string KeyName)
        {
            DSMaster entDSMaster = new DSMaster();

            entDSMaster = dsmcontext.DSMasters.Where(x => x.KeyName == KeyName).OrderByDescending(y => y.CreatedDate).FirstOrDefault();

            return entDSMaster;
        }

        public List<DSMaster> GetAllKeysDSmaster()
        {
            List<DSMaster> lstDSMaster = new List<DSMaster>();

            lstDSMaster = dsmcontext.DSMasters.OrderByDescending(y => y.CreatedDate).ToList();

            return lstDSMaster;
        }

        public List<DSMasterDisplayModel> GetAllDSKeys()
        {
            List<DSMasterDisplayModel> lstDSMaster = new List<DSMasterDisplayModel>();

            lstDSMaster = dsmcontext.DSMasters.Where(x => x.IsActive == true).OrderByDescending(y => y.CreatedDate).Select(x => new DSMasterDisplayModel()
            {
                DSMasterId = x.DSMasterId,
                KeyName = x.KeyName,
                KeyPinNo = x.KeyPinNo,
                SignName = x.SignName,
                Department = x.Department,
                Designation = x.Designation,
                ValidityAfter = x.ValidityAfter.Value,
                ValidityBefore = x.ValidityBefore.Value
            }).ToList();

            return lstDSMaster;
        }


        public void UpdateInvoiceDSstatusByInvoiceNo(string InvoiceNo)
        {
            using (var context = new DSMdbEntities())
            {
                var lstinvoice = context.Invoices.Where(x => x.InvNumber == InvoiceNo).ToList();
                if (lstinvoice.Count > 0)
                {
                    foreach (var inv in lstinvoice)
                    {
                        inv.DSStatus = "TRUE";
                       
                        inv.UpdatedDate = DateTime.Now;
                    }
                    context.SaveChanges();
                }
            }
        }

        public void UpdateUserExitDetails(string UserName)
        {
            using (var context = new DSMdbEntities())
            {
                //var lstinvoice = context.Invoices.Where(x => x.InvNumber == InvoiceNo).ToList();
                var lstusers = context.Users.Where(x => x.Username == UserName.ToString().Trim()).ToList();
                if (lstusers.Count > 0)
                {
                    foreach (var user in lstusers)
                    {
                        //user.IsActive = false;
                        user.LoginStatus = "FALSE";
                        //inv.UpdatedDate = DateTime.Now;
                    }
                    context.SaveChanges();
                }
            }
        }

        public void UpdateUserEntryDetails(string UserName)
        {
            using (var context = new DSMdbEntities())
            {
                //var lstinvoice = context.Invoices.Where(x => x.InvNumber == InvoiceNo).ToList();
                var lstusers = context.Users.Where(x => x.Username == UserName.ToString().Trim()).ToList();
                if (lstusers.Count > 0)
                {
                    foreach (var user in lstusers)
                    {
                        //user.IsActive = true;
                        user.LoginStatus = "TRUE";
                        //inv.UpdatedDate = DateTime.Now;
                    }
                    context.SaveChanges();
                }
            }
        }

        public void UpdateInvoicePrintstatusByInvoiceNo(string InvoiceNo, string printStatus)
        {
            using (var context = new DSMdbEntities())
            {
                var lstinvoice = context.Invoices.Where(x => x.InvNumber == InvoiceNo).ToList();
                if (lstinvoice.Count > 0)
                {
                    foreach (var inv in lstinvoice)
                    {
                        inv.PrintStatus = printStatus;
                        inv.UpdatedDate = DateTime.Now;
                    }
                    context.SaveChanges();
                }
            }
        }

        public void UpdateEmailstatusByInvoiceNo(string InvoiceNo, bool emailStatus)
        {
            using (var context = new DSMdbEntities())
            {
                var lstinvoice = context.Invoices.Where(x => x.InvNumber == InvoiceNo).ToList();
                if (lstinvoice.Count > 0)
                {
                    foreach (var inv in lstinvoice)
                    {
                        inv.IsEmailSend = emailStatus;
                        inv.UpdatedDate = DateTime.Now;
                    }
                    context.SaveChanges();
                }
            }
        }

        public void UpdateInvoiceAPIstatusByInvoiceNo(string InvoiceNo, string APIStatus, string APIMessage)
        {
            using (var context = new DSMdbEntities())
            {
                var lstinvoice = context.Invoices.Where(x => x.InvNumber == InvoiceNo).ToList();
                if (lstinvoice.Count > 0)
                {
                    foreach (var inv in lstinvoice)
                    {
                        inv.APIStatus = APIStatus;
                        inv.APIMessage = APIMessage;
                        inv.UpdatedDate = DateTime.Now;
                    }
                    context.SaveChanges();
                }
            }
        }

        public void UpdateInvoiceAPIstatusByInvoiceNoFinYear(string InvoiceNo, string APIStatus, string APIMessage, int FinYear)
        {
            using (var context = new DSMdbEntities())
            {
                var lstinvoice = context.Invoices.Where(x => x.InvNumber == InvoiceNo && x.Finyear == FinYear).ToList();
                if (lstinvoice.Count > 0)
                {
                    foreach (var inv in lstinvoice)
                    {
                        inv.APIStatus = APIStatus;
                        inv.APIMessage = APIMessage;
                        inv.UpdatedDate = DateTime.Now;
                    }
                    context.SaveChanges();
                }
            }
        }

        public void UpdateHXValueByInvoiceNo(string pdfType, byte[] hexafile, string InvoiceNo)
        {
            using (var context = new DSMdbEntities())
            {
                var lstinvoice = context.Invoices.Where(x => x.InvNumber == InvoiceNo).ToList();
                if (lstinvoice.Count > 0)
                {
                    foreach (var inv in lstinvoice)
                    {
                        if (pdfType == "ORIGINAL")
                        {
                            inv.OriginalPDF = hexafile;
                        }
                        else if (pdfType == "DUPLICATE")
                        {
                            inv.DuplicatePDF = hexafile;
                        }
                        else if (pdfType == "TRIPLICATE")
                        {
                            inv.QuadraplicatePDF = hexafile;
                        }
                        else if (pdfType == "EXTRA")
                        {
                            inv.ExtraPDF = hexafile;
                        }
                        inv.UpdatedDate = DateTime.Now;
                    }
                    context.SaveChanges();
                }
            }
        }

        public UserDisplayModel GetUserType(string UserName, string Password)
        {
            UserDisplayModel objUser = new UserDisplayModel();
            //var user = dsmcontext.Users.Where(x => x.Username == UserName && x.Password == Password && x.IsActive == true).FirstOrDefault();
            var user = dsmcontext.Users.Where(x => x.Username == UserName && x.Password == Password).FirstOrDefault();
            if (user != null)
            {
                objUser.UserId = user.UserId;
                objUser.UserName = user.Username;
                objUser.UserType = user.UserType;
                
            }
            return objUser;
        }

        //public UserDisplayModel GetApiUserType(string UserName)
        //{
        //    UserDisplayModel objUser = new UserDisplayModel();
        //    //var user = dsmcontext.Users.Where(x => x.Username == UserName && x.Password == Password && x.IsActive == true).FirstOrDefault();
        //    //var Apiuser;
        //    var Apiuser = dsmcontext.Users.Where(x => x.Username == UserName).FirstOrDefault();
        //    if (Apiuser != null)
        //    {
        //        objUser.UserId = Apiuser.UserId;
        //        objUser.UserName = Apiuser.Username;
        //        objUser.UserType = Apiuser.UserType;
        //        objUser.UserIsActive = Apiuser.IsActive == true ? 1 : 0;
        //        objUser.LoginStatus = Apiuser.LoginStatus;
        //    }
        //    return objUser;




        //}

        public List<UserDisplayModel> GetApiUserType(string UserName)
        {
            List<UserDisplayModel> lstUsers = new List<UserDisplayModel>();
            //var user = dsmcontext.Users.Where(x => x.Username == UserName && x.Password == Password && x.IsActive == true).FirstOrDefault();
            //var Apiuser;
            //var invDet = dsmcontext.Invoices.Where(x => x.DSStatus == "TRUE" && x.APIStatus == "FALSE").GroupBy(n => new { n.InvNumber })
            //                                               .Select(g => g.FirstOrDefault())
            //                                               .ToList();

            //return invDet;
            using (var context = new DSMdbEntities())
            {
                lstUsers = context.Users.Where(x => x.Username == UserName).Select(x => new UserDisplayModel()
                {
                    UserId = x.UserId,
                    UserName = x.Username,
                    UserType = x.UserType,
                    LoginStatus = x.LoginStatus
                }).ToList();
            }

            return lstUsers;

            //var Apiuser = dsmcontext.Users.Where(x => x.Username == UserName).FirstOrDefault();
            //if (Apiuser != null)
            //{
            //    objUser.UserId = Apiuser.UserId;
            //    objUser.UserName = Apiuser.Username;
            //    objUser.UserType = Apiuser.UserType;
            //    objUser.UserIsActive = Apiuser.IsActive == true ? 1 : 0;
            //    objUser.LoginStatus = Apiuser.LoginStatus;
            //}
            //return objUser;




        }
        public List<Invoice> GetInvoiceDetailsToReprocess(string invno, string PO, string Part_no, string qty)
        { 
            var invDet = dsmcontext.Invoices.Where(x => x.PONumber == PO && x.PartNumber == Part_no && x.InvQuantity == qty && x.InvNumber == invno).ToList();
            return invDet;
        }
        public SettingsDisplayModel GetSettingsData(string sysName)
        {
            SettingsDisplayModel objSettings = new SettingsDisplayModel();
            var settings = dsmcontext.Settings.Where(x => x.SystemName == sysName).FirstOrDefault();
            if (settings != null)
            {
                objSettings.SettingsId = settings.SettingsId;
                objSettings.SystemName = settings.SystemName;
                objSettings.PrinterName = settings.PrinterName;
                objSettings.InputPath = settings.InputPath;
                objSettings.OutputPath = settings.OutputPath;
                objSettings.InvoicePath = settings.InvoicePath;
                objSettings.FlatFile = settings.Flatfilepath;
                //objSettings.ApiUrl = settings.APIUrl;
            }
            return objSettings;
        }


        public void SaveSettings(SettingsDisplayModel settingsData)
        {
            using (var context = new DSMdbEntities())
            {
                var settings = context.Settings.Where(x => x.SystemName == settingsData.SystemName).FirstOrDefault();
                if (settings != null)
                {
                    settings.PrinterName = settingsData.PrinterName;
                    settings.InputPath = settingsData.InputPath;
                    settings.OutputPath = settingsData.OutputPath;
                    settings.InvoicePath = settingsData.InvoicePath;
                    settings.Flatfilepath = settingsData.FlatFile;
                    settings.UpdatedDate = DateTime.Now;
                    context.SaveChanges();
                }
                else
                {
                    Setting entSettings = new Setting();
                    entSettings.SystemName = settingsData.SystemName;
                    entSettings.PrinterName = settingsData.PrinterName;
                    entSettings.InputPath = settingsData.InputPath;
                    entSettings.OutputPath = settingsData.OutputPath;
                    entSettings.InvoicePath = settingsData.InvoicePath;
                    entSettings.Flatfilepath = settingsData.FlatFile;
                    entSettings.CreatedDate = DateTime.Now;
                    context.Settings.Add(entSettings);
                    context.SaveChanges();
                }
            }
        }

        public List<Invoice> GetInvoiceDetailsByInvoiceNumber(string InvoiceNumber)
        {
            var invDet = dsmcontext.Invoices.Where(x => x.InvNumber == InvoiceNumber).ToList();
            return invDet;
        }

        public List<Invoice> GetInvoiceDetailsByInvoiceNumberFinYear(string InvoiceNumber,int Finyear)
        {
            //var invDet = dsmcontext.Invoices.Where(x => x.InvNumber == InvoiceNumber && x.Finyear == Finyear.Trim()).ToList();
            //var invDet = dsmcontext.Invoices.Where(x => x.InvNumber == InvoiceNumber).ToList();
            var invDet = dsmcontext.Invoices.Where(x => x.InvNumber == InvoiceNumber && x.Finyear == Finyear && x.PDF_Generation!="Processed").ToList();
            return invDet;
        }

        public Invoice GetInvoiceByInvoiceNumber(string InvoiceNumber)
        {
            var invDet = dsmcontext.Invoices.Where(x => x.InvNumber == InvoiceNumber).FirstOrDefault();
            
            return invDet;
        }

        public byte[] GetHexValueByInvoiceNumber(string InvoiceNumber)
        {
            byte[] originalPDF = new byte[0];
            var invDet = dsmcontext.Invoices.Where(x => x.InvNumber == InvoiceNumber).FirstOrDefault();
            if (invDet != null)
            {
                originalPDF = invDet.OriginalPDF;
            }
            return originalPDF;
        }

        //public List<Invoice> GetInvoiceDetailsToReprocess(string invno, string PO, string Part_no, string qty)
        //{
        //    //var invDet = dsmcontext.Invoices.Where(x => x.InvNumber == InvoiceNumber && x.Finyear == Finyear.Trim()).ToList();
        //    //var invDet = dsmcontext.Invoices.Where(x => x.InvNumber == InvoiceNumber).ToList();
        //    var invDet = dsmcontext.Invoices.Where(x => x.PONumber == PO && x.PartNumber == Part_no && x.InvQuantity == qty && x.InvNumber == invno).ToList();
        //    return invDet;
        //}


        //public List<InvoiceDisplayModel> GetAllInvoiceStatus()
        //{
        //    List<InvoiceDisplayModel> lstInv = new List<InvoiceDisplayModel>();
        //    using (var context = new DSMdbEntities())
        //    {
        //        List<Invoice> listObjects = (from obj in context.Invoices
        //                                     select obj).GroupBy(n => new { n.InvNumber })
        //                                                   .Select(g => g.FirstOrDefault())
        //                                                   .ToList();

        //        lstInv = listObjects.Select(x => new InvoiceDisplayModel()
        //        {
        //            InvNumber = x.InvNumber,
        //            InvDate = x.InvDate.Value,
        //            DSStatus = x.DSStatus,
        //            PrintStatus = x.PrintStatus,
        //            ApiStatus = x.APIStatus,
        //            ApiMsg = x.APIMessage,
        //            CreatedDate = x.UpdatedDate == null ? x.CreatedDate.Value : x.UpdatedDate.Value
        //        }).ToList();
        //    }
        //    return lstInv.Where(c => c.CreatedDate.Date == DateTime.Now.Date).OrderByDescending(x => x.CreatedDate).ToList();
        //    //return lstInv;
        //}

        //public List<InvoiceDisplayModel> GetAllInvoiceStatus()
        //{
        //    List<InvoiceDisplayModel> lstInv = new List<InvoiceDisplayModel>();
        //    using (var context = new DSMdbEntities())
        //    {
        //        lstInv = context.SP_GET_INVOICE().Select(x => new InvoiceDisplayModel()
        //        {
        //            InvNumber = x.InvNumber,
        //            InvDate = x.InvDate.Value,
        //            DSStatus = x.DSStatus,
        //            PrintStatus = x.PrintStatus,
        //            ApiStatus = x.APIStatus,
        //            ApiMsg = x.APIMessage

        //        }).ToList();
        //    }

        //    return lstInv;
        //}

        public List<InvoiceDisplayModel> GetAllInvoiceStatus()
        {
            try
            {
                List<InvoiceDisplayModel> lstInv = new List<InvoiceDisplayModel>();
                using (var context = new DSMdbEntities())
                {
                    lstInv = context.SP_GET_INVOICE().Select(x => new InvoiceDisplayModel()
                    {
                        InvNumber = x.InvNumber,
                        InvDate = x.InvDate.Value,
                        DSStatus = x.DSStatus,
                        PrintStatus = x.PrintStatus,
                        ApiStatus = x.APIStatus,
                        ApiMsg = x.APIMessage,
                        CreatedDate = x.UpdatedDate == null ? x.CreatedDate.Value : x.UpdatedDate.Value
                    }).ToList();
                }

                return lstInv;
            }
            catch(Exception ex)
            {
                List<InvoiceDisplayModel> lstInv = new List<InvoiceDisplayModel>();
                MessageBox.Show("Server Disconnected");
                return lstInv;
            }
        }
        //public List<InvoiceDisplayModel> GetAllSYSInvoiceStatus()
        //{
        //    List<InvoiceDisplayModel> lstInv = new List<InvoiceDisplayModel>();
        //    using (var context = new DSMdbEntities())
        //    {
        //        List<Invoice> listObjects = (from obj in context.Invoices
        //                                     select obj).GroupBy(n => new { n.InvNumber })
        //                                                   .Select(g => g.FirstOrDefault())
        //                                                   .ToList();

        //        lstInv = listObjects.Select(x => new InvoiceDisplayModel()
        //        {
        //            InvNumber = x.InvNumber,
        //            InvDate = x.InvDate.Value,
        //            DSStatus = x.DSStatus,
        //            PrintStatus = x.PrintStatus,
        //            ApiStatus = x.APIStatus,
        //            ApiMsg = x.APIMessage,
        //            CreatedDate = x.UpdatedDate == null ? x.CreatedDate.Value : x.UpdatedDate.Value
        //        }).ToList();
        //    }
        //    return lstInv.Where(c => c.CreatedDate.Date.ToString("dd-MM-yyyy") == DateTime.Now.ToString("dd-MM-yyyy")).OrderByDescending(x => x.CreatedDate).ToList();
        //    //return lstInv;
        //}

        //public List<InvoiceDisplayModel> GetAllSYSInvoiceStatus()
        //{
        //    List<InvoiceDisplayModel> lstInv = new List<InvoiceDisplayModel>();
        //    using (var context = new DSMdbEntities())
        //    {
        //        lstInv = context.SP_GET_SYSINVOICE(System.Environment.MachineName).Select(x => new InvoiceDisplayModel()
        //        {
        //            InvNumber = x.InvNumber,
        //            InvDate = x.InvDate.Value,
        //            DSStatus = x.DSStatus,
        //            PrintStatus = x.PrintStatus,
        //            ApiStatus = x.APIStatus,
        //            ApiMsg = x.APIMessage,
        //            CreatedDate = x.UpdatedDate == null ? x.CreatedDate.Value : x.UpdatedDate.Value
        //        }).ToList();
        //    }

        //    return lstInv;
        //}

        public List<InvoiceDisplayModel> GetAllSearchInvoiceStatus(string InvNo)
        {
            List<InvoiceDisplayModel> lstInv = new List<InvoiceDisplayModel>();
            using (var context = new DSMdbEntities())
            {
                lstInv = context.SP_GET_SEARCHINVOICE(InvNo).Select(x => new InvoiceDisplayModel()
                {
                    InvNumber = x.InvNumber,
                    InvDate = x.InvDate.Value,
                    DSStatus = x.DSStatus,
                    PrintStatus = x.PrintStatus,
                    ApiStatus = x.APIStatus,
                    ApiMsg = x.APIMessage,
                    CreatedDate = x.UpdatedDate == null ? x.CreatedDate.Value : x.UpdatedDate.Value
                }).ToList();
            }

            return lstInv;
        }
        public List<InvoiceDisplayModel> GetAllReprocessInvoiceStatus()
        {
            List<InvoiceDisplayModel> lstInv = new List<InvoiceDisplayModel>();
            using (var context = new DSMdbEntities())
            {
                List<Invoice> listObjects = (from obj in context.Invoices
                                             select obj).Where(n => n.PDF_Generation == "Reprocessed").GroupBy(n => new { n.InvNumber })
                                                           .Select(g => g.FirstOrDefault())
                                                           .ToList();

                lstInv = listObjects.Select(x => new InvoiceDisplayModel()
                {
                    InvNumber = x.InvNumber,
                    InvDate = x.InvDate.Value,
                    DSStatus = x.DSStatus,
                    PrintStatus = x.PrintStatus,
                    ApiStatus = x.APIStatus,
                    ApiMsg = x.APIMessage,
                    CreatedDate = x.UpdatedDate == null ? x.CreatedDate.Value : x.UpdatedDate.Value
                }).ToList();
            }
            return lstInv.OrderByDescending(x => x.CreatedDate).ToList();
            //return lstInv;
        }

        public List<InvoiceDisplayModel> GetAllSYSInvoiceStatus()
        {
            List<InvoiceDisplayModel> lstInv = new List<InvoiceDisplayModel>();
            using (var context = new DSMdbEntities())
            {
                lstInv = context.SP_GET_INVOICE().Select(x => new InvoiceDisplayModel()
                {
                    InvNumber = x.InvNumber,
                    InvDate = x.InvDate.Value,
                    DSStatus = x.DSStatus,
                    PrintStatus = x.PrintStatus,
                    ApiStatus = x.APIStatus,
                    ApiMsg = x.APIMessage

                }).ToList();
            }

            return lstInv;
        }

        public List<InvoiceDisplayModel> GetAllManualInvoiceStatus()
        {
            List<InvoiceDisplayModel> lstInv = new List<InvoiceDisplayModel>();
            using (var context = new DSMdbEntities())
            {
                List<Invoice> listObjects = (from obj in context.Invoices
                                             select obj).GroupBy(n => new { n.InvNumber })
                                                           .Select(g => g.FirstOrDefault())
                                                           .ToList();

                lstInv = listObjects.Select(x => new InvoiceDisplayModel()
                {
                    InvNumber = x.InvNumber,
                    InvDate = x.InvDate.Value,
                    DSStatus = x.DSStatus,
                    PrintStatus = x.PrintStatus,
                    ApiStatus = x.APIStatus,
                    ApiMsg = x.APIMessage,
                    CreatedDate = x.UpdatedDate == null ? x.CreatedDate.Value : x.UpdatedDate.Value
                }).ToList();
            }
            //return lstInv.Where(c => c.CreatedDate.Date == DateTime.Now.Date).OrderByDescending(x => x.CreatedDate).ToList();
            return lstInv;
        }

        //public List<InvoiceDisplayModel> GetAllInvoiceStatusAll()
        //{
        //    List<InvoiceDisplayModel> lstInv = new List<InvoiceDisplayModel>();
        //    using (var context = new DSMdbEntities())
        //    {
        //        List<Invoice> listObjects = (from obj in context.Invoices
        //                                     select obj).GroupBy(n => new { n.InvNumber })
        //                                                   .Select(g => g.FirstOrDefault())
        //                                                   .ToList();

        //        lstInv = listObjects.Select(x => new InvoiceDisplayModel()
        //        {
        //            InvNumber = x.InvNumber,
        //            InvDate = x.InvDate.Value,
        //            DSStatus = x.DSStatus,
        //            PrintStatus = x.PrintStatus,
        //            ApiStatus = x.APIStatus,
        //            ApiMsg = x.APIMessage,
        //            IsEmailSend = x.IsEmailSend == null ? false : x.IsEmailSend.Value,
        //            CreatedDate = x.UpdatedDate == null ? x.CreatedDate.Value : x.UpdatedDate.Value
        //        }).ToList();
        //    }
        //    //return lstInv.OrderByDescending(x => x.CreatedDate).ToList();
        //    return lstInv.OrderByDescending(x => x.CreatedDate).ToList();
        //}

        public List<InvoiceDisplayModel> GetAllInvoiceStatusAll()
        {
            List<InvoiceDisplayModel> lstInv = new List<InvoiceDisplayModel>();
            using (var context = new DSMdbEntities())
            {
                List<Invoice> listObjects = (from obj in context.Invoices
                                             select obj).GroupBy(n => new { n.InvNumber })
                                                           .Select(g => g.FirstOrDefault())
                                                           .ToList();

                lstInv = listObjects.Select(x => new InvoiceDisplayModel()
                {
                    InvNumber = x.InvNumber,
                    InvDate = x.InvDate.Value,
                    DSStatus = x.DSStatus,
                    PrintStatus = x.PrintStatus,
                    ApiStatus = x.APIStatus,
                    ApiMsg = x.APIMessage,
                    IsEmailSend = x.IsEmailSend == null ? false : x.IsEmailSend.Value
                    
                }).ToList();
            }
            //return lstInv.OrderByDescending(x => x.CreatedDate).ToList();
            return lstInv.OrderByDescending(x => x.CreatedDate).ToList();
        }

        //public List<InvoiceDisplayModel> GetAllAPIInvoiceStatusAll()
        //{
        //    List<InvoiceDisplayModel> lstInv = new List<InvoiceDisplayModel>();
        //    using (var context = new DSMdbEntities())
        //    {
        //        List<Invoice> listObjects = (from obj in context.Invoices
        //                                     where obj.DSStatus == "TRUE" && obj.APIStatus == "FALSE"
        //                                     select obj).GroupBy(n => new { n.InvNumber })
        //                                                   .Select(g => g.FirstOrDefault())
        //                                                   .ToList();

        //        //lstInv = listObjects.Select(x => new InvoiceDisplayModel()
        //        //{
        //        //    InvNumber = x.InvNumber,
        //        //    InvDate = x.InvDate.Value,
        //        //    DSStatus = x.DSStatus,
        //        //    PrintStatus = x.PrintStatus,
        //        //    ApiStatus = x.APIStatus,
        //        //    ApiMsg = x.APIMessage,
        //        //    IsEmailSend = x.IsEmailSend == null ? false : x.IsEmailSend.Value,
        //        //    CreatedDate = x.UpdatedDate == null ? x.CreatedDate.Value : x.UpdatedDate.Value
        //        //}).ToList();
        //        lstInv = listObjects.Select(x => new InvoiceDisplayModel()
        //        {
        //            InvNumber = x.InvNumber,
        //        }).ToList();
        //    }
        //    return lstInv.OrderByDescending(x => x.CreatedDate).ToList();
        //    //return lstInv;
        //}

        public List<Invoice> GetAllAPIInvoiceStatusAll()
        {
            //var invDet = dsmcontext.Invoices.Where(x => x.DSStatus == "TRUE" && x.APIStatus == "FALSE").ToList();
            var invDet = dsmcontext.Invoices.Where(x => x.DSStatus == "TRUE" && x.APIStatus == "FALSE").GroupBy(n => new { n.InvNumber })
                                                           .Select(g => g.FirstOrDefault())
                                                           .ToList();

            return invDet;
        }

        public List<Invoice> GetAllAPIInvoiceStatusAll(int Finyear)
        {
            //var invDet = dsmcontext.Invoices.Where(x => x.DSStatus == "TRUE" && x.APIStatus == "FALSE").ToList();
            //var invDet = dsmcontext.Invoices.Where(x => x.DSStatus == "TRUE" && x.APIStatus == "FALSE" && x.Finyear == Finyear).GroupBy(n => new { n.InvNumber })
            //                                               .Select(g => g.FirstOrDefault())
            //                                               .ToList();
            //var invDet = dsmcontext.Invoices.Where(x => x.DSStatus == "TRUE" && (x.APIStatus == "FALSE" || x.APIStatus == "SKIPPEDAPI") && x.Finyear == Finyear).GroupBy(n => new { n.InvNumber })
            //                                               .Select(g => g.FirstOrDefault())
            //                                               .ToList();
            var invDet = dsmcontext.Invoices.Where(x => x.DSStatus == "TRUE" && (x.APIStatus == "FALSE" || x.APIStatus == "SKIPPEDAPI") && x.Finyear == Finyear).GroupBy(n => new { n.InvNumber })
                                                          .Select(g => g.FirstOrDefault())
                                                          .ToList();

            return invDet;
        }

        //public System.Collections.Generic.List<DSMData.User> GetApiUserType(string UserName)
        //{
        //    //UserDisplayModel objUser = new UserDisplayModel();
        //    //var user = dsmcontext.Users.Where(x => x.Username == UserName && x.Password == Password && x.IsActive == true).FirstOrDefault();
        //    //var Apiuser;
        //    var Apiuser = dsmcontext.Users.Where(x => x.Username == UserName).FirstOrDefault();
            
        //    return Apiuser;

            


        //}
        public List<InvoiceDisplayModel> GetSelectedInvoice(string InvoiceNo)
        {
            List<InvoiceDisplayModel> lstInv = new List<InvoiceDisplayModel>();
            using (var context = new DSMdbEntities())
            {
                lstInv = context.SP_GET_QUARNINVOICE(InvoiceNo).Select(x => new InvoiceDisplayModel()
                {
                    InvNumber = x.InvNumber,
                    InvDate = x.InvDate.Value,
                    DSStatus = x.DSStatus,
                    PrintStatus = x.PrintStatus,
                    ApiStatus = x.APIStatus,
                    ApiMsg = x.APIMessage

                }).ToList();
            }

            return lstInv;
        }

        //public List<InvoiceDisplayModel> GetSelectedInvoice(string InvoiceNo)
        //{
        //    List<InvoiceDisplayModel> lstInv = new List<InvoiceDisplayModel>();
        //    using (var context = new DSMdbEntities())
        //    {
        //        List<Invoice> listObjects = (from obj in context.Invoices
        //                                     select obj).GroupBy(n => new { n.InvNumber })
        //                                                   .Select(g => g.FirstOrDefault())
        //                                                   .ToList();

        //        lstInv = listObjects.Select(x => new InvoiceDisplayModel()
        //        {
        //            InvNumber = x.InvNumber,
        //            InvDate = x.InvDate.Value,
        //            DSStatus = x.DSStatus,
        //            PrintStatus = x.PrintStatus,
        //            ApiStatus = x.APIStatus,
        //            ApiMsg = x.APIMessage

        //        }).Where(x => x.InvNumber == InvoiceNo).ToList();
        //    }
        //    return lstInv.OrderByDescending(x => x.CreatedDate).ToList();
        //}

        //public List<InvoiceReportModel> FilterInvoiceData(string InvoiceNo, string FromDate, string ToDate)
        //{
        //    List<InvoiceReportModel> lstInv = new List<InvoiceReportModel>();

        //    using (var context = new DSMdbEntities())
        //    {
        //        List<SP_FILTER_INVOICE_Result> listObjects = (from obj in context.SP_FILTER_INVOICE(InvoiceNo, FromDate == null ? "" : FromDate, ToDate == null ? "" : ToDate)
        //                                                      select obj).GroupBy(n => new { n.InvNumber })
        //                                                   .Select(g => g.FirstOrDefault())
        //                                                   .ToList();

        //        lstInv = listObjects.Select(x => new InvoiceReportModel()
        //        {
        //            InvNumber = x.InvNumber,
        //            InvDate = x.InvDate.Value,
        //            ApiStatus = x.APIStatus,
        //            ApiMsg = x.APIMessage
        //        }).ToList();
        //    }
        //    return lstInv;
        //}
        public List<InvoiceReportModel> FilterInvoiceData(string InvoiceNo, string FromDate, string ToDate)
        {
            //string frmDate= Convert.ToDateTime(FromDate).ToString("yyyy-MM-dd 00:00:00:000", CultureInfo.InvariantCulture);
            //string toDate = Convert.ToDateTime(ToDate).ToString("yyyy-MM-dd 00:00:00:000", CultureInfo.InvariantCulture);




            List<InvoiceReportModel> lstInv = new List<InvoiceReportModel>();

            using (var context = new DSMdbEntities())
            {
                List<SP_FILTER_INVOICE_Result> listObjects = (from obj in context.SP_FILTER_INVOICE(InvoiceNo, FromDate == null ? "" : FromDate, ToDate == null ? "" : ToDate)
                                                              select obj).GroupBy(n => new { n.InvNumber })
                                                           .Select(g => g.FirstOrDefault())
                                                           .ToList();

                lstInv = listObjects.Select(x => new InvoiceReportModel()
                {
                    InvNumber = x.InvNumber,
                    InvDate = x.CreatedDate.Value,
                    ApiStatus = x.APIStatus,
                    ApiMsg = x.APIMessage
                }).ToList();
            }
            return lstInv;
        }

        //public void saveOtherInvoice(List<InvoiceDisplayModel> invDataList)
        //{
        //    using(var context = new DSMdbEntities())
        //    {
        //        var dd = invDataList.FirstOrDefault().InvNumber;
        //        var ss = context.Invoices.Where(x => x.InvNumber == dd).FirstOrDefault();
        //        //var ds = context.Invoices.Where(y => y.InvNumber == ss.InvNumber).ToList();
        //        List<Invoice> lstInvoice = new List<Invoice>();
        //        foreach (var invData in invDataList)
        //        {
        //            Invoice entInvoice = new Invoice();

        //            entInvoice.InvNumber = invData.InvNumber;
        //            entInvoice.InvDate = invData.InvDate;

        //            entInvoice.CustomerName = invData.CustomerName;
        //            entInvoice.VehicleNumber = invData.VehicleNumber;
        //            entInvoice.VendorCode = invData.VendorCode;
        //            //entInvoice.DSStatus = "FALSE";
        //            //entInvoice.PrintStatus = "FALSE";
        //            entInvoice.DSStatus = "TRUE";
        //            entInvoice.PrintStatus = "TRUE";
        //            entInvoice.APIStatus = "N/A";
        //            entInvoice.APIMessage = "N/A";
        //            //entInvoice.IsEmailSend = false;
        //            entInvoice.IsEmailSend = invData.isEmailSend;
        //            entInvoice.CreatedDate = DateTime.Now;
        //            entInvoice.OriginalPDF = invData.OriginalPDF;
        //            entInvoice.Finyear = invData.Finyear;
        //            entInvoice.SystemName = invData.SystemName;
        //            lstInvoice.Add(entInvoice);
        //        }
        //        context.Invoices.AddRange(lstInvoice);
        //        context.SaveChanges();
        //    }
        //}

        public void saveOtherInvoice(List<InvoiceDisplayModel> invDataList)
        {
            try
            {
                using (var context = new DSMdbEntities())
                {
                    foreach (var invData in invDataList)
                    {
                        // Try to find existing invoice
                        var existingInvoice = context.Invoices
                            .FirstOrDefault(x => x.InvNumber == invData.InvNumber);

                        if (existingInvoice != null)
                        {
                            // ✅ UPDATE existing record
                            existingInvoice.InvDate = invData.InvDate;
                            existingInvoice.CustomerName = invData.CustomerName;
                            existingInvoice.VehicleNumber = invData.VehicleNumber;
                            existingInvoice.VendorCode = invData.VendorCode;
                            existingInvoice.DSStatus = "TRUE";
                            existingInvoice.PrintStatus = "TRUE";
                            //existingInvoice.APIStatus = "N/A";
                            existingInvoice.APIMessage = "N/A";
                            existingInvoice.IsEmailSend = invData.isEmailSend;
                            existingInvoice.OriginalPDF = invData.OriginalPDF;
                            existingInvoice.Finyear = invData.Finyear;
                            existingInvoice.SystemName = invData.SystemName;
                            existingInvoice.UpdatedDate = DateTime.Now;
                        }
                        else
                        {
                            // ✅ INSERT new record
                            Invoice entInvoice = new Invoice
                            {
                                InvNumber = invData.InvNumber,
                                InvDate = invData.InvDate,
                                CustomerName = invData.CustomerName,
                                VehicleNumber = invData.VehicleNumber,
                                VendorCode = invData.VendorCode,
                                DSStatus = "TRUE",
                                PrintStatus = "TRUE",
                                //APIStatus = "N/A",
                                APIMessage = "N/A",
                                IsEmailSend = invData.isEmailSend,
                                CreatedDate = DateTime.Now,
                                OriginalPDF = invData.OriginalPDF,
                                Finyear = invData.Finyear,
                                SystemName = invData.SystemName
                            };

                            context.Invoices.Add(entInvoice);
                        }
                    }

                    // Save both insert/update
                    context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                LogErrors(ex);
            }
        }

        public void SaveInvoice(List<InvoiceDisplayModel> invDataList)
        {
            
            try
            {
                using (var context = new DSMdbEntities())
                {
                    var dd = invDataList.FirstOrDefault().InvNumber;
                    var ss = context.Invoices.Where(x => x.InvNumber == dd).FirstOrDefault();
                    if (ss != null)
                    {
                        if (ss.APIStatus.ToUpper().Trim() != "S")
                        {
                            if (ss.APIStatus.Trim() != "I/F Success, ASN Not created,Create Manual ASN")
                            {
                                var ds = context.Invoices.Where(y => y.InvNumber == ss.InvNumber).ToList();
                                if (ds.Count() > 0)
                                {
                                    context.Invoices.RemoveRange(ds);
                                    context.SaveChanges();
                                }
                                List<Invoice> lstInvoice = new List<Invoice>();
                                foreach (var invData in invDataList)
                                {
                                    Invoice entInvoice = new Invoice();
                                    entInvoice.ShopCode = invData.ShopCode;
                                    entInvoice.PONumber = invData.PONumber;
                                    entInvoice.PartNumber = invData.PartNumber;
                                    entInvoice.InvNumber = invData.InvNumber;
                                    entInvoice.InvDate = invData.InvDate;
                                    entInvoice.InvQuantity = invData.InvQuantity;
                                    entInvoice.InvValue = invData.InvValue;
                                    entInvoice.TarrifNumber = invData.TarrifNumber;
                                    entInvoice.BedAmount = invData.BedAmount;
                                    entInvoice.SGST = invData.SGST;
                                    entInvoice.IGST = invData.IGST;
                                    entInvoice.VatAmount = invData.VatAmount;
                                    entInvoice.UnitPrice = invData.UnitPrice;
                                    entInvoice.MaterialCost = invData.MaterialCost;
                                    entInvoice.CGST = invData.CGST;
                                    entInvoice.ConsigneePartCost = invData.ConsigneePartCost;
                                    entInvoice.ExciseDutyCost = invData.ExciseDutyCost;
                                    entInvoice.AssessableValue = invData.AssessableValue;
                                    entInvoice.CSTAmount = invData.CSTAmount;
                                    entInvoice.ToolCost = invData.ToolCost;
                                    entInvoice.ConsigneeMatlCost = invData.ConsigneeMatlCost;
                                    entInvoice.GSTN = invData.GSTN;
                                    entInvoice.CustomerName = invData.CustomerName;
                                    entInvoice.VehicleNumber = invData.VehicleNumber;
                                    entInvoice.VendorCode = invData.VendorCode;
                                    //entInvoice.DSStatus = "FALSE";
                                    //entInvoice.PrintStatus = "FALSE";
                                    entInvoice.DSStatus = "TRUE";
                                    entInvoice.PrintStatus = "TRUE";
                                    entInvoice.APIStatus = "FALSE";
                                    //entInvoice.IsEmailSend = false;
                                    entInvoice.IsEmailSend = invData.isEmailSend;
                                    entInvoice.CreatedDate = DateTime.Now;
                                    entInvoice.TCS = invData.TCS;
                                    entInvoice.Ewaybill_no = invData.Ewaybill_no;
                                    entInvoice.E_InvNo = invData.E_InvNo;
                                    entInvoice.Manfac_Date = invData.InvDate;
                                    entInvoice.Lot_Code = invData.Lot_Code;
                                    entInvoice.EXTRA_NUM_1 = invData.EXTRA_NUM_1;
                                    entInvoice.EXTRA_NUM_2 = invData.EXTRA_NUM_2;
                                    entInvoice.EXTRA_NUM_3 = invData.EXTRA_NUM_3;
                                    entInvoice.EXTRA_NUM_4 = invData.EXTRA_NUM_4;
                                    entInvoice.EXTRA_NUM_5 = invData.EXTRA_NUM_5;
                                    entInvoice.EXTRA_NUM_6 = invData.EXTRA_NUM_6;
                                    entInvoice.EXTRA_NUM_7 = invData.EXTRA_NUM_7;
                                    entInvoice.EXTRA_CHAR_1 = invData.EXTRA_CHAR_1;
                                    entInvoice.EXTRA_CHAR_2 = invData.EXTRA_CHAR_2;
                                    entInvoice.EXTRA_CHAR_3 = invData.EXTRA_CHAR_3;
                                    entInvoice.EXTRA_CHAR_4 = invData.EXTRA_CHAR_4;
                                    entInvoice.EXTRA_CHAR_5 = invData.EXTRA_CHAR_5;
                                    entInvoice.Date_1 = invData.InvDate;
                                    entInvoice.Date_2 = invData.InvDate;
                                    entInvoice.Date_3 = invData.InvDate;
                                    entInvoice.Date_4 = invData.InvDate;
                                    entInvoice.Date_5 = invData.InvDate;
                                    entInvoice.OriginalPDF = invData.OriginalPDF;
                                    entInvoice.PDF_Generation = invData.PdfGeneration;
                                    entInvoice.Finyear = invData.Finyear;
                                    entInvoice.SystemName = invData.SystemName;
                                    entInvoice.CreatedDate = DateTime.Now;
                                    entInvoice.UpdatedDate = DateTime.Now;
                                    lstInvoice.Add(entInvoice);
                                }
                                context.Invoices.AddRange(lstInvoice);
                                context.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        List<Invoice> lstInvoice = new List<Invoice>();
                        foreach (var invData in invDataList)
                        {
                            Invoice entInvoice = new Invoice();
                            entInvoice.ShopCode = invData.ShopCode;
                            entInvoice.PONumber = invData.PONumber;
                            entInvoice.PartNumber = invData.PartNumber;
                            entInvoice.InvNumber = invData.InvNumber;
                            entInvoice.InvDate = invData.InvDate;
                            entInvoice.InvQuantity = invData.InvQuantity;
                            entInvoice.InvValue = invData.InvValue;
                            entInvoice.TarrifNumber = invData.TarrifNumber;
                            entInvoice.BedAmount = invData.BedAmount;
                            entInvoice.SGST = invData.SGST;
                            entInvoice.IGST = invData.IGST;
                            entInvoice.VatAmount = invData.VatAmount;
                            entInvoice.UnitPrice = invData.UnitPrice;
                            entInvoice.MaterialCost = invData.MaterialCost;
                            entInvoice.CGST = invData.CGST;
                            entInvoice.ConsigneePartCost = invData.ConsigneePartCost;
                            entInvoice.ExciseDutyCost = invData.ExciseDutyCost;
                            entInvoice.AssessableValue = invData.AssessableValue;
                            entInvoice.CSTAmount = invData.CSTAmount;
                            entInvoice.ToolCost = invData.ToolCost;
                            entInvoice.ConsigneeMatlCost = invData.ConsigneeMatlCost;
                            entInvoice.GSTN = invData.GSTN;
                            entInvoice.CustomerName = invData.CustomerName;
                            entInvoice.VehicleNumber = invData.VehicleNumber;
                            entInvoice.VendorCode = invData.VendorCode;
                            //entInvoice.DSStatus = "FALSE";
                            //entInvoice.PrintStatus = "FALSE";
                            entInvoice.DSStatus = "TRUE";

                            entInvoice.PrintStatus = "TRUE";
                            entInvoice.APIStatus = "FALSE";
                            entInvoice.CreatedDate = DateTime.Now;
                            entInvoice.UpdatedDate = DateTime.Now;
                            //entInvoice.IsEmailSend = false;
                            entInvoice.IsEmailSend = invData.isEmailSend;

                            entInvoice.Finyear = invData.Finyear;
                            entInvoice.SystemName = invData.SystemName;
                            entInvoice.TCS = invData.TCS;
                            entInvoice.Ewaybill_no = invData.Ewaybill_no;
                            entInvoice.E_InvNo = invData.E_InvNo;
                            entInvoice.Manfac_Date = invData.InvDate;
                            entInvoice.Lot_Code = invData.Lot_Code;
                            entInvoice.EXTRA_NUM_1 = invData.EXTRA_NUM_1;
                            entInvoice.EXTRA_NUM_2 = invData.EXTRA_NUM_2;
                            entInvoice.EXTRA_NUM_3 = invData.EXTRA_NUM_3;
                            entInvoice.EXTRA_NUM_4 = invData.EXTRA_NUM_4;
                            entInvoice.EXTRA_NUM_5 = invData.EXTRA_NUM_5;
                            entInvoice.EXTRA_NUM_6 = invData.EXTRA_NUM_6;
                            entInvoice.EXTRA_NUM_7 = invData.EXTRA_NUM_7;
                            entInvoice.EXTRA_CHAR_1 = invData.EXTRA_CHAR_1;
                            entInvoice.EXTRA_CHAR_2 = invData.EXTRA_CHAR_2;
                            entInvoice.EXTRA_CHAR_3 = invData.EXTRA_CHAR_3;
                            entInvoice.EXTRA_CHAR_4 = invData.EXTRA_CHAR_4;
                            entInvoice.EXTRA_CHAR_5 = invData.EXTRA_CHAR_5;
                            entInvoice.Date_1 = invData.InvDate;
                            entInvoice.Date_2 = invData.InvDate;
                            entInvoice.Date_3 = invData.InvDate;
                            entInvoice.Date_4 = invData.InvDate;
                            entInvoice.Date_5 = invData.InvDate;
                            entInvoice.OriginalPDF = invData.OriginalPDF;
                            entInvoice.PDF_Generation = invData.PdfGeneration;
                            lstInvoice.Add(entInvoice);
                        }
                        context.Invoices.AddRange(lstInvoice);
                        context.SaveChanges();
                    }
                }
            }
            catch (EntityException ex)
            {

            }
        }
        public void SaveQuarantineInvoice(List<InvoiceDisplayModel> invDataList)
        {

            try
            {
                using (var context = new DSMdbEntities())
                {
                
                        List<Invoice> lstInvoice = new List<Invoice>();
                        foreach (var invData in invDataList)
                        {
                            Invoice entInvoice = new Invoice();
                            entInvoice.ShopCode = invData.ShopCode;
                            entInvoice.PONumber = invData.PONumber;
                            entInvoice.PartNumber = invData.PartNumber;
                            entInvoice.InvNumber = invData.InvNumber;
                            entInvoice.InvDate = invData.InvDate;
                            entInvoice.InvQuantity = invData.InvQuantity;
                            entInvoice.InvValue = invData.InvValue;
                            entInvoice.TarrifNumber = invData.TarrifNumber;
                            entInvoice.BedAmount = invData.BedAmount;
                            entInvoice.SGST = invData.SGST;
                            entInvoice.IGST = invData.IGST;
                            entInvoice.VatAmount = invData.VatAmount;
                            entInvoice.UnitPrice = invData.UnitPrice;
                            entInvoice.MaterialCost = invData.MaterialCost;
                            entInvoice.CGST = invData.CGST;
                            entInvoice.ConsigneePartCost = invData.ConsigneePartCost;
                            entInvoice.ExciseDutyCost = invData.ExciseDutyCost;
                            entInvoice.AssessableValue = invData.AssessableValue;
                            entInvoice.CSTAmount = invData.CSTAmount;
                            entInvoice.ToolCost = invData.ToolCost;
                            entInvoice.ConsigneeMatlCost = invData.ConsigneeMatlCost;
                            entInvoice.GSTN = invData.GSTN;
                            entInvoice.CustomerName = invData.CustomerName;
                            entInvoice.VehicleNumber = invData.VehicleNumber;
                            entInvoice.VendorCode = invData.VendorCode;
                            //entInvoice.DSStatus = "FALSE";
                            //entInvoice.PrintStatus = "FALSE";
                            entInvoice.DSStatus = "FALSE";

                            entInvoice.PrintStatus = "FALSE";
                            entInvoice.APIStatus = "FALSE";
                            entInvoice.CreatedDate = DateTime.Now;
                            entInvoice.UpdatedDate = DateTime.Now;
                            //entInvoice.IsEmailSend = false;
                            entInvoice.IsEmailSend = invData.isEmailSend;

                            entInvoice.Finyear = invData.Finyear;
                            entInvoice.SystemName = invData.SystemName;
                            entInvoice.TCS = invData.TCS;
                            entInvoice.Ewaybill_no = invData.Ewaybill_no;
                            entInvoice.E_InvNo = invData.E_InvNo;
                            entInvoice.Manfac_Date = invData.InvDate;
                            entInvoice.Lot_Code = invData.Lot_Code;
                            entInvoice.EXTRA_NUM_1 = invData.EXTRA_NUM_1;
                            entInvoice.EXTRA_NUM_2 = invData.EXTRA_NUM_2;
                            entInvoice.EXTRA_NUM_3 = invData.EXTRA_NUM_3;
                            entInvoice.EXTRA_NUM_4 = invData.EXTRA_NUM_4;
                            entInvoice.EXTRA_NUM_5 = invData.EXTRA_NUM_5;
                            entInvoice.EXTRA_NUM_6 = invData.EXTRA_NUM_6;
                            entInvoice.EXTRA_NUM_7 = invData.EXTRA_NUM_7;
                            entInvoice.EXTRA_CHAR_1 = invData.EXTRA_CHAR_1;
                            entInvoice.EXTRA_CHAR_2 = invData.EXTRA_CHAR_2;
                            entInvoice.EXTRA_CHAR_3 = invData.EXTRA_CHAR_3;
                            entInvoice.EXTRA_CHAR_4 = invData.EXTRA_CHAR_4;
                            entInvoice.EXTRA_CHAR_5 = invData.EXTRA_CHAR_5;
                            entInvoice.Date_1 = invData.InvDate;
                            entInvoice.Date_2 = invData.InvDate;
                            entInvoice.Date_3 = invData.InvDate;
                            entInvoice.Date_4 = invData.InvDate;
                            entInvoice.Date_5 = invData.InvDate;
                            entInvoice.OriginalPDF = invData.OriginalPDF;
                            entInvoice.PDF_Generation = invData.PdfGeneration;
                            lstInvoice.Add(entInvoice);
                        }
                        context.Invoices.AddRange(lstInvoice);
                        context.SaveChanges();
                    }
                
            }
            catch (EntityException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SaveDigitalSignature(DSMasterDisplayModel objDSdata)
        {
            try
            {
                using (var context = new DSMdbEntities())
                {
                    var entDS = context.DSMasters.Where(x => x.DSMasterId == objDSdata.DSMasterId).FirstOrDefault();
                    if (entDS != null)
                    {
                        entDS.KeyPinNo = objDSdata.KeyPinNo;
                        entDS.Department = objDSdata.Department;
                        entDS.Designation = objDSdata.Designation;
                        entDS.SignName = objDSdata.SignName;
                        entDS.UpdatedDate = DateTime.Now;
                        context.SaveChanges();
                    }
                    else
                    {
                        DSMaster entDsMaster = new DSMaster();

                        entDsMaster.KeyName = objDSdata.KeyName;
                        entDsMaster.KeyPinNo = objDSdata.KeyPinNo;
                        entDsMaster.Department = objDSdata.Department;
                        entDsMaster.Designation = objDSdata.Designation;
                        entDsMaster.SignName = objDSdata.SignName;
                        entDsMaster.IssuerName = objDSdata.IssuerName;
                        entDsMaster.SerialNo = objDSdata.SerialNo;
                        entDsMaster.ThumbPrint = objDSdata.ThumbPrint;
                        entDsMaster.CertificateName = objDSdata.CertificateName;
                        entDsMaster.ContainerName = objDSdata.ContainerName;
                        entDsMaster.ProviderName = objDSdata.ProviderName;
                        entDsMaster.ValidityBefore = objDSdata.ValidityBefore;
                        entDsMaster.ValidityAfter = objDSdata.ValidityAfter;
                        entDsMaster.IsActive = objDSdata.IsActive;
                        entDsMaster.CreatedDate = DateTime.Now;
                        context.DSMasters.Add(entDsMaster);
                        context.SaveChanges();
                    }
                }
            }
            catch (EntityException ex)
            {

            }
        }

        public bool ValidateDSKey(string Cerificate, string SerialNo)
        {
            bool val = false;
            using (var context = new DSMdbEntities())
            {
                var lstDS = context.DSMasters.Where(x => x.CertificateName.Trim() == Cerificate.Trim() && x.SerialNo.Trim() == SerialNo.Trim() && x.IsActive == true).ToList();
                if (lstDS.Count > 0)
                {
                    val = false;
                }
                else
                {
                    val = true;
                }
            }
            return val;
        }

        public void DeleteDSKey(int DSMasterId)
        {
            using (var context = new DSMdbEntities())
            {
                var ds = context.DSMasters.Where(x => x.DSMasterId == DSMasterId).FirstOrDefault();
                if (ds != null)
                {
                    ds.IsActive = false;
                    context.SaveChanges();
                }

                var dsdet = context.DSMasters.Where(x => x.DSMasterId == DSMasterId).FirstOrDefault();
                if (dsdet != null)
                {
                    dsdet.IsActive = false;
                    context.SaveChanges();
                }
            }
        }

        public void SaveCustomer(CustomerDisplayModel customer, List<DSMasterDisplayModel> lstDS)
        {
            try
            {
                using (var context = new DSMdbEntities())
                {
                    int ClientLineId;
                    var client = context.ClientMasters.Where(x => x.ClientId == customer.CustomerId && x.IsActive == true).FirstOrDefault();
                    if (client != null)
                    {
                        client.ClientName = customer.CustomerName;
                        client.VendorCode = customer.VendorCode;
                        client.IsActive = true;
                        client.IsDigitalSign = customer.IsDigitalSign;
                        client.IsAPIpost = customer.IsAPIpost;
                        client.APIUrl = customer.APIUrl;
                        client.APItype = customer.APItype;
                        client.PDFtype = customer.PDFtype;
                        client.PrintType = customer.PrintType;
                        client.PrintCopies = customer.PrintCopies;
                        client.CreatedDate = DateTime.Now;
                        client.TemplateName = customer.Tempname;
                        context.SaveChanges();
                        ClientLineId = client.ClientLineId;
                    }
                    else
                    {
                        ClientMaster entClientMaster = new ClientMaster();
                        entClientMaster.ClientId = customer.CustomerId;
                        entClientMaster.ClientName = customer.CustomerName;
                        entClientMaster.VendorCode = customer.VendorCode;
                        entClientMaster.TemplateName = customer.Tempname;
                        entClientMaster.IsActive = true;
                        entClientMaster.IsDigitalSign = customer.IsDigitalSign;
                        entClientMaster.IsAPIpost = customer.IsAPIpost;
                        entClientMaster.APIUrl = customer.APIUrl;
                        entClientMaster.APItype = customer.APItype;
                        entClientMaster.PDFtype = customer.PDFtype;
                        entClientMaster.PrintType = customer.PrintType;
                        entClientMaster.PrintCopies = customer.PrintCopies;
                        entClientMaster.CreatedDate = DateTime.Now;
                        context.ClientMasters.Add(entClientMaster);
                        context.SaveChanges();
                        ClientLineId = entClientMaster.ClientLineId;
                    }

                    if (customer.IsDigitalSign)
                    {
                        List<DigitalSignDetail> entlistDS = new List<DigitalSignDetail>();
                        foreach (var objDS in lstDS)
                        {
                            var checkExist = context.DigitalSignDetails.Where(x => x.KeyName == objDS.KeyName && x.ClientId == customer.CustomerId && x.IsActive == true).FirstOrDefault();
                            if (checkExist == null)
                            {
                                DigitalSignDetail entDSDet = new DigitalSignDetail();
                                entDSDet.ClientLineId = ClientLineId;
                                entDSDet.ClientId = customer.CustomerId;
                                entDSDet.KeyName = objDS.KeyName;
                                entDSDet.KeyPinNo = objDS.KeyPinNo;
                                entDSDet.CreatedDate = DateTime.Now;
                                entDSDet.IsActive = true;
                                entlistDS.Add(entDSDet);
                            }
                        }
                        context.DigitalSignDetails.AddRange(entlistDS);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        //public void SaveCustomer(CustomerDisplayModel customer, List<DigitalSignDetailDisplayModel> lstDS)
        //{
        //    try
        //    {
        //        using (var context = new DSMdbEntities())
        //        {
        //            int ClientLineId;
        //            var client = context.ClientMasters.Where(x => x.ClientId == customer.CustomerId && x.IsActive == true).FirstOrDefault();
        //            if (client != null)
        //            {
        //                client.ClientName = customer.CustomerName;
        //                client.VendorCode = customer.VendorCode;
        //                client.IsActive = true;
        //                client.IsDigitalSign = customer.IsDigitalSign;
        //                client.IsAPIpost = customer.IsAPIpost;
        //                client.APIUrl = customer.APIUrl;
        //                client.APItype = customer.APItype;
        //                client.PDFtype = customer.PDFtype;
        //                client.PrintType = customer.PrintType;
        //                client.PrintCopies = customer.PrintCopies;
        //                client.CreatedDate = DateTime.Now;
        //                context.SaveChanges();
        //                ClientLineId = client.ClientLineId;
        //            }
        //            else
        //            {
        //                ClientMaster entClientMaster = new ClientMaster();
        //                entClientMaster.ClientId = customer.CustomerId;
        //                entClientMaster.ClientName = customer.CustomerName;
        //                entClientMaster.VendorCode = customer.VendorCode;
        //                entClientMaster.IsActive = true;
        //                entClientMaster.IsDigitalSign = customer.IsDigitalSign;
        //                entClientMaster.IsAPIpost = customer.IsAPIpost;
        //                entClientMaster.APIUrl = customer.APIUrl;
        //                entClientMaster.APItype = customer.APItype;
        //                entClientMaster.PDFtype = customer.PDFtype;
        //                entClientMaster.PrintType = customer.PrintType;
        //                entClientMaster.PrintCopies = customer.PrintCopies;
        //                entClientMaster.CreatedDate = DateTime.Now;
        //                context.ClientMasters.Add(entClientMaster);
        //                context.SaveChanges();
        //                ClientLineId = entClientMaster.ClientLineId;
        //            }

        //            if (customer.IsDigitalSign)
        //            {
        //                List<DigitalSignDetail> entlistDS = new List<DigitalSignDetail>();
        //                foreach (var objDS in lstDS)
        //                {
        //                    var checkExist = context.DigitalSignDetails.Where(x => x.KeyName == objDS.KeyName && x.ClientId == customer.CustomerId && x.IsActive == true).FirstOrDefault();
        //                    if (checkExist == null)
        //                    {
        //                        DigitalSignDetail entDSDet = new DigitalSignDetail();
        //                        entDSDet.ClientLineId = ClientLineId;
        //                        entDSDet.ClientId = customer.CustomerId;
        //                        entDSDet.KeyName = objDS.KeyName;
        //                        entDSDet.KeyPinNo = objDS.KeyPinNo;
        //                        entDSDet.CreatedDate = DateTime.Now;
        //                        entDSDet.IsActive = true;
        //                        entlistDS.Add(entDSDet);
        //                    }
        //                }
        //                context.DigitalSignDetails.AddRange(entlistDS);
        //                context.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //public void AddDigitalCustomer(List<DigitalSignDetailDisplayModel> lstDS)
        //{
        //    try
        //    {
        //        using (var context = new DSMdbEntities())
        //        {
        //            int ClientLineId;
        //            var client = context.ClientMasters.Where(x => x.ClientId == customer.CustomerId && x.IsActive == true).FirstOrDefault();
        //            if (client != null)
        //            {
        //                client.ClientName = customer.CustomerName;
        //                client.VendorCode = customer.VendorCode;
        //                client.IsActive = true;
        //                client.IsDigitalSign = customer.IsDigitalSign;
        //                client.IsAPIpost = customer.IsAPIpost;
        //                client.APIUrl = customer.APIUrl;
        //                client.APItype = customer.APItype;
        //                client.PDFtype = customer.PDFtype;
        //                client.PrintType = customer.PrintType;
        //                client.PrintCopies = customer.PrintCopies;
        //                client.CreatedDate = DateTime.Now;
        //                context.SaveChanges();
        //                ClientLineId = client.ClientLineId;
        //            }
        //            else
        //            {
        //                ClientMaster entClientMaster = new ClientMaster();
        //                entClientMaster.ClientId = customer.CustomerId;
        //                entClientMaster.ClientName = customer.CustomerName;
        //                entClientMaster.VendorCode = customer.VendorCode;
        //                entClientMaster.IsActive = true;
        //                entClientMaster.IsDigitalSign = customer.IsDigitalSign;
        //                entClientMaster.IsAPIpost = customer.IsAPIpost;
        //                entClientMaster.APIUrl = customer.APIUrl;
        //                entClientMaster.APItype = customer.APItype;
        //                entClientMaster.PDFtype = customer.PDFtype;
        //                entClientMaster.PrintType = customer.PrintType;
        //                entClientMaster.PrintCopies = customer.PrintCopies;
        //                entClientMaster.CreatedDate = DateTime.Now;
        //                context.ClientMasters.Add(entClientMaster);
        //                context.SaveChanges();
        //                ClientLineId = entClientMaster.ClientLineId;
        //            }

        //            if (customer.IsDigitalSign)
        //            {
        //                List<DigitalSignDetail> entlistDS = new List<DigitalSignDetail>();
        //                foreach (var objDS in lstDS)
        //                {
        //                    var checkExist = context.DigitalSignDetails.Where(x => x.KeyName == objDS.KeyName && x.ClientId == customer.CustomerId && x.IsActive == true).FirstOrDefault();
        //                    if (checkExist == null)
        //                    {
        //                        DigitalSignDetail entDSDet = new DigitalSignDetail();
        //                        entDSDet.ClientLineId = ClientLineId;
        //                        entDSDet.ClientId = customer.CustomerId;
        //                        entDSDet.KeyName = objDS.KeyName;
        //                        entDSDet.KeyPinNo = objDS.KeyPinNo;
        //                        entDSDet.CreatedDate = DateTime.Now;
        //                        entDSDet.IsActive = true;
        //                        entlistDS.Add(entDSDet);
        //                    }
        //                }
        //                context.DigitalSignDetails.AddRange(entlistDS);
        //                context.SaveChanges();
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        public CustomerDisplayModel GetCustomerByInvNumber(string Invoice)
        {
            CustomerDisplayModel objCustomer = new CustomerDisplayModel();
            using (var context = new DSMdbEntities())
            {

                objCustomer = context.Invoices.Where(s => s.InvNumber == Invoice).Select(x => new CustomerDisplayModel()
                {
                     
                    CustomerName = x.CustomerName,
                     
                }).FirstOrDefault();
            }
            return objCustomer;
        }

        public CustomerDisplayModel GetCustomerByName(string CustomerName)
        {
            CustomerDisplayModel objCustomer = new CustomerDisplayModel();
            using (var context = new DSMdbEntities())
            {

                objCustomer = context.ClientMasters.Where(s => s.IsActive == true && s.ClientName==CustomerName).Select(x => new CustomerDisplayModel()
                {
                    CustomerId = x.ClientId,
                    CustomerName = x.ClientName,
                    VendorCode = x.VendorCode,
                    ClientLineId = x.ClientLineId,
                    IsActive = x.IsActive == null ? false : x.IsActive.Value,
                    IsDigitalSign = x.IsDigitalSign == null ? false : x.IsDigitalSign.Value,
                    IsAPIpost = x.IsAPIpost == null ? false : x.IsAPIpost.Value,
                    APItype = x.APItype == null ? false : x.APItype.Value,
                    PDFtype = x.PDFtype == null ? false : x.PDFtype.Value,
                    PrintType = x.PrintType == null ? false : x.PrintType.Value,
                    PrintCopies = x.PrintCopies,
                    APIUrl = x.APIUrl
                }).FirstOrDefault();
            }
            return objCustomer;
        }

        public List<CustomerDisplayModel> GetAllCustomers()
        {
            List<CustomerDisplayModel> lstCustomer = new List<CustomerDisplayModel>();
            using (var context = new DSMdbEntities())
            {

                lstCustomer = context.ClientMasters.Where(s => s.IsActive == true).Select(x => new CustomerDisplayModel()
                {
                    CustomerId = x.ClientId,
                    CustomerName = x.ClientName,
                    VendorCode = x.VendorCode,
                    ClientLineId = x.ClientLineId,
                    IsActive = x.IsActive == null ? false : x.IsActive.Value,
                    IsDigitalSign = x.IsDigitalSign == null ? false : x.IsActive.Value,
                    IsTemplateCheck = x.IsActive == null ? false : x.IsDigitalSign.Value,
                    IsAPIpost = x.IsAPIpost == null ? false : x.IsAPIpost.Value,
                    APItype = x.APItype == null ? false : x.APItype.Value,
                    PDFtype = x.PDFtype == null ? false : x.PDFtype.Value,
                    PrintType = x.PrintType == null ? false : x.PrintType.Value,
                    PrintCopies = x.PrintCopies,
                    APIUrl = x.APIUrl,
                    Tempname=x.TemplateName
                    
                }).ToList();
            }
            return lstCustomer;
        }

        public void DeleteCustomer(string ClientId)
        {
            using (var context = new DSMdbEntities())
            {
                //var customer = context.ClientMasters.Where(x => x.ClientId == ClientId).FirstOrDefault();
                var customer = context.ClientMasters.Where(x => x.ClientId == ClientId).FirstOrDefault();
                if (customer != null)
                {
                    customer.IsActive = false;
                    context.SaveChanges();
                }

                var dsdetail = context.DigitalSignDetails.Where(x => x.ClientLineId == customer.ClientLineId).ToList();
                if (dsdetail.Count > 0)
                {
                    foreach (var ds in dsdetail)
                    {
                        ds.IsActive = false;
                    }
                    context.SaveChanges();
                }
            }
        }

        //public void DeleteCustomerDSDet(string ClientId,int DSDetId)
        public void DeleteCustomerDSDet(string ClientId, string KeyName)
        {
            try
            {

                using (var context = new DSMdbEntities())
                {
                    var customer = context.ClientMasters.Where(x => x.ClientId == ClientId).FirstOrDefault();
                    //if (customer != null)
                    //{
                    //    customer.IsActive = false;
                    //    context.SaveChanges();
                    //}

                    var dsdetail = context.DigitalSignDetails.Where(x => x.ClientLineId == customer.ClientLineId && x.KeyName == KeyName).ToList();
                    if (dsdetail.Count > 0)
                    {
                        foreach (var ds in dsdetail)
                        {
                            ds.IsActive = false;
                        }
                        context.SaveChanges();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void SaveEDI(EdiDisplayModel objEdi)
        {
            using (var context = new DSMdbEntities())
            {
                var entEdi = context.EDIConfigurations.Where(x => x.EdiId == objEdi.EdiId).FirstOrDefault();
                if (entEdi != null)
                {
                    //entEdi.ClientLineId = objEdi.ClientLineId;
                    //entEdi.ClientId = objEdi.CustomerId;
                    entEdi.IsEdi = objEdi.IsEdi;
                    entEdi.IsEmail = objEdi.IsEmail;
                    entEdi.FromEmail = objEdi.FromEmail;
                    entEdi.Host = objEdi.Host;
                    entEdi.Port = objEdi.Port;
                    entEdi.Email = objEdi.Email;
                    entEdi.FromPwd = objEdi.FromPwd;
                    entEdi.EdiLink = objEdi.EdiLink;
                    entEdi.UpdatedDate = DateTime.Now;
                    context.SaveChanges();
                }
                else
                {
                    EDIConfiguration entEdiConfig = new EDIConfiguration();
                    entEdiConfig.ClientLineId = objEdi.ClientLineId;
                    entEdiConfig.ClientId = objEdi.CustomerId;
                    entEdiConfig.IsEdi = objEdi.IsEdi;
                    entEdiConfig.IsEmail = objEdi.IsEmail;
                    entEdiConfig.FromEmail = objEdi.FromEmail;
                    entEdiConfig.Host = objEdi.Host;
                    entEdiConfig.Port = objEdi.Port;
                    entEdiConfig.Email = objEdi.Email;
                    entEdiConfig.EdiLink = objEdi.EdiLink;
                    entEdiConfig.FromPwd = objEdi.FromPwd;
                    entEdiConfig.IsActive = true;
                    entEdiConfig.CreatedDate = DateTime.Now;
                    context.EDIConfigurations.Add(entEdiConfig);
                    context.SaveChanges();
                }
            }
        }

        public List<EdiDisplayModel> GetAllEdi()
        {
            List<EdiDisplayModel> lstEDI = new List<EdiDisplayModel>();
            List<CustomerDisplayModel> listCus = new List<CustomerDisplayModel>();

            using (var context = new DSMdbEntities())
            {
                //var query = context.EDIConfigurations.Join(context.ClientMasters, r => r.ClientId, p => p.ClientId, (r, p) => new { p.ClientName });
                //if (query != null)
                //{
                    //query.

                    //            var UserInRole = db.UserProfiles.
                    //Join(db.UsersInRoles, u => u.UserId, uir => uir.UserId,
                    //(u, uir) => new { u, uir }).
                    //Join(db.Roles, r => r.uir.RoleId, ro => ro.RoleId, (r, ro) => new { r, ro })
                    //.Select(m => new AddUserToRole
                    //{
                    //    UserName = m.r.u.UserName,
                    //    RoleName = m.ro.RoleName
                    //});

                    lstEDI = context.EDIConfigurations.Join(context.ClientMasters, e => e.ClientId, c => c.ClientId, (e, c) => new { e, c }).Where(x => x.e.IsActive == true).Select(x => new EdiDisplayModel()
                    {
                        EdiId = x.e.EdiId,
                        ClientLineId = x.e.ClientLineId == null ? 0 : x.e.ClientLineId.Value,
                        CustomerId = x.e.ClientId,
                        IsEdi = x.e.IsEdi == null ? true : x.e.IsEdi.Value,
                        IsEmail = x.e.IsEmail == null ? false : x.e.IsEmail.Value,
                        Email = x.e.Email,
                        FromEmail = x.e.FromEmail,
                        Host = x.e.Host,
                        Port = x.e.Port,
                        EdiLink = x.e.EdiLink,
                        FromPwd = x.e.FromPwd,
                        IsActive = x.e.IsActive == null ? false : x.e.IsActive.Value,
                        CustomerName = x.c.ClientName
                    }).ToList();
                    //lstEDI = context.EDIConfigurations.Where(x => x.IsActive == true).Select(x => new EdiDisplayModel()
                    //{
                    //    EdiId = x.EdiId,
                    //    ClientLineId = x.ClientLineId == null ? 0 : x.ClientLineId.Value,
                    //    CustomerId = x.ClientId,
                    //    IsEdi = x.IsEdi == null ? true : x.IsEdi.Value,
                    //    IsEmail = x.IsEmail == null ? false : x.IsEmail.Value,
                    //    Email = x.Email,
                    //    FromEmail = x.FromEmail,
                    //    Host = x.Host,
                    //    Port = x.Port,
                    //    EdiLink = x.EdiLink,
                    //    FromPwd = x.FromPwd,
                    //    IsActive = x.IsActive == null ? false : x.IsActive.Value
                    //   // CustomerName = p.
                    //}).ToList();
                //}

                //var client = dsmcontext.ClientMasters.Where(x => x.ClientLineId == ClientLineId && x.IsActive == true).FirstOrDefault();
            }
            return lstEDI;
        }

        public EdiDisplayModel GetEdiByClientName(string ClientName)
        {
            EdiDisplayModel lstEDI = new EdiDisplayModel();

            using (var context = new DSMdbEntities())
            {

                var client = context.ClientMasters.Where(x => x.IsActive == true && x.ClientName == ClientName).FirstOrDefault();
                var cilentId = client == null ? "" : client.ClientId;
                var lst = context.EDIConfigurations.Where(x => x.IsActive == true && x.ClientId == cilentId).FirstOrDefault();
                if (lst != null)
                {
                    lstEDI = new EdiDisplayModel();
                    lstEDI.EdiId = lst.EdiId;
                    lstEDI.ClientLineId = lst.ClientLineId == null ? 0 : lst.ClientLineId.Value;
                    lstEDI.CustomerId = lst.ClientId;
                    lstEDI.IsEdi = lst.IsEdi == null ? true : lst.IsEdi.Value;
                    lstEDI.IsEmail = lst.IsEmail == null ? false : lst.IsEmail.Value;
                    lstEDI.Email = lst.Email;
                    lstEDI.FromEmail = lst.FromEmail;
                    lstEDI.Host = lst.Host;
                    lstEDI.Port = lst.Port;
                    lstEDI.EdiLink = lst.EdiLink;
                    lstEDI.FromPwd = lst.FromPwd;
                    lstEDI.IsActive = lst.IsActive == null ? false : lst.IsActive.Value;
                }
            }
            return lstEDI;
        }

        public void DeleteEDI(int EdiId)
        {
            using (var context = new DSMdbEntities())
            {
                var edi = context.EDIConfigurations.Where(x => x.EdiId == EdiId).FirstOrDefault();
                if (edi != null)
                {
                    edi.IsActive = false;
                    context.SaveChanges();
                }
            }
        }

        public void SaveQuarantine(string InvNumber)
        {
            using (var context = new DSMdbEntities())
            {
                var entQ = context.Quarantines.Where(x => x.InvNumber == InvNumber).FirstOrDefault();
                if (entQ != null)
                {
                    entQ.IsActive = true;
                    context.SaveChanges();
                }
                else
                {
                    Quarantine entQr = new Quarantine();
                    entQr.InvNumber = InvNumber;
                    entQr.IsActive = true;
                    context.Quarantines.Add(entQr);
                    context.SaveChanges();
                }
            }
        }

        public List<QuarantineDisplayModel> GetQuarantineList()
        {
            List<QuarantineDisplayModel> lstQuarantine = new List<QuarantineDisplayModel>();
            using (var context = new DSMdbEntities())
            {
                lstQuarantine = context.Quarantines.Where(x => x.IsActive == true).Select(y => new QuarantineDisplayModel()
                {
                    QId = y.QId,
                    InvNumber = y.InvNumber,
                    IsActive = y.IsActive == null ? false : y.IsActive.Value
                }).ToList();
            }
            return lstQuarantine;
        }



        public void UpdateOldInvoice(string InvNumber)
        {
            using (var context = new DSMdbEntities())
            {
                var lstinvoice = context.Invoices.Where(x => x.InvNumber == InvNumber).ToList();
                if (lstinvoice.Count > 0)
                {
                    foreach (var inv in lstinvoice)
                    {
                        inv.PDF_Generation = "Processed";
                        inv.UpdatedDate = DateTime.Now;
                    }
                    context.SaveChanges();
                }
            }

        }

        public void DeleteQuarantine(string InvNumber)
        {
            using (var context = new DSMdbEntities())
            {
                var qT = context.Quarantines.Where(x => x.InvNumber == InvNumber).FirstOrDefault();
                if (qT != null)
                {
                    qT.IsActive = false;
                    context.SaveChanges();
                }
            }
        }
        public void UpdateClientCoordinates(string Coordinates, string templatename, int counter)
        {
            using (var context = new DSMdbEntities())
            {
                var lstcustomer = context.TemplateMasters.Where(x => x.Template_Name == templatename).ToList();
                if (lstcustomer.Count > 0)
                {
                    if (counter == 1)
                    {
                        foreach (var cust in lstcustomer)
                        {
                            cust.Inv_No_Cord = Coordinates;
                        }
                        context.SaveChanges();
                    }

                    else if (counter == 2)
                    {
                        foreach (var cust in lstcustomer)
                        {
                            cust.Inv_Date_Cord = Coordinates;

                        }
                        context.SaveChanges();
                    }
                    else if (counter == 3)
                    {
                        foreach (var cust in lstcustomer)
                        {
                            cust.Vendor_Cord = Coordinates;

                        }
                        context.SaveChanges();
                    }
                    else if (counter == 4)
                    {
                        foreach (var cust in lstcustomer)
                        {
                            cust.Truck_No_Cord = Coordinates;

                        }
                        context.SaveChanges();
                    }
                    else if (counter == 5)
                    {
                        foreach (var cust in lstcustomer)
                        {
                            cust.Client_Name_Cord = Coordinates;

                        }
                        context.SaveChanges();
                    }
                    else if (counter == 6)
                    {
                        foreach (var cust in lstcustomer)
                        {
                            cust.DS_Cord = Coordinates;

                        }
                        context.SaveChanges();
                    }
                    else if (counter == 7)
                    {
                        foreach (var cust in lstcustomer)
                        {
                            cust.Shopcd_Cord = Coordinates;

                        }
                        context.SaveChanges();
                    }

                }
            }
        }

        public void SaveTemplateName(string Template)
        {
            
            using (var context = new DSMdbEntities())
            {
                var user = context.TemplateMasters.Where(x => x.Template_Name.ToLower() == Template.ToString().ToLower()).FirstOrDefault();
                if (user == null)
                {
                    TemplateMaster cust = new TemplateMaster();
                    cust.Template_Name = Template.ToString();
                    context.TemplateMasters.Add(cust);
                    context.SaveChanges();
                  
                }
            }
          
        }
        public List<CustomerDisplayModel> GetInputTemplates()
        {
            List<CustomerDisplayModel> lstTemplate = new List<CustomerDisplayModel>();
            using (var context = new DSMdbEntities())
            {
                //.Where(x => !string.IsNullOrEmpty(x.Inv_No_Cord))
                //lstTemplate = context.TemplateMasters.Where(x => !string.IsNullOrEmpty(x.Inv_No_Cord) && !string.IsNullOrEmpty(x.Inv_Date_Cord) && !string.IsNullOrEmpty(x.Vendor_Cord) && !string.IsNullOrEmpty(x.Truck_No_Cord) && !string.IsNullOrEmpty(x.Client_Name_Cord) && !string.IsNullOrEmpty(x.DS_Cord)).Select(x => new CustomerDisplayModel()
                lstTemplate = context.TemplateMasters.Select(x => new CustomerDisplayModel()
                {


                    Tempname = x.Template_Name,
                    //Invnocoord = x.Inv_No_Cord,
                    //Invdatecoord = x.Inv_Date_Cord,
                    //Vendorcoord = x.Vendor_Cord,
                    //Trucknocoord = x.Truck_No_Cord,
                    //Clientnamecoord = x.Client_Name_Cord,
                    //Dscoord = x.DS_Cord

                }).ToList();
            }
            return lstTemplate;
        }
        public List<CustomerDisplayModel> GetTemplatesName(string TempName)
        {
            List<CustomerDisplayModel> lstTemplate = new List<CustomerDisplayModel>();
            using (var context = new DSMdbEntities())
            {
                //.Where(x => !string.IsNullOrEmpty(x.Inv_No_Cord))
                lstTemplate = context.TemplateMasters.Select(x => new CustomerDisplayModel()
                {
                  Tempname = x.Template_Name,
                }).ToList();
            }
            return lstTemplate;
        }
        public List<CustomerDisplayModel> GetTemplatesCord(string templatename,string CustId)
        {
            List<CustomerDisplayModel> lstTemplate = new List<CustomerDisplayModel>();
            using (var context = new DSMdbEntities())
            {
                //.Where(x => !string.IsNullOrEmpty(x.Inv_No_Cord))
                //lstTemplate = context.TemplateMasters.Select(x => new CustomerDisplayModel()
                 lstTemplate = context.TemplateMasters.Join(context.ClientMasters, t => t.Template_Name, c => c.TemplateName, 
                     (t, c) => new { t, c }).Where(x => !string.IsNullOrEmpty(x.t.Inv_No_Cord) && !string.IsNullOrEmpty(x.t.Inv_Date_Cord) && 
                     !string.IsNullOrEmpty(x.t.Vendor_Cord) && !string.IsNullOrEmpty(x.t.Truck_No_Cord) && !string.IsNullOrEmpty(x.t.Client_Name_Cord) && 
                     !string.IsNullOrEmpty(x.t.DS_Cord) == true && x.c.TemplateName == templatename && x.c.ClientId==CustId ).Select(x => new CustomerDisplayModel()
                 {


                            //Tempname = x.Template_Name,
                            Invnocoord = x.t.Inv_No_Cord,
                            Invdatecoord = x.t.Inv_Date_Cord,
                            Vendorcoord = x.t.Vendor_Cord,
                            Trucknocoord = x.t.Truck_No_Cord,
                            Clientnamecoord = x.t.Client_Name_Cord,
                            Shopcdcoord = x.t.Shopcd_Cord,
                             Dscoord = x.t.DS_Cord

                }).ToList();
            }
            return lstTemplate;
        }
        public List<CustomerDisplayModel> GetTemplates(string CustId)
        {
            List<CustomerDisplayModel> lstTemplate = new List<CustomerDisplayModel>();
            using (var context = new DSMdbEntities())
            {
                //.Where(x => !string.IsNullOrEmpty(x.Inv_No_Cord))
                //lstTemplate = context.TemplateMasters.Select(x => new CustomerDisplayModel()
                lstTemplate = context.ClientMasters.Where(x => x.ClientId == CustId).Select(x => new CustomerDisplayModel()
                    {
                    Tempname=x.TemplateName
                    }).ToList();
            }
            return lstTemplate;
        }
        public List<CustomerDisplayModel> GetAllTemplatesCord(string Tempname)
        {
            List<CustomerDisplayModel> lstTemplate = new List<CustomerDisplayModel>();
            using (var context = new DSMdbEntities())
            {
                //.Where(x => !string.IsNullOrEmpty(x.Inv_No_Cord))
                //lstTemplate = context.TemplateMasters.Select(x => new CustomerDisplayModel()
                lstTemplate = context.TemplateMasters.Join(context.ClientMasters, t => t.Template_Name, c => c.TemplateName,
                    (t, c) => new { t, c }).Where(x =>  x.t.Template_Name == Tempname).Select(x => new CustomerDisplayModel()
                    {


                         //Tempname = x.Template_Name,
                         Invnocoord = x.t.Inv_No_Cord,
                        Invdatecoord = x.t.Inv_Date_Cord,
                        Vendorcoord = x.t.Vendor_Cord,
                        Trucknocoord = x.t.Truck_No_Cord,
                        Clientnamecoord = x.t.Client_Name_Cord,
                        Dscoord = x.t.DS_Cord,
                        Shopcdcoord = x.t.Shopcd_Cord

                    }).ToList();
            }
            return lstTemplate;
        }
        public List<CustomerDisplayModel> GetAllTemplates()
        {
            List<CustomerDisplayModel> lstTemplate = new List<CustomerDisplayModel>();
            using (var context = new DSMdbEntities())
            {
                //.Where(x => !string.IsNullOrEmpty(x.Inv_No_Cord))
                lstTemplate = context.TemplateMasters.Select(x => new CustomerDisplayModel()
                {
                    
                    Tempname = x.Template_Name,
                    //Invnocoord = x.Inv_No_Cord,
                    //Invdatecoord = x.Inv_Date_Cord,
                    //Vendorcoord = x.Vendor_Cord,
                    //Trucknocoord = x.Truck_No_Cord,
                    //Clientnamecoord = x.Client_Name_Cord,
                    //Dscoord = x.DS_Cord

                }).ToList();
            }
            return lstTemplate;
        }
        public List<ClientMaster> GetCoordinates(string ClientId)
        {
            List<ClientMaster> lstClientMaster = new List<ClientMaster>();

            lstClientMaster = dsmcontext.ClientMasters.Where(x => x.ClientId == ClientId).ToList();

            return lstClientMaster;
        }
        public bool IsEmailAlreadySent(string invoiceNo)
        {
            bool sent = false;
            string conStr = ConfigurationManager.ConnectionStrings["strcon"].ConnectionString;

            using (SqlConnection _objCon = new SqlConnection(conStr))
            {
                string query = "SELECT [APIStatus] FROM [Invoice] WHERE [InvNumber] = @InvoiceNo";
                using (SqlCommand cmd = new SqlCommand(query, _objCon))
                {
                    cmd.Parameters.AddWithValue("@InvoiceNo", invoiceNo);
                    _objCon.Open();
                    var result = cmd.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        string status = result.ToString().Trim().ToUpper();

                        // Check if it equals "MAIL SENT"
                        if (status == "MAIL SENT")
                        {
                            sent = true;
                        }
                    }
                }
            }
            return sent;
        }


        public void MarkEmailAsSent(string invoiceNo)
        {
            string conStr = ConfigurationManager.ConnectionStrings["strcon"].ConnectionString;

            string apistatus = "MAIL SENT";

            using (SqlConnection _objCon = new SqlConnection(conStr))
            {
                string query = "UPDATE [Invoice] SET APIStatus ='"+ apistatus.ToString().Trim() + "' WHERE [InvNumber] = @InvoiceNo";
                using (SqlCommand cmd = new SqlCommand(query, _objCon))
                {
                    cmd.Parameters.AddWithValue("@InvoiceNo", invoiceNo);
                    _objCon.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void LogErrors(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            string path = AppDomain.CurrentDomain.BaseDirectory + "LogError.txt";
            // flush every 20 seconds as you do it

            sb.AppendLine();
            sb.Append("Log Time: " + DateTime.Now);
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();
            if (ex != null)
            {
                sb.Append("Exception: " + ex.ToString());
                if (ex.InnerException != null)
                {
                    sb.Append("Inner Exception: " + ex.InnerException.ToString());
                }
            }
            sb.AppendLine();
            sb.Append("--------------------------------------------------");
            sb.AppendLine();

            File.AppendAllText(path, sb.ToString());
            sb.Clear();
        }
    }
}

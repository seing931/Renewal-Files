using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using RenewalTransFiles.Models;
using System.IO;
using System.Text;
using Ionic.Zip;
using System.Net;

namespace RenewalTransFiles.Services
{
    public class MotorPolicyService
    {
        private readonly SqlConnConfig _config;
        public MotorPolicyService(SqlConnConfig config)
        {
            _config = config;
        }
        public async Task<List<MotorPolicy>> GetIssueMotor()
        {
            using (var conn = new SqlConnection(_config.Value))
            {
                string query = @"Select CoverNoteNo,QuotationNo,CASE JPJStatus WHEN 'C' THEN 'Successfully Captured by JPJ' WHEN 'R' THEN 'Reject' WHEN 'S' THEN 'Sent to JPJ but waiting for acknowledgment' END AS JPJStatus,AgentCode,IssueDate From MotorPolicy where PolicyStatus='I' and JPJStatus IN ('S','C') and Condition NOT IN ('0','N') AND DATEDIFF(DAY,IssueDate,GETDATE()) >= 1 and DataBatchNo is null and CoverNoteNo not in (SELECT * FROM (SELECT CoverNoteNo FROM MotorPolicy GROUP BY CoverNoteNo HAVING COUNT(CoverNoteNo) > 1) AS a)order by IssueDate desc";

                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                try
                {
                    return await Task.FromResult((List<MotorPolicy>)await conn.QueryAsync<MotorPolicy>(query));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
        }
        public async Task<List<MotorPolicy>> SearchMotor(string CoverNote)
        {
            using (var conn = new SqlConnection(_config.Value))
            {
                string query = @"Select CoverNoteNo,QuotationNo,CASE PolicyStatus WHEN 'I' THEN 'Policy Issued' WHEN 'P' THEN 'Cover Note Generated' WHEN 'Q' THEN 'Quotation Generated' WHEN 'AP' THEN 'Pending Payment from Customer' WHEN 'C' THEN 'Cancel' WHEN 'WIP' THEN 'Work In progress' WHEN 'RN' THEN 'Not Process'WHEN 'RA' THEN 'Approved'WHEN 'RR' THEN 'Rejected'WHEN 'RNQ' THEN 'Not Process (Quotation)'WHEN 'RAQ' THEN 'Approved (Quotation)'WHEN 'RRQ' THEN 'Rejected (Quotation)'END AS PolicyStatus,CASE JPJStatus WHEN 'C' THEN 'Successfully Captured by JPJ' WHEN 'R' THEN 'Reject' WHEN 'S' THEN 'Sent to JPJ but waiting for acknowledgment' END AS JPJStatus,AgentCode,IssueDate,Isnull(DataBatchNo,'')As DataBatchNo From MotorPolicy where CoverNoteNo In ('" + CoverNote.Replace(",","','") + "') order by IssueDate desc";

                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                try
                {
                    return await Task.FromResult((List<MotorPolicy>)await conn.QueryAsync<MotorPolicy>(query));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
        }
        public async Task<List<MotorPolicy>> GetPendMotor(string CoverNote)
        {
            using (var conn = new SqlConnection(_config.Value))
            {
                string query = @"Select CoverNoteNo,QuotationNo,'Policy Issued' As PolicyStatus,AgentCode,IssueDate,Isnull(DataBatchNo,'')As DataBatchNo From MotorPolicy where PolicyStatus='I' and JPJStatus='S' and CoverNoteNo='" + CoverNote + "'";

                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                try
                {
                    return await Task.FromResult((List<MotorPolicy>)await conn.QueryAsync<MotorPolicy>(query));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                }
            }
        }
        public void UpdStatus(string CoverNote)
        {
            using (var conn = new SqlConnection(_config.Value))
            {
                string[] result = CoverNote.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);

                foreach (string strCN in result)
                {
                    string query = @"EXEC [TIMB_Motor].[dbo].[Prc_DE_JPJStatus] @CoverNote";

                    if (conn.State == ConnectionState.Closed)
                        conn.Open();
                    try
                    {
                        SqlCommand command = new SqlCommand(query, conn);
                        command.Parameters.Add("@CoverNote", SqlDbType.VarChar);
                        command.Parameters["@CoverNote"].Value = strCN;
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
            }
        }
        public void ExportTo(string CoverNote)
        {
            using (var conn = new SqlConnection(_config.Value))
            {
                string query = @"EXEC [TIMB_Motor].[dbo].[Prc_DE_CoverNote_Mst_SIT] '" + CoverNote + "'";

                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                try
                {
                    SqlCommand command = new SqlCommand(query, conn);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                        conn.Close();

                        DownloadFile();
                }
            }
        }
        public void DownloadFile()
        {
            try
            {
                string path = @"C:\Renewal_CN";
                Directory.Delete(path, true);
            }catch {}
            finally
            {
                //Int_out_MTTRXPOL
                string qry_MTTRXPOL = "Select cnagentcode,cncovernoteno,cncompcode,cntitle,cnname,cnnewic,cnemail,cngender,cnbirthdate,cnaddress1,cnaddress2,cnaddress3,cnpostcode,cntowndesc,cnstatecode,cnstatedesc,cncountry,cnmaritalstatus,cntphoneno,cnhphoneno,cnophoneno,cnoccupdesc,cndriveexp,cnvehclaim,cnwinclaim,cntheftclaim,cnthirdclaim,cnmakeyear,cnregno,cnchassisno,cnlogbookno,cntrailerno,cnsuminsured,cnengineno,cnvehbodycode,cnvehbody,cnvehcolor,cnmakecodemajor,cnmakecodeminor,cnmakedesc,cnvehcapacity,cnvehcapacitycode,cnseatcapacity,cnvehtypecode,cnvehtypedesc,cnregion,cnownershiptype,cnvehiclefrom,cncondition,cncovercode,cncoverdesc,CONVERT(varchar, cneffectivedate),CONVERT(varchar,cneffectivetime),CONVERT(varchar,cnexpirydate),cnadddrv,CONVERT(varchar,cnInsertStmp,120),cnbizregno,cntotaldue,cntariffpremium,cntariffloading,cnloadingperc,cnloadingamt,cnloadingperc2,cnloadingamt2,cnncdperc,cnncdamt,cnexcess,cnusecode,cnusage,cnreferer,cnstatus,CONVERT(varchar,cnpaydate,120),cnhpcode,cnlessor,cnoldic,cnpaystatus,cnpurpose,cnstampduty,cncommiperc,cncommiamt,cnservicetaxperc,cnservicetaxamt,cndiscountperc,cndiscountamt,cnmudarabahamt,cngrossprem,cnnetprem,cngrossdue,cnnetdue,cnpayamt,cnrenewno,cndocpropform,cndocpropformdate,cndocregcard,cndocregcarddate,cndocphycvnote,cndocphycvnotedate,cndocncdproof,cndocncdproofdate,cndocSubmitted,cnbranchcode,cnpassportno,cnissueby,cnrace,cnreligion,cnoccupmajor,cnoccupminor,cngarage,cnpiamdrv,cnsafety,cnantitd,cnsperiodrate,cnmaspolicyno,cnissuedate,cnjpjstatus,Ref1,Ref2,preinscode,preinsname,preinspolno,preinsregno,preinsncd,Insbatchno,cnadmfeesperc,cnadmfeesamt,prepoleffdate,prepolexpdate,insref1,insref2,cnrefreasoncode,cnremarks,cncino,cnactprem,cnpurchaseprice,CONVERT(varchar, cnpurchasedate),cnmtcycrider,cnncdrecovery,cntransferfee,cnfecode,cnfename,cnlgcode,cnlgname,CONVERT(varchar,cnciprintstmp,120),cnpaytype,cnbankrefno,cnbankname,cnziipinfo1,cnziipinfo2,cnzicvv,cnziname,cnzimode,cnziappcode,cnzitraceno,cnrefundind,cndrivelicense,cncampaigncode,cnmktcode,cnhipcode,cndealercode,Cngeofactor,Cnhptype,Cnexcesscode,Cndocdist,clausecode,cicode,cidrvcode,warranty,cndppano,CONVERT(varchar,cndppatimestmp,120),cnredbookind,hsbcprod,hsbccode,hsbcclient,cnwcino,CONVERT(varchar,cnwcitimestmp,120),cndapano,CONVERT(varchar,cndapatimestmp,120),wcisumins,bizsectorcode,cnagreedvalue,CONVERT(varchar,Ismncdeffdt),cnvehcarrygood,cnvehaccidentdate,CONVERT(varchar,Disncdeffdt),custcosent,n4nindicator,Cnuwagreed,Hsbcname,seqno,nobenef,cnstaffcode,Cnservins,oldcino,olddppano,oldwcino,olddapano,NVIC,ABISUMINS,ABIOVERSI,PDPACONSENT,TAXABLE,UAG_NATIONALITYCODE,GSTPURPOSE,GSTSOLEPROPRIETOR,GSTREGNO,CONVERT(varchar,GSTREGDATE),CONVERT(varchar,GSTENDDATE),GSTCODE,GSTPERC,GSTAMOUNT,GSTAMTCOMM,TAXINVNO,CONVERT(varchar,TAXEFFDATE),TMMI_NATIONALITYCODE,GSTCAIND,cnPolicyNo,QuotationNo,oPmtStatus,oCoverCode,oUsage,CONVERT(varchar,obirthdate),CONVERT(varchar,odocpropformdate),CONVERT(varchar,odocregcarddate),CONVERT(varchar,odocphycvnotedate),CONVERT(varchar,odocncdproofdate),CONVERT(varchar,oprepoleffdate),CONVERT(varchar,oprepolexpdate),ooccupcode,oVehicleBodyCode,oVehicleBody,oCoverNoteStatus,PAYDStatus,VERKEY,TECH_PREM,T_BASICPREM_SR,ANNUALPREM,SRATE,BizNewRegNo,RiskDiscPerc,SchoolName,CorporateDisc from Int_out_MTTRXPOL";
                GenerateTxtRcdFile("MTTRXPOL", qry_MTTRXPOL, "CorporateDisc");

                //Int_out_MTTRXDRV
                string qry_MTTRXDRV = "Select cncompcode,cncovernoteno,cnregno,cndrvno,cndrvname,cnnewic,cnoldic,cnpassportno,cnoccupdesc,cndrvage,cndrvexp,cnoccupmajor,cnoccupminor,CONVERT(varchar, birthdate),maritalstatus,gender,drvrelation,seqno from Int_out_MTTRXDRV";
                GenerateTxtRcdFile("MTTRXDRV", qry_MTTRXDRV, "seqno");

                //Int_out_MTTRXCEW
                string qry_MTTRXCEW = "Select cncompcode,cncovernoteno,cnregno,cncewcode,cnbencode,cnbendesc,cnsuminsured,cncapacityunit,cnbenpremium,cnbenexcess,cnformulatype,cnrate,cncertno,cnaddress1,cnaddress2,cnaddress3,cnpostcode,cntowndesc,cnstatecode,cnstatedesc,cncomperc,cncomamt,cnstampduty,cnsertaxperc,cnsertaxamt,cndiscperc,cndiscamt,seqno from Int_out_MTTRXCEW";
                GenerateTxtRcdFile("MTTRXCEW", qry_MTTRXCEW, "seqno");

                ////int_out_NMCNPOL
                string qry_NMCNPOL = "Select compcode, covernoteno, endorsementno, agentcode, accountcode, branchcode, policyno, renewno, classcode, subclasscode, subclassdesc, status, title, insuredname, newic, oldic, passportno, bizregno, birthdate, address1, address2, address3, postcode, towndesc, statecode, statedesc, address21, address22, address23, postcode2, towndesc2, statecode2, statedesc2, maritalcode, gender, race, religion, nationcode, nationality, email, tphoneno, hphoneno, ophoneno, occupmajor, occupminor, occupdesc, dutycode, dutydesc, effectdate, CONVERT(varchar, effecttime), expirydate, empnm, emaddress1, emaddress2, emaddress3, empostcode, emtowndesc, emstatecode, emstatedesc, emptelno, income, suminsured, totaldue, tariffpremium, tariffloading, loadingperc, loadingamt, grosspremium, netpremium, grossdue, netdue, commissionperc, commissionamt, discountperc, discountamt, servtaxperc, servtaxamt, stampduty, mudhamt, fees, claimtamt, speriodrate, purpose, substat, CONVERT(varchar, substmp, 120), payno, paymode, paystat, payamt, paystmp, payconfirmind, payconfirmby, payconfirmdate, receiptno, receiptdate, masterpolicyno, prdofmonth, imgbchcode, issueby, CONVERT(varchar, insertstmp, 120), cancelby, cancelbystmp, cancelreason, prcsflag, fecode, fename, lgcode, lgname, paytype, bankrefno, bankname, ziipinfo1, ziipinfo2, zicvv, ziname, zimode, ziappcode, zitraceno, f1fees, rebate, prevcvnno, fininterest, faxno, bankrefno2, bizcode, bizdesc, insuredname2, newic2, oldic2, referenceno, netdue2, grossdue2, totaldue2, addfees1, pdpaconsent, taxable, seqno, fwno, GSTREG, GSTREGNO, GSTREGDATE, GSTENDDATE, GSTPURPOSE, GSTPERC, GSTAMT, GSTCODE, COMMGSTAMT, GSTDESTINATION, GSTOVERWRITE, GSTADDFEES, GSTTAXINVNO, GSTTAXINVDATE, GSTSOLEPROPRIETOR, GSTCAIND, QuotationNo, opaystat, ostatus, obirthdate, CONVERT(varchar, opaystmp, 120), CONVERT(varchar, oreceiptdate, 120), CONVERT(varchar, ocancelbystmp, 120), CONVERT(varchar, opayconfirmdate, 120)As opayconfirmdate from int_out_NMCNPOL";
                GenerateTxtRcdFile("NMCNPOL", qry_NMCNPOL, "opayconfirmdate");

                //int_out_NMCNITEM
                string qry_NMCNITEM = "Select compcode,covernoteno,endorsementno,locno,[lineno],itemcode,itemdesc,suminsured,premium,rate,iteminfo1,iteminfo2,iteminfo3,iteminfo4,iteminfo5,Iteminfo6,Iteminfo7,iteminfo8,iteminfo9,iteminfo10,iteminfo11,iteminfo12,iteminfo13,iteminfo14,iteminfo15,iteminfo16,iteminfo17,iteminfo18,iteminfo19,Iteminfo20,seqno,QuotationNo from int_out_NMCNITEM";
                GenerateTxtRcdFile("NMCNITEM", qry_NMCNITEM, "QuotationNo");

                //int_out_NMCNNOMINATE
                string qry_NMCNNOMINATE = "Select compcode,covernoteno,endorsementno,[lineno],type,title,name,newic,oldic,passportno,bizregno,CONVERT(varchar,birthdate),relationship,percentage,address1,address2,address3,postcode,towndesc,statecode,statedesc,insuredname,contactno,nationcode,nationdesc,age,seqno,locno,QuotationNo from int_out_NMCNNOMINATE";
                GenerateTxtRcdFile("NMCNNOMINATE", qry_NMCNNOMINATE, "QuotationNo");
            }
        }
        public string GenerateTxtRcdFile(string filename, string query, string lastClmName)
        {
            string path = @"C:\Renewal_CN";
            string filedate = DateTime.Now.ToString("yyyyMMddHHmmss");
            string RcdFilePath;
            string txt = string.Empty;

            SqlConnection con = new SqlConnection(_config.Value);
            SqlCommand cmd = new SqlCommand(query);
            SqlDataAdapter sda = new SqlDataAdapter();

            cmd.Connection = con;
            cmd.CommandTimeout = 3600;
            sda.SelectCommand = cmd;

            using (DataTable dt = new DataTable())
            {
                sda.Fill(dt);

                txt = "BEGIN";
                txt += "\r\n";

                foreach (DataRow row in dt.Rows)
                {
                    foreach (DataColumn column in dt.Columns)
                    {
                        if (column.ColumnName == lastClmName)
                        {
                            //without "|"
                            txt += row[column.ColumnName].ToString();
                        }
                        else
                        {
                            txt += row[column.ColumnName].ToString() + "|";
                        }
                    }
                    txt += "\r\n";
                }

                txt += "END";

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                RcdFilePath = String.Concat(@"C:\", @"Renewal_CN\" + filename + "_" + filedate + ".RECORD");

                FileInfo RcdFile = new FileInfo(RcdFilePath);

                if (!RcdFile.Directory.Exists) { RcdFile.Directory.Create(); }

                StreamWriter sw = new StreamWriter(RcdFilePath, true, Encoding.ASCII);
                sw.Write(txt);
                sw.Close();
            }

            return txt;
        }
    }
}

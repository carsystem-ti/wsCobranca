using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace wsCobranca
{
    /// <summary>
    /// Summary description for Cobranca
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]

    public class Cobranca : System.Web.Services.WebService
    {
        funcoesCobranca iFuncoes = new funcoesCobranca();

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string getBoletos(string pContrato, string pUsuario, string pSenha)
        {
            try
            {

                if (!iFuncoes.isLoginValido(pUsuario, pSenha))
                    return ("[{\"erro\": \"login invalido\"}]");

                return CarSystem.Utilidades.Rede.HTML.tableToJson(iFuncoes.getBoletos(pContrato));

            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "-" + ex.Message + "\"}]";
            }
        }

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string getClientes(string pUsuario, string pSenha)
        {
            try
            {

                if (!iFuncoes.isLoginValido(pUsuario, pSenha))
                    return "[{\"erro\": \"login invalido\"}]";

                return tableToJson(iFuncoes.getClientes());

            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - " + ex.Message + "\"}]";
            }
        }
        public static string tableToJson(DataTable pTabela)
        {
            JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> dictionaryList = new List<Dictionary<string, object>>();
            scriptSerializer.MaxJsonLength = 50000000;
            foreach (DataRow row in (InternalDataCollectionBase)pTabela.Rows)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                foreach (DataColumn column in (InternalDataCollectionBase)pTabela.Columns)
                    dictionary.Add(column.ColumnName.Trim(), row[column.ColumnName.Trim()]);
                dictionaryList.Add(dictionary);
            }
            return scriptSerializer.Serialize((object)dictionaryList);
        }
     
        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string getCheques(string pContrato, string pUsuario, string pSenha)
        {
            try
            {

                if (!iFuncoes.isLoginValido(pUsuario, pSenha))
                    return ("[{\"erro\": \"login invalido\"}]");

                return CarSystem.Utilidades.Rede.HTML.tableToJson(iFuncoes.getCheques(pContrato));

            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - " + ex.Message + "\"}]";
            }
        }

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string geraNegociacao(string pParcelas, string pValor, string pVencimento, string pUsuario, string pSenha)
        {
            try
            {
                if (!iFuncoes.isLoginValido(pUsuario, pSenha))
                    return ("[{\"erro\": \"login invalido\"}]");

                double iValor = Convert.ToDouble(pValor);//.Insert(pValor.Length - 2, ","));

                return iFuncoes.geraNegociacao(pParcelas.Split(';'), iValor, DateTime.ParseExact(pVencimento, "d/M/yyyy", null), funcoesCobranca.tipoDebito.Boleto);
            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - " + ex.Message + "\"}]";
            }
        }

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string geraNegociacaoCheque(string pParcelas, string pValor, string pVencimento, string pUsuario, string pSenha)
        {
            try
            {
                if (!iFuncoes.isLoginValido(pUsuario, pSenha))
                    return ("[{\"erro\": \"login invalido\"}]");

                double iValor = Convert.ToDouble(pValor);//.Insert(pValor.Length - 2, ","));

                return iFuncoes.geraNegociacao(pParcelas.Split(';'), iValor, DateTime.ParseExact(pVencimento, "d/M/yyyy", null), funcoesCobranca.tipoDebito.Cheque);
            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - " + ex.Message + "\"}]";
            }
        }

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string quitaParcela(string pCodigoParcela, string pValorQuitado, string pNumeroAutorizacao, string pTipoQuitacao, string pUsuario, string pSenha)
        {
            try
            {
                if (!iFuncoes.isLoginValido(pUsuario, pSenha))
                    return ("[{\"erro\": \"login invalido\"}]");

                return iFuncoes.efetuaQuitacao(pCodigoParcela, Convert.ToDouble(pValorQuitado), pNumeroAutorizacao);
            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - " + ex.Message + "\"}]";
            }
        }

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string setCancelaParcela(string pCodigoParcela, string pUsuario, string pSenha)
        {
            try
            {

                if (!iFuncoes.isLoginValido(pUsuario, pSenha))
                    return ("[{\"erro\": \"login invalido\"}]");

                return iFuncoes.setCancelarParcela(Convert.ToInt64(pCodigoParcela));
            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - " + ex.Message + "\"}]";
            }
        }

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string setCargaContratos(string pNumerosContrato, string pUsuario, string pSenha)
        {
            try
            {
                if (!iFuncoes.isLoginValido(pUsuario, pSenha))
                    return ("[{\"erro\": \"login invalido\"}]");

                return iFuncoes.setCarga(pNumerosContrato);
            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - " + ex.Message + "\"}]";
            }
        }

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string getParcelasQuitadas(string pDataInicial, string pDataFinal, string pUsuario, string pSenha)
        {
            try
            {
                if (!iFuncoes.isLoginValido(pUsuario, pSenha))
                    return ("[{\"erro\": \"login invalido\"}]");

                return CarSystem.Utilidades.Rede.HTML.tableToJson(iFuncoes.getParcelasQuitadas(DateTime.ParseExact(pDataInicial, "d/M/yyyy", null), DateTime.ParseExact(pDataFinal, "d/M/yyyy", null)));
            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - " + ex.Message + "\"}]";
            }
        }

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string getContratosExcluidos(string pUsuario, string pSenha)
        {
            try
            {
                if (!iFuncoes.isLoginValido(pUsuario, pSenha))
                    return ("[{\"erro\": \"login invalido\"}]");

                return CarSystem.Utilidades.Rede.HTML.tableToJson(iFuncoes.getContratosExcluidos());
            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - " + ex.Message + "\"}]";
            }
        }

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string getLogCobranca(string pNumerosContrato, string pUsuario, string pSenha)
        {
            try
            {
                if (!iFuncoes.isLoginValido(pUsuario, pSenha))
                    return ("[{\"erro\": \"login invalido\"}]");

                return iFuncoes.getLogCobranca(pNumerosContrato);
            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - " + ex.Message + "\"}]";
            }
        }

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string setLogCobranca(string pNumeroContrato, string pCodigoCobrador, string pDataContato, string pNomeUsuario, string pDataLancamento
            , string pDataProxLigacao, string pDescricaoAnalise, string pObservacao, string pUsuario, string pSenha)
        {
            try
            {
                if (!iFuncoes.isLoginValido(pUsuario, pSenha))
                    return ("[{\"erro\': \"login invalido\"}]");

                return iFuncoes.setLogCobranca(pNumeroContrato, pCodigoCobrador, DateTime.ParseExact(pDataContato, "d/M/yyyy", null)
                    , pNomeUsuario, DateTime.ParseExact(pDataLancamento, "d/M/yyyy", null), DateTime.ParseExact(pDataProxLigacao, "d/M/yyyy", null)
                    , pDescricaoAnalise, pObservacao);
            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - " + ex.Message + "\"}]";
            }
        }

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string setAlteraParcela(string pCodigoParcela, double pValor, DateTime pVencimento, string pUsuario, string pSenha)
        {
            try
            {
                if (!iFuncoes.isLoginValido(pUsuario, pSenha))
                    return ("[{\"erro\": \"login invalido\"}]");

                double iValor = Convert.ToDouble(pValor);//.Insert(pValor.Length - 2, ","));

                return iFuncoes.setAlteraVencimento(pCodigoParcela, pValor, pVencimento);
            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - " + ex.Message + "\"}]";
            }
        }
        //11-04-2014
        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string setRenovaCliente(string nContrato, double vlRenova, double vlTaxa, double vlGuincho, string nrAut,int nrPC, string cdBanco, string nrCob,int nrPcRen, string pUsuario, string pSenha)
        {
            try
            {
                if (!iFuncoes.isLoginValido(pUsuario, pSenha))
                    return ("[{\"erro\": \"login invalido\"}]");

                return iFuncoes.setRenovaCliente(nContrato, vlRenova, vlTaxa, vlGuincho, nrAut, nrPC, cdBanco, nrCob,nrPcRen);
            }
            catch (Exception ex)
           { 
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - " + ex.Message + "\"}]";
            }
        }

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string setProcessaPagamento(string pUsuario,string  pSenha,int ReferenceCode, decimal TotalAmount, string Number, string ExpirationDate, string SecutityCode, int  NumberOfInstallments, string User)
        {
            return efetuaTEFTransacao(pUsuario, pSenha,ReferenceCode.ToString(), TotalAmount.ToString(), Number, ExpirationDate, SecutityCode, NumberOfInstallments.ToString(),User, "", "");
        }

        [WebMethod]
        [System.Web.Script.Services.ScriptMethod(UseHttpGet = false, ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public string efetuaTEFTransacao(string user, string senha, string evento, string valor, string nr_cartao, string dt_validade, string nr_seguranca, string nr_parcelas, string usuario, string Cpf_Cnpj, string ds_NomePagamento)
        {
            string retorno = null;

            if (!iFuncoes.isLoginValido(user, senha))
                return ("[{\"erro\": \"login invalido\"}]");

               string sUrl = "https://10.0.0.11/promocao/ctr/pagamento.ashx";

                //string iParemetros = "pFuncao=setTEF&pIdentifica=" + pIdentifica + "&pValor=" + pValor + "&pDados={\"numeroCartao\":\"" + numeroCartao + "\",\"codigoSeguranca\":\"" + codigoSeguranca + "\",\"validade\":\"" + validade + "\",\"parcelas\":" + parcelas + ",\"formaPagamento\":1,\"usuario\":\"GDA-TEF\"}";

                valor = Convert.ToDouble(valor).ToString();



                string iParemetros =
                "{" +
                    "\"pFuncao\":\"setTEF\"," +
                    "\"pIdentifica\":" + evento + "," +
                    "\"pValor\":\"" + valor + "\"," +
                    "\"pDados\":'{" +
                        "\"numeroCartao\":\"" + nr_cartao + "\"," +
                        "\"codigoSeguranca\":\"" + nr_seguranca + "\"," +
                        "\"validade\":\"" + dt_validade + "\"," +
                        "\"parcelas\":" + nr_parcelas + "," +
                        "\"formaPagamento\":1," +
                        "\"usuario\":\"" + usuario + "\"" +
                    "}'" +
                "}";

                string sResponse = executeWS(tipoRequisicao.POST, sUrl, iParemetros);

                sResponse = sResponse.Replace("\"", "");
                sResponse = sResponse.Replace("{", "");
                sResponse = sResponse.Replace("}", "");
                sResponse = sResponse.Replace("\\u003cbr/\u003e", " ");
                sResponse = sResponse.Replace("\\u0026Atilde;", "Ã");
                sResponse = sResponse.Replace("\\u0026Ccedil;", "Ç");
                sResponse = sResponse.Replace("\\u003cbr/\\u003e", " ");
                sResponse = sResponse.Replace("\u003cbr/\u003e", " ");

                string[] iTexto;

                foreach (string iDividido in sResponse.Split(','))
                {
                    iTexto = iDividido.Split(':');

                    if (iTexto[0] == "isOK" && Convert.ToBoolean(iTexto[1]))
                    {
                        retorno = iTexto[1].ToString().Replace("\\r\\n", "-");

                        return "APROVADO";

                    }
                    else if (iTexto[0] == "erro")
                        retorno = "REPROVADO : " + iTexto[1].ToString().Replace("\\r\\n", "-");
                }
            
            return retorno;
        }
        public enum tipoRequisicao
        {
            POST, PUT, DELETE
        }

        public string executeWS(tipoRequisicao pTipoRequisicao, string pEndereco, string pData)
        {

            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (Object obj, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                     System.Security.Cryptography.X509Certificates.X509Chain chain,
                    System.Net.Security.SslPolicyErrors errors)
                {
                    return (true);
                };

                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                Uri iAddress = new Uri(pEndereco);

                System.Net.WebRequest iRequest = System.Net.WebRequest.Create(iAddress) as HttpWebRequest;
                iRequest.Headers.Add("Authorization", "Basic c2V0VEVGOmNhcmxvc0VkdWFyZG9QaWVyZW4=");

                iRequest.Method = pTipoRequisicao.ToString();
                iRequest.ContentType = "application/json; charset=utf-8";

                using (Stream s = iRequest.GetRequestStream())
                {
                    using (StreamWriter sw = new StreamWriter(s))
                        sw.Write(pData);
                }

                using (System.Net.HttpWebResponse response = iRequest.GetResponse() as System.Net.HttpWebResponse)
                {
                    System.IO.StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
                    return reader.ReadToEnd().Replace("\r", "").Replace("\n", "");
                }
            }
            catch (WebException we)
            {
                HttpWebResponse errorResponse = we.Response as HttpWebResponse;
                string r = new StreamReader(we.Response.GetResponseStream()).ReadToEnd();

                return r;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}

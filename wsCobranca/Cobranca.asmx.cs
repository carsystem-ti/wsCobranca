using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

                return CarSystem.Utilidades.Rede.HTML.tableToJson(iFuncoes.getClientes());

            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - " + ex.Message + "\"}]";
            }
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


        public string setRenovaCliente(string nContrato, double vlRenova, double vlTaxa, double vlGuincho, string nrAut, int nrPC, string cdBanco, string nrCob, string pUsuario, string pSenha)
        {
            try
            {
                if (!iFuncoes.isLoginValido(pUsuario, pSenha))
                    return ("[{\"erro\": \"login invalido\"}]");

                return iFuncoes.getLogCobranca(nContrato);
            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + " -  Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + " - " + ex.Message + "\"}]";
            }
        }

    }
}

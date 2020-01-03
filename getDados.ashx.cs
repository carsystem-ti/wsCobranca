using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsCobranca
{
    /// <summary>
    /// Summary description for getDados
    /// </summary>
    public class getDados : IHttpHandler
    {
        funcoesCobranca iFuncoes = new funcoesCobranca();

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "json";

            if (!string.IsNullOrEmpty(context.Request.Form["pUsuario"]) && !string.IsNullOrEmpty(context.Request.Form["pUsuario"]))
            {
                if (!iFuncoes.isLoginValido(context.Request.Form["pUsuario"].ToString(), context.Request.Form["pSenha"]))
                {
                    context.Response.Write("[{\"erro\": \"login invalido\"}]");
                }
                else
                {
                    if (!string.IsNullOrEmpty(context.Request.Form["pFuncao"]))
                    {

                        switch (context.Request.Form["pFuncao"].ToString())
                        {
                            case "getBoletos":
                                context.Response.Write(getBoletos(context.Request.Form["pContrato"].ToString()));
                                break;
                            case "getClientes":
                                context.Response.Write(getClientes());
                                break;
                            case "getCheques":
                                context.Response.Write(getCheques(context.Request.Form["pContrato"].ToString()));
                                break;
                        }
                        context.Response.End();
                    }
                }
            }

            if (string.IsNullOrEmpty(context.Request.Form["pUsuario"]))
                context.Response.Write("[{\"erro\": \"usuario invalido\"}]");
            else if (string.IsNullOrEmpty(context.Request.Form["pSenha"]))
                context.Response.Write("[{\"erro\": \"senha invalida\"}]");
            else if (string.IsNullOrEmpty(context.Request.Form["pFuncao"]))
                context.Response.Write("[{\"erro\": \"funcao invalida\"}]");
            else if (string.IsNullOrEmpty(context.Request.Form["pContrato"]))
                context.Response.Write("[{\"erro\": \"contrato invalido\"}]");

        }
        

        private string getBoletos(string pContrato)
        {
            try
            {
                return CarSystem.Utilidades.Rede.HTML.tableToJson(iFuncoes.getBoletos(pContrato));
            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message + "\"}]";
            }
        }

        private string getClientes()
        {
            try
            {
                return CarSystem.Utilidades.Rede.HTML.tableToJson(iFuncoes.getClientes());
            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message + "\"}]";
            }
        }

        private string getCheques(string pContrato)
        {
            try
            {
                return CarSystem.Utilidades.Rede.HTML.tableToJson(iFuncoes.getCheques(pContrato));
            }
            catch (Exception ex)
            {
                return "[{\"erro\": \"" + "ERRO##class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message + "\"}]";
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
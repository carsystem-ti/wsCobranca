using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace wsCobranca
{
    public class funcoesCobranca
    {

        CarSystem.BancoDados.Dados _bancoDados;
        CarSystem.Login.Login oLogin = new CarSystem.Login.Login();

        private readonly string codigoCobradora = "009530";
        public enum tipoDebito { Invalido = 0, Cheque = 2, Boleto = 5 };

        public CarSystem.BancoDados.Dados bancoDados
        {
            get
            {
                if (_bancoDados == null)
                    _bancoDados = new CarSystem.BancoDados.Dados("principal", CarSystem.Tipos.Servidores.Fury
                        , System.Web.Configuration.WebConfigurationManager.AppSettings["usuarioBanco"]
                        , System.Web.Configuration.WebConfigurationManager.AppSettings["senhaBanco"]);

                return _bancoDados;
            }
            set { _bancoDados = value; }
        }

        private tipoDebito getTipoDebito(Int64 pNumeroParcela, System.Data.SqlClient.SqlTransaction pTransacao)
        {
            try
            {
                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "Principal.Cobranca.pro_getTipoDebito"; // ( @pCodigoNegociacao int )
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = true;

                bancoDados.Comandos.adicionaParametro("@pNumeroParcela", System.Data.SqlDbType.Int, 4, pNumeroParcela);

                bancoDados.Comandos.comando.Transaction = pTransacao;
                System.Data.DataTable iTabela = bancoDados.execute().Tables[0];

                return (tipoDebito)Convert.ToInt32(iTabela.Rows[0][0]);
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
        }

        public bool isLoginValido(string pUsuario, string pSenha)
        {
            if (pUsuario != "cobranca")
                return true;

            oLogin.usuario = pUsuario;
            oLogin.senha = pSenha;

            return true;
                //oLogin.validar();
        }

        public System.Data.DataTable getBoletos(string pContrato)
        {
            try
            {
                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "Principal.Cobranca.Proc_GetRegistrosDebitos";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;

                bancoDados.Comandos.adicionaParametro("@nr_contrato", System.Data.SqlDbType.VarChar, 10, pContrato);
                bancoDados.retornaDados = true;
                bancoDados.tempoLimite = 3000;
                
                System.Data.DataTable iTabelaRetorno = bancoDados.execute().Tables[0];
                bancoDados.Conexoes.close();
                return iTabelaRetorno;
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
        }

        public System.Data.DataTable getCheques(string pContrato)
        {
            try
            {
                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "Principal.Cobranca.Proc_GetRegistrosDebitosCheques";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;

                bancoDados.Comandos.adicionaParametro("@nr_contrato", System.Data.SqlDbType.VarChar, 10, pContrato);
                bancoDados.retornaDados = true;
                bancoDados.tempoLimite = 3000;
                System.Data.DataTable iTabelaRetorno = bancoDados.execute().Tables[0];
                bancoDados.Conexoes.close();
                return iTabelaRetorno;
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
        }
        public System.Data.DataTable getClientes()
        {
            try
            {
                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "Principal.Cobranca.Proc_GetClientes";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = true;
                  bancoDados.tempoLimite = 3000;
                System.Data.DataTable iTabelaRetorno = bancoDados.execute().Tables[0];
                bancoDados.Conexoes.close();
                return iTabelaRetorno;
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
        }

        private bool setParcelaNegociada(Int64 pCodigoParcela, Int64 pCodigoNegociacao, System.Data.SqlClient.SqlTransaction pTransacao)
        {

            try
            {
                bool iRetorno = false;

                bancoDados.Comandos.limpaParametros();

                bancoDados.Comandos.textoComando = "Principal..Proc_CAC_NegociarClienteParcela";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = false;
                  bancoDados.tempoLimite = 3000;
                bancoDados.Comandos.adicionaParametro("@nr_pc", System.Data.SqlDbType.BigInt, 10, pCodigoParcela);
                bancoDados.Comandos.adicionaParametro("@ds_usuario", System.Data.SqlDbType.BigInt, 10, codigoCobradora);
                bancoDados.Comandos.adicionaParametro("@id_negociacao", System.Data.SqlDbType.BigInt, 10, pCodigoNegociacao);

                bancoDados.Comandos.comando.Transaction = pTransacao;

                bancoDados.execute();

                iRetorno = (bancoDados.linhasAfetadas > 0);

                return iRetorno;
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }

        }
        private bool setItemNegociacao(Int64 pCodigoParcela, int pStatusParcela, Int64 pCodigoNegociacao, System.Data.SqlClient.SqlTransaction pTransacao)
        {
            try
            {
                bool iRetorno = false;

                bancoDados.Comandos.limpaParametros();
                //- Cobranca.Proc_GravaNegociacao '" & Usuário = “009528” & "','" & Contrato
                bancoDados.Comandos.textoComando = "Principal.Cobranca.Proc_GravaNegociacaoItens";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = false;
                  bancoDados.tempoLimite = 3000;
                bancoDados.Comandos.adicionaParametro("@id", System.Data.SqlDbType.Int, 4, pCodigoNegociacao);
                bancoDados.Comandos.adicionaParametro("@cd_parcela", System.Data.SqlDbType.Int, 4, pCodigoParcela);
                bancoDados.Comandos.adicionaParametro("@st_parcela", System.Data.SqlDbType.SmallInt, 2, pStatusParcela);

                bancoDados.Comandos.comando.Transaction = pTransacao;

                bancoDados.execute();

                iRetorno = (bancoDados.linhasAfetadas > 0);

                return iRetorno;
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
        }
        private string getContrato(Int64 pCodigoParcela, System.Data.SqlClient.SqlTransaction pTransacao)
        {
            try
            {
                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "Principal.Cobranca.pro_getContrato";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = true;
                  bancoDados.tempoLimite = 3000;
                bancoDados.Comandos.comando.Transaction = pTransacao;

                bancoDados.Comandos.adicionaParametro("@pCodigoParcela", System.Data.SqlDbType.BigInt, 4, pCodigoParcela);
                bancoDados.Comandos.adicionaParametro("@pCodigoCobrador", System.Data.SqlDbType.Char, 6, codigoCobradora);

                System.Data.DataTable iTabela = bancoDados.execute().Tables[0];

                return iTabela.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
        }
        private Int64 setNegociacao(string pContrato, System.Data.SqlClient.SqlTransaction pTransacao)
        {
            try
            {
                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "Principal.Cobranca.Proc_GravaNegociacao";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = true;
                  bancoDados.tempoLimite = 3000;
                bancoDados.Comandos.adicionaParametro("@cob", System.Data.SqlDbType.VarChar, 6, codigoCobradora);
                bancoDados.Comandos.adicionaParametro("@con", System.Data.SqlDbType.VarChar, 10, pContrato);

                bancoDados.Comandos.comando.Transaction = pTransacao;

                System.Data.DataTable iTabela = bancoDados.execute().Tables[0];

                return Convert.ToInt64(iTabela.Rows[0][0]);
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
        }
        private string setParcelaNegociacao(string pContrato, double pValor, DateTime pVencimento, Int64 pCodigoNegociacao, tipoDebito pTipoDebito, System.Data.SqlClient.SqlTransaction pTransacao)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer jsonSerialiser = new System.Web.Script.Serialization.JavaScriptSerializer();

                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "SGB.Boleto.pro_criaBoleto";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = true;
                  bancoDados.tempoLimite = 3000;
                bancoDados.Comandos.comando.Transaction = pTransacao;

                bancoDados.Comandos.adicionaParametro("@dataVencimento", System.Data.SqlDbType.DateTime, 8, pVencimento);
                bancoDados.Comandos.adicionaParametro("@valorBoleto", System.Data.SqlDbType.Money, 4, pValor);
                bancoDados.Comandos.adicionaParametro("@contrato", System.Data.SqlDbType.VarChar, 10, pContrato);
                bancoDados.Comandos.adicionaParametro("@documentoReferencia", System.Data.SqlDbType.VarChar, 12, "");
                bancoDados.Comandos.adicionaParametro("@codigoDebito", System.Data.SqlDbType.Char, 3, pTipoDebito == tipoDebito.Boleto ? "CNB" : "VNC");
                bancoDados.Comandos.adicionaParametro("@nomeCliente", System.Data.SqlDbType.VarChar, 160, "");
                bancoDados.Comandos.adicionaParametro("@numeroInterno", System.Data.SqlDbType.VarChar, 12, "");
                bancoDados.Comandos.adicionaParametro("@usuarioGerador", System.Data.SqlDbType.VarChar, 30, codigoCobradora);

                System.Data.DataTable iTabela = bancoDados.execute().Tables[0];

                if (!setItemNegociacao(Convert.ToInt64(iTabela.Rows[0]["codigoParcela"]), 0, pCodigoNegociacao, pTransacao))
                    throw new Exception("Parcela nao gravada");

                return CarSystem.Utilidades.Rede.HTML.tableToJson(iTabela);
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
        }
        private double getValorNegociacao(Int64 pCodigoNegociacao, System.Data.SqlClient.SqlTransaction pTransacao)
        {
            try
            {
                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "Cobranca.pro_getTotalNegociacao"; // ( @pCodigoNegociacao int )
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = true;
                  bancoDados.tempoLimite = 3000;
                bancoDados.Comandos.adicionaParametro("@pCodigoNegociacao", System.Data.SqlDbType.Int, 4, pCodigoNegociacao);

                bancoDados.Comandos.comando.Transaction = pTransacao;
                System.Data.DataTable iTabela = bancoDados.execute().Tables[0];

                return Convert.ToDouble(iTabela.Rows[0]["total"]);
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
        }

        private bool setQuitacaoCartao(string pCodigoParcela, double pValorQuitado, string pNumeroAutorizacao, System.Data.SqlClient.SqlTransaction pTransacao)
        {
            // Proc_CAC_QuitaParcelaCobranca @nr_pc int, @ds_usuario Varchar(50), @vl_quitacao as money, @ds_doc varchar(10)*/
            try
            {
                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "Principal..Proc_CAC_QuitaParcelaCobranca";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = false;
                bancoDados.tempoLimite = 2000;
                bancoDados.Comandos.adicionaParametro("@nr_pc", System.Data.SqlDbType.Int, 4, pCodigoParcela);
                bancoDados.Comandos.adicionaParametro("@ds_usuario", System.Data.SqlDbType.VarChar, 50, codigoCobradora);
                bancoDados.Comandos.adicionaParametro("@vl_quitacao", System.Data.SqlDbType.Money, 4, pValorQuitado);
                bancoDados.Comandos.adicionaParametro("@ds_doc", System.Data.SqlDbType.VarChar, 10, pNumeroAutorizacao);

                bancoDados.Comandos.comando.Transaction = pTransacao;
                System.Data.DataTable iTabela = bancoDados.execute().Tables[0];

                return bancoDados.linhasAfetadas > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }

        }
        private bool getExisteAutorizacao(string pNumeroAutorizacao, System.Data.SqlClient.SqlTransaction pTransacao)
        {
            bool iRetorno = false;

            try
            {
                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "Principal.Cobranca.pro_getExisteAutorizacaoCartao";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = true;
                bancoDados.tempoLimite = 2000;
                bancoDados.Comandos.adicionaParametro("@pNumeroAutorizacao", System.Data.SqlDbType.VarChar, 10, pNumeroAutorizacao);
                bancoDados.Comandos.comando.Transaction = pTransacao;

                iRetorno = Convert.ToBoolean(bancoDados.execute().Tables[0].Rows[0][0]);

                return iRetorno;
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
        }

        public string efetuaQuitacao(string pCodigoParcela, double pValorQuitado, string pNumeroAutorizacao)
        {

            System.Data.SqlClient.SqlTransaction iTransacao = bancoDados.Conexoes.conexao.BeginTransaction();

            try
            {
                if (getExisteAutorizacao(pNumeroAutorizacao, iTransacao))
                    throw new Exception("autorizacao ja cadastrada");

                if (setQuitacaoCartao(pCodigoParcela, pValorQuitado, pNumeroAutorizacao, iTransacao))
                    throw new Exception("O vencimento nao pode ser anterior a data atual");

                iTransacao.Commit();

                return "OK";
            }
            catch (Exception ex)
            {
                if (iTransacao.Connection != null)
                    iTransacao.Rollback();

                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
            finally { bancoDados.Conexoes.close(); }
        }

        public string geraNegociacao(string[] pParcelas, double pValor, DateTime pVencimento, tipoDebito pTipoDebito)
        {

            string iContrato;
            System.Data.SqlClient.SqlTransaction iTransacao = bancoDados.Conexoes.conexao.BeginTransaction();

            try
            {
                if (pParcelas.Length == 0)
                    throw new Exception("Parcelas nao informadas");

                if (pVencimento < DateTime.Today)
                    throw new Exception("O vencimento nao pode ser anterior a data atual");

                if (pVencimento > DateTime.Today.AddDays(15))
                    throw new Exception("O vencimento nao pode ser maior que 15 dias ");

                iContrato = getContrato(Convert.ToInt64(pParcelas[0]), iTransacao);

                if (iContrato == "")
                    throw new Exception("Contrato nao autorizado");

                Int64 iCodigoNegociacao = setNegociacao(iContrato, iTransacao);

                foreach (string iParcela in pParcelas)
                {
                    if (iContrato != getContrato(Convert.ToInt64(iParcela), iTransacao))
                        throw new Exception("Apenas um contrato por negociacao");

                    if (pTipoDebito != getTipoDebito(Convert.ToInt64(iParcela), iTransacao))
                        throw new Exception("Apenas um tipo de documento por negociacao");

                    if (!setParcelaNegociada(Convert.ToInt64(iParcela), iCodigoNegociacao, iTransacao))
                        throw new Exception("Item nao gerado");
                }

                double iValorTotal = getValorNegociacao(iCodigoNegociacao, iTransacao);

                if (iValorTotal * 0.5 > pValor || pValor > iValorTotal * 1.5)
                    throw new Exception("valor da negociacao invalido");

                string iRetorno = setParcelaNegociacao(iContrato, pValor, pVencimento, iCodigoNegociacao, pTipoDebito, iTransacao);

                iTransacao.Commit();

                return iRetorno;
            }
            catch (Exception ex)
            {
                if (iTransacao.Connection != null)
                    iTransacao.Rollback();

                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
            finally { bancoDados.Conexoes.close(); }
        }

        //----14-03-2014----//

        public string setCancelarParcela(Int64 pCodigoParcela)
        {

            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer jsonSerialiser = new System.Web.Script.Serialization.JavaScriptSerializer();

                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "principal.Cobranca.pro_setCancelaParcela";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = true;

                bancoDados.Comandos.adicionaParametro("@pCodigoParcela", System.Data.SqlDbType.BigInt, 8, pCodigoParcela);
                bancoDados.Comandos.adicionaParametro("@pUsuario", System.Data.SqlDbType.VarChar, 10, codigoCobradora);

                System.Data.DataTable iTabela = bancoDados.execute().Tables[0];

                return CarSystem.Utilidades.Rede.HTML.tableToJson(iTabela);

            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
        }
        public string setCarga(string pNumeroContratos)
        {
            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer jsonSerialiser = new System.Web.Script.Serialization.JavaScriptSerializer();

                string[] iContratos = pNumeroContratos.Split(';');
                List<object> iListaContratos = new List<object>();

                foreach (string iNumeroContrato in iContratos)
                    iListaContratos.Add(new { contrato = iNumeroContrato, efetuado = setCargaContrato(iNumeroContrato) ? 1 : 0 });

                return jsonSerialiser.Serialize(iListaContratos);
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
            finally { bancoDados.Conexoes.close(); }

        }
        private bool setCargaContrato(string pNumeroContrato)
        {
            try
            {
                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "principal.Cobranca.pro_setCargaContrato";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = false;

                bancoDados.Comandos.adicionaParametro("@pNumeroContrato", System.Data.SqlDbType.VarChar, 10, pNumeroContrato);
                bancoDados.execute();

                return bancoDados.linhasAfetadas > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
            finally { bancoDados.Conexoes.close(); }
        }
        public System.Data.DataTable getParcelasQuitadas(DateTime pDataInicial, DateTime pDataFinal)
        {
            try
            {
                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "principal.Cobranca.pro_GetParcelaQuitadas";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = true;

                bancoDados.Comandos.adicionaParametro("@dt_ini", System.Data.SqlDbType.DateTime, 10, pDataInicial);
                bancoDados.Comandos.adicionaParametro("@dt_fim", System.Data.SqlDbType.DateTime, 10, pDataFinal);

                return bancoDados.execute().Tables[0];
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
            finally { bancoDados.Conexoes.close(); }
        }
        public System.Data.DataTable getContratosExcluidos()
        {
            try
            {
                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "principal.Cobranca.pro_GetContratosExcluidos";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = true;

                return bancoDados.execute().Tables[0];
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
            finally { bancoDados.Conexoes.close(); }
        }
        public string getLogCobranca(string pNumeroContrato)
        {
            try
            {
                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "principal.Cobranca.Proc_GetLogCobranca";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = true;

                bancoDados.Comandos.adicionaParametro("@nr_contrato", System.Data.SqlDbType.VarChar, 10, pNumeroContrato);

                return CarSystem.Utilidades.Rede.HTML.tableToJson(bancoDados.execute().Tables[0]);

            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
            finally { bancoDados.Conexoes.close(); }
        }
        public string setLogCobranca(string pNumeroContrato, string pCodigoCobrador, DateTime pDataContato, string pNomeUsuario, DateTime pDataLancamento, DateTime pDataProxLigacao, string pDescricaoAnalise, string pObservacao)
        {

            try
            {
                System.Web.Script.Serialization.JavaScriptSerializer jsonSerialiser = new System.Web.Script.Serialization.JavaScriptSerializer();

                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "Principal.Cobranca.pro_setLog";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = true;

                bancoDados.Comandos.comando.Parameters.AddWithValue("@pNumeroContrato", pNumeroContrato);
                bancoDados.Comandos.comando.Parameters.AddWithValue("@pCodigoCobrador", codigoCobradora); //pCodigoCobrador);
                bancoDados.Comandos.comando.Parameters.AddWithValue("@pDataContato", pDataContato);
                bancoDados.Comandos.comando.Parameters.AddWithValue("@pNomeUsuario", pNomeUsuario);
                bancoDados.Comandos.comando.Parameters.AddWithValue("@pDataLancamento", pDataLancamento);
                bancoDados.Comandos.comando.Parameters.AddWithValue("@pDataProxLigacao", pDataProxLigacao);
                bancoDados.Comandos.comando.Parameters.AddWithValue("@pDescricaoAnalise", pDescricaoAnalise);
                bancoDados.Comandos.comando.Parameters.AddWithValue("@pObservacao", pObservacao);

                System.Data.DataTable iTabela = bancoDados.execute().Tables[0];

                return CarSystem.Utilidades.Rede.HTML.tableToJson(iTabela);

            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
            finally { bancoDados.Conexoes.close(); }
        }

        public string setAlteraVencimento(string pCodigoParcela, double pValor, DateTime pVencimento)
        {
            System.Data.SqlClient.SqlTransaction iTransacao = bancoDados.Conexoes.conexao.BeginTransaction();

            try
            {
                if (pVencimento < DateTime.Today)
                    throw new Exception("O vencimento nao pode ser anterior a data atual");

                if (pVencimento > DateTime.Today.AddDays(15))
                    throw new Exception("O vencimento nao pode ser maior que 15 dias ");

                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "Principal.Cobranca.Proc_AlteraParcela";
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = true;

                bancoDados.Comandos.adicionaParametro("@CdParcela", System.Data.SqlDbType.BigInt, 4, pCodigoParcela);
                bancoDados.Comandos.adicionaParametro("@VlParcela", System.Data.SqlDbType.Money, 4, pValor);
                bancoDados.Comandos.adicionaParametro("@DtVencimento", System.Data.SqlDbType.DateTime, 8, pVencimento);
                bancoDados.Comandos.adicionaParametro("@CdCobrador ", System.Data.SqlDbType.VarChar, 6, codigoCobradora);


                bancoDados.Comandos.comando.Transaction = iTransacao;

                System.Data.DataTable iTabela = bancoDados.execute().Tables[0];

                if (Convert.ToBoolean(iTabela.Rows[0]["isEfetuada"]))
                {
                    iTransacao.Commit();
                    return CarSystem.Utilidades.Rede.HTML.tableToJson(iTabela);
                }

                throw new Exception(iTabela.Rows[0]["isEfetuada"].ToString());
            }
            catch (Exception ex)
            {
                if (iTransacao.Connection != null)
                    iTransacao.Rollback();

                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
            finally { bancoDados.Conexoes.close(); }
        }

        public string setRenovaCliente(string nContrato, double vlRenova, double vlTaxa, double vlGuincho, string nrAut, int nrPC, string cdBanco, string nrCob, int nrPcRen)
        {
            
            try
            {
                string retorno = "Não executou";
                System.Data.SqlClient.SqlTransaction pTransacao = null;
                System.Web.Script.Serialization.JavaScriptSerializer jsonSerialiser = new System.Web.Script.Serialization.JavaScriptSerializer();
 
                bancoDados.Comandos.limpaParametros();
                bancoDados.Comandos.textoComando = "Principal.Cobranca.pro_SetRenovaCliente"; 
                bancoDados.Comandos.tipoComando = System.Data.CommandType.StoredProcedure;
                bancoDados.retornaDados = true;

                bancoDados.Comandos.comando.Transaction = pTransacao;

                bancoDados.Comandos.adicionaParametro("@contrato", System.Data.SqlDbType.VarChar, 10, nContrato);
                bancoDados.Comandos.adicionaParametro("@vlRenova", System.Data.SqlDbType.Money, 4, vlRenova);
                bancoDados.Comandos.adicionaParametro("@vlTaxa", System.Data.SqlDbType.Money, 4, vlTaxa);
                bancoDados.Comandos.adicionaParametro("@vlGuincho", System.Data.SqlDbType.Money, 4, vlGuincho);
                bancoDados.Comandos.adicionaParametro("@nrAut", System.Data.SqlDbType.VarChar, 10, nrAut);
                bancoDados.Comandos.adicionaParametro("@nrPC", System.Data.SqlDbType.SmallInt, 2, nrPC);
                bancoDados.Comandos.adicionaParametro("@cdBanco", System.Data.SqlDbType.Char, 3, cdBanco);
                bancoDados.Comandos.adicionaParametro("@nrCob", System.Data.SqlDbType.VarChar, 6, nrCob);
                bancoDados.Comandos.adicionaParametro("@nrPcRen", System.Data.SqlDbType.SmallInt, 6, nrPcRen);

                retorno = bancoDados.execute().ToString();

               return retorno;

                //return CarSystem.Utilidades.Rede.HTML.tableToJson(iTabela);
            }
            catch (Exception ex)
            {
                throw new Exception("class:" + this.GetType().Name + "\r\n Method:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\r\n" + ex.Message);
            }
        }
    }
}
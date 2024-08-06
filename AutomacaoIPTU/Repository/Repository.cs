using AutomacaoIPTU.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AutomacaoIPTU.Repository
{
    public class Repository : IRepository
    {
        private readonly IRepository _repository;

        public int Nregistros { get; set; }
        public bool Boolregistros { get; set; }

        public Repository(IRepository repository)
        {
            _repository = repository;
        }

        public Repository()
        {
        }

        public void AlterarConsultados(Iptu Solicitacoes)
        {

            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Fila_Iptu set [status]='{(int)StatusEnum.Status.Processando}',Dt_Hora_Processamento='{DateTime.Now}' where Numero_Inscricao='{Solicitacoes.NumeroInscricao}'", conexao);
            try
            {
                conexao.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            conexao.Close();
            return;
        }


        public void AlterarSemCPF()
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Fila_Iptu set codigo_cidade='24',log_erro='Processo Sem CPF' where Codigo_Cidade='Sem CPF'", conexao);
            try
            {
                conexao.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            conexao.Close();
            return;
        }

        public int InserirTextoTabelaAndamento(Iptu Processo)
        {
            int NRegistros = 0;
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Fila_Andamento set Andamento='{Processo.Pagamento}',[STATUS]='{(int)StatusEnum.Status.Processado}',Dt_Ult_Acao='{DateTime.Now.ToString("dd/MM/yyyy")}',Dt_Proxima_Acao='{DateTime.Now.AddDays(Convert.ToDouble(Processo.Vencimento)).ToString("dd/MM/yyyy")}', Dt_Hora_Processamento='{DateTime.Now}' where Numero_Processo='{Processo.NumeroInscricao}'", conexao);
            conexao.Open();
            try
            {
                NRegistros = cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }

            return NRegistros;
        }

        public int InserirTextoTabelaFila(List<Iptu> Processo)
        {
            int NRegistros = 0;
            foreach (var Andamento in Processo)
            {
                SqlConnection conexao = ConexaoBanco();
                SqlCommand cmd = new($"insert into ImpostoUnicoAutomacao(Numero_Inscricao,Exercicio,Divida,Numero_Processo,Valor_Venal,Tipo,Total,Valor_Parcela,Numero_Parcela,Titular,status,Dt_Hora_Processamento) Values('{Andamento.NumeroInscricao}','{Andamento.Ano}','{Andamento.Total}','{Andamento.Processo}','{Andamento.VlrVenal}','{Andamento.Tipo}','{Andamento.Total}','{Andamento.SubTotal}','{Andamento.VlParcela}','{Andamento.Titular}','{Andamento.Status}','{DateTime.Now}')", connection: conexao);
                conexao.Open();
                try
                {
                    NRegistros = cmd.ExecuteNonQuery();
                    conexao.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return NRegistros;
        }

        public int InserirTextoTabelaFilaSuzaso(List<Iptu> Processo)
        {
            int NRegistros = 0;
            foreach (var Andamento in Processo)
            {
                SqlConnection conexao = ConexaoBanco();
                SqlCommand cmd = new($"insert into ImpostoUnicoAutomacao(Numero_Inscricao,Exercicio,Divida,Numero_Processo,Valor_Venal,Tipo,Total,Valor_Parcela,Numero_Parcela,Titular,status,Dt_Hora_Processamento) Values('{Andamento.NumeroInscricao}','{Andamento.Ano}','{Andamento.Total}','{Andamento.Processo}','{Andamento.VlrVenal}','IPTU','{Andamento.Total}','{Andamento.SubTotal}','{Andamento.VlParcela}','{Andamento.Titular}','{Andamento.Status}','{DateTime.Now}')", connection: conexao);
                conexao.Open();
                try
                {
                    NRegistros = cmd.ExecuteNonQuery();
                    conexao.Close();
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return NRegistros;
        }

        public int EnviarAvisoSaldo()
        {
            int NRegistros = 0;

            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"insert into Pendencias(usuario,data,titulo,pendencia,setor) Values('Forkel','{DateTime.Now}','SaldoAutomação','Saldo da conta no 2Captcha esta em R$5','TI')", connection: conexao);
            conexao.Open();
            try
            {
                NRegistros = cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            return NRegistros;
        }

        public bool ConsultarAvisoSaldo()
        {
            bool NRegistros = true;
            int cont = 0;
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"select count(usuario) From Pendencias where titulo='SaldoAutomação' and pendencia='Saldo da conta no 2Captcha esta em R$5' and setor='TI'", connection: conexao);
            conexao.Open();
            try
            {
                cont = Convert.ToInt32(cmd.ExecuteScalar());
                if (cont == 1)
                    NRegistros = false;
                conexao.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            return NRegistros;
        }



        public int InserirCPF(string? NInscricao, string? CPF)
        {
            int NRegistros = 0;

            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Fila_IPTU   set CPF='{CPF}' where  Numero_Inscricao='{NInscricao}'", conexao);
            conexao.Open();
            try
            {
                NRegistros = cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            return NRegistros;
        }

        public int InserirSemDados(string? NInscricao)
        {
            int NRegistros = 0;

            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Fila_IPTU   set Log_Erro='Processo sem Dados', Dt_Hora_Processamento='{DateTime.Now}',Dt_Proxima_Acao='{DateTime.Now.AddDays(30):dd/MM/yyyy}', status='{(int)StatusEnum.Status.SemDados}' where  Numero_Inscricao='{NInscricao}'", conexao);
            conexao.Open();
            try
            {
                NRegistros = cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            return NRegistros;
        }

        public int InserirValorVenal(Iptu Processo)
        {
            int NRegistros = 0;

            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"insert into ImpostoUnicoAutomacao(Numero_Inscricao,Exercicio,Valor_Venal,Titular,Dt_Hora_Processamento) Values('{Processo.NumeroInscricao.Trim()}','2024','{Processo.VlrVenal}','{Processo.Titular}','{DateTime.Now}')", connection: conexao);
            conexao.Open();
            try
            {
                NRegistros = cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception ex)
            {
                throw;
            }

            return NRegistros;
        }

        public int AtualizarTextoTabela(Iptu Processo)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Controle_Andamentos set Andamento_Data='{DateTime.Now.AddMinutes(1).ToString("dd/MM/yyyy")}' where Andamento_Num='{Processo.NumeroInscricao}'", conexao);
            conexao.Open();
            try
            {
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return 0;
        }

        public void AlimentcarFilaSeguinte()
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Fila_IPTU set [STATUS]='1',log_Erro='',Tentativas='0' where [STATUS] in('3','4') and Dt_Proxima_Acao<='{DateTime.Now.ToString("dd/MM/yyyy")}'", connection: conexao);
            conexao.Open();
            try
            {
                cmd.ExecuteNonQuery();
                conexao.Close();
                DeletarDuplicada();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void AlimentarFila()
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new("insert into Fila_IPTU(Numero_Inscricao,Cidade,Codigo_Lote,Status_Lote,Dt_Proxima_Acao,Dt_Ult_Acao,Vencimento,Completo,tentativas,[Status],Codigo_Loteamento_Loteador) select  A.insc_cad,A.cidade,B.cd_lote,C.status,GETDATE(),GETDATE(),'30',0,'0','1',A.cd_loteamento from insc_cadast as A inner join lote_vinculado as B on B.inscricao=A.insc_cad inner join lotes as C on codigo=B.cd_lote  where A.insc_cad not in (select distinct Numero_Inscricao from Fila_IPTU) and cidade in ('Guarulhos','Itaquaquecetuba','Suzano') and C.status not in('Patrimônio','Desapropriado','Doado','Registrado')", conexao);
            try
            {
                conexao.Open();
                cmd.ExecuteNonQuery();

            }
            catch (Exception)
            {
                throw;
            }
            conexao.Close();
            return;
        }

        public void ArrumarDatas()
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new("UPDATE Fila_IPTU SET Dt_Proxima_Acao = DATEADD(MONTH, 1, GETDATE()) WHERE [Status] = '3'", conexao);
            try
            {
                conexao.Open();
                cmd.ExecuteNonQuery();

            }
            catch (Exception)
            {
                throw;
            }
            conexao.Close();
            return;
        }
        public List<Iptu> RetornaSolicitacoesAtrasadas()
        {
            List<Iptu> Solicitacoes = new();
            for (int NCid = 14; NCid > 0; NCid--)
            {
                SqlConnection conexao = ConexaoBanco();
                SqlCommand cmd = new($"select top 50 * From Fila_Iptu where codigo_cidade='{NCid}' and [status]='{(int)StatusEnum.Status.Processando}'  Order By Dt_Ult_Acao", conexao);
                try
                {
                    conexao.Open();
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Iptu Lersolicitacao = new()
                        {
                            NumeroInscricao = reader[1].ToString(),
                            Cidade = reader[2].ToString(),
                            CodigoLote = reader[3].ToString(),
                            Processo = reader[4].ToString(),
                            StatusLote = reader[5].ToString(),
                            Cpf = reader[7].ToString(),
                            CodigoCidade = reader[8].ToString(),
                            Vencimento = reader[9].ToString(),
                            CodigoLoteamentoLoteador = reader[16].ToString(),

                        };

                        Solicitacoes.Add(Lersolicitacao);
                    }
                    conexao.Close();
                    if (Solicitacoes.Count > 0)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return Solicitacoes;
        }

        public static void CorrigirProcesso(string New, string Old)
        {
            SqlConnection conexao = ConexaoBanco();
            conexao.Open();
            SqlCommand cmd = new($"update ImpostoUnicoAutomacao set Numero_Processo='{New}' where Numero_Processo='{Old}'", conexao);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            conexao.Close();
        }

        public void AlterarProcessando()
        {
            SqlConnection conexao = ConexaoBanco();
            conexao.Open();
            SqlCommand cmd = new($"update Fila_IPTU set status='{(int)StatusEnum.Status.Pendente}' where status='{(int)StatusEnum.Status.Processando}'", conexao);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            conexao.Close();
        }

        public void RetirarFilaNaoEncontrado(string? NInscricao)
        {
            SqlConnection conexao = ConexaoBanco();
            conexao.Open();
            SqlCommand cmd = new($"update Fila_IPTU   set Log_Erro='Nao Encontrado' ,Dt_Hora_Processamento='{DateTime.Now}',Dt_Proxima_Acao='{DateTime.Now.AddDays(30):dd/MM/yyyy}', status='{(int)StatusEnum.Status.Erro}' where  Numero_Inscricao='{NInscricao}'", conexao);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            conexao.Close();
        }

        public void RetirarFilaInscricaoErrada(string? NInscricao)
        {
            SqlConnection conexao = ConexaoBanco();
            conexao.Open();
            SqlCommand cmd = new($"update Fila_IPTU   set Log_Erro='Numero de Inscrisao desformatado' ,Dt_Hora_Processamento='{DateTime.Now}',Dt_Proxima_Acao='{DateTime.Now.AddDays(30):dd/MM/yyyy}', status='{(int)StatusEnum.Status.Desformatado}' where  Numero_Inscricao='{NInscricao}'", conexao);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            conexao.Close();
        }

        public void RetirarFilaSemCPF(string? NInscricao)
        {
            SqlConnection conexao = ConexaoBanco();
            conexao.Open();
            SqlCommand cmd = new($"update Fila_IPTU   set Log_Erro='Processo sem CPF' ,Dt_Hora_Processamento='{DateTime.Now}',Dt_Proxima_Acao='{DateTime.Now.AddDays(30):dd/MM/yyyy}', status='{(int)StatusEnum.Status.Erro}' where  Numero_Inscricao='{NInscricao}'", conexao);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            conexao.Close();
        }

        public void Processado(string NInscricao)
        {
            SqlConnection conexao = ConexaoBanco();
            conexao.Open();
            SqlCommand cmd = new($"update Fila_IPTU   set Dt_Hora_Processamento='{DateTime.Now}',Dt_Proxima_Acao='{DateTime.Now.AddDays(30):dd/MM/yyyy}', status='{(int)StatusEnum.Status.Processado}' where status='{(int)StatusEnum.Status.Processando}' and Numero_Inscricao='{NInscricao}'", conexao);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            conexao.Close();
        }

        public void InserirOrgTabela(string municipio, string? numeroInscricao)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update  Fila_Iptu set Codigo_Cidade='{municipio}' where Numero_Inscricao='{numeroInscricao}'", conexao);
            conexao.Open();
            try
            {
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }

        }


        public void InserirValorVenal(string? numeroInscricao, string? ValorVenal)
        {
            try
            {
                SqlConnection conexao = ConexaoBanco();
                SqlCommand cmd = new($"update  ImpostoUnicoAutomacao set Valor_Venal='{ValorVenal}',dt_hora_processamento='{DateTime.Now}' where Numero_Inscricao='{numeroInscricao.Trim()}' ", conexao);
                conexao.Open();
                cmd.ExecuteNonQuery();
                conexao.Close();

            }
            catch (Exception)
            {
                SqlConnection conexao = ConexaoBanco();
                SqlCommand cmd = new($"insert into  ImpostoUnicoAutomacao (Numero_Inscricao,Valor_Venal,dt_hora_processamento) Values('{numeroInscricao.Trim()}','{ValorVenal}','{DateTime.Now.Year}')", conexao);
                conexao.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                    conexao.Close();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public void InserirValorVenalProp(string? numeroInscricao, string Titular, string? ValorVenal)
        {
            try
            {
                SqlConnection conexao = ConexaoBanco();
                SqlCommand cmd = new($"update  ImpostoUnicoAutomacao set Valor_Venal='{ValorVenal}',Titular='{Titular}',dt_hora_processamento='{DateTime.Now}' where Numero_Inscricao='{numeroInscricao.Trim()}' ", conexao);
                conexao.Open();
                cmd.ExecuteNonQuery();
                conexao.Close();

            }
            catch (Exception)
            {
                SqlConnection conexao = ConexaoBanco();
                SqlCommand cmd = new($"insert into  ImpostoUnicoAutomacao (Numero_Inscricao,Valor_Venal,dt_hora_processamento) Values('{numeroInscricao.Trim()}','{ValorVenal}','{DateTime.Now.Year}')", conexao);
                conexao.Open();
                try
                {
                    cmd.ExecuteNonQuery();
                    conexao.Close();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public static List<Iptu> RetornaSolicitacoesOrg()
        {
            List<Iptu> Solicitacoes = new();

            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"select * From Fila_IPTU where Codigo_cidade is null and status='{(int)StatusEnum.Status.Pendente}'", conexao);
            try
            {
                conexao.Open();
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Iptu Lersolicitacao = new()
                    {
                        NumeroInscricao = reader[1].ToString(),
                        Cidade = reader[2].ToString(),
                        CodigoLote = reader[3].ToString(),
                        Processo = reader[4].ToString(),
                        StatusLote = reader[5].ToString(),
                        Cpf = reader[7].ToString(),
                        CodigoCidade = reader[8].ToString(),
                        Vencimento = reader[9].ToString(),
                        CodigoLoteamentoLoteador = reader[16].ToString(),
                    };

                    Solicitacoes.Add(Lersolicitacao);

                }
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return Solicitacoes;
        }

        public static List<Iptu> RetornaSolicitacoesAll()
        {
            List<Iptu> Solicitacoes = new();

            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"select  Numero_Processo From ImpostoUnicoAutomacao where Numero_Processo is not null and Numero_Processo!=''", conexao);
            try
            {
                conexao.Open();
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Iptu Lersolicitacao = new()
                    {
                        Processo = reader[0].ToString(),
                    };
                    Solicitacoes.Add(Lersolicitacao);

                }
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return Solicitacoes;
        }

        public int GravarLogErro(Iptu Solicitacoes, string numProc = null)
        {
            SqlConnection conexao = ConexaoBanco();
            int tamanho = 0;
            if (Solicitacoes.Log_Erro == null)
                Solicitacoes.Log_Erro = " ";
            else
                tamanho = Solicitacoes.Log_Erro.Length;

            if (tamanho > 200)
            {
                tamanho = 200;
            }
            Solicitacoes.Log_Erro = Solicitacoes.Log_Erro.Replace("'", " ");
            SqlCommand cmd = new();
            if (numProc == null)
                cmd = new($"update Fila_Iptu set [status]=4, Log_Erro='{Solicitacoes.Log_Erro.Substring(0, tamanho)}' where numero_inscricao='{Solicitacoes.NumeroInscricao}'", conexao);
            else
                cmd = new($"update  Fila_Iptu set [status]=4, Log_Erro='{Solicitacoes.Log_Erro.Substring(0, tamanho)}' where numero_inscricao='{numProc}'", conexao);

            conexao.Open();
            try
            {

                cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return (int)Nregistros;
        }

        public List<Iptu> RetornaSolicitacoes()
        {
            int NCid = 0;
            var dia = DateTime.Now.DayOfWeek;
            if (dia.ToString().Contains("Monday") || dia.ToString().Contains("Thursday"))
                NCid = 13;
            else if (dia.ToString().Contains("Tuesday") || dia.ToString().Contains("Friday"))
                NCid = 24;
            else
                NCid = 14;

            List<Iptu> Solicitacoes = new();
            for (NCid = NCid; NCid > 0; NCid++)
            {
                SqlConnection conexao = ConexaoBanco();
                SqlCommand cmd = new($"select top 3 * From Fila_Iptu where codigo_cidade='{NCid}' and [status]='{(int)StatusEnum.Status.Pendente}'  Order By Dt_Hora_Processamento", conexao);
                try
                {
                    conexao.Open();
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Iptu Lersolicitacao = new()
                        {
                            NumeroInscricao = reader[1].ToString(),
                            Cidade = reader[2].ToString(),
                            CodigoLote = reader[3].ToString(),
                            Processo = reader[4].ToString(),
                            StatusLote = reader[5].ToString(),
                            Cpf = reader[7].ToString(),
                            CodigoCidade = reader[8].ToString(),
                            Vencimento = reader[9].ToString(),
                            CodigoLoteamentoLoteador = reader[16].ToString(),

                        };
                        Solicitacoes.Add(Lersolicitacao);
                    }
                    conexao.Close();
                    if (Solicitacoes.Count > 0)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return Solicitacoes;
        }

        public string RetornaVinculado(string Busca)
        {
            string? retorno = null;
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"select CIC from lote_vinculado where inscricao='{Busca}'", conexao);
            try
            {
                conexao.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    retorno = reader[0].ToString();
                }
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }

            return retorno;
        }

        public List<string?> RetornaCPF(string Busca)
        {
            List<string?> retorno = new List<string?>();
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"select Loteamento From PARTICIPACAO where receita_incorporador<>0 and Loteador='{Busca}'", conexao);
            try
            {
                conexao.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    retorno.Add(reader[0].ToString());
                }
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }

            return retorno;
        }

        public List<string> RetornaCPFVinculado(string Busca)
        {
            List<string?> retorno = new List<string?>();
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"select CIC from prestamista where codigo='{Busca}'", conexao);
            try
            {
                conexao.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    retorno.Add(reader[0].ToString());
                }
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }

            return retorno;
        }

        public bool RetornaExcluido(string Busca)
        {
            bool retorno = true;
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"select Excluir from Loteamento where cd_Loteamento='{Busca}'", conexao);
            try
            {
                conexao.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    retorno = (bool)reader[0];
                }
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }

            return retorno;
        }

        public string RetornaCPFPorLoteador(string? Busca)
        {
            string retorno = "";
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"select CIC From loteador where CD_LOTEADOR='{Busca}'", conexao);
            try
            {
                conexao.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!string.IsNullOrEmpty(reader[0].ToString()))
                        retorno = reader[0].ToString().Trim();
                }
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }

            return retorno;
        }


        public static SqlConnection ConexaoBanco()
        {
            SqlConnectionStringBuilder builder = new()
            {
                //DataSource = "VC0003\\SQLEXPRESS01",
                DataSource = "MEDWS3",
                InitialCatalog = "Jzamf_loteador6",
                UserID = "loteador",
                TrustServerCertificate = true,
                Password = "mediterraneo"
            };
            string conexao = builder.ConnectionString;
            SqlConnection conexaoBanco = new(conexao);
            return conexaoBanco;
        }
    }
}

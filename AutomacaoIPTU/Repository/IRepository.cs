using AutomacaoIPTU.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomacaoIPTU.Repository
{
    public interface IRepository
    {
        int Nregistros { get; set; }
        bool Boolregistros { get; set; }

        public void AlterarConsultados(Iptu Solicitacoes)
        {

        }

        public void AlterarSemCPF()
        {

        }

        public int InserirTextoTabelaAndamento(Iptu Processo)
        {
            return Nregistros;
        }

        public int InserirTextoTabelaFila(List<Iptu> Processo)
        {
            return Nregistros;
        }

        public int InserirTextoTabelaFilaSuzaso(List<Iptu> Processo)
        {
            return Nregistros;
        }

        public int EnviarAvisoSaldo()
        {
            return Nregistros;
        }

        public bool ConsultarAvisoSaldo()
        {
            return Boolregistros;
        }

        public int InserirCPF(string? NInscricao, string? CPF)
        {
            return Nregistros;
        }

        public int InserirSemDados(string? NInscricao)
        {
            return Nregistros;
        }

        public int InserirValorVenal(Iptu Processo)
        {
            return Nregistros;
        }

        public int AtualizarTextoTabela(Iptu Processo)
        {
            return Nregistros;
        }

        public void AlimentarFila()
        {

        }

        public List<Iptu> RetornaSolicitacoesAtrasadas()
        {
            List<Iptu> Retorno = new List<Iptu>();
            return Retorno;
        }

        public static void CorrigirProcesso(string New, string Old)
        {

        }

        public void RetirarFilaSemCPF(string? NInscricao)
        {

        }

        public void Processado(string NInscricao)
        {

        }

        public void InserirOrgTabela(string municipio, string? numeroInscricao)
        {


        }

        public void InserirValorVenal(string? numeroInscricao, string? ValorVenal)
        {

        }

        public void InserirValorVenalProp(string? numeroInscricao, string Titular, string? ValorVenal)
        {

        }

        public static List<Iptu> RetornaSolicitacoesOrg()
        {
            List<Iptu> Retorno = new List<Iptu>();
            return Retorno;
        }

        public static List<Iptu> RetornaSolicitacoesAll()
        {
            List<Iptu> Retorno = new List<Iptu>();
            return Retorno;
        }

        public int GravarLogErro(Iptu Solicitacoes, string numProc = null)
        {
            return Nregistros;
        }

        public List<Iptu> RetornaSolicitacoes()
        {
            List<Iptu> Retorno = new List<Iptu>();
            return Retorno;
        }

        public string RetornaVinculado(string Busca)
        {
            string Retorno = " ";
            return Retorno;
        }

        public List<string> RetornaCPF(string Busca)
        {
            List<string> Retorno = new List<string>();
            return Retorno;
        }

        public List<string> RetornaCPFVinculado(string Busca)
        {
            List<string> Retorno = new List<string>();
            return Retorno;
        }

        public string RetornaCPFPorLoteador(string? Busca)
        {
            string Retorno = " ";
            return Retorno;
        }

        public static SqlConnection ConexaoBanco()
        {
            SqlConnection conexaoBanco = new();
            return conexaoBanco;
        }
    }
}


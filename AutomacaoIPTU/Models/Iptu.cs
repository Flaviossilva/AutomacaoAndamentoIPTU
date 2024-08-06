using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomacaoIPTU.Models
{
    public class Iptu
    {
        public string? NumeroInscricao { get; set; }
        public string? Ano { get; set; }
        public string? Log_Erro { get; set; }
        public string? Vencimento { get; set; }
        public string? Cpf { get; set; }
        public string? Pagavel { get; set; }
        public string? Pagamento { get; set; }
        public string? Tipo { get; set; }
        public string? Recibo { get; set; }
        public string? VlPrincipal { get; set; }
        public string? VlParcela { get; set; }
        public string? VlPago { get; set; }
        public string? Original { get; set; }
        public string? Correcao { get; set; }
        public string? Multa { get; set; }
        public string? Juros { get; set; }
        public string? Honorarios { get; set; }
        public string? SubTotal { get; set; }
        public string? Custas { get; set; }
        public string? Total { get; set; }
        public string? Processo { get; set; }
        public string? CDA { get; set; }
        public string? Desconto { get; set; }
        public string? Titular { get; set; }
        public int? Municipio { get; set; }
        public string? Status { get; set; }
        public string? StatusLote { get; set; }
        public string? Cidade { get; set; }
        public string? VlrVenal { get; set; }
        public string? CodigoLote { get; set; }
        public string? CodigoLoteamentoLoteador { get; set; }
        public string? CodigoCidade { get; set; }

    }
}

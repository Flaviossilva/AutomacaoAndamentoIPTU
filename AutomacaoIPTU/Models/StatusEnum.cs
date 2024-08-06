using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomacaoIPTU.Models
{
    internal class StatusEnum
    {
        public enum Status
        {
            Pendente = 1,
            Processando = 2,
            Processado = 3,
            Erro = 4,
            Desativado = 10,
            SemDados = 14,
            Desformatado = 50,
        };
    }
}

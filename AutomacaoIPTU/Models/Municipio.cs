using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomacaoIPTU.Models
{
    internal enum Municipio
    {
        [MunicipioInfoAttribute("São Paulo", 1)]
        SAO,

        [MunicipioInfoAttribute("Itaquaquecetuba", 24)]
        ITA,

        [MunicipioInfoAttribute("Bom", 2)]
        BOM,

        [MunicipioInfoAttribute("Igaratá", 3)]
        IGA,

        [MunicipioInfoAttribute("Sumaré", 4)]
        SML,

        [MunicipioInfoAttribute("Taboão da Serra", 5)]
        TAB,

        [MunicipioInfoAttribute("Atibaia", 6)]
        ATI,

        [MunicipioInfoAttribute("Campinas", 7)]
        CAM,

        [MunicipioInfoAttribute("Franca", 8)]
        FRA,

        [MunicipioInfoAttribute("Botucatu", 9)]
        BOJ,

        [MunicipioInfoAttribute("Poá", 10)]
        POA,

        [MunicipioInfoAttribute("Sorocaba", 11)]
        SOR,

        [MunicipioInfoAttribute("Itatiba", 12)]
        ITT,

        [MunicipioInfoAttribute("Suzano", 13)]
        SUZ,

        [MunicipioInfoAttribute("Guarulhos", 14)]
        GUA,

        [MunicipioInfoAttribute("Lorena", 15)]
        LOR,

        [MunicipioInfoAttribute("Fernandópolis", 16)]
        FMO,

        [MunicipioInfoAttribute("Ferraz de Vasconcelos", 17)]
        FER,

        [MunicipioInfoAttribute("Mairinque", 18)]
        MAI,

        [MunicipioInfoAttribute("Bertioga", 19)]
        BER,

        [MunicipioInfoAttribute("Avaré", 20)]
        AVA,

        [MunicipioInfoAttribute("Mogi das Cruzes", 21)]
        MOG,

        [MunicipioInfoAttribute("Resende", 22)]
        RES,

        [MunicipioInfoAttribute("Arujá", 23)]
        ARU,
    }

    // Atributo personalizado para armazenar informações sobre o município
    internal class MunicipioInfoAttribute : Attribute
    {
        public string Nome { get; }
        public int Codigo { get; }


        public MunicipioInfoAttribute(string nome, int codigo)
        {
            Nome = nome;
            Codigo = codigo;
        }

      
    }
}

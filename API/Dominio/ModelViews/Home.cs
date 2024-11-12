using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minimal_API.Dominio.ModelViews
{

    public struct Home
    {
        public String Mensagem { get => "Bem Vindo a API de Veiculos"; }   
        public String Doc { get => "/swagger"; }   
    }

}
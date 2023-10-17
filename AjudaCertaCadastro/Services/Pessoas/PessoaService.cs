using AjudaCertaCadastro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjudaCertaCadastro.Services.Pessoas
{
    public class PessoaService : Request
    {
        private readonly Request _request;
        private const string apiUrlBase = "http://fuscatcc.somee.com/ApiAjudaCerta/Pessoas";

        private string _token;
        public PessoaService(string token)
        {
            _request = new Request();
            _token = token;
        }

        public async Task<Pessoa> PostRegistrarPessoaAsync(Pessoa p)
        {
            string urlComplementar = "/Registrar";
            p.Id = await _request.PostReturnIntAsync(apiUrlBase + urlComplementar, p, _token);

            return p;
        }


    }
}

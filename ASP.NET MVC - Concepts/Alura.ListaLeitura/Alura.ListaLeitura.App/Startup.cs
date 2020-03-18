using Alura.ListaLeitura.App.Repositorio;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.App
{
    public class Startup
    {

        public void Configure(IApplicationBuilder app)
        {//Configurar a sequencia "Chegou requisicao e executa o tal método
            //Fluxo de request-response
            /*
             * Request Pipeline: Termo usado pelo ASP.NET Core para representar o fluxo que uma requisição HTTP percorre dentro de sua aplicação até que a resposta seja entregue ao cliente.
             */
            //IAapplicationBuilder realiza a configuração do Request Pipeline de requisicao para a aplicação
            app.Run(LivrosParaLer);
            //O método Run, precisa de um retorno do tipo RequestDelegate
            //Um requestDelegate é um metodo que tem como retorno tipos Task
        }

        public Task LivrosParaLer(HttpContext context)
        {//Codigo executado quando chegar uma requisicao, recebe um obj com todas as informações referentes a aquela request
            //Toda informacao encapsulada em uma request especifica fica encapsulada em objs. do tipo HttpContext

            var _repo = new LivroRepositorioCSV();

            //Response é a property do httpcontext que vem como resposta, e escrevemos nossa lista.
            return context.Response.WriteAsync(_repo.ParaLer.ToString());

            


        }
    }
}
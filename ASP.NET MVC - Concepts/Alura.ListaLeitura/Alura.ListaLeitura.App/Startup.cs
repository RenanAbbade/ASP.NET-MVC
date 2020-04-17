using Alura.ListaLeitura.App.Negocio;
using Alura.ListaLeitura.App.Repositorio;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.App
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();//Significa que minha app está usando o serviço de roteamento do ASP.NET CORE
        }

        public void Configure(IApplicationBuilder app)
        {//Configurar a sequencia "Chegou requisicao e executa o tal método
         //Fluxo de request-response
         /*
          * Request Pipeline: Termo usado pelo ASP.NET Core para representar o fluxo que uma requisição HTTP percorre dentro de sua aplicação até que a resposta seja entregue ao cliente.
          */
         //IAapplicationBuilder realiza a configuração do Request Pipeline de requisicao para a aplicação

            var builder = new RouteBuilder(app);//Classe responsavel pela construção das rotas no ASP.NET CORE

            builder.MapRoute("Livros/ParaLer",LivrosParaLer);//Para cada rota que queremos atender, usamos o MapRoute
            builder.MapRoute("Livros/Lendo", LivrosLendo);
            builder.MapRoute("Livros/Lido", LivrosLidos);
            builder.MapRoute("Cadastro/NovoLivro/{nome}/{autor}", NovoLivroParaler);
            builder.MapRoute("Livros/Detalhes/{id:int}", ExibeDetalhes);//Inserindo restrição Constraint para ir para o metodo somente quanto id for int
            builder.MapRoute("Cadastro/NovoLivro", ExibeFormulario);
            builder.MapRoute("Cadastro/Incluir", ProcessaFormulario);
            var rotas = builder.Build();//Construção das rotas

            app.UseRouter(rotas);

            //app.Run(Roteamento);
            //O método Run, precisa de um retorno do tipo RequestDelegate
            //Um requestDelegate é um metodo que tem como retorno tipos Task
        }

        private Task ProcessaFormulario(HttpContext context)
        {
            var livro = new Livro()
            {
                Titulo = context.Request.Form["titulo"].First(),
                Autor = context.Request.Form["nome"].First(),
            };

            var repo = new LivroRepositorioCSV();
            repo.Incluir(livro);
            return context.Response.WriteAsync("O livro foi adicionado com sucesso");

        }

        private string CarregaArquivoHTML(string nomeArquivo)
        {//C:\Users\Dell\Documents\GitHub\ASP.NET MVC\ASP.NET-MVC\ASP.NET MVC - Concepts\Alura.ListaLeitura\Alura.ListaLeitura.App\HTML\formulario.html
            string nomeCompletoArquivo = $"C:/Users/Dell/Documents/GitHub/ASP.NET MVC/ASP.NET-MVC/ASP.NET MVC - Concepts/Alura.ListaLeitura/Alura.ListaLeitura.App/HTML/{nomeArquivo}.html";
            using (var arquivo = File.OpenText(nomeCompletoArquivo))
            {
                return arquivo.ReadToEnd();
            }
        }

        private Task ExibeFormulario(HttpContext context)
        {
            var html = CarregaArquivoHTML("formulario");
            return context.Response.WriteAsync(html);
        }

        private Task ExibeDetalhes(HttpContext context)
        {
            int id = Convert.ToInt32(context.GetRouteValue("id"));
            var repo = new LivroRepositorioCSV();
            var livro = repo.Todos.First(l => l.Id == id);
            return context.Response.WriteAsync(livro.Detalhes());
        }

        private Task NovoLivroParaler(HttpContext context)
        {
            var livro = new Livro()
            {
                Titulo = context.GetRouteValue("nome").ToString(),//Acessa valor passado no {nome} no rota do contexto
                Autor = context.GetRouteValue("autor").ToString(),//Acessa valor passado no {autor} no rota do contexto
            };

            var repo = new LivroRepositorioCSV();
            repo.Incluir(livro);
            return context.Response.WriteAsync("O livro foi adicionado com sucesso");
        }

        public Task Roteamento(HttpContext context)
        {

            var _repo = new LivroRepositorioCSV();

            var caminhosAtendidos = new Dictionary<string, RequestDelegate>
            {
             //Settando elementos chave/valor do dicionario, que se tornaram parte do roteamento     
                {"/Livros/ParaLer", LivrosParaLer },
                {"/Livros/Lendo", LivrosLendo},
                {"/Livros/Lido", LivrosLidos}
            };
            //Verifica se o context.Request.Path é igual a uma das chaves do dicionario, se for acessa a url
            if (caminhosAtendidos.ContainsKey(context.Request.Path)){

                var metodo = caminhosAtendidos[context.Request.Path];
                //Para invocar um metodo do tipo RequestDelegate, utiliza-se o Invoke
                return metodo.Invoke(context);
            }

            context.Response.StatusCode = 404;//Caso retorne o endereço invalido, o resultado do status request deve ser 404 de error:not found.
            return context.Response.WriteAsync("Endereço invalido."); //Se o caminho do context não estiver como key do Dictionary, envio "caminho inexistente"
        }

        public Task LivrosParaLer(HttpContext context)
        {//Codigo executado quando chegar uma requisicao, recebe um obj com todas as informações referentes a aquela request
            //Toda informacao encapsulada em uma request especifica fica encapsulada em objs. do tipo HttpContext
            var conteudoArquivo = CarregaArquivoHTML("para-ler");

            var _repo = new LivroRepositorioCSV();

            foreach(var livro in _repo.ParaLer.Livros)
            {
                conteudoArquivo = conteudoArquivo.Replace("#NOVO-ITEM", $"<li>{livro.Titulo} - {livro.Autor}</li>#NOVO-ITEM");
            }

            return context.Response.WriteAsync(conteudoArquivo);

        }

        public Task LivrosLendo(HttpContext context)
        {//Codigo executado quando chegar uma requisicao, recebe um obj com todas as informações referentes a aquela request
            //Toda informacao encapsulada em uma request especifica fica encapsulada em objs. do tipo HttpContext

            var _repo = new LivroRepositorioCSV();

            //Response é a property do httpcontext que vem como resposta, e escrevemos nossa lista.
            return context.Response.WriteAsync(_repo.Lendo.ToString());

        }

        public Task LivrosLidos(HttpContext context)
        {//Codigo executado quando chegar uma requisicao, recebe um obj com todas as informações referentes a aquela request
            //Toda informacao encapsulada em uma request especifica fica encapsulada em objs. do tipo HttpContext

            var _repo = new LivroRepositorioCSV();

            //Response é a property do httpcontext que vem como resposta, e escrevemos nossa lista.
            return context.Response.WriteAsync(_repo.Lidos.ToString());

        }
    }
}
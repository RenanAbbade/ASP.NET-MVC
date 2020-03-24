using Alura.ListaLeitura.App.Negocio;
using Alura.ListaLeitura.App.Repositorio;
using Microsoft.AspNetCore.Hosting;
using System;

namespace Alura.ListaLeitura.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var _repo = new LivroRepositorioCSV();

            //Classe de hospedagem do ASP.NET CORE
            IWebHost host = new WebHostBuilder()
                .UseKestrel() //Esse metodo diz qual será a implementação do servidor HTTP que iremos utilizar, sendo está o Kestrel
                .UseStartup<Startup>()//Classe que inicializa o host
                .Build();//Classe WebHostBuilder é responsavel por construir um servidor WEB, porém é uma interface, sendo necessário o método .Build() para implementa-la.
            //O host necessita de configurações para ser executado.

            //Para subir o hospedeiro para o mesmo ficar disponivel para receber as chamadas web.
            host.Run();

            ImprimeLista(_repo.ParaLer);
            ImprimeLista(_repo.Lendo);
            ImprimeLista(_repo.Lidos);
        }

        static void ImprimeLista(ListaDeLeitura lista)
        {
            Console.WriteLine(lista);
        }
    }
}

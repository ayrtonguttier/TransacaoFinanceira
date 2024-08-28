using TransacaoFinanceira.Conta.Abstractions;
using TransacaoFinanceira.Abstractions;
using TransacaoFinanceira.Console.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace TransacaoFinanceira.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Information);
            });

            services.AddScoped<IContaRepository>(_ =>
            {
                return InicializarRepository();
            });
            services.AddScoped<TransacaoExecutor>();


            var provider = services.BuildServiceProvider();


            using (var scope = provider.CreateScope())
            {

                var executor = scope.ServiceProvider.GetRequiredService<TransacaoExecutor>();
                var transacoes = new Transacao[] {
                    new Transacao(1,"09/09/2023 14:15:00", 938485762, 2147483649, 150),
                    new Transacao(2,"09/09/2023 14:15:05", 2147483649, 210385733, 149),
                    new Transacao(3,"09/09/2023 14:15:29", 347586970, 238596054, 1100),
                    new Transacao(4,"09/09/2023 14:17:00", 675869708, 210385733, 5300),
                    new Transacao(5,"09/09/2023 14:18:00", 238596054, 674038564, 1489),
                    new Transacao(6,"09/09/2023 14:18:20", 573659065, 563856300, 49),
                    new Transacao(7,"09/09/2023 14:19:00", 938485762, 2147483649, 44),
                    new Transacao(8,"09/09/2023 14:19:01", 573659065, 675869708, 150),
                };

                foreach (var transacao in transacoes)
                {
                    executor.ExecutarTransacao(transacao);
                }
            }

            System.Console.ReadKey();
        }


        //Apenas para exemplo
        static IContaRepository InicializarRepository()
        {
            var repo = new ContaRepository();
            repo.AtualizarSaldo(new ContaSaldo(938485762, 180));
            repo.AtualizarSaldo(new ContaSaldo(347586970, 1200));
            repo.AtualizarSaldo(new ContaSaldo(2147483649, 0));
            repo.AtualizarSaldo(new ContaSaldo(675869708, 4900));
            repo.AtualizarSaldo(new ContaSaldo(238596054, 478));
            repo.AtualizarSaldo(new ContaSaldo(573659065, 787));
            repo.AtualizarSaldo(new ContaSaldo(210385733, 10));
            repo.AtualizarSaldo(new ContaSaldo(674038564, 400));
            repo.AtualizarSaldo(new ContaSaldo(563856300, 1200));

            return repo;
        }

    }
}

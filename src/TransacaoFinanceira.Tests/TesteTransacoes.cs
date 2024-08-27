using FluentAssertions;
using TransacaoFinanceira.Abstractions;
using TransacaoFinanceira.Conta.Abstractions;
using Microsoft.Extensions.Logging.Testing;
using Xunit.Abstractions;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace TransacaoFinanceira.Tests;

public class TesteTransacoes
{
    ITestOutputHelper output;

    public TesteTransacoes(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Fact]
    public void TransacaoEntreContas()
    {
        //arrange
        var culture = CultureInfo.GetCultureInfo("pr-BR");
        ILogger<TransacaoExecutor> logger = new FakeLogger<TransacaoExecutor>(output.WriteLine);
        IContaRepository repository = InicializarRepository();
        TransacaoExecutor executor = new TransacaoExecutor(logger, repository);
        var data = DateTime.Parse("09/09/2023 14:15:00", culture);
        var transacao = new Transacao(1, data, 938485762, 2147483649, 150);

        //act
        executor.ExecutarTransacao(transacao);

        //assert
        var contaOrigem = repository.ConsultarConta(938485762);
        var contaDestino = repository.ConsultarConta(2147483649);
        contaOrigem.Saldo.Should().Be(30);
        contaDestino.Saldo.Should().Be(150);
    }


    private static IContaRepository InicializarRepository()
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

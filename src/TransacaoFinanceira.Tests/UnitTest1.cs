using FluentAssertions;
using TransacaoFinanceira.Abstractions;
using TransacaoFinanceira.Conta.Abstractions;
using TransacaoFinanceira.Conta;
using Microsoft.Extensions.Logging.Testing;
using Xunit.Abstractions;
using System.Globalization;

namespace TransacaoFinanceira.Tests;

public class UnitTest1
{
    ITestOutputHelper output;
    IContaRepository repository;
    FakeLogger<TransacaoExecutor> logger;

    public UnitTest1(ITestOutputHelper output)
    {
        this.output = output;
        repository = new ContaRepository();
        logger = new FakeLogger<TransacaoExecutor>(x => output.WriteLine(x));
    }

    [Fact]
    public void Test1()
    {
        var origem = new ContaSaldo(9383123981, 300);
        var destino = new ContaSaldo(9383123982, 500);

        var executor = new TransacaoExecutor(logger, new ContaRepository());

        (var novaOrigem, var novaDestino) = executor.TransferirValor(origem, destino, 100);

        novaOrigem.Saldo.Should().Be(200);
        novaDestino.Saldo.Should().Be(600);

    }

    public static IEnumerable<object[]> TesteCompletoData()
    {
        yield return new object[] { 1, "09/09/2023 14:15:00", 938485762, 2147483649, 150, 30, 150 };
        yield return new object[] { 2, "09/09/2023 14:15:05", 2147483649, 210385733, 149, 1, 149 };
        yield return new object[] { 3, "09/09/2023 14:15:29", 347586970, 238596054, 1100, 0, 0 };
        yield return new object[] { 4, "09/09/2023 14:17:00", 675869708, 210385733, 5300, 0, 0 };
        yield return new object[] { 5, "09/09/2023 14:18:00", 238596054, 674038564, 1489, 0, 0 };
        yield return new object[] { 6, "09/09/2023 14:18:20", 573659065, 563856300, 49, 0, 0 };
        yield return new object[] { 7, "09/09/2023 14:19:00", 938485762, 2147483649, 44, 0, 0 };
        yield return new object[] { 8, "09/09/2023 14:19:01", 573659065, 675869708, 150, 0, 0 };
    }

    [Theory]
    [MemberData(nameof(TesteCompletoData))]
    public void TesteCompleto(int correlationId, string data, ulong numeroContaOrigem, ulong numeroContaDestino, decimal valor, decimal saldoOrigem, decimal saldoDestino)
    {
        var executor = new TransacaoExecutor(logger, repository);
        DateTime date =  DateTime.Parse(data, CultureInfo.GetCultureInfo("pt-br"));
        var transacao = new Transacao(correlationId, date, numeroContaOrigem, numeroContaDestino, valor);
        executor.ExecutarTransferencia(transacao);

        var contaOrigem = repository.ConsultarConta(numeroContaOrigem);
        var contaDestino = repository.ConsultarConta(numeroContaDestino);


        contaOrigem.Saldo.Should().Be(saldoOrigem);
        contaDestino.Saldo.Should().Be(saldoDestino);

    }

}

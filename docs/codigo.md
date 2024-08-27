## Problemas com o fonte inicial

- Paralelismo nas transações pode corromper os dados
- A transação em si não tem um modelo próprio
- A classe executarTransacaoFinanceira herda acessoDados.
- acessoDados usa tipos genéricos onde não há necessidade

## Fix

### Paralelismo.

```CSharp
Parallel.ForEach(transacoes, item =>
{
    executor.transferir(item.correlation_id, item.conta_origem, item.conta_destino, item.VALOR);
});
```

Executar as transações em paralelo, pela sua falta de ordem, pode causar corrupção nos dados. Para garantir a ordem troquei por um laço de repetição comum.
``` CSharp
foreach (var item in transacoes)
{
    executor.transferir(item.correlation_id, item.conta_origem, item.conta_destino, item.VALOR);
}        
```

### Tipos de dados

No que diz respeito ao correlation id e ao número da conta troquei de `int` para `ulong`. Isso garante que as variáveis tenha capacidade de armazenar as contas informadas e que não sejam menores que zero.

## Refactor

> Keep it simple stupid
> Arquitetura hexagonal

Foi escolhido utilizar `ILogger` por sua versatilidade. Pode ser integrado com Serilog fazendo Sink em uma instância do Grafana ou outro serviço de logs.
Passando ao logger os parâmetros como foi feito, as propriedades podem ser gravadas pelo logger adotado.
Ex: no grafana seria possível realizar filtros pelo correlationId da transação.

A classe `TransacaoExecutor` é responsável pro agregar as dependências e executar todo o processo.

``` CSharp
public class TransacaoExecutor
{
    private readonly ILogger<TransacaoExecutor> logger;
    private readonly IContaRepository contaRepository;

    public TransacaoExecutor(ILogger<TransacaoExecutor> logger, IContaRepository contaRepository)
    {
        this.logger = logger;
        this.contaRepository = contaRepository;
    }

    public void ExecutarTransacao(Transacao transacao)
    {
        var contaOrigem = contaRepository.ConsultarConta(transacao.ContaOrigem);

        if (contaOrigem.Saldo < transacao.Valor)
        {
            logger.LogInformation("Transacao número {correlationId} foi cancelada por falta de saldo", transacao.CorrelationId);
        }
        else
        {
            var contaDestino = contaRepository.ConsultarConta(transacao.ContaDestino);
            (var novoSaldoOrigem, var novoSaldoDestino) = TransferirValor(contaOrigem, contaDestino, transacao.Valor);

            contaRepository.AtualizarSaldo(novoSaldoOrigem);
            contaRepository.AtualizarSaldo(novoSaldoDestino);

            logger.LogInformation("Transacao número {correlationId} foi efetivada com sucesso! Novos saldos: Conta Origem: {saldoOrigem} | Conta Destino {saldoDestino}",
                    transacao.CorrelationId,
                    contaOrigem.Saldo,
                    contaDestino.Saldo);
        }
    }

    internal (ContaSaldo origem, ContaSaldo destino) TransferirValor(ContaSaldo origem, ContaSaldo destino, decimal valor)
    {
        var novoOrigem = new ContaSaldo(origem.NumeroConta, origem.Saldo - valor);
        var novoDestino = new ContaSaldo(destino.NumeroConta, destino.Saldo + valor);
        return (novoOrigem, novoDestino);
    }
}
```

A biblioteca criada não possui implementação do `IContaRepository` e mantém sua implementação a cargo de quem for utilizar.
Isso nos permite realizar testes com dados frios.

``` CSharp
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

    [Fact]
    public void TransacaoValorMaiorQueSaldo()
    {
        //arrange
        var culture = CultureInfo.GetCultureInfo("pr-BR");
        ILogger<TransacaoExecutor> logger = new FakeLogger<TransacaoExecutor>(output.WriteLine);
        IContaRepository repository = InicializarRepository();
        TransacaoExecutor executor = new TransacaoExecutor(logger, repository);
        var data = DateTime.Parse("09/09/2023 14:15:00", culture);
        var transacao = new Transacao(1, data, 938485762, 2147483649, 200);

        //act
        executor.ExecutarTransacao(transacao);

        //assert
        var contaOrigem = repository.ConsultarConta(938485762);
        var contaDestino = repository.ConsultarConta(2147483649);
        contaOrigem.Saldo.Should().Be(180);
        contaDestino.Saldo.Should().Be(0);
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
```

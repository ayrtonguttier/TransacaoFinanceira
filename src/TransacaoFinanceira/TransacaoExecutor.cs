using Microsoft.Extensions.Logging;
using TransacaoFinanceira.Abstractions;
using TransacaoFinanceira.Conta.Abstractions;

namespace TransacaoFinanceira;

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

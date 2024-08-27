using TransacaoFinanceira.Conta.Abstractions;

namespace TransacaoFinanceira.Tests;

public class ContaRepository : IContaRepository
{
    Dictionary<ulong, ContaSaldo> saldos = new();
    public ContaSaldo ConsultarConta(ulong numeroConta)
    {
        return saldos[numeroConta];
    }

    public void AtualizarSaldo(ContaSaldo conta)
    {
        saldos[conta.NumeroConta] = conta;
    }

    public void Clear()
    {
        saldos.Clear();
    }
}

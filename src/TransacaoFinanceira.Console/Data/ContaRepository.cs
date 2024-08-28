using TransacaoFinanceira.Conta.Abstractions;

namespace TransacaoFinanceira.Console.Data;

public class ContaRepository : IContaRepository
{
    Dictionary<ulong, ContaSaldo> contas = new();

    public ContaSaldo ConsultarConta(ulong numeroConta)
    {
        return contas[numeroConta];
    }

    public void AtualizarSaldo(ContaSaldo conta)
    {
        contas[conta.NumeroConta] = conta;
    }
}

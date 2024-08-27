namespace TransacaoFinanceira.Conta.Abstractions;

public class ContaSaldo
{
    public ContaSaldo(ulong numeroConta, decimal saldo)
    {
        NumeroConta = numeroConta;
        Saldo = saldo;
    }

    public ulong NumeroConta { get; }
    public decimal Saldo { get; }
}

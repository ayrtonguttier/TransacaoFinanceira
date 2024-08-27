using TransacaoFinanceira.Conta.Abstractions;

namespace TransacaoFinanceira.Conta;

public class ContaRepository : IContaRepository
{
    Dictionary<ulong, ContaSaldo> saldos;
    public ContaRepository()
    {
        var saldosIniciais = new List<ContaSaldo>();
        saldosIniciais.Add(new ContaSaldo(938485762, 180));
        saldosIniciais.Add(new ContaSaldo(347586970, 1200));
        saldosIniciais.Add(new ContaSaldo(2147483649, 0));
        saldosIniciais.Add(new ContaSaldo(675869708, 4900));
        saldosIniciais.Add(new ContaSaldo(238596054, 478));
        saldosIniciais.Add(new ContaSaldo(573659065, 787));
        saldosIniciais.Add(new ContaSaldo(210385733, 10));
        saldosIniciais.Add(new ContaSaldo(674038564, 400));
        saldosIniciais.Add(new ContaSaldo(563856300, 1200));

        saldos = saldosIniciais.ToDictionary(x => x.NumeroConta, x => x);
    }

    public ContaSaldo ConsultarConta(ulong numeroConta)
    {
        return saldos[numeroConta];
    }

    public void AtualizarSaldo(ContaSaldo conta)
    {
        saldos[conta.NumeroConta] = conta;
    }
}

namespace TransacaoFinanceira.Conta.Abstractions;

public interface IContaRepository
{
    ContaSaldo ConsultarConta(ulong numeroConta);
    void AtualizarSaldo(ContaSaldo conta);
}

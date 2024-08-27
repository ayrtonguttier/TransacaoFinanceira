namespace TransacaoFinanceira.Abstractions;

public class Transacao
{
    public Transacao(int correlationId, DateTime dataTransacao, ulong contaOrigem, ulong contaDestino, decimal valor)
    {
        CorrelationId = correlationId;
        DataTransacao = dataTransacao;
        ContaOrigem = contaOrigem;
        ContaDestino = contaDestino;
        Valor = valor;
    }

    public int CorrelationId { get; }
    public DateTime DataTransacao { get; }
    public ulong ContaOrigem { get; }
    public ulong ContaDestino { get; }
    public decimal Valor { get; }
}

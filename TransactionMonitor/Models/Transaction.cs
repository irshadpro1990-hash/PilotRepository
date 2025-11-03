namespace TransactionMonitor.Models;

public class Transaction
{
    public int TransactionId { get; set; }
    public string Status { get; set; }
    public DateTime LastModifiedDate { get; set; }
}

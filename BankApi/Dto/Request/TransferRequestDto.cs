using System.Text.Json.Serialization;

public class TransferRequestDto
{
    public string ReceiverAccountNumber { get; set; }
    public decimal Amount { get; set; }
    [JsonIgnore]
    public int ReceiverUserId { get; set; }

}
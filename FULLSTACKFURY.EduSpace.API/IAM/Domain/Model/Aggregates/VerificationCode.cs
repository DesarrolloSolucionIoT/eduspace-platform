namespace FULLSTACKFURY.EduSpace.API.IAM.Domain.Model.Aggregates;

public class VerificationCode
{
    public VerificationCode()
    {
        Code = default!;
        Account = default!;
    }

    public int Id { get; set; }
    public int AccountId { get; set; }
    public string Code { get; set; }
    public DateTime ExpirationDate { get; set; }
    public bool IsUsed { get; set; }

    public Account Account { get; set; }
}
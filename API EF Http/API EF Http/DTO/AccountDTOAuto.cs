using API_EF_Http.DataAccess;

namespace API_EF_Http.DTO
{
    public class AccountDTOAuto
    {
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? Role { get; set; }
        public virtual Customer Customer { get; set; }
    }
}

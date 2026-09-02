using System.ComponentModel.DataAnnotations;

namespace ReraModel.UserSessionModel
{

    public class UserSessionModel
    {
        public string UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public int UserRole_Id { get; set; }
        public string Rolename { get; set; } = string.Empty;
        public string Token { get; set; }
        public string SessionId { get; set; }
        public string UserType { get; set; }

        public string SSOId { get; set; }

        public string MobileNo { get; set; }
    }
}

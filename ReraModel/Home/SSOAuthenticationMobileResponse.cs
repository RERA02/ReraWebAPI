using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ReraModel.UserSessionModel
{

    public class SSOAuthenticationMobileResponse
    {
        [JsonPropertyName("valid")]
        public bool Valid { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; }

        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; }

        [JsonPropertyName("designation")]
        public string Designation { get; set; }

        [JsonPropertyName("mobile")]
        public string Mobile { get; set; }

        [JsonPropertyName("mailPersonal")]
        public string MailPersonal { get; set; }

        [JsonPropertyName("aadhaarId")]
        public string AadhaarId { get; set; }

        [JsonPropertyName("bhamashahId")]
        public string BhamashahId { get; set; }

        [JsonPropertyName("bhamashahMId")]
        public string BhamashahMId { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("oldSSOIDs")]
        public string OldSSOIDs { get; set; }

        [JsonPropertyName("janaadhaarId")]
        public string JanaadhaarId { get; set; }

        [JsonPropertyName("janaadhaarMemberId")]
        public string JanaadhaarMemberId { get; set; }

        [JsonPropertyName("userType")]
        public string UserType { get; set; }

        [JsonPropertyName("userStatus")]
        public string UserStatus { get; set; }

        [JsonPropertyName("mfa")]
        public string Mfa { get; set; }

        [JsonPropertyName("sansthaAadhaar")]
        public string SansthaAadhaar { get; set; }
    }
}

using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json.Serialization;

namespace ReraWebPortal.Code.Security
{
    public class CustomPrincipal : IPrincipal
    {
        [JsonIgnore]
        public IIdentity? Identity { get; private set; }

        public bool IsAuthenticated { get; private set; }
        public int UserID { get; set; }
        public string UserName { get; private set; }
        public string Email { get; set; }
        public string RoleIDs { get; set; }
        public string RoleNames { get; set; }

        private readonly ClaimsPrincipal _claimsPrincipal;

        public CustomPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            _claimsPrincipal = claimsPrincipal;

            SetCustomPrincipal();
        }
        public bool IsInRole(string roleName)
        {
            return RoleNames == roleName;
        }
        private void SetCustomPrincipal()
        {
            if (_claimsPrincipal.Identity != null)
            {
                IsAuthenticated = _claimsPrincipal.Identity.IsAuthenticated;

                if (IsAuthenticated)
                {
                    UserID = Convert.ToInt32(_claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
                    Identity = new GenericIdentity(_claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value);
                    UserName = _claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
                    Email = _claimsPrincipal.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value;

                    RoleIDs = _claimsPrincipal.Claims.FirstOrDefault(x => x.Type == nameof(RoleIDs))?.Value;
                    RoleNames = _claimsPrincipal.Claims.FirstOrDefault(x => x.Type == nameof(RoleNames))?.Value;
                }
            }
        }
    }
}

using System;

namespace OrderDesk.Security
{
    /// <summary>
    /// The signed-in user of ONE browser session. Stored in Application.Session (Module 4 replaced the
    /// static AppState.CurrentUser with this), never in a static field.
    /// </summary>
    public sealed class UserSessionContext
    {
        public string UserName { get; set; }
        public string Company { get; set; }
        public string Role { get; set; }
        public DateTime SignedInUtc { get; set; }
        public string SessionId { get; set; }

        public override string ToString() => $"{UserName} · {Company} · {Role}";
    }
}

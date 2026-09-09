using System;

namespace EnterpriseOps.Security
{
    /// <summary>
    /// THE ANTI-PATTERN — kept only so the "Static-state leak" button can demonstrate it and the audit can
    /// catch it. A static that holds the current user is shared by every session on the server: the last
    /// session to sign in overwrites it, and the next request from another user runs as that person.
    /// Nothing in this sample authorizes anything on these fields; production code deletes this class.
    /// </summary>
    public static class LegacyCurrentUser
    {
        public static string UserId;
        public static string TenantId;
        public static string SetBySessionId;
        public static DateTime SetAtUtc;

        /// <summary>What a legacy sign-in helper used to do: "remember" the user in process-wide memory.</summary>
        public static void Remember(string userId, string tenantId, string sessionId)
        {
            UserId = userId;
            TenantId = tenantId;
            SetBySessionId = sessionId;
            SetAtUtc = DateTime.UtcNow;
        }

        public static string Describe()
            => UserId == null ? "(unset)" : $"{UserId}@{TenantId} set by session {SessionContext.ShortId(SetBySessionId)} at {SetAtUtc:HH:mm:ss.fff}";
    }
}

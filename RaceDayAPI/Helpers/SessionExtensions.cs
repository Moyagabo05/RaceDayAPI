using Microsoft.AspNetCore.Http;

namespace RaceDayAPI.Helpers
{
    public static class SessionExtensions
    {
        public static void SetInt(this ISession session, string key, int value)
        {
            session.SetInt32(key, value);
        }

        public static int? GetInt(this ISession session, string key)
        {
            return session.GetInt32(key);
        }

        // NEW: Get User ID from MVC Header, or fallback to API Session (for Swagger)
        public static int? GetUserId(this HttpContext context)
        {
            var headerId = context.Request.Headers["X-User-Id"].FirstOrDefault();
            if (int.TryParse(headerId, out int id)) return id;
            return context.Session.GetInt("UserId");
        }

        // NEW: Get Role from MVC Header, or fallback to API Session (for Swagger)
        public static string GetRole(this HttpContext context)
        {
            return context.Request.Headers["X-User-Role"].FirstOrDefault()
                   ?? context.Session.GetString("UserRole");
        }
    }
}
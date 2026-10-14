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
    }
}

using System;

namespace WebApplication.API.Models.LiveDemo
{
    public class Visit
    {
        public Visit(string ip, string referer, string language, string userAgent)
        {
            IP = ip;
            Referer = referer;
            Language = language;
            UserAgent = userAgent;
        }

        protected Visit()
        {
        }

        public Guid Id { get; protected set; } = Guid.NewGuid();
        public string IP { get; set; }
        public string Referer { get; set; }
        public string Language { get; set; }
        public string UserAgent { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

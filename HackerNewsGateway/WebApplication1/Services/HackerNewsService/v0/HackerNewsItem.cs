namespace HackerNewsGateway.Services
{
    /// <summary>
    /// Internal model for deserializing raw Hacker News API responses.
    /// Matches the API structure exactly (Url, By, Descendants, Unix timestamp, Type field).
    /// </summary>
    internal class HackerNewsItem
    {
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? By { get; set; }
        public long Time { get; set; }
        public int Score { get; set; }
        public int? Descendants { get; set; }
        public string? Type { get; set; }
    }
}


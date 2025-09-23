namespace BoardJob.Domain.Configurations
{
    public class EventBusSettings
    {
        public string HostName { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string EventQueue { get; set; } = string.Empty;
        public string Fanout {  get; set; } = string.Empty;
    }
}
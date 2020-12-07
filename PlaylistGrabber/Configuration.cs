using System.Configuration;

namespace PlaylistGrabber
{
    public interface IConfiguration
    {
        string DestinationPathBase { get; }
    }

    public class Configuration : IConfiguration
    {
        public string DestinationPathBase => ConfigurationManager.AppSettings.Get("DestinationPathBase");
    }
}

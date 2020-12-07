using Autofac;
using System;
using System.Windows.Forms;

namespace PlaylistGrabber
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using var scope = BuildContainer().BeginLifetimeScope();
            var playlistGrabber = new PlaylistGrabber(scope.Resolve<IDownloader>());

            Application.Run(playlistGrabber);
        }

        static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Configuration>().As<IConfiguration>();
            builder.RegisterType<DestinationPathBuilder>().As<IDestinationPathBuilder>();
            builder.RegisterType<DirectoryWrapper>().As<IDirectoryWrapper>();
            builder.RegisterType<Downloader>().As<IDownloader>();
            builder.RegisterType<FileWrapper>().As<IFileWrapper>();
            builder.RegisterType<WebClientWrapper>().As<IWebClientWrapper>();
            return builder.Build();
        }
    }
}

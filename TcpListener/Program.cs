using System;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using log4net.Config;

namespace TcpListener
{
    public class Program
    {
        private static readonly ILog Log = log4net.LogManager.GetLogger(typeof(Program));
        private static void Main()
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));
            TcpServerAsync().Wait();
        }

        private static async Task TcpServerAsync()
        {
            if (!IPAddress.TryParse(ConfigurationManager.AppSettings["ipAddress"], out var ip))
            {
                    Console.WriteLine("Failed to get IP address, service will listen for client activity on all network interfaces.");
                    ip = IPAddress.Any;
            }

            if (!int.TryParse(ConfigurationManager.AppSettings["port"], out var port))
            {
                throw new ArgumentException("Port is not valid.");
            }

            Log.Info("Starting listener...");
            var server = new System.Net.Sockets.TcpListener(ip, port);

            server.Start();
            Log.Info("Listening : " + server.LocalEndpoint.ToString());
            while (true)
            {
                var client = await server.AcceptTcpClientAsync();
                var cw = new TcpClientService(client);

                _ = ThreadPool.UnsafeQueueUserWorkItem(async x =>
                {
                    try
                    {
                        await ((TcpClientService) x).Run(CancellationToken.None);
                        ((TcpClientService) x).Dispose();
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Exception : "+ex.Message);
                    }
                    finally
                    {
                        cw.Dispose();
                        Log.Info($"End of connection : {cw.RemoteEndPoint}");
                    }
                }, cw);
            }
        }
    }
}
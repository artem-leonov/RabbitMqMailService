using System.ServiceProcess;

namespace RabbitMQMailServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new RabbitMqMailServerService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}

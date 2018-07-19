using Base_MicroService;
using CommonMessages;
using EasyNetQ;
using EasyNetQ.MessageVersioning;
using EasyNetQ.Topology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeploymentMonitorMicroServiceTest
{
    class Program
    {
        static void Main(string[] args)
        {

            IBus bus = RabbitHutch.CreateBus("host=localhost", x =>
            {
                x.Register<IConventions, AttributeBasedConventions>();
                x.EnableMessageVersioning();
            });
            IExchange exchange = bus.Advanced.ExchangeDeclare("EvolvedAI", ExchangeType.Topic);
            IQueue queue = bus.Advanced.QueueDeclare("Deployments");
            bus.Advanced.Bind(exchange, queue, "");
            for (int i = 101; i < 1000; i++)
            {
                bus.Publish<DeploymentStartMessage>(new DeploymentStartMessage()
                {
                    ID = i,
                    Date = DateTime.Now
                },"Deployments");
                Console.WriteLine("Publishing Start Message:" + i);
            }
            // Sleep for 10 seconds
            Thread.Sleep(10000);

            for (int i = 101; i < 1000; i++)
            {
                // Publish Stop Message
                bus.Publish<DeploymentStopMessage>(new DeploymentStopMessage()
                {
                    ID = i,
                    Date = DateTime.Now
                }, "Deployments");
                Console.WriteLine("Publishing Stop Message:" + i);
            }

        }
    }
}

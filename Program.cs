using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Util;

namespace dotnetapp
{
    public static class Program
    {
        private static ManualResetEventSlim _done = new ManualResetEventSlim(false);
        private static IBusControl _bus;

        public static void Main(string[] args)
        {
            Task.Run(() => MainAsync(args)).Wait();
        }

        public static async Task MainAsync(string[] args)
        {
            using (var cts = new CancellationTokenSource())
            {
                Action Shutdown = () =>
                {
                    if (!cts.IsCancellationRequested)
                    {
                        Console.WriteLine("Application is shutting down...");
                        _bus.Stop();
                        cts.Cancel();
                    }
                    _done.Wait();
                };

                Console.CancelKeyPress += (sender, eventArgs) =>
                {
                    Shutdown();
                    // Don't terminate the process immediately, wait for the Main thread to exit gracefully.
                    eventArgs.Cancel = true;
                };

                _bus = MassTransit.Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri("rabbitmq://localhost"), h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });

                    cfg.ReceiveEndpoint(host, "my_queue", endpoint =>
                    {
                        endpoint.Handler<MyMessage>(async context =>
                        {
                            await Console.Out.WriteLineAsync($"Received: {context.Message.Value}");
                        });
                    });
                });

                await Console.Out.WriteLineAsync("Application is starting up...");
                await _bus.StartAsync(cts.Token);
                _done.Set();

                await _bus.Publish(new MyMessage { Value = "Hello, World" }, cts.Token);
            }
        }
    }

    public class MyMessage
    {
        public string Value { get; set; }
    }
}

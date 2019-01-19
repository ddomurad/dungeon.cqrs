using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using dungeon.cqrs.core.commands;
using dungeon.cqrs.core.event_sourcing.models;
using dungeon.cqrs.implementation;
using dungeon.cqrs.memory_store;
using dungeon.cqrs.mongodb;
using dungeon.cqrs.test.Models;
using Microsoft.Extensions.DependencyInjection;

namespace dungeon.cqrs.test
{
    class Program
    {

        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();

            DungeonMongoRegistrationTool.RegisterInternals();

            var serviceProvider = serviceCollection
                .AddCqrs<ConsoleLogger>()
                    .AddMongoEventSourcing("mongodb://172.17.0.2:27017", "test_cqrs")
                    // .WithoutSnapshots()
                    .WithMongoSnapshots(
                        "mongodb://172.17.0.2:27017", "test_cqrs",
                        c => c.SetDefaultSnapshotConfig(SnapshotConfig.VersionDiff(10)))
                    .RegisterCommandHandler<UserCommandHandler>()
                    .RegisterEventHandler<UserEventHandler>()
                    .Build()
                .BuildServiceProvider();

            DungeonMongoRegistrationTool.RegisterAll(typeof(Program).Assembly);

            var cmdDispatch = serviceProvider.GetService<ICommandDispatcher>();

            for (int i = 0; i < 10; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    while (true)
                    {
                        System.Console.WriteLine("E");
                        sw.Restart();
                        try
                        {

                            //cmdDispatch.Dispatch<Guid, RegisterUserCommand> (new RegisterUserCommand { Login = "Boleslaw", }, 5).Wait ();
                            cmdDispatch.DispatchAbstract<Guid>(new RegisterUserCommand { Login = "Boleslaw", }, 5).Wait();
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine(e);
                        }

                        try
                        {
                            cmdDispatch.DispatchAbstract<Guid>(new RegisterUserCommand { Login = "Marian" }, 5).Wait();
                            //cmdDispatch.Dispatch<Guid, RegisterUserCommand> (new RegisterUserCommand { Login = "Marian" }, 5).Wait ();
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine(e);
                        }

                        try
                        {
                            cmdDispatch.DispatchAbstract(new RemoveUserCommand { Login = "Marian" }, 5).Wait();
                            //cmdDispatch.Dispatch (new RemoveUserCommand { Login = "Marian" }, 5).Wait ();
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine(e);
                        }

                        try
                        {
                            cmdDispatch.DispatchAbstract(new RemoveUserCommand { Login = "Boleslaw" }, 5).Wait();
                            //cmdDispatch.Dispatch (new RemoveUserCommand { Login = "Boleslaw" }, 5).Wait ();
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine(e);
                        }
                        System.Console.WriteLine("E: " + sw.ElapsedMilliseconds);
                    }
                }, TaskCreationOptions.LongRunning);
            }

            while (true)
            {
                Thread.Sleep(10000);
            }
        }
    }
}
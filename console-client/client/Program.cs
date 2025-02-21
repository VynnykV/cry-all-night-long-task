﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static IConfigurationRoot configuration;
        private static QueryService queryService;
        static async Task Main(string[] args)
        {
            ServiceCollection serviceDescriptors = new ServiceCollection();
            ConfigureServices(serviceDescriptors);
            IServiceProvider serviceProvider = serviceDescriptors.BuildServiceProvider();
            queryService = serviceProvider.GetService<QueryService>();
            DisplayMainMenu();
            await MainMenu();
        }

        static void DisplayMainMenu()
        {
            Console.WriteLine("Welcome to LINQ!\n\n" +
                              "Enter a number what have to do:\n" +
                              "1. Get the number of tasks in the project of a particular user (by id).\n" +
                              "2. Get a list of tasks designed for a specific user (by id), where the name tag <45 characters.\n" +
                              "3. Get a list (id, name) from the collection of tasks completed (completed) in the current (2021) year for a specific user (by id).\n" +
                              "4. Get a list (id, team name and user list) from the collection of teams over 10 years old, sorted for this burning user registration, and grouped by team.\n" +
                              "5. Get a list of users in alphabetical order first_name (ascending) with sorted tasks by length name (descending).\n" +
                              "6. Get user info by id.\n" +
                              "7. Get project info by id\n" +
                              "8. Mark a random task as completed\n" +
                              "9. Clear console.\n" +
                              "10. Exit program.\n");
        }

        static async Task MainMenu()
        {

            Console.Write("Your choice: ");
            string choice = Console.ReadLine();
            try
            {
                switch (choice)
                {
                    case "1":
                    {
                        Console.Write("Enter userId: ");
                        int userId;
                        while (!int.TryParse(Console.ReadLine(), out userId))
                        {
                            Console.WriteLine("Wrong id");
                            Console.Write("Enter userId: ");
                        }

                        var result = await queryService.TasksInProjectByUserCount(userId);
                        foreach (var r in result)
                        {
                            Console.WriteLine($"Project:\n{r.Project}");
                            Console.WriteLine($"Has {r.NumsOfTasks} tasks");
                            Console.WriteLine("-------------------------------------");
                        }
                    }

                        break;

                    case "2":
                    {
                        Console.Write("Enter userId: ");
                        int userId;
                        while (!int.TryParse(Console.ReadLine(), out userId))
                        {
                            Console.WriteLine("Wrong id");
                            Console.Write("Enter userId: ");
                        }

                        var result = await queryService.GetTasksLimitedByName(userId, 45);
                        foreach (var task in result)
                        {
                            Console.WriteLine($"Task:\n{task}");
                        }

                        break;
                    }

                    case "3":
                    {
                        Console.Write("Enter userId: ");
                        int userId;
                        while (!int.TryParse(Console.ReadLine(), out userId))
                        {
                            Console.WriteLine("Wrong id");
                            Console.Write("Enter userId: ");
                        }

                        var result = await queryService.GetFinishedTasks(userId, 2021);
                        foreach (var r in result)
                        {
                            Console.WriteLine($"id: {r.Id} name: {r.Name}");
                        }

                        break;
                    }

                    case "4":
                    {
                        var result = await queryService.GetOlderUsers(10);
                        foreach (var r in result)
                        {
                            Console.WriteLine($"id: {r.Id} name: {r.Name}");
                            Console.WriteLine("-------------------------------------\nUsers:");
                            foreach (var user in r.Users)
                            {
                                Console.WriteLine(user);
                            }

                            Console.WriteLine("-------------------------------------");
                        }

                        break;
                    }

                    case "5":
                    {
                        var result = await queryService.GetSortedUsersWithTasks();
                        foreach (var r in result)
                        {
                            Console.WriteLine("-------------------------------------");
                            Console.WriteLine($"User:\n{r.User}");
                            Console.WriteLine("-------------------------------------");
                            Console.WriteLine("User's tasks");
                            foreach (var task in r.Tasks)
                            {
                                Console.WriteLine(task);
                            }
                        }

                        break;
                    }

                    case "6":
                    {
                        Console.Write("Enter userId: ");
                        int userId;
                        while (!int.TryParse(Console.ReadLine(), out userId))
                        {
                            Console.WriteLine("Wrong id");
                            Console.Write("Enter userId: ");
                        }

                        var result = await queryService.GetUserInfo(userId);
                        if (result is not null)
                        {
                            Console.WriteLine($"user:\n{result.User}\n" +
                                              $"last project:\n{result.LastProject}\n" +
                                              $"tasks quantity int last project: {result.TasksQuantityLastProject}\n" +
                                              $"general quantity of cancelled tasks: {result.GeneralQuantityCancelledTasks}\n" +
                                              $"general quantity of unfinished tasks: {result.GeneralQuantityUnfinishedTasks}\n" +
                                              $"the longest task by duration:\n {result.LongestTaskByDuration}");
                        }
                        else
                            Console.WriteLine("No information about this user.");

                        break;
                    }

                    case "7":
                    {
                        Console.Write("Enter projectId: ");
                        int projectId;
                        while (!int.TryParse(Console.ReadLine(), out projectId))
                        {
                            Console.WriteLine("Wrong id");
                            Console.Write("Enter userId: ");
                        }

                        var result = await queryService.GetProjectInfo(projectId);
                        if (result is not null)
                        {
                            Console.WriteLine($"project:\n{result.Project}\n" +
                                              $"the longest task by desc:\n {result.LongestProjectTaskByDesc}\n" +
                                              $"the shortest task by name:\n {result.ShortestProjectTaskByName}\n" +
                                              $"The total number of users in the project team, where either the project description> 20 characters, or the number of tasks <3:" +
                                              $" {result.TeamUsersCount}");
                        }
                        else
                            Console.WriteLine("No information about this project");

                        break;
                    }

                    case "8":
                    {
                        Task.Run(async () =>
                        {
                            var markedTaskId = await queryService.MarkRandomTaskWithDelay(1000);
                            Console.WriteLine($"markedTaskId: {markedTaskId}");
                        });
                        
                        break;
                    }

                    case "9":
                    {
                        Console.Clear();
                        break; 
                    }


                    case "10":
                    {
                        Environment.Exit(0);
                        break;
                    }

                    default:
                    {
                        Console.WriteLine("Entered incorrect number, try again");
                        break; 
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine();
                DisplayMainMenu();
                await MainMenu();
            }
        }

        private static async Task foo()
        {
            
            
        }
        
        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json")
                .Build();

            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);
            serviceCollection.AddHttpClient<QueryService>();
            serviceCollection.AddTransient<QueryService>();
        }
    }
}

using BenchmarkDotNet.Running;
using System.Reflection;

var currentAssembly = Assembly.GetExecutingAssembly();
BenchmarkSwitcher.FromAssembly(currentAssembly).Run(args: args);

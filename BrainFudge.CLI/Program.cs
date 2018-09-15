using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace BrainFudge.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("No arugments provided. Exiting...");
                Environment.Exit(0);
            }
            var input = File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, args[0]));         
            
            var interpreter = new Interpreter();
            Console.OutputEncoding = Encoding.UTF8;
            interpreter.ProgramOutput = (output) =>
            {
                Console.Write(Encoding.UTF8.GetChars(new byte[] { output }));
            };
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            interpreter.Interpret(input);
            stopwatch.Stop();
            Console.WriteLine($"Elapsed time: {stopwatch.Elapsed}");
            Console.Read();
        }
    }
}

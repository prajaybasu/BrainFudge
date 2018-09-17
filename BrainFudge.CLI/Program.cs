using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace BrainFudge.CLI
{
    public class Program
    {
        public static byte[] Instructions { get; set; } = null;

        public static Interpreter<byte> Interpreter { get; set; } = null;

        public static Stopwatch Stopwatch { get; set; } = new Stopwatch();

        static bool HasExecutionFinished { get; set; } = false;

        public static string[] Arguments { get; set; }

        static void Setup()
        {           
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;
            Arguments = Environment.GetCommandLineArgs().Skip(1).ToArray();
        }
        static void TakeInput()
        {
            if (Debugger.IsAttached || Arguments.Contains("-i"))
            {
                Instructions = Encoding.UTF8.GetBytes(Console.ReadLine());
            }
            else if (Arguments[0].Contains(".b"))
            {
                Instructions = File.ReadAllBytes(Path.Combine(Environment.CurrentDirectory, Arguments[0]));
            }
            else
            {
                Environment.Exit(0);
            }
            Interpreter = new Interpreter<byte>(new byte[65536])
            {
                ProgramOutput = (output) =>
                {
                    Console.Out.WriteAsync(Encoding.UTF8.GetChars(new byte[] { output }));
                },
                ProgramInput = () =>
                {
                    return Encoding.UTF8.GetBytes(Console.ReadLine())[0];
                }
            };
        }
        public static void Exit()
        {
            Stopwatch.Stop();
            Console.WriteLine();
            if (Arguments.Contains("-st"))
            {
                Console.WriteLine($"Elapsed time: {Stopwatch.Elapsed}");
            }
            HasExecutionFinished = true;
        }
        public static void Main()
        {
            Setup();
            TakeInput();
            Stopwatch.Start();
            Interpreter.Interpret(Instructions);
            if (!HasExecutionFinished)
            {
                Exit();
            }
            Console.Read();
        }
    }
}

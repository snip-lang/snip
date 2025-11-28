using Snip.CLI;
using snip.CLI;
using Snip.Lexer;
using Snip.Parser;
using Snip.Evaluator;

namespace Snip
{
    public static class Program
    {
        static void Main(string[] args)
        {
            string? source = null;
            var cli = new CLI.CLI();
            try
            {
                source = cli.Parse(args);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (snip.CLI.InvalidArgumentException e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            if (source is not null)
            {
                // File mode
                try
                {
                    var lexer = new Lexer.Lexer(source);
                    lexer.Tokenize();

                    var parser = new Parser.Parser(lexer);
                    var program = parser.Parse();

                    var evaluator = new Evaluator.Evaluator();
                    var env = new Evaluator.Environment();
                    env.InitializeBuiltins();

                    // Evaluate the program
                    var result = evaluator.Eval(program, env);
                    if (result.Type == Snip.Evaluator.ValueType.Return)
                    {
                        return; // Exit successfully
                    }
                }
                catch (Snip.Parser.ParsingError e)
                {
                    Console.WriteLine($"Parse Error: {e.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Runtime Error: {e.Message}");
                }
            }
            else
            {
                // REPL mode
                StartRepl();
            }
        }

        static void StartRepl()
        {
            Console.WriteLine("Type '.help' for commands or '.exit' to quit");

            var evaluator = new Evaluator.Evaluator();
            var env = new Evaluator.Environment();
            env.InitializeBuiltins();

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                if (input == null) break;

                if (input.StartsWith("."))
                {
                    if (input == ".exit") break;
                    if (input == ".help")
                    {
                        Console.WriteLine("Commands:");
                        Console.WriteLine("  .exit  - Exit REPL");
                        Console.WriteLine("  .help  - Show this help");
                        Console.WriteLine("  .clear - Clear environment");
                        continue;
                    }
                    if (input == ".clear")
                    {
                        env = new Evaluator.Environment();
                        Console.WriteLine("Environment cleared");
                        continue;
                    }
                    Console.WriteLine("Unknown command: " + input);
                    continue;
                }

                try
                {
                    var lexer = new Lexer.Lexer(input);
                    lexer.Tokenize();

                    var parser = new Parser.Parser(lexer);
                    var program = parser.Parse();

                    var result = evaluator.Eval(program, env);
                    if (result != null)
                    {
                        Console.WriteLine(result);
                    }
                }
                catch (Snip.Parser.ParsingError e)
                {
                    Console.WriteLine($"🔴 Parse Error: {e.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"💥 Runtime Error: {e.Message}");
                }
            }
        }
    }
}

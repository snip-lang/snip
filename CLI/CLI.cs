using snip.CLI;

namespace Snip.CLI
{
    public class CLI
    {
        public string? Parse(string[] args)
        {
            if (args.Length == 0)
            {
                return null; // No file provided, enter REPL mode
            }

            var file = args[0];
            try
            {
                var source = File.ReadAllText(file);
                return source;
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("Invalid filename: " + file);
            }
        }
    }
}

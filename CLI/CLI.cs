using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using snip.CLI;

namespace Snip.CLI
{
    public class CLI
    {
        public string Parse(string[] args)
        {
            if (args.Length == 0)
            {
                throw new InvalidArgumentException("Missing arguments: snip filename");
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
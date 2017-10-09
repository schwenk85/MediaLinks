using System;
using System.Linq;

namespace MediaLinks
{
    public class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length <= 1) return;
            if (!Enum.TryParse(args[0], true, out Key key)) return;

            foreach (var arg in args.Skip(1))
            {
                var parser = new MediaLinkParser();
                parser.Parse(key, arg);
                parser.OpenUrl();
            }
        }
    }
}
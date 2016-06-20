using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace basics
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = "test.kju";

            try
            {
                Lexer lexer = null;
                using (TextReader input = File.OpenText(filename))
                {
                    lexer = new Lexer(input);
                }

                var a = lexer.Tokens;
                var parser = new Parser(a);
                //Parser parser = new Parser(scanner.Tokens);
                //CodeGen codeGen = new CodeGen(parser.Result, Path.GetFileNameWithoutExtension(args[0]) + ".exe");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.ReadKey();
            }
        }
    }
}

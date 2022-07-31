namespace Assembler
{
    class Program
    {
        static void Main(string[] args)
        {
            int romAddress = 0;
            int ramAddress = 16;
            
            Parser symbolParser = new Parser(args[0]);
            Parser codeParser = new Parser(args[0]);

            SymbolTable symbols = new SymbolTable();

            while(symbolParser.HasMoreCommands())
            {
                symbolParser.Advance();
                switch (symbolParser.CurrentCommandType())
                {
                    case CommandType.C_COMMAND:
                    case CommandType.A_COMMAND:
                        romAddress++;
                        break;
                    case CommandType.L_COMMAND:
                        string currentSymbol = symbolParser.Symbol();
                        if (!symbols.Contains(currentSymbol))
                        {
                            symbols.AddEntry(currentSymbol, romAddress);
                        }
                        break;
                }
            }

            StreamWriter writer = new StreamWriter(Path.GetDirectoryName(args[0]) + "/" + Path.GetFileNameWithoutExtension(args[0]) + ".hack");

            while(codeParser.HasMoreCommands())
            {
                codeParser.Advance();
                switch (codeParser.CurrentCommandType())
                {
                    case CommandType.A_COMMAND:
                        string currentSymbol = codeParser.Symbol();
                        int num;
                        if (!int.TryParse(currentSymbol, out num))
                        {
                            if (!symbols.Contains(currentSymbol))
                            {
                                symbols.AddEntry(currentSymbol, ramAddress++);
                            }

                            num = symbols.GetAddress(currentSymbol);
                        }

                        string bin = Convert.ToString(num, 2);
                        writer.WriteLine(new string('0', 16 - bin.Length) + bin);
                        
                        break;
                    case CommandType.C_COMMAND:
                        writer.WriteLine("111" + Code.Comp(codeParser.Comp()) + Code.Dest(codeParser.Dest()) + Code.Jump(codeParser.Jump()));
                        break;
                    case CommandType.L_COMMAND:
                        break;
                }
            }

            writer.Close();
        }
    }
}
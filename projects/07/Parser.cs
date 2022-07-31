namespace VMTranslator
{
    enum CommandType
    {
        NULL,
        C_ARITHMETIC,
        C_PUSH,
        C_POP,
        C_LABEL,
        C_GOTO,
        C_IF,
        C_FUNCTION,
        C_RETURN,
        C_CALL
    }

    class Parser
    {
        StreamReader file;
        string currentCommand;
        CommandType currentCommandType;

        public Parser(string inputFile) 
        {
            file = new StreamReader(inputFile);
            currentCommand = string.Empty;
            currentCommandType = CommandType.NULL;
        }

        public bool HasMoreCommands()
        {
            bool hasMoreCommands = !file.EndOfStream;
            if(!hasMoreCommands)
            {
                file.Close();
            }

            return hasMoreCommands;
        }

        public void Advance()
        {
            string? str;

            str = file.ReadLine()?.Split('/')[0];

            while(string.IsNullOrWhiteSpace(str)) 
            {
                str = file.ReadLine()?.Split('/')[0];
            }

            currentCommand = str;

            currentCommandType = (currentCommand.Split()[0]) switch 
            {
                "add" or "sub" or "neg" or "eq" or "gt" or "lt" or "and" or "or" or "not" => CommandType.C_ARITHMETIC,
                "push" => CommandType.C_PUSH,
                _ => CommandType.NULL
            };
        }

        public CommandType CurrentCommandType()
        {
            return currentCommandType;
        }

        /// <summary>
        /// <para>Returns the first argument of the current command</para>
        /// <para>In the case of C_ARITHMETIC, the command itself is returned</para>
        /// <para>Should NOT be called when currentCommandType is C_RETURN</para>
        /// </summary>
        public string Arg1()
        {
            if(currentCommandType == CommandType.C_ARITHMETIC)
            {
                return currentCommand.Split()[0];
            }
            else if(currentCommandType == CommandType.C_PUSH)
            {
                return currentCommand.Split()[1];
            }
            else
            {
                throw new Exception("Invalid command type");
            }
        }

        /// <summary>
        /// <para>Returns the second argument of the current command</para>
        /// <para>Should be called only when currentCommandType is C_PUSH, C_POP, C_FUNCTION, or C_CALL</para>
        /// </summary>
        public int Arg2()
        {
            if(currentCommandType == CommandType.C_PUSH)
            {
                if(!(int.TryParse(currentCommand.Split()[2], out int arg)))
                    throw new FormatException("Parsing error");
                return arg;
            }
            else
            {
                throw new Exception("Invalid command type");
            }
        }
    }
}
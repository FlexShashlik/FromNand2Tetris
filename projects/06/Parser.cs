namespace Assembler
{
    enum CommandType
    {
        NULL,
        A_COMMAND,
        C_COMMAND,
        L_COMMAND
    }

    class Parser
    {
        StreamReader file;
        string currentCommand;
        /// <summary>
        /// Starting index of the actual command (without white spaces)
        /// </summary>
        CommandType currentCommandType;

        public Parser(string fileName) 
        {
            file = new StreamReader(fileName);
            currentCommand = String.Empty;
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

            str = file.ReadLine()?.Split('/')[0].Replace(" ", "");

            while(string.IsNullOrWhiteSpace(str)) 
            {
                str = file.ReadLine()?.Split('/')[0].Replace(" ", "");
            }

            currentCommand = str;

            currentCommandType = (currentCommand[0] == '@') ? CommandType.A_COMMAND : (currentCommand[0] == '(') ? CommandType.L_COMMAND : CommandType.C_COMMAND;
        }

        public CommandType CurrentCommandType()
        {
            return currentCommandType;
        }

        /// <summary>
        /// Should be called only when currentCommandType is A_COMMAND or L_COMMAND
        /// </summary>
        public string Symbol()
        {
            if(currentCommandType == CommandType.A_COMMAND)
            {
                return currentCommand.Substring(1);
            }
            else if(currentCommandType == CommandType.L_COMMAND)
            {
                return currentCommand.Substring(1, currentCommand.Length - 2);
            }
            else
            {
                throw new Exception("Invalid command type");
            }
        }

        // C-command: dest=comp;jump
        // Either the dest or jump fields may be empty
        // If there is no "=", the dest is null
        // If there is no ";", the jump is null

        /// <summary>
        /// <para>Returns the dest mnemonic in the current C_COMMAND</para>
        /// <para>Should be called only when currentCommandType is С_COMMAND</para>
        /// </summary>
        public string Dest()
        {
            if(currentCommandType == CommandType.C_COMMAND)
            {
                int i;

                if((i = currentCommand.IndexOf('=')) != -1)
                {
                    return currentCommand.Substring(0, i);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                throw new Exception("Invalid command type");
            }
        }

        /// <summary>
        /// <para>Returns the comp mnemonic in the current C_COMMAND</para>
        /// <para>Should be called only when currentCommandType is С_COMMAND</para>
        /// </summary>
        public string Comp()
        {
            if(currentCommandType == CommandType.C_COMMAND)
            {
                int i;

                if((i = currentCommand.IndexOf('=')) != -1)
                {
                    return currentCommand.Substring(i + 1);
                }
                else
                {
                    return currentCommand.Substring(0, currentCommand.IndexOf(';'));
                }
            }
            else
            {
                throw new Exception("Invalid command type");
            }
        }

        /// <summary>
        /// <para>Returns the jump mnemonic in the current C_COMMAND</para>
        /// <para>Should be called only when currentCommandType is С_COMMAND</para>
        /// </summary>
        public string Jump()
        {
            if(currentCommandType == CommandType.C_COMMAND)
            {
                int i;

                if((i = currentCommand.IndexOf(';')) != -1)
                {
                    return currentCommand.Substring(i + 1);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                throw new Exception("Invalid command type");
            }
        }
    }
}
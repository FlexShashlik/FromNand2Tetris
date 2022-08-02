namespace VMTranslator
{
    /// <summary>
    /// Translation from assembly mnemonics into binary codes
    /// </summary>
    class CodeWriter
    {
        public string InputFileName { private get; set; }
        StreamWriter file;
        int labelIndex;

        public CodeWriter(string outputFile)
        {
            file = new StreamWriter(outputFile);
            labelIndex = 0;
            InputFileName = string.Empty;
        }

        public void WriteInit()
        {
            file.WriteLine("@256");
            file.WriteLine("D=A");
            file.WriteLine("@SP");
            file.WriteLine("M=D");
            file.WriteLine("@SP");
            //TODO call sys init
        }

        public void WriteLabel(string label)
        {
            file.WriteLine($"({label})");
        }

        public void WriteGoto(string label)
        {
            file.WriteLine($"@{label}");
            file.WriteLine("0;JMP");
        }
        
        public void WriteIf(string label)
        {
            file.WriteLine("@SP");
            file.WriteLine("M=M-1");
            file.WriteLine("A=M");
            file.WriteLine("D=M");
            file.WriteLine($"@{label}");
            file.WriteLine("D;JNE");
        }

        public void WriteCall(string functionName, int numArgs)
        {

        }
        
        public void WriteReturn()
        {

        }
        
        public void WriteFunction(string functionName, int numArgs)
        {

        }

        public void WriteCurrentCommandInComment(string cmd)
        {
            file.WriteLine($"//{cmd}");
        }

        public void WriteArithmetic(string command)
        {
            file.WriteLine("@SP");
            file.WriteLine("A=M-1");
            file.WriteLine("D=M"); // D = y
            file.WriteLine("@SP");

            switch (command)
            {
                case "add":
                    file.WriteLine("M=M-1");
                    file.WriteLine("A=M-1");
                    file.WriteLine("M=M+D"); // M = x
                    break;
                case "sub":
                    file.WriteLine("M=M-1");
                    file.WriteLine("A=M-1");
                    file.WriteLine("M=M-D"); // M = x
                    break;
                case "neg":
                    file.WriteLine("A=M-1");
                    file.WriteLine("M=-M"); // M = y
                    break;
                case "eq":
                    file.WriteLine("M=M-1");
                    file.WriteLine("A=M-1");
                    file.WriteLine("M=M-D"); // M = x - y
                    file.WriteLine("D=M");
                    file.WriteLine($"@IFNE{labelIndex}");
                    file.WriteLine("D;JNE");
                    // If equal => -1
                    file.WriteLine("@SP");
                    file.WriteLine("A=M-1");
                    file.WriteLine("M=-1");
                    file.WriteLine($"@NEXT{labelIndex}");
                    file.WriteLine("0;JMP");
                    // If not equal, continue to next command
                    file.WriteLine($"(IFNE{labelIndex})");
                    file.WriteLine("@SP");
                    file.WriteLine("A=M-1");
                    file.WriteLine("M=0");
                    file.WriteLine($"(NEXT{labelIndex++})");
                    break;
                case "gt":
                    file.WriteLine("M=M-1");
                    file.WriteLine("A=M-1");
                    file.WriteLine("M=M-D");
                    file.WriteLine("D=M");
                    file.WriteLine($"@IFLE{labelIndex}");
                    file.WriteLine("D;JLE");
                    // If (res>0) => -1
                    file.WriteLine("@SP");
                    file.WriteLine("A=M-1");
                    file.WriteLine("M=-1");
                    file.WriteLine($"@NEXT{labelIndex}");
                    file.WriteLine("0;JMP");
                    // If (res<=0), continue to next command
                    file.WriteLine($"(IFLE{labelIndex})");
                    file.WriteLine("@SP");
                    file.WriteLine("A=M-1");
                    file.WriteLine("M=0");
                    file.WriteLine($"(NEXT{labelIndex++})");
                    break;
                case "lt":
                    file.WriteLine("M=M-1");
                    file.WriteLine("A=M-1");
                    file.WriteLine("M=D-M");
                    file.WriteLine("D=M");
                    file.WriteLine($"@IFLE{labelIndex}");
                    file.WriteLine("D;JLE");
                    // If (res>0) => -1
                    file.WriteLine("@SP");
                    file.WriteLine("A=M-1");
                    file.WriteLine("M=-1");
                    file.WriteLine($"@NEXT{labelIndex}");
                    file.WriteLine("0;JMP");
                    // If (res<=0), continue to next command
                    file.WriteLine($"(IFLE{labelIndex})");
                    file.WriteLine("@SP");
                    file.WriteLine("A=M-1");
                    file.WriteLine("M=0");
                    file.WriteLine($"(NEXT{labelIndex++})");
                    break;
                case "and":
                    file.WriteLine("M=M-1");
                    file.WriteLine("A=M-1");
                    file.WriteLine("M=M&D");
                    break;
                case "or":
                    file.WriteLine("M=M-1");
                    file.WriteLine("A=M-1");
                    file.WriteLine("M=M|D");
                    break;
                case "not":
                    file.WriteLine("A=M-1");
                    file.WriteLine("M=!M"); // M = y
                    break;
            }
        }

        public void WritePushPop(CommandType command, string segment, int index)
        {
            file.WriteLine($"@{index}");
            file.WriteLine("D=A");

            if(command == CommandType.C_PUSH)
            {
                switch (segment)
                {
                    case "constant":
                        break;
                    case "static":
                        file.WriteLine($"@{InputFileName}.{index}");
                        file.WriteLine("D=M");
                        break;
                    case "local":
                        file.WriteLine("@LCL");
                        file.WriteLine("A=D+M");
                        file.WriteLine("D=M");
                        break;
                    case "argument":
                        file.WriteLine("@ARG");
                        file.WriteLine("A=D+M");
                        file.WriteLine("D=M");
                        break;
                    case "this":
                        file.WriteLine("@THIS");
                        file.WriteLine("A=D+M");
                        file.WriteLine("D=M");
                        break;
                    case "that":
                        file.WriteLine("@THAT");
                        file.WriteLine("A=D+M");
                        file.WriteLine("D=M");
                        break;
                    case "pointer":
                        file.WriteLine("@R3");
                        file.WriteLine("A=D+A");
                        file.WriteLine("D=M");
                        break;
                    case "temp":
                        file.WriteLine("@R5");
                        file.WriteLine("A=D+A");
                        file.WriteLine("D=M");
                        break;
                }

                file.WriteLine("@SP");
                file.WriteLine("A=M");
                file.WriteLine("M=D");
                file.WriteLine("@SP");
                file.WriteLine("M=M+1");
            }
            else if(command == CommandType.C_POP)
            {
                switch (segment)
                {
                    case "static":
                        file.WriteLine($"@{InputFileName}.{index}");
                        file.WriteLine("D=A");
                        break;
                    case "local":
                        file.WriteLine("@LCL");
                        file.WriteLine("D=D+M");
                        break;
                    case "argument":
                        file.WriteLine("@ARG");
                        file.WriteLine("D=D+M");
                        break;
                    case "this":
                        file.WriteLine("@THIS");
                        file.WriteLine("D=D+M");
                        break;
                    case "that":
                        file.WriteLine("@THAT");
                        file.WriteLine("D=D+M");
                        break;
                    case "pointer":
                        file.WriteLine("@R3");
                        file.WriteLine("D=D+A");
                        break;
                    case "temp":
                        file.WriteLine("@R5");
                        file.WriteLine("D=D+A");
                        break;
                }
                
                file.WriteLine("@R13");
                file.WriteLine("M=D"); // R13 = mem. segment base + index

                file.WriteLine("@SP");
                file.WriteLine("M=M-1");
                file.WriteLine("A=M");
                file.WriteLine("D=M");

                file.WriteLine("@R13");
                file.WriteLine("A=M");
                file.WriteLine("M=D");
            }
            else
            {
                throw new Exception("PUSH or POP command expected");
            }
        }

        public void Close()
        {
            file.Close();
        }
    }
}
namespace VMTranslator
{
    /// <summary>
    /// Translation from assembly mnemonics into binary codes
    /// </summary>
    class CodeWriter
    {
        StreamWriter file;
        int labelIndex;

        public CodeWriter(string outputFile)
        {
            file = new StreamWriter(outputFile);
            labelIndex = 0;
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
            if(command == CommandType.C_PUSH)
            {
                if (segment == "constant")
                {
                    file.WriteLine($"@{index}");
                    file.WriteLine("D=A");
                    file.WriteLine("@SP");
                    file.WriteLine("A=M");
                    file.WriteLine("M=D");
                    file.WriteLine("@SP");
                    file.WriteLine("M=M+1");
                }
            }
        }

        public void Close()
        {
            file.Close();
        }
    }
}
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

        void PushDToStack()
        {
            file.WriteLine("@SP");
            file.WriteLine("A=M");
            file.WriteLine("M=D");
            file.WriteLine("@SP");
            file.WriteLine("M=M+1");
        }

        void PopFromStackToD()
        {
            file.WriteLine("@SP");
            file.WriteLine("AM=M-1");
            file.WriteLine("D=M");
        }

        public void WriteInit()
        {
            file.WriteLine("@256");
            file.WriteLine("D=A");
            file.WriteLine("@SP");
            file.WriteLine("M=D");
            WriteCall("Sys.init", 0);
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
            // push return address
            file.WriteLine($"@RET{labelIndex}");
            file.WriteLine("D=A");
            PushDToStack();

            // push LCL
            file.WriteLine("@LCL");
            file.WriteLine("D=M");
            PushDToStack();

            // push ARG
            file.WriteLine("@ARG");
            file.WriteLine("D=M");
            PushDToStack();
            
            // push THIS
            file.WriteLine("@THIS");
            file.WriteLine("D=M");
            PushDToStack();

            // push THAT
            file.WriteLine("@THAT");
            file.WriteLine("D=M");
            PushDToStack();
            
            // ARG = SP-numArgs-5
            file.WriteLine($"@{numArgs}");
            file.WriteLine("D=A");
            file.WriteLine("@SP");
            file.WriteLine("D=M-D");
            file.WriteLine("@5");
            file.WriteLine("D=D-A");
            file.WriteLine("@ARG");
            file.WriteLine("M=D");

            // LCL = SP
            file.WriteLine("@SP");
            file.WriteLine("D=M");
            file.WriteLine("@LCL");
            file.WriteLine("M=D");

            WriteGoto(functionName);

            WriteLabel($"RET{labelIndex++}");
        }
        
        public void WriteReturn()
        {
            file.WriteLine("@LCL");
            file.WriteLine("D=M");
            file.WriteLine("@R13"); // R13 = Frame
            file.WriteLine("M=D");
            file.WriteLine("@5");
            file.WriteLine("D=A");
            file.WriteLine("@R13");
            file.WriteLine("A=M-D");
            file.WriteLine("D=M");
            file.WriteLine("@R14"); // R14 = Ret
            file.WriteLine("M=D");

            PopFromStackToD();
            file.WriteLine("@ARG");
            file.WriteLine("A=M");
            file.WriteLine("M=D");
            file.WriteLine("D=A+1");
            file.WriteLine("@SP");
            file.WriteLine("M=D");

            file.WriteLine("@R13");
            file.WriteLine("A=M-1");
            file.WriteLine("D=M");
            file.WriteLine("@THAT");
            file.WriteLine("M=D");

            file.WriteLine("@R13");
            file.WriteLine("D=M");
            file.WriteLine("@2");
            file.WriteLine("D=D-A");
            file.WriteLine("A=D");
            file.WriteLine("D=M");
            file.WriteLine("@THIS");
            file.WriteLine("M=D");

            file.WriteLine("@R13");
            file.WriteLine("D=M");
            file.WriteLine("@3");
            file.WriteLine("D=D-A");
            file.WriteLine("A=D");
            file.WriteLine("D=M");
            file.WriteLine("@ARG");
            file.WriteLine("M=D");
            
            file.WriteLine("@R13");
            file.WriteLine("D=M");
            file.WriteLine("@4");
            file.WriteLine("D=D-A");
            file.WriteLine("A=D");
            file.WriteLine("D=M");
            file.WriteLine("@LCL");
            file.WriteLine("M=D");

            file.WriteLine("@R14");
            file.WriteLine("A=M");
            file.WriteLine("0;JMP");
        }
        
        public void WriteFunction(string functionName, int numArgs)
        {
            WriteLabel(functionName);
            for (int i = 0; i < numArgs; i++)
            {
                WritePushPop(CommandType.C_PUSH, "constant", 0);
            }
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

                PushDToStack();
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

                PopFromStackToD();

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
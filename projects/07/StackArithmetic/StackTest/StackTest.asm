//push constant 17
@17
D=A
@SP
A=M
M=D
@SP
M=M+1
//push constant 17
@17
D=A
@SP
A=M
M=D
@SP
M=M+1
//eq
@SP
A=M-1
D=M
@SP
M=M-1
A=M-1
M=M-D
D=M
@IFNE0
D;JNE
@SP
A=M-1
M=-1
@NEXT0
0;JMP
(IFNE0)
@SP
A=M-1
M=0
(NEXT0)
//push constant 17
@17
D=A
@SP
A=M
M=D
@SP
M=M+1
//push constant 16
@16
D=A
@SP
A=M
M=D
@SP
M=M+1
//eq
@SP
A=M-1
D=M
@SP
M=M-1
A=M-1
M=M-D
D=M
@IFNE1
D;JNE
@SP
A=M-1
M=-1
@NEXT1
0;JMP
(IFNE1)
@SP
A=M-1
M=0
(NEXT1)
//push constant 16
@16
D=A
@SP
A=M
M=D
@SP
M=M+1
//push constant 17
@17
D=A
@SP
A=M
M=D
@SP
M=M+1
//eq
@SP
A=M-1
D=M
@SP
M=M-1
A=M-1
M=M-D
D=M
@IFNE2
D;JNE
@SP
A=M-1
M=-1
@NEXT2
0;JMP
(IFNE2)
@SP
A=M-1
M=0
(NEXT2)
//push constant 892
@892
D=A
@SP
A=M
M=D
@SP
M=M+1
//push constant 891
@891
D=A
@SP
A=M
M=D
@SP
M=M+1
//lt
@SP
A=M-1
D=M
@SP
M=M-1
A=M-1
M=D-M
D=M
@IFLE3
D;JLE
@SP
A=M-1
M=-1
@NEXT3
0;JMP
(IFLE3)
@SP
A=M-1
M=0
(NEXT3)
//push constant 891
@891
D=A
@SP
A=M
M=D
@SP
M=M+1
//push constant 892
@892
D=A
@SP
A=M
M=D
@SP
M=M+1
//lt
@SP
A=M-1
D=M
@SP
M=M-1
A=M-1
M=D-M
D=M
@IFLE4
D;JLE
@SP
A=M-1
M=-1
@NEXT4
0;JMP
(IFLE4)
@SP
A=M-1
M=0
(NEXT4)
//push constant 891
@891
D=A
@SP
A=M
M=D
@SP
M=M+1
//push constant 891
@891
D=A
@SP
A=M
M=D
@SP
M=M+1
//lt
@SP
A=M-1
D=M
@SP
M=M-1
A=M-1
M=D-M
D=M
@IFLE5
D;JLE
@SP
A=M-1
M=-1
@NEXT5
0;JMP
(IFLE5)
@SP
A=M-1
M=0
(NEXT5)
//push constant 32767
@32767
D=A
@SP
A=M
M=D
@SP
M=M+1
//push constant 32766
@32766
D=A
@SP
A=M
M=D
@SP
M=M+1
//gt
@SP
A=M-1
D=M
@SP
M=M-1
A=M-1
M=M-D
D=M
@IFLE6
D;JLE
@SP
A=M-1
M=-1
@NEXT6
0;JMP
(IFLE6)
@SP
A=M-1
M=0
(NEXT6)
//push constant 32766
@32766
D=A
@SP
A=M
M=D
@SP
M=M+1
//push constant 32767
@32767
D=A
@SP
A=M
M=D
@SP
M=M+1
//gt
@SP
A=M-1
D=M
@SP
M=M-1
A=M-1
M=M-D
D=M
@IFLE7
D;JLE
@SP
A=M-1
M=-1
@NEXT7
0;JMP
(IFLE7)
@SP
A=M-1
M=0
(NEXT7)
//push constant 32766
@32766
D=A
@SP
A=M
M=D
@SP
M=M+1
//push constant 32766
@32766
D=A
@SP
A=M
M=D
@SP
M=M+1
//gt
@SP
A=M-1
D=M
@SP
M=M-1
A=M-1
M=M-D
D=M
@IFLE8
D;JLE
@SP
A=M-1
M=-1
@NEXT8
0;JMP
(IFLE8)
@SP
A=M-1
M=0
(NEXT8)
//push constant 57
@57
D=A
@SP
A=M
M=D
@SP
M=M+1
//push constant 31
@31
D=A
@SP
A=M
M=D
@SP
M=M+1
//push constant 53
@53
D=A
@SP
A=M
M=D
@SP
M=M+1
//add
@SP
A=M-1
D=M
@SP
M=M-1
A=M-1
M=M+D
//push constant 112
@112
D=A
@SP
A=M
M=D
@SP
M=M+1
//sub
@SP
A=M-1
D=M
@SP
M=M-1
A=M-1
M=M-D
//neg
@SP
A=M-1
D=M
@SP
A=M-1
M=-M
//and
@SP
A=M-1
D=M
@SP
M=M-1
A=M-1
M=M&D
//push constant 82
@82
D=A
@SP
A=M
M=D
@SP
M=M+1
//or
@SP
A=M-1
D=M
@SP
M=M-1
A=M-1
M=M|D
//not
@SP
A=M-1
D=M
@SP
A=M-1
M=!M

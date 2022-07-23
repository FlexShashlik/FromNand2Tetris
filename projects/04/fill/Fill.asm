// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Fill.asm

// Runs an infinite loop that listens to the keyboard input.
// When a key is pressed (any key), the program blackens the screen,
// i.e. writes "black" in every pixel;
// the screen should remain fully black as long as the key is pressed. 
// When no key is pressed, the program clears the screen, i.e. writes
// "white" in every pixel;
// the screen should remain fully clear as long as no key is pressed.

// Put your code here.
    @24575
    D=A
    @end_of_screen
    M=D
(KEYBOARD_LISTENING)
    @KBD
    D=M
    @FILL_BLACK
    D;JGT
    @FILL_WHITE
    D;JEQ
    @KEYBOARD_LISTENING
    0;JMP
(FILL_WHITE)
    @current_color
    M=0
    @FILL_LOOP
    0;JMP
(FILL_BLACK)
    @current_color
    M=-1
    @FILL_LOOP
    0;JMP
(FILL_LOOP)
    @SCREEN
    D=A
    @pos
    M=D
    (FILLING)
    @current_color
    D=M
    @pos
    A=M
    M=D
    D=A
    @end_of_screen
    D=M-D
    @KEYBOARD_LISTENING
    D;JEQ
    @pos
    M=M+1
    @FILLING
    0;JMP
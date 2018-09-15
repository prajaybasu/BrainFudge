using System.Runtime.CompilerServices;

namespace BrainFudge
{
    public class Interpreter
    {
        const byte UTF8_COMMA  = 44;
        const byte UTF8_FULL_STOP = 46;
        const byte UTF8_GREATER_THAN_SIGN = 62;
        const byte UTF8_LESS_THAN_SIGN = 60;
        const byte UTF8_PLUS_SIGN = 43;
        const byte UTF8_MINUS_SIGN = 45;
        const byte UTF8_LEFT_SQUARE_BRACKET = 91;
        const byte UTF8_RIGHT_SQUARE_BRACKET = 93;

        public delegate void Output (byte output);
        public delegate byte Input();

        public Output ProgramOutput;
        public Input ProgramInput;

        public byte[] Memory { get; }
        public uint MemoryPointer { get; private set; } = 0;
        public uint InstructionPointer { get; private set; } = 0;

        public Interpreter() : this(new byte[65536], 0, 0) { }

        public Interpreter(byte[] memory, uint memoryPointer, uint instructionPointer)
        {
            Memory = memory;
            MemoryPointer = memoryPointer;
            InstructionPointer = instructionPointer;
        }   

        public void Interpret(byte[] instructions)
        {
            unchecked
            {
                while (InstructionPointer < instructions.Length)
                {
                    switch (instructions[InstructionPointer])
                    {
                        case UTF8_PLUS_SIGN: Memory[MemoryPointer]++; break;
                        case UTF8_MINUS_SIGN: Memory[MemoryPointer]--; break;
                        case UTF8_GREATER_THAN_SIGN: MemoryPointer++; break;
                        case UTF8_LESS_THAN_SIGN: MemoryPointer--; break;
                        case UTF8_LEFT_SQUARE_BRACKET:
                            {
                                if (Memory[MemoryPointer] == 0)
                                {
                                    int depth = 1;
                                    while (depth > 0)
                                    {
                                        InstructionPointer++;
                                        switch (instructions[InstructionPointer])
                                        {
                                            case UTF8_LEFT_SQUARE_BRACKET: depth++; break;
                                            case UTF8_RIGHT_SQUARE_BRACKET: depth--; break;
                                        }
                                    }
                                }
                                break;
                            }
                        case UTF8_RIGHT_SQUARE_BRACKET:
                            {
                                if (Memory[MemoryPointer] != 0)
                                {
                                    int depth = 1;
                                    while (depth > 0)
                                    {
                                        InstructionPointer--;
                                        switch (instructions[InstructionPointer])
                                        {
                                            case UTF8_LEFT_SQUARE_BRACKET: depth--; break;
                                            case UTF8_RIGHT_SQUARE_BRACKET: depth++; break;
                                        }
                                    }
                                }
                                break;
                            }
                        case UTF8_COMMA: Memory[MemoryPointer] = ProgramInput.Invoke(); break;
                        case UTF8_FULL_STOP: ProgramOutput.Invoke(Memory[MemoryPointer]); break;
                    }
                    InstructionPointer++;
                }
            }
        }
    }
}

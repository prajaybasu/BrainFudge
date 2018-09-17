using System;

namespace BrainFudge
{
    public class Interpreter<TCell> where TCell: struct
    {
        const byte UTF8_COMMA = 44;
        const byte UTF8_FULL_STOP = 46;
        const byte UTF8_GREATER_THAN_SIGN = 62;
        const byte UTF8_LESS_THAN_SIGN = 60;
        const byte UTF8_PLUS_SIGN = 43;
        const byte UTF8_MINUS_SIGN = 45;
        const byte UTF8_LEFT_SQUARE_BRACKET = 91;
        const byte UTF8_RIGHT_SQUARE_BRACKET = 93;

        public delegate void Output(TCell output);
        public delegate TCell Input();

        public Output ProgramOutput;
        public Input ProgramInput;

        public TCell[] Memory { get; private set; }
        public uint MemoryPointer { get; private set; } = 0;
        public uint InstructionPointer { get; private set; } = 0;

        public Interpreter(TCell[] memory, uint memoryPointer = 0, uint instructionPointer = 0)
        {           
            Memory = memory;
            MemoryPointer = memoryPointer;
            InstructionPointer = instructionPointer;
            switch(typeof(TCell))
            {
                case Type t when t == typeof(byte) || t == typeof(ushort) || t == typeof(uint) || t == typeof(ulong): break;
                case Type t when t == typeof(sbyte) || t == typeof(short) || t == typeof(int) || t == typeof(long): //break; here to allow signed types
                default: throw new NotSupportedException();             }
        }
        /* Black Magic */
        public void Increment(ref TCell cell)
        {
            unchecked
            {
                if (typeof(TCell) == typeof(byte)) cell = (TCell)((object)((byte)((byte)((object) cell) + 1)));
                else if (typeof(TCell) == typeof(ushort)) cell = (TCell)((object)((ushort)((ushort)((object) cell) + 1)));
                else if (typeof(TCell) == typeof(uint)) cell = (TCell)((object)((uint)((object) cell) + 1U));
                else if (typeof(TCell) == typeof(ulong)) cell = (TCell)(object)(((ulong)((object) cell) + 1UL));

                else if (typeof(TCell) == typeof(sbyte)) cell = (TCell)((object)((sbyte)((sbyte)((object) cell) + 1)));
                else if (typeof(TCell) == typeof(short)) cell = (TCell)((object)((short)((short)((object) cell) + 1)));
                else if (typeof(TCell) == typeof(int)) cell = (TCell)((object)((int)(object) cell + 1));
                else if (typeof(TCell) == typeof(long)) cell = (TCell)(object)(((long)((object) cell) + 1L));
                else throw new NotSupportedException();
            }
        }
        public void Decrement(ref TCell cell)
        {
            unchecked
            {
                if (typeof(TCell) == typeof(byte)) cell = (TCell)((object)((byte)((byte)((object) cell) - 1)));
                else if (typeof(TCell) == typeof(ushort)) cell = (TCell)((object)((ushort)((ushort)((object) cell) - 1)));
                else if (typeof(TCell) == typeof(uint)) cell = (TCell)((object)(((uint)((object) cell) - 1U)));
                else if (typeof(TCell) == typeof(ulong)) cell = (TCell)((object)(((ulong)((object) cell) - 1UL)));

                else if (typeof(TCell) == typeof(sbyte)) cell = (TCell)((object)((sbyte)((sbyte)((object) cell) - 1)));
                else if (typeof(TCell) == typeof(short)) cell = (TCell)((object)((short)((short)((object) cell) - 1)));
                else if (typeof(TCell) == typeof(int)) cell = (TCell)((object)(((int)((object) cell) - 1)));
                else if (typeof(TCell) == typeof(long)) cell = (TCell)((object)(((long)((object) cell) - 1L)));
                else throw new NotSupportedException();
            }
        }

        public bool EqualsToZero(in TCell cell)
        {
            if (typeof(TCell) == typeof(byte)) return ((byte)((object) cell) == 0);
            else if (typeof(TCell) == typeof(ushort)) return ((ushort)((object) cell) == 0);
            else if (typeof(TCell) == typeof(uint)) return ((uint)((object) cell) == 0);
            else if (typeof(TCell) == typeof(ulong)) return ((ulong)((object) cell) == 0);

            else if (typeof(TCell) == typeof(sbyte)) return ((sbyte)((object) cell) == 0);
            else if (typeof(TCell) == typeof(short)) return ((short)((object) cell) == 0);
            else if (typeof(TCell) == typeof(int)) return ((int)((object) cell) == 0);
            else if (typeof(TCell) == typeof(long)) return ((long)((object) cell) == 0);
            else throw new NotSupportedException();

        }

        public void Interpret(byte[] instructions)
        {
            while (InstructionPointer < instructions.Length)
            {
                switch (instructions[InstructionPointer])
                {                   
                    case UTF8_PLUS_SIGN: Increment(ref Memory[MemoryPointer]); break;                     
                    case UTF8_MINUS_SIGN: Decrement(ref Memory[MemoryPointer]); break;
                    case UTF8_GREATER_THAN_SIGN: MemoryPointer++; break;
                    case UTF8_LESS_THAN_SIGN: MemoryPointer--; break;
                    case UTF8_LEFT_SQUARE_BRACKET:
                        {
                            if (EqualsToZero(in Memory[MemoryPointer]))
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
                            if (!EqualsToZero(in Memory[MemoryPointer]))
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

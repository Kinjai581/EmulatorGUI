using System;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace Emulator_Graphical_Interface
{
    //TODO:
//- Make the running program(which is the program memory code) run on a separate thread from the GUI.The GUI will run on the thread
// Make the while loop run on a seperate thread.
    public partial class Form1 : Form
    {
        private const int PixelSize = 15;
        private Color[,] pixelColors;
        public byte[] memory;
        public struct CPU
        {
            public byte RegisterA;
            public byte RegisterB;
            public ushort ProgramCounter;
            public byte StatusRegister; // Status register to hold flags
            public byte StackPointerRegister;
        }

        public enum StatusFlags
        {
            Equals,
            NotEquals,
            GreaterThan,
            LessThan,
            GreaterThanEquals,
            LessThanEquals,
        }

        public Form1()
        {
            InitializeComponent();
            panel1.Paint += Panel1_Paint;
            panel1.Dock = DockStyle.Fill;
            pixelColors = new Color [64, 64];
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CPU cpu = new CPU();
            byte[] programMemory = new byte[]
            {
            0x01, 0x42, // Load 66 into RegisterA
            0x02, 0x84, // Load 132 into RegisterB
            0x19, 0x02, 0x01, // Store Reg B in memory address
            0x03, 0x02, // Add 0x02 to RegisterA
            0x04, 0x01, // Add 0x01 to RegisterB
            0x05, 0x01, // Subtract 0x01 from RegisterA
            0x06, 0x01, // Subtract 0x01 from RegisterB
            0x07, 0x02, // Multiply RegisterA by 0x02
            0x08, 0x02, // Multiply RegisterB by 0x02
            0x09, 0x02, // Divide RegisterA by 0x02
            0x0A, 0x02, // Divide RegisterB by 0x02
            0x0B, 0x35, // Add value from memory address 0x35 to RegisterA
            0x0C, 0x35, // Add value from memory address 0x35 to RegisterB
            //0x01, 0x14, // Load 20 into Register A
            //0x0D, 0x00, // Jump to address 0x00
            0x10, 0x01, 0x02, // MOV instruction 
            //0x02, 0x67, // Loaded 103 into Register B (Used for testing the jumps)
            0x0E, 0x01, 0x02, // Comparing the two registers A and B
            //0x11, 0x00, //If the equals flag is set in Status Register (Jump not equals)
            //0x12, 0x00, // If the equals flag is set in the Status Register (Jump Greather than)
            //0x13, 0x00, // If the equals flag is set in the Status Register (Jump Less than)
            //0x14, 0x00, // If the equals flag is set in the Status Register (Jump Greater than equals)
            //0x15, 0x00, // If the equals flag is set in the Status Register (Jump Less than equals)
            0x16, 0x32, // Stack Pointer push instruction (start at memory address 100)
            0x16, 0x26,
            0x17, 0x23, // Stack pointer pop instrucion
            0x01, 3, // Load Register A with Red
            0x18, 0x28, 0x11, // Store address into Reg A
            0x1a, 0xd0, 0x07,//Load index into Register A from memory address
            0x1b, 0x02, 0x00, //Load inedec into Reg B from memory address
            //0x0F, 0x00, //If the equals flag is set in Status Register (Jump equals)
            0xFF        // Halt
            };

            // Initialize memory with some values 
            memory = new byte[8192];
            memory[0x7d0] = 1; // First pixel at top left is blue
            memory[0x7d1] = 2;
            memory[0x7d2] = 3;
            memory[0x7d3] = 4;
            memory[0x7d4] = 5;
            memory[0x7d5] = 6;
            memory[0x7d6] = 7;
            memory[0x7d7] = 8;
            memory[0x1129] = 1; // eyes
            memory[0x112c] = 1; // eyes
            memory[0x11a7] = 2;
            memory[0x11e8] = 2;
            memory[0x1229] = 2;
            memory[0x122a] = 2;
            memory[0x122b] = 2;
            memory[0x122c] = 2;
            memory[0x11ed] = 2;
            memory[0x11ae] = 2;
            //memory[0x80f] = 3;
            //memory[0x810] = 3;
            memory[0x35] = 0x10;


            cpu.StackPointerRegister = 0x64; //Stores memory address of 100 into the stack pointer (starts at this address)

            // Start executing instructions
            while (cpu.ProgramCounter < programMemory.Length)
            {
                if (ExecuteInstruction(ref cpu, programMemory, memory))
                    break; // Exit the loop if a halt instruction is encountered
            }
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int x = 0, y = 0;

            for (int i = 0x7d0; i <= 0x17d0; i++)
            {
                //for (int y = 0; y < pixelColors.GetLength(1); y++)
                //{
                // Draw each "pixel" as a filled rectangle
                //g.FillRectangle(new SolidBrush(pixelColors[x, y]),
                // x * PixelSize, y * PixelSize, PixelSize, PixelSize);
                //using (SolidBrush brush = new SolidBrush(Color.Red))
                //{
                //g.FillRectangle(brush, x * PixelSize, y * PixelSize, PixelSize, PixelSize);
                //}
                //}
                switch (memory[i])
                {

                    case 0:
                        using (SolidBrush brush = new SolidBrush(Color.Black))
                        {
                            g.FillRectangle(brush, x, y, PixelSize, PixelSize);
                        }
                        break;

                    case 1:
                        using (SolidBrush brush = new SolidBrush(Color.Blue))
                        {
                            g.FillRectangle(brush, x, y, PixelSize, PixelSize);
                        }
                        break;

                    case 2:
                        using (SolidBrush brush = new SolidBrush(Color.Yellow))
                        {
                            g.FillRectangle(brush, x, y, PixelSize, PixelSize);
                        }
                        break;

                    case 3:
                        using (SolidBrush brush = new SolidBrush(Color.Red))
                        {
                            g.FillRectangle(brush, x, y, PixelSize, PixelSize);
                        }
                        break;

                    case 4:
                        using (SolidBrush brush = new SolidBrush(Color.Green))
                        {
                            g.FillRectangle(brush, x, y, PixelSize, PixelSize);
                        }
                        break;

                    case 5:
                        using (SolidBrush brush = new SolidBrush(Color.Orange))
                        {
                            g.FillRectangle(brush, x, y, PixelSize, PixelSize);
                        }
                        break;

                    case 6:
                        using (SolidBrush brush = new SolidBrush(Color.Purple))
                        {
                            g.FillRectangle(brush, x, y, PixelSize, PixelSize);
                        }
                        break;

                    case 7:
                        using (SolidBrush brush = new SolidBrush(Color.Gray))
                        {
                            g.FillRectangle(brush, x, y, PixelSize, PixelSize);
                        }
                        break;

                    case 8:
                        using (SolidBrush brush = new SolidBrush(Color.Indigo))
                        {
                            g.FillRectangle(brush, x, y, PixelSize, PixelSize);
                        }
                        break;

                }
                x += PixelSize;
                if (x % 0x40 == 0)
                {
                    x = 0;
                    y += PixelSize;
                }
            }
        }
        public static bool ExecuteInstruction(ref CPU cpu, byte[] programMemory, byte[] memory)
        {
            byte opcode = programMemory[cpu.ProgramCounter];
            byte operand = (cpu.ProgramCounter + 1 < programMemory.Length) ? programMemory[cpu.ProgramCounter + 1] : (byte)0;
            byte operand1 = (cpu.ProgramCounter + 2 < programMemory.Length) ? programMemory[cpu.ProgramCounter + 2] : (byte)0;

            switch (opcode)
            {
                case 0x01: // Load immediate value into RegisterA
                    cpu.RegisterA = operand;
                    //StatusRegisterFunc(ref cpu, cpu.RegisterA);
                    cpu.ProgramCounter += 2;
                    Console.WriteLine($"Loaded {cpu.RegisterA} into RegisterA");
                    break;

                case 0x02: // Load immediate value into RegisterB
                    cpu.RegisterB = operand;
                    //StatusRegisterFunc(ref cpu, cpu.RegisterB);
                    cpu.ProgramCounter += 2;
                    Console.WriteLine($"Loaded {cpu.RegisterB} into RegisterB");
                    break;

                case 0x03: // Add immediate value to RegisterA
                    cpu.RegisterA += operand;
                    //StatusRegisterFunc(ref cpu, cpu.RegisterA);
                    cpu.ProgramCounter += 2;
                    Console.WriteLine($"Added {operand} to RegisterA = {cpu.RegisterA}");
                    break;

                case 0x04: // Add immediate value to RegisterB
                    cpu.RegisterB += operand;
                    //StatusRegisterFunc(ref cpu, cpu.RegisterB);
                    cpu.ProgramCounter += 2;
                    Console.WriteLine($"Added {operand} to RegisterB = {cpu.RegisterB}");
                    break;

                case 0x05: // Subtract immediate value from RegisterA
                    cpu.RegisterA -= operand;
                    //StatusRegisterFunc(ref cpu, cpu.RegisterA);
                    cpu.ProgramCounter += 2;
                    Console.WriteLine($"Subtracted {operand} from RegisterA = {cpu.RegisterA}");
                    break;

                case 0x06: // Subtract immediate value from RegisterB
                    cpu.RegisterB -= operand;
                    //StatusRegisterFunc(ref cpu, cpu.RegisterB);
                    cpu.ProgramCounter += 2;
                    Console.WriteLine($"Subtracted {operand} from RegisterB = {cpu.RegisterB}");
                    break;

                case 0x07: // Multiply RegisterA by immediate value
                    cpu.RegisterA *= operand;
                    //StatusRegisterFunc(ref cpu, cpu.RegisterA);
                    cpu.ProgramCounter += 2;
                    Console.WriteLine($"Multiplied RegisterA by {operand} = {cpu.RegisterA}");
                    break;

                case 0x08: // Multiply RegisterB by immediate value
                    cpu.RegisterB *= operand;
                    //StatusRegisterFunc(ref cpu, cpu.RegisterB);
                    cpu.ProgramCounter += 2;
                    Console.WriteLine($"Multiplied RegisterB by {operand} = {cpu.RegisterB}");
                    break;

                case 0x09: // Divide RegisterA by immediate value
                    if (operand != 0)
                    {
                        cpu.RegisterA /= operand;
                        //StatusRegisterFunc(ref cpu, cpu.RegisterA);
                        cpu.ProgramCounter += 2;
                        Console.WriteLine($"Divided RegisterA by {operand} = {cpu.RegisterA}");
                    }
                    else
                    {
                        Console.WriteLine("Error: Division by zero");
                        cpu.ProgramCounter += 2;
                    }
                    break;


                case 0x0A: // Divide RegisterB by immediate value
                    if (operand != 0)
                    {
                        cpu.RegisterB /= operand;
                        //StatusRegisterFunc(ref cpu, cpu.RegisterB);
                        cpu.ProgramCounter += 2;
                        Console.WriteLine($"Divided RegisterB by {operand} = {cpu.RegisterB}");
                    }
                    else
                    {
                        Console.WriteLine("Error: Division by zero");
                        cpu.ProgramCounter += 2;
                    }
                    break;

                case 0x0B: // Add value from memory address (operand) to RegisterA
                    cpu.RegisterA += memory[operand];
                    //StatusRegisterFunc(ref cpu, cpu.RegisterA);
                    cpu.ProgramCounter += 2;
                    Console.WriteLine($"Added value from memory address {operand} to RegisterA = {cpu.RegisterA}");
                    break;

                case 0x0C: // Add value from memory address (operand) to RegisterB
                    cpu.RegisterB += memory[operand];
                    //StatusRegisterFunc(ref cpu, cpu.RegisterB);
                    cpu.ProgramCounter += 2;
                    Console.WriteLine($"Added value from memory address {operand} to RegisterB = {cpu.RegisterB}");
                    break;

                case 0x0D: // Jump instruction (operand)
                    cpu.ProgramCounter = operand;
                    Console.WriteLine($"Jumped to address {operand}");
                    break;


                case 0x0E: // Comparing the two registers
                    byte val1 = 0, val2 = 0;
                    if (operand == 1)
                    {
                        val1 = cpu.RegisterA;
                    }
                    else if (operand == 2)
                    {
                        val1 = cpu.RegisterB;
                    }

                    if (operand1 == 1)
                    {
                        val2 = cpu.RegisterA;
                    }

                    else if (operand1 == 2)
                    {
                        val2 = cpu.RegisterB;
                    }

                    else
                    { // Exception error handling
                        Console.WriteLine("Invalid operand");
                        return true;
                    }

                    StatusRegisterFunc(ref cpu, val1, val2);

                    cpu.ProgramCounter += 3;

                    Console.WriteLine("Status register equals {0}", Convert.ToString(cpu.StatusRegister, 2).PadLeft(8, '0'));
                    break;

                case 0x10: // MOV instruction
                    if (operand == 1)
                    { // Source is Reg A
                        if (operand1 == 2)
                        { // Destination is Reg B
                            cpu.RegisterB = cpu.RegisterA;
                            Console.WriteLine($"Moved the value from Register A to Register B: {cpu.RegisterB}");
                        }

                        else
                        {
                            Console.WriteLine("Invalid MOV destination");
                            return true;
                        }
                    }

                    else if (operand == 2)
                    { // Source is Reg B
                        if (operand1 == 1)
                        { // Destination is Reg A
                            cpu.RegisterA = cpu.RegisterB;
                            Console.WriteLine($"Moved the value from Register B to Register A: {cpu.RegisterA}");
                        }
                        else
                        {
                            Console.WriteLine("Invalid MOV destination");
                            return true;
                        }
                    }

                    else
                    {
                        Console.WriteLine("Invalid MOV destination");
                        return true;
                    }

                    cpu.ProgramCounter += 3;
                    break;


                case 0x0F: // Jump if equals instruction
                    if (((cpu.StatusRegister & (1 << (byte)StatusFlags.Equals)) != 0))
                    { // returns true, first bit set to 1
                        cpu.ProgramCounter = operand;
                        Console.WriteLine("Jumped to address {0} because registers are equal", operand);
                    }
                    else
                    {
                        cpu.ProgramCounter += 2;
                        Console.WriteLine("Conditional jump not taken");
                    }
                    break;

                case 0x11: // Jump if not equals instruction
                    if (((cpu.StatusRegister & (1 << (byte)StatusFlags.NotEquals)) != 0))
                    { // returns true, shifting the value 1
                        cpu.ProgramCounter = operand;
                        Console.WriteLine("Jumped to address {0} because registers are not equal", operand);
                    }
                    else
                    {
                        cpu.ProgramCounter += 2;
                        Console.WriteLine("Conditional jump not taken");
                    }
                    break;

                case 0x12: // Jump if Greater than instruction
                    if (((cpu.StatusRegister & (1 << (byte)StatusFlags.GreaterThan)) != 0))
                    { // returns true,shifting the value 1
                        cpu.ProgramCounter = operand;
                        Console.WriteLine("Jumped to address {0} because greater than flag is set", operand);
                    }
                    else
                    {
                        cpu.ProgramCounter += 2;
                        Console.WriteLine("Conditional jump not taken");
                    }
                    break;

                case 0x13: // Jump if Less than instruction
                    if (((cpu.StatusRegister & (1 << (byte)StatusFlags.LessThan)) != 0))
                    { // returns true,shifting the value 1
                        cpu.ProgramCounter = operand;
                        Console.WriteLine("Jumped to address {0} because less than flag", operand);
                    }
                    else
                    {
                        cpu.ProgramCounter += 2;
                        Console.WriteLine("Conditional jump not taken");
                    }
                    break;

                case 0x14: // Jump if Greater than equals instruction
                    if (((cpu.StatusRegister & (1 << (byte)StatusFlags.GreaterThanEquals)) != 0))
                    { // returns true,shifting the value 1
                        cpu.ProgramCounter = operand;
                        Console.WriteLine("Jumped to address {0} because greater than equals the flag", operand);
                    }
                    else
                    {
                        cpu.ProgramCounter += 2;
                        Console.WriteLine("Conditional jump not taken");
                    }
                    break;

                case 0x15: // Jump if Less than equals instruction
                    if (((cpu.StatusRegister & (1 << (byte)StatusFlags.LessThanEquals)) != 0))
                    { // returns true,shifting the value 1
                        cpu.ProgramCounter = operand;
                        Console.WriteLine("Jumped to address {0} because less than equals the flag", operand);
                    }
                    else
                    {
                        cpu.ProgramCounter += 2;
                        Console.WriteLine("Conditional jump not taken");
                    }
                    break;

                case 0x16: // Push stack pointer instruction

                    memory[cpu.StackPointerRegister] = operand;// Add the index
                    Console.WriteLine($"Memory address at {cpu.StackPointerRegister} is equal to {operand}");
                    cpu.StackPointerRegister++;
                    cpu.ProgramCounter += 2;
                    break;

                case 0x17: // Pop stack pointer instruction
                    memory[cpu.StackPointerRegister] = operand;// Add the index
                    cpu.StackPointerRegister--;
                    Console.WriteLine($"Memory address at {cpu.StackPointerRegister} is equal to {operand}");
                    cpu.ProgramCounter += 2;
                    break;

                case 0x18: // Store register value A
                    byte lowerByte = operand;
                    byte higherByte = operand1;
                    ushort address = (ushort)((higherByte << 8) | lowerByte);
                    memory[address] = cpu.RegisterA; // store value of Reg A into memory
                    //MessageBox.Show($"{memory[address]}");
                    cpu.ProgramCounter += 3;
                    break;

                case 0x19: // Store register value B
                    byte lowerByte1 = operand;
                    byte higherByte1 = operand1;
                    ushort address1 = (ushort)((higherByte1 << 8) | lowerByte1);
                    memory[address1] = cpu.RegisterB; // store value of Reg A into memory
                    //MessageBox.Show($"{memory[address1]}");
                    cpu.ProgramCounter += 3;
                    break;

                case 0x1a:
                    byte lowByte = operand;
                    byte highByte = operand1;
                    ushort address2 = (ushort)((highByte << 8) | lowByte);
                    cpu.RegisterA = memory[address2];
                    cpu.ProgramCounter += 3;
                    break;

                case 0x1b:
                    byte lowByte1 = operand;
                    byte highByte1 = operand1;
                    ushort address3 = (ushort)((highByte1 << 8) | lowByte1);
                    cpu.RegisterB = memory[address3];
                    cpu.ProgramCounter += 3;
                    break;


                case 0xFF: // Halt instruction
                    Console.WriteLine("Halt instruction encountered. Stopping execution.");
                    return true;

                default:
                    Console.WriteLine($"Unknown instruction: {opcode}");
                    cpu.ProgramCounter += 2;
                    break;
            }

            return false; // Continue execution
        }

        // Method to update the Status Register
        public static void StatusRegisterFunc(ref CPU cpu, byte A, byte B)
        {
            cpu.StatusRegister = 0;
            if (A == B)
            {
                cpu.StatusRegister |= (1 << (byte)StatusFlags.Equals); // If Reg A is equal to Register B
            }

            if (A != B)
            {
                cpu.StatusRegister |= (1 << (byte)StatusFlags.NotEquals); // If Reg A is not equal to Register B
            }

            if (A > B)
            {
                cpu.StatusRegister |= (1 << (byte)StatusFlags.GreaterThan); //If Reg A is > Reg B
            }

            if (A < B)
            {
                cpu.StatusRegister |= (1 << (byte)StatusFlags.LessThan); //If Reg
            }

            if (A >= B)
            {
                cpu.StatusRegister |= (1 << (byte)StatusFlags.GreaterThanEquals);
            }

            if (A <= B)
            {
                cpu.StatusRegister |= (1 << (byte)StatusFlags.LessThanEquals);
            }

        }
    }
}


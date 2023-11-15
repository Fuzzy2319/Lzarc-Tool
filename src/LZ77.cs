namespace LzarcTool
{
    public class LZ77
    {
        // Ported from 
        // https://github.com/mistydemeo/quickbms/blob/5752a6a2a38e16211952553fcffa59570855ac42/included/nintendo.c#L58
        // various code from DSDecmp: http://code.google.com/p/dsdecmp/
        // original code of unlz77wii_raw10 from "Hector Martin <marcan@marcansoft.com>" http://wiibrew.org/wiki/Wii.py
        // ported to C by Luigi Auriemma
        // Credit: https://github.com/KillzXGaming/Switch-Toolbox/blob/master/Switch_Toolbox_Library/Compression/LZ77_WII.cs#L22
        public static byte[] Decompress(byte[] input, int decomp_size)
        {
            int i, j, disp = 0, len = 0, cdest;
            byte b1, bt, b2, b3, flags;
            int threshold = 1;
            bool flag = false;

            int inputOffset = 0;
            int curr_size = 0;

            byte[] outdata = new byte[decomp_size];

            while (curr_size < decomp_size)
            {
                if (inputOffset >= input.Length) break;

                flags = input[inputOffset++];
                for (i = 0; i < 8 && curr_size < decomp_size; i++)
                {
                    flag = (flags & (0x80 >> i)) > 0;
                    if (flag)
                    {
                        if (inputOffset > input.Length) break;
                        b1 = input[inputOffset++];

                        switch ((int)(b1 >> 4))
                        {
                            //#region case 0
                            case 0:
                                {
                                    // ab cd ef
                                    // =>
                                    // len = abc + 0x11 = bc + 0x11
                                    // disp = def

                                    len = b1 << 4;
                                    if (inputOffset >= input.Length) break;
                                    bt = input[inputOffset++];
                                    len |= bt >> 4;
                                    len += 0x11;

                                    disp = (bt & 0x0F) << 8;
                                    if (inputOffset > input.Length) break;
                                    b2 = input[inputOffset++];
                                    disp |= b2;
                                    break;
                                }
                            //#endregion

                            //#region case 1
                            case 1:
                                {
                                    // ab cd ef gh
                                    // => 
                                    // len = bcde + 0x111
                                    // disp = fgh
                                    // 10 04 92 3F => disp = 0x23F, len = 0x149 + 0x11 = 0x15A

                                    if (inputOffset + 3 > input.Length) break;
                                    bt = input[inputOffset++];
                                    b2 = input[inputOffset++];
                                    b3 = input[inputOffset++];

                                    len = (b1 & 0xF) << 12; // len = b000
                                    len |= bt << 4; // len = bcd0
                                    len |= (b2 >> 4); // len = bcde
                                    len += 0x111; // len = bcde + 0x111
                                    disp = (b2 & 0x0F) << 8; // disp = f
                                    disp |= b3; // disp = fgh
                                    break;
                                }
                            //#endregion

                            //#region other
                            default:
                                {
                                    // ab cd
                                    // =>
                                    // len = a + threshold = a + 1
                                    // disp = bcd

                                    len = (b1 >> 4) + threshold;

                                    disp = (b1 & 0x0F) << 8;
                                    if (inputOffset >= input.Length) break;

                                    b2 = input[inputOffset++];
                                    disp |= b2;
                                    break;
                                }
                                //#endregion
                        }

                        if (disp > curr_size)
                            return new byte[] {};

                        cdest = curr_size;

                        for (j = 0; j < len && curr_size < decomp_size; j++)
                            outdata[curr_size++] = outdata[cdest - disp - 1 + j];

                        if (curr_size > decomp_size)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (inputOffset >= input.Length) break;
                        outdata[curr_size++] = input[inputOffset++];

                        if (curr_size > decomp_size)
                        {
                            break;
                        }
                    }
                }
            }

            return outdata;
        }

        public static byte[] Compress(byte[] input, out int compressedSize)
        {
            //TODO: Add LZ77 Compression
            compressedSize = input.Length;
            return input;
        }
    }
}

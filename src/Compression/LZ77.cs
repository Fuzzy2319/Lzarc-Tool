namespace LzarcTool.Compression
{
    public class LZ77
    {
        // Credit: https://github.com/KillzXGaming/Switch-Toolbox/blob/master/Switch_Toolbox_Library/Compression/LZ77_WII.cs#L22
        public static byte[] Decompress(byte[] input, int decomp_size)
        {
            int i;
            int j;
            int disp = 0;
            int len = 0;
            int cdest;
            byte b1;
            byte bt;
            byte b2;
            byte b3;
            byte flags;
            int threshold = 1;
            int inputOffset = 0;
            int curr_size = 0;

            byte[] outdata = new byte[decomp_size];

            while (curr_size < decomp_size)
            {
                if (inputOffset >= input.Length) break;

                flags = input[inputOffset++];
                for (i = 0; i < 8 && curr_size < decomp_size; i++)
                {
                    bool flag = (flags & 0x80 >> i) > 0;
                    if (flag)
                    {
                        if (inputOffset > input.Length)
                        {
                            break;
                        }

                        b1 = input[inputOffset++];

                        switch (b1 >> 4)
                        {
                            //#region case 0
                            case 0:
                                // ab cd ef
                                // =>
                                // len = abc + 0x11 = bc + 0x11
                                // disp = def

                                len = b1 << 4;

                                if (inputOffset >= input.Length)
                                {
                                    break;
                                }

                                bt = input[inputOffset++];
                                len |= bt >> 4;
                                len += 0x11;

                                disp = (bt & 0x0F) << 8;

                                if (inputOffset > input.Length)
                                {
                                    break;
                                }

                                b2 = input[inputOffset++];
                                disp |= b2;
                                break;

                            case 1:
                                // ab cd ef gh
                                // => 
                                // len = bcde + 0x111
                                // disp = fgh
                                // 10 04 92 3F => disp = 0x23F, len = 0x149 + 0x11 = 0x15A

                                if (inputOffset + 3 > input.Length)
                                {
                                    break;
                                }

                                bt = input[inputOffset++];
                                b2 = input[inputOffset++];
                                b3 = input[inputOffset++];

                                len = (b1 & 0xF) << 12; // len = b000
                                len |= bt << 4; // len = bcd0
                                len |= b2 >> 4; // len = bcde
                                len += 0x111; // len = bcde + 0x111
                                disp = (b2 & 0x0F) << 8; // disp = f
                                disp |= b3; // disp = fgh
                                break;

                            default:
                                // ab cd
                                // =>
                                // len = a + threshold = a + 1
                                // disp = bcd

                                len = (b1 >> 4) + threshold;

                                disp = (b1 & 0x0F) << 8;
                                if (inputOffset >= input.Length)
                                {
                                    break;
                                }

                                b2 = input[inputOffset++];
                                disp |= b2;
                                break;
                        }

                        if (disp > curr_size)
                        {
                            return new byte[] { };
                        }

                        cdest = curr_size;

                        for (j = 0; j < len && curr_size < decomp_size; j++)
                        {
                            outdata[curr_size++] = outdata[cdest - disp - 1 + j];
                        }

                        if (curr_size > decomp_size)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (inputOffset >= input.Length)
                        {
                            break;
                        }

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

        public static byte[] Compress(byte[] input)
        {
            // TODO implement LZ77 type 11 compression
            return input;
        }
    }
}

namespace ThGameMgr.Ex.Score
{
    /**
    Copyright (c) 2013, IIHOSHI Yoshinori

    Redistribution and use in source and binary forms, with or without
    modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this
      list of conditions and the following disclaimer.

    * Redistributions in binary form must reproduce the above copyright notice,
      this list of conditions and the following disclaimer in the documentation
      and/or other materials provided with the distribution.

    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
    AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
    IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
    DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
    FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
    DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
    SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
    CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
    OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
    OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
    **/
    internal class Lzss
    {
        /// <summary>
        /// The size of the dictionary.
        /// </summary>
        private const int DicSize = 0x2000;

        /// <summary>
        /// Compresses data by LZSS format.
        /// </summary>
        /// <param name="input">The stream to input data.</param>
        /// <param name="output">The stream that is output the compressed data.</param>
        public static void Compress(Stream input, Stream output)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Decompresses the LZSS formatted data.
        /// </summary>
        /// <param name="input">The stream to input data.</param>
        /// <param name="output">The stream that is output the decompressed data.</param>
        public static void Decompress(Stream input, Stream output)
        {
            BitReader reader = new BitReader(input);
            byte[] dictionary = new byte[DicSize];
            int dicIndex = 1;

            while (dicIndex < dictionary.Length)
            {
                int flag = reader.ReadBits(1);
                if (flag != 0)
                {
                    byte ch = (byte)reader.ReadBits(8);
                    output.WriteByte(ch);
                    dictionary[dicIndex] = ch;
                    dicIndex = (dicIndex + 1) & 0x1FFF;
                }
                else
                {
                    int offset = reader.ReadBits(13);
                    if (offset == 0)
                    {
                        break;
                    }
                    else
                    {
                        int length = reader.ReadBits(4) + 3;
                        for (int i = 0; i < length; i++)
                        {
                            byte ch = dictionary[(offset + i) & 0x1FFF];
                            output.WriteByte(ch);
                            dictionary[dicIndex] = ch;
                            dicIndex = (dicIndex + 1) & 0x1FFF;
                        }
                    }
                }
            }
        }
    }
}

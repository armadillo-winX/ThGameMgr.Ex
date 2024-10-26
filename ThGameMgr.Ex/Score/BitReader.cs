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
    internal class BitReader
    {
        /// <summary>
        /// The stream to read.
        /// </summary>
        private readonly Stream stream;

        /// <summary>
        /// The byte that is currently reading.
        /// </summary>
        private int current;

        /// <summary>
        /// The mask value that represents the reading bit position.
        /// </summary>
        private byte mask;

        /// <summary>
        /// Initializes a new instance of the <see cref="BitReader"/> class.
        /// </summary>
        /// <param name="stream">
        /// The stream to read. Since a <see cref="BitReader"/> instance does not own <paramref name="stream"/>,
        /// it is responsible for the caller to close <paramref name="stream"/>.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> is not readable.</exception>
        public BitReader(Stream stream)
        {
            if (stream is null)
                throw new ArgumentNullException(nameof(stream));
            if (!stream.CanRead)
                throw new ArgumentException();

            this.stream = stream;
            this.current = 0;
            this.mask = 0x80;
        }

        /// <summary>
        /// Reads the specified number of bits from the stream.
        /// </summary>
        /// <param name="num">The number of reading bits.</param>
        /// <returns>The value that is read from the stream.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="num"/> is negative.</exception>
        public int ReadBits(int num)
        {
            if (num < 0)
                throw new ArgumentOutOfRangeException(nameof(num));

            int value = 0;
            for (int i = 0; i < num; i++)
            {
                if (this.mask == 0x80)
                {
                    this.current = this.stream.ReadByte();
                    if (this.current < 0)   // EOF
                        this.current = 0;
                }

                value <<= 1;
                if (((byte)this.current & this.mask) != 0)
                    value |= 1;
                this.mask >>= 1;
                if (this.mask == 0)
                    this.mask = 0x80;
            }

            return value;
        }
    }
}

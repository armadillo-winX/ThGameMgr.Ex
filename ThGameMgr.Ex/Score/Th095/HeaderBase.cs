namespace ThGameMgr.Ex.Score.Th095
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
    internal class HeaderBase
    {
        public const int SignatureSize = 4;
        public const int Size = SignatureSize + (sizeof(int) * 3) + (sizeof(uint) * 2);

        private uint unknown1;
        private uint unknown2;

        public HeaderBase()
        {
            this.Signature = string.Empty;
            this.EncodedAllSize = 0;
            this.EncodedBodySize = 0;
            this.DecodedBodySize = 0;
        }

        public string Signature { get; private set; }

        public int EncodedAllSize { get; private set; }

        public int EncodedBodySize { get; private set; }

        public int DecodedBodySize { get; private set; }

        public virtual bool IsValid => (this.EncodedAllSize - this.EncodedBodySize) == Size;

        public void ReadFrom(BinaryReader reader)
        {
            this.Signature = Encoding.Default.GetString(reader.ReadExactBytes(SignatureSize));

            this.EncodedAllSize = reader.ReadInt32();
            if (this.EncodedAllSize < 0)
            {
                throw new InvalidDataException();
            }

            this.unknown1 = reader.ReadUInt32();
            this.unknown2 = reader.ReadUInt32();

            this.EncodedBodySize = reader.ReadInt32();
            if (this.EncodedBodySize < 0)
            {
                throw new InvalidDataException();
            }

            this.DecodedBodySize = reader.ReadInt32();
            if (this.DecodedBodySize < 0)
            {
                throw new InvalidDataException();
            }
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(Encoding.Default.GetBytes(this.Signature));
            writer.Write(this.EncodedAllSize);
            writer.Write(this.unknown1);
            writer.Write(this.unknown2);
            writer.Write(this.EncodedBodySize);
            writer.Write(this.DecodedBodySize);
        }
    }
}

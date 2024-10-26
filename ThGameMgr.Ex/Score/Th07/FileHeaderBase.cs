namespace ThGameMgr.Ex.Score.Th07
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
    internal class FileHeaderBase
    {
        private ushort unknown1;
        private ushort unknown2;
        private uint unknown3;

        public FileHeaderBase()
        {
        }

        public ushort Checksum { get; private set; }

        public short Version { get; private set; }

        public int Size { get; private set; }

        public int DecodedAllSize { get; private set; }

        public int DecodedBodySize { get; private set; }

        public int EncodedBodySize { get; private set; }

        public virtual bool IsValid => this.DecodedAllSize == this.Size + this.DecodedBodySize;

        public void ReadFrom(BinaryReader reader)
        {
            this.unknown1 = reader.ReadUInt16();
            this.Checksum = reader.ReadUInt16();
            this.Version = reader.ReadInt16();
            this.unknown2 = reader.ReadUInt16();
            this.Size = reader.ReadInt32();
            this.unknown3 = reader.ReadUInt32();
            this.DecodedAllSize = reader.ReadInt32();
            this.DecodedBodySize = reader.ReadInt32();
            this.EncodedBodySize = reader.ReadInt32();
        }

        public void WriteTo(BinaryWriter writer)
        {
            writer.Write(this.unknown1);
            writer.Write(this.Checksum);
            writer.Write(this.Version);
            writer.Write(this.unknown2);
            writer.Write(this.Size);
            writer.Write(this.unknown3);
            writer.Write(this.DecodedAllSize);
            writer.Write(this.DecodedBodySize);
            writer.Write(this.EncodedBodySize);
        }
    }
}

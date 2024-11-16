namespace ThGameMgr.Ex.Score
{
    internal class ScoreDecoder
    {
        public static bool Decode(string gameId, string scorePath, Stream outputData)
        {
            if (gameId == GameIndex.Th06)
            {
                return Th06.Th06Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIndex.Th07)
            {
                return Th07.Th07Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIndex.Th08)
            {
                return Th08.Th08Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIndex.Th09)
            {
                return Th09.Th09Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIndex.Th10)
            {
                return Th10.Th10Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIndex.Th11)
            {
                return Th11.Th11Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIndex.Th12)
            {
                return Th12.Th12Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIndex.Th13)
            {
                return Th13.Th13Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIndex.Th14)
            {
                return Th14.Th14Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIndex.Th15)
            {
                return Th15.Th15Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIndex.Th16)
            {
                return Th16.Th16Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIndex.Th17)
            {
                return Th17.Th17Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIndex.Th18)
            {
                return Th18.Th18Decoder.Convert(scorePath, outputData);
            }
            else
            {
                throw new NotSupportedException("対応していない作品です。");
            }
        }
    }
}

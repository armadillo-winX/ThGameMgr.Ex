namespace ThGameMgr.Ex.Score
{
    internal class ScoreDecoder
    {
        public static bool Decode(string gameId, string scorePath, Stream outputData)
        {
            if (gameId == GameIdIndex.Th06)
            {
                return Th06.Th06Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIdIndex.Th07)
            {
                return Th07.Th07Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIdIndex.Th08)
            {
                return Th08.Th08Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIdIndex.Th09)
            {
                return Th09.Th09Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIdIndex.Th10)
            {
                return Th10.Th10Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIdIndex.Th11)
            {
                return Th11.Th11Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIdIndex.Th12)
            {
                return Th12.Th12Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIdIndex.Th13)
            {
                return Th13.Th13Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIdIndex.Th14)
            {
                return Th14.Th14Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIdIndex.Th15)
            {
                return Th15.Th15Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIdIndex.Th16)
            {
                return Th16.Th16Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIdIndex.Th17)
            {
                return Th17.Th17Decoder.Convert(scorePath, outputData);
            }
            else if (gameId == GameIdIndex.Th18)
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

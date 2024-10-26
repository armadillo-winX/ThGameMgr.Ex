using System.Xml;

namespace ThGameMgr.Ex.Score
{
    internal class SpellCard
    {
        public string? CardID { get; set; }

        public string? CardName { get; set; }

        public string? Enemy { get; set; }

        public string? Place { get; set; }

        public static SpellCard GetSpellCardData(string gameId, int cardId)
        {
            string spellcardDataFilePath = GetSpellCardDataFilePath(gameId);

            if (File.Exists(spellcardDataFilePath))
            {
                XmlDocument spellCardDataDocument = new();
                spellCardDataDocument.Load(spellcardDataFilePath);
                XmlNode cardNameNode = spellCardDataDocument.SelectSingleNode($"//SpellCard[@ID='{cardId}']/Name");
                XmlNode cardEnemyNode = spellCardDataDocument.SelectSingleNode($"//SpellCard[@ID='{cardId}']/Enemy");
                XmlNode cardPlaceNode = spellCardDataDocument.SelectSingleNode($"//SpellCard[@ID='{cardId}']/Place");

                SpellCard spellCard = new()
                {
                    CardID = cardId.ToString(),
                    CardName = cardNameNode.InnerText,
                    Enemy = cardEnemyNode.InnerText,
                    Place = cardPlaceNode.InnerText
                };
                return spellCard;
            }
            else
            {
                throw new FileNotFoundException("スペルカードデータファイルが見つかりませんでした。");
            }
        }

        public static string GetSpellCardDataFilePath(string gameId)
        {
            string spellcardDataDirectory = PathInfo.SpellCardDataDirectory;
            return $"{spellcardDataDirectory}\\{gameId}SpellCardData.xml";
        }
    }
}

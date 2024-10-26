using System.Xml;

namespace ThGameMgr.Ex
{
    internal class User
    {
        private readonly static string? _usersDirectory = PathInfo.UsersDirectory;

        public static string? CurrentUserName { get; set; }

        public static string? CurrentUserDirectoryPath { get; set; }

        public static void Add(string userName)
        {
            if (!Directory.Exists(_usersDirectory))
            {
                Directory.CreateDirectory(_usersDirectory);
            }

            string? usersIndexFile = PathInfo.UsersIndexFile;
            if (!File.Exists(usersIndexFile))
            {
                CreateUsersIndex();
            }

            int i = 0;
            string newUserDirectoryName = $"user{i}";
            while (Directory.Exists($"{_usersDirectory}\\{newUserDirectoryName}"))
            {
                i++;
                newUserDirectoryName = $"user{i}";
            }

            Directory.CreateDirectory($"{_usersDirectory}\\{newUserDirectoryName}");
            Directory.CreateDirectory($"{_usersDirectory}\\{newUserDirectoryName}\\Settings");

            XmlDocument usersIndexDocument = new();
            usersIndexDocument.Load(usersIndexFile);

            XmlElement rootNode = usersIndexDocument.DocumentElement;

            XmlElement userElement = usersIndexDocument.CreateElement("User");
            XmlAttribute index = usersIndexDocument.CreateAttribute("Index");
            index.Value = userName;
            _ = userElement.Attributes.Append(index);
            _ = rootNode.AppendChild(userElement);

            XmlElement nameElement = usersIndexDocument.CreateElement("Name");
            _ = nameElement.AppendChild(usersIndexDocument.CreateTextNode(userName));
            _ = userElement.AppendChild(nameElement);

            XmlElement pathElement = usersIndexDocument.CreateElement("DirectoryName");
            _ = pathElement.AppendChild(usersIndexDocument.CreateTextNode(newUserDirectoryName));
            _ = userElement.AppendChild(pathElement);

            usersIndexDocument.Save(usersIndexFile);
        }

        public static bool Exists(string userName)
        {
            string? usersIndexFile = PathInfo.UsersIndexFile;
            if (File.Exists(usersIndexFile))
            {
                XmlDocument usersIndexDocument = new();
                usersIndexDocument.Load(usersIndexFile);
                XmlNode? node
                    = usersIndexDocument.DocumentElement?.SelectSingleNode($"//User[@Index='{userName}']");
                return node != null;
            }
            else
            {
                return false;
            }
        }

        public static string? GetUserDirectoryName(string userName)
        {
            string? usersIndexFile = PathInfo.UsersIndexFile;
            if (Exists(userName))
            {
                XmlDocument usersIndexDocument = new();
                usersIndexDocument.Load(usersIndexFile);

                string userDirectoryName
                    = usersIndexDocument.SelectSingleNode($"//User[@Index='{userName}']/DirectoryName").InnerText;

                return userDirectoryName;
            }
            else
            {
                return null;
            }
        }

        public static bool Switch(string userName)
        {
            if (!string.IsNullOrEmpty(userName))
            {
                string userDirectoryName = GetUserDirectoryName(userName);
                CurrentUserName = userName;
                CurrentUserDirectoryPath = $"{_usersDirectory}\\{userDirectoryName}";

                return true;
            }
            else
            {
                return false;
            }
        }

        public static void CreateUsersIndex()
        {
            if (!Directory.Exists(_usersDirectory))
            {
                Directory.CreateDirectory(_usersDirectory);
            }

            string? usersIndexFile = PathInfo.UsersIndexFile;

            if (!File.Exists(usersIndexFile))
            {
                XmlDocument usersIndexDocument = new();
                XmlNode docNode = usersIndexDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
                _ = usersIndexDocument.AppendChild(docNode);

                XmlNode rootNode = usersIndexDocument.CreateElement("UsersIndex");
                _ = usersIndexDocument.AppendChild(rootNode);

                usersIndexDocument.Save(usersIndexFile);
            }
        }

        public static void SaveUserSelectionConfig()
        {
            string? userSelectionConfigFile = PathInfo.UserSelectionConfigFile;
            if (!string.IsNullOrEmpty(CurrentUserName))
            {
                XmlDocument userSelectionConfigXml = new();
                XmlNode docNode = userSelectionConfigXml.CreateXmlDeclaration("1.0", "UTF-8", null);
                _ = userSelectionConfigXml.AppendChild(docNode);

                XmlNode rootNode = userSelectionConfigXml.CreateElement("UserSelectionConfig");
                _ = userSelectionConfigXml.AppendChild(rootNode);

                XmlElement selectionNode = userSelectionConfigXml.CreateElement("UserSelection");
                _ = selectionNode.AppendChild(userSelectionConfigXml.CreateTextNode(CurrentUserName));
                _ = rootNode.AppendChild(selectionNode);
                userSelectionConfigXml.Save(userSelectionConfigFile);
            }
        }

        public static string GetUserSelection()
        {
            string? userSelectionConfigFile = PathInfo.UserSelectionConfigFile;

            XmlDocument userSelectionConfigDocument = new();
            userSelectionConfigDocument.Load(userSelectionConfigFile);
            string userName = userSelectionConfigDocument.SelectSingleNode("//UserSelection").InnerText;

            return userName;
        }
    }
}

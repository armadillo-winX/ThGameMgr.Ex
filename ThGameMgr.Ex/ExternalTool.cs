using System.Xml;

namespace ThGameMgr.Ex
{
    internal class ExternalTool
    {
        public string? Name { get; set; }

        public string? ToolPath { get; set; }

        public string? Option { get; set; }

        public bool AsAdmin { get; set; }

        public static void Start(string toolName)
        {
            ExternalTool externalTool = GetToolInfo(toolName);

            ProcessStartInfo toolStartInfo = new()
            {
                FileName = externalTool.ToolPath,
                Arguments = externalTool.Option,
                WorkingDirectory
                = Path.GetDirectoryName(externalTool.ToolPath)
            };

            if (externalTool.AsAdmin)
            {
                toolStartInfo.Verb = "runas";
                toolStartInfo.UseShellExecute = true;
            }

            _ = Process.Start(toolStartInfo);
        }

        public static void CreateExternalConfigFile()
        {
            string exToolsConfig = $"{User.CurrentUserDirectoryPath}\\Settings\\ExternalTools.xml";

            XmlDocument exToolsConfigXml = new();
            XmlNode docNode = exToolsConfigXml.CreateXmlDeclaration("1.0", "UTF-8", null);
            _ = exToolsConfigXml.AppendChild(docNode);

            XmlNode rootNode = exToolsConfigXml.CreateElement("ExternalTools");
            _ = exToolsConfigXml.AppendChild(rootNode);
            exToolsConfigXml.Save(exToolsConfig);
        }

        public static bool Add(string toolName, string toolPath, string toolOption, bool asAdmin)
        {
            string exToolsConfig = $"{User.CurrentUserDirectoryPath}\\Settings\\ExternalTools.xml";

            XmlDocument exToolsConfigXml = new();
            exToolsConfigXml.Load(exToolsConfig);
            XmlElement? rootNode = exToolsConfigXml.DocumentElement;
            if (rootNode == null)
                throw new InvalidDataException("外部ツール管理ファイルが不正です。外部ツールを追加できませんでした。");

            XmlNode? externalToolNode = rootNode.SelectSingleNode($"//ExternalTool[@Index='{toolName}']");

            if (externalToolNode == null)
            {
                XmlElement externalToolElement = exToolsConfigXml.CreateElement("ExternalTool");
                //属性の新規作成
                XmlAttribute Index = exToolsConfigXml.CreateAttribute("Index");
                Index.Value = toolName;
                //属性をノードに追加
                _ = externalToolElement.Attributes.Append(Index);
                //ノードをRootノードに追加
                _ = rootNode.AppendChild(externalToolElement);

                //Nameノード, Pathノード, Optionノード, Adminノードを作成、追加
                XmlElement exToolName = exToolsConfigXml.CreateElement("Name");
                _ = exToolName.AppendChild(exToolsConfigXml.CreateTextNode(toolName));
                _ = externalToolElement.AppendChild(exToolName);

                XmlElement exToolPath = exToolsConfigXml.CreateElement("Path");
                _ = exToolPath.AppendChild(exToolsConfigXml.CreateTextNode(toolPath));
                _ = externalToolElement.AppendChild(exToolPath);

                XmlElement exToolOption = exToolsConfigXml.CreateElement("Option");
                _ = exToolOption.AppendChild(exToolsConfigXml.CreateTextNode(toolOption));
                _ = externalToolElement.AppendChild(exToolOption);

                XmlElement exToolAdmin = exToolsConfigXml.CreateElement("Admin");
                _ = exToolAdmin.AppendChild(exToolsConfigXml.CreateTextNode(asAdmin.ToString()));
                _ = externalToolElement.AppendChild(exToolAdmin);

                exToolsConfigXml.Save(exToolsConfig);

                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Delete(string toolName)
        {
            string exToolsConfig = $"{User.CurrentUserDirectoryPath}\\Settings\\ExternalTools.xml";

            XmlDocument exToolsConfigXml = new();
            exToolsConfigXml.Load(exToolsConfig);

            //ルート要素の取得
            XmlElement? rootElement = exToolsConfigXml.DocumentElement;
            if (rootElement == null)
                throw new InvalidDataException("外部ツール管理ファイルが不正です。外部ツールを削除できませんでした。");

            XmlNode? node = exToolsConfigXml.SelectSingleNode($"//ExternalTool[@Index='{toolName}']");
            if (node == null)
                throw new InvalidOperationException($"外部ツール '{toolName}' が見つかりません。外部ツールを削除できませんでした。");
            //タグの削除
            _ = rootElement.RemoveChild(node);

            exToolsConfigXml.Save(exToolsConfig);
        }

        public static ExternalTool GetToolInfo(string toolName)
        {
            string exToolsConfig = $"{User.CurrentUserDirectoryPath}\\Settings\\ExternalTools.xml";

            XmlDocument exToolsConfigXml = new();
            exToolsConfigXml.Load(exToolsConfig);

            XmlNode? toolPathNode = exToolsConfigXml.SelectSingleNode($"//ExternalTool[@Index='{toolName}']/Path");
            XmlNode? optionNode = exToolsConfigXml.SelectSingleNode($"//ExternalTool[@Index='{toolName}']/Option");
            XmlNode? adminNode = exToolsConfigXml.SelectSingleNode($"//ExternalTool[@Index='{toolName}']/Admin");

            ExternalTool externalTool = new()
            {
                ToolPath = toolPathNode != null ? toolPathNode.InnerText : string.Empty,
                Option = optionNode != null ? optionNode.InnerText : string.Empty,
                AsAdmin = 
                adminNode != null ? Convert.ToBoolean(adminNode.InnerText) : false
            };

            return externalTool;
        }
    }
}

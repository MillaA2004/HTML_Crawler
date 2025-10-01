using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTMLfile.CustomFeatures;

namespace HTMLfile
{
    public class HTMLmodifier
    {
        //SET realisation

        private static int counter = 0;
        private static string tagName = "";

        static public void SetPath(HTMLelement element, string path, string newContent)
        {
            StrFeatures strF = new StrFeatures(path);
            string[] parts = strF.Split('/');
            SetPath(element, parts, 0, newContent);
        }

        private static void SetPath(HTMLelement element, string[] parts, int depth, string newContent)
        {
            bool isIndexed = false;
            StrFeatures sf = new StrFeatures(parts[depth]);

            if (depth >= parts.Length || element == null)
                return;

            if (depth != parts.Length - 1)
                {
                StrFeatures sf2= new StrFeatures(parts[depth+1]);
                if (sf2.Contains("[") && !sf2.Contains("[@"))
                {
                    string[] tag = sf2.Split('[');
                    tagName = tag[0];
                    parts[depth + 1] = tagName;
                    sf2= new StrFeatures(tag[1]);
                    string indexStr = sf2.Substring(0, sf2.Length() - 1);

                    if (!int.TryParse(indexStr, out int target))
                    {
                        Console.WriteLine("Invalid path in [index].");
                        return;
                    }
                    else
                    {
                        counter = target - 1;
                        isIndexed = true;
                    }
                }
            }

            if (sf.Contains("[@"))
            {
                string[] tag = sf.Split("[@");
                tagName = tag[0];
                sf= new StrFeatures(tag[1]);
                string[] attr = sf.Split('=');
                string attrName = attr[0];
                sf=new StrFeatures(attr[1]);
                string attrValue = sf.Substring(1, sf.Length() - 2);//deviding '] from the atrValue
                if (element.Tag == tagName && element.Attributes.ContainsKey(attrName) && element.Attributes[attrName] == attrValue)
                {
                    if (depth == parts.Length - 1)
                    {
                        UpdateElement(element, newContent);
                    }
                    else
                    {
                        Parallel.ForEach(element.Children, child =>
                        {
                            SetPath(child, parts, depth + 1, newContent);
                        });
                    }
                }
            }
            else if (element.Tag == parts[depth] || parts[depth] == "*")
            {
                if (depth == parts.Length - 1)
                {
                    UpdateElement(element, newContent);
                }
                else
                {
                    foreach (var child in element.Children)
                    {
                        if (isIndexed && child.Tag == tagName)
                        {
                            if (counter == 0)
                            {
                                SetPath(child, parts, depth + 1, newContent);
                                isIndexed = false;
                                break;
                            }
                            else
                            {
                                counter--;
                            }
                        }
                        else
                        {
                            SetPath(child, parts, depth + 1, newContent);
                        }
                    }
                }
            }
        }
        private static void UpdateElement(HTMLelement element, string newContent)
        {
            StrFeatures strFeatures = new StrFeatures(newContent);
            if (strFeatures.Contains("<"))
            {
                HTMLelement newElement = ExtractElem(newContent);
                element.Content = "";
                element.Children.Add(newElement);
            }
            else
            {
                element.Content = newContent;
            }
        }

        private static HTMLelement ExtractElem(string html)
        {
            StrFeatures strFeatures = new StrFeatures(html);
            int tagStart = strFeatures.FindFirstOccurrence("<") + 1;

            int tagEnd = strFeatures.FindFirstOccurrence(">");//, tagStart);
            string tagName = strFeatures.Substring(tagStart, tagEnd);// - tagStart);

            int contentEnd = strFeatures.Length();
            string content = "";

            HTMLelement element = new HTMLelement(tagName);
            int contentStart = tagEnd + 1;
            StrFeatures sf = new StrFeatures(tagName); 
            if (strFeatures.ToString()[sf.Length() + 1] == '/')
            {
                element.Tag = tagName;
                if(!HTMLvalidator.CheckTagName(tagName))
                {
                    Console.WriteLine("Invalid self-closing tag.");
                    return element;
                }
                element._IsSelfClosing = true;

                content = strFeatures.Substring(contentStart, contentEnd);
            }
            else
            {
                contentEnd = strFeatures.FindFirstOccurrence($"</{tagName}>");
                content = strFeatures.Substring(contentStart, contentEnd);
            }
            element.Content = content;

            return element;
        }

    }
}

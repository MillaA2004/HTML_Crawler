using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTMLfile.CustomFeatures;

namespace HTMLfile
{
    public class HTMLvalidator
    {
        private static readonly List<string> selfClosingTags = new List<string>
            {
            "area", "base", "br", "col", "embed", "hr", "img", "input", "link", "meta", "param", "source", "track", "wbr"//selfclosing tags
            };

        public static bool CheckTagName(string tagName)
        {
            return selfClosingTags.Contains(tagName);
        }

        public static bool ValidateHTML(string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                Console.WriteLine("Error: Empty HTML content");
                return false;
            }

            NewStack<TagInfo> tagStack = new NewStack<TagInfo>();
            int position = 0;
            StrFeatures htmlStr = new StrFeatures(html);

            while (position < htmlStr.Length())
            {
                if (html[position] == '<')
                {
                    if (position + 1 < htmlStr.Length() && html[position + 1] == '/')
                    {
                        position += 2; // Skip '</'
                        string closingTag = ReadUntil('>', ref position, htmlStr);
                        StrFeatures tagFeature = new StrFeatures(closingTag);
                        string normalizedTag = tagFeature.ToLower();

                        if (tagStack.Count == 0)
                        {
                            Console.WriteLine($"Error: Found closing tag '{closingTag}' without matching opening tag");
                            return false;
                        }

                        TagInfo lastOpenTag = tagStack.Peek();
                        if (lastOpenTag.Name != normalizedTag)
                        {
                            Console.WriteLine($"Error: Mismatched tags. Expected closing tag for '{lastOpenTag.Name}' but found '{normalizedTag}' at position {position}");
                            return false;
                        }

                        tagStack.Pop();
                    }
                    else
                    {
                        position++; // Skip '<'
                        string tagContent = ReadUntil('>', ref position, htmlStr);
                        StrFeatures contentFeature = new StrFeatures(tagContent.Trim());

                        int spaceIndex = contentFeature.FindFirstOccurrence(" ");
                        string tagName;
                        if (spaceIndex == -1)
                        {
                            tagName = contentFeature.ToString();
                        }
                        else
                        {
                            tagName = contentFeature.Substring(0, spaceIndex);
                        }

                        StrFeatures tagFeature = new StrFeatures(tagName);
                        string normalizedTag = tagFeature.ToLower();

                        // self-closing tags?
                        bool isSelfClosing = CheckTagName(normalizedTag) ||
                                           contentFeature.FindLastOccurrence("/") == contentFeature.Length() - 1;

                        if (!isSelfClosing)
                        {
                            tagStack.Push(new TagInfo { Name = normalizedTag, Position = position });
                        }
                    }
                }
                position++;
            }

            // unclosed tags?
            if (tagStack.Count > 0)
            {
                var unclosedTag = tagStack.Peek();
                Console.WriteLine($"Error: Unclosed tag '{unclosedTag.Name}' | position {unclosedTag.Position}");
                return false;
            }

            return true;
        }

        private static string ReadUntil(char stopChar, ref int position, StrFeatures htmlStr)
        {
            int start = position;
            while (position < htmlStr.Length() && htmlStr.ToString()[position] != stopChar)
            {
                position++;
            }

            if (position - start <= 0)
                return string.Empty;

            return htmlStr.Substring(start, position).Trim();
        }

        private class TagInfo
        {
            public string Name { get; set; }
            public int Position { get; set; }
        }

        public static bool ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            StrFeatures pathStr = new StrFeatures(path);

            string[] parts = pathStr.Split('/');
            if (parts.Length < 2) return false;

            for (int i = 2; i < parts.Length; i++) 
            {
                StrFeatures part = new StrFeatures(parts[i]);

                if (part.Length() == 0)
                    return false;
                if(part.Contains("[@"))
                {
                    int openBracket = part.FindFirstOccurrence("[@");
                    int closeBracket = part.FindFirstOccurrence("]");

                    if (openBracket == -1 || closeBracket == -1 || openBracket >= closeBracket)
                        return false;

                    string attributePart = part.Substring(openBracket + 2, closeBracket - openBracket);
                    if (!attributePart.Contains("="))
                        return false;

                    string[] attributeParts = new StrFeatures(attributePart).Split('=');
                    if (attributeParts.Length != 2)
                        return false;

                    string attributeName = attributeParts[0].Trim();
                    string attributeValue = attributeParts[1].Trim('\'', '"');

                    if (string.IsNullOrEmpty(attributeName) || string.IsNullOrEmpty(attributeValue))
                        return false;

                    string tagName = part.Substring(0, openBracket);
                    if (new StrFeatures(tagName).Length() == 0)
                        return false;
                }
            else
                if (part.Contains("["))
                {
                    if (!part.Contains("]"))
                        return false;

                    int openBracket = part.FindFirstOccurrence("[");
                    int closeBracket = part.FindFirstOccurrence("]");

                    if (openBracket >= closeBracket || closeBracket != part.Length() - 1)
                        return false;
                    string indexPart = part.Substring(openBracket + 1, closeBracket);
                    foreach (char c in indexPart)
                    {
                        if (c < '0' || c > '9')
                            return false;
                    }
                    string tagName = part.Substring(0, openBracket);
                    StrFeatures tagNameFeature = new StrFeatures(tagName);
                    if (tagNameFeature.Length() == 0)
                        return false;
                }
            }

            return true;
        }

    }
}

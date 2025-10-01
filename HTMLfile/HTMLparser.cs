using System;
using System.Collections.Generic;
using HTMLfile.CustomFeatures;

namespace HTMLfile
{
    public class HTMLparser
    {
        private StrFeatures htmlText;
        private int position = 6;

        public HTMLparser(StrFeatures html)
        {
            htmlText = html;
        }

        public HTMLelement Parse()
        {
            HTMLelement root = new HTMLelement("html");
            NewStack<HTMLelement> stack = new(); //Stack<HTMLelement> stack = new Stack<HTMLelement>();
            stack.Push(root);

            while (position < htmlText.Length())
            {
                if (htmlText.ToString()[position] == '<')
                {
                    if (htmlText.ToString()[position + 1] == '/')
                    {
                        position = MoveTo('>', position) + 1; // closing tag
                        stack.Pop();
                    }
                    else
                    {
                        position++;
                        string tagContent = ReadUntil('>', ref position);

                        bool isSelfClosing = tagContent.EndsWith("/") || HTMLvalidator.CheckTagName(tagContent);

                        int spaceIndex = FindFirstSpace(tagContent);
                        string tagName = "";

                        if (isSelfClosing && spaceIndex == -1)
                        {
                            tagName = new StrFeatures(tagContent).Substring(0, tagContent.Length - 1); // removing the '/' at the end like in <br/>
                        }
                        else
                        {
                            tagName = spaceIndex == -1 ? tagContent : new StrFeatures(tagContent).Substring(0, spaceIndex);
                        }

                        HTMLelement element = new HTMLelement(tagName)
                        {
                            _IsSelfClosing = isSelfClosing
                        };

                        if (spaceIndex != -1)
                        {
                            string attributesPart = isSelfClosing ? new StrFeatures(tagContent).Substring(spaceIndex + 1, tagContent.Length - 1).Trim() : new StrFeatures(tagContent).Substring(spaceIndex + 1).Trim();
                            ParseAttributes(attributesPart, element);
                        }

                        stack.Peek().Children.Add(element);

                        if (!isSelfClosing)
                        {
                            stack.Push(element);
                        }
                    }
                }
                else
                {
                    // content
                    position++; // skip the '>'
                    string text = ReadUntil('<', ref position).Trim();
                    if (!string.IsNullOrEmpty(text))
                    {
                        stack.Peek().Content += text;
                    }
                }
            }

            return root;
        }

        private string ReadUntil(char stopChar, ref int startIndex)
        {
            int start = startIndex;
            while (startIndex < htmlText.Length() && htmlText.ToString()[startIndex] != stopChar)
            {
                startIndex++;
            }
            return htmlText.Substring(start, startIndex);
        }

        private int MoveTo(char target, int startIndex)
        {
            while (startIndex < htmlText.Length() && htmlText.ToString()[startIndex] != target)
            {
                startIndex++;
            }
            return startIndex;
        }

        private int FindFirstSpace(string input)
        {
            StrFeatures strFeature = new StrFeatures(input);
            for (int i = 0; i < strFeature.Length(); i++)
            {
                if (strFeature.ToString()[i] == ' ')
                    return i;
            }
            return -1;
        }

        private void ParseAttributes(string attributesText, HTMLelement element)
        {
            StrFeatures strFeature = new StrFeatures(attributesText);
            int i = 0;
            while (i < strFeature.Length())
            {
                int start = i;
                while (i < strFeature.Length() && strFeature.ToString()[i] != '=')
                {
                    i++;
                } // attribute name

                string attributeName = strFeature.Substring(start, i).Trim();

                i++; // Skip '='

                int m = 1; // parameter for " symbol after the '='
                if (strFeature.ToString()[i] == '"') m = 0;
                i++; // Skip ' or "
                start = i;

                while (i < strFeature.Length() && strFeature.ToString()[i] != '"')
                {
                    i++;
                }

                string attributeValue = strFeature.Substring(start, i - m);
                element.Attributes[attributeName] = attributeValue;
                i++; // skip "
            }
        }
    }
}


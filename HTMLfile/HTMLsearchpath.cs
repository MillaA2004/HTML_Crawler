using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTMLfile.CustomFeatures;

namespace HTMLfile
{
    public class HTMLsearchpath
    {

        //PRINT realisation
        private static int counter = 0;
        private static string tagName = "";
        //private static StrFeatures strF = new StrFeatures(tagName);


        static public void PrintPath(HTMLelement element, string path)
        {
            StrFeatures strF = new StrFeatures(path);
            string[] parts = strF.Split('/');
            PrintPath(element, parts, 0);
        }

        private static void PrintPath(HTMLelement element, string[] parts, int depth)
        {
            bool isIndexed = false;
            StrFeatures sf = new StrFeatures(parts[depth]);

            if (depth >= parts.Length || element == null)
                return;

            if (depth != parts.Length - 1)
            {
                StrFeatures sf2 = new StrFeatures(parts[depth + 1]);
                if (sf2.Contains("[") && !sf2.Contains("[@"))//checking incoming path part for [index]
                {
                    string[] tag = sf2.Split('[');
                    tagName = tag[0];
                    parts[depth + 1] = tagName;
                    sf2 = new StrFeatures(tag[1]);
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
                //StrFeatures strF = new StrFeatures(parts[depth]);
                string[] tag = sf.Split("[@");
                tagName = tag[0];
                sf = new StrFeatures(tag[1]);
                if (!sf.Contains("="))
                {
                    Console.WriteLine("Invalid path in [@attribute].");
                    return;
                }
                string[] attr = sf.Split('=');
                string attrName = attr[0];
                sf = new StrFeatures(attr[1]);
                string attrValue = sf.Substring(1, sf.Length() - 2);
                if (element.Tag == tagName && element.Attributes.ContainsKey(attrName) && element.Attributes[attrName] == attrValue)
                {
                    if (depth == parts.Length - 1)
                    {
                        PrintTree(element, depth);
                    }
                    else
                    {
                        Parallel.ForEach(element.Children, child =>//foreach (var child in element.Children)
                        {
                            PrintPath(child, parts, depth + 1);
                        });
                    }
                }
            }
            else if (element.Tag == parts[depth] || parts[depth] == "*")
            {
                if (depth == parts.Length - 1)
                {
                    PrintTree(element, depth);
                }
                else
                {
                    foreach (var child in element.Children)
                    {
                        if (isIndexed && child.Tag == tagName)
                        {
                            if (counter == 0)
                            {
                                PrintPath(child, parts, depth + 1);
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
                            PrintPath(child, parts, depth + 1);
                        }
                    }
                }
            }

        }

        static public void PrintTree(HTMLelement element, int indent)
        {
            string indentation = new string(' ', indent * 2);


            if (element._IsSelfClosing && !HTMLvalidator.CheckTagName(element.Tag))
            {
                Console.WriteLine($"<{element.Tag}/> is presented as a self-closing tag.");
                //throw new Exception($"Unexpected value: {element.Tag}");
            }

            if (element._IsSelfClosing)
                Console.WriteLine($"{indentation}<{element.Tag}/>");
            else
                Console.WriteLine($"{indentation}<{element.Tag}>");

            foreach (var attr in element.Attributes)
            {
                Console.WriteLine($"{indentation} Atributes: {attr.Key}=\"{attr.Value}\"");
            }

            if (!string.IsNullOrEmpty(element.Content))
            {
                Console.WriteLine($"{indentation} Content: {element.Content}");
            }

            foreach (var child in element.Children)
            {
                PrintTree(child, indent + 1);
            }
            if (!element._IsSelfClosing)
                Console.WriteLine($"{indentation}</{element.Tag}>");
        }


        //COPY realisation
        static public void CopyPath(HTMLelement root, string sourcePath, string targetPath)
        {
            HTMLelement sourceElement = FindElementByPath(root, sourcePath);
            HTMLelement targetElement = FindElementByPath(root, targetPath);

            if (sourceElement == null)
            {
                Console.WriteLine("Source element not found.");
                return;
            }

            if (targetElement == null)
            {
                Console.WriteLine("Target element not found.");
                return;
            }

            if (targetElement.Children == null)
            {
                targetElement.Children = new List<HTMLelement>();
            }

            HTMLelement copiedElement = sourceElement.DeepCopy();
            targetElement.Children.Add(copiedElement);
        }

        static private HTMLelement FindElementByPath(HTMLelement root, string path)
        {
            StrFeatures strF = new StrFeatures(path);
            string[] parts = strF.Split('/');
            return FindElementByPath(root, parts, 0);
        }

        static private HTMLelement FindElementByPath(HTMLelement element, string[] parts, int depth)
        {
            bool isIndexed = false;

            if (depth >= parts.Length || element == null)
                return null;

            if (depth != parts.Length - 1)
            {
                StrFeatures sf = new StrFeatures(parts[depth + 1]);
                if (sf.Contains("[") && !sf.Contains("[@"))
                {
                    string[] tag = sf.Split('[');
                    tagName = tag[0];
                    parts[depth + 1] = tagName;
                    StrFeatures indexStrF = new StrFeatures(tag[1]);
                    string indexStr = indexStrF.Substring(0, indexStrF.Length() - 1);

                    if (!int.TryParse(indexStr, out int target))
                    {
                        Console.WriteLine("Invalid path in [index].");
                        return null;
                    }
                    else
                    {
                        counter = target - 1;
                        isIndexed = true;
                    }
                }
            }

            StrFeatures currentPartSF = new StrFeatures(parts[depth]);

            if (currentPartSF.Contains("[@"))
            {
                string[] tag = currentPartSF.Split("[@");
                tagName = tag[0];
                StrFeatures attrSF = new StrFeatures(tag[1]);
                if (!attrSF.Contains("="))
                {
                    Console.WriteLine("Invalid path in [@attribute].");
                    return null;
                }
                string[] attr = attrSF.Split('=');
                string attrName = attr[0];
                StrFeatures attrValueSF = new StrFeatures(attr[1]);
                string attrValue = attrValueSF.Substring(1, attrValueSF.Length() - 2);

                if (element.Tag == tagName && element.Attributes.ContainsKey(attrName) && element.Attributes[attrName] == attrValue)
                {
                    if (depth == parts.Length - 1)
                    {
                        return element;
                    }
                    else
                    {
                        foreach (var child in element.Children)
                        {
                            HTMLelement foundElement = FindElementByPath(child, parts, depth + 1);
                            if (foundElement != null)
                            {
                                return foundElement;
                            }
                        }
                    }
                }
            }
            else if (element.Tag == parts[depth] || parts[depth] == "*")
            {
                if (depth == parts.Length - 1)
                {
                    return element;
                }
                else
                {
                    foreach (var child in element.Children)
                    {
                        if (isIndexed && child.Tag == tagName)
                        {
                            if (counter == 0)
                            {
                                HTMLelement foundElement = FindElementByPath(child, parts, depth + 1);
                                if (foundElement != null)
                                {
                                    return foundElement;
                                }
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
                            HTMLelement foundElement = FindElementByPath(child, parts, depth + 1);
                            if (foundElement != null)
                            {
                                return foundElement;
                            }
                        }
                    }
                }
            }

            return null;
        }        
    }
}


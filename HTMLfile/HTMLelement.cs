using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HTMLfile.CustomFeatures;

namespace HTMLfile
{
    public class HTMLelement
    {
        public string Tag { get; set; }
        public NewDictionary<string, string> Attributes { get; set; } = new NewDictionary<string, string>();
        public string Content { get; set; } = string.Empty;
        public List<HTMLelement> Children { get; set; } = new List<HTMLelement>();
        public bool _IsSelfClosing { get; set; }

        public HTMLelement(string tag)
        {
            Tag = tag;
            Content = string.Empty;
            Attributes = new NewDictionary<string, string>();
            Children = new List<HTMLelement>();
            _IsSelfClosing = false;
        }

        public HTMLelement(HTMLelement copiedElement)
        {
            Tag = copiedElement.Tag;
            Content = copiedElement.Content;
            _IsSelfClosing = copiedElement._IsSelfClosing;
            Attributes = new NewDictionary<string, string>();
            Children = copiedElement.Children != null
            ? copiedElement.Children.Select(child => new HTMLelement(child)).ToList()
            :   new List<HTMLelement>();
        }
        public HTMLelement DeepCopy()
        {
            return new HTMLelement(this);
        }
    }
}

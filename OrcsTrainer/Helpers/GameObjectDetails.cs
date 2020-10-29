
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trainer
{
    [Serializable]
    public class GameObjectDetails
    {
        #region[Declarations]

        public string name = "";
        public string parent = "";
        public bool enabled = false;
        public int layer = -1;
        public string position = "";
        public string localPosition = "";
        public List<string> components = new List<string>();
        public List<GameObjectDetails> children = new List<GameObjectDetails>();

        #endregion

        public GameObjectDetails() { }

        public GameObjectDetails(GameObject rootObject)
        {
            this.name = rootObject.name;

            if (rootObject.transform.parent != null)
            {
                this.parent = rootObject.transform.parent.gameObject.name;
            }
            else
            {
                this.parent = "null";
            }

            this.enabled = rootObject.activeSelf;
            this.layer = rootObject.layer;
            this.position = rootObject.transform.position.ToString();
            this.localPosition = rootObject.transform.localPosition.ToString();

            var tmpComps = rootObject.GetComponents<Component>();
            foreach(var comp in tmpComps)
            {
                components.Add(comp.GetIl2CppType().FullName);
            }

            var childCount = rootObject.transform.childCount;
            for (int idx = 0; idx < childCount; idx++)
            {
                this.children.Add(new GameObjectDetails(rootObject.transform.GetChild(idx).gameObject));
            }
        }

        public static string XMLSerialize(List<GameObjectDetails> objectTree)
        {
            string xml = "<?xml version=\"1.0\"?>\r\n";

            xml += "<GameObjects>\r\n";
            xml += "<Count value=\"" + objectTree.Count.ToString() + "\" />\r\n";
            foreach(var obj in objectTree)
            {
                xml += "<GameObject name=\"" + obj.name + "\">\r\n";
                xml += CreateXMLGameObject(obj);
                xml += "</GameObject>\r\n";
            }

            xml += "</GameObjects>";

            return xml;
        }

        private static string CreateXMLGameObject(GameObjectDetails obj)
        {
            string xml = "";

            xml += "<parent name=\"" + obj.parent + "\" />\r\n";
            xml += "<enabled value=\"" + obj.enabled.ToString() + "\" />\r\n";
            xml += "<layer value=\"" + obj.layer.ToString() + "\" />\r\n";

            xml += "<components>\r\n";
            foreach (var comp in obj.components)
            {
                xml += "<component name=\"" + comp + "\" />\r\n";
            }
            xml += "</components>\r\n";

            if (obj.children.Count > 0)
            {
                xml += "<children>\r\n";
                foreach (GameObjectDetails child in obj.children)
                {
                    xml += "<child name=\"" + child.name + "\">\r\n";
                    xml += CreateXMLGameObject(child);
                    xml += "</child>\r\n";
                }
                xml += "</children>\r\n";
            }

            return xml;
        }
    }
}

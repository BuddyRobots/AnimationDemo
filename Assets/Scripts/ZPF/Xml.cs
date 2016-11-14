using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace MagicCircuit
{
    public class XmlCircuitItem                 //图标管理类 id,名字，类型，坐标
    {
        [XmlAttribute("ID")]
        public int ID;
        public string name;
        public ItemType type;                   //图标类型
        public List<Vector3> list;              //图标的坐标
        public double theta;                    //图标的朝向（单位：角度）
        public int showOrder;                   //显示顺序 从0开始（图标的显示顺序是灯泡）
        public bool powered;                    //元件是否通电
        public Vector2 connect_left;            //Connect point on card
        public Vector2 connect_right;
		public Vector2 connect_middle;

        public XmlCircuitItem()
        {}

        public XmlCircuitItem(CircuitItem src)
        {
            ID = src.ID;
            name = src.name;
            type = src.type;
            list = src.list;
            theta = src.theta;
            showOrder = src.showOrder;
            powered = src.powered;
            connect_left = src.connect_left;
            connect_right = src.connect_right;
			connect_middle = src.connect_middle;
        }
    }

    [XmlRoot("XmlCircuitItemCollection")]
    public class XmlCircuitItemCollection
    {
        [XmlArray("XmlCircuitItems")]
        [XmlArrayItem("XmlCircuitItem")]
        public List<XmlCircuitItem> xmlCircuitItems = new List<XmlCircuitItem>();


        public void Save(string path)
        {
            var serializer = new XmlSerializer(typeof(XmlCircuitItemCollection));
            using (var stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        public static XmlCircuitItemCollection Load(string path)
        {
            var serializer = new XmlSerializer(typeof(XmlCircuitItemCollection));
			using (var stream = new FileStream(path, FileMode.Open,FileAccess.Read))
            {
                return serializer.Deserialize(stream) as XmlCircuitItemCollection;
            }
        }

        public List<CircuitItem> toCircuitItems()
        {
            List<CircuitItem> res = new List<CircuitItem>();

            for(var i = 0; i < xmlCircuitItems.Count; i++)
            {
                res.Add(new CircuitItem(xmlCircuitItems[i]));
            }
            return res;
        }
    }
}
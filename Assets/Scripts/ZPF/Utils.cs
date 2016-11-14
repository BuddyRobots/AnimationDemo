using UnityEngine;
using OpenCVForUnity;
using System.Collections.Generic;

namespace MagicCircuit
{
    public enum ItemType
    {
        Battery,              // class 1
		Switch,               // class 2
		LightActSwitch,       // class 3
		VoiceOperSwitch,      // class 4
		VoiceTimedelaySwitch, // class 5
		SPDTSwitch,           // class 6
		Bulb,		          // class 7
		Loudspeaker,          // class 8
		InductionCooker,      // class 9

        CircuitLine
    }
    
	public class CircuitItem                        //图标管理类 (id,名字，类型，坐标)
    {
        public int           ID        {get; set;}
        public string        name      {get; set;}
        public ItemType      type      {get; set;}  //图标类型
        public List<Vector3> list      {get; set;}  //图标的坐标
		public double        theta     {get; set;}  //图标的朝向（单位：角度）
		public int           showOrder {get; set;}  //显示顺序 从0开始（图标的显示顺序是灯泡）
		public bool          powered   {get; set;}  //元件是否通电

		public enum PowerStatus
		{
			noBattery,
			oneBattery,
			twoBattery
		}
		[HideInInspector]
		public PowerStatus powerStatus = PowerStatus.noBattery;
			
        public Vector2 connect_left;            // Connect point on card
        public Vector2 connect_right;			// For lines : connect_left == start, connect_right == end
		public Vector2 connect_middle;	

		public CircuitItem()
        {}

        // @Override
        public CircuitItem(int _id, string _name, ItemType _type, int _order, bool _p = false)
        {
            ID = _id;
            name = _name;
            type = _type;
            showOrder = _order;
            powered = _p;
            list = new List<Vector3>();
            theta = 0;

            connect_left = new Vector2();
            connect_right = new Vector2();
			connect_middle= new Vector2();
        }

        // @Override  
        // Constructor for reading in Xml		
        public CircuitItem(XmlCircuitItem src)
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
			connect_middle= src.connect_middle;
        }		

        // @Override
        public void extractCard(int direction, List<Point> outer_square)
        {
			Vector2 centerFrameImg = new Vector2((float)(outer_square[0].x + outer_square[2].x) / 2, (float)(outer_square[0].y + outer_square[2].y) / 2);

			Vector3 centerCamQuad = cordinateMat2Unity(centerFrameImg.x, centerFrameImg.y);
				
			list.Add(centerCamQuad);

            double angle = Mathf.Atan((float)(outer_square[0].y - outer_square[1].y) / (float)(outer_square[1].x - outer_square[0].x));

            theta = Mathf.PI / 2 * direction + angle; // 0 < theta < 2 * PI

            // Calculate connect_left & connect_right
            float width = Mathf.Sqrt(Mathf.Pow((float)(outer_square[0].x - outer_square[1].x), 2) + Mathf.Pow((float)(outer_square[0].y - outer_square[1].y), 2));

            float dx = width / 2 * Mathf.Cos((float)theta);
			float dy = width / 2 * Mathf.Sin((float)theta);

			connect_left   = new Vector2(centerCamQuad.x - dx, centerCamQuad.y - dy);
			connect_right  = new Vector2(centerCamQuad.x + dx, centerCamQuad.y + dy);
			connect_middle = new Vector2(centerCamQuad.x + dy, centerCamQuad.y - dx);

            theta = theta * Mathf.Rad2Deg; // 0 < theta < 360
        }

        public void extractLine(List<Point> line, OpenCVForUnity.Rect rect)
        {
            Point center = new Point(rect.tl().x, rect.tl().y);

            for (var i = 0; i < line.Count; i++)
            {
				list.Add(cordinateMat2Unity((float)(line[i].x + center.x), (float)(line[i].y + center.y)));
            }
			connect_left  = new Vector2(list[0].x, list[0].y);
            connect_right = new Vector2(list[list.Count - 1].x, list[list.Count - 1].y);
        } 

        private Vector3 cordinateMat2Unity(float x, float y)
        {            
			return new Vector3(x + Constant.CAM_QUAD_ORIGINAL_POINT_X, Constant.CAM_QUAD_ORIGINAL_POINT_Y - y);
        }
    }
}
 
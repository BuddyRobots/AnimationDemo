using UnityEngine;
using System.Collections.Generic;
using OpenCVForUnity;
using LitJson;


namespace AnimationDemo
{
	abstract class BodyPart
	{
		public Texture2D texture;
		public List<Vector2> position;
		public List<double>  rotation;

		protected readonly int NUM_OF_FRAME;

		protected Mat originMask;
		protected OpenCVForUnity.Rect minimalBB;

		// Animation template offset & vector
		protected List<Vector2> animeOffset;
		protected List<Vector2> animeVector;
		// Player drawing image offset & vector
		protected List<Vector2> imageOffset;
		protected List<Vector2> imageVector;

		protected Vector2 centerPoint;
		protected Vector2 anchorPoint;


		protected BodyPart(Texture2D _texture, Mat _originMask, OpenCVForUnity.Rect _minimalBB, string _jsonPath)
		{
			position = new List<Vector2>();
			rotation = new List<double>();
			animeOffset = new List<Vector2>();
			animeVector = new List<Vector2>();
			centerPoint = new Vector2();
			anchorPoint = new Vector2();
			imageOffset = new List<Vector2>();
			imageVector = new List<Vector2>();

			texture = _texture;
			originMask = _originMask;
			minimalBB = _minimalBB;

			NUM_OF_FRAME = calcAnimation(_jsonPath);
			findCenterPoint();
		}


		private int calcAnimation(string jsonPath)
		{
			var asset = Resources.Load<TextAsset>(jsonPath);

			JsonData data = JsonMapper.ToObject(asset.text);
			JsonData offset_x_data = data["offset_x"];
			JsonData offset_y_data = data["offset_y"];
			JsonData vector_x_data = data["vector_x"];
			JsonData vector_y_data = data["vector_y"];
			JsonData rotation_data = data["rotation"];

			int num_of_frame = rotation_data.Count;
			for (var i = 0; i < num_of_frame; i++)
			{
				animeOffset.Add(new Vector2((int)offset_x_data[i], (int)offset_y_data[i]));
				animeVector.Add(new Vector2((int)vector_x_data[i], (int)vector_y_data[i]));
				rotation.Add((int)rotation_data[i]);
			}
			return num_of_frame;
		}


		protected void findCenterPoint()
		{
			float x = (float)(minimalBB.tl().x + minimalBB.br().x)/2;
			float y = (float)(minimalBB.tl().y + minimalBB.br().y)/2;
			centerPoint = new Vector2(x, y);
		}


		public void calcImageOffset(Vector2 parentCenter)
		{
			for (var i = 0; i < NUM_OF_FRAME; i++)
				imageOffset.Add(new Vector2((anchorPoint.x - parentCenter.x), (parentCenter.y - anchorPoint.y)));
		}


		public void calcImageVector()
		{
			Vector2 initImageVector = new Vector2((centerPoint.x - anchorPoint.x), (anchorPoint.y - centerPoint.y));

			float ratio = 1;
			float initAnimeVectorMagnitude = animeVector[0].magnitude;
			if (initAnimeVectorMagnitude != 0)
				ratio = initImageVector.magnitude / initAnimeVectorMagnitude;

			for (var i = 0; i < NUM_OF_FRAME; i++)
				imageVector.Add(new Vector2(animeVector[i].x*ratio, animeVector[i].y*ratio));
		}


		public void calcPosition(List<Vector2> parentPosition)
		{
			for (var i = 0; i < NUM_OF_FRAME; i++)
				position.Add(parentPosition[i] + imageOffset[i] + imageVector[i]);
		}
	}


	class Body : BodyPart
	{
		public Body(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb, string _jsonPath)
			: base(_texture, _mask, _bb, _jsonPath)
		{
			findAnchorPoint();
		}


		private void findAnchorPoint()
		{
			anchorPoint = centerPoint;
		}


		public Vector2 getCenterPoint()
		{
			return centerPoint;
		}


		public void calcPosition(Point originPoint)
		{
			// TODO simplify this!
			for (var i = 0; i < NUM_OF_FRAME; i++)
			{
				Vector2 imagePosition = new Vector2((float)originPoint.x + imageVector[i].x, (float)originPoint.y - imageVector[i].y) + centerPoint + imageOffset[i]/* + imageVector[i]*/;
				Vector2 camQuadPosition = transform(imagePosition);
				position.Add(camQuadPosition);
			}				
		}


		private Vector2 transform(Vector2 imagePosition)
		{
			Vector2 result = new Vector2(imagePosition.x - 305, 375 - imagePosition.y);
			return result;
		}
	}


	class LeftWing : BodyPart
	{
		public LeftWing(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb, string _jsonPath)
			: base(_texture, _mask, _bb, _jsonPath)
		{
			findAnchorPoint();
		}


		private void findAnchorPoint()
		{
			int top    = (int)minimalBB.tl().y;
			int bottom = (int)minimalBB.br().y;
			int right  = (int)minimalBB.br().x - 1;
			List<int> yList = new List<int>();
			for (var i = top; i < bottom; i++)
				if (originMask.get(i, right)[0] > 200)
					yList.Add(i);
			if (yList.Count == 0)
				Debug.Log("Owl.cs RightWing findAnchorPoint() : did not find anchorPoint!!");
			anchorPoint = new Vector2(right, MyUtils.average(yList));
		}
	}


	class RightWing : BodyPart
	{
		public RightWing(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb, string _jsonPath)
			: base(_texture, _mask, _bb, _jsonPath)
		{
			findAnchorPoint();
		}


		private void findAnchorPoint()
		{
			int top    = (int)minimalBB.tl().y;
			int bottom = (int)minimalBB.br().y;
			int left   = (int)minimalBB.tl().x;
			List<int> yList = new List<int>();
			for (var i = top; i < bottom; i++)
				if (originMask.get(i, left)[0] > 200)
					yList.Add(i);
			if (yList.Count == 0)
				Debug.Log("Owl.cs RightWing findAnchorPoint() : did not find anchorPoint!!");
			anchorPoint = new Vector2(left, MyUtils.average(yList));
		}
	}


	class LeftLeg : BodyPart
	{
		public LeftLeg(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb, string _jsonPath)
			: base(_texture, _mask, _bb, _jsonPath)
		{
			findAnchorPoint();
		}


		private void findAnchorPoint()
		{
			int top   = (int)minimalBB.tl().y;
			int left  = (int)minimalBB.tl().x;
			int right = (int)minimalBB.br().x;
			List<int> xList = new List<int>();
			for (var j = left; j < right; j++)
				if (originMask.get(top, j)[0] > 200)
					xList.Add(j);
			if (xList.Count == 0)
				Debug.Log("Owl.cs LeftLeg findAnchorPoint() : did not find anchorPoint!!");
			anchorPoint = new Vector2(MyUtils.average(xList), top);
		}
	}


	class RightLeg : BodyPart
	{
		public RightLeg(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb, string _jsonPath)
			: base(_texture, _mask, _bb, _jsonPath)
		{
			findAnchorPoint();
		}


		private void findAnchorPoint()
		{
			int top   = (int)minimalBB.tl().y;
			int left  = (int)minimalBB.tl().x;
			int right = (int)minimalBB.br().x;
			List<int> xList = new List<int>();
			for (var j = left; j < right; j++)
				if (originMask.get(top, j)[0] > 200)
					xList.Add(j);
			if (xList.Count == 0)
				Debug.Log("Owl.cs RightLeg findAnchorPoint() : did not find anchorPoint!!");
			anchorPoint = new Vector2(MyUtils.average(xList), top);
		}
	}		
}
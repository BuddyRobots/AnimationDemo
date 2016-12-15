using UnityEngine;
using System;
using System.Collections.Generic;
using LitJson;
using OpenCVForUnity;
using UnityEditor;


namespace AnimationDemo
{
	class Owl
	{
		public Body      body;
		public LeftWing  leftWing;
		public RightWing rightWing;
		public LeftLeg   leftLeg;
		public RightLeg  rightLeg;

		private List<Texture2D>           partTexList;
		private List<Mat>                 partMaskList;
		private List<OpenCVForUnity.Rect> partBBList;

		public Owl(Texture2D _owlTexture, List<string> _jsonPaths)
		{
			partTexList  = new List<Texture2D>();
			partMaskList = new List<Mat>();
			partBBList   = new List<OpenCVForUnity.Rect>();

			Segmentation.segment(_owlTexture, out partTexList, out partMaskList, out partBBList);

			body      = new Body     (partTexList[0], partMaskList[0], partBBList[0]);
			leftWing  = new LeftWing (partTexList[1], partMaskList[1], partBBList[1]);
			rightWing = new RightWing(partTexList[2], partMaskList[2], partBBList[2]);
			leftLeg   = new LeftLeg  (partTexList[3], partMaskList[3], partBBList[3]);
			rightLeg  = new RightLeg (partTexList[4], partMaskList[4], partBBList[4]);

			calcPartAnimation(_jsonPaths);
			calcOffset();
			calcPosition();
		}


		private void calcPartAnimation(List<string> jsonPaths)
		{
			if (jsonPaths.Count != 5)
			{
				Debug.LogError("Owl.cs calcAnimation() : Wrong number of json files passed! " +
					"Expect 5, Recieved " + jsonPaths.Count);
				return;
			}

			body.calcAnimation(jsonPaths[0]);
			leftWing.calcAnimation(jsonPaths[1]);
			rightWing.calcAnimation(jsonPaths[2]);
			leftLeg.calcAnimation(jsonPaths[3]);
			rightLeg.calcAnimation(jsonPaths[4]);
		}


		private void calcOffset()
		{
			body.calcOffset(body.getCenterPoint());
			leftWing.calcOffset(body.getCenterPoint());
			rightWing.calcOffset(body.getCenterPoint());
			leftLeg.calcOffset(body.getCenterPoint());
			rightLeg.calcOffset(body.getCenterPoint());
		}


		private void calcPosition()
		{
			body.calcPosition();
			leftWing.calcPosition();
			rightWing.calcPosition();
			leftLeg.calcPosition();
			rightLeg.calcPosition();
		}
	}


	class Body : BodyPart
	{
		public Body(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb)
			: base(_texture, _mask, _bb)
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
	}


	class LeftWing : BodyPart
	{
		public LeftWing(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb)
			: base(_texture, _mask, _bb)
		{
			findAnchorPoint();
		}


		private void findAnchorPoint()
		{
			int top    = (int)bb.tl().y;
			int bottom = (int)bb.br().y;
			int right  = (int)bb.br().x;
			List<int> yList = new List<int>();
			for (var i = top; i < bottom; i++)
				if (mask.get(i, right)[0] > 200)
					yList.Add(i);
			if (yList.Count == 0)
				Debug.Log("Owl.cs LeftWing findAnchorPoint() : did not find anchorPoint!!");
			anchorPoint = new Vector2(right, MyUtils.average(yList));
		}
	}


	class RightWing : BodyPart
	{
		public RightWing(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb)
			: base(_texture, _mask, _bb)
		{
			findAnchorPoint();
		}


		private void findAnchorPoint()
		{
			int top    = (int)bb.tl().y;
			int bottom = (int)bb.br().y;
			int left   = (int)bb.tl().x;
			List<int> yList = new List<int>();
			for (var i = top; i < bottom; i++)
				if (mask.get(i, left)[0] > 200)
					yList.Add(i);
			if (yList.Count == 0)
				Debug.Log("Owl.cs RightWing findAnchorPoint() : did not find anchorPoint!!");
			anchorPoint = new Vector2(left, MyUtils.average(yList));
		}
	}


	class LeftLeg : BodyPart
	{
		public LeftLeg(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb)
			: base(_texture, _mask, _bb)
		{
			findAnchorPoint();
		}


		private void findAnchorPoint()
		{
			int top   = (int)bb.tl().y;
			int left  = (int)bb.tl().x;
			int right = (int)bb.br().x;
			List<int> xList = new List<int>();
			for (var j = left; j < right; j++)
				if (mask.get(top, j)[0] > 200)
					xList.Add(j);
			if (xList.Count == 0)
				Debug.Log("Owl.cs LeftLeg findAnchorPoint() : did not find anchorPoint!!");
			anchorPoint = new Vector2(MyUtils.average(xList), top);
		}
	}


	class RightLeg : BodyPart
	{
		public RightLeg(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb)
			: base(_texture, _mask, _bb)
		{
			findAnchorPoint();
		}


		private void findAnchorPoint()
		{
			int top   = (int)bb.tl().y;
			int left  = (int)bb.tl().x;
			int right = (int)bb.br().x;
			List<int> xList = new List<int>();
			for (var j = left; j < right; j++)
				if (mask.get(top, j)[0] > 200)
					xList.Add(j);
			if (xList.Count == 0)
				Debug.Log("Owl.cs RightLeg findAnchorPoint() : did not find anchorPoint!!");
			anchorPoint = new Vector2(MyUtils.average(xList), top);
		}
	}


	abstract class BodyPart
	{
		public Texture2D texture;
		public List<Vector2> position;
		public List<double>  rotation;
		protected List<Vector2> animePosition;

		protected Mat mask;
		protected OpenCVForUnity.Rect bb;

		// Animation template offset & vector
		protected List<Vector2> animeOffset;
		protected List<Vector2> animeVector;
		// Player drawing image offset & vector
		protected List<Vector2> imageOffset;
		protected List<Vector2> imageVector;

		protected Vector2 centerPoint;
		protected Vector2 anchorPoint;


		protected BodyPart(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb)
		{
			position = new List<Vector2>();
			rotation = new List<double>();
			animePosition = new List<Vector2>();
			animeOffset = new List<Vector2>();
			animeVector = new List<Vector2>();
			centerPoint = new Vector2();
			anchorPoint = new Vector2();

			texture = _texture;
			mask = _mask;
			bb = _bb;

			findCenterPoint();
		}


		public virtual void calcAnimation(string jsonPath)
		{
			var asset = Resources.Load<TextAsset>(jsonPath);

			JsonData data = JsonMapper.ToObject(asset.text);
			JsonData offset_x_data = data["offset_x"];
			JsonData offset_y_data = data["offset_y"];
			JsonData vector_x_data = data["vector_x"];
			JsonData vector_y_data = data["vector_y"];
			JsonData rotation_data = data["rotation"];

			for (var i = 0; i < vector_x_data.Count; i++)
			{
				animeOffset.Add(new Vector2((int)offset_x_data[i], (int)offset_y_data[i]));
				animeVector.Add(new Vector2((int)vector_x_data[i], (int)vector_y_data[i]));
				animePosition.Add(animeOffset[i] + animeVector[i]);
				rotation.Add((int)rotation_data[i]);
			}
		}


		protected void findCenterPoint()
		{
			float x = (float)(bb.tl().x + bb.br().x)/2;
			float y = (float)(bb.tl().y + bb.br().y)/2;
			centerPoint = new Vector2(x, y);
		}


		public void calcOffset(Vector2 parentCenter)
		{
			for (var i = 0; i < animeOffset.Count; i++)
			{
				imageOffset.Add(new Vector2((anchorPoint.x - parentCenter.x), (parentCenter.y - anchorPoint.y)));
			}
		}


		public void calcPosition()
		{
			for (var i = 0; i < animePosition.Count; i++)
				position.Add(imageOffset[i] + animePosition[i]);
		}
	}
}
using UnityEngine;
using System;
using System.Collections.Generic;
using LitJson;
using OpenCVForUnity;

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

			body      = new Body     (partTexList[0]);
			leftWing  = new LeftWing (partTexList[1]);
			rightWing = new RightWing(partTexList[2]);
			leftLeg   = new LeftLeg  (partTexList[3]);
			rightLeg  = new RightLeg (partTexList[4]);

			calcPartAnimation(_jsonPaths);
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
	}


	class Body : BodyPart
	{
		public Body(Texture2D _texture)
		{
			texture = _texture;
		}


		public override void calcAnimation(string jsonPath)
		{
			Debug.Log(jsonPath);
			var asset = Resources.Load<TextAsset>(jsonPath);

			JsonData data = JsonMapper.ToObject(asset.text);
			JsonData position_x = data["x"];
			JsonData position_y = data["y"];
			Debug.Log("position_x[0]----"+position_x[0]);
			Debug.Log("position_y[0]----"+position_y[0]);
//			if (position_x[0].IsInt) {
//				Debug.Log("--------position_x is int ");
//			}

			for (var i = 0; i < position_x.Count; i++)
			{
//				position.Add(new Vector2((float)position_x[i], (float)position_y[i]));
				position.Add(new Vector2((int)position_x[i], (int)position_y[i]));

				rotation.Add(0);
			}
		}
	}


	class LeftWing : BodyPart
	{
		public LeftWing(Texture2D _texture)
		{
			texture = _texture;
		}
	}


	class RightWing : BodyPart
	{
		public RightWing(Texture2D _texture)
		{
			texture = _texture;
		}
	}


	class LeftLeg : BodyPart
	{
		public LeftLeg(Texture2D _texture)
		{
			texture = _texture;
		}
	}


	class RightLeg : BodyPart
	{
		public RightLeg(Texture2D _texture)
		{
			texture = _texture;
		}
	}


	abstract class BodyPart
	{
		public Texture2D texture;
		public List<Vector2> position;
		public List<double>  rotation;

		// TODO List<Vector2> offset to support anchor point movement.
		protected Vector2 offset;
		protected List<Vector2> vector;

		protected Vector2 centerPoint;
		protected Vector2 anchorPoint;


		protected void calcCenterPoint(OpenCVForUnity.Rect boundingBox)
		{
			float x = (float)(boundingBox.tl().x + boundingBox.br().x)/2;
			float y = (float)(boundingBox.tl().y + boundingBox.br().y)/2;
			centerPoint = new Vector2(x, y);
		}


		public virtual void calcAnimation(string jsonPath)
		{
			var asset = Resources.Load<TextAsset>(jsonPath);

			JsonData data = JsonMapper.ToObject(asset.text);
			JsonData vector_x_data = data["vector_x"];
			JsonData vector_y_data = data["vector_y"];
			JsonData rotation_data = data["rotation"];

			for (var i = 0; i < vector_x_data.Count; i++)
			{
				vector.Add(new Vector2((float)vector_x_data[i], (float)vector_y_data[i]));
				position.Add(offset + vector[i]); 
				rotation.Add((double)rotation_data[i]);
			}
		}
	}
}
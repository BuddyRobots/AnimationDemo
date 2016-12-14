using UnityEngine;
using System.Collections.Generic;

namespace AnimationDemo
{
	class Owl
	{
		public Body      body;
		public LeftWing  leftWing;
		public RightWing rightWing;
		public LeftLeg   leftLeg;
		public RightLeg  rightLeg;

		Owl(Texture2D _owlTexture)
		{
			List<Texture2D> partTexList = Segmentation.segment(_owlTexture);
			body      = new Body     (partTexList[0]);
			leftWing  = new LeftWing (partTexList[1]);
			rightWing = new RightWing(partTexList[2]);
			leftLeg   = new LeftLeg  (partTexList[3]);
			rightLeg  = new RightLeg (partTexList[4]);
		}

		~Owl()
		{}

		public void calcAnimation()
		{
			
		}


	}


	class Body : BodyPart
	{
		public Body(Texture2D _texture)
		{
			texture = _texture;
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

		protected Skeleton skeleton;

		protected class Skeleton
		{
			public Vector2 centerPoint {get; set;}
			public Vector2 anchorPoint {get; set;}
			public double  rotation    {get; set;}
		}
	}
}
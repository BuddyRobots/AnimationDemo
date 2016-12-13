using UnityEngine;

namespace AnimationDemo
{
	class Owl
	{
		public Body      body;
		public LeftWing  leftWing;
		public RightWing rightWing;
		public LeftLeg   leftLeg;
		public RightLeg  rightLeg;

		Owl()
		{}

		~Owl()
		{}
	}

	class BodyPart
	{
		public Vector2 centerPoint {get; set;}
		public Vector2 anchorPoint {get; set;}
		public double  rotation    {get; set;}
	}

	class Body : BodyPart
	{
		
	}
		
	class LeftWing : BodyPart
	{
		public LeftWing()
		{

		}
	}

	class RightWing : BodyPart
	{
		
	}

	class LeftLeg : BodyPart
	{

	}

	class RightLeg : BodyPart
	{
		
	}
}
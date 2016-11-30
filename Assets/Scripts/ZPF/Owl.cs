using UnityEngine;

class Owl
{
	public Body body;
	public Wing leftWing;
	public Wing rightWing;
	public Leg  leftLeg;
	public Leg  rightLeg;

	Owl()
	{}

	~Owl()
	{}
}

class BodyPart
{
	public Vector2 position;
	public double  rotation;
}

class Body : BodyPart
{
	
}
	
class Wing : BodyPart
{

}

class Leg : BodyPart
{

}
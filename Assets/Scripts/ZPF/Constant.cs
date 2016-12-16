namespace AnimationDemo
{
	public static class Constant
	{
		// GetImage.cs : Parameter for processing 10 photos
		public const int TAKE_NUM_OF_PHOTOS        = 1;

		// Utils.cs : CamQuad original point(x, y) for coverting cordinate from frameImg to Unity3D
		public const int CAM_QUAD_ORIGINAL_POINT_X = -305;
		public const int CAM_QUAD_ORIGINAL_POINT_Y = 375;

		// RotateCamera.cs : Width and Height of CamQuad in Unity3D
		public const int CAM_QUAD_WIDTH            = 610;
		public const int CAM_QUAD_HEIGHT           = 703;

		// Segmentation.cs : 
		public const int NUM_OF_PARTS              = 5;
		public const int NUM_OF_CLASS              = 6;
		public const int CHANNEL                   = 3;
		public const int WIDTH                     = 224;
		public const int HEIGHT                    = 224;
		public const int MORPH_KERNEL_SIZE         = 3;


		// WangQian
		public const float ANIMATION_FRAME_INTERVAL   = 0.04f;//1秒24帧

	}
}
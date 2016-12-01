namespace MagicCircuit
{
	public static class Constant
	{
		// GetImage.cs : Parameter for processing 10 photos
		public const int    TAKE_NUM_OF_PHOTOS        = 1;

		// Utils.cs : CamQuad original point(x, y) for coverting cordinate from frameImg to Unity3D
		public const int    CAM_QUAD_ORIGINAL_POINT_X = -305;
		public const int    CAM_QUAD_ORIGINAL_POINT_Y = 375;

		// RotateCamera.cs : Width and Height of CamQuad in Unity3D
		public const int    CAM_QUAD_WIDTH            = 610;
		public const int    CAM_QUAD_HEIGHT           = 703;

		// CardDetector.cs : Parameters for limiting and filtering detected cards
		public const int    CARD_MIN_SQUARE_LEN       = 70;
		public const int    CARD_MAX_SQUARE_LEN       = 100;
		public const double CARD_MAX_SQUARE_LEN_RATIO = 1.3;
		public const double CARD_OUTER_SQUARE_RATIO   = 1.4;

		// LineDetector.cs : Parameter for limiting and filtering detected lines
		public const int    LINE_ADPTTHRES_KERNEL     = 21;
		public const int    LINE_ADPTTHRES_SUB        = 10;
		public const int    LINE_MORPH_KERNEL         = 3;
		public const int    LINE_REGION_MIN_AREA      = 150;
		public const int    LINE_STEP_SMALL           = 10;
		public const int    LINE_STEP_MEDIUM          = 15;
		public const int    LINE_STEP_LARGE           = 25;
		public const int    LINE_MIN_POINT_NUM        = 3;



		// CurrentFlow.cs : Parameter for determining whether two points are connected
		public const int    POINT_CONNECT_REGION      = 40;

		// RecognizeAlgo.cs : Model image size 3*28*28, 9 classes
		public const int    MODEL_IMAGE_SIZE          = 28;
		public const int    NUM_OF_CLASS              = 9;




		public const float ANIMATION_FRAME_INTERVAL = 0.04f;//1秒24帧
	}
}
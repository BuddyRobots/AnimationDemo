using OpenCVForUnity;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MagicCircuit
{
    public static class RotateCamera
    {
        private static List<Point> ptsBoard;
        private static List<Point> ptsWindow;

        private static Mat homo;
		//Mat rst;

        static RotateCamera()
        {
            ptsBoard = new List<Point>();
            ptsWindow = new List<Point>();
          
			ptsBoard.Add(new Point(133, 24));
			ptsBoard.Add(new Point(133, 433));
			ptsBoard.Add(new Point(418, 99));
			ptsBoard.Add(new Point(418, 359));

			ptsWindow.Add(new Point(0, 0));
			ptsWindow.Add(new Point(Constant.CAM_QUAD_WIDTH - 1, 0));
			ptsWindow.Add(new Point(0, Constant.CAM_QUAD_HEIGHT - 1));
			ptsWindow.Add(new Point(Constant.CAM_QUAD_WIDTH - 1, Constant.CAM_QUAD_HEIGHT - 1));

            Mat rectBrd = Converters.vector_Point2f_to_Mat(ptsBoard);
            Mat rectWin = Converters.vector_Point2f_to_Mat(ptsWindow);

            homo = Imgproc.getPerspectiveTransform(rectBrd, rectWin);
        }

        public static void rotate(ref Mat img)
        {
			Mat rst = new Mat(Constant.CAM_QUAD_HEIGHT, Constant.CAM_QUAD_WIDTH, CvType.CV_8UC3);
			Imgproc.warpPerspective(img, rst, homo, rst.size());
			img = rst.clone();
        }
    }
}
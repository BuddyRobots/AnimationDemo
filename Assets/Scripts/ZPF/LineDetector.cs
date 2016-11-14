using UnityEngine;
using System.Collections.Generic;
using OpenCVForUnity;

namespace MagicCircuit
{
    public class LineDetector
    {		
		public void detectLine(Mat frameImg, List<List<List<Point>>> lineGroupList, List<OpenCVForUnity.Rect> boundingRectList, List<List<Point>> cardSquares)
        {

			//Debug.Log("LineDetector.cs detectLine Start!");


			List<Mat> roiList = new List<Mat>();

			getLines(frameImg, roiList, boundingRectList, cardSquares);

            for (var i = 0; i < roiList.Count; i++)
            {
                lineGroupList.Add(vectorize(roiList[i]));
            }
        }

		private void getLines(Mat frameImg, List<Mat> roiList, List<OpenCVForUnity.Rect> rectList, List<List<Point>> cardSquares)
		{
			Mat hsvImg = new Mat();
			Mat binaryImg = new Mat();
			Mat lineImg = new Mat();

			if (roiList.Count  != 0) roiList.Clear();
			if (rectList.Count != 0) rectList.Clear();

			// Color Thresholding
			Imgproc.cvtColor(frameImg, binaryImg, Imgproc.COLOR_BGR2GRAY);
			Imgproc.adaptiveThreshold(binaryImg, binaryImg, 255, Imgproc.ADAPTIVE_THRESH_GAUSSIAN_C, Imgproc.THRESH_BINARY_INV, Constant.LINE_ADPTTHRES_KERNEL, Constant.LINE_ADPTTHRES_SUB);
			Imgproc.morphologyEx(binaryImg, binaryImg, Imgproc.MORPH_OPEN, Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(Constant.LINE_MORPH_KERNEL, Constant.LINE_MORPH_KERNEL)));

			// Remove card region
			removeCard(ref binaryImg, cardSquares);
			lineImg = binaryImg.clone();

			// Find Contours
			List<MatOfPoint> contours = new List<MatOfPoint>();
			Mat hierarchy = new Mat();
			Imgproc.findContours(binaryImg, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE, new Point(0, 0));

			// Extract components using contour area
			for (int i = 0; i < contours.Count; i++)
			{

				Debug.Log("LineDetector.cs getLine : Imgproc.contourArea(contours[" + i + "]) = " + Imgproc.contourArea(contours[i]));




				if (Imgproc.contourArea(contours[i]) > Constant.LINE_REGION_MIN_AREA)
				{
					OpenCVForUnity.Rect re = Imgproc.boundingRect(contours[i]);

					// Extract only the correspoding component from frame using roi
					// The size of roi is a variable
					Mat roi = new Mat(lineImg, re);
					roiList.Add(roi);
					rectList.Add(re);
				}
			}
			return;
		}    

		public static void removeCard(ref Mat img, List<List<Point>> squares)
		{
			for (var i = 0; i < img.rows(); i++)
				for (var j = 0; j < img.cols(); j++)
					for (var k = 0; k < squares.Count; k++)
						if (isInROI(new Point(j, i), squares[k]))
						{
							img.put(i, j, new byte[3] { 0, 0, 0 });
							break;
						}
		}

		private static bool isInROI(Point p, List<Point> roi)
		{
			double[] pro = new double[4];
			for (int i = 0; i < 4; ++i)
			{
				pro[i] = computeProduct(p, roi[i], roi[(i + 1) % 4]);
			}
			if (pro[0] * pro[2] < 0 && pro[1] * pro[3] < 0)
			{
				return true;
			}
			return false;
		}

		// function pro = kx-y+j, take two points a and b,
		// compute the line argument k and j, then return the pro value
		// so that can be used to determine whether the point p is on the left or right
		// of the line ab
		private static double computeProduct(Point p, Point a, Point b)
		{
			double k = (a.y - b.y) / (a.x - b.x);
			double j = a.y - k * a.x;
			return k * p.x - p.y + j;
		}

        private List<List<Point>> vectorize(Mat lineImg)
        {
            List<Point> myLine = new List<Point>();
            List<Point> line = new List<Point>();
            List<List<Point>> listLine = new List<List<Point>>();
            Queue<Point> allQueue = new Queue<Point>();
            Queue<Point> itrQueue = new Queue<Point>();
            Queue<Point> intQueue = new Queue<Point>();

            // Skeletonize the input first.
            Mat skel = new Mat(lineImg.size(), CvType.CV_8UC1);
            skel = skeletonization(lineImg);
            
            // Pick an init point on the line
            Point firstPoint = findFirstPoint(skel);
            if (firstPoint.x != 0 && firstPoint.y != 0)
            {
                myLine.Add(firstPoint);
                line.Add(firstPoint);
            }
            if (line.Count == 0) return listLine; // If we don't have any point






//			Debug.Log("LineDetector.cs vectorize : firstPoint = " + firstPoint);






            bool firstPointFlag = true;
            bool shouldMergeFirstLine = false;
            //@
            while (true)
            {
                Point interPoint = new Point();
                // @allQueue : all intersect points
                // @itrQueue : intersect points in one iteration
                // @intQueue : intersect point to be added to line
                constructLine(skel, firstPoint, ref line, ref itrQueue);
                
                // Determine whether to merge the first two lines or not
				// We should merge the first two lines only when we constructLine using the first point 
				// and detected 2 next points immediately.
				if (firstPointFlag && (line.Count < 2))
                {
					firstPointFlag = false;
                    if (itrQueue.Count == 2)
                        shouldMergeFirstLine = true;                    
                }

                // Add intersect point to the end of this line
                if (itrQueue.Count > 0)
                {
                    Queue<Point> tmpQueue = new Queue<Point>(itrQueue);
                    interPoint = findIntersectPoint(tmpQueue);
                    line.Add(interPoint);
                }

				// If we have more than MIN_POINT_NUM points on one line, add this line
                if (line.Count > Constant.LINE_MIN_POINT_NUM)
                    listLine.Add(line);

                // Enqueue the intersect points
                while (itrQueue.Count > 0)
                {
                    allQueue.Enqueue(itrQueue.Dequeue());
                    intQueue.Enqueue(interPoint);
                }

                // Break when all intersect points are processed
                if (allQueue.Count == 0)
                    break; 
                 
                // Preperation for next iteration
                firstPoint = allQueue.Dequeue();
                line = new List<Point>();
                line.Add(intQueue.Dequeue());
                line.Add(firstPoint);
            }

            // Merge the first two lines
			if (shouldMergeFirstLine)
                mergeFirstLine(ref listLine);

            // Merge mis-detected lines
            mergeLine(ref listLine);

            return listLine;
        }

        private Mat skeletonization(Mat grayImg)
        {
            Mat skel = new Mat(grayImg.size(), CvType.CV_8UC1, new Scalar(0, 0, 0));
            Mat temp = new Mat();
            Mat eroded = new Mat();

            Mat element = Imgproc.getStructuringElement(Imgproc.MORPH_CROSS, new Size(3, 3));

            for (var i = 0; i < 200; i++)
            {
                Imgproc.erode(grayImg, eroded, element);
                Imgproc.dilate(eroded, temp, element); // temp = open(grayImg)
                Core.subtract(grayImg, temp, temp);
                Core.bitwise_or(skel, temp, skel);
                grayImg = eroded.clone();

                if (Core.countNonZero(grayImg) == 0)   // done.
                    break;
            }

            //Imgproc.GaussianBlur(skel, skel, new Size(5, 5), 0);
            //pointFilter(skel, 2, 5);
            element = Imgproc.getStructuringElement(Imgproc.MORPH_CROSS, new Size(2, 2));
            Imgproc.dilate(skel, skel, element);

            return skel;
        }

        private Point findFirstPoint(Mat skel)
        {
            for (var i = 1; i < skel.rows(); i++)
                for (var j = 1; j < skel.cols(); j++)
                    if (skel.get(i, j)[0] > 125)
                    {                        
                        return new Point(j, i);
                    }
            return new Point(0, 0);
        }

		private Queue<Point> findNextPoints(Mat skel, Point current, int step)
        {
            Queue<Point> temp = new Queue<Point>();
            Point temPoint_1 = new Point();
            Point temPoint_2 = new Point();
            Queue<Point> result = new Queue<Point>();

            // Point Range[0, rows() - 1][0, cols() - 1]          
            int _xl = Mathf.Max((int)current.x - step, 0);
            int _xr = Mathf.Min((int)current.x + step, skel.cols() - 1);
            int _yu = Mathf.Max((int)current.y - step, 0);
            int _yd = Mathf.Min((int)current.y + step, skel.rows() - 1);

            // left
            for (var y = _yu + 1; y < _yd; y++)
                if (skel.get(y, _xl)[0] > 125)
                    temp.Enqueue(new Point(_xl, y));
            if (temp.Count > 0)
            {
                temPoint_1 = temp.Dequeue();
                result.Enqueue(temPoint_1);
                while(temp.Count > 0)
                {
                    temPoint_2 = temp.Dequeue();
                    if (temPoint_1.y - temPoint_2.y > 1) // if the new point is not connected to the old point
                        result.Enqueue(temPoint_2);
                    temPoint_1 = temPoint_2;
                }
            }
            temp.Clear();
            // right
            for (var y = _yu + 1; y < _yd; y++)
                if (skel.get(y, _xr)[0] > 125)
                    temp.Enqueue(new Point(_xr, y));
            if (temp.Count > 0)
            {
                temPoint_1 = temp.Dequeue();
                result.Enqueue(temPoint_1);
                while (temp.Count > 0)
                {
                    temPoint_2 = temp.Dequeue();
                    if (temPoint_1.y - temPoint_2.y > 1) // if the new point is not connected to the old point
                        result.Enqueue(temPoint_2);
                    temPoint_1 = temPoint_2;
                }
            }
            temp.Clear();
            // up
            for (var x = _xl; x <= _xr; x++)
                if (skel.get(_yu, x)[0] > 125)
                    temp.Enqueue(new Point(x, _yu));
            if (temp.Count > 0)
            {
                temPoint_1 = temp.Dequeue();
                result.Enqueue(temPoint_1);
                while (temp.Count > 0)
                {
                    temPoint_2 = temp.Dequeue();
                    if (temPoint_1.x - temPoint_2.x > 1) // if the new point is not connected to the old point
                        result.Enqueue(temPoint_2);
                    temPoint_1 = temPoint_2;
                }
            }
            temp.Clear();
            // down
            for (var x = _xl; x <= _xr; x++)
                if (skel.get(_yd, x)[0] > 125)
                    temp.Enqueue(new Point(x, _yd));
            if (temp.Count > 0)
            {
                temPoint_1 = temp.Dequeue();
                result.Enqueue(temPoint_1);
                while (temp.Count > 0)
                {
                    temPoint_2 = temp.Dequeue();
                    if (temPoint_1.x - temPoint_2.x > 1) // if the new point is not connected to the old point
                        result.Enqueue(temPoint_2);
                    temPoint_1 = temPoint_2;
                }
            }
            temp.Clear();

            // Delete detected region
            removeBox(skel, _xl, _xr, _yu, _yd);
            return result;
        }

        private Point findIntersectPoint(Queue<Point> pointQueue)
        {
            if (pointQueue.Count == 0)
                return new Point(0, 0);

            double x = 0;
            double y = 0;
            int count = pointQueue.Count;

            while (pointQueue.Count > 0)
            {
                Point p = pointQueue.Dequeue();
                x += p.x;
                y += p.y;
            }

            return new Point(x / count, y / count);
        }

        private void constructLine(Mat skel, Point current, ref List<Point> line, ref Queue<Point> pointQueue)
        {
            while(true)
            {
                Queue<Point> myPoint = findNextPoints(skel, current, Constant.LINE_STEP_SMALL);

                if (myPoint.Count != 1)
                {
                    // increase radius
                    myPoint = findNextPoints(skel, current, Constant.LINE_STEP_MEDIUM);

                    if (myPoint.Count != 1)
                    {
                        // increase radius
                        myPoint = findNextPoints(skel, current, Constant.LINE_STEP_LARGE);

                        if (myPoint.Count != 1)
                        {
                            // intersect point or deadend
                            pointQueue = myPoint; // Save pointQueue
                            return; 
                        }
                    }                    
                }

                // current = next
                current = myPoint.Dequeue();

                line.Add(current);
            }
        }

        private void removeBox(Mat img, int xl, int xr, int yu, int yd)
        {
            for(var i = yu; i <= yd; i++)
                for(var j = xl; j <= xr; j++)
                {
                    img.put(i, j, 0);
                }
        }

        private void mergeFirstLine(ref List<List<Point>> listLine)
        {
            if (listLine.Count >= 2)
            {
                listLine[0].Reverse();
                listLine[0].AddRange(listLine[1]);
                listLine.RemoveAt(1);
            }
        }

        /// <summary>
        /// If a point in @end has and only has one match in @start, 
        /// merge these two lines.
        /// </summary>
        /// <param name="listLine"></param>
        /// 
        private void mergeLine(ref List<List<Point>> listLine)
        {
            List<Point> start = new List<Point>();
            List<Point> end = new List<Point>();

            // Construct @start and @end
            for (var i = 0; i < listLine.Count; i++)
            {
                start.Add(listLine[i][0]);
                end.Add(listLine[i][listLine[i].Count - 1]);
            }

            // Compare
            for (var i = 0; i < end.Count; i++)
            {
                int count = 0;
                int end_idx = i;
                int start_idx = 0;

                for (var j = 0; j < start.Count && count < 2; j++)
                {
                    if (start[j].x == end[i].x && start[j].y == end[i].y)
                    {
                        count++;
                        start_idx = j;
                    }
                }

                if (count == 1)
                {
                    // merge start_idx and end_idx
                    listLine[end_idx].AddRange(listLine[start_idx]);

                    listLine.RemoveAt(start_idx);
                    start.RemoveAt(start_idx);
                    end.RemoveAt(end_idx);
                }
            }
        }
    }
}
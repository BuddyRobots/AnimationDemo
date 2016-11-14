using UnityEngine;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenCVForUnity;

namespace MagicCircuit
{
	public class RecognizeAlgo
	{
		[DllImport("__Internal")]  
		private static extern int callLua_predictClass(double[] imageData,int imageDataLength);

		[DllImport("__Internal")]  
		private static extern int callLua_predictDirection(double[] imageData, int imageDataLength, int klass);

		private static MatOfPoint2f modelImageSizePoint;

		private LineDetector lineDetector;


	    public RecognizeAlgo()
	    {
	        lineDetector = new LineDetector();

			modelImageSizePoint = new MatOfPoint2f(new Point[4]
				{ new Point(0, 0), new Point(Constant.MODEL_IMAGE_SIZE, 0), new Point(Constant.MODEL_IMAGE_SIZE, Constant.MODEL_IMAGE_SIZE), new Point(0, Constant.MODEL_IMAGE_SIZE) });
	    }
			
	    public Mat process(Mat frameImg, ref List<CircuitItem> itemList)
	    {
	        Mat grayImg = new Mat();
	        Mat binaryImg = new Mat();
	        Mat frameTransImg = new Mat();
	        Mat resultImg = frameImg.clone();

			int showOrder = 0;
	        CircuitItem tmpItem;

	        /// Detect Cards =============================================================





			int startTime_1 = DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;







	        // Thresholding
	        Imgproc.cvtColor(frameImg, grayImg, Imgproc.COLOR_BGR2GRAY);
	        Imgproc.adaptiveThreshold(grayImg, binaryImg, 255, Imgproc.ADAPTIVE_THRESH_GAUSSIAN_C, Imgproc.THRESH_BINARY, 15, 1);






	        // Get all the squares
	        List<List<Point>> squares = new List<List<Point>>();
	        List<List<Point>> outer_squares = new List<List<Point>>();
	        squares = CardDetector.findSquares(binaryImg);
	        outer_squares = CardDetector.computeOuterSquare(squares);




			Debug.Log("RecognizeAlgo.cs process : squares.Count = " + squares.Count);
			Debug.Log("RecognizeAlgo.cs process : outer_squares.Count = " + outer_squares.Count);





	        for (int i = 0; i < squares.Count; i++)
	        {
	            // Perspective transform
	            Mat homography = Calib3d.findHomography(new MatOfPoint2f(squares[i].ToArray()), modelImageSizePoint);
	            Imgproc.warpPerspective(frameImg, frameTransImg, homography, new Size());

				// predictClass
	            // Input  : Mat cardTransImg.submat(0, imageSize, 0, imageSize);
				// Output : int klass;
				string name = "name";
	            ItemType type = new ItemType();            
				Mat cardImg = frameTransImg.submat(0, Constant.MODEL_IMAGE_SIZE, 0, Constant.MODEL_IMAGE_SIZE);
				double[] cardArray = mat2array(cardImg);
				int klass = predictClass(cardArray);

				// predictDirection
				// Input  : int klass,
				//           Mat cardImg;
				// Output : int direction (1, 2, 3, 4)
				//int direction = 4;
				int direction = predictDirection(cardArray, klass);
				correctDirection(ref direction, klass);

				switch (klass)
				{
				case 1:
					name = "Battery";
					type = ItemType.Battery;
					break;
				case 2:
					name = "Switch";
					type = ItemType.Switch;
					break;
				case 3:
					name = "LightActSwitch";
					type = ItemType.LightActSwitch;
					break;
				case 4:
					name = "VoiceOperSwitch";
					type = ItemType.VoiceOperSwitch;
					break;
				case 5:
					name = "VoiceTimedelaySwitch";
					type = ItemType.VoiceTimedelaySwitch;
					break;
				case 6:
					name = "SPDTSwitch";
					type = ItemType.SPDTSwitch;
					break;
				case 7:
					name = "Bulb";
					type = ItemType.Bulb;
					break;
				case 8:
					name = "LoudSpeaker";
					type = ItemType.Loudspeaker;
					break;
				case 9:
					name = "InductionCooker";
					type = ItemType.InductionCooker;
					break;
				}





	            // Add to listItem
				tmpItem = new CircuitItem(klass, name, type, showOrder++);
	            tmpItem.extractCard(direction, outer_squares[i]);
	            itemList.Add(tmpItem);
	        }
			// ReOrder listItem
			reOrder(ref itemList);








			int time_1 = DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
			int elapse_1 = time_1 - startTime_1;
			//Debug.Log("RecognizeAlgo.cs DetectCards Time elapse : " + elapse_1);







	        /// Detect Lines =============================================================        
			//Debug.Log("RecognizeAlgo.cs DetectLine Start!");
			int startTime_2 = DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;





			List<List<List<Point>>> lineGroupList = new List<List<List<Point>>>();
			List<OpenCVForUnity.Rect> boundingRectList = new List<OpenCVForUnity.Rect>();
			lineDetector.detectLine(frameImg, lineGroupList, boundingRectList, outer_squares);

	        // Add to CircuitItem
			for (var i = 0; i < lineGroupList.Count; i++)
	            for (var j = 0; j < lineGroupList[i].Count; j++)
	            {
	                tmpItem = new CircuitItem(showOrder, "CircuitLine", ItemType.CircuitLine, showOrder++);
	                tmpItem.extractLine(lineGroupList[i][j], boundingRectList[i]);
	                itemList.Add(tmpItem);
	            }





			int time_2 = DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
			int elapse_2 = time_2 - startTime_2;
			//Debug.Log("RecognizeAlgo DetectLines Time elapse : " + elapse_2);



	        return resultImg;
	    }
			
		// Call lua to do card classification
		private int predictClass(double[] cardArray)
		{
			int startTime = DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;




			int prediction = callLua_predictClass(cardArray, cardArray.Length);





			int currentTime = DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
			int elapseTime = currentTime - startTime;
			//Debug.Log("RecognizeAlgo.cs predictClass() : Time elapse : " + elapseTime);



			return prediction;
		}

		// Call lua to do direction classification
		private int predictDirection(double[] cardArray, int klass)
		{
			int startTime = DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;





			int prediction = callLua_predictDirection(cardArray, cardArray.Length, klass);





			int currentTime = DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
			int elapseTime = currentTime - startTime;
			//Debug.Log("RecognizeAlgo.cs predictDirection() : Time elapse : " + elapseTime);



			return prediction;
		}

		private double[] mat2array(Mat img)
		{
			int startTime = DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;




			double[] sample = new double[28*28*3];

			int pointer = 0;
			for (var y = 0; y < img.rows(); y++)
				for (var x = 0; x < img.cols(); x++)
				{
					double[] value = new double[3];
					value = img.get(x, y);

					sample[pointer]        = value[0] / 255;
					sample[pointer + 784]  = value[1] / 255;
					sample[pointer + 1568] = value[2] / 255;
					pointer++;
				}



			int currentTime = DateTime.Now.Second * 1000 + DateTime.Now.Millisecond;
			int elapseTime = currentTime - startTime;
			//Debug.Log("RecognizeAlgo.cs mat2array() : Time elapse : " + elapseTime);




			return sample;
		}

		private void reOrder(ref List<CircuitItem> listItem)
		{
			int counter = 0;
			List<CircuitItem> tmpList = new List<CircuitItem>();

			for (var i = 1; i <= Constant.NUM_OF_CLASS; i++)
				for (var j = 0; j < listItem.Count; j++)
					if (listItem[j].ID == i)
					{
						listItem[j].ID = counter++;
						tmpList.Add(listItem[j]);
					}
			listItem = tmpList;
		}

		// TODO
		// Need to correct predicted direction due to unknown, strange bug
		private void correctDirection(ref int direction, int klass)
		{
			if (klass == 1 || klass == 8)
			{
				switch (direction)
				{
				case 1:
					direction = 2; break;
				case 2:
					direction = 1; break;
				case 3:
					direction = 4; break;
				case 4:
					direction = 3; break;
				}
			}
			else
			{
				switch (direction)
				{
				case 1:
					direction = 4; break;
				case 2:
					direction = 3; break;
				case 3:
					direction = 2; break;
				case 4:
					direction = 1; break;
				}
			}
		}

		/*public List<Mat> createDataSet(Mat frameImg)
	    {
			
	        Mat grayImg = new Mat();
	        Mat binaryImg = new Mat();
	        Mat cardTransImg = new Mat();
	        Mat cardImg = new Mat();
	        List<Mat> result = new List<Mat>();

	        /// Detect Cards =============================================================
	        MatOfPoint2f point = new MatOfPoint2f(new Point[4]
	            { new Point(0, 0), new Point(imageSize, 0), new Point(imageSize, imageSize), new Point(0, imageSize) });

	        // Thresholding
	        Imgproc.cvtColor(frameImg, grayImg, Imgproc.COLOR_BGR2GRAY);
	        Imgproc.adaptiveThreshold(grayImg, binaryImg, 255, Imgproc.ADAPTIVE_THRESH_GAUSSIAN_C, Imgproc.THRESH_BINARY, 15, 1);

	        // Get all the squares
	        List<List<Point>> squares = new List<List<Point>>();
	        squares = CardDetector.findSquares(binaryImg);

	        for (int i = 0; i < squares.Count; i++)
	        {
	            // Perspective transform
	            Mat homography = Calib3d.findHomography(new MatOfPoint2f(squares[i].ToArray()), point);
	            Imgproc.warpPerspective(frameImg, cardTransImg, homography, new Size());

	            cardImg = cardTransImg.submat(0, imageSize, 0, imageSize);

	            result.Add(cardImg);
	            //path = path + "/" + System.DateTime.Now.Ticks + ".jpg";

	            //Imgcodecs.imwrite(path, cardImg);
	        }
	        return result;
	    }*/
	}
}
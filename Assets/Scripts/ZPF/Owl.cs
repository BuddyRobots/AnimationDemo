using UnityEngine;
using System;
using System.Collections.Generic;
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

		private Mat   originImage;
		private Point originPoint;
		private Size  originalSize;
		 

		public Owl(Texture2D _owlTexture, List<string> _jsonPathList)
		{
			partTexList  = new List<Texture2D>();
			partMaskList = new List<Mat>();
			partBBList   = new List<OpenCVForUnity.Rect>();

			List<int> thresList = new List<int>();
			thresList.Add(0);
			thresList.Add(180);
			thresList.Add(50);
			thresList.Add(255);
			thresList.Add(0);
			thresList.Add(255);

			Mat croppedImage = cropTexToModelSizeMat(_owlTexture, thresList);

			Mat modelMaskImage = Segmentation.segment(croppedImage);
			Mat originMaskImage = new Mat(originalSize, CvType.CV_8UC1);
			Imgproc.resize(modelMaskImage, originMaskImage, originalSize, 0, 0, Imgproc.INTER_NEAREST);

			Segmentation.getLists(originImage, originMaskImage, out partTexList, out partMaskList, out partBBList);
					
			body      = new Body     (partTexList[0], partMaskList[0], partBBList[0], _jsonPathList[0]);
			leftWing  = new LeftWing (partTexList[1], partMaskList[1], partBBList[1], _jsonPathList[1]);
			rightWing = new RightWing(partTexList[2], partMaskList[2], partBBList[2], _jsonPathList[2]);
			leftLeg   = new LeftLeg  (partTexList[3], partMaskList[3], partBBList[3], _jsonPathList[3]);
			rightLeg  = new RightLeg (partTexList[4], partMaskList[4], partBBList[4], _jsonPathList[4]);

			calcOffset();
			calcImageVector();
			calcPosition();
		}


		private Mat cropTexToModelSizeMat(Texture2D sourceTex, List<int> thresList)
		{
			Mat sourceImage = new Mat(sourceTex.height, sourceTex.width, CvType.CV_8UC3);
			Utils.texture2DToMat(sourceTex, sourceImage);

			// BGR to HSV
			Mat hsvImage = new Mat(sourceImage.rows(), sourceImage.cols(), CvType.CV_8UC3);
			List<Mat> hsvList = new List<Mat>();
			Imgproc.cvtColor(sourceImage, hsvImage, Imgproc.COLOR_BGR2HSV);
			// InRange
			Mat grayImage = new Mat(sourceImage.rows(), sourceImage.cols(), CvType.CV_8UC1);
			Core.inRange(hsvImage,
				new Scalar(thresList[0], thresList[2], thresList[4]),
				new Scalar(thresList[1], thresList[3], thresList[5]),
				grayImage);
			Imgproc.morphologyEx(grayImage, grayImage, Imgproc.MORPH_OPEN,
				Imgproc.getStructuringElement(Imgproc.MORPH_ELLIPSE, new Size(5, 5)));
			
			// Find Contours
			List<MatOfPoint> contours = new List<MatOfPoint>();
			Mat hierarchy = new Mat();
			Imgproc.findContours(grayImage, contours, hierarchy, Imgproc.RETR_EXTERNAL,
				Imgproc.CHAIN_APPROX_SIMPLE, new Point(0, 0));

			int maxAreaIdex = 0;
			double maxArea = 0;
			for (var i = 0; i < contours.Count; i++)
			{
				double area = Imgproc.contourArea(contours[i]);
				if (area > maxArea)
				{
					maxArea = area;
					maxAreaIdex = i;
				}
			}	
			// Find Bounding Box
			OpenCVForUnity.Rect roi = Imgproc.boundingRect(contours[maxAreaIdex]);
			OpenCVForUnity.Rect bb = new OpenCVForUnity.Rect(
				new Point(Math.Max(roi.tl().x - 50.0, 0),
				          Math.Max(roi.tl().y - 50.0, 0)),
				new Point(Math.Min(roi.br().x + 50.0, sourceImage.cols()),
					      Math.Min(roi.br().y + 50.0, sourceImage.rows())));
			Mat croppedImage = new Mat(sourceImage, bb);
			// Zoom to 224*224
			zoomCropped(ref croppedImage, ref bb);		

			return croppedImage;
		}


		private void zoomCropped(ref Mat croppedImage, ref OpenCVForUnity.Rect bb)
		{
			int croppedWidth = croppedImage.cols();
			int croppedHeight = croppedImage.rows();
			OpenCVForUnity.Rect expandedBB;

			if (croppedWidth > croppedHeight)
			{
				int topMargin = (croppedWidth - croppedHeight)/2;
				int botMargin = topMargin;

				// Needed due to percision loss when /2
				if ((croppedHeight + topMargin*2) != croppedWidth)
					botMargin = croppedWidth - croppedHeight - topMargin;
											
				Core.copyMakeBorder(croppedImage, croppedImage, topMargin, botMargin, 0, 0, Core.BORDER_REPLICATE);
				expandedBB = new OpenCVForUnity.Rect(
					new Point(bb.tl().x, bb.tl().y - topMargin),
					new Point(bb.br().x, bb.br().y + botMargin));
			}
			else if (croppedHeight > croppedWidth)
			{
				int lefMargin = (croppedHeight - croppedWidth)/2;
				int rigMargin = lefMargin;

				// Need due to percision loss when /2
				if ((croppedWidth + lefMargin*2) != croppedHeight)
					rigMargin = croppedHeight - croppedWidth - lefMargin;

				Core.copyMakeBorder(croppedImage, croppedImage, 0, 0, lefMargin, rigMargin, Core.BORDER_REPLICATE);
				expandedBB = new OpenCVForUnity.Rect(
					new Point(bb.tl().x - lefMargin, bb.tl().y),
					new Point(bb.br().x + rigMargin, bb.br().y));
			}
			else
			{
				expandedBB = bb;
			}

			// We have the originPoint & originalSize in the frame cordinate here.
			originPoint = expandedBB.tl();
			originImage = croppedImage.clone();
			originalSize = expandedBB.size();

			Mat scaleImage = new Mat();
			Imgproc.resize(croppedImage, scaleImage, new Size(Constant.MODEL_HEIGHT, Constant.MODEL_WIDTH));

			// Return croppedImage[224*224*3] bb(original cordinate expandedBB)
			croppedImage = scaleImage;
			bb = expandedBB;
		}


		private void calcOffset()
		{
			Vector2 bodyCenter = body.getCenterPoint();
			body.calcImageOffset(bodyCenter);
			leftWing.calcImageOffset(bodyCenter);
			rightWing.calcImageOffset(bodyCenter);
			leftLeg.calcImageOffset(bodyCenter);
			rightLeg.calcImageOffset(bodyCenter);
		}


		private void calcImageVector()
		{
			body.calcImageVector();
			leftWing.calcImageVector();
			rightWing.calcImageVector();
			leftLeg.calcImageVector();
			rightLeg.calcImageVector();
		}


		private void calcPosition()
		{
			body.calcPosition(originPoint);
			leftWing.calcPosition(body.position);
			rightWing.calcPosition(body.position);
			leftLeg.calcPosition(body.position);
			rightLeg.calcPosition(body.position);
		}
	}
}
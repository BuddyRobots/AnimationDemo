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

		private Mat   originImage;
		private Point originPoint;
		private Size  originalSize;
		 

		public Owl(Texture2D _owlTexture, List<string> _jsonPaths)
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



			///
			Debug.Log("Owl.cs Owl() : originMaskImage.size = " + originMaskImage.size());
			Debug.Log("Owl.cs Owl() : originImage.size = " + originImage.size());
			///



			Segmentation.getLists(originImage, originMaskImage, out partTexList, out partMaskList, out partBBList);



			///
			for (var i = 0; i < 5; i++)
				Debug.Log("Owl.cs Owl : partTexList["+i+"].Size = " + partTexList[i].width + "x" + partTexList[i].height);
			///


					
			body      = new Body     (partTexList[0], partMaskList[0], partBBList[0]);
			leftWing  = new LeftWing (partTexList[1], partMaskList[1], partBBList[1]);
			rightWing = new RightWing(partTexList[2], partMaskList[2], partBBList[2]);
			leftLeg   = new LeftLeg  (partTexList[3], partMaskList[3], partBBList[3]);
			rightLeg  = new RightLeg (partTexList[4], partMaskList[4], partBBList[4]);

			calcPartAnimation(_jsonPaths);
			calcOffset();
			calcImageVector();
			calcPosition();



			///
			Debug.Log("Owl.cs Owl() : body.centerPoint = " + body.getCenterPoint());
			///


		
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



			///
			Debug.Log("Owl.cs cropTexToModelSizeMat() : sourceTex.size = " + sourceTex.width + "x" + sourceTex.height);
			Debug.Log("Owl.cs cropTexToModelSizeMat() : sourceImage.size = " + sourceImage.size());
			Debug.Log("Owl.cs cropTexToModelSizeMat() : grayImage.size = " + grayImage.size());
			///


			
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
//				Debug.Log("CropImage.cs crop() : contours["+i+"].Area = " + area);
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



			///
			Debug.Log("Owl.cs cropTexToModelSizeMat() : roi.size = " + roi.size());
			Debug.Log("Owl.cs cropTexToModelSizeMat() : bb.size = " + bb.size());
			Debug.Log("Owl.cs cropTexToModelSizeMat() : croppedImage.size = " + croppedImage.size());
			///



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
			body.calcPosition();
			leftWing.calcPosition(body.position);
			rightWing.calcPosition(body.position);
			leftLeg.calcPosition(body.position);
			rightLeg.calcPosition(body.position);
		}
	}


	class Body : BodyPart
	{
		public Body(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb)
			: base(_texture, _mask, _bb)
		{
			findAnchorPoint();
		}


		private void findAnchorPoint()
		{
			anchorPoint = centerPoint;
		}


		public Vector2 getCenterPoint()
		{
			return centerPoint;
		}


		public void calcPosition()
		{
			// TODO coordinate issue!
			for (var i = 0; i < animePosition.Count; i++)
				position.Add(centerPoint + imageOffset[i] + imageVector[i]);
		}
	}


	class LeftWing : BodyPart
	{
		public LeftWing(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb)
			: base(_texture, _mask, _bb)
		{
			findAnchorPoint();
		}


		private void findAnchorPoint()
		{
			int top    = (int)bb.tl().y;
			int bottom = (int)bb.br().y;
			int right  = (int)bb.br().x - 1;
			List<int> yList = new List<int>();
			for (var i = top; i < bottom; i++)
				if (mask.get(i, right)[0] > 200)
					yList.Add(i);
			if (yList.Count == 0)
				Debug.Log("Owl.cs RightWing findAnchorPoint() : did not find anchorPoint!!");
			anchorPoint = new Vector2(right, MyUtils.average(yList));
		}
	}


	class RightWing : BodyPart
	{
		public RightWing(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb)
			: base(_texture, _mask, _bb)
		{
			findAnchorPoint();
		}


		private void findAnchorPoint()
		{
			int top    = (int)bb.tl().y;
			int bottom = (int)bb.br().y;
			int left   = (int)bb.tl().x;
			List<int> yList = new List<int>();
			for (var i = top; i < bottom; i++)
				if (mask.get(i, left)[0] > 200)
					yList.Add(i);
			if (yList.Count == 0)
				Debug.Log("Owl.cs RightWing findAnchorPoint() : did not find anchorPoint!!");
			anchorPoint = new Vector2(left, MyUtils.average(yList));
		}
	}


	class LeftLeg : BodyPart
	{
		public LeftLeg(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb)
			: base(_texture, _mask, _bb)
		{
			findAnchorPoint();
		}


		private void findAnchorPoint()
		{
			int top   = (int)bb.tl().y;
			int left  = (int)bb.tl().x;
			int right = (int)bb.br().x;
			List<int> xList = new List<int>();
			for (var j = left; j < right; j++)
				if (mask.get(top, j)[0] > 200)
					xList.Add(j);
			if (xList.Count == 0)
				Debug.Log("Owl.cs LeftLeg findAnchorPoint() : did not find anchorPoint!!");
			anchorPoint = new Vector2(MyUtils.average(xList), top);
		}
	}


	class RightLeg : BodyPart
	{
		public RightLeg(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb)
			: base(_texture, _mask, _bb)
		{
			findAnchorPoint();
		}


		private void findAnchorPoint()
		{
			int top   = (int)bb.tl().y;
			int left  = (int)bb.tl().x;
			int right = (int)bb.br().x;
			List<int> xList = new List<int>();
			for (var j = left; j < right; j++)
				if (mask.get(top, j)[0] > 200)
					xList.Add(j);
			if (xList.Count == 0)
				Debug.Log("Owl.cs RightLeg findAnchorPoint() : did not find anchorPoint!!");
			anchorPoint = new Vector2(MyUtils.average(xList), top);
		}
	}


	abstract class BodyPart
	{
		public Texture2D texture;
		public List<Vector2> position;
		public List<double>  rotation;
		protected List<Vector2> animePosition;

		protected Mat mask;
		protected OpenCVForUnity.Rect bb;

		// Animation template offset & vector
		protected List<Vector2> animeOffset;
		protected List<Vector2> animeVector;
		// Player drawing image offset & vector
		protected List<Vector2> imageOffset;
		protected List<Vector2> imageVector;

		protected Vector2 centerPoint;
		protected Vector2 anchorPoint;


		protected BodyPart(Texture2D _texture, Mat _mask, OpenCVForUnity.Rect _bb)
		{
			position = new List<Vector2>();
			rotation = new List<double>();
			animePosition = new List<Vector2>();
			animeOffset = new List<Vector2>();
			animeVector = new List<Vector2>();
			centerPoint = new Vector2();
			anchorPoint = new Vector2();
			imageOffset = new List<Vector2>();
			imageVector = new List<Vector2>();

			texture = _texture;
			mask = _mask;
			bb = _bb;

			findCenterPoint();
		}
			

		public virtual void calcAnimation(string jsonPath)
		{
			var asset = Resources.Load<TextAsset>(jsonPath);

			JsonData data = JsonMapper.ToObject(asset.text);
			JsonData offset_x_data = data["offset_x"];
			JsonData offset_y_data = data["offset_y"];
			JsonData vector_x_data = data["vector_x"];
			JsonData vector_y_data = data["vector_y"];
			JsonData rotation_data = data["rotation"];

			for (var i = 0; i < vector_x_data.Count; i++)
			{
				animeOffset.Add(new Vector2((int)offset_x_data[i], (int)offset_y_data[i]));
				animeVector.Add(new Vector2((int)vector_x_data[i], (int)vector_y_data[i]));
				animePosition.Add(animeOffset[i] + animeVector[i]);
				rotation.Add((int)rotation_data[i]);
			}
		}


		protected void findCenterPoint()
		{
			float x = (float)(bb.tl().x + bb.br().x)/2;
			float y = (float)(bb.tl().y + bb.br().y)/2;
			centerPoint = new Vector2(x, y);



			///
			Debug.Log("Owl.cs BadyPart findCenterPoint() : centerPoint = " + centerPoint); 
			///



		}


		public void calcImageOffset(Vector2 parentCenter)
		{
			for (var i = 0; i < animeOffset.Count; i++)
			{
				imageOffset.Add(new Vector2((anchorPoint.x - parentCenter.x), (parentCenter.y - anchorPoint.y)));



				///
				Debug.Log("Owl.cs BodyPart calcImageOffset() : imageOffset = " + imageOffset);
				///



			}
		}


		public void calcImageVector()
		{
			Vector2 initImageVector = new Vector2((centerPoint.x - anchorPoint.x), (anchorPoint.y - centerPoint.y));

			float ratio = 1;
			float initAnimeVectorMagnitude = animeVector[0].magnitude;
			if (initAnimeVectorMagnitude != 0)
				ratio = initImageVector.magnitude / initAnimeVectorMagnitude;

			for (var i = 0; i < animeVector.Count; i++)
				imageVector.Add(new Vector2(animeVector[i].x*ratio, animeVector[i].y*ratio));
		}


		public void calcPosition(List<Vector2> parentPosition)
		{
			for (var i = 0; i < animePosition.Count; i++)
				position.Add(parentPosition[i] + imageOffset[i] + imageVector[i]);
		}
	}
}
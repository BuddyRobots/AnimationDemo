using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using OpenCVForUnity;
using AnimationDemo;


public static class CropImage
{
	private const int MORPH_KERNEL_SIZE = 5;


	public static Mat crop(Mat sourceImage, List<int> thresList)
	{
		Mat hsvImage = new Mat(sourceImage.rows(), sourceImage.cols(), CvType.CV_8UC3);
		List<Mat> hsvList = new List<Mat>();
		Imgproc.cvtColor(sourceImage, hsvImage, Imgproc.COLOR_BGR2HSV);

		Mat grayImage = new Mat(sourceImage.rows(), sourceImage.cols(), CvType.CV_8UC3);
		Core.inRange(hsvImage,
			new Scalar(thresList[0], thresList[2], thresList[4]),
			new Scalar(thresList[1], thresList[3], thresList[5]),
			grayImage);
		Imgproc.morphologyEx(grayImage, grayImage, Imgproc.MORPH_OPEN,
			Imgproc.getStructuringElement(Imgproc.MORPH_ELLIPSE, new Size(MORPH_KERNEL_SIZE, MORPH_KERNEL_SIZE)));

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
			Debug.Log("CropImage.cs crop() : contours["+i+"].Area = " + area);
			if (area > maxArea)
			{
				maxArea = area;
				maxAreaIdex = i;
			}
		}	

		OpenCVForUnity.Rect roi = Imgproc.boundingRect(contours[maxAreaIdex]);
		OpenCVForUnity.Rect bb = new OpenCVForUnity.Rect(new Point(Math.Max(roi.tl().x - 50.0, 0), Math.Max(roi.tl().y - 50.0, 0)),
			new Point(Math.Min(roi.br().x + 50.0, sourceImage.cols()), Math.Min(roi.br().y + 50.0, sourceImage.rows())));
		Mat croppedImage = new Mat(sourceImage, bb);

		Mat resultImage = zoomCropped(croppedImage);
		return resultImage;
	}


	private static Mat zoomCropped(Mat croppedImage)
	{
		double scale = 0.0;

		if (croppedImage.cols() > croppedImage.rows())
			scale = (double)Constant.MODEL_WIDTH / croppedImage.cols();
		else
			scale = (double)Constant.MODEL_HEIGHT / croppedImage.rows();

		Mat scaleImage = new Mat();
		Imgproc.resize(croppedImage, scaleImage, new Size(), scale, scale, Imgproc.INTER_AREA);

		int horMargin = (Constant.MODEL_WIDTH - scaleImage.cols())/2;
		int verMargin = (Constant.MODEL_HEIGHT - scaleImage.rows())/2;

		Mat resultImage = new Mat();
		Core.copyMakeBorder(scaleImage, resultImage, verMargin, verMargin, horMargin, horMargin, Core.BORDER_REPLICATE);

		if (resultImage.cols() != Constant.MODEL_WIDTH)
			Core.copyMakeBorder(resultImage, resultImage, 0, 0, Constant.MODEL_WIDTH - resultImage.cols(), 0, Core.BORDER_REPLICATE);
		if (resultImage.rows() != Constant.MODEL_HEIGHT)
			Core.copyMakeBorder(resultImage, resultImage, Constant.MODEL_HEIGHT - resultImage.rows(), 0, 0, 0, Core.BORDER_REPLICATE);
		return resultImage;
	}
}
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using OpenCVForUnity;


namespace AnimationDemo
{
	public static class Segmentation
	{
		[DllImport("__Internal")]
		private static extern IntPtr dll_SendArray(IntPtr array, int channel, int width, int height, int klass);
		[DllImport("__Internal")]
		private static extern int dll_ReleaseMemory(IntPtr ptr);


		public static Mat segment(Mat modelSizeImage)
		{
			float[] dataArray = mat2tensorArray(modelSizeImage);
			float[] segmentationResult = call_dll_SendArray(dataArray);

			int[] maskImageData = new int[Constant.MODEL_HEIGHT*Constant.MODEL_WIDTH];

			for (var i = 0; i < maskImageData.Length; i++)
			{
				float[] pixel = new float[Constant.NUM_OF_CLASS];
				for (var j = 0; j < pixel.Length; j++)
					pixel[j] = segmentationResult[i*Constant.NUM_OF_CLASS + j];
				// Change klass 0 ~ 6 to parts -1(bg), 0 ~ 5
				maskImageData[i] = softmax(pixel) - 1;
			}

			Mat maskImage = new Mat(Constant.MODEL_HEIGHT, Constant.MODEL_WIDTH, CvType.CV_8UC1);
			maskImage.put(0, 0, maskImageData);

			return maskImage;
		}


		public static void getLists(Mat originImage, Mat originMaskImage, out List<Texture2D>_partTexList, out List<Mat> _partMaskList, out List<OpenCVForUnity.Rect> _partBBList)
		{
			int originHeight = originMaskImage.rows();
			int originWidth  = originMaskImage.cols();

			int[] maskImageData = new int[originHeight*originWidth];
			originMaskImage.get(0, 0, maskImageData);

			List<Mat> partMaskList = new List<Mat>();
			for (var i = 0; i < Constant.NUM_OF_PARTS; i++)
				partMaskList.Add(new Mat(originHeight, originWidth, CvType.CV_8UC1, new Scalar(0)));

			for (var i = 0; i < originHeight; i++)
				for (var j = 0; j < originWidth; j++)
				{
					int part = maskImageData[i*originWidth + j];
					try{
						if (part == -1) continue;
						partMaskList[part].put(i, j, (byte)255);
					}
					catch(Exception ex)
					{
						Debug.Log("partImageList.count = " + partMaskList.Count);
						Debug.Log("part = " + part + " " + ex.Message);
						break;
					}
				}

			for (var i = 0; i < partMaskList.Count; i++)
			{
				Imgproc.morphologyEx(partMaskList[i], partMaskList[i], Imgproc.MORPH_OPEN,
					Imgproc.getStructuringElement(Imgproc.MORPH_ELLIPSE, new Size(Constant.MORPH_KERNEL_SIZE, Constant.MORPH_KERNEL_SIZE)));
			}				

			_partMaskList = partMaskList;
			_partBBList = getROIList(partMaskList);

			Mat originImageAlpha = new Mat();
			Imgproc.cvtColor(originImage, originImageAlpha, Imgproc.COLOR_BGR2BGRA);

			List<Texture2D> partTextureList = new List<Texture2D>();
			for (var i = 0; i < partMaskList.Count; i++)
			{
				Mat resultImage = new Mat(originHeight, originWidth, CvType.CV_8UC4, new Scalar(0, 0, 0, 0));
				originImageAlpha.copyTo(resultImage, partMaskList[i]);
				Mat cropImage = cropROI(resultImage, _partBBList[i]);
				removeBorder(cropImage);

				Texture2D tmpTex = new Texture2D(cropImage.width(), cropImage.height());
				Utils.matToTexture2D(cropImage, tmpTex);
				partTextureList.Add(tmpTex);
			}

			_partTexList = partTextureList;
		}





		/*public static void segment(Mat originImage, out List<Texture2D>_partTexList, out List<Mat> _partMaskList, out List<OpenCVForUnity.Rect> _partBBList)
		{
			float[] dataArray = mat2tensorArray(originImage);
			float[] segmentationResult = call_dll_SendArray(dataArray);

			int[] maskImageData = new int[Constant.MODEL_HEIGHT*Constant.MODEL_WIDTH];

			for (var i = 0; i < maskImageData.Length; i++)
			{
				float[] pixel = new float[Constant.NUM_OF_CLASS];
				for (var j = 0; j < pixel.Length; j++)
					pixel[j] = segmentationResult[i*Constant.NUM_OF_CLASS + j];
				// Change klass 0 ~ 6 to parts -1(bg), 0 ~ 5
				maskImageData[i] = softmax(pixel) - 1;
			}

			List<Mat> partMaskList = new List<Mat>();
			for (var i = 0; i < Constant.NUM_OF_PARTS; i++)
				partMaskList.Add(new Mat(Constant.MODEL_HEIGHT, Constant.MODEL_WIDTH, CvType.CV_8UC1, new Scalar(0)));

			for (var i = 0; i < Constant.MODEL_HEIGHT; i++)
				for (var j = 0; j < Constant.MODEL_WIDTH; j++)
				{
					int part = maskImageData[i*Constant.MODEL_WIDTH + j];
					try{
						if (part == -1) continue;
						partMaskList[part].put(i, j, (byte)255);
					}
					catch(Exception ex)
					{
						Debug.Log("partImageList.count = " + partMaskList.Count);
						Debug.Log("part = " + part + " " + ex.Message);
						break;
					}
				}

			for (var i = 0; i < partMaskList.Count; i++)
			{
				Imgproc.morphologyEx(partMaskList[i], partMaskList[i], Imgproc.MORPH_OPEN,
					Imgproc.getStructuringElement(Imgproc.MORPH_ELLIPSE, new Size(Constant.MORPH_KERNEL_SIZE, Constant.MORPH_KERNEL_SIZE)));
			}				
				
			_partMaskList = partMaskList;
			_partBBList = getROIList(partMaskList);

			Mat originImageAlpha = new Mat();
			Imgproc.cvtColor(originImage, originImageAlpha, Imgproc.COLOR_BGR2BGRA);

			List<Texture2D> partTextureList = new List<Texture2D>();
			for (var i = 0; i < partMaskList.Count; i++)
			{
				Mat resultImage = new Mat(Constant.MODEL_HEIGHT, Constant.MODEL_WIDTH, CvType.CV_8UC4, new Scalar(0, 0, 0, 0));
				originImageAlpha.copyTo(resultImage, partMaskList[i]);
				Mat cropImage = cropROI(resultImage, _partBBList[i]);
				removeBorder(cropImage);

				Texture2D tmpTex = new Texture2D(cropImage.width(), cropImage.height());
				Utils.matToTexture2D(cropImage, tmpTex);
				partTextureList.Add(tmpTex);
			}

			_partTexList = partTextureList;
		}*/


		private static float[] mat2tensorArray(Mat image)
		{
			byte [] byteArray  = new byte [image.rows()*image.cols()*image.channels()];
			float[] floatArray = new float[image.rows()*image.cols()*image.channels()];

			image.get(0, 0, byteArray); 

			for (var i = 0; i < byteArray.Length; i++)
				floatArray[i] = (float)(1 - (float)byteArray[i]/255.0);

			return floatArray;
		}


		private static float[] call_dll_SendArray(float[] dataArray)
		{
			unsafe {
				fixed (float* p = dataArray)
				{
					IntPtr array = (IntPtr)p;
					// do you stuff here
					IntPtr returnArray = dll_SendArray(array, Constant.MODEL_CHANNEL, Constant.MODEL_WIDTH, Constant.MODEL_HEIGHT, Constant.NUM_OF_CLASS);			

					Debug.Log("test_tensorflow.cs call_dll_SendArray.cs : finished running model!");

					float[] result = new float[Constant.MODEL_WIDTH*Constant.MODEL_HEIGHT*Constant.NUM_OF_CLASS];
					Marshal.Copy(returnArray, result, 0, Constant.MODEL_WIDTH*Constant.MODEL_HEIGHT*Constant.NUM_OF_CLASS);					

					GC.KeepAlive(returnArray);

					Debug.Log("test_tensorflow.cs call_dll_SendArray() : SUCCESS!");

					return result;
				}
			}
		}


		private static int softmax(float[] data)
		{
			int maxIndex = 0;
			float maxValue = 0;

			for (int i = 0; i < data.Length; i++)
				if (data[i] > maxValue)
				{
					maxValue = data[i];
					maxIndex = i;
				}
			return maxIndex;
		}


		private static List<OpenCVForUnity.Rect> getROIList(List<Mat> partMaskList)
		{
			List<OpenCVForUnity.Rect> roiList = new List<OpenCVForUnity.Rect>();
			for (var i = 0; i < partMaskList.Count; i++)
			{
				// Find Contours
				List<MatOfPoint> contours = new List<MatOfPoint>();
				Mat hierarchy = new Mat();
				Imgproc.findContours(partMaskList[i], contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE, new Point(0, 0));

				OpenCVForUnity.Rect roi = Imgproc.boundingRect(contours[0]);
				roiList.Add(roi);
			}
			return roiList;
		}


		private static Mat cropROI(Mat image, OpenCVForUnity.Rect roi)
		{
			return new Mat(image, roi);
		}


		// To deal with random black lines on cropped image border.
		private static void removeBorder(Mat image)
		{
			if (image.type() != CvType.CV_8UC4)
			{
				Debug.LogError("Segmentation.cs removeBorder() Err: Expect input CvType.CV_8UC4, got " + image.type());
				return;
			}
			byte[] zero = new byte[4] {0, 0, 0, 0};
			for (var i = 0; i < image.rows(); i++)
			{
				image.put(i, 0, zero);
				image.put(i, image.cols() - 1, zero);
			}
			for (var j = 0; j < image.cols(); j++)
			{
				image.put(0, j, zero);
				image.put(image.rows() - 1, j, zero);
			}
		}
	}
}
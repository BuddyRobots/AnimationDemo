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


		public static List<Texture2D> segment(Texture2D texture)
		{
			Mat originImage = new Mat(texture.height, texture.width, CvType.CV_8UC4);
			Utils.texture2DToMat(texture, originImage);

			float[] dataArray = texture2d2tensorArray(texture);










			//float[] segmentationResult = call_dll_SendArray(dataArray);

			float[] segmentationResult = new float[Constant.WIDTH*Constant.HEIGHT*Constant.NUM_OF_CLASS];











			int[] maskImageData = new int[Constant.HEIGHT*Constant.WIDTH];

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
				partMaskList.Add(new Mat(Constant.HEIGHT, Constant.WIDTH, CvType.CV_8UC1, new Scalar(0)));

			for (var i = 0; i < Constant.HEIGHT; i++)
				for (var j = 0; j < Constant.WIDTH; j++)
				{
					int part = maskImageData[i*Constant.WIDTH + j];
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
				Imgproc.morphologyEx(partMaskList[i], partMaskList[i],
					Imgproc.MORPH_OPEN, Imgproc.getStructuringElement(Imgproc.MORPH_ELLIPSE,
					new Size(Constant.MORPH_KERNEL_SIZE, Constant.MORPH_KERNEL_SIZE)));
			}

			List<Texture2D> partTextureList = new List<Texture2D>();

			for (var i = 0; i < partMaskList.Count; i++)
			{
				Mat resultImage = new Mat(Constant.HEIGHT, Constant.WIDTH, CvType.CV_8UC4, new Scalar(0, 0, 0, 0));
				originImage.copyTo(resultImage, partMaskList[i]);
				Mat cropImage = cropROI(resultImage, partMaskList[i]);
				removeBorder(cropImage);

				Texture2D tmpTex = new Texture2D(cropImage.width(), cropImage.height());
				Utils.matToTexture2D(cropImage, tmpTex);
				partTextureList.Add(tmpTex);
			}

			return partTextureList;
		}


		public static Texture2D loadPNG(string filePath)
		{
			return Resources.Load<Texture2D>(filePath);
		}


		private static float[] texture2d2tensorArray(Texture2D texture)
		{
			Mat image = texture2d2mat(texture);

			byte [] byteArray  = new byte [image.rows()*image.cols()*image.channels()];
			float[] floatArray = new float[image.rows()*image.cols()*image.channels()];

			image.get(0, 0, byteArray); 

			for (var i = 0; i < byteArray.Length; i++)
				floatArray[i] = (float)(1 - (float)byteArray[i]/255.0);

			return floatArray;
		}


		private static Mat texture2d2mat(Texture2D texture)
		{
			Mat image = new Mat(texture.height, texture.width, CvType.CV_8UC3);
			Utils.texture2DToMat(texture, image);
			return image;
		}
			

		private static float[] call_dll_SendArray(float[] dataArray)
		{
			unsafe {
				fixed (float* p = dataArray)
				{
					IntPtr array = (IntPtr)p;
					// do you stuff here
					IntPtr returnArray = dll_SendArray(array, Constant.CHANNEL, Constant.WIDTH, Constant.HEIGHT, Constant.NUM_OF_CLASS);			

					Debug.Log("test_tensorflow.cs call_dll_SendArray.cs : finished running model!");

					float[] result = new float[Constant.WIDTH*Constant.HEIGHT*Constant.NUM_OF_CLASS];
					Marshal.Copy(returnArray, result, 0, Constant.WIDTH*Constant.HEIGHT*Constant.NUM_OF_CLASS);					

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


		private static Mat cropROI(Mat image, Mat mask)
		{
			// Find Contours
			List<MatOfPoint> contours = new List<MatOfPoint>();
			Mat hierarchy = new Mat();
			Imgproc.findContours(mask, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_SIMPLE, new Point(0, 0));

			OpenCVForUnity.Rect roi = Imgproc.boundingRect(contours[0]);

			Mat result = new Mat(image, roi);

			return result;
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
			

		private static void test_createPicture(ref Texture2D texture, ref Mat image)
		{
			byte[] data = new byte[image.rows()*image.cols()*image.channels()];

			for (var i = 0; i < image.rows(); i++)
				for (var j = 0; j < image.cols(); j++)
					for (var k = 0; k < image.channels(); k++)
						data[i*Constant.WIDTH*Constant.CHANNEL + j*Constant.CHANNEL + k] = (byte)((2 - k)*255/(Constant.CHANNEL - 1));

			image.put(0, 0, data);
			Utils.matToTexture2D(image, texture);
		}


		private static void test_klassImage_statistics(byte[] klassImage)
		{
			int[] statistics = new int[6];
			for (var i = 0; i < statistics.Length; i++)
				statistics[i] = 0;

			Debug.Log("klassImage.Length = " + klassImage.Length);

			for (var i = 0; i < klassImage.Length; i++)
				statistics[klassImage[i]]++;

			for (var i = 0; i < statistics.Length; i++)
				Debug.Log("test_klassImage_statistics["+i+"] = " + statistics[i]);
		}
	}
}
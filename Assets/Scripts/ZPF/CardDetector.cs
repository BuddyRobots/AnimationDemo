using UnityEngine;
using System.Collections.Generic;
using OpenCVForUnity;
using MagicCircuit;

public class CardDetector
{	
    public static List<List<Point>> findSquares(Mat binaryImg)
    {
        List<List<Point>> squares = new List<List<Point>>();
        List<MatOfPoint> contours = new List<MatOfPoint>();

        Imgproc.findContours(binaryImg, contours, new Mat(), Imgproc.RETR_LIST, Imgproc.CHAIN_APPROX_SIMPLE, new Point(0, 0));

        List<MatOfPoint2f> contours2f = new List<MatOfPoint2f>(contours.Count);

        for (int i = 0; i < contours.Count; i++)
        {
            MatOfPoint2f t = new MatOfPoint2f();
            contours[i].convertTo(t, CvType.CV_32FC2);
            contours2f.Add(t);
        }

        MatOfPoint2f approx2f = new MatOfPoint2f();
        for (int i = 0; i < contours.Count; i++)
        {
            Imgproc.approxPolyDP(contours2f[i], approx2f, Imgproc.arcLength(contours2f[i], true) * 0.05, true);
            //Imgproc.approxPolyDP(contours2f[i], approx2f, 0.003, true);

            MatOfPoint approx = new MatOfPoint();
            approx2f.convertTo(approx, CvType.CV_32S);

            if (approx.size().height == 4 && Mathf.Abs((float)Imgproc.contourArea(approx)) > 1000 && Imgproc.isContourConvex(approx))
            {
                double maxCosine = 0;
                for (int j = 2; j < 5; j++)
                {
                    double cosine = Mathf.Abs(Angle(approx.toArray()[j % 4], approx.toArray()[j - 2], approx.toArray()[j - 1]));
                    maxCosine = maxCosine > cosine ? maxCosine : cosine;
                }

                if (maxCosine < 0.8)
                    squares.Add(approx.toList());
            }
        }

        return filterSquares(squares);
    }

    public static List<List<Point>> computeOuterSquare(List<List<Point>> squareList)
    {
		List<List<Point>> outerSquareList = new List<List<Point>>();

        for (var i = 0; i < squareList.Count; i++)
        {
			List<Point> tmpSquare = new List<Point>();
			Point squareCenter = new Point((squareList[i][0].x + squareList[i][2].x) / 2, (squareList[i][0].y + squareList[i][2].y) / 2);

            for (var j = 0; j < 4; j++)
            {
				double x = Constant.CARD_OUTER_SQUARE_RATIO * (squareList[i][j].x - squareCenter.x) + squareCenter.x;
				double y = Constant.CARD_OUTER_SQUARE_RATIO * (squareList[i][j].y - squareCenter.y) + squareCenter.y;
                tmpSquare.Add(new Point(x, y));
            }



			//Debug.Log("CardDetector.cs computeOuterSquare : squareList[" + i + "] : " + squareList[i][0] + " " + squareList[i][1] + " " + squareList[i][2] + " " + squareList[i][3]);
			//Debug.Log("CardDetector.cs computeOuterSquare : outerSquareList[" + i + "] : " + tmpSquare[0] + " " + tmpSquare[1] + " " + tmpSquare[2] + " " + tmpSquare[3]);









            outerSquareList.Add(tmpSquare);
        }
        return outerSquareList;
    }

    private static List<List<Point>> filterSquares(List<List<Point>> squares)
    {
        List<List<Point>> filteredSquares = new List<List<Point>>();

        for (int j = 0; j < squares.Count; j++)
        {
            double len, curMaxLen = 0, curMinLen = 10000;

            for (int i = 0; i < 3; i++)
            {
                len = findLen(squares[j][i % 4], squares[j][(i + 1) % 4]);
                curMaxLen = len > curMaxLen ? len : curMaxLen;
                curMinLen = len < curMinLen ? len : curMinLen;
            }



			//Debug.Log("CardDetector : curMaxLen = " + curMaxLen + " curMinLen = " + curMinLen + " ratio = " + curMaxLen / curMinLen);




			if (curMaxLen > Constant.CARD_MAX_SQUARE_LEN || curMinLen < Constant.CARD_MIN_SQUARE_LEN || curMaxLen / curMinLen > Constant.CARD_MAX_SQUARE_LEN_RATIO)
                continue;
            if (isSquareClockwise(squares[j]))
				filteredSquares.Add(squares[j]);
        }
        return filteredSquares;
    }

    private static double findLen(Point p1, Point p2, bool sqrt_v = true)
    {
        float v = Mathf.Pow((float)(p1.x - p2.x), 2) + Mathf.Pow((float)(p1.y - p2.y), 2);
        return sqrt_v ? Mathf.Sqrt(v) : v;
    }

    private static bool isSquareClockwise(List<Point> square)
    {
        bool clockwise;
        int direction;

        if (Mathf.Abs((float)(square[0].x - square[1].x)) > Mathf.Abs((float)(square[0].y - square[0].y)))
        {
            direction = square[1].x > square[0].x ? 0 : 1;
        }
        else
        {
            direction = square[1].y > square[0].y ? 2 : 3;
        }

        if (direction == 0 || direction == 1)
        {
            int second_direction = square[2].y > square[1].y ? 2 : 3;
            clockwise = (direction == 0 && second_direction == 2) || (direction == 1 && second_direction == 3);
        }
        else
        {
            int second_direction = square[2].x > square[1].x ? 0 : 1;
            clockwise = (direction == 2 && second_direction == 1) || (direction == 3 && second_direction == 0);
        }
        return clockwise;
    }

    private static float Angle(Point pt1, Point pt2, Point pt0)
    {
        double dx1 = pt1.x - pt0.x;
        double dy1 = pt1.y - pt0.y;
        double dx2 = pt2.x - pt0.x;
        double dy2 = pt2.y - pt0.y;
        return (float)(dx1 * dx2 + dy1 * dy2) / Mathf.Sqrt((float)((dx1 * dx1 + dy1 * dy1) * (dx2 * dx2 + dy2 * dy2) + 1e-10));
    }		        

    /*public static void Img2Pts(Mat image, ref List<Point> imgPts)
    {
        Core.bitwise_not(image, image);
        imgPts = new List<Point>();
        Mat mat = new Mat();
        Core.findNonZero(image, mat);
        Converters.Mat_to_vector_Point(mat, imgPts);
    }

    public static void FindHullCircle(List<Point> imgPts, ref Point center, ref float radius)
    {
        MatOfInt hull = new MatOfInt();
        MatOfPoint imgPtsMat = new MatOfPoint(imgPts.ToArray());
        Imgproc.convexHull(imgPtsMat, hull, false);

        /// the center point is at the average of all hull points
        center = new Point(0, 0);
        for (int i = 0; i < hull.rows(); i++)
        {
            center.x += imgPts[(int)hull.get(i, 0)[0]].x;
            center.y += imgPts[(int)hull.get(i, 0)[0]].y;
        }
        center.x = center.x / hull.rows();
        center.y = center.y / hull.rows();

        /// then traverse all points to find out the radius
        Point radiusP = new Point(center.x, center.y);
        radius = 0;
        float temp;
        for (int i = 0; i < hull.rows(); i++)
        {
            if (Mathf.Abs((float)(imgPts[(int)hull.get(i, 0)[0]].x - center.x)) < Mathf.Abs((float)(radiusP.x - center.x)) && Mathf.Abs((float)(imgPts[(int)hull.get(i, 0)[0]].y - center.y)) < Mathf.Abs((float)(radiusP.y - center.y)))
            {
                continue;
            }
            temp = Mathf.Pow((float)(imgPts[(int)hull.get(i, 0)[0]].x - center.x), 2) + Mathf.Pow((float)(imgPts[(int)hull.get(i, 0)[0]].y - center.y), 2);
            if (temp > radius)
            {
                radius = temp;
                radiusP = imgPts[(int)hull.get(i, 0)[0]];
            }
        }
        radius = Mathf.Sqrt(radius);
    }

    public static List<float> FindCircularDist(Mat image, int binNum, Point center)
    {
        List<float> dist = new List<float>(binNum);
        for (int i = 0; i < binNum; i++)
        {
            dist.Add(0.0f);
        }
        int binIndex;
        int totP = 0;

        int x, y;
        byte[] data = new byte[1];
        float len, radian;

        for (int row = 0; row < image.rows(); row++)
        {
            for (int col = 0; col < image.cols(); col++)
            {
                image.get(row, col, data);
                if (data[0] != 0)
                {
                    continue;
                }
                x = col - (int)center.x;
                y = (int)center.y - row;

                if (x == 0 && y == 0)
                {
                    continue;
                }

                len = Mathf.Sqrt(x * x + y * y);
                radian = Mathf.Asin(y / len);
                if (x > 0 && y >= 0)
                    radian = radian;
                else if (x <= 0 && y > 0)
                    radian = Mathf.PI - radian;
                else if (x < 0 && y <= 0)
                    radian = Mathf.PI - radian;
                else if (x >= 0 && y < 0)
                    radian = 2 * Mathf.PI + radian;
                else
                    continue;
                binIndex = Mathf.FloorToInt(radian * binNum / 2 / Mathf.PI);
                if (binIndex < 0)
                {
                    binIndex = 0;
                }
                else if (binIndex >= binNum)
                {
                    binIndex = binNum - 1;
                }
                dist[binIndex] += 1;
                totP += 1;
            }
        }

        /// normalize the circular distribution
        for (int i = 0; i < binNum; i++)
        {
            dist[i] = dist[i] / totP;
        }

        return dist;
    }

    public static Mat GenImgFromPts(int width, int height, List<Point> pts)
    {
        Mat image = new Mat(height, width, CvType.CV_8U, new Scalar(255));
        for (int i = 0; i < pts.Count; i++)
        {
            image.put((int)pts[i].y, (int)pts[i].x, new byte[1]);
        }
        return image;
    }

    public static Mat ToMnistFormat(List<Point> cluster)
    {
        List<int> ret = new List<int>();

        List<Point> stdSquare = new List<Point>();
        stdSquare.Add(new Point(4, 4));
        stdSquare.Add(new Point(4, 23));
        stdSquare.Add(new Point(23, 23));
        stdSquare.Add(new Point(23, 4));

        List<Point> clusterSquare = new List<Point>();
        double minX = 10000, maxX = -1, minY = 10000, maxY = -1;
        for (int i = 0; i < cluster.Count; i++)
        {
            minX = cluster[i].x < minX ? cluster[i].x : minX;
            maxX = cluster[i].x > maxX ? cluster[i].x : maxX;
            minY = cluster[i].y < minY ? cluster[i].y : minY;
            maxY = cluster[i].y > maxY ? cluster[i].y : maxY;
        }
        double xSpan = maxX - minX, ySpan = maxY - minY;
        double diff = Mathf.Abs((float)(xSpan - ySpan));
        if (xSpan >= ySpan)
        {
            minY = minY - diff / 2;
            maxY = maxY + diff / 2;
        }
        else
        {
            minX = minX - diff / 2;
            maxX = maxX + diff / 2;
        }
        clusterSquare.Add(new Point(minX, minY));
        clusterSquare.Add(new Point(minX, maxY));
        clusterSquare.Add(new Point(maxX, maxY));
        clusterSquare.Add(new Point(maxX, minY));

        Mat transform = new Mat();
        transform = Calib3d.findHomography(new MatOfPoint2f(clusterSquare.ToArray()), new MatOfPoint2f(stdSquare.ToArray()));

        Mat img = GenImgFromPts(Mathf.RoundToInt((float)(2 * maxX)), Mathf.RoundToInt((float)(2 * maxY)), cluster);
        Mat result = new Mat(28, 28, CvType.CV_8UC4);
        Imgproc.warpPerspective(img, result, transform, new Size(28, 28));
        return result;
    }*/
}
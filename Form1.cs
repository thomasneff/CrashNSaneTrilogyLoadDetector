using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Drawing;
// using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Timers;
using System.IO;

namespace autosplittercnn_test
{
	public partial class Form1 : Form
	{
		System.Timers.Timer captureTimer;
		System.Timers.Timer gameTimer;
		System.Timers.Timer realTimer;
		List<List<int>> list_of_feature_vectors;
		List<bool> histogram_matches;
		List<long> ms_elapsed;
		List<int> pause_matching_bins;
		List<int> run_matching_bins;
		bool run_matching = false;
		bool currently_paused = false;
		DateTime oldGameTime;
		DateTime oldRealTime;
		DateTime timerBegin;
		TimeSpan gameTime;
		TimeSpan loadTime;
		TimeSpan loadTimeTemp;
		DateTime loadStart;
		TimeSpan realTime;
		int frameCount = 0;
		int lastRunningFrame = 0;
		int lastPausedFrame = 0;
		int lastSaveFrame = 0;
		bool recordMode = false;
		bool wasPaused = false;
		int matching_bins = 0;

		int[] feature_vector_loading_english = { 628, 1247, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 628, 1247, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 629, 1246, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 638, 1171, 23, 15, 7, 5, 6, 1, 1, 1, 1, 1, 1, 2, 2, 625, 637, 1171, 23, 15, 7, 5, 6, 1, 1, 1, 1, 1, 1, 2, 2, 625, 632, 1174, 23, 16, 7, 5, 7, 1, 1, 1, 1, 1, 1, 2, 2, 625, 464, 817, 20, 15, 13, 14, 10, 12, 15, 13, 22, 24, 142, 23, 269, 625, 459, 820, 22, 16, 13, 15, 11, 13, 14, 14, 23, 24, 142, 25, 265, 625, 457, 808, 23, 17, 13, 16, 11, 13, 14, 15, 24, 25, 152, 27, 260, 625, 639, 1236, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 639, 1236, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 639, 1236, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 529, 1031, 17, 25, 36, 9, 12, 17, 32, 7, 9, 11, 20, 44, 75, 625, 530, 1029, 17, 25, 36, 9, 13, 17, 32, 7, 9, 11, 20, 44, 75, 625, 530, 1020, 17, 25, 36, 9, 13, 17, 32, 7, 9, 11, 22, 49, 74, 625, 232, 425, 26, 23, 15, 13, 19, 12, 14, 16, 21, 25, 337, 65, 631, 625, 230, 423, 25, 23, 15, 13, 19, 12, 13, 15, 21, 26, 336, 66, 638, 625, 228, 412, 23, 24, 14, 12, 19, 12, 13, 14, 21, 27, 344, 66, 646, 625, 639, 1236, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 638, 1237, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 637, 1238, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 514, 911, 37, 24, 16, 15, 9, 10, 9, 9, 10, 13, 26, 100, 171, 625, 501, 913, 38, 24, 17, 15, 9, 11, 9, 10, 11, 13, 26, 101, 179, 625, 504, 906, 39, 23, 17, 15, 8, 11, 9, 11, 11, 13, 25, 96, 187, 625, 63, 142, 32, 27, 19, 26, 17, 22, 25, 24, 31, 38, 478, 55, 878, 625, 58, 140, 32, 26, 19, 26, 17, 22, 25, 24, 30, 37, 477, 54, 888, 625, 58, 133, 33, 25, 19, 27, 16, 21, 25, 25, 30, 35, 476, 54, 899, 625, 619, 1256, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 620, 1255, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 622, 1253, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 448, 791, 46, 23, 31, 17, 18, 20, 25, 17, 15, 24, 37, 129, 234, 625, 458, 790, 46, 23, 30, 17, 18, 20, 25, 16, 14, 25, 37, 129, 227, 625, 455, 790, 45, 23, 30, 18, 17, 20, 26, 16, 13, 26, 38, 138, 220, 625, 165, 261, 19, 27, 24, 15, 16, 15, 13, 26, 20, 26, 394, 57, 797, 625, 174, 263, 19, 26, 24, 15, 16, 15, 13, 26, 20, 26, 394, 56, 788, 625, 176, 279, 18, 26, 24, 14, 16, 15, 13, 24, 20, 26, 390, 55, 779, 625, 623, 1252, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 622, 1253, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 621, 1254, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 415, 806, 32, 23, 42, 22, 23, 9, 36, 11, 21, 30, 61, 132, 212, 625, 403, 815, 36, 23, 42, 21, 23, 9, 37, 11, 21, 30, 61, 131, 213, 625, 402, 818, 42, 22, 42, 20, 24, 9, 36, 11, 22, 29, 61, 123, 214, 625, 325, 530, 21, 20, 10, 4, 8, 6, 11, 10, 17, 12, 282, 29, 590, 625, 319, 537, 22, 22, 12, 5, 9, 7, 12, 10, 17, 12, 283, 29, 579, 625, 318, 541, 23, 23, 13, 6, 11, 9, 14, 10, 17, 12, 281, 30, 569, 625, 637, 1238, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 638, 1237, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 639, 1236, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 449, 887, 49, 20, 21, 24, 17, 16, 19, 16, 20, 24, 37, 103, 173, 625, 464, 879, 44, 20, 21, 24, 17, 16, 18, 16, 20, 24, 37, 103, 171, 625, 464, 881, 38, 21, 21, 24, 16, 16, 19, 16, 19, 24, 37, 109, 169, 625, 308, 558, 20, 16, 14, 10, 11, 14, 13, 13, 15, 21, 290, 34, 540, 625, 307, 551, 19, 14, 13, 9, 10, 13, 12, 13, 15, 21, 290, 35, 552, 625, 307, 543, 19, 13, 12, 8, 9, 12, 11, 14, 16, 21, 289, 37, 564, 625, 641, 1234, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 640, 1235, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 640, 1235, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 483, 902, 27, 16, 9, 9, 7, 9, 13, 8, 11, 16, 17, 123, 225, 625, 473, 901, 27, 16, 9, 9, 7, 9, 13, 8, 11, 17, 18, 123, 234, 625, 474, 899, 27, 17, 8, 9, 7, 9, 14, 8, 11, 17, 17, 115, 244, 625, 259, 452, 38, 23, 25, 18, 19, 23, 19, 18, 25, 29, 306, 40, 582, 625, 263, 451, 39, 23, 24, 19, 19, 23, 19, 18, 25, 29, 306, 39, 580, 625, 264, 452, 39, 23, 24, 20, 19, 22, 19, 18, 25, 30, 305, 39, 578, 625, 648, 1227, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 650, 1225, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 650, 1225, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 470, 854, 25, 42, 23, 13, 12, 11, 16, 25, 31, 19, 42, 102, 190, 625, 477, 855, 25, 42, 23, 13, 12, 12, 15, 25, 31, 19, 42, 103, 182, 625, 480, 855, 26, 41, 23, 14, 12, 13, 14, 26, 31, 19, 42, 104, 174, 625, 305, 501, 28, 21, 15, 21, 13, 18, 16, 14, 22, 29, 283, 48, 541, 625, 311, 500, 27, 21, 15, 20, 13, 18, 16, 14, 21, 29, 283, 48, 540, 625, 312, 507, 26, 20, 15, 19, 13, 18, 17, 14, 20, 28, 281, 46, 538, 625, 634, 1241, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 634, 1241, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 633, 1242, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 625, 589, 1003, 34, 22, 15, 18, 7, 10, 13, 12, 15, 13, 27, 36, 59, 625, 593, 1004, 34, 22, 15, 18, 7, 9, 13, 11, 15, 13, 27, 36, 58, 625, 593, 1013, 33, 21, 14, 17, 7, 8, 13, 11, 15, 12, 26, 34, 57, 625, 403, 673, 29, 21, 19, 17, 17, 15, 17, 15, 20, 26, 199, 33, 370, 625, 402, 673, 29, 22, 20, 17, 17, 15, 18, 15, 20, 26, 200, 33, 368, 625, 402, 682, 29, 22, 20, 17, 17, 15, 18, 15, 20, 26, 192, 33, 367, 625 };

		float percent_of_bins_correct = 0.7f; //percentage of histogram bins which have to match for loading detection
		float variance_of_bins_allowed = 0.3f; //amount of variance allowed for correct comparison
		int numberOfBins = 16;

		public Form1()
		{
			InitializeComponent();
			captureTimer = new System.Timers.Timer();
			
			if(recordMode)
			{
				captureTimer.Elapsed += recordFeature;
			}
			else
			{
				captureTimer.Elapsed += CaptureAndUpdate;
			}
			
			oldGameTime = DateTime.Now;
			oldRealTime = DateTime.Now;
			captureTimer.Interval = 33;
			gameTimer = new System.Timers.Timer();
			realTimer = new System.Timers.Timer();
			gameTimer.Interval = 33;
			realTimer.Interval = 33;
			gameTimer.Elapsed += countGameTime;
			realTimer.Elapsed += countRealTime;
			list_of_feature_vectors = new List<List<int>>();
			histogram_matches = new List<bool>();
			ms_elapsed = new List<long>();
			pause_matching_bins = new List<int>();
			run_matching_bins = new List<int>();

			//captureTimer.Enabled = true;
		}

		
		private void countGameTime(object source, ElapsedEventArgs e)
		{
			if(currently_paused == true && wasPaused == false)
			{
				//gameTime = DateTime.Now - oldGameTime;
				//starting loading
				loadStart = DateTime.Now;
				loadTimeTemp = loadTime;
			}
			else if (currently_paused == false && wasPaused == true)
			{
				//oldGameTime = DateTime.Now;
				//loading done
				loadTime = loadTimeTemp + (DateTime.Now - loadStart);
			}
			else if (currently_paused == true && wasPaused == true)
			{
				loadTime = DateTime.Now - loadStart + loadTimeTemp;
				//lastLoadTimeTemp = DateTime.Now;
			}
			gameTime = realTime - loadTime;

			Invoke(new Action(() =>
			{
				label2.Text = gameTime.ToString();
				label4.Text = loadTime.ToString();
			}));

			
			wasPaused = currently_paused;
		}

		private void countRealTime(object source, ElapsedEventArgs e)
		{
			
			realTime = DateTime.Now - timerBegin;

			Invoke(new Action(() =>
			{
				label3.Text = realTime.ToString();
				
			}));
			oldRealTime = DateTime.Now;
		}

		private bool compareFeatureVector(int[] comparison_vector, int[] new_vector)
		{
			int size = new_vector.Length;

			if(comparison_vector.Length < size)
			{
				size = comparison_vector.Length;
			}

			int number_of_bins_needed = 290;// (int) (size * percent_of_bins_correct);
			matching_bins = 0;
			for(int bin = 0; bin < size; bin++)
			{
				//Determine upper/lower histogram ranges for matching bins
				int lower_bound = (int)(comparison_vector[bin] * (1.0f - variance_of_bins_allowed));
				int upper_bound = (int)(comparison_vector[bin] * (1.0f + variance_of_bins_allowed));

				if(new_vector[bin] < upper_bound && new_vector[bin] > lower_bound)
				{
					matching_bins++;
					
				}


			}

			System.Console.WriteLine("Matching bins: " + matching_bins);
			if(run_matching)
			{
				run_matching_bins.Add(matching_bins);
			}
			else
			{
				pause_matching_bins.Add(matching_bins);
			}

			if (matching_bins >= number_of_bins_needed)
			{
				//if we found enough similarities, we found a match.
				return true;
			}
			return false;
		}

		private void recordFeature(object source, ElapsedEventArgs e)
		{
			list_of_feature_vectors.Add(featuresAtScreenCenter());
		}

		private List<int> featuresFromBitmap(Bitmap capture)
		{

			List<int> features = new List<int>();
			
			int patch_size = 50;
			BitmapData bData = capture.LockBits(new Rectangle(0, 0, capture.Width, capture.Height), ImageLockMode.ReadWrite, capture.PixelFormat);

			byte bitsPerPixel = 32;
			int size = bData.Stride * bData.Height;

			byte[] data = new byte[size];
			
			/*This overload copies data of /size/ into /data/ from location specified (/Scan0/)*/
			System.Runtime.InteropServices.Marshal.Copy(bData.Scan0, data, 0, size);
			int y_add = 0;
			int r = 0;
			int g = 0;
			int b = 0;
			//we look at 50x50 patches and compute histogram bins for the a/r/g/b values.
			
			int stride = 1; //spacing between feature pixels

			Stopwatch stopwatch = Stopwatch.StartNew();
			for (int patch_x = 0; patch_x < (capture.Width / patch_size); patch_x++)
			{
				for (int patch_y = 0; patch_y < (capture.Height / patch_size); patch_y++)
				{
					
					//int[] patch_hist_a = new int[numberOfBins];
					int[] patch_hist_r = new int[numberOfBins];
					int[] patch_hist_g = new int[numberOfBins];
					int[] patch_hist_b = new int[numberOfBins];

					int x_start = patch_x * (patch_size * stride);
					int y_start = patch_y * (patch_size * stride);
					int x_end = (patch_x + 1) * (patch_size * stride);
					int y_end = (patch_y + 1) * (patch_size * stride);
					
					for (int x_index = x_start; x_index < x_end; x_index+=stride)
					{
						for (int y_index = y_start; y_index < y_end; y_index+=stride)
						{
							y_add = y_index * capture.Width;
							//int alpha = Convert.ToInt32(data[x_index + y_index * capture.Width]);
							r = (int)(data[x_index + y_add + 1]);
							g = (int)(data[x_index + y_add + 2]);
							b = (int)(data[x_index + y_add + 3]);
							//patch_hist_a[(alpha * numberOfBins) / 256]++;
							patch_hist_r[(r * numberOfBins) / 256]++;
							patch_hist_g[(g * numberOfBins) / 256]++;
							patch_hist_b[(b * numberOfBins) / 256]++;

						}
					}

					//enter the histograms as our features
					//features.AddRange(patch_hist_a);
					features.AddRange(patch_hist_r);
					features.AddRange(patch_hist_g);
					features.AddRange(patch_hist_b);
				}

				
			}
			stopwatch.Stop();
			//Console.WriteLine("Copy: " + stopwatch.ElapsedMilliseconds);

			capture.UnlockBits(bData);

			return features;
		}

		private List<int> featuresAtScreenCenter()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			Bitmap bmp = null;

			// Position the Form on The screen taking in account
			// the resolution
			Rectangle screenRect = Screen.GetBounds(Bounds);
			// get the Screen Boundy
			//ClientSize = new Size((int)(screenRect.Width / 2), (int)(screenRect.Height / 2)); // set the size of the form
			Point screenCenter = new Point(screenRect.Width / 2, screenRect.Height / 2);

			Size captureSize = new Size(450, 150);

			bmp = CaptureImage(screenCenter.X, screenCenter.Y - 40, captureSize.Width, captureSize.Height);
			//Bitmap bmp2 = CaptureImage(screenCenter.X - 200, screenCenter.Y - 200, 10, 10);
			//Bitmap bmp3 = CaptureImage(screenCenter.X + 200, screenCenter.Y + 200, 50, 50);
			//stopwatch.Stop();
			//Console.WriteLine("Capture: " + stopwatch.ElapsedMilliseconds);

			//stopwatch = Stopwatch.StartNew();
			//Compute some very basic feature vector which is still real-time capable

			List<int> features = featuresFromBitmap(bmp);

			//stopwatch.Stop();
			//Console.WriteLine("Capture+Features: " + stopwatch.ElapsedMilliseconds);
			//stopwatch = Stopwatch.StartNew();
			bool matching_histograms = compareFeatureVector(feature_vector_loading_english, features.ToArray());
			currently_paused = matching_histograms;
			stopwatch.Stop();
			

			ms_elapsed.Add(stopwatch.ElapsedMilliseconds);
		    if(ms_elapsed.Count > 20)
			{
				long sum = 0;

				foreach(var ms in ms_elapsed)
				{
					sum += ms;
				}
				sum /= ms_elapsed.Count;
				ms_elapsed.Clear();
				Console.WriteLine("DetectMatch (avg): " + sum + "ms");
			}

			if (currently_paused)
			{
				//only save if we haven't saved for at least 10 frames, just for diagnostics to see if any false positives are in there.
				//or if we haven't seen a paused frame for at least 30 frames.
				if(frameCount > (lastSaveFrame + 10) || (frameCount - lastPausedFrame) > 30 )
				{
					bmp.Save("imgs_stopped/img_" + frameCount + "_" + matching_bins + ".jpg", ImageFormat.Jpeg);
					lastSaveFrame = frameCount;
				}
				//using (var newBitmap = new Bitmap(bmp))
				//{
				//	newBitmap.Save("imgs/img_" + img_debug_counter + ".jpg", ImageFormat.Jpeg);
				//}
				//
				lastPausedFrame = frameCount;
			}
			else
			{
				//save if we haven't seen a running frame for at least 30 frames (to detect false runs - e.g. aku covering "loading"
				if ((frameCount - lastRunningFrame) > 30)
				{
					bmp.Save("imgs_running/img_" + frameCount + "_" + matching_bins + ".jpg", ImageFormat.Jpeg);
					lastSaveFrame = frameCount;
				}
				lastRunningFrame = frameCount;
			}
			
			frameCount++;
			histogram_matches.Add(matching_histograms);
			Invoke(new Action(() =>
			{
				pictureBox1.Image = bmp;
				pictureBox1.Size = new Size(captureSize.Width, captureSize.Height);
				pictureBox1.BackgroundImage = bmp;
				pictureBox1.Refresh();
				label1.Text = string.Join(";", features);
				if(matching_histograms)
				{
					//paused
					panel1.BackColor = Color.Red;
				}
				else
				{
					//paused
					panel1.BackColor = Color.Green;
				}
				
			}));
			bmp.Dispose();
			return features;
		}

		private void CaptureAndUpdate(object source, ElapsedEventArgs e)
		{

			List<int> features = featuresAtScreenCenter();
			
			
		}

		//private float mse_to_loading(List<int> features)
		//{
		//	List
		//}

		private static Bitmap CaptureImage(int x, int y, int width, int height)
		{
			Bitmap b = new Bitmap(width, height);
			using (Graphics g = Graphics.FromImage(b))
			{
				g.CopyFromScreen(x - width / 2, y - height / 2, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
			}
			return b;
		}
		private void button1_Click(object sender, EventArgs e)
		{
			//CaptureAndUpdate(sender, null);
			captureTimer.Enabled = !captureTimer.Enabled;

			if(recordMode)
			{
				if (captureTimer.Enabled == false)
				{
					//TODO: save features to .csv
					using (var file = File.CreateText("loading_eng.csv"))
					{
						foreach (var list in list_of_feature_vectors)
						{
							file.WriteLine(string.Join(",", list));
						}
					}

					list_of_feature_vectors.Clear();
				}

			}
			else
			{
				oldGameTime = DateTime.Now;
				oldRealTime = DateTime.Now;
				timerBegin = DateTime.Now;
				loadStart = DateTime.Now;
				pause_matching_bins = new List<int>();
				run_matching_bins = new List<int>();
				realTime = new TimeSpan(0);
				gameTime = new TimeSpan(0);
				loadTime = new TimeSpan(0);
				loadTimeTemp = new TimeSpan(0);
				
				currently_paused = false;
				frameCount = 0;
				lastRunningFrame = 0;
				lastPausedFrame = 0;
				lastSaveFrame = 0;
				ms_elapsed.Clear();

				gameTimer.Enabled = !gameTimer.Enabled;
				realTimer.Enabled = !realTimer.Enabled;
			}
			/*captureTimer.Enabled = !captureTimer.Enabled;
			
			*/



		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (run_matching)
			{
				using (var file = File.CreateText("run_matching.csv"))
				{
					file.WriteLine(string.Join(",", run_matching_bins));
				}
			}
			else
			{
				using (var file = File.CreateText("pause_matching.csv"))
				{
					file.WriteLine(string.Join(",", pause_matching_bins));
				}
			}
			run_matching = !run_matching;
			
		}
	}
}


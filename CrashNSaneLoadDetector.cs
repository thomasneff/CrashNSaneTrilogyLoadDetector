using CrashNSaneLoadDetector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
 
namespace CrashNSaneLoadDetector
{
	public partial class CrashNSaneLoadDetector : Form
	{
		#region Private Fields

		private Size captureSize = new Size(300, 100);
		private float featureVectorResolutionX = 1920.0f;
		private float featureVectorResolutionY = 1080.0f;

		private float captureAspectRatioX = 16.0f;
		private float captureAspectRatioY = 9.0f;

		private float cropOffsetX = 0.0f;
		private float cropOffsetY = -40.0f;

		private ImageCaptureInfo imageCaptureInfo;

		private System.Timers.Timer captureTimer;
		private bool currentlyPaused = false;

		private int frameCount = 0;
		private TimeSpan gameTime;
		private System.Timers.Timer gameTimer;
		private List<bool> histogramMatches;
		private List<int> lastFeatures;
		private int lastPausedFrame = 0;
		private int lastRunningFrame = 0;
		private int lastSaveFrame = 0;
		private List<List<int>> listOfFeatureVectors;

		private DateTime loadStart;
		private TimeSpan loadTime;
		private TimeSpan loadTimeTemp;
		private int matchingBins = 0;
		private int milliSecondsBetweenSnapshots = 500;
		private List<long> msElapsed;

		

		private int numberOfBins = 16;

		//float percent_of_bins_correct = 0.7f; //percentage of histogram bins which have to match for loading detection
		private int numberOfBinsCorrect = 520;

		//Just used for diagnostics, to check how fast the detection works
		private DateTime oldGameTime;

		private DateTime oldRealTime;

		private int patchSizeX = 50;

		private int patchSizeY = 50;

		private TimeSpan realTime;

		private System.Timers.Timer realTimer;

		private bool recordMode = false;

		private bool saveDiagnosticImages = true;

		private List<List<int>> segmentFeatureVectors;

		private List<int> segmentFrameCounts;

		private List<int> segmentMatchingBins;

		//TODO: factor into a separate class?
		private List<Bitmap> segmentSnapshots;

		private int snapshotFrameCount = 0;
		private int snapshotMilliseconds = 500;
		private int timerIntervalMilliseconds = 20;
		private DateTime timerBegin;
		private string DiagnosticsFolderName = "CrashNSTDiagnostics/";

		//used as a cutoff for when a match is detected correctly
		private float varianceOfBinsAllowed = 0.2f;

		private bool wasPaused = false;

		private KeyHandler StartKeyHandler;
		private KeyHandler ResetKeyHandler;

		private Process[] processList;

		private Rectangle selectionRectanglePreviewBox;
		private Point selectionTopLeft = new Point(0, 0);
		private Point selectionBottomRight = new Point(0, 0);

		//negative -> screen indices, other -> index to processList
		private int processCaptureIndex = -1;
		private int numScreens = 1;
		private bool drawingPreview = false;
		private Bitmap previewImage = null;

		private int scalingValue = 100;
		private float scalingValueFloat = 1.0f;
		#endregion Private Fields

		#region Public Constructors

		//amount of variance allowed for correct comparison
		public CrashNSaneLoadDetector()
		{
			InitializeComponent();
			captureTimer = new System.Timers.Timer();

			captureTimer.Elapsed += recordOrDetect;

			oldGameTime = DateTime.Now;
			oldRealTime = DateTime.Now;
			captureTimer.Interval = timerIntervalMilliseconds;
			gameTimer = new System.Timers.Timer();
			realTimer = new System.Timers.Timer();
			gameTimer.Interval = timerIntervalMilliseconds;
			realTimer.Interval = timerIntervalMilliseconds;
			gameTimer.Elapsed += timerValueUpdate;
			realTimer.Elapsed += timerValueUpdate;
			snapshotMilliseconds = milliSecondsBetweenSnapshots;
			listOfFeatureVectors = new List<List<int>>();
			histogramMatches = new List<bool>();
			msElapsed = new List<long>();
			lastFeatures = new List<int>();
			segmentSnapshots = new List<Bitmap>();
			segmentMatchingBins = new List<int>();
			segmentFrameCounts = new List<int>();
			segmentFeatureVectors = new List<List<int>>();

			selectionTopLeft = new Point(0, 0);
			selectionBottomRight = new Point(previewPictureBox.Width, previewPictureBox.Height);

			selectionRectanglePreviewBox = new Rectangle(selectionTopLeft.X, selectionTopLeft.Y, selectionBottomRight.X - selectionTopLeft.X, selectionBottomRight.Y - selectionTopLeft.Y);


			requiredMatchDisplayLabel.Text = numberOfBinsCorrect.ToString();

			

			StartKeyHandler = new KeyHandler(Keys.NumPad1, this);
			StartKeyHandler.Register();

			ResetKeyHandler = new KeyHandler(Keys.NumPad3, this);
			ResetKeyHandler.Register();

			Process[] processListtmp = Process.GetProcesses();
			List<Process> processes_with_name = new List<Process>();
			processListBox.Items.Clear();
			numScreens = 0;
			foreach (var screen in Screen.AllScreens)
			{
				// For each screen, add the screen properties to a list box.
				processListBox.Items.Add("Screen: " + screen.DeviceName + ", " + screen.Bounds.ToString());
				numScreens++;
			}

			foreach (Process process in processListtmp)
			{
				if (!String.IsNullOrEmpty(process.MainWindowTitle))
				{
					Console.WriteLine("Process: {0} ID: {1} Window title: {2} HWND PTR {3}", process.ProcessName, process.Id, process.MainWindowTitle, process.MainWindowHandle);
					processListBox.Items.Add(process.ProcessName + ": " + process.MainWindowTitle);
					processes_with_name.Add(process);
				}
				
			}

			processList = processes_with_name.ToArray();
			
		
			imageCaptureInfo = new ImageCaptureInfo();

			imageCaptureInfo.featureVectorResolutionX = featureVectorResolutionX;
			imageCaptureInfo.featureVectorResolutionY = featureVectorResolutionY;
			imageCaptureInfo.captureSizeX = captureSize.Width;
			imageCaptureInfo.captureSizeY = captureSize.Height;
			imageCaptureInfo.cropOffsetX = cropOffsetX;
			imageCaptureInfo.cropOffsetY = cropOffsetY;
			imageCaptureInfo.captureAspectRatio = captureAspectRatioX / captureAspectRatioY;

			processListBox.SelectedIndex = 0;
		}

		#endregion Public Constructors


		#region Private Methods
		
		/*
		[DllImport("user32.dll")]
		internal static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);
		[DllImport("user32.dll")]
		internal static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);


		private Bitmap PrintWindow(IntPtr hwnd)
		{
			Rectangle rc;
			GetWindowRect(hwnd, out rc);

			if (rc.Width < 0)
				return new Bitmap(1, 1);

			Bitmap bmp = new Bitmap(300, 100, PixelFormat.Format32bppArgb);
			Graphics gfxBmp = Graphics.FromImage(bmp);
			IntPtr hdcBitmap = gfxBmp.GetHdc();
			bool succeeded = PrintWindow(hwnd, hdcBitmap, 0);
			gfxBmp.ReleaseHdc(hdcBitmap);
			if (!succeeded)
			{
				gfxBmp.FillRectangle(new SolidBrush(Color.Gray), new Rectangle(Point.Empty, bmp.Size));
			}
			
			gfxBmp.Dispose();
			return bmp;
		}*/
		

		private void HandleHotkey(ref Message m)
		{
			// Do stuff...
			Keys key = (Keys)m.WParam.ToInt32();

			if(key == Keys.NumPad3)
			{
				resetButton_Click(this, null);
			}
			else if (key == Keys.NumPad1)
			{
				startButton_Click(this, null);
			}

		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
				HandleHotkey(ref m);
			base.WndProc(ref m);
		}
	
		private Bitmap CaptureImage()
		{
			Bitmap b = new Bitmap(1, 1);

			//Full screen capture
			if(processCaptureIndex < 0)
			{
				Screen selected_screen = Screen.AllScreens[-processCaptureIndex - 1];
				Rectangle screenRect = selected_screen.Bounds;

				screenRect.Width = (int)(screenRect.Width * scalingValueFloat);
				screenRect.Height = (int)(screenRect.Height * scalingValueFloat);

				Point screenCenter = new Point(screenRect.Width / 2, screenRect.Height / 2);


				//Change size according to selected crop
				screenRect.Width = (int)(imageCaptureInfo.crop_coordinate_right - imageCaptureInfo.crop_coordinate_left);
				screenRect.Height = (int)(imageCaptureInfo.crop_coordinate_bottom - imageCaptureInfo.crop_coordinate_top);

				//Compute crop coordinates and width/ height based on resoution
				ImageCapture.SizeAdjustedCropAndOffset(screenRect.Width, screenRect.Height, ref imageCaptureInfo);

				//Adjust for crop offset
				imageCaptureInfo.center_of_frame_x += imageCaptureInfo.crop_coordinate_left;
				imageCaptureInfo.center_of_frame_y += imageCaptureInfo.crop_coordinate_top;

				//Adjust for selected screen offset
				imageCaptureInfo.center_of_frame_x += selected_screen.Bounds.X;
				imageCaptureInfo.center_of_frame_y += selected_screen.Bounds.Y;

				
				

				b = ImageCapture.CaptureFromDisplay(ref imageCaptureInfo);
			}
			else
			{
				IntPtr handle = new IntPtr(0);

				if (processCaptureIndex >= processList.Length)
					return b;

				if(processCaptureIndex != -1)
				{
					handle = processList[processCaptureIndex].MainWindowHandle;
				}
				//Capture from specific process
				processList[processCaptureIndex].Refresh();
				if ((int)handle == 0)
					return b;

				b = ImageCapture.PrintWindow(handle, ref imageCaptureInfo, useCrop: true);
			}

			
			return b;
		}

		private Bitmap CaptureImageFullPreview(ref ImageCaptureInfo imageCaptureInfo, bool useCrop = false)
		{
			Bitmap b = new Bitmap(1, 1);

			//Full screen capture
			if (processCaptureIndex < 0)
			{
				Screen selected_screen = Screen.AllScreens[-processCaptureIndex - 1];
				Rectangle screenRect = selected_screen.Bounds;

				screenRect.Width = (int)(screenRect.Width * scalingValueFloat);
				screenRect.Height = (int)(screenRect.Height * scalingValueFloat);


				Point screenCenter = new Point((int)(screenRect.Width / 2.0f), (int)(screenRect.Height/ 2.0f));

				if(useCrop)
				{
					//Change size according to selected crop
					screenRect.Width = (int)(imageCaptureInfo.crop_coordinate_right - imageCaptureInfo.crop_coordinate_left);
					screenRect.Height = (int)(imageCaptureInfo.crop_coordinate_bottom - imageCaptureInfo.crop_coordinate_top);
				}
				

				//Compute crop coordinates and width/ height based on resoution
				ImageCapture.SizeAdjustedCropAndOffset(screenRect.Width, screenRect.Height, ref imageCaptureInfo);


				imageCaptureInfo.actual_crop_size_x = 2 * imageCaptureInfo.center_of_frame_x;
				imageCaptureInfo.actual_crop_size_y = 2 * imageCaptureInfo.center_of_frame_y;

				if(useCrop)
				{
					//Adjust for crop offset
					imageCaptureInfo.center_of_frame_x += imageCaptureInfo.crop_coordinate_left;
					imageCaptureInfo.center_of_frame_y += imageCaptureInfo.crop_coordinate_top;
				}

				//Adjust for selected screen offset
				imageCaptureInfo.center_of_frame_x += selected_screen.Bounds.X;
				imageCaptureInfo.center_of_frame_y += selected_screen.Bounds.Y;

				imageCaptureInfo.actual_offset_x = 0;
				imageCaptureInfo.actual_offset_y = 0;

				b = ImageCapture.CaptureFromDisplay(ref imageCaptureInfo);

				imageCaptureInfo.actual_offset_x = cropOffsetX;
				imageCaptureInfo.actual_offset_y = cropOffsetY;



			}
			else
			{
				IntPtr handle = new IntPtr(0);

				if (processCaptureIndex >= processList.Length)
					return b;

				if (processCaptureIndex != -1)
				{
					handle = processList[processCaptureIndex].MainWindowHandle;
				}
				//Capture from specific process
				processList[processCaptureIndex].Refresh();
				if ((int)handle == 0)
					return b;

			

				b = ImageCapture.PrintWindow(handle, ref imageCaptureInfo, full:true, useCrop:useCrop, scalingValueFloat:scalingValueFloat);
			}


			return b;
		}

		private void captureAndUpdate(object source, ElapsedEventArgs e)
		{
			List<int> features = featuresAtScreenCenter();
		}

	

		private void countGameTime(object source, ElapsedEventArgs e)
		{
			if (currentlyPaused == true && wasPaused == false)
			{
				//starting loading
				loadStart = DateTime.Now;
				loadTimeTemp = loadTime;
			}
			else if (currentlyPaused == false && wasPaused == true)
			{
				//loading done
				loadTime = loadTimeTemp + (DateTime.Now - loadStart);
				wasPaused = currentlyPaused;
				try
				{
					
					Invoke(new Action(() =>
					{
						pauseSegmentList.Items.Add(DateTime.Now - loadStart);

						if (saveDiagnosticImages)
						{
							//Set this early, otherwise we might enter multiple times if there are a lot of images to save

							int currentSnapshotFrameCount = snapshotFrameCount;
							//If diagnostics are enabled: we save snapshots from this segment to a folder.
							System.IO.Directory.CreateDirectory(DiagnosticsFolderName + "pauseSegmentSnapshots/" + currentSnapshotFrameCount.ToString());

							for (int idx = 0; idx < segmentSnapshots.Count; idx++)
							{
								Bitmap bmp = segmentSnapshots[idx];
								bmp.Save(DiagnosticsFolderName + "pauseSegmentSnapshots/" + currentSnapshotFrameCount.ToString() + "/" + idx + "_" + segmentMatchingBins[idx] + ".jpg", ImageFormat.Jpeg);
								saveFeatureVectorToTxt(segmentFeatureVectors[idx], "features_" + segmentFrameCounts[idx] + "_" + segmentMatchingBins[idx] + ".txt", DiagnosticsFolderName + "pauseSegmentSnapshots/" + currentSnapshotFrameCount.ToString());
							}
						}
						//Clear segment data

						foreach (Bitmap bmp in segmentSnapshots)
						{
							bmp.Dispose();
						}
						segmentSnapshots.Clear();
						segmentMatchingBins.Clear();
						segmentFeatureVectors.Clear();
						segmentFrameCounts.Clear();
					}));
				}
				catch
				{
					//if the form is closing, we just do nothing.
					Console.WriteLine("CLOSING CATCH");
				}
			}
			else if (currentlyPaused == true && wasPaused == true)
			{
				//intermediate loading time values for display
				loadTime = DateTime.Now - loadStart + loadTimeTemp;
			}

			gameTime = realTime - loadTime;
			try
			{
				Invoke(new Action(() =>
				{
					gameTimeLabel.Text = gameTime.ToString();
					pausedTimeLabel.Text = loadTime.ToString();
				}));
			}
			catch
			{
				//if the form is closing, we just do nothing.
			}
			wasPaused = currentlyPaused;
		}

		private void timerValueUpdate(object source, ElapsedEventArgs e)
		{
			countRealTime(source, e);
			countGameTime(source, e);
		}

		private void countRealTime(object source, ElapsedEventArgs e)
		{
			realTime = DateTime.Now - timerBegin;
			try
			{
				Invoke(new Action(() =>
				{
					realTimeLabel.Text = realTime.ToString();
				}));
			}
			catch
			{
				//if the form is closing, we just do nothing.
			}
			oldRealTime = DateTime.Now;
		}


		
		private List<int> featuresAtScreenCenter()
		{
			try
			{
				Stopwatch stopwatch = Stopwatch.StartNew();
				Bitmap bmp = null;



				bmp = CaptureImage();

				List<int> features = FeatureDetector.featuresFromBitmap(bmp);
				lastFeatures = features;
				bool matchingHistograms = FeatureDetector.compareFeatureVector(features.ToArray(), out matchingBins, false);

				if (snapshotMilliseconds <= 0)
				{
					snapshotMilliseconds = milliSecondsBetweenSnapshots;

					snapshotFrameCount = frameCount;

					segmentSnapshots.Add(bmp);
					segmentMatchingBins.Add(matchingBins);
					segmentFeatureVectors.Add(features);
					segmentFrameCounts.Add(frameCount);
				}

				currentlyPaused = matchingHistograms;
				stopwatch.Stop();

				msElapsed.Add(stopwatch.ElapsedMilliseconds);
				if (msElapsed.Count > 20)
				{
					long sum = 0;

					foreach (var ms in msElapsed)
					{
						sum += ms;
					}
					sum /= msElapsed.Count;
					msElapsed.Clear();
					//Console.WriteLine("DetectMatch (avg): " + sum + "ms");
				}

				if (currentlyPaused)
				{
					//only save if we haven't saved for at least 10 frames, just for diagnostics to see if any false positives are in there.
					//or if we haven't seen a paused frame for at least 30 frames.
					if ((frameCount > (lastSaveFrame + 10) || (frameCount - lastPausedFrame) > 30) && saveDiagnosticImages)
					{
						System.IO.Directory.CreateDirectory(DiagnosticsFolderName + "imgs_stopped");

						try
						{
							bmp.Save(DiagnosticsFolderName + "imgs_stopped/img_" + frameCount + "_" + matchingBins + ".jpg", ImageFormat.Jpeg);
						}
						catch
						{
						}

						saveFeatureVectorToTxt(features, "features_" + frameCount + "_" + matchingBins + ".txt", DiagnosticsFolderName + "features_stopped");

						lastSaveFrame = frameCount;
					}

					lastPausedFrame = frameCount;
				}
				else
				{
					//save if we haven't seen a running frame for at least 30 frames (to detect false runs - e.g. aku covering "loading"
					if ((frameCount - lastRunningFrame) > 10 && saveDiagnosticImages)
					{
						System.IO.Directory.CreateDirectory(DiagnosticsFolderName + "imgs_running");
						try
						{
							bmp.Save(DiagnosticsFolderName + "imgs_running/img_" + frameCount + "_" + matchingBins + ".jpg", ImageFormat.Jpeg);
						}
						catch
						{
						}

						saveFeatureVectorToTxt(features, "features_" + frameCount + "_" + matchingBins + ".txt", DiagnosticsFolderName + "features_running");

						lastSaveFrame = frameCount;
					}
					lastRunningFrame = frameCount;
				}

				frameCount++;
				histogramMatches.Add(matchingHistograms);
				try
				{
					Invoke(new Action(() =>
					{
						try
						{


							imageDisplay.Image = bmp;
							imageDisplay.Size = new Size(captureSize.Width, captureSize.Height);
							imageDisplay.BackgroundImage = bmp;
							imageDisplay.Refresh();
							matchDisplayLabel.Text = matchingBins.ToString();
							requiredMatchDisplayLabel.Text = numberOfBinsCorrect.ToString();
							if (matchingHistograms)
							{
							//paused
							pausedDisplay.BackColor = Color.Red;
							}
							else
							{
							//running
							pausedDisplay.BackColor = Color.Green;
							}
						}
						catch
						{

						}
					}));
				}
				catch
				{
					//if the form is closing, we just do nothing.
				}

				return features;
			}
			catch (Exception ex)
			{
				Console.WriteLine("TESTESTESTEST");
				return new List<int>();
			}
			//bmp.Dispose();
			
		}
		

		private void recordCheck_CheckedChanged(object sender, EventArgs e)
		{
			recordMode = recordCheck.Checked;
		}

		private void recordFeature(object source, ElapsedEventArgs e)
		{
			listOfFeatureVectors.Add(featuresAtScreenCenter());
		}

		private void recordOrDetect(object source, ElapsedEventArgs e)
		{
			snapshotMilliseconds -= timerIntervalMilliseconds;
			if (recordMode)
			{
				recordFeature(source, e);
			}
			else
			{
				captureAndUpdate(source, e);
			}
		}

		private void saveDiagnosticCheck_CheckedChanged(object sender, EventArgs e)
		{
			saveDiagnosticImages = saveDiagnosticCheck.Checked;
		}


		private void resetButton_Click(object sender, EventArgs e)
		{
			resetState();
			captureTimer.Enabled = false;
			gameTimer.Enabled = false;
			realTimer.Enabled = false;
		}

		private void resetState()
		{
			oldGameTime = DateTime.Now;
			oldRealTime = DateTime.Now;
			timerBegin = DateTime.Now;
			loadStart = DateTime.Now;
			realTime = new TimeSpan(0);
			gameTime = new TimeSpan(0);
			loadTime = new TimeSpan(0);
			loadTimeTemp = new TimeSpan(0);

			currentlyPaused = false;
			frameCount = 0;
			lastRunningFrame = 0;
			lastPausedFrame = 0;
			lastSaveFrame = 0;

			snapshotMilliseconds = milliSecondsBetweenSnapshots;
			msElapsed.Clear();

			foreach (Bitmap bmp in segmentSnapshots)
			{
				bmp.Dispose();
			}
			segmentSnapshots.Clear();
			segmentMatchingBins.Clear();
			segmentFeatureVectors.Clear();
			segmentFrameCounts.Clear();
			pauseSegmentList.Items.Clear();
			
			gameTimer.Enabled = false;
			realTimer.Enabled = false;
		}

		private void startButton_Click(object sender, EventArgs e)
		{

			if(captureTimer.Enabled == true)
			{
				return;
			}


			captureTimer.Enabled = true;

			if (recordMode)
			{
				if (captureTimer.Enabled == false)
				{
					System.IO.Directory.CreateDirectory(DiagnosticsFolderName);
					using (var file = File.CreateText(DiagnosticsFolderName + "loading_eng.csv"))
					{
						foreach (var list in listOfFeatureVectors)
						{
							file.WriteLine(string.Join(",", list));
						}
					}

					listOfFeatureVectors.Clear();
				}
			}
			else
			{
				resetState();
				gameTimer.Enabled = true;
				realTimer.Enabled = true;
			}
		}

		#endregion Private Methods

		private int currentRecordCount = 0;

		private void CrashNSaneLoadDetector_FormClosing(object sender, FormClosingEventArgs e)
		{
			gameTimer.Enabled = false;
			realTimer.Enabled = false;
			captureTimer.Enabled = false;
		}

		private void recordCurrentButton_Click(object sender, EventArgs e)
		{
			currentRecordCount++;
			listOfFeatureVectors.Add(lastFeatures);
			System.IO.Directory.CreateDirectory(DiagnosticsFolderName);
			using (var file = File.CreateText(DiagnosticsFolderName + "loading_eng_features.txt"))
			{
				file.Write("private int[,] listOfFeatureVectorsEng = {\n");
				foreach (var list in listOfFeatureVectors)
				{
					file.Write("{");
					file.Write(string.Join(",", list));
					file.Write("},\n");
				}

				file.Write("};\n");
			}
		}

		private void saveFeatureVectorToTxt(List<int> featureVector, string filename, string directoryName)
		{
			System.IO.Directory.CreateDirectory(directoryName);
			try
			{
				using (var file = File.CreateText(directoryName + "/" + filename))
				{
					file.Write("{");
					file.Write(string.Join(",", featureVector));
					file.Write("},\n");
				}
			}
			catch
			{
				//yeah, silent catch is bad, I don't care
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			OpenFileDialog ofd = new OpenFileDialog();

			DialogResult result = ofd.ShowDialog();

			if(result != DialogResult.OK)
			{
				return;
			}

			Bitmap bmp = new Bitmap(ofd.FileName);

			//Make 32 bit ARGB bitmap
			Bitmap clone = new Bitmap(bmp.Width, bmp.Height,
				System.Drawing.Imaging.PixelFormat.Format32bppArgb);

			using (Graphics gr = Graphics.FromImage(clone))
			{
				gr.DrawImage(bmp, new Rectangle(0, 0, clone.Width, clone.Height));
			}

			List<int> features = FeatureDetector.featuresFromBitmap(clone);

			bool compare_result = FeatureDetector.compareFeatureVector(features.ToArray(), out matchingBins);

			Console.WriteLine("RESULT: " + matchingBins + " matching bins, compare == " + compare_result);



			currentRecordCount++;
			listOfFeatureVectors.Add(features);

			System.IO.Directory.CreateDirectory(DiagnosticsFolderName);
			using (var file = File.CreateText(DiagnosticsFolderName + "loading_eng_features.txt"))
			{
				file.Write("private int[,] listOfFeatureVectorsEng = {\n");
				foreach (var list in listOfFeatureVectors)
				{
					file.Write("{");
					file.Write(string.Join(",", list));
					file.Write("},\n");
				}

				file.Write("};\n");
			}

			bmp.Dispose();
			clone.Dispose();
		}

		private void processListBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (processListBox.SelectedIndex < numScreens)
			{
				processCaptureIndex = -processListBox.SelectedIndex - 1;
			}
			else
			{
				processCaptureIndex = processListBox.SelectedIndex - numScreens;
			}
			selectionTopLeft = new Point(0, 0);
			selectionBottomRight = new Point(previewPictureBox.Width, previewPictureBox.Height);
			selectionRectanglePreviewBox = new Rectangle(selectionTopLeft.X, selectionTopLeft.Y, selectionBottomRight.X - selectionTopLeft.X, selectionBottomRight.Y - selectionTopLeft.Y);

			DrawPreview();

		}

		private void DrawCaptureRectangleBitmap()
		{
			Bitmap capture_image = (Bitmap)previewImage.Clone();
			//Draw selection rectangle
			using (Graphics g = Graphics.FromImage(capture_image))
			{
				Pen drawing_pen = new Pen(Color.Magenta, 8.0f);
				drawing_pen.Alignment = PenAlignment.Inset;
				g.DrawRectangle(drawing_pen, selectionRectanglePreviewBox);

			}

			previewPictureBox.Image = capture_image;
		}

		private void DrawPreview()
		{
			

			ImageCaptureInfo copy = imageCaptureInfo;
			copy.captureSizeX = previewPictureBox.Width;
			copy.captureSizeY = previewPictureBox.Height;

			//Show something in the preview
			previewImage = CaptureImageFullPreview(ref copy);
			float crop_size_x = copy.actual_crop_size_x;
			float crop_size_y = copy.actual_crop_size_y;
			

			//Draw selection rectangle
			DrawCaptureRectangleBitmap();




			//Compute image crop coordinates according to selection rectangle

			//Get raw image size from imageCaptureInfo.actual_crop_size to compute scaling between raw and rectangle coordinates

			//Console.WriteLine("SIZE X: {0}, SIZE Y: {1}", imageCaptureInfo.actual_crop_size_x, imageCaptureInfo.actual_crop_size_y);

			imageCaptureInfo.crop_coordinate_left = selectionRectanglePreviewBox.Left  * (crop_size_x / previewPictureBox.Width);
			imageCaptureInfo.crop_coordinate_right = selectionRectanglePreviewBox.Right  * (crop_size_x / previewPictureBox.Width);
			imageCaptureInfo.crop_coordinate_top = selectionRectanglePreviewBox.Top  * (crop_size_y / previewPictureBox.Height);
			imageCaptureInfo.crop_coordinate_bottom = selectionRectanglePreviewBox.Bottom  * (crop_size_y / previewPictureBox.Height);

			copy.crop_coordinate_left = selectionRectanglePreviewBox.Left  * (crop_size_x / previewPictureBox.Width);
			copy.crop_coordinate_right = selectionRectanglePreviewBox.Right  * (crop_size_x / previewPictureBox.Width);
			copy.crop_coordinate_top = selectionRectanglePreviewBox.Top  * (crop_size_y / previewPictureBox.Height);
			copy.crop_coordinate_bottom = selectionRectanglePreviewBox.Bottom * (crop_size_y / previewPictureBox.Height);



			croppedPreviewPictureBox.Image = CaptureImageFullPreview(ref copy, useCrop:true);


			copy.captureSizeX = captureSize.Width;
			copy.captureSizeY = captureSize.Height;
			

		}

		private void SetRectangleFromMouse(MouseEventArgs e)
		{
			//Clamp values to pictureBox range
			int x = Math.Min(Math.Max(0, e.Location.X), previewPictureBox.Width);
			int y = Math.Min(Math.Max(0, e.Location.Y), previewPictureBox.Height);

			if (e.Button == MouseButtons.Left
				&& (selectionRectanglePreviewBox.Left + selectionRectanglePreviewBox.Width) - x > 0
				&& (selectionRectanglePreviewBox.Top + selectionRectanglePreviewBox.Height) - y > 0)
			{
				selectionTopLeft = new Point(x, y);
			}
			else if (e.Button == MouseButtons.Right && x - selectionRectanglePreviewBox.Left > 0 && y - selectionRectanglePreviewBox.Top > 0)
			{
				selectionBottomRight = new Point(x, y);
			}

			selectionRectanglePreviewBox = new Rectangle(selectionTopLeft.X, selectionTopLeft.Y, selectionBottomRight.X - selectionTopLeft.X, selectionBottomRight.Y - selectionTopLeft.Y);


		}

		private void previewPictureBox_MouseClick(object sender, MouseEventArgs e)
		{
			
		}

		private void previewPictureBox_MouseMove(object sender, MouseEventArgs e)
		{
			SetRectangleFromMouse(e);
			if (drawingPreview == false)
			{
				drawingPreview = true;
				//Draw selection rectangle
				DrawCaptureRectangleBitmap();
				drawingPreview = false;
			}
			
		}

		private void previewPictureBox_MouseUp(object sender, MouseEventArgs e)
		{
			SetRectangleFromMouse(e);
			DrawPreview();
		}

		private void previewPictureBox_MouseDown(object sender, MouseEventArgs e)
		{
			SetRectangleFromMouse(e);
			DrawPreview();
		}

		private void trackBar1_ValueChanged(object sender, EventArgs e)
		{
			scalingValue = trackBar1.Value;

			if (scalingValue % trackBar1.SmallChange != 0)
			{
				scalingValue = (scalingValue / trackBar1.SmallChange) * trackBar1.SmallChange;

				trackBar1.Value = scalingValue;
				
			}

			scalingValueFloat = ((float)scalingValue) / 100.0f;

			scalingLabel.Text = "Scaling: " + trackBar1.Value.ToString() + "%";

			DrawPreview();
		}
	}
}
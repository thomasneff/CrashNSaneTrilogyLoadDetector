state("LiveSplit") //this is hacky, but .asl scripts need a "state" otherwise they won't update.
{

}
startup 
{
//print("STARTUP");
  // Add setting 'Split every N Saves', enabled by default
  //settings.Add("split_n", true, "Split every N Saves");
  
  // Add setting 'Split on Save String', enabled by default
  //settings.Add("split_string", true, "Split on Save String");
  // Add setting 'Split on abandoned cores collected', enabled by default
  //settings.Add("split_cores", true, "Split on abandoned cores collected");
  
  // Add setting 'Split on checkpoint coordinates', enabled by default
  //settings.Add("split_CP", true, "Split on checkpoint coordinates");
  //vars.splitVar = 0;
  //vars.feature_vector_loading_english = new int[] { 3542, 143, 103, 92, 92, 101, 296, 2030, 3537, 142, 102, 92, 90, 100, 304, 2031, 3534, 141, 101, 92, 89, 101, 304, 2039, 3530, 140, 100, 92, 87, 101, 303, 2048, 3533, 138, 106, 92, 88, 105, 275, 2064, 3524, 140, 108, 94, 88, 104, 271, 2071, 3515, 142, 111, 96, 89, 104, 270, 2072, 3505, 145, 113, 98, 90, 105, 270, 2074, 2897, 128, 85, 78, 77, 81, 335, 2719, 2891, 126, 82, 77, 76, 81, 345, 2723, 2885, 124, 79, 74, 75, 82, 345, 2737, 2879, 121, 77, 72, 73, 82, 345, 2751, 3020, 122, 113, 58, 74, 95, 315, 2602, 3030, 122, 114, 58, 74, 95, 308, 2598, 3040, 122, 115, 57, 76, 94, 309, 2586, 3050, 122, 116, 57, 78, 92, 310, 2575, 3505, 150, 126, 75, 108, 83, 266, 2086, 3507, 151, 126, 75, 108, 83, 268, 2081, 3509, 151, 125, 77, 107, 85, 267, 2079, 3511, 152, 124, 78, 106, 86, 266, 2076, 3403, 139, 92, 96, 79, 111, 319, 2161, 3406, 137, 91, 95, 79, 110, 320, 2161, 3409, 137, 91, 94, 79, 109, 321, 2160, 3412, 136, 91, 94, 78, 109, 321, 2159 };
  //vars.percent_of_bins_correct = 0.4f; //percentage of histogram bins which have to match for loading detection
	//vars.variance_of_bins_allowed = 0.3f; //amount of variance allowed for correct comparison
  //vars.currently_loading = false;
  print("start");
}
reset
{
  print("reset");
} 

//isLoading
//{
//  return vars.currently_loading;
//}


update
{
  //How it works: 
  //we capture the screen region around "LOADING"
  //cut the screenshot region into patches
  //compute a histogram of color values with a fixed amount of bins inside each patch
  //serialize this into a vector
  //and use this as a feature vector for comparison.
  
  //to check if we are in "LOADING" or not, we simply compare every new feature vector to a precomputed average of the load screen, with some tolerance
  //to make it more robust (e.g. against Aku Aku flying inside the "LOADING" text)
  //TODO: this probably only works @ 1080p atm, would need to increase screen region and patch size accordingly for e.g. 4k
  //TODO: this currently requires either using Remote Play maximized on the primary screen or using your capture card preview maximized on the primary screen.
  //TODO: also other languages might not work, I dunno
  print("update"); 
  System.Drawing.Bitmap b = new System.Drawing.Bitmap(100, 100);
  print("update2");
	using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(b))
	{
		g.CopyFromScreen(100 / 2, 100 / 2, 0, 0, new System.Drawing.Size(100, 100), System.Drawing.CopyPixelOperation.SourceCopy);
    print("update3");
	}
  
	

}

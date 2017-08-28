# CrashNSaneTrilogyLoadDetector
Loading Screen Detector (Standalone testing tool and .asl script for LiveSplit) for the Crash N. Sane Trilogy (Crash NST).

# Special Thanks
Special thanks go to McCrodi from the Crash Speedrunning Discord, who helped me by providing 1080p/720p captured data and general feedback regarding the functionality.

# Disclaimer (READ THIS IF YOU TRY IT OUT)
I'm still tinkering with the detection to make it better. Currently, it sometimes unpauses for 1 frame when it shouldn't, which can be solved by me collecting more data. So the Pause Time should be fairly accurate most of the time, while the "Pause Segments" could be screwed up. Still work in progress.

# How does it work?
The method works by taking a small "screenshot" (currently 300x100) from your primary display at the center, where "LOADING" is displayed when playing the Crash NST. It then cuts this 300x100 image into patches (currently of size 50x50). From these patches, a color histogram is computed (currently using 16 histogram bins -> [0-15, 16-31, 32-47, ..., 240-255]) of the red, green and blue color channels. These histograms are put into a large vector, which describes our image (feature vector).

To detect if a screen is "LOADING" or not, we compute this feature vector every ~16ms (fast enough for real-time load detection) and compare it to a precomputed list of feature vectors. This list has currently been precomputed for the english version of the NST using different VODs and Remote Play footage. The precomputed vectors are simply snapshots during the "LOADING" screen (also during animation, when Aku Aku flies over "LOADING", different quality settings...).
We detect a "LOADING" screen if our current feature vector has similar enough histogram bins to any of the precomputed vectors. Comparing against multiple vectors allows for more robust detection in settings where "LOADING" is partially occluded or different video quality settings.

I decided to go for this simplistic approach (rather than e.g. computing SIFT features, histogram of gradients, deep learning detection...) as it doesn't have any external dependencies (which e.g. deep learning would have) and allows for real-time detection.

# Requirements

Currently, english version of Crash NST. I have not tested the other versions, but I can probably make them work if I collect enough features from the other versions.

Currently, the tool requires that you have the footage fullscreened on your primary display. That can either be a video, PS4 remote play, or your capture card preview. This is required as I'm taking screenshots to detect "LOADING".

Currently, it also probably requires your primary monitor to have a resolution of 1080p. This is because the screenshot has a fixed size, and increasing the size to make up for increased resolution (e.g. 4K) would slow down the detection by a lot. Different resolutions also require different feature vectors to be precomputed.

If you do full trilogy run, you'll still need to time your title screen loads. Sorry, that's just because the title screen loads are different than the ingame loads, and I can't keep it real-time while still accounting for those differences.

# UI Options
## Start
Start simply starts the timers of the C# tool. You'll see the currently captured image region in real time, as well as the current histogram matches and required matches to detect a pause.

## Record Mode
If you want to record multiple feature vectors in succession, use record mode. If you then Start/Stop the timer, the features are put into a .csv file in the same folder as the binary.

## Record Current
This can be used to record feature snapshots during timing. It stores all the features into a .txt file in the same folder as the binary.

## Save Diagnostics
If enabled, images where the load screen is detected / not detected are saved periodically into folders "imgs_running" and "imgs_stopped". Images in "imgs_stopped" should *exclusively* contain images of "LOADING", while images in "imgs_running" should *never* contain images of "LOADING".
For each of these images, also the corresponding feature vectors are saved in "features_running" and "features_stopped", so please, if you report a wrong detection, attach both the relevant diagnostics image and feature_x_y.txt file. Both files have the frame count and the number of matching histogram bins in their filename to uniquely identify them.


# TODO:
Currently I'm working on improving the robustness of the tool, before I start working on the .asl script. I want to check if the tool works with videos of multiple runs, my own runs through remote play, all in varying quality. I want to remove, if possible, all wrongly detected load screens.

As far as I can tell, the current version works ~95% of the time with the footage I tested. There are multiple instances in the NST where colors are very similar to "LOADING", which makes it difficult to be correct in all cases.

If the tool works 100% consistently, I could also add auto-split functionality to the .asl script, by just splitting on the end of "LOADING", but not sure how necessary that is.

# How to Track Issues:
Please use the github issue tracking in this repository. If you find a wrong detection, please post at least the following information in the issue:

Where did it happen? (Area in the Game)
What were you doing in game?
Diagnostic image and corresponding feature_x_y.txt showing the wrong detection. ("LOADING" in "imgs_running" or non-"LOADING" in "imgs_stopped")

Additional, very very helpful stuff would be:
VOD/video with timestamp to reproduce the issue.
(optional, as I can capture that myself as well) .txt file captured in the exact frame where the wrong detection happens using "Record Current".

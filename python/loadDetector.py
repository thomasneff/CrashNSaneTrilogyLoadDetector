import numpy as np
import cv2
import matplotlib.pyplot as plt
from multiprocessing import Process, Queue
from queue import Empty

import time
import csv
import datetime
import sys

# Boilerplate opencv video processing code from
# https://stackoverflow.com/questions/42284122/opencv-python-multi-threading-for-live-facial-recognition

# from common import clock, draw_str, StatValue
# import video

from featureDetection import isLoading
from featureDetection import extractFeatures
from featureDetection import DEBUG_OUTPUT

featureVectorResolutionX = 1920.0
featureVectorResolutionY = 1080.0

featureVectorCropSizeX = 300.0
featureVectorCropSizeY = 100.0

cropOffsetX = 0.0
cropOffsetY = -40.0

frameCounter = 0
fps = 0.0

class FrameProcess(Process):
    def __init__(self, frame_queue, output_queue):
        Process.__init__(self)
        self.frame_queue = frame_queue
        self.output_queue = output_queue
        self.stop = False
        #this counts the number of non-paused frames.
        self.frameCounterRunning = 0
        self.frameCounterPaused = 0
        self.frameCounterTotal = 0


    def get_frame(self):
        if not self.frame_queue.empty():
            return True, self.frame_queue.get()
        else:
            return False, None

    def stopProcess(self):
        self.stop = True

    def getFrameCounts(self):
        return self.frameCounterRunning, self.frameCounterPaused, self.frameCounterTotal

    def processFrame(self, frame):
        # Crop center of the image for feature extraction, crop 300 x 100 @ 1080p, scale it using frame resolution
        height, width, channels = frame.shape

        resolution_factor_x = (width / featureVectorResolutionX)
        resolution_factor_y = (height / featureVectorResolutionY)

        actual_crop_size_x = featureVectorCropSizeX * resolution_factor_x
        actual_crop_size_y = featureVectorCropSizeY * resolution_factor_y

        # Scale offset depending on resolution
        actual_offset_x = cropOffsetX * resolution_factor_x
        actual_offset_y = cropOffsetY * resolution_factor_y

        center_of_frame_x = width / 2
        center_of_frame_y = height / 2

        left_x = np.rint(center_of_frame_x - actual_crop_size_x / 2 + actual_offset_x).astype(int)
        right_x = np.rint(center_of_frame_x + actual_crop_size_x / 2 + actual_offset_x).astype(int)

        top_y = np.rint(center_of_frame_y - actual_crop_size_y / 2 + actual_offset_y).astype(int)
        bot_y = np.rint(center_of_frame_y + actual_crop_size_y / 2 + actual_offset_y).astype(int)

        # Crop "LOADING" area out of the frame
        area_of_interest = frame[top_y:bot_y, left_x:right_x]

        # Resize to 300x100, as the features_second have been computed on that.
        area_of_interest_300x100 = cv2.resize(area_of_interest, (300, 100))


        # TODO: update framecounters using the detection method
        self.frameCounterTotal = self.frameCounterTotal + 1
        loading, matching_bins = isLoading(area_of_interest_300x100)
        if loading:
            self.frameCounterPaused = self.frameCounterPaused + 1
        else:
            self.frameCounterRunning = self.frameCounterRunning + 1


        #print("Frame: {}".format(self.frameCounterTotal))

        if self.output_queue.full():
            try:
                self.output_queue.get()
            except Empty:
                # Handle empty queue here
                pass

        # Assemble object to return from the multiprocessing function
        queue_obj = {}
        queue_obj['frameCounterTotal'] = self.frameCounterTotal
        queue_obj['frameCounterRunning'] = self.frameCounterRunning
        queue_obj['frameCounterPaused'] = self.frameCounterPaused
        queue_obj['frame'] = area_of_interest_300x100
        queue_obj['matching_bins'] = matching_bins
        queue_obj['is_loading'] = loading

        self.output_queue.put(queue_obj)

    def run(self):
        while not self.stop:
            ret, frame = self.get_frame()
            if ret:
                self.processFrame(frame)



def main():
    frame_sum = 0
    init_time = time.time()

    def put_frame(frame):
        while Input_Queue.full():
            pass
            #try:
            #    Input_Queue.get()
            #except Empty:
                # Handle empty queue here
            #    pass

        Input_Queue.put(frame)

    def cap_read(cv2_cap):
        ret, frame = cv2_cap.read()
        if ret:
            put_frame(frame)

        return ret

    cap = cv2.VideoCapture(sys.argv[1])

    fps = cap.get(cv2.CAP_PROP_FPS)

    print("Video running at {} fps".format(fps))

    # Multiprocessing has shown to not improve the speed yet - it's disabled for now.
    # However, it should still work fine - just assemble all frameCounts at the end and calculate final times.

    #threadn = cv2.getNumberOfCPUs()
    threadn = 1

    threaded_mode = True

    process_list = []
    Input_Queue = Queue(maxsize=5)
    Output_Queue = Queue(maxsize=5)

    for x in range((threadn)):
        canny_process = FrameProcess(frame_queue=Input_Queue, output_queue=Output_Queue)
        canny_process.daemon = True
        canny_process.start()
        process_list.append(canny_process)

    ch = cv2.waitKey(1)
    # cv2.namedWindow('Threaded Video', cv2.WINDOW_NORMAL)
    cv2.namedWindow('Threaded Video', cv2.WINDOW_AUTOSIZE)
    cv2.resizeWindow('image', 300, 100)

    # Just some diagnostics
    matchingBinsHistogram = np.zeros(577)
    is_loading = None
    while True:
        ret = cap_read(cap)



        if not Output_Queue.empty():
            result = Output_Queue.get()
            features = extractFeatures(result['frame'])
            matchingBinsHistogram[result['matching_bins']] += 1

            if result['matching_bins'] >= 480 and result['matching_bins'] <= 520:
                with open('features/' + str(result['frameCounterTotal']) + '.txt', 'w') as myfile:
                    wr = csv.writer(myfile, quoting=csv.QUOTE_ALL)
                    wr.writerow(features)
                    cv2.imwrite('features/' + str(result['frameCounterTotal']) + '.png', result['frame'])

            if result['is_loading'] != is_loading:
                is_loading = result['is_loading']

                if is_loading:
                    print("LOADING at frame {}!".format(result['frameCounterTotal']))
                else:
                    print("Not LOADING at frame {}!".format(result['frameCounterTotal']))

            if DEBUG_OUTPUT == True:
                print("Total: {}, Running: {}, Paused: {}".format(result['frameCounterTotal'], result['frameCounterRunning'], result['frameCounterPaused']))

            cv2.imshow('Threaded Video', result['frame'])
            ch = cv2.waitKey(5)

        if ch == ord(' '):
            threaded_mode = not threaded_mode
        if ch == 32:
            # Space is used to collect feature vectors.
            with open('features/' + str(result['frameCounterTotal']) + '.txt', 'w') as myfile:
                wr = csv.writer(myfile, quoting=csv.QUOTE_ALL)
                wr.writerow(features)
                cv2.imwrite('features/' + str(result['frameCounterTotal']) + '.png', result['frame'])

        if ch == 27 or ret == False:
            break
    cv2.destroyAllWindows()


    # Save matching_bins histogram (for diagnosis)
    np.savetxt('matching_bins_1.txt', matchingBinsHistogram, fmt='%d', delimiter=',')
    fig, ax = plt.subplots()
    histogram_width = 1
    #hist = [sum(matchingBinsHistogram[current: current+histogram_width]) for current in range(0, len(matchingBinsHistogram), histogram_width)]
    hist = matchingBinsHistogram
    bins = np.linspace(0, len(hist) + 1, len(hist) + 1)
    width = 2 * (bins[1] - bins[0])
    center = (bins[:-1] + bins[1:]) / 2
    plt.yscale('log')

    plt.bar(center, hist, align='center', width=width)
    ax.bar(center, hist, align='center', width=width)
    fig.set_size_inches(18.5, 10.5)
    fig.savefig("matching_bins_1.png")

    print("")
    print("--------- SUMMARY ---------")
    print("Frames: Total: {}, Running: {}, Paused: {}".format(result['frameCounterTotal'], result['frameCounterRunning'],
                                                      result['frameCounterPaused']))

    # Compute times from frames and fps
    seconds_total = result['frameCounterTotal'] / fps
    seconds_running = result['frameCounterRunning'] / fps
    seconds_paused = result['frameCounterPaused'] / fps

    print("Time (video @ {} fps): Total: {}, Running: {}, Paused: {}".format(fps,
                                                                             datetime.timedelta(seconds=seconds_total),
                                                                             datetime.timedelta(seconds=seconds_running),
                                                                             datetime.timedelta(seconds=seconds_paused)
                                                                             ))

    print("---------------------------")

if __name__ == '__main__':
    main()

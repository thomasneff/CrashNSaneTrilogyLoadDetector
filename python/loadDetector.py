import numpy as np
import cv2

from multiprocessing import Process, Queue
from queue import Empty

import time

# Boilerplate opencv video processing code from
# https://stackoverflow.com/questions/42284122/opencv-python-multi-threading-for-live-facial-recognition

# from common import clock, draw_str, StatValue
# import video

from featureDetection import isLoading

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

        # Resize to 300x100, as the features have been computed on that.
        area_of_interest_300x100 = cv2.resize(area_of_interest, (300, 100))


        # TODO: update framecounters using the detection method
        self.frameCounterTotal = self.frameCounterTotal + 1

        if isLoading(area_of_interest_300x100):
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
            #    # Handle empty queue here
            #    pass

        Input_Queue.put(frame)

    def cap_read(cv2_cap):
        ret, frame = cv2_cap.read()
        if ret:
            put_frame(frame)

    cap = cv2.VideoCapture('test.mp4')

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
    while True:
        cap_read(cap)



        if not Output_Queue.empty():
            result = Output_Queue.get()

            print("Total: {}, Running: {}, Paused: {}".format(result['frameCounterTotal'], result['frameCounterRunning'], result['frameCounterPaused']))
            cv2.imshow('Threaded Video', result['frame'])
            ch = cv2.waitKey(5)

        if ch == ord(' '):
            threaded_mode = not threaded_mode
        if ch == 27:
            break
    cv2.destroyAllWindows()

if __name__ == '__main__':
    main()

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScreenRecorderLib;

namespace VSScreenRecording
{
    class VSRecord
    {
          
        
        Recorder _rec;
        Stream _outStream;
        string inpDev = null;
        string outPath = "";
        string fileName = "";
        String windowType = "window";
        String fullScreenType = "fullscreen";
        String portionType = "portion";
        String monitorDeviceName;
        RecordableWindow window;
        int left;
        int top;
        int right;
        int bottom;

        public void setPath(string pth, string nm)
        {
            outPath = pth;
            fileName = nm;
        }

        public void setWindow(RecordableWindow rw)
        {
            window = rw;
        }

        public void setDisplay(String deviceName)
        {
            monitorDeviceName = deviceName;
        }

        public void setInputDev(string inp)
        {
            inpDev = inp;
        }

        public void setCoordinates(int lf, int tp, int rt, int bt)
        {
            //crop to a 400x400px square at x=400,y=400. Passing 0 for these values will default to full screen recording.
            left = lf;
            top = tp;
            right = rt;
            bottom = bt;
        }

        public List<RecordableWindow> getAllWindows()
        {
            List<RecordableWindow> windows = Recorder.GetWindows();
            return windows;
        }
        RecorderOptions getRecordingOptions(String recType)
        {
            
            RecorderOptions options;

            if(recType == windowType)
            {
                options = new RecorderOptions
                {
                    RecorderMode = RecorderMode.Video,
                    //If throttling is disabled, out of memory exceptions may eventually crash the program,
                    //depending on encoder settings and system specifications.
                    IsThrottlingDisabled = false,
                    //Hardware encoding is enabled by default.
                    IsHardwareEncodingEnabled = true,
                    //Low latency mode provides faster encoding, but can reduce quality.
                    IsLowLatencyEnabled = false,
                    //Fast start writes the mp4 header at the beginning of the file, to facilitate streaming.
                    IsMp4FastStartEnabled = false,
                    AudioOptions = new AudioOptions
                    {
                        Bitrate = AudioBitrate.bitrate_128kbps,
                        Channels = AudioChannels.Stereo,
                        IsAudioEnabled = true,
                        //IsOutputDeviceEnabled = true,
                        IsInputDeviceEnabled = true,
                        //AudioOutputDevice = selectedOutputDevice,
                        AudioInputDevice = inpDev,
                        InputVolume = 1
                    },
                    VideoOptions = new VideoOptions
                    {
                        BitrateMode = BitrateControlMode.UnconstrainedVBR,
                        Bitrate = 8000 * 1000,
                        Framerate = 60,
                        IsFixedFramerate = true,
                        EncoderProfile = H264Profile.Main
                    },
                    MouseOptions = new MouseOptions
                    {
                        //Displays a colored dot under the mouse cursor when the left mouse button is pressed.	
                        IsMouseClicksDetected = true,
                        MouseClickDetectionColor = "#FFFF00",
                        MouseRightClickDetectionColor = "#FFFF00",
                        MouseClickDetectionRadius = 30,
                        MouseClickDetectionDuration = 100,

                        IsMousePointerEnabled = true,
                        /* Polling checks every millisecond if a mouse button is pressed.
                           Hook works better with programmatically generated mouse clicks, but may affect
                           mouse performance and interferes with debugging.*/
                        MouseClickDetectionMode = MouseDetectionMode.Hook
                    },

                    // Display Options for recording a portion of the screen
                    DisplayOptions = new DisplayOptions
                    {
                        WindowHandle = window.Handle,
                        MonitorDeviceName = monitorDeviceName
                    },
                    RecorderApi = RecorderApi.WindowsGraphicsCapture
                };
            }
            else if(recType == portionType)
            {
                options = new RecorderOptions
                {

                    RecorderMode = RecorderMode.Video,
                    //If throttling is disabled, out of memory exceptions may eventually crash the program,
                    //depending on encoder settings and system specifications.
                    IsThrottlingDisabled = false,
                    //Hardware encoding is enabled by default.
                    IsHardwareEncodingEnabled = true,
                    //Low latency mode provides faster encoding, but can reduce quality.
                    IsLowLatencyEnabled = false,
                    //Fast start writes the mp4 header at the beginning of the file, to facilitate streaming.
                    IsMp4FastStartEnabled = false,
                    AudioOptions = new AudioOptions
                    {
                        Bitrate = AudioBitrate.bitrate_128kbps,
                        Channels = AudioChannels.Stereo,
                        IsAudioEnabled = true,
                        //IsOutputDeviceEnabled = true,
                        IsInputDeviceEnabled = true,
                        //AudioOutputDevice = selectedOutputDevice,
                        AudioInputDevice = inpDev,
                        InputVolume = 1
                    },
                    VideoOptions = new VideoOptions
                    {
                        BitrateMode = BitrateControlMode.UnconstrainedVBR,
                        Bitrate = 8000 * 1000,
                        Framerate = 60,
                        IsFixedFramerate = true,
                        EncoderProfile = H264Profile.Main
                    },
                    MouseOptions = new MouseOptions
                    {
                        //Displays a colored dot under the mouse cursor when the left mouse button is pressed.	
                        IsMouseClicksDetected = true,
                        MouseClickDetectionColor = "#FFFF00",
                        MouseRightClickDetectionColor = "#FFFF00",
                        MouseClickDetectionRadius = 30,
                        MouseClickDetectionDuration = 100,

                        IsMousePointerEnabled = true,
                        /* Polling checks every millisecond if a mouse button is pressed.
                           Hook works better with programmatically generated mouse clicks, but may affect
                           mouse performance and interferes with debugging.*/
                        MouseClickDetectionMode = MouseDetectionMode.Hook
                    },

                    // Display Options for recording a portion of the screen
                    DisplayOptions = new DisplayOptions
                    {
                        Left = left,
                        Top = top,
                        Right = right,
                        Bottom = bottom,
                        MonitorDeviceName = monitorDeviceName
                    },
                    RecorderApi = RecorderApi.DesktopDuplication

                };
            }
            else  // This is default
            {
                // if (recType == fullScreenType)
                options = new RecorderOptions
                {

                    RecorderMode = RecorderMode.Video,
                    //If throttling is disabled, out of memory exceptions may eventually crash the program,
                    //depending on encoder settings and system specifications.
                    IsThrottlingDisabled = false,
                    //Hardware encoding is enabled by default.
                    IsHardwareEncodingEnabled = true,
                    //Low latency mode provides faster encoding, but can reduce quality.
                    IsLowLatencyEnabled = false,
                    //Fast start writes the mp4 header at the beginning of the file, to facilitate streaming.
                    IsMp4FastStartEnabled = false,
                    AudioOptions = new AudioOptions
                    {
                        Bitrate = AudioBitrate.bitrate_128kbps,
                        Channels = AudioChannels.Stereo,
                        IsAudioEnabled = true,
                        //IsOutputDeviceEnabled = true,
                        IsInputDeviceEnabled = true,
                        //AudioOutputDevice = selectedOutputDevice,
                        AudioInputDevice = inpDev,
                        InputVolume = 1
                    },
                    VideoOptions = new VideoOptions
                    {
                        BitrateMode = BitrateControlMode.UnconstrainedVBR,
                        Bitrate = 8000 * 1000,
                        Framerate = 60,
                        IsFixedFramerate = true,
                        EncoderProfile = H264Profile.Main
                    },
                    MouseOptions = new MouseOptions
                    {
                        //Displays a colored dot under the mouse cursor when the left mouse button is pressed.	
                        IsMouseClicksDetected = true,
                        MouseClickDetectionColor = "#FFFF00",
                        MouseRightClickDetectionColor = "#FFFF00",
                        MouseClickDetectionRadius = 30,
                        MouseClickDetectionDuration = 100,

                        IsMousePointerEnabled = true,
                        /* Polling checks every millisecond if a mouse button is pressed.
                           Hook works better with programmatically generated mouse clicks, but may affect
                           mouse performance and interferes with debugging.*/
                        MouseClickDetectionMode = MouseDetectionMode.Hook
                    },
                    DisplayOptions = new DisplayOptions
                    {
                        MonitorDeviceName = monitorDeviceName
                    },
                };
            }
            

            return options;
        }

        public IDictionary<string, string> GetAudioDevices()
        {
            IDictionary<string, string> inputDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.InputDevices);
            IDictionary<string, string> outputDevices = Recorder.GetSystemAudioDevices(AudioDeviceSource.OutputDevices);

            Console.WriteLine($"Input devices are : {inputDevices}");
            foreach (KeyValuePair<string, string> entry in inputDevices)
            {
                Console.WriteLine($"Key : {entry.Key}, Val : {entry.Value}");
            }

            Console.WriteLine($"Output devices are : {outputDevices}");
            foreach (KeyValuePair<string, string> entry in outputDevices)
            {
                Console.WriteLine($"Key : {entry.Key}, Val : {entry.Value}");
            }

            return inputDevices;
        }

        public void CreateRecording(String recordingType)
        {
            RecorderOptions recOpt = getRecordingOptions(recordingType);
            _rec = Recorder.CreateRecorder(recOpt);
            _rec.OnRecordingComplete += Rec_OnRecordingComplete;
            _rec.OnRecordingFailed += Rec_OnRecordingFailed;
            _rec.OnStatusChanged += Rec_OnStatusChanged;

            //Record to a file
            string videoPath = Path.Combine(outPath, fileName);
            Console.WriteLine($"Full path is {videoPath}");
            if (File.Exists(videoPath))
            {
                Console.WriteLine($"Deleting file {videoPath}");
                File.Delete(videoPath);
            }
            
            _rec.Record(videoPath);
            //..Or to a stream
            //_outStream = new MemoryStream();
            //_rec.Record(_outStream);
        }
        public void EndRecording()
        {
            if(_rec != null)
            {
               _rec.Stop();
            }
        }
        private void Rec_OnRecordingComplete(object sender, RecordingCompleteEventArgs e)
        {
            //Get the file path if recorded to a file
            string path = e.FilePath;
            //or do something with your stream
            //... something ...
            _outStream?.Dispose();
        }
        private void Rec_OnRecordingFailed(object sender, RecordingFailedEventArgs e)
        {
            string error = e.Error;
                _outStream?.Dispose();
        }
        private void Rec_OnStatusChanged(object sender, RecordingStatusEventArgs e)
        {
            RecorderStatus status = e.Status;
        }
    }
}

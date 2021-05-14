using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VSScreenRecording
{
    public partial class Form1 : Form
    {
        VSRecord vsr;
        List<string> inputDevsKeys = new List<string>();
        List<string> inputDevsVals = new List<string>();
        IDictionary<string, ScreenRecorderLib.RecordableWindow> allwindows = new Dictionary<string, ScreenRecorderLib.RecordableWindow>();
        String vsr_dir = "vs_record";
        private GlobalHotkey ghk, ghk1;
        String recType;
        String windowType = "window";
        String fsType = "fullscreen";
        String portionType = "portion";
        bool textBoxFocus4 = true;
        bool textBoxFocus5 = true;
        bool textBoxFocus6 = true;
        bool textBoxFocus7 = true;

        Thread thread4 = null, thread5 = null, thread6 = null, thread7 = null;

        int stop_rec_id;
        int cap_scr_id;

        public Form1()
        {
            InitializeComponent();
            // To stop the recording
            ghk = new GlobalHotkey(Constants.CTRL + Constants.ALT, Keys.Z, this);

            // To register screen coordinates
            ghk1 = new GlobalHotkey(Constants.CTRL + Constants.ALT, Keys.C, this);

            vsr = new VSRecord();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out POINT lpPoint);

        public static Point GetCursorPosition()
        {
            POINT lpPoint;
            GetCursorPos(out lpPoint);
            // NOTE: If you need error handling
            // bool success = GetCursorPos(out lpPoint);
            // if (!success)

            return lpPoint;
        }

        private void Handle_Stop_Recording_HotKey()
        {
            // "Hotkey pressed!" : CTRL+ALT+Z
            vsr.EndRecording();
            button1.Enabled = true;
            button2.Enabled = false;
            label2.Text = "Recording ended...";

            this.WindowState = FormWindowState.Normal;
        }

        private void Handle_Screen_Capture_HotKey()
        {
            // "Hotkey pressed!" : CTRL+ALT+C
            textBoxFocus4 = false;
            textBoxFocus5 = false;
            textBoxFocus6 = false;
            textBoxFocus7 = false;

            if (thread4 != null && thread4.IsAlive)
            {
                try
                {
                    thread4.Abort();
                    thread4 = null;
                }
                catch (ThreadAbortException)
                {
                    Console.WriteLine("Handle_Screen_Capture_HotKey: Thread4 aborted.");
                    thread4 = null;
                }
                
            }

            
            if (thread5 != null && thread5.IsAlive)
            {
                try
                {
                    thread5.Abort();
                    thread5 = null;
                }
                catch (ThreadAbortException)
                {
                    Console.WriteLine("Handle_Screen_Capture_HotKey: Thread5 aborted.");
                    thread5 = null;
                }
            }

            if (thread6 != null && thread6.IsAlive)
            {
                try
                {
                    thread6.Abort();
                    thread6 = null;
                }
                catch (ThreadAbortException)
                {
                    Console.WriteLine("Handle_Screen_Capture_HotKey: Thread6 aborted.");
                    thread6 = null;
                }
            }

            if (thread4 != null && thread7.IsAlive)
            {
                try
                {
                    thread7.Abort();
                    thread7 = null;
                }
                catch (ThreadAbortException)
                {
                    Console.WriteLine("Handle_Screen_Capture_HotKey: Thread7 aborted.");
                    thread7 = null;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ghk.Unregiser())
                MessageBox.Show("Hotkey failed to unregister!");

            if (!ghk1.Unregiser())
                MessageBox.Show("Hotkey failed to unregister!");

            Handle_Screen_Capture_HotKey();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == Constants.WM_HOTKEY_MSG_ID)
            {
                int id = m.WParam.ToInt32();
                Console.WriteLine("The id is = " + id);
                if(id == stop_rec_id)
                {
                    Handle_Stop_Recording_HotKey();
                }
                else if(id == cap_scr_id)
                {
                    Handle_Screen_Capture_HotKey();
                }
                
            }
                
            base.WndProc(ref m);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Start recording when button is clicked
            // Disable the button recording starts
            String myfile = textBox2.Text.Trim() + label6.Text;
            vsr.setPath(textBox1.Text, myfile);

            int selInd = comboBox1.SelectedIndex;
            if (selInd < 0)
            {
                MessageBox.Show("Please select an Input Device", "Input Device", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Press Ctrl+ALT+Z to stop recording.", "Recording...",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            button1.Enabled = false;
            label2.Text = "Recording started...";
            button2.Enabled = true;
            vsr.setInputDev(inputDevsKeys[selInd]);
            this.WindowState = FormWindowState.Minimized;

            // Set Monitor (multiple displays)
            vsr.setDisplay(comboBox2.GetItemText(comboBox2.SelectedItem));

            // Check recording type 
            if(recType == windowType)
            {
                ScreenRecorderLib.RecordableWindow wnd = allwindows[comboBox3.GetItemText(comboBox3.SelectedItem)];
                vsr.setWindow(wnd);
                Console.WriteLine("Selecting a window");
            }
            else if(recType == portionType)
            {
                int left = Int32.Parse(textBox4.Text);
                int top = Int32.Parse(textBox6.Text);
                int right = Int32.Parse(textBox5.Text);
                int bottom = Int32.Parse(textBox7.Text);

                if(left >= right && top >= bottom)
                {
                    MessageBox.Show("Invalid Coordinates, left should be less than right and top should be less than bottom", "Screen Capture",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                vsr.setCoordinates(left, top, right, bottom);
                Console.WriteLine("Selecting a portion of the Screen.");
            }
            else
            {
                // Do nothing for full Screen mode
                Console.WriteLine("Full Screen Mode");
            }

            Handle_Screen_Capture_HotKey();
            vsr.CreateRecording(recType);
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (ghk.Register())
            {
                stop_rec_id = ghk.GetHashCode();
                Console.WriteLine("Hotkey for stopping the recording registered.");
                Console.WriteLine("The hashCode for CTRL + ALT + Z is: " + stop_rec_id);
            }  
            else
            {
                Console.WriteLine("Hotkey for stopping the recording failed to register");
                
            }
                

            if (ghk1.Register())
            {
                cap_scr_id = ghk1.GetHashCode();
                Console.WriteLine("Hotkey for registering the screen coordinates registered.");
                Console.WriteLine("The hashCode for CTRL + ALT + C is: " + cap_scr_id);
            }  
            else
            {
                Console.WriteLine("Hotkey for registering the screen coordinates failed to register");
            }
                

            String mydoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string[] paths = { mydoc, vsr_dir };
            string fullPath = Path.Combine(paths);
            // Create vsr directory in mydocuments if not exists already
            Directory.CreateDirectory(fullPath);

            textBox1.Text = fullPath;
            textBox1.Enabled = false;
            String timeStamp = DateTime.Now.ToString("yyyyMMdd-HHmmssffff");
            String filen = "vsr-" + timeStamp;
            textBox2.Text = filen;
            IDictionary<string, string> inputDevs = vsr.GetAudioDevices();
            
            comboBox1.Items.Clear();
            inputDevsKeys.Clear();
            inputDevsVals.Clear();
            foreach (KeyValuePair<string, string> entry in inputDevs)
            {
                comboBox1.Items.Add(entry.Value);
                inputDevsKeys.Add(entry.Key);
                inputDevsVals.Add(entry.Value);
            }
            button2.Enabled = false;

            // Get all the monitors connected.
            Screen[] allsc = Screen.AllScreens;
            // Primary Screen 
            // Primary Screen : Screen.PrimaryScreen.DeviceName
            foreach (var scr in allsc)
            {
                comboBox2.Items.Add(scr.DeviceName);
            }
            comboBox2.SelectedIndex = 0;

            comboBox3.Hide();
            textBox3.Hide();

            label9.Hide();
            label10.Hide();
            label11.Hide();
            label12.Hide();
            textBox4.Hide();
            textBox5.Hide();
            textBox6.Hide();
            textBox7.Hide();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            vsr.EndRecording();
            button1.Enabled = true;
            button2.Enabled = false;
            label2.Text = "Recording ended...";

            Handle_Screen_Capture_HotKey();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Handle_Screen_Capture_HotKey();

            textBox3.Text = "";
            textBox3.Hide();

            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";
            label9.Hide();
            label10.Hide();
            label11.Hide();
            label12.Hide();
            textBox4.Hide();
            textBox5.Hide();
            textBox6.Hide();
            textBox7.Hide();

            comboBox3.Show();
            comboBox3.Items.Clear();
            allwindows.Clear();
            //List<ScreenRecorderLib.RecordableWindow> = vsr.getAllWindows();
            List<ScreenRecorderLib.RecordableWindow> windows = ScreenRecorderLib.Recorder.GetWindows();
            Console.WriteLine("The list of values are : " + windows.Count());
            foreach(var wnd in windows)
            {
                comboBox3.Items.Add(wnd.Title);
                allwindows.Add(wnd.Title, wnd);
            }
            comboBox3.SelectedIndex = 0;
            recType = windowType;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            Handle_Screen_Capture_HotKey();

            label9.Hide();
            label10.Hide();
            label11.Hide();
            label12.Hide();
            textBox4.Hide();
            textBox5.Hide();
            textBox6.Hide();
            textBox7.Hide();
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            textBox7.Text = "";

            comboBox3.Items.Clear();
            allwindows.Clear();
            comboBox3.Hide();

            textBox3.Text = "Full Screen Recording";
            textBox3.Show();
            textBox3.Enabled = false;
            
            recType = fsType;
        }

        private delegate void DisplayValDelegate(TextBox tb, int i);

        private void DisplayVal(TextBox tb, int i)
        {
            tb.Text = i.ToString();
        }

        private void DisplayCoord4()
        {
            Console.WriteLine("Inside loop : DisplayCoord4 - thread4");
            while (textBoxFocus4)
            {
                Point p = GetCursorPosition();
                textBox4.Invoke(new DisplayValDelegate(DisplayVal), textBox4, p.X);
            }
            Console.WriteLine("Out of the loop : DisplayCoord4 - thread4");
            if (thread4!= null && thread4.IsAlive)
            {
                try
                {
                    thread4.Abort();
                    thread4 = null;
                }
                catch(ThreadAbortException)
                {
                    Console.WriteLine("DisplayCoord4: Thread4 aborted.");
                    thread4 = null;
                }
            }
                
        }

        private void DisplayCoord5()
        {
            while (textBoxFocus5)
            {
                Point p = GetCursorPosition();
                textBox5.Invoke(new DisplayValDelegate(DisplayVal), textBox5, p.X);
            }
            if (thread5 != null &&  thread5.IsAlive)
            {
                try
                {
                    thread5.Abort();
                    thread5 = null;
                }
                catch (ThreadAbortException)
                {
                    Console.WriteLine("DisplayCoord5: Thread5 aborted.");
                    thread5 = null;
                }
            }
        }

        private void DisplayCoord6()
        {
            while (textBoxFocus6)
            {
                Point p = GetCursorPosition();
                textBox6.Invoke(new DisplayValDelegate(DisplayVal), textBox6, p.Y);
            }
            if (thread6 != null && thread6.IsAlive)
            {
                try
                {
                    thread6.Abort();
                    thread6 = null;
                }
                catch (ThreadAbortException)
                {
                    Console.WriteLine("DisplayCoord6: Thread6 aborted.");
                    thread6 = null;
                }
            }
        }

        private void textBox6_Enter(object sender, EventArgs e)
        {
            textBoxFocus6 = true;
            thread6 = new Thread(DisplayCoord6);
            thread6.IsBackground = true;
            thread6.Start();
        }

        private void textBox7_Enter(object sender, EventArgs e)
        {
            textBoxFocus7 = true;
            thread7 = new Thread(DisplayCoord7);
            thread7.IsBackground = true;
            thread7.Start();
        }

        private void DisplayCoord7()
        {
            while (textBoxFocus7)
            {
                Point p = GetCursorPosition();
                textBox7.Invoke(new DisplayValDelegate(DisplayVal), textBox7, p.Y);
            }
            if (thread7 != null && thread7.IsAlive)
            {
                try
                {
                    thread7.Abort();
                    thread7 = null;
                }
                catch (ThreadAbortException)
                {
                    Console.WriteLine("DisplayCoord7: Thread7 aborted.");
                    thread7 = null;
                }
            }
        }

        private void textBox5_Enter(object sender, EventArgs e)
        {
            textBoxFocus5 = true;
            thread5 = new Thread(DisplayCoord5);
            thread5.IsBackground = true;
            thread5.Start();
        }

        


        private void textBox4_Enter(object sender, EventArgs e)
        {
            textBoxFocus4 = true;
            thread4 = new Thread(DisplayCoord4);
            thread4.IsBackground = true;
            thread4.Start();
            Console.WriteLine("Thread4 started....................=" + thread4.Name);
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            comboBox3.Items.Clear();
            comboBox3.Hide();
            allwindows.Clear();

            textBox3.Text = "";
            textBox3.Hide();
                        
            recType = portionType;

            //Point p = GetCursorPosition();
            //textBox3.Text = p.X + ":" + p.Y;
            label9.Show();
            label10.Show();
            label11.Show();
            label12.Show();
            textBox4.Show();
            textBox5.Show();
            textBox6.Show();
            textBox7.Show();

            textBox4.Focus();
        }
    }
}

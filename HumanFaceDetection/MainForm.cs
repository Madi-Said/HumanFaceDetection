using AForge.Video;
using AForge.Video.DirectShow;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HumanFaceDetection
{
    public partial class MainForm : Form
    {
        private System.Windows.Forms.Button detectBtn;
        private System.Windows.Forms.ComboBox cboDevice;
        private System.Windows.Forms.Label cameraLbl;
        private System.Windows.Forms.PictureBox captureBox;
        private FilterInfoCollection filter;
        private VideoCaptureDevice device;
        static readonly CascadeClassifier cascadeClassifier = new CascadeClassifier("training.xml");
        public MainForm()
        {
            InitializeComponent();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.detectBtn = new System.Windows.Forms.Button();
            this.cboDevice = new System.Windows.Forms.ComboBox();
            this.cameraLbl = new System.Windows.Forms.Label();
            this.captureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.captureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // detectBtn
            // 
            this.detectBtn.Location = new System.Drawing.Point(799, 16);
            this.detectBtn.Name = "detectBtn";
            this.detectBtn.Size = new System.Drawing.Size(121, 33);
            this.detectBtn.TabIndex = 0;
            this.detectBtn.Text = "&Détecter";
            this.detectBtn.UseVisualStyleBackColor = true;
            this.detectBtn.Click += new System.EventHandler(this.DetectBtn_Click);
            // 
            // cboDevice
            // 
            this.cboDevice.FormattingEnabled = true;
            this.cboDevice.Location = new System.Drawing.Point(54, 23);
            this.cboDevice.Name = "cboDevice";
            this.cboDevice.Size = new System.Drawing.Size(276, 21);
            this.cboDevice.TabIndex = 1;
            // 
            // cameraLbl
            // 
            this.cameraLbl.AutoSize = true;
            this.cameraLbl.Location = new System.Drawing.Point(3, 26);
            this.cameraLbl.Name = "cameraLbl";
            this.cameraLbl.Size = new System.Drawing.Size(49, 13);
            this.cameraLbl.TabIndex = 2;
            this.cameraLbl.Text = "Caméra :";
            // 
            // captureBox
            // 
            this.captureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.captureBox.Location = new System.Drawing.Point(3, 67);
            this.captureBox.Name = "captureBox";
            this.captureBox.Size = new System.Drawing.Size(918, 349);
            this.captureBox.TabIndex = 3;
            this.captureBox.TabStop = false;
            this.captureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 418);
            this.Controls.Add(this.captureBox);
            this.Controls.Add(this.cameraLbl);
            this.Controls.Add(this.cboDevice);
            this.Controls.Add(this.detectBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Détection du visage en utilisant la Webcam";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.captureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void MainForm_Load(object sender, EventArgs e)
        {
            filter = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in filter)
                cboDevice.Items.Add(device.Name);
            cboDevice.SelectedIndex = 0;
        }

        private void DetectBtn_Click(object sender, EventArgs e)
        {
            device = new VideoCaptureDevice(filter[cboDevice.SelectedIndex].MonikerString);
            device.NewFrame += Device_NewFrame;
            device.Start();
        }

        private void Device_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            Image<Bgr, byte> grayImage = new Image<Bgr, byte>(bitmap);
            Rectangle[] rectangles = cascadeClassifier.DetectMultiScale(grayImage, 1.2, 1);
            foreach (Rectangle rectangle in rectangles)
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    using (Pen pen = new Pen(Color.Red, 1))
                    {
                        graphics.DrawRectangle(pen, rectangle);
                    }
                }
            }
            captureBox.Image = bitmap;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (device.IsRunning)
                device.Stop();
        }
    }
}
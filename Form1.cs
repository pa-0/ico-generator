using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace PngtoIco
{
    public partial class Form1 : Form
    {

        string directory, nameFile, filePath, LAname = "";
        bool isFileOpen=false, isDrag=false;
        double imgWidth, imgHeight;
        Image img, newImage;
        string[] sizeIco = { "8x8", "16x16", "32x32", "64x64", "128x128", "256x256", "512x512", "1024x1024" };
        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < 8; i++)
            {
                SizeIcoBox.Items.Add(sizeIco[i]);
            }
            SizeIcoBox.Text = "16x16";
            FileNameIcon.Text = "icon";

        }

        private void OpenPng_Click(object sender, EventArgs e)
        {
            if (isDrag == false)
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = LAname;
                    openFileDialog.Filter = "image files (*.png, *.bmp, *.gif, *.jpg)|*.png;*.bmp;*.gif;*.jpg";//|All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 2;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        //Get the path of specified file
                        filePath = openFileDialog.FileName;
                        directory = System.IO.Path.GetDirectoryName(filePath);
                        FilePathName.Text = directory;
                    }
                }
            }
            else
            {

                filePath = FilePathName.Text;
                directory = System.IO.Path.GetDirectoryName(filePath);
                FilePathName.Text = directory;
            }
            if (filePath != null)
            {
                LAname = directory;
                Bitmap bitmap = new Bitmap(@filePath);
                PicinBox(bitmap);

                img = System.Drawing.Image.FromFile(filePath);
                imgWidth = img.Width;
                imgHeight = img.Height;
                MessageBox.Show("Width: " + img.Width + ", Height: " + img.Height);


                nameFile = (Path.GetFileName(filePath));
                Resize(filePath, @directory + "\\new" + nameFile, 500, 500);

                FileStream fs = new FileStream((@directory + "\\new" + nameFile), FileMode.Open, FileAccess.Read);
                Image imgn = Image.FromStream(fs);
                fs.Close();
                bitmap = (Bitmap)imgn.Clone();
                imgn.Dispose();

                PicinBox(bitmap);
                File.Delete(@directory + "\\new" + nameFile);
                isFileOpen = true;
   
            }
            isDrag = false;
        }

        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void panel1_DragEnter(object sender, DragEventArgs e)
        {

            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
           foreach(string file in files) FilePathName.Text = file;
           
            isDrag = true;
            OpenPng_Click(sender, e);
            // ListFiles.Items.Add(file.ToString());
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (isFileOpen == true)
            {
                nameFile = (Path.GetFileName(filePath));
                Resize(filePath, @directory + "\\new" + nameFile, 500, 500);
                FileStream fs = new FileStream((@directory + "\\new" + nameFile), FileMode.Open, FileAccess.Read);
                Image imgn = Image.FromStream(fs);
                fs.Close();
                Bitmap bitmap = (Bitmap)imgn.Clone();
                imgn.Dispose();
                PicinBox(bitmap);

                LAname = FilePathName.Text;
                bool exists = System.IO.Directory.Exists(LAname);

                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(LAname);
                }
                string outputFileName = FileNameIcon.Text;
                if (outputFileName == "")
                {
                    outputFileName = "icon";
                    FileNameIcon.Text = "icon";
                }
                int size = 16;
                if (SizeIcoBox.Text == "8x8") size = 8;
                if (SizeIcoBox.Text == "16x16") size = 16;
                if (SizeIcoBox.Text == "32x32") size = 32;
                if (SizeIcoBox.Text == "64x64") size = 64;
                if (SizeIcoBox.Text == "128x128") size = 128;
                if (SizeIcoBox.Text == "256x256") size = 256;
                if (SizeIcoBox.Text == "512x512") size = 512;
                if (SizeIcoBox.Text == "1024x1024") size = 1024;
                if (!System.IO.File.Exists(LAname + "\\" + outputFileName + ".ico"))
                {
                    ConvertToIcon(@directory + "\\new" + nameFile, LAname + "\\" + outputFileName + ".ico", size, false);
                    // ConvertToIco(img, LAname + "\\" + outputFileName + ".ico", 500);
                    MessageBox.Show("The file created successfully in:" + LAname + "\\" + outputFileName + ".ico");
                }
                else
                {
                    DialogResult d;
                    d = MessageBox.Show("The file " + nameFile + " EXISTS. Do you want to OVERLAY?", "or No to Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                    if (d == DialogResult.Yes)
                    {
                        ConvertToIcon(@directory + "\\new" + nameFile, LAname + "\\" + outputFileName + ".ico", size, false);
                        MessageBox.Show("The file created successfully in:" + LAname + "\\" + outputFileName + ".ico");
                    }
                    else
                    {
                        MessageBox.Show("The file was not created");
                    }
                    
                }

                System.IO.File.Delete(@directory + "\\new" + nameFile);
                pictureBox1.Update();
            }
            else
            {
                MessageBox.Show("open a picture");
            }
        }

        private void Dir_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fd1 = new FolderBrowserDialog();
                //fd1.ShowDialog();
                if (fd1.ShowDialog() == DialogResult.OK)
                {
                    FilePathName.Text = fd1.SelectedPath;
                    LAname = FilePathName.Text;
                    directory = FilePathName.Text;
                }
            }
            catch
            {

            }
        }

        public void Resize(string imageFile, string outputFile, int newWidth, int newHeight)
        {
            using (var srcImage = Image.FromFile(imageFile))
            {
                // var newWidth = (int)(srcImage.Width * scaleFactor);
                // var newHeight = (int)(srcImage.Height * scaleFactor);

                using (newImage = new Bitmap(newWidth, newHeight))
                using (var graphics = Graphics.FromImage(newImage))
                {
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawImage(srcImage, new Rectangle(0, 0, newWidth, newHeight));


                    newImage.Save(outputFile);
                }
            }
        }



      
        private void PicinBox(Bitmap bitmap)
        {

            pictureBox1.Image = bitmap;
            Color pixel5by10 = bitmap.GetPixel(5, 10);
        }


        public static bool ConvertToIcon(string inputPath, string outputPath, int size, bool preserveAspectRatio)
        {
           
            using (FileStream input = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
            using (FileStream output = new FileStream(outputPath, FileMode.OpenOrCreate))
            {
                Bitmap inputBitmap = (Bitmap)Bitmap.FromStream(input);
                if (inputBitmap != null)
                {
                    int width, height;
                    if (preserveAspectRatio)
                    {
                        width = size;
                        height = inputBitmap.Height / inputBitmap.Width * size;
                    }
                    else
                    {
                        width = height = size;
                    }
                    Bitmap newBitmap = new Bitmap(inputBitmap, new Size(width, height));
                    if (newBitmap != null)
                    {
                        // save the resized png into a memory stream for future use
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            newBitmap.Save(memoryStream, ImageFormat.Png);

                            BinaryWriter iconWriter = new BinaryWriter(output);
                            if (output != null && iconWriter != null)
                            {
                                // 0-1 reserved, 0
                                iconWriter.Write((byte)0);
                                iconWriter.Write((byte)0);

                                // 2-3 image type, 1 = icon, 2 = cursor
                                iconWriter.Write((short)1);

                                // 4-5 number of images
                                iconWriter.Write((short)1);

                                // image entry 1
                                // 0 image width
                                iconWriter.Write((byte)width);
                                // 1 image height
                                iconWriter.Write((byte)height);

                                // 2 number of colors
                                iconWriter.Write((byte)0);

                                // 3 reserved
                                iconWriter.Write((byte)0);

                                // 4-5 color planes
                                iconWriter.Write((short)0);

                                // 6-7 bits per pixel
                                iconWriter.Write((short)32);

                                // 8-11 size of image data
                                iconWriter.Write((int)memoryStream.Length);

                                // 12-15 offset of image data
                                iconWriter.Write((int)(6 + 16));

                                // write image data
                                // png data must contain the whole png data file
                                iconWriter.Write(memoryStream.ToArray());

                                iconWriter.Flush();

                                return true;
                            }
                        }
                    }
                    return false;
                }
                return false;
            }
        }
    }
}

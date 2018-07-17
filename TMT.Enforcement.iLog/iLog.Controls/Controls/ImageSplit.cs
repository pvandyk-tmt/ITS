using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TMT.Core.Camera.Base;
using TMT.Core.Camera.Utils;

namespace TMT.Enforcement.iLog.Controls.Controls
{
    public partial class ImageSplit : UserControl, IImageViewer
    {
        private const int Spacing = 4;
        private const int Rows = 2;
        private const int Columns = 2;

        public ImageSplit()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            Controls.Clear();
        }

        public objImageViewer VisibleImage()
        {
            if (Controls.Count > 0)
            {
                return (objImageViewer)Controls[0];
            }

            return null;
        }

        public bool AddPictureFile(cPictureFile pic, bool fitToFrame)
        {
            try
            {
                foreach (objImageViewer viewer in Controls)
                {
                    viewer.Image = null;
                }

                Controls.Clear();

                cCamera driver = pic.pBelongsToFilm.pCameraDriver;
                List<cPicture> pictures;
                string message;

                if (!driver.developPicture(pic, out pictures, out message))
                {
                    return false;
                }

                int i = 0;
                foreach (cPicture picture in pictures)
                {
                    AddToPanel(i, picture, fitToFrame);
                    i++;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        private void AddToPanel(int i, cPicture picture, bool fitToFrame)
        {
            objImageViewer viewer = new objImageViewer
            {
                Width = GetWidth(),
                Height = GetHeight(),
                Left = GetLeft(i, Columns),
                Top = GetTop(i, Columns),
            };

            Controls.Add(viewer);
            Controls.SetChildIndex(viewer, i);

            if (fitToFrame)
            {
                Image img = ResizeImage(cImage.ByteArrToImage(picture.pJpeg), new Size(viewer.Width, viewer.Height));
                viewer.Image = img;
            }
            else
            {
                viewer.Image = cImage.ByteArrToImage(picture.pJpeg);
            }
        }

        private int GetLeft(int index, int colT)
        {
            int r = index / colT;
            int c = index - (r * colT);
            return Spacing + c * GetWidth() + c * Spacing;
        }

        private int GetTop(int index, int colT)
        {
            int r = index / colT;
            return Spacing + r * GetHeight() + r * Spacing;
        }

        private int GetHeight()
        {
            return (Height / Rows) - (Spacing * Rows);
        }

        private int GetWidth()
        {
            return (Width / Columns) - (Spacing * Columns);
        }

        public Image ResizeImage(Image imgToResize, Size size)
        {
            return new Bitmap(imgToResize, size);
        }

        public void Zoom()
        {
            if (Controls.Count > 0)
            {
                foreach (var control in Controls)
                {
                    if (control is objImageViewer)
                    {
                        var viewer = (objImageViewer)control;
                        viewer.Zoom += 0.1F;
                    }
                }
            }
        }
    }
}

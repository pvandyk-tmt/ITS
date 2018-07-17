using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using iLog.Camera.Base;
using iLog.Utils;

namespace TMT.iLog.Controls
{
    public partial class ImageTab : UserControl, IImageViewer
    {
        public ImageTab()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            tabControlImages.TabPages.Clear();
        }

        public objImageViewer VisibleImage()
        {
            if (tabControlImages.TabPages.Count > 0)
            {
                return (objImageViewer) tabControlImages.TabPages[tabControlImages.SelectedIndex].Controls[0];
            }

            return null;
        }

        public bool AddPictureFile(cPictureFile pic, bool fitToFrame)
        {
            try
            {
                foreach (TabPage tabPage in tabControlImages.TabPages)
                {
                    if (tabPage.Controls.Count > 0)
                    {
                        var viewer = (objImageViewer) tabPage.Controls[0];
                        viewer.Image = null;
                        viewer.Dispose();
                    }
                }

                tabControlImages.TabPages.Clear();

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
                    tabControlImages.TabPages.Add("Image " + i);

                    if (fitToFrame)
                    {
                        Image img = ResizeImage(cImage.ByteArrToImage(picture.pJpeg), new Size(tabControlImages.TabPages[i].Width, tabControlImages.TabPages[i].Height));
                        var viewer = new objImageViewer {Image = img, Dock = DockStyle.Fill};
                        tabControlImages.TabPages[i].Controls.Add(viewer);
                    }
                    else
                    {
                        var viewer = new objImageViewer {Image = cImage.ByteArrToImage(picture.pJpeg), Dock = DockStyle.Fill};
                        tabControlImages.TabPages[i].Controls.Add(viewer);
                    }

                    i++;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public Image ResizeImage(Image imgToResize, Size size)
        {
            return new Bitmap(imgToResize, size);
        }

        public void Zoom()
        {
            if (tabControlImages.TabPages.Count > 0)
            {
                var viewer = (objImageViewer) tabControlImages.TabPages[tabControlImages.SelectedIndex].Controls[0];
                viewer.Zoom += 0.1F;
            }
        }
    }
}
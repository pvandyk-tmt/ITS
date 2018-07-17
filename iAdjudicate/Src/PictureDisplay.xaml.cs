using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TMT.iAdjudicate
{
    /// <summary>
    /// Interaction logic for PictureDisplay.xaml
    /// </summary>
    public partial class cPictureDisplay : UserControl
    {
        //Image mImage = null;
        ////private System.Drawing.Size mSize = new Size();
        ////private System.Drawing.RectangleF mRect = new RectangleF();

        private int mZoomFactor = 150;
        private double mImageWidth = 0;
        private double mImageHeight = 0;
        private int mImagePages = 1;
        private int mCurrentPage = 0;
        private string mError = string.Empty;

        private TransformGroup mMoveScale = new TransformGroup();
        private Point origin;
        private Point start;

        /// <summary>
        /// Last error message (if any).
        /// </summary>
        public string pError
        {
            get { return mError; }
        }

        /// <summary>
        /// Displays images for 'WPF' applications.
        /// </summary>
        public cPictureDisplay()
        {
            InitializeComponent();

            ////this.AutoScroll = true;
            ////this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);


            mMoveScale = new TransformGroup();

            var scle = new ScaleTransform();
            var move = new TranslateTransform();
            mMoveScale.Children.Add(scle);
            mMoveScale.Children.Add(move);

            mImage.RenderTransformOrigin = new Point(0.5, 0.5);
            mImage.RenderTransform = mMoveScale;
            mImage.MinWidth = bdrImage.ActualWidth / 2;
            mImage.MouseWheel += image_MouseWheel;
            mImage.MouseLeftButtonDown += image_MouseLeftButtonDown;
            mImage.MouseLeftButtonUp += image_MouseLeftButtonUp;
            mImage.MouseMove += image_MouseMove;
        }

        public void releaseImage()
        {
            //lblNoImageMessage.Visibility = Visibility.Hidden;

            //if (mImage != null)
            //{
            //    mImage.MouseWheel -= image_MouseWheel;
            //    mImage.MouseLeftButtonDown -= image_MouseLeftButtonDown;
            //    mImage.MouseLeftButtonUp -= image_MouseLeftButtonUp;
            //    mImage.MouseMove -= image_MouseMove;
            //    //    mImage.Dispose();
            //}

            bdrImage.Child = null;
            mImage.Source = null;

            ////this.Refresh();
            this.InvalidateVisual();
        }

        /// <summary>
        /// Display the next image from a multi-page image file.
        /// </summary>
        public void switchPicture()
        {
            if (mImagePages > 1)
            {
                mCurrentPage++;
                if (mCurrentPage >= mImagePages)
                    mCurrentPage = 0;
                ////mImage.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, mCurrentPage);
                ////this.InvalidateVisual();
            }
        }

        /// <summary>
        /// Loads and display an image.
        /// </summary>
        /// <param name="filename">Full path to image file</param>
        /// <param name="multiPage">More than 1 image exist in image file (common for .tiff images)</param>
        /// <returns>True if all okay, else tooltip displays error message</returns>
        public bool loadPicture(string filename, bool scaleParent, out bool multiPage)
        {
            mError = string.Empty;

            multiPage = false;

            releaseImage();

            try
            {
                var bitm = new BitmapImage();
                bitm.BeginInit();
                bitm.UriSource = new Uri(filename);
                bitm.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitm.CacheOption = BitmapCacheOption.OnLoad;
                bitm.EndInit();

                //if (mImage == null)
                //{
                //    mImage = new Image();
                //    mImage.RenderTransformOrigin = new Point(0.5, 0.5);
                //    mImage.RenderTransform = mMoveScale;
                //    mImage.MinWidth = bdrImage.ActualWidth / 2;
                //    mImage.MouseWheel += image_MouseWheel;
                //    mImage.MouseLeftButtonDown += image_MouseLeftButtonDown;
                //    mImage.MouseLeftButtonUp += image_MouseLeftButtonUp;
                //    mImage.MouseMove += image_MouseMove;
                //    bdrImage.Child = mImage;
                //}
                mImage.Source = bitm;
                bdrImage.Child = mImage;

                mCurrentPage = 0;
                mImagePages = 1;// mImage.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page);
                multiPage = (mImagePages > 1);

                //if (filename.ToLower().IndexOf(".tif") > 1)
                //{
                //    // Get all frames from the TIFF file.
                //    BitmapSource[] oBmpSrce = TIFF_Open(filename);
                //    // Number of frames.
                //    int iLength = oBmpSrce.Length;
                //    Image[] oImage = new Image[iLength];
                //    for (int i = 0; i < iLength; i++)
                //    {
                //        oImage[i] = new Image();
                //    }
                //    // Show frames in Image controls.
                //    for (int i = 0; i < iLength; i++)
                //    {
                //        oImage[i].Width = oBmpSrce[i].PixelWidth;
                //        oImage[i].Height = oBmpSrce[i].PixelHeight;
                //        oImage[i].Source = oBmpSrce[i];
                //    }
                //}

                ////mZoomFactor = mImage.Width / 5; // Zoom In/Out at 5 times the Image width

                ////this.AutoScrollMinSize = new Size(mImage.Width, mImage.Height);

                mImage.ToolTip = filename;

                mImageWidth = mImage.ActualWidth;
                mImageHeight = mImage.ActualHeight;

                if (this.Parent is Border && scaleParent)
                {
                    Border bdr = (Border)this.Parent;

                    if (mImage.Source.Height > 100)
                    {
                        bdr.Width = (mImage.Source.Width / 2) + 4;
                        bdr.Height = (mImage.Source.Height / 2) + 4;
                    }
                    else
                    {
                        bdr.Width = mImage.Source.Width + 4;
                        bdr.Height = mImage.Source.Height + 4;
                    }
                }

                ////mSize.Width = mImageWidth;
                ////mSize.Height = mImageHeight;

                zoomOriginal();

                ////mLastPosition.X = AutoScrollPosition.X;
                ////mLastPosition.Y = AutoScrollPosition.Y;

                //lblNoImageMessage.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                mError = ex.Message;
                ////this.toolTip.SetToolTip(this, "Error loading '" + filename + "'. \n" + ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Override OnPaint event.
        /// </summary>
        /// <param name="e">Paint event arguments</param>
        ////protected override void OnPaint(PaintEventArgs e)
        ////{
        ////    base.OnPaint(e);

        ////    if (mImage != null)
        ////    {
        ////        double fact = 0;

        ////        if (mImageWidth < this.Width)
        ////            fact = ((double)this.Width / 2.0) - ((double)mImageWidth / 2.0); // Move image to centre of picture box when it's display gets smaller than the box size

        ////        mRect.X = this.AutoScrollPosition.X + (int)fact;
        ////        mRect.Y = this.AutoScrollPosition.Y;

        ////        mRect.Width = mImageWidth;
        ////        mRect.Height = mImageHeight;
        ////        e.Graphics.DrawImage(mImage, mRect);
        ////        //e.Graphics.DrawImage(mImage, new RectangleF(this.AutoScrollPosition.X, this.AutoScrollPosition.Y, mImageWidth, mImageHeight));
        ////    }

        ////    lblNoImageMessage.Top = this.Top + (this.Height / 2) - (lblNoImageMessage.Height / 2);
        ////    lblNoImageMessage.Left = this.Left + (this.Width / 2) - (lblNoImageMessage.Width / 2);
        ////}

        #region ----- Mouse-down scrolling -----
        //protected Point mClickPosition;
        //protected Point mLastPosition;
        //protected Point mScrollPosition;

        //protected override void OnMouseDown(MouseButtonEventArgs e)
        //{
        //    mClickPosition = e.GetPosition(this);
        //}

        //protected override void OnMouseUp(MouseButtonEventArgs e)
        //{
        //    this.Cursor = null;
        //    mLastPosition.X = AutoScrollPosition.X;
        //    mLastPosition.Y = AutoScrollPosition.Y;
        //}

        //protected override void OnMouseMove(MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        this.Cursor = Cursors.Hand;
        //        mScrollPosition.X = mClickPosition.X - e.X - mLastPosition.X;
        //        mScrollPosition.Y = mClickPosition.Y - e.Y - mLastPosition.Y;
        //        AutoScrollPosition = mScrollPosition;
        //    }
        //}
        #endregion


        /// <summary>
        /// Zooms are based on increasing / decreasing Width.
        /// Correct the Height to keep the aspect ration correct.
        /// </summary>
        /// <param name="newWidth">New Width</param>
        /// <returns>Corrected new Height</returns>
        //private double correctHeightAspect(double newWidth)
        //{
        //    return (((double)newWidth / (double)mImage.Width) * (double)mImage.Height);
        //}

        ///// <summary>
        ///// Zoom in.
        ///// </summary>
        //public void zoomIn()
        //{
        //    if (mImage == null)
        //        return;

        //    Point pnt = mImage.RenderTransformOrigin;
        //    pnt.X = pnt.Y = 0.5;  // Zoom around middle
        //    zoom(pnt, 0.2);

        //    //if (mImageWidth < mZoomFactor*30)
        //    //{
        //    //    mImageWidth += mZoomFactor;
        //    //    mImageHeight = correctHeightAspect(mImageWidth);

        //    ////mSize.Width = mImageWidth;
        //    ////mSize.Height = mImageHeight;
        //    ////this.AutoScrollMinSize = mSize;

        //    //this.InvalidateVisual();
        //    //}
        //}

        ///// <summary>
        ///// Zoom out.
        ///// </summary>
        //public void zoomOut()
        //{
        //    if (mImage == null)
        //        return;

        //    Point pnt = mImage.RenderTransformOrigin;
        //    pnt.X = pnt.Y = 0.5;  // Zoom around middle
        //    zoom(pnt, -0.2);

        //    //if ((mImageWidth > mZoomFactor) && (mImageHeight > mZoomFactor) && (mImageHeight > this.Height))
        //    //{
        //    //    mImageWidth -= mZoomFactor;
        //    //    mImageHeight = correctHeightAspect(mImageWidth);

        //    //mSize.Width = mImageWidth;
        //    //mSize.Height = mImageHeight;
        //    //this.AutoScrollMinSize = mSize;

        //    //this.InvalidateVisual();
        //    //}
        //}

        /// <summary>
        /// Zoom 1:1.
        /// </summary>
        public void zoomOriginal()
        {
            if (mImage == null)
                return;

            Point pnt = mImage.RenderTransformOrigin;
            pnt.X = pnt.Y = 0.5;
            mImage.RenderTransformOrigin = pnt;

            TransformGroup transformGroup = (TransformGroup)mImage.RenderTransform;
            ScaleTransform scale = (ScaleTransform)transformGroup.Children[0];
            TranslateTransform move = (TranslateTransform)transformGroup.Children[1];
            move.X = move.Y = 0;
            scale.ScaleX = scale.ScaleY = 1;

            //mImageWidth = mImage.Width;
            //mImageHeight = mImage.Height;

            //mSize.Width = mImageWidth;
            //mSize.Height = mImageHeight;
            //this.AutoScrollMinSize = mSize;

            //this.InvalidateVisual();
        }

        ///// <summary>
        ///// Zoom that the Image width fits in area.
        ///// </summary>
        //public void zoomWidth()
        //{
        //    if (mImage == null)
        //        return;

        //    Point pnt = mImage.RenderTransformOrigin;
        //    pnt.X = 0.5;
        //    pnt.Y = 0;
        //    mImage.RenderTransformOrigin = pnt;

        //    TransformGroup transformGroup = (TransformGroup)mImage.RenderTransform;
        //    ScaleTransform scale = (ScaleTransform)transformGroup.Children[0];
        //    TranslateTransform move = (TranslateTransform)transformGroup.Children[1];

        //    move.X = move.Y = 0;
        //    scale.ScaleX = scale.ScaleY = this.ActualWidth / mImage.ActualWidth;

        //    //mImageWidth = this.Width - 25; // The 25 is the right scrollbar space
        //    //mImageHeight = correctHeightAspect(mImageWidth);

        //    //mSize.Width = mImageWidth;
        //    //mSize.Height = mImageHeight;
        //    //this.AutoScrollMinSize = mSize;

        //    //this.InvalidateVisual();
        //}

        /// <summary>
        /// Zoom that the Image height fits in area.
        /// </summary>
        //public void zoomHeight()
        //{
        //    if (mImage != null)
        //    {
        //        //mImageHeight = this.Height - 25; // The 25 is the bottom scrollbar space
        //        //mImageWidth = (int)((((double)mImageHeight / (double)mImage.Height) * (double)mImage.Width) + 0.5);

        //        //mSize.Width = mImageWidth;
        //        //mSize.Height = mImageHeight;
        //        //this.AutoScrollMinSize = mSize;

        //        //this.InvalidateVisual();
        //    }
        //}

        private void image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (mImage == null)
                return;

            this.Cursor = null;
            mImage.ReleaseMouseCapture();
        }

        private Point mPicOrigin;
        private void image_MouseMove(object sender, MouseEventArgs e)
        {
            if (mImage == null)
                return;

            if (!mImage.IsMouseCaptured)
                return;

            var tt = (TranslateTransform)((TransformGroup)mImage.RenderTransform).Children.First(tr => tr is TranslateTransform);
            Vector v = start - e.GetPosition(bdrImage);
            tt.X = origin.X - v.X;
            tt.Y = origin.Y - v.Y;
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (mImage == null)
                return;

            this.Cursor = Cursors.Hand;
            mImage.CaptureMouse();
            var tt = (TranslateTransform)((TransformGroup)mImage.RenderTransform).Children.First(tr => tr is TranslateTransform);
            start = e.GetPosition(bdrImage);
            origin = new Point(tt.X, tt.Y);
        }

        private void zoom(Point orig, double zoomfact)
        {
            mImage.RenderTransformOrigin = orig;

            TransformGroup transformGroup = (TransformGroup)mImage.RenderTransform;
            ScaleTransform scale = (ScaleTransform)transformGroup.Children[0];

            bool doZoom = true;

            if (scale.ScaleX < 0.8 && zoomfact < 0) // Do not zoom to far out (too small)
                doZoom = false;

            if (doZoom)
            {
                scale.ScaleX += zoomfact;
                scale.ScaleY += zoomfact;
            }
        }

        private bool mBusyD = false;
        private void image_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (mImage == null)
                return;

            if (mBusyD)
                return;
            mBusyD = true;

            double sx, sy;
            //Point mpB = e.GetPosition(bdrImage); // Mouse pointer on Border
            Point mpI = e.GetPosition(mImage);   // Mouse pointer on Image

            sx = mpI.X / mImage.ActualWidth;
            sy = mpI.Y / mImage.ActualHeight;

            Point pnt1 = mImage.RenderTransformOrigin;
            pnt1.X = sx;
            pnt1.Y = sy;
            this.zoom(pnt1, e.Delta > 0 ? 0.2 : -0.2);


            //mImage.RenderTransformOrigin = pnt1;

            //TransformGroup transformGroup = (TransformGroup) mImage.RenderTransform;
            //ScaleTransform scale = (ScaleTransform) transformGroup.Children[0];
            //TranslateTransform move = (TranslateTransform)transformGroup.Children[1];

            //double zoom = e.Delta > 0 ? .2 : -.2;
            //move.X = move.Y = 0;
            //scale.ScaleX += zoom;
            //scale.ScaleY += zoom;

            mBusyD = false;
        }
        /*
        http://msdn.microsoft.com/en-us/library/ms748873.aspx
       
        http://stackoverflow.com/questions/741956/wpf-pan-zoom-image
        http://msdn.microsoft.com/en-us/library/ms750596.aspx
        http://stackoverflow.com/questions/1030367/silverlight-3-scaletransform-or-other-method-to-zoom-in-a-canvas
        http://stackoverflow.com/questions/777262/whats-wrong-with-this-image-panning-algorithm
        http://www.wpf-graphics.com/ZoomPanel.aspx
         
        http://www.xs4all.nl/~wrb/Articles_2010/Article_WPFImageTIFF_01.htm
        */
    }
}

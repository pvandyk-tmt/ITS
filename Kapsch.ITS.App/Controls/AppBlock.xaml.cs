using System.Windows;
using System.Windows.Controls;

namespace Kapsch.ITS.App.Controls
{
    /// <summary>
    /// Interaction logic for AppBlock.xaml
    /// </summary>
    public partial class AppBlock : UserControl
    {
        private int _appCount;

        public AppBlock()
        {
            InitializeComponent();
        }

        public bool CanAdd()
        {
            if (AppearanceType == AppearanceType.OneBlock)
                return _appCount < 1;

            if (AppearanceType == AppearanceType.TwoHorizontalBlocks)
                return _appCount < 2;

            if (AppearanceType == AppearanceType.TwoVerticalBlocks)
                return _appCount < 2;

            if (AppearanceType == AppearanceType.ThreeHorizontalBlocks)
                return _appCount < 3;

            if (AppearanceType == AppearanceType.FourBlocks)
                return _appCount < 4;

            return true;
        }

        public void Add(Tile button)
        {
            button.Margin = new Thickness(5);

            if (AppearanceType == AppearanceType.OneBlock)
            {
                Grid.SetColumn(button, 0);
                Grid.SetColumnSpan(button, 2);
                Grid.SetRow(button, 0);
                Grid.SetRowSpan(button, 2);

                ContentGrid.Children.Add(button);
            }

            if (AppearanceType == AppearanceType.TwoHorizontalBlocks)
            {
                Grid.SetColumn(button, 0);
                Grid.SetColumnSpan(button, 2);
                Grid.SetRow(button, _appCount == 0 ? 0 : 1);

                ContentGrid.Children.Add(button);
            }

            if (AppearanceType == AppearanceType.TwoVerticalBlocks)
            {
                Grid.SetRow(button, 0);
                Grid.SetRowSpan(button, 2);
                Grid.SetColumn(button, _appCount == 0 ? 0 : 1);

                ContentGrid.Children.Add(button);
            }

            if (AppearanceType == AppearanceType.FourBlocks)
            {
                switch (_appCount)
                {
                    case 0:
                        Grid.SetColumn(button, 0);
                        Grid.SetRow(button, 0);
                        break;
                    case 1:
                        Grid.SetColumn(button, 1);
                        Grid.SetRow(button, 0);
                        break;
                    case 2:
                        Grid.SetColumn(button, 0);
                        Grid.SetRow(button, 1);
                        break;

                    default:
                        Grid.SetColumn(button, 1);
                        Grid.SetRow(button, 1);
                        break;
                }

                ContentGrid.Children.Add(button);
            }

            if (AppearanceType == AppearanceType.ThreeHorizontalBlocks)
            {
                switch (_appCount)
                {
                    case 0:
                        Grid.SetColumn(button, 0);
                        Grid.SetColumnSpan(button, 2);
                        Grid.SetRow(button, 0);
                        break;

                    case 1:
                        Grid.SetColumn(button, 0);
                        Grid.SetRow(button, 1);
                        break;

                    default:
                        Grid.SetColumn(button, 1);
                        Grid.SetRow(button, 1);
                        break;
                }

                ContentGrid.Children.Add(button);
            }

            _appCount++;
        }

        public AppearanceType AppearanceType { get; set; }
    }

    public enum AppearanceType
    {
        OneBlock = 0,
        TwoHorizontalBlocks = 1,
        TwoVerticalBlocks = 2,
        FourBlocks = 3,
        ThreeHorizontalBlocks = 4
    }
}

//Copyright 2019 Volodymyr Podshyvalov
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfHandler.UI.Controls
{
    /// <summary>
    /// Interaction logic for SelectableGrid.xaml
    /// </summary>
    public partial class SelectableGrid : UserControl
    {
        /// <summary>
        /// Class that contains information about active border element.
        /// </summary>
        public class ActiveBorder
        {
            /// <summary>
            /// Define direction of the border.
            /// </summary>
            public enum Direction
            {
                /// <summary>
                /// Cells snaped by horizontal order.
                /// </summary>
                Horizontal,
                /// <summary>
                /// Cells snaped by vertical order.
                /// </summary>
                Vertical
            }

            /// <summary>
            /// X coords of element in grid.
            /// </summary>
            public int x;

            /// <summary>
            /// Y coords of element in grid.
            /// </summary>
            public int y;

            /// <summary>
            /// Size of unselected element.
            /// </summary>
            public double FromSize { get; set; } = 5;

            /// <summary>
            /// Size of selected element.
            /// </summary>
            public float ToSize { get; set; } = 10;

            /// <summary>
            /// Z order of sorting.
            /// </summary>
            public int ZOrder { get; set; } = 1;

            /// <summary>
            /// Direction of the border.
            /// </summary>
            public Direction direction;

            /// <summary>
            /// Grid that contain's that Active border.
            /// </summary>
            public Grid Parent { get; set; }

            /// <summary>
            /// Delegate that contains methods that will called when border will be clicled.
            /// </summary>
            public MouseButtonEventHandler OnClickHandler { get; set; }

            /// <summary>
            /// Background of the Active border.
            /// </summary>
            public Brush Background
            {
                get
                {
                    return _Background;
                }
                set
                {
                    _Background = value;

                    // Update UI if already instiniated..
                    if (_UI != null)
                    {
                        _UI.Background = _Background;
                    }
                }
            }

            /// <summary>
            /// Bifer that contains background value.
            /// </summary>
            protected Brush _Background = Brushes.Black;

            /// <summary>
            /// UI control binded to that active border.
            /// </summary>
            public Canvas UI
            {
                get
                {
                    // Instiniate if null.
                    if (_UI == null)
                    {
                        // Instiniate UI element.
                        _UI = new Canvas
                        {
                            Background = Background, // Set default color.
                            Opacity = 0 // Disable visibility.
                        };
                        //_UI.SetValue(NameProperty, "AB_UI_" + x + "_" + y + "_" + direction);

                        // Bufers that will hold created storyboards.
                        System.Windows.Media.Animation.Storyboard sb = null;
                        System.Windows.Media.Animation.Storyboard sbRevers = null;
                        bool isCompleted = false;

                        #region LBM click
                        // Subscribe on mouse click.
                        UI.MouseLeftButtonDown += delegate (object sender, MouseButtonEventArgs e)
                        {
                            // Inform subscribers.
                            OnClickHandler?.Invoke(sender, e);
                        };
                        #endregion

                        #region Mouse enter
                        // React on mouse focusing
                        UI.MouseEnter += delegate (object sender, MouseEventArgs e)
                        {
                            // Set logs to debug label.
                            if (DebugLabel != null)
                            {
                                DebugLabel.Content = "Selected: " + direction + " " + x + "," + y;
                            }

                            // Drop previous completed status.
                            isCompleted = false;

                            // Enable visibility.
                            WpfHandler.UI.Animations.FloatAnimation.StartStoryboard(
                                Parent,
                                _UI,
                                new PropertyPath(Control.OpacityProperty),
                                new TimeSpan(0, 0, 0, 0, 200),
                                System.Windows.Media.Animation.FillBehavior.HoldEnd,
                                0, 1, delegate (System.Windows.Media.Animation.Storyboard sbb)
                                {
                                    sbb.Completed += Sbb_Completed;
                                });

                            void Sbb_Completed(object sender2, EventArgs e2)
                            {
                                // Mark animation as completed.
                                isCompleted = true;
                            }

                            if (FromSize != ToSize)
                            {
                                sbRevers?.Stop();
                                sb?.Stop();

                                // Enable visibility.
                                sb = WpfHandler.UI.Animations.GridLengthAnimation.StartStoryboard(
                                    Parent,
                                    direction == Direction.Horizontal ?
                                    (DependencyObject)Parent.RowDefinitions[y] : (DependencyObject)Parent.ColumnDefinitions[x],
                                    direction == Direction.Horizontal ?
                                    new PropertyPath(RowDefinition.HeightProperty) : new PropertyPath(ColumnDefinition.WidthProperty),
                                    new TimeSpan(0, 0, 0, 0, 200),
                                    System.Windows.Media.Animation.FillBehavior.HoldEnd,
                                    new GridLength(FromSize), new GridLength(ToSize),
                                    null);

                                var animation = (WpfHandler.UI.Animations.GridLengthAnimation)sb.Children[0];
                                animation.ReverseValue = FromSize;
                                animation.AutoReverse = false;
                            }
                        };
                        #endregion

                        #region Mpuse leave
                        // Delegate that will be called when mouse leave active border.
                        UI.MouseLeave += delegate (object sender, MouseEventArgs e)
                        {
                            if (DebugLabel != null)
                            {
                                DebugLabel.Content = "Leaved: " + direction + " " + x + "," + y;
                            }

                            // Disable visibility.
                            WpfHandler.UI.Animations.FloatAnimation.StartStoryboard(
                                Parent,
                                _UI,
                                new PropertyPath(Control.OpacityProperty),
                                new TimeSpan(0, 0, 0, 0, 200),
                                System.Windows.Media.Animation.FillBehavior.HoldEnd,
                                1, 0, null);

                            if (FromSize != ToSize)
                            {
                                //sb?.Stop();

                                // Revers if not continue.
                                if (!isCompleted)
                                {
                                    sb.Pause();
                                    ((WpfHandler.UI.Animations.GridLengthAnimation)sb.Children[0]).AutoReverse = true;
                                    sb.Begin();
                                }
                                // Start new animation if previous is over.
                                else
                                {
                                    sbRevers?.Stop();
                                    sb?.Stop();

                                    sbRevers = WpfHandler.UI.Animations.GridLengthAnimation.StartStoryboard(
                                        Parent,
                                        direction == Direction.Horizontal ?
                                        (DependencyObject)Parent.RowDefinitions[y] : (DependencyObject)Parent.ColumnDefinitions[x],
                                        direction == Direction.Horizontal ?
                                        new PropertyPath(RowDefinition.HeightProperty) : new PropertyPath(ColumnDefinition.WidthProperty),
                                        new TimeSpan(0, 0, 0, 0, 200),
                                        System.Windows.Media.Animation.FillBehavior.HoldEnd,
                                        new GridLength(ToSize), new GridLength(FromSize),
                                        null);
                                }
                            }
                        };
                        #endregion
                    }

                    return _UI;
                }
            }

            /// <summary>
            /// Bufer that contains instiniated UI element.
            /// </summary>
            protected Canvas _UI;

            /// <summary>
            /// Label that will be used to debug logs.
            /// </summary>
            public Label DebugLabel { get; set; }


            /// <summary>
            /// Updating layout settings.
            /// </summary>
            public void UpdateLayout()
            {
                // Validate
                if (Parent == null)
                {
                    MessageBox.Show("Error: Parent Grid for border element nto defined.");
                    return;
                }

                // Set as child.
                Parent.Children.Add(UI);

                // Set grid layout.
                Panel.SetZIndex(UI, ZOrder);
                Grid.SetColumn(UI, x);
                Grid.SetRow(UI, y);
            }
        }

        #region Dependency properties
        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty ColumnsCountProperty = DependencyProperty.Register(
          "ColumnsCount", typeof(int), typeof(SelectableGrid), new PropertyMetadata(1));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty RowsCountProperty = DependencyProperty.Register(
          "RowsCount", typeof(int), typeof(SelectableGrid), new PropertyMetadata(1));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty SelectebleRowsProperty = DependencyProperty.Register(
          "SelectebleRows", typeof(bool), typeof(SelectableGrid), new PropertyMetadata(true));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty SelectebleColumnsProperty = DependencyProperty.Register(
          "SelectebleColumns", typeof(bool), typeof(SelectableGrid), new PropertyMetadata(true));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty SelectebleBlocksProperty = DependencyProperty.Register(
          "SelectebleBlocks", typeof(bool), typeof(SelectableGrid), new PropertyMetadata(true));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty CellSizeProperty = DependencyProperty.Register(
            "CellSize", typeof(float), typeof(SelectableGrid), new PropertyMetadata(25.0f));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty CellsSpaceProperty = DependencyProperty.Register(
            "CellsSpace", typeof(float), typeof(SelectableGrid), new PropertyMetadata(5.0f));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty SelectedBorderBackgroundProperty = DependencyProperty.Register(
            "SelectedBorderBackground", typeof(Brush), typeof(SelectableGrid), new PropertyMetadata(Brushes.WhiteSmoke));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty SelectedGridBackgroundProperty = DependencyProperty.Register(
            "SelectedGridBackground", typeof(Brush), typeof(SelectableGrid), new PropertyMetadata(Brushes.WhiteSmoke));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty HorizontalSymmetryProperty = DependencyProperty.Register(
            "HorizontalSymmetry", typeof(bool), typeof(SelectableGrid), new PropertyMetadata(false));

        /// <summary>
        /// Bridging XAML declaring and the member.
        /// </summary>
        public static readonly DependencyProperty HorizontalSymmetrySpaceProperty = DependencyProperty.Register(
            "HorizontalSymmetrySpace", typeof(float), typeof(SelectableGrid), new PropertyMetadata(0.0f));
        #endregion


        /// <summary>
        /// Current active grid.
        /// </summary>
        public SelectableGrid Active { get; set; }

        /// <summary>
        /// Event that will be called when the grid will be activated.
        /// </summary>
        public event Action<SelectableGrid> GridSelected;

        /// <summary>
        /// Event that will be called when some border will selected.
        /// </summary>
        public event Action<SelectableGrid, ActiveBorder> BorderSelected;


        /// <summary>
        /// How many collumns will contain grid.
        /// </summary>
        public int ColumnsCount
        {
            get { return (int)this.GetValue(ColumnsCountProperty); }
            set 
            { 
                this.SetValue(ColumnsCountProperty, value);
                // Update GUI
                UpdateGrid();
            }
        }

        /// <summary>
        /// How many rows will contain grid.
        /// </summary>
        public int RowsCount
        {
            get { return (int)this.GetValue(RowsCountProperty); }
            set 
            {
                this.SetValue(RowsCountProperty, value);
                // Update GUI
                UpdateGrid();
            }
        }

        /// <summary>
        /// Is you able to select row's zone between elements?
        /// </summary>
        public bool SelectebleRows
        {
            get { return (bool)this.GetValue(SelectebleRowsProperty); }
            set 
            {
                this.SetValue(SelectebleRowsProperty, value);
                // Update GUI
                UpdateGrid();
            }
        }

        /// <summary>
        /// Is you able to select column's zone between elements?
        /// </summary>
        public bool SelectebleColumns
        {
            get { return (bool)this.GetValue(SelectebleColumnsProperty); }
            set 
            { 
                this.SetValue(SelectebleColumnsProperty, value);
                // Update GUI
                UpdateGrid();
            }
        }

        /// <summary>
        /// Is you able to select whole block by selecting corner?
        /// </summary>
        public bool SelectebleBlocks
        {
            get { return (bool)this.GetValue(SelectebleBlocksProperty); }
            set
            {
                this.SetValue(SelectebleBlocksProperty, value);

                // Update GUI
                UpdateGrid();
            }
        }

        /// <summary>
        /// Color of the border elements when that has selected state.
        /// </summary>
        public Brush SelectedBorderBackground
        {
            get { return (Brush)this.GetValue(SelectedBorderBackgroundProperty); }
            set { this.SetValue(SelectedBorderBackgroundProperty, value); }
        }

        /// <summary>
        /// Color of the entire grid when that has selected state.
        /// </summary>
        public Brush SelectedGridBackground
        {
            get { return (Brush)this.GetValue(SelectedGridBackgroundProperty); }
            set { this.SetValue(SelectedGridBackgroundProperty, value); }
        }

        /// <summary>
        /// Size of the grid's cell.
        /// </summary>
        public float CellSize
        {
            get { return (float)this.GetValue(CellSizeProperty); }
            set
            {
                this.SetValue(CellSizeProperty, value);

                // Update GUI
                UpdateGrid();
            }
        }

        /// <summary>
        /// Space between grid's cells.
        /// </summary>
        public float CellsSpace
        {
            get { return (float)this.GetValue(CellsSpaceProperty); }
            set
            {
                this.SetValue(CellsSpaceProperty, value);

                // Update GUI
                UpdateGrid();
            }
        }

        /// <summary>
        /// Is you able to select whole block by selecting corner?
        /// </summary>
        public bool HorizontalSymmetry
        {
            get { return (bool)this.GetValue(HorizontalSymmetryProperty); }
            set { this.SetValue(HorizontalSymmetryProperty, value); }
        }

        /// <summary>
        /// Space betwwen sivided symmetric blocks.
        /// </summary>
        public float HorizontalSymmetrySpace
        {
            get { return (float)this.GetValue(HorizontalSymmetrySpaceProperty); }
            set { this.SetValue(HorizontalSymmetrySpaceProperty, value); }
        }

        /// <summary>
        /// Handler that will be called suring grid filling.
        /// </summary>
        public UISpawnHandler OnElementIstiniation { get; set; }

        /// <summary>
        /// Delegate that will be called duiring UI spawn.
        /// </summary>
        /// <param name="x">X coordinate of an element in the grid.</param>
        /// <param name="y">Y coordinate of an element in the grid.</param>
        /// <returns>Instiniated element. Will be applied to the grid layout automaticly.</returns>
        public delegate UIElement UISpawnHandler(int x, int y);


        /// <summary>
        /// Default constructor.
        /// </summary>
        public SelectableGrid()
        {
            InitializeComponent();
            DataContext = this;
        }
        

        #region API
        /// <summary>
        /// Updating grid of elements relative to that block.
        /// </summary>
        public void UpdateGrid()
        {
            #region Init & validation
            var width = ColumnsCount;
            var height = RowsCount;
            #endregion

            #region Clear current grid.
            grid.ColumnDefinitions.Clear();
            grid.RowDefinitions.Clear();
            grid.Children.Clear();
            #endregion

            #region Instiniating new grid.
            // Define columns.
            for (int i = 0; i < width; i++)
            {
                // Adding space between columns.
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(CellsSpace) });
                // Adding rows.
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(CellSize) });
            }
            // Adding final space column.
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(CellsSpace) });

            // Add divinding dolumn between symetryc blocks.
            if (HorizontalSymmetry)
            {
                grid.ColumnDefinitions.Insert(grid.ColumnDefinitions.Count / 2,
                    new ColumnDefinition() { Width = new GridLength(HorizontalSymmetrySpace) });
            }

            // Define rows.
            for (int i = 0; i < height; i++)
            {
                // Adding space between rows.
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(CellsSpace) });
                // Adding rows.
                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(CellSize) });
            }
            // Adding final space row.
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(CellsSpace) });
            #endregion

            #region Fill grid by content
            if (HorizontalSymmetry)
            {
                // Fill left sub block.
                FillGridRect(0, 0, width / 2, height);

                // Skip 1 separaate cell and continue block.
                FillGridRect(width / 2 + 1, 0, width / 2, height);
            }
            else
            {
                FillGridRect(0, 0, width, height);
            }
            #endregion;
        }

        /// <summary>
        /// Filling rect of grid with a cells.
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected void FillGridRect(int startX, int startY, int width, int height)
        {
            // Drop if init process not possible.
            if (OnElementIstiniation == null)
            {
                MessageBox.Show("Error: OnElementIstiniation event has no subscribers." +
                    " Initialization not possible.");
                return;
            }

            #region Apply block active background
            if (SelectebleBlocks)
            {
                var activeBackplate = new ActiveBorder()
                {
                    Parent = grid,
                    //DebugLabel = debugLog,
                    direction = ActiveBorder.Direction.Horizontal,
                    x = startX,
                    y = startY,
                    Background = SelectedGridBackground,
                    FromSize = 5,
                    ToSize = 5,
                    ZOrder = 0,
                    OnClickHandler = delegate (object sender, MouseButtonEventArgs e)
                    {
                        // Update data.
                        Active = this;

                        // Inform subscribers.
                        GridSelected?.Invoke(this);
                    }
                };

                activeBackplate.UpdateLayout();
                Grid.SetRowSpan(activeBackplate.UI, height * 2);
                Grid.SetColumnSpan(activeBackplate.UI, width * 2);
                //activeBackplate.UI.Margin = new Thickness(-5, -5, -10, -10);
                activeBackplate.UI.Margin = new Thickness(0, 0, -5, -5);

                // Add grid that will block glitching selecting of the block during fast mouse moves.
                Grid overlay = new Grid()
                {
                    Background = Brushes.Transparent
                };
                grid.Children.Add(overlay);
                Grid.SetColumn(overlay, startX + 1);
                Grid.SetRow(overlay, startY + 1);
                Grid.SetRowSpan(overlay, height * 2);
                Grid.SetColumnSpan(overlay, width * 2);
            }
            #endregion

            #region Set active borders to columns
            if (SelectebleColumns)
            {
                for (int i = startX; i < (startX + width) * 2 + 2; i += 2)
                {
                    var abm = new ActiveBorder()
                    {
                        Parent = grid,
                        direction = ActiveBorder.Direction.Vertical,
                        x = i,
                        y = startY + 1,
                        Background = SelectedBorderBackground
                    };

                    // Inserting new column into the grid.
                    abm.OnClickHandler = delegate (object sender, MouseButtonEventArgs e)
                    {
                        // Log
                        //MessageBox.Show("Column " + abm.x + " inserting.");
                        
                        // Inform subscribers.
                        BorderSelected?.Invoke(this, abm);
                    };

                    abm.UpdateLayout();
                    Grid.SetRowSpan(abm.UI, height * 2 - 1);
                }
            }
            #endregion

            #region Set active borders to rows
            if (SelectebleRows)
            { 
                for (int i = startY; i < (startY + height) * 2 + 2; i += 2)
                {
                    var abm = new ActiveBorder()
                    {
                        Parent = grid,
                        direction = ActiveBorder.Direction.Horizontal,
                        x = startX + 1,
                        y = i,
                        Background = SelectedBorderBackground
                    };

                    // Instrting new row into the grid.
                    abm.OnClickHandler = delegate (object sender, MouseButtonEventArgs e)
                    {
                        // Log
                        //MessageBox.Show("Row " + abm.y + " inserting.");

                        // Inform subscribers.
                        BorderSelected?.Invoke(this, abm);
                    };

                    abm.UpdateLayout();
                    Grid.SetColumnSpan(abm.UI, width * 2 - 1);
                }
            }
            #endregion

            #region Insiniate elements to grid's cells.
            for (int y = startY; y < height; y++)
                for (int x = startX; x < width; x++)
                {
                    // Create seat UI.
                    var seat = OnElementIstiniation.Invoke(x, y);

                    // Append it to the gris.
                    grid.Children.Add(seat);
                    Grid.SetColumn(seat, x * 2 + 1);
                    Grid.SetRow(seat, y * 2 + 1);
                }
            #endregion
        }
        #endregion
    }
}

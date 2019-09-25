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

namespace ColorPicker
{
    /// <summary>
    /// Interaction logic for ColorPicker2.xaml
    /// </summary>
    public partial class ColorPicker2 : UserControl
    {
        public ColorPicker2()
        {
            InitializeComponent();
            CreateColorWheelBackground( 1024, 1024 );
        }

        //-------------------------------------------
        void CreateColorWheelBackground( int resolutionx, int resolutionY )
        {
            WriteableBitmap bitmap = new WriteableBitmap( resolutionx, resolutionY, 96, 96, PixelFormats.Bgra32, null );
            uint[] pixels = new uint[ resolutionx * resolutionY ];

            byte r,g,b,a;
            double radius = resolutionx / 2.0;

            for ( int x = 0 ; x < resolutionx ; ++x )
            {
                for ( int y = 0 ; y < resolutionY ; ++y )
                {
                    int i = resolutionx * y + x;

                    double relX = ( double )x - ( double )resolutionx / 2.0;
                    double relY = ( double )y - ( double )resolutionY / 2.0;
                    double distance;
                    ColorFromPosition( relX, relY, resolutionx / 2.0, out distance, out r, out g, out b );
                    
                    // if distance is bigger than radius, do not draw
                    a = ( byte )( ( radius <= distance ) ? 0 : 255 );

                    pixels[ i ] = ( uint )( ( a << 24 ) + ( r << 16 ) + ( g << 8 ) + b );
                }
            }
            
            // apply pixels to bitmap
            bitmap.WritePixels( new Int32Rect( 0, 0, resolutionx, resolutionY ), pixels, resolutionx * 4, 0 );
            ColorWheel.Fill = new ImageBrush( bitmap);
        }

        //-------------------------------------------
        Color ColorFromMousePosition( Point mousePos, double radius )
        {
            // Position relative to center of canvas
            double xRel = mousePos.X - radius;
            double yRel = mousePos.Y - radius;

            byte r, g, b;
            double distance;
            ColorFromPosition( xRel, yRel, radius, out distance, out r, out g, out b );
            return Color.FromRgb( r, g, b );
        }

        //-------------------------------------------
        void ColorFromPosition( double x, double y, double radius, out double distance, out byte r, out byte g, out byte b )
        {
            // Hue is angle in deg, 0-360
            double angleRadians = Math.Atan2( x, y );
            double hue = angleRadians * ( 180 / Math.PI );
            if ( hue < 0 )
                hue = 360 + hue;

            // Saturation is distance from center
            distance = Math.Sqrt( x * x + y * y ) ;
            double saturation = Math.Min( distance / radius, 1.0 );
            VCColor.HsvToRgb( hue, saturation, 1.0, out r, out g, out b );
        }


        private void Ellipse_PreviewMouseDown( object sender, MouseEventArgs e )
        {
            double radius = ( ColorWheel.ActualWidth / 2 );

            Color c = ColorFromMousePosition( e.GetPosition( ColorWheel ), radius );

            this.Resources[ "ColorPreview" ] = (Color)c;

            this.Resources[ "HueSaturationColor" ] = c;
            

            //RGBInfo = string.Format( "R={0}, G={1}, B={2}", c.R, c.G, c.B );
        }
    }
}

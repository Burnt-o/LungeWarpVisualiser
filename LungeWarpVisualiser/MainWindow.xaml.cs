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
using System.Numerics;
using System.Diagnostics;
using System.Windows.Controls.Primitives;


namespace LungeWarpVisualiser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();


            Vector3 inputpos = new Vector3(0, 800, 200);
            Vector3 inputrot = new Vector3(0, 0, 0);


            Vector3 firstoutput = Calc_out(inputpos, inputrot);

            Console.WriteLine("Input position" + inputpos);
            Console.WriteLine("Input rotation" + inputrot);
            Console.WriteLine("Output position: "+ firstoutput);
            Console.WriteLine("Reversing output position to get input position " + Calc_out_reverse(firstoutput, inputrot.Z));






            //bunch of code for drag and dropping origin image stolen from https://stackoverflow.com/questions/17035225/c-sharp-wpf-drag-an-image
            var uriSource = new Uri(@"dragme.png", UriKind.Relative);
            var bitmap = new BitmapImage(uriSource);
            var image = new Image { Source = bitmap };
            var image2 = new Image { Source = bitmap };
            Canvas.SetLeft(image, 0);
            Canvas.SetTop(image, 0);
            Origin.Children.Add(image);
            Output.Children.Add(image2);
        }


        private Image draggedImage;
        private Image storeddraggedImage;
        private Image outputImage;
    
        private Point mousePosition;

        private void OriginMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var image = e.Source as Image;
            var output = Output.Children[0] as Image;
            outputImage = output;
            storeddraggedImage = image;

            if (image != null && Origin.CaptureMouse())
            {
                mousePosition = e.GetPosition(Origin);
                draggedImage = image;
                Panel.SetZIndex(draggedImage, 1); // in case of multiple images
            }
        }

        private void OriginMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (draggedImage != null)
            {
                Origin.ReleaseMouseCapture();
                Panel.SetZIndex(draggedImage, 0);
                draggedImage = null;
            }
        }

        private void RecalculateOutput()
        {
            //this shuold get called on: dragging origin, adjusting sliders
            //calculate phobic stuff
            //need to figure out how to align map with actual coords but that comes later
            Vector2 position = new Vector2((float)Canvas.GetLeft(storeddraggedImage), (float)Canvas.GetTop(storeddraggedImage)) ;

        
 

            Vector2 transformedoffset = ConvertCoords((float)position.X, (float)position.Y);
            Vector3 inputpos = new Vector3(transformedoffset.X, transformedoffset.Y, (float)0);
            Vector3 inputrot = new Vector3((float)Roll.Value, (float)Pitch.Value, (float)Yaw.Value);

            //Console.WriteLine("rol: " + inputrot.X);
           // Console.WriteLine("pit: " + inputrot.Y);
            //Console.WriteLine("yaw: " + inputrot.Z);

            Console.WriteLine("ix: " + inputpos.X);
            Console.WriteLine("iy: " + inputpos.Y);

            Vector3 outputpos = Calc_out(inputpos, inputrot);
            Console.WriteLine("ox: " + outputpos.X);
            Console.WriteLine("oy: " + outputpos.Y);

            Canvas.SetLeft(outputImage, outputpos.X + 250);
            Canvas.SetTop(outputImage, outputpos.Y + 250);
        }


        private void OriginMouseMove(object sender, MouseEventArgs e)
        {
            if (draggedImage != null)
            {
                var position = e.GetPosition(Origin);
                var offset = position - mousePosition;

                mousePosition = position;
                Canvas.SetLeft(draggedImage, Canvas.GetLeft(draggedImage) + offset.X);
                Canvas.SetTop(draggedImage, Canvas.GetTop(draggedImage) + offset.Y);



                RecalculateOutput();
               

                //test
                //Canvas.SetLeft(outputImage, 100);
               // Canvas.SetTop(outputImage, 100);

            }
        }


        private void slider1_DragDelta(object sender, DragDeltaEventArgs e)
        {

            RecalculateOutput();
        }

        private void slider2_DragDelta(object sender, DragDeltaEventArgs e)
        {
            RecalculateOutput();
        }

        private void slider3_DragDelta(object sender, DragDeltaEventArgs e)
        {
            RecalculateOutput();
        }


        public Vector2 ConvertCoords(float x, float y)
        { 
        
        //grid is 500x500. we want 250,250 to be 0,0
        Vector2 output = new Vector2(x - 250, y - 250);
            return output;
        
        }

        public double Radians(double degrees)
        {
            return degrees * Math.PI / 180;
        }



        //take in input position and input rotation (in degrees) (where you're lunge warping from, and return output position (where you will get lunge warped to)
        public Vector3 Calc_out(Vector3 inputpos, Vector3 inputrot)
        {
            inputrot.X = (float)Radians(inputrot.X);
            inputrot.Y = (float)Radians(inputrot.Y);
            inputrot.Z = (float)Radians(inputrot.Z);

            //math straight up copy pasted from phobics calculator  https://docs.google.com/spreadsheets/d/1Tb4HeOHJxyXm04Chrqn9oYAtX2-jjoTvPJFPiaIhALM/edit#gid=1184370500
            double outputx = inputpos.X + inputpos.X * Math.Cos(inputrot.Z) * Math.Cos(inputrot.Y) - inputpos.Y * (Math.Cos(inputrot.Z) * Math.Sin(inputrot.Y) * Math.Sin(inputrot.X) + Math.Sin(inputrot.Z) * Math.Cos(inputrot.X)) + inputpos.Z * (Math.Sin(inputrot.Z) * Math.Sin(inputrot.X) - Math.Cos(inputrot.Z) * Math.Sin(inputrot.Y) * Math.Cos(inputrot.X));

            double outputy = inputpos.Y + inputpos.X * (Math.Sin(inputrot.Z) * Math.Cos(inputrot.Y)) + inputpos.Y * (Math.Cos(inputrot.Z) * Math.Cos(inputrot.X) - Math.Sin(inputrot.Z) * Math.Sin(inputrot.Y) * Math.Sin(inputrot.X)) - inputpos.Z * (Math.Sin(inputrot.Z) * Math.Sin(inputrot.Y) * Math.Cos(inputrot.X) + Math.Cos(inputrot.Z) * Math.Sin(inputrot.X));

            double outputz = inputpos.Z + inputpos.X * (Math.Sin(inputrot.Y)) + inputpos.Y * (Math.Cos(inputrot.Y) * Math.Sin(inputrot.X)) + inputpos.Z * (Math.Cos(inputrot.Y) * Math.Cos(inputrot.X));

            Vector3 Killme = new Vector3((float)outputx, (float)outputy, (float)outputz);
            return Killme;
        }



        //EXTREMELY wip - trying to basically do the lunge warp calculation in reverse. currently assuming 0 pitch and 0 roll
        //this is basically a simultaneous equation 
        //and is extremely unfinished ugh
        public Vector3 Calc_out_reverse(Vector3 input, float yaw)
        {
            yaw = (float)Radians(yaw);
            double sinyaw = Math.Sin(yaw);
            double cosyaw = Math.Cos(yaw);

            //none of this shit is working yet. big issues when one of the holdvals is 0
            double holdval1 = (input.X * sinyaw) + (input.X * sinyaw * cosyaw) + input.Y;
            double holdval2 = sinyaw * ((cosyaw * cosyaw) + 2 + (2 * cosyaw));


            Console.WriteLine("hv1 " + holdval1);
            Console.WriteLine("hv2 " + holdval2);

            double outputx = holdval1 / holdval2;


            double outputy = (input.Y - (outputx * sinyaw)) / (1 + cosyaw);
            //y = (b  - X * sin(d)) / (1 + cos(d))

            double outputz = (input.Z - outputx) / 2;

            //GOD WHY IS MATH HARD


            Vector3 Killme = new Vector3((float)outputx, (float)outputy, (float)outputz);
            return Killme;
        }


    }
}

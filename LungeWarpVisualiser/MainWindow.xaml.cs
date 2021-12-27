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

namespace LungeWarpVisualiser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Console.WriteLine("1");
            InitializeComponent();
            Console.WriteLine("2");
            Vector3 inputpos = new Vector3(0, 800, 200);
            Vector3 inputrot = new Vector3(0, 0, 0);


            Vector3 firstoutput = Calc_out(inputpos, inputrot);
            Console.WriteLine(firstoutput);
            Console.WriteLine("reversed: " + Calc_out_reverse(firstoutput, 0));
        }

        public double Radians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public Vector3 Calc_out(Vector3 inputpos, Vector3 inputrot)
        {
            inputrot.X = (float)Radians(inputrot.X);
            inputrot.Y = (float)Radians(inputrot.Y);
            inputrot.Z = (float)Radians(inputrot.Z);

            //math straight up copy pasted from phobics calculator  https://docs.google.com/spreadsheets/d/1EcPsPU6DdgsKG--NVndPSWeZWwM04pCjSro9t0RxF3U/edit#gid=1184370500
            double outputx = inputpos.X + inputpos.X * Math.Cos(inputrot.Z) * Math.Cos(inputrot.Y) - inputpos.Y * (Math.Cos(inputrot.Z) * Math.Sin(inputrot.Y) * Math.Sin(inputrot.X) + Math.Sin(inputrot.Z) * Math.Cos(inputrot.X)) + inputpos.Z * (Math.Sin(inputrot.Z) * Math.Sin(inputrot.X) - Math.Cos(inputrot.Z) * Math.Sin(inputrot.Y) * Math.Cos(inputrot.X));

            double outputy = inputpos.Y + inputpos.X * (Math.Sin(inputrot.Z) * Math.Cos(inputrot.Y)) + inputpos.Y * (Math.Cos(inputrot.Z) * Math.Cos(inputrot.X) - Math.Sin(inputrot.Z) * Math.Sin(inputrot.Y) * Math.Sin(inputrot.X)) - inputpos.Z * (Math.Sin(inputrot.Z) * Math.Sin(inputrot.Y) * Math.Cos(inputrot.X) + Math.Cos(inputrot.Z) * Math.Sin(inputrot.X));

            double outputz = inputpos.Z + inputpos.X * (Math.Sin(inputrot.Y)) + inputpos.Y * (Math.Cos(inputrot.Y) * Math.Sin(inputrot.X)) + inputpos.Z * (Math.Cos(inputrot.Y) * Math.Cos(inputrot.X));

            Vector3 Killme = new Vector3((float)outputx, (float)outputy, (float)outputz);
            return Killme;
        }

        public Vector3 Calc_out_reverse(Vector3 input, float yaw)
        {
            yaw = (float)Radians(yaw);
            double sinyaw = Math.Sin(yaw);
            double cosyaw = Math.Cos(yaw);


            double holdval1 = (input.X * sinyaw) + (input.X * sinyaw * cosyaw) + input.Y;
            double holdval2 = sinyaw * ((cosyaw * cosyaw) + 2 + (2 * cosyaw));


            Console.WriteLine("hv1 " + holdval1);
            Console.WriteLine("hv2 " + holdval2);

            double outputx = holdval1 / holdval2;


            double outputy = input.Y - (output)
            //y = (b  - X * sin(d)) / (1 + cos(d))

            double outputx = (input.X + outputy * sinyaw) / (1 + cosyaw);

            double outputz = (input.Z - outputx) / 2;

    


            Vector3 Killme = new Vector3((float)outputx, (float)outputy, (float)outputz);
            return Killme;
        }


    }
}

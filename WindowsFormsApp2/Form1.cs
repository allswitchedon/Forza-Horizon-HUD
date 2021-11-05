using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Axes;
using System.Threading;

namespace WindowsFormsApp2
{
    public partial class Form1 : Form
    {
        UdpClient Client = new UdpClient(8000);
        string data = "";
        // Telemetry DATA
        public int IsRaceOn, MaxRPM, minRPM, RPM;
        // public int TimestampMS;
        public double AccelerationX, AccelerationY, AccelerationZ;
        //public double VelocityX, VelocityY, VelocityZ;
        //public double AngularVelocityX, AngularVelocityY, AngularVelocityZ;
        //public double Yaw, Pitch, Roll;
        public double NormalizedSuspensionTravelFrontLeft, NormalizedSuspensionTravelFrontRight, NormalizedSuspensionTravelRearLeft, NormalizedSuspensionTravelRearRight;
        public double TireSlipRatioFrontLeft, TireSlipRatioFrontRight, TireSlipRatioRearLeft, TireSlipRatioRearRight;
        //public double WheelRotationSpeedFrontLeft, WheelRotationSpeedFrontRight, WheelRotationSpeedRearLeft, WheelRotationSpeedRearRight;
        //public int WheelOnRumbleStripFrontLeft, WheelOnRumbleStripFrontRight, WheelOnRumbleStripRearLeft, WheelOnRumbleStripRearRight;
        //public double WheelInPuddleDepthFrontLeft, WheelInPuddleDepthFrontRight, WheelInPuddleDepthRearLeft, WheelInPuddleDepthRearRight;
        //public double SurfaceRumbleFrontLeft, SurfaceRumbleFrontRight, SurfaceRumbleRearLeft, SurfaceRumbleRearRight;
        //public double TireSlipAngleFrontLeft, TireSlipAngleFrontRight, TireSlipAngleRearLeft, TireSlipAngleRearRight;
        //public double TireCombinedSlipFrontLeft, TireCombinedSlipFrontRight, TireCombinedSlipRearLeft, TireCombinedSlipRearRight;
        //public double SuspensionTravelMetersFrontLeft, SuspensionTravelMetersFrontRight, SuspensionTravelMetersRearLeft, SuspensionTravelMetersRearRight;
        public int CarOrdinal, CarClass, CarPerformanceIndex, DrivetrainType, NumCylinders, CarType;
        //public double PositionX, PositionY, PositionZ;
        public int Speed, Power, Torque;
        public double TireTempFrontLeft, TireTempFrontRight, TireTempRearLeft, TireTempRearRight;
        public double Boost, Fuel, DistanceTraveled, BestLap, LastLap, CurrentLap, CurrentRaceTime;


        //public int LapNumber, RacePosition;
        public int Accel, Brake, Clutch, HandBrake, Gear, Steer;
        //public int NormalizedDrivingLine, NormalizedAIBrakeDifference;
        //END of Telemetry DATA
        int run100, run200, run300, run400;
        double atime0, atime100, atime200, atime300, atime400;
        int stoprun, stoprun100, stoprun200, stoprun300, stoprun400;
        double dtime0, dtime100, dtime200, dtime300, dtime400;
        double dragrun_0_100, dragrun_0_200, dragrun_0_300, dragrun_0_400;
        double stoprun_400_0, stoprun_300_0, stoprun_200_0, stoprun_100_0;
        string readytorun = "";
        string maxpowerbeetwen = "";
        byte[] FH4UDPBYTE;
        public int maxSpeed;
        public int chartdraw;
        public int rmRPM, maxPower, maxTorque;
        public int mpaRPM, mpbRPM, mpRPM, mtaRPM, mtbRPM;
        public int ymax;

        public Form1()
        {
            InitializeComponent();
            //CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        void recv(IAsyncResult res)
        {
            IPEndPoint RemoteIP = new IPEndPoint(IPAddress.Any, 60240);
            FH4UDPBYTE = Client.EndReceive(res, ref RemoteIP);
            //Convert Byte to Values                
            IsRaceOn = BitConverter.ToInt32(FH4UDPBYTE, 0); // = 1 when race is on. = 0 when in menus/race stopped …
            if (IsRaceOn == 1)
            {
                ConvertData();
                CalculateData();
            }
            else
            {
                if (checkBox2.CheckState == CheckState.Unchecked)
                {
                    rmRPM = 0;
                    MaxRPM = Convert.ToInt32(BitConverter.ToSingle(FH4UDPBYTE, 8));
                    minRPM = Convert.ToInt32(BitConverter.ToSingle(FH4UDPBYTE, 12));
                    maxPower = 0;
                    maxTorque = 0;
                    mpaRPM = 0;
                    mpbRPM = 0;
                    mtaRPM = 0;
                    mtbRPM = 0;
                    maxpowerbeetwen = "";
                }
                else {
                    run100 = run200 = run300 = run400 = 0;
                    atime0 = atime100 = atime200 = atime300 = atime400 = 0;
                    stoprun = stoprun100 = stoprun200 = stoprun300 = stoprun400 = 0;
                    dtime0 = dtime100 = dtime200 = dtime300 = dtime400 = 0;
                    dragrun_0_100 = dragrun_0_200 = dragrun_0_300 = dragrun_0_400 = 0;
                    stoprun_400_0 = stoprun_300_0 = stoprun_200_0 = stoprun_100_0 = 0;
                }

            }

            this.Invoke(new MethodInvoker(delegate
        {
            //update GUI
            if (IsRaceOn == 1)
            {
                aGauge2.Value = Speed;
                aGauge1.Value = RPM;
                label1.Text = "Acceleration: " + "\n0-100 Time: " + dragrun_0_100.ToString("0.000") + "\n0-200 Time: " + dragrun_0_200.ToString("0.000") + "\n0-300 Time: " + dragrun_0_300.ToString("0.000") + "\n0-400 Time: " + dragrun_0_400.ToString("0.000");
                label2.Text = "Decceletation: " + "\n100-0 Time: " + stoprun_100_0.ToString("0.000") + "\n200-0 Time: " + stoprun_200_0.ToString("0.000") + "\n300-0 Time: " + stoprun_300_0.ToString("0.000") + "\n400-0 Time: " + stoprun_400_0.ToString("0.000");
                label4.Text = readytorun;
                // Gear
                if (Gear > 0 && Gear < 11)
                    label3.Text = Gear.ToString("0");
                else;
                if (Gear == 0)
                    label3.Text = "R";
                else;
                if (Gear == 11)
                    label3.Text = "N";
                else;
                if (aGauge1.MaxValue != MaxRPM & Speed == 0)
                    aGauge1.MaxValue = MaxRPM;
                {
                    if (RPM < mtaRPM)
                    {
                        label3.BackColor = SystemColors.Control;
                    }
                    if (RPM >= mtaRPM && RPM < mpaRPM)
                    {
                        label3.BackColor = Color.Yellow;
                    }
                    if (RPM >= mpaRPM && RPM < rmRPM - 100)
                    {
                        label3.BackColor = Color.Green;
                    }
                    if (RPM >= mpbRPM && RPM < rmRPM - 100)
                    {
                        label3.BackColor = Color.Orange;
                    }
                    if (RPM >= rmRPM - 100)
                    {
                        label3.BackColor = Color.Red;
                    }
                }
                label6.Text = Boost.ToString("0.00") + " Bar";
                label8.Text = "Suspension:\n" + NormalizedSuspensionTravelFrontLeft.ToString("0.000") + "     " + NormalizedSuspensionTravelFrontRight.ToString("0.000") + "\n" + NormalizedSuspensionTravelRearLeft.ToString("0.000") + "     " + NormalizedSuspensionTravelRearRight.ToString("0.000");
                progressBar1.Value = Torque;
                progressBar2.Value = Power;
                label5.Text = maxPower.ToString() + "\n" + maxTorque.ToString() ;
                label9.Text = sCarClass(CarClass) + " " + CarPerformanceIndex.ToString() + "  " + WheelDrive(DrivetrainType) + "\nMaxSpeed: " + maxSpeed.ToString() + "\nMaximum RPM: " + rmRPM.ToString() + "\n" + maxpowerbeetwen;
                if (checkBox1.CheckState == CheckState.Checked)
                {
                    if (Torque > ymax)
                    {
                        ymax = Torque;
                        plotView1.OnModelChanged();
                    }
                    else
                    {
                        plotView1.Refresh();
                    }
                }
            }
            else
            {
                if (aGauge1.MaxValue != MaxRPM)
                {
                    aGauge1.MaxValue = MaxRPM;
                }
                aGauge2.Value = Speed;
                aGauge1.Value = RPM;
                progressBar1.Value = Torque;
                progressBar2.Value = Power;
                label3.Text = "N";
                label4.Text = "Back to the Game";
                ymax = 0;
            }


        }));
            Client.BeginReceive(new AsyncCallback(recv), null);
        }


        public async Task ChartDrawAsync()
        {
            PlotModel EngineData = new PlotModel { Title = "Engine Data" };
            ScatterSeries TorqueSeries = new ScatterSeries { MarkerType = MarkerType.Circle };
            ScatterSeries PowerSeries = new ScatterSeries { MarkerType = MarkerType.Circle };
            EngineData.Series.Add(TorqueSeries);
            EngineData.Series.Add(PowerSeries);
            plotView1.Model = EngineData;
            while (true)
            {
                if (IsRaceOn == 1)
                {
                    await Task.Run(async () =>
                    {
                        TorqueSeries.Points.Add(new ScatterPoint(RPM, Torque, 1));
                        PowerSeries.Points.Add(new ScatterPoint(RPM, Power, 1));
                    });
                    await Task.Delay(16);
                }
                else;
            }
            await Task.Delay(1);
        }


        public async void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Client.BeginReceive(new AsyncCallback(recv), null);
            }
            catch (Exception ex)
            {
                label3.Text += ex.Message.ToString();
            }
        }

        public void ConvertData()
        {
            //TimestampMS = Convert.ToInt32(BitConverter.ToUInt32(FH4UDPBYTE, 4)); //Can overflow to 0 eventually
            MaxRPM = Convert.ToInt32(BitConverter.ToSingle(FH4UDPBYTE, 8));
            minRPM = Convert.ToInt32(BitConverter.ToSingle(FH4UDPBYTE, 12));
            RPM = Convert.ToInt32(BitConverter.ToSingle(FH4UDPBYTE, 16));

            AccelerationX = BitConverter.ToSingle(FH4UDPBYTE, 20); //In the car's local space; X = right, Y = up, Z = forward
            AccelerationY = BitConverter.ToSingle(FH4UDPBYTE, 24);
            AccelerationZ = BitConverter.ToSingle(FH4UDPBYTE, 28);

            //VelocityX = BitConverter.ToSingle(FH4UDPBYTE, 32); //In the car's local space; X = right, Y = up, Z = forward
            //VelocityY = BitConverter.ToSingle(FH4UDPBYTE, 36);
            //VelocityZ = BitConverter.ToSingle(FH4UDPBYTE, 40);

            //AngularVelocityX = BitConverter.ToSingle(FH4UDPBYTE, 44); //In the car's local space; X = pitch, Y = yaw, Z = roll
            //AngularVelocityY = BitConverter.ToSingle(FH4UDPBYTE, 48);
            //AngularVelocityZ = BitConverter.ToSingle(FH4UDPBYTE, 52);

            //Yaw = BitConverter.ToSingle(FH4UDPBYTE, 56);
            //Pitch = BitConverter.ToSingle(FH4UDPBYTE, 60);
            //Roll = BitConverter.ToSingle(FH4UDPBYTE, 64);

            NormalizedSuspensionTravelFrontLeft = BitConverter.ToSingle(FH4UDPBYTE, 68); // Suspension travel normalized: 0.0f = max stretch; 1.0 = max compression
            NormalizedSuspensionTravelFrontRight = BitConverter.ToSingle(FH4UDPBYTE, 72);
            NormalizedSuspensionTravelRearLeft = BitConverter.ToSingle(FH4UDPBYTE, 76);
            NormalizedSuspensionTravelRearRight = BitConverter.ToSingle(FH4UDPBYTE, 80);

            TireSlipRatioFrontLeft = BitConverter.ToSingle(FH4UDPBYTE, 84); // Tire normalized slip ratio, = 0 means 100% grip and |ratio| > 1.0 means loss of grip.
            TireSlipRatioFrontRight = BitConverter.ToSingle(FH4UDPBYTE, 88);
            TireSlipRatioRearLeft = BitConverter.ToSingle(FH4UDPBYTE, 92);
            TireSlipRatioRearRight = BitConverter.ToSingle(FH4UDPBYTE, 96);

            //WheelRotationSpeedFrontLeft = BitConverter.ToSingle(FH4UDPBYTE, 100); // Wheel rotation speed radians/sec.
            //WheelRotationSpeedFrontRight = BitConverter.ToSingle(FH4UDPBYTE, 104);
            //WheelRotationSpeedRearLeft = BitConverter.ToSingle(FH4UDPBYTE, 108);
            //WheelRotationSpeedRearRight = BitConverter.ToSingle(FH4UDPBYTE, 112);

            //WheelOnRumbleStripFrontLeft = BitConverter.ToInt32(FH4UDPBYTE, 116); // = 1 when wheel is on rumble strip, = 0 when off.
            //WheelOnRumbleStripFrontRight = BitConverter.ToInt32(FH4UDPBYTE, 120);
            //WheelOnRumbleStripRearLeft = BitConverter.ToInt32(FH4UDPBYTE, 124);
            //WheelOnRumbleStripRearRight = BitConverter.ToInt32(FH4UDPBYTE, 128);

            //WheelInPuddleDepthFrontLeft = BitConverter.ToSingle(FH4UDPBYTE, 132); // = from 0 to 1, where 1 is the deepest puddle
            //WheelInPuddleDepthFrontRight = BitConverter.ToSingle(FH4UDPBYTE, 136);
            //WheelInPuddleDepthRearLeft = BitConverter.ToSingle(FH4UDPBYTE, 140);
            //WheelInPuddleDepthRearRight = BitConverter.ToSingle(FH4UDPBYTE, 144);

            //SurfaceRumbleFrontLeft = BitConverter.ToSingle(FH4UDPBYTE, 148); // Non-dimensional surface rumble values passed to controller force feedback
            //SurfaceRumbleFrontRight = BitConverter.ToSingle(FH4UDPBYTE, 152);
            //SurfaceRumbleRearLeft = BitConverter.ToSingle(FH4UDPBYTE, 156);
            //SurfaceRumbleRearRight = BitConverter.ToSingle(FH4UDPBYTE, 160);

            //TireSlipAngleFrontLeft = BitConverter.ToSingle(FH4UDPBYTE, 164); // Tire normalized slip angle, = 0 means 100% grip and |angle| > 1.0 means loss of grip.
            //TireSlipAngleFrontRight = BitConverter.ToSingle(FH4UDPBYTE, 168);
            //TireSlipAngleRearLeft = BitConverter.ToSingle(FH4UDPBYTE, 172);
            //TireSlipAngleRearRight = BitConverter.ToSingle(FH4UDPBYTE, 176);

            //TireCombinedSlipFrontLeft = BitConverter.ToSingle(FH4UDPBYTE, 180); // Tire normalized combined slip, = 0 means 100% grip and |slip| > 1.0 means loss of grip.
            //TireCombinedSlipFrontRight = BitConverter.ToSingle(FH4UDPBYTE, 184);
            //TireCombinedSlipRearLeft = BitConverter.ToSingle(FH4UDPBYTE, 188);
            //TireCombinedSlipRearRight = BitConverter.ToSingle(FH4UDPBYTE, 192);

            //SuspensionTravelMetersFrontLeft = BitConverter.ToSingle(FH4UDPBYTE, 196); // Actual suspension travel in meters
            //SuspensionTravelMetersFrontRight = BitConverter.ToSingle(FH4UDPBYTE, 200);
            //SuspensionTravelMetersRearLeft = BitConverter.ToSingle(FH4UDPBYTE, 204);
            //SuspensionTravelMetersRearRight = BitConverter.ToSingle(FH4UDPBYTE, 208);

            CarOrdinal = BitConverter.ToInt32(FH4UDPBYTE, 212); //Unique ID of the car make/model
            CarClass = BitConverter.ToInt32(FH4UDPBYTE, 216); //Between 0 (D -- worst cars) and 7 (X class -- best cars) inclusive
            CarPerformanceIndex = BitConverter.ToInt32(FH4UDPBYTE, 220); //Between 100 (slowest car) and 999 (fastest car) inclusive
            DrivetrainType = BitConverter.ToInt32(FH4UDPBYTE, 224); //Corresponds to EDrivetrainType; 0 = FWD, 1 = RWD, 2 = AWD
            NumCylinders = BitConverter.ToInt32(FH4UDPBYTE, 228); //Number of cylinders in the engine
            CarType = BitConverter.ToInt32(FH4UDPBYTE, 228); //represents the car type

            //NEW DATA OUT STRUCTURE

            //PositionX = BitConverter.ToSingle(FH4UDPBYTE, 244); //Position (meters)
            //PositionY = BitConverter.ToSingle(FH4UDPBYTE, 248);
            //PositionZ = BitConverter.ToSingle(FH4UDPBYTE, 252);

            Speed = Convert.ToInt32(BitConverter.ToSingle(FH4UDPBYTE, 256) * 3600 / 1000); // *meters per second
            Power = Convert.ToInt32(BitConverter.ToSingle(FH4UDPBYTE, 260) / 735); // *watts
            Torque = Convert.ToInt32(BitConverter.ToSingle(FH4UDPBYTE, 264)); // newton meter

            TireTempFrontLeft = BitConverter.ToSingle(FH4UDPBYTE, 268);
            TireTempFrontRight = BitConverter.ToSingle(FH4UDPBYTE, 272);
            TireTempRearLeft = BitConverter.ToSingle(FH4UDPBYTE, 276);
            TireTempRearRight = BitConverter.ToSingle(FH4UDPBYTE, 280);

            Boost = BitConverter.ToSingle(FH4UDPBYTE, 284) * 0.0689476;
            Fuel = BitConverter.ToSingle(FH4UDPBYTE, 290);
            DistanceTraveled = BitConverter.ToSingle(FH4UDPBYTE, 294);
            BestLap = BitConverter.ToSingle(FH4UDPBYTE, 298);
            LastLap = BitConverter.ToSingle(FH4UDPBYTE, 302);
            CurrentLap = BitConverter.ToSingle(FH4UDPBYTE, 304);
            CurrentRaceTime = BitConverter.ToSingle(FH4UDPBYTE, 308);

            //   LapNumber=BitConverter.ToUInt32(FH4UDPBYTE, 0);
            //   RacePosition=BitConverter.ToUInt16(FH4UDPBYTE, 0);

            Accel = Convert.ToInt32(FH4UDPBYTE[315]);
            Brake = Convert.ToInt32(FH4UDPBYTE[316]);
            Clutch = Convert.ToInt32(FH4UDPBYTE[317]);
            HandBrake = Convert.ToInt32(FH4UDPBYTE[318]);
            Gear = Convert.ToInt32(FH4UDPBYTE[319]);
            Steer = Convert.ToInt32(FH4UDPBYTE[320]);

            //NormalizedDrivingLine = Convert.ToInt32(FH4UDPBYTE[321]);  // ????
            //NormalizedAIBrakeDifference = Convert.ToInt32(FH4UDPBYTE[322]);  // ????
            // END of Convert
        }

        public async void CalculateData()
        {
            if (Speed == 0)
            {
                atime0 = DateTime.Now.Ticks;
                run100 = run200 = run300 = run400 = 1;
                readytorun = "Ready, Steady, GO!";
                chartdraw = 1;
            }
            if (Speed == 0 && HandBrake > 0 && Accel > 0 && Brake > 0)
            {
                if (RPM > rmRPM)
                    rmRPM = RPM;
                mpbRPM = mtbRPM = minRPM;
                mpaRPM = mtaRPM = MaxRPM;
            }
            if (Speed == 10 && run100 == 1)
            {
                dragrun_0_100 = dragrun_0_200 = dragrun_0_300 = dragrun_0_400 = 0;
                stoprun_100_0 = stoprun_200_0 = stoprun_300_0 = stoprun_400_0 = 0;
                readytorun = "in Progress";
                maxSpeed = 0;
            }
            if (Speed == 10 && chartdraw == 1 && Torque > 0 && Power > 0)
            {
                await Task.Run(() => chartDrawNew());
                chartdraw = 0;
            }
            if (Speed == 100 && run100 == 1)
            {
                atime100 = DateTime.Now.Ticks;
                dragrun_0_100 = (atime100 - atime0) / 10000000;
                run100 = 0;
                stoprun = 1;
            }
            if (Speed == 200 && run200 == 1)
            {
                atime200 = DateTime.Now.Ticks;
                dragrun_0_200 = (atime200 - atime0) / 10000000;
                run200 = 0;
            }
            if (Speed == 300 && run300 == 1)
            {
                atime300 = DateTime.Now.Ticks;
                dragrun_0_300 = (atime300 - atime0) / 10000000;
                run300 = 0;
            }
            if (Speed == 400 && run400 == 1)
            {
                atime400 = DateTime.Now.Ticks;
                dragrun_0_400 = (atime400 - atime0) / 10000000;
                run400 = 0;
            }
            if (Speed >= 400 && Brake > 0)
            {
                dtime400 = DateTime.Now.Ticks;
                stoprun400 = 1;
            }
            if (Speed >= 300 && Brake > 0)
            {
                dtime300 = DateTime.Now.Ticks;
                stoprun300 = 1;
            }
            if (Speed >= 200 && Brake > 0)
            {
                dtime200 = DateTime.Now.Ticks;
                stoprun200 = 1;
            }
            if (Speed >= 100 && Brake > 0)
            {
                dtime100 = DateTime.Now.Ticks;
                stoprun100 = 1;
            }
            if (Speed == 0 && stoprun == 1)
            {
                dtime0 = DateTime.Now.Ticks;
                if (stoprun400 == 1)
                    stoprun_400_0 = (dtime0 - dtime400) / 10000000;
                else stoprun_300_0 = 0;
                if (stoprun300 == 1)
                    stoprun_300_0 = (dtime0 - dtime300) / 10000000;
                else stoprun_300_0 = 0;
                if (stoprun200 == 1)
                    stoprun_200_0 = (dtime0 - dtime200) / 10000000;
                else stoprun_200_0 = 0;
                if (stoprun100 == 1)
                    stoprun_100_0 = (dtime0 - dtime100) / 10000000;
                else stoprun_100_0 = 0;
                stoprun = stoprun100 = stoprun200 = stoprun300 = stoprun400 = 0;
                dtime0 = dtime100 = dtime200 = dtime300 = 0;
                maxpowerbeetwen = maxPower.ToString() + "Hp at " + mpaRPM.ToString() + " - " + mpbRPM.ToString() + "\n" + maxTorque.ToString() + "Nm at " + mtaRPM.ToString() + " - " + mtbRPM.ToString();
            }
            else { }
            if (Speed !=0 &&Power > maxPower )
            {
                maxPower = Power;
                progressBar2.Maximum = Power;
            }
            if (Speed !=0 && Torque > maxTorque)
            {
                maxTorque = Torque;
                progressBar1.Maximum = Torque;
            }
            if (Power == maxPower && RPM > mpbRPM)
                mpbRPM = RPM;
            if (Power == maxPower && RPM < mpaRPM)
                mpaRPM = RPM;
            if (Torque == maxTorque && RPM > mtbRPM)
                mtbRPM = RPM;
            if (Torque == maxTorque && RPM < mtaRPM)
                mtaRPM = RPM;
            if (Speed > maxSpeed)
                maxSpeed = Speed;
        }

        private async void chartDrawNew()
        {
            var EngineChart = new PlotModel { Title = "Engine Data" };
            var PowerSeries = new ScatterSeries { MarkerType = MarkerType.Circle };
            var TorqueSeries = new ScatterSeries { MarkerType = MarkerType.Circle };
            EngineChart.Series.Add(PowerSeries);
            EngineChart.Series.Add(TorqueSeries);
            EngineChart.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Minimum = minRPM, Maximum = MaxRPM });
            EngineChart.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0 });
            plotView1.Model = EngineChart;
            //int ymax = 100;
            while (Speed != 0)
            {
                if (RPM > 0)
                {
                    PowerSeries.Points.Add(new ScatterPoint(RPM, Power, 2));
                }
                if (Torque > 0)
                {
                    TorqueSeries.Points.Add(new ScatterPoint(RPM, Torque, 2));
                }
                await Task.Delay(17);             
            }

            plotView1.OnModelChanged();
        }

            public string sCarClass(int vCarClass)
            {
                string nCarClass = "";
                if (vCarClass == 0)
                    nCarClass = "D";
                if (vCarClass == 1)
                    nCarClass = "C";
                if (vCarClass == 2)
                    nCarClass = "B";
                if (vCarClass == 3)
                    nCarClass = "A";
                if (vCarClass == 4)
                    nCarClass = "S1";
                if (vCarClass == 5)
                    nCarClass = "S2";
                if (vCarClass == 6)
                    nCarClass = "X";
                if (vCarClass == 7)
                    nCarClass = "X3";
                return nCarClass;
            }

            public string WheelDrive(int vWD)
            {
                string nWD = "";
                if (vWD == 0)
                    nWD = "FWD";
                if (vWD == 1)
                    nWD = "RWD";
                if (vWD == 2)
                    nWD = "AWD";
                return nWD;
            }
        }
    }


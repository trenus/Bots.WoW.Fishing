using System.Threading;
using WoW.Fishing.Utilities;
using System.Drawing;

namespace WoW.Fishing
{
    public class Manager
    {
        const int ScanningDelay = 30;
        const int WaitOnBobber = 22;
        const int LootDelay = 2000;

        const string DefaultFileName = "default.bmp";
        const string TargetFileName = "target.bmp";

        public static bool RefreshLure { get; set; }

        private static Bitmap _defaultCursor = null;
        public static Bitmap DefaultCursor
        {
            get
            {
                if (_defaultCursor == null)
                {
                    Windows.ActivateGameWindow();
                    Devices.Mouse.Move(new Point(10, 45));
                    Thread.Sleep(500);
                    _defaultCursor = new Bitmap(Devices.Mouse.Image);
                }

                return _defaultCursor;
            }
            set
            {
                // cleanup - so we don't abuse memory
                if (_defaultCursor != null)
                    _defaultCursor.Dispose();

                if (value != null)
                    _defaultCursor = new Bitmap(value);
                else
                    _defaultCursor = value;
            }
        }

        private Bitmap _currentCursor = null;
        public Bitmap CurrentCursor
        {
            get
            {
                return _currentCursor;
            }
            set
            {
                // cleanup - so we don't abuse memory
                if (_currentCursor != null)
                    _currentCursor.Dispose();

                _currentCursor = new Bitmap(value);
            }
        }

        public static Bitmap TargetCursor { get; set; }


        private Thread workerThread = null;
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
        System.Diagnostics.Stopwatch lureTimer = new System.Diagnostics.Stopwatch();


        public Manager()
        {
            // load default cursors
            try
            {
                Image d = Image.FromFile(DefaultFileName);
                DefaultCursor = new Bitmap(d);
                d.Dispose();

                Image t = Image.FromFile(TargetFileName);
                TargetCursor = new Bitmap(t);
                t.Dispose();
            }
            catch
            { }
        }

        public void SaveCursors()
        {
            Windows.ActivateGameWindow();

            if (System.IO.File.Exists(TargetFileName))
                System.IO.File.Delete(TargetFileName);

            if (System.IO.File.Exists(DefaultFileName))
                System.IO.File.Delete(DefaultFileName);

            Thread.Sleep(3000);

            Bitmap target = new Bitmap(Devices.Mouse.Image);

            target.Save(TargetFileName);
            TargetCursor = target;

            // dispose old one
            DefaultCursor = null;

            // when get is called cursor is regenerated
            // - then saved
            DefaultCursor.Save(DefaultFileName);

            Windows.ActivateBotWindow();
        }

        public void StartFishing()
        {
            if (workerThread != null && workerThread.IsAlive)
                workerThread.Abort();

            Windows.ActivateGameWindow();

            workerThread = new Thread(DoFishing);
            workerThread.Start();
        }

        public void StopFishing()
        {
            if (workerThread != null)
            {
                workerThread.Abort();
                workerThread = null;
            }
        }


        #region Thread Operations

        private void DoFishing()
        {
            Point BobberPosition;

            while (true)
            {
                //Windows.ActivateGameWindow();

                BobberPosition = default(Point);

                if (!RefreshLure && lureTimer.IsRunning)
                    lureTimer.Reset();

                if (RefreshLure && (lureTimer.Elapsed.Minutes >= 10 || !lureTimer.IsRunning))
                {
                    Devices.Keyboard.Click((ushort)VirtualKeyCodes.VirtualKeyCode.VK_2);
                    lureTimer.Restart();

                    // allow for casting time
                    Thread.Sleep(2000);
                }

                for (int i = 0; i < 3; i++)
                {
                    // cast
                    Devices.Keyboard.Click((ushort)VirtualKeyCodes.VirtualKeyCode.VK_1);
                    Thread.Sleep(500);

                    // look for bobber
                    BobberPosition = LookForBobber();

                    if (BobberPosition != default(Point))
                        break;
                }

                if (BobberPosition == default(Point))
                    //throw new Exception("Unable to find bobber.");
                    break;

                // determine bobber size
                Rectangle bobber = GetBobberSize();

                // sit directly under bobber
                Devices.Mouse.Move(new Point(bobber.X + (int)(bobber.Width * .5), bobber.Bottom + 3));

                // wait for change in curosr - reel in catch when it does
                WaitForBite();

                //Thread.Sleep(3000);
            }


            //autoEvent.WaitOne(30000);
        }

        private Point LookForBobber()
        {
            const int ScanningRetries = 3;
            const int ScanningSteps = 10;

            Point Position = new Point();

            int xMin = Windows.ScreenArea.Width / 4;
            int xMax = xMin * 3;
            int yMin = Windows.ScreenArea.Height / 5;
            int yMax = yMin * 3;

            int xStep = (int)((xMax - xMin) / ScanningSteps);
            int yStep = (int)((yMax - yMin) / ScanningSteps);
            int xOffSet = (int)(xStep / ScanningRetries);


            for (int ScanAttempt = 1; ScanAttempt <= ScanningRetries; ScanAttempt++)
            {
                for (int x = xMin + xOffSet * ScanAttempt; x < xMax; x += xStep)
                {
                    for (int y = yMin; y < yMax; y += yStep)
                    {
                        // move mouse
                        Position.X = Windows.ScreenArea.X + x;
                        Position.Y = Windows.ScreenArea.Y + y;
                        Devices.Mouse.Move(Position);

                        // allow system time to render new cursor on screen
                        Thread.Sleep(ScanningDelay);

                        // get image of newly rendered cursor
                        CurrentCursor = Devices.Mouse.Image;

                        // if we know what we are looking for
                        if (TargetCursor != null)
                        {
                            // return if cursor matches target
                            if (Compare(TargetCursor, CurrentCursor))
                                return Position;
                        }
                        else
                        {
                            // return if cursor has changed
                            if (!Compare(DefaultCursor, CurrentCursor))
                                return Position;
                        }
                    }
                }
            }

            return default(Point);
        }

        private Rectangle GetBobberSize()
        {
            const int step = 5;

            Point pos = Devices.Mouse.Location;
            Rectangle result = new Rectangle(pos, new Size(1, 1));

            // check x dimensions

            #region Bounding Box Left Side

            pos.X -= step;
            Devices.Mouse.Move(pos);
            Thread.Sleep(ScanningDelay);

            while (Compare(CurrentCursor, Devices.Mouse.Image))
            {
                pos.X -= step;
                Devices.Mouse.Move(pos);
                Thread.Sleep(ScanningDelay);
            }

            // -- undo last step since is not on bobber anymore
            pos.X += step;

            // -- resize/reposition result to account for known bobber size
            result.Width = result.X - pos.X;
            result.X = pos.X;

            #endregion

            #region Bounding Box Right Side

            pos.X = result.Right + step;
            Devices.Mouse.Move(pos);
            Thread.Sleep(ScanningDelay);

            while (Compare(CurrentCursor, Devices.Mouse.Image))
            {
                pos.X += step;
                Devices.Mouse.Move(pos);
                Thread.Sleep(ScanningDelay);
            }
            result.Width = pos.X - result.X;

            #endregion

            // check y dimensions
            // - move to horizontal center of bounding box
            pos.X = result.X + (int)(result.Width * .5);

            #region Bounding Box Top

            /* top of bobber area doesn't matter since we aren't doing anything with it */
            //pos.Y -= step;
            //Devices.Mouse.Move(pos);
            //Thread.Sleep(ScanningDelay);

            //while (Compare(CurrentCursor, Devices.Mouse.Image))
            //{
            //    pos.Y -= step;
            //    Devices.Mouse.Move(pos);
            //    Thread.Sleep(ScanningDelay);
            //}
            //result.Y = pos.Y;

            #endregion

            #region Bounding Box Bottom

            pos.Y = result.Bottom + step;
            Devices.Mouse.Move(pos);
            Thread.Sleep(ScanningDelay);

            while (Compare(CurrentCursor, Devices.Mouse.Image))
            {
                pos.Y += step;
                Devices.Mouse.Move(pos);
                Thread.Sleep(ScanningDelay);
            }
            result.Height = pos.Y - result.Y;

            #endregion

            return result; // new Rectangle(pos, new Size(35, 35));
        }

        private void WaitForBite()
        {
            stopWatch.Restart();

            while (stopWatch.Elapsed.Seconds <= WaitOnBobber)
            {
                // if cursor changes - we have a bite
                if (Compare(CurrentCursor, Devices.Mouse.Image))
                {
                    // right click the bobber
                    Devices.Mouse.Click(Devices.Mouse.MouseButton.Right);

                    // let auto loot work
                    Thread.Sleep(LootDelay);

                    // exit the routine
                    break;
                }

                // no bites yet - wait another .25 seconds and check again
                Thread.Sleep(250);
            }

            stopWatch.Stop();
        }

        private static bool Compare(Bitmap bmp1, Bitmap bmp2)
        {
            if (!bmp1.Size.Equals(bmp2.Size))
                return false;

            // GetPixel() method is *slow* and since I know the
            // fishing icon is not the same as DefaultIcon in
            // the first 5x5 pixel area - just check those pixels
            for (int x = 0; x < 5 /*bmp1.Width*/; x++)
                for (int y = 0; y < 5 /*bmp1.Height*/; y++)
                    if (bmp1.GetPixel(x, y) != bmp2.GetPixel(x, y))
                        return false;

            return true;
        }

        #endregion
    }
}

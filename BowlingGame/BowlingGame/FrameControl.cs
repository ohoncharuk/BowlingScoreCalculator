using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BowlingGame
{
    public partial class FrameControl : UserControl
    {
        public bool FrameCompleted { get; set; }
        public FrameControl NextFrameControlRef { get; set; }
        public int FrameNumber { get; set; }
        public FrameInfo Frame { get; set; }
        public bool FrameActive { get; set; }
        public ScoreActionDelegate scoreAction{ get; set; }

        public FrameControl(int frameNumber)
        {
            Frame = new FrameInfo {LastKnockInfo = KnockType.Default};
            FrameNumber = frameNumber;
            InitializeComponent();
            AdjustControlView(frameNumber);
        }

        private void InitItemsToComboBox(ComboBox comboBox, int itemsCount)
        {
            comboBox.Items.Clear();
            for(var i=0; i<=itemsCount;i++)
            {
                comboBox.Items.Add(i.ToString());
            }
        }

        public void UpdateFrameViewState()
        {
            if(Frame.Score != -1)//Score is already calculated
            {
                label1.Text = Frame.Score.ToString();
                if (Frame.LastKnockInfo != KnockType.Default)
                {
                    label2.Text = Frame.LastKnockInfo == KnockType.Strike ? "Strike" : "Spare";
                }
            }

            if (!FrameActive)
            {
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                return;
            }

            if (FrameNumber == 9)
            {
                if(Frame.Knocks.Count == 0)
                {
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                }
                else if (Frame.Knocks.Count == 1)
                {
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    var pinsToAdd = Frame.Knocks[0].Result == KnockType.Strike ? 10 : Frame.Knocks[0].RemainPins;
                    InitItemsToComboBox(comboBox2, pinsToAdd);
                    comboBox3.Enabled = false;
                }
                else if (Frame.Knocks.Count == 2)
                {
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    var strikeOrSpare = Frame.Knocks.Any(x => x.Value.Result != KnockType.Default);
                    if (strikeOrSpare)
                    {
                        comboBox3.Enabled = true;
                        InitItemsToComboBox(comboBox3, Constants.PinsInFrame);
                    }
                }

                else if (Frame.Knocks.Count == 3)
                {
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    DeActivate();
                }

                return;
            }

            if (FrameActive && Frame.Knocks.Count==0)
            {
                comboBox1.Enabled = true;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
            }
            if (FrameActive && Frame.Knocks.Count == 1)
            {
                if (Frame.LastKnockInfo == KnockType.Strike)
                {
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    DeActivate();
                }
                else
                {
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = true;
                    InitItemsToComboBox(comboBox2, Frame.Knocks[0].RemainPins);
                    comboBox3.Enabled = false;
                }
            }
            
            if (FrameActive && Frame.Knocks.Count == 2)
            {
                comboBox1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                DeActivate();
            }
        }

        public void Activate()
        {
            FrameActive = true;
            UpdateFrameViewState();
        }

        public void DeActivate()
        {
            FrameCompleted = true;
            FrameActive = false;
            UpdateFrameViewState();
            
            if(FrameNumber!=9)
            {
                NextFrameControlRef.Activate();
            }
        }

        private void AdjustControlView(int frameNumber)
        {
            //Do resizing for the frame views
            if (frameNumber != 9)//9 it's a number of last frame, where possible three throwns.
            {
                comboBox3.Visible = false;
                groupBox1.Size = new Size(groupBox1.Size.Width - 50, groupBox1.Size.Height);
                Size = new Size(groupBox1.Size.Width + 6, Size.Height);
            }
            
            groupBox1.Text = string.Format("Frame {0}", frameNumber + 1);

            UpdateFrameViewState();
        }

        private void ComboBoxKeyDown(object sender, KeyEventArgs e)
        {
            e.SuppressKeyPress = true;
        }

        private void FirstThrowResultUpdated(object sender, EventArgs e)
        {
            var pinsKnocked = int.Parse(comboBox1.SelectedItem.ToString());
            var knock = new Knock(ThrowOrder.First, pinsKnocked, Constants.PinsInFrame - pinsKnocked);
            Frame.Knocks.Add(knock.Order, knock);

            if (knock.Result == KnockType.Strike)
            {
                Frame.LastKnockInfo = KnockType.Strike;
            }
            scoreAction();
            UpdateFrameViewState();
        }

        private void SecondThrowResultUpdated(object sender, EventArgs e)
        {
            var pinsKnocked = int.Parse(comboBox2.SelectedItem.ToString());
            var knock = new Knock(ThrowOrder.Second, pinsKnocked, Frame.Knocks.First(x => x.Value.Order == ThrowOrder.First).Value.RemainPins - pinsKnocked);
            Frame.Knocks.Add(knock.Order, knock);

            Frame.LastKnockInfo = knock.Result == KnockType.Spare ? KnockType.Spare : KnockType.Default;

            scoreAction();
            UpdateFrameViewState();
        }

        private void ThirdThrowResultUpdated(object sender, EventArgs e)
        {
            var pinsKnocked = int.Parse(comboBox3.SelectedItem.ToString());
            var knock = new Knock(ThrowOrder.Third, pinsKnocked, Constants.PinsInFrame - pinsKnocked);
            Frame.Knocks.Add(knock.Order, knock);

            scoreAction();
            UpdateFrameViewState();
        }
    }
}

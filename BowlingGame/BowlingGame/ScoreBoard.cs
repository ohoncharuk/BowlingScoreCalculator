using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BowlingGame
{
    public delegate List<FrameInfo> ReadCompletedFrames();
    public delegate void RefreshFramesScoreDelegate(List<FrameInfo> framesInfo);
    public partial class ScoreBoard : Form
    {
        private List<FrameControl> _frameControls;
        public ReadCompletedFrames ReadFrames;
        public RefreshFramesScoreDelegate RefreshFramesScore;
        private void InitFrames()
        {
            if(_frameControls != null)
            {
                _frameControls.ForEach(x => Controls.Remove(x));
            }
            _frameControls = new List<FrameControl>(10);
            
            const int normalFrameWidth = 107;

            for (var i = 0; i < 10; i++)
            {
                var frameControl = new FrameControl(i);
                frameControl.Location = new Point((normalFrameWidth + 5) * i, frameControl.Location.Y);
                frameControl.scoreAction = _scoreEngine.ScoreAction;
                _frameControls.Add(frameControl);
            }

            _frameControls.ForEach(x => Controls.Add(x));

            for (var i = 1; i < _frameControls.Count; i++)
            {
                _frameControls[i - 1].NextFrameControlRef = _frameControls[i];
            }

            _frameControls[0].Activate();
        }

        public ScoreBoard()
        {
            ReadFrames = GetFrames;
            RefreshFramesScore = SetFramesScore;
            _scoreEngine = new ScoreEngine(ReadFrames, RefreshFramesScore);
            InitializeComponent();
            InitFrames();
        }

        public List<FrameInfo> GetFrames()
        {
            return _frameControls.Where(x=>x.Frame.Knocks.Count>0).Select(y => y.Frame).ToList();
        }

        public void SetFramesScore(List<FrameInfo> framesInfo)
        {
            for (int i = 0; i < framesInfo.Count; i++)
            {
                _frameControls[i].Frame.Score = framesInfo[i].Score;
                _frameControls[i].UpdateFrameViewState();
            }
        }

        private void PlayAgainClick(object sender, EventArgs e)
        {
            InitFrames();
        }
    }
}


using System.Collections.Generic;

namespace BowlingGame
{
    public class FrameInfo
    {
        //public int FrameNumber;
        public Dictionary<ThrowOrder, Knock> Knocks = new Dictionary<ThrowOrder, Knock>(3);

        public int Score { get; set; }
        public KnockType LastKnockInfo { get; set; }
        
        
        public FrameInfo()
        {
            Score = -1;
        }
    }
}
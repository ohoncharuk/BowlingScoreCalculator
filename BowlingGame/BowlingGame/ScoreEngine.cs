using System.Linq;

namespace BowlingGame
{
    public delegate void ScoreActionDelegate();

    class ScoreEngine
    {
        public ScoreActionDelegate ScoreAction;
        private readonly ReadCompletedFrames ReadeadFrames;
        private readonly RefreshFramesScoreDelegate RefreshFramesScore;
        public ScoreEngine(ReadCompletedFrames readFrames, RefreshFramesScoreDelegate refreshFramesScore)
        {
            ScoreAction = CalculateFrames;
            ReadeadFrames = readFrames;
            RefreshFramesScore = refreshFramesScore;
        }

        public void CalculateFrames()
        {
            var frames = ReadeadFrames();
            for (var i = 0; i < frames.Count; i++)
            {
                frames[i].Score = frames[i].Knocks.Sum(x => x.Value.PinsKnocked);
                
                if (frames[i].LastKnockInfo == KnockType.Spare)
                {
                    if (frames.Count > i + 1)
                    {
                        frames[i].Score += frames[i + 1].Knocks[ThrowOrder.First].PinsKnocked;
                    }

                    if (i != 0)
                    {
                        frames[i].Score += frames[i - 1].Score;
                    }
                }
                else if (frames[i].LastKnockInfo == KnockType.Strike)
                {
                    if (frames.Count > i + 1)
                    {
                        frames[i].Score += frames[i + 1].Knocks[ThrowOrder.First].PinsKnocked;
                        if (frames[i + 1].Knocks.Count > 1)
                        {
                            frames[i].Score += frames[i + 1].Knocks[ThrowOrder.Second].PinsKnocked;
                        }
                    }

                    if (frames.Count > i + 2)
                    {
                        if (frames[i + 1].LastKnockInfo == KnockType.Strike)
                        {
                            frames[i].Score += frames[i + 2].Knocks[ThrowOrder.First].PinsKnocked;
                        }
                    }

                    if (i != 0)
                    {
                        frames[i].Score += frames[i - 1].Score;
                    }
                }
                else
                {
                    if (i != 0)
                    {
                        frames[i].Score += frames[i - 1].Score;
                    }
                }
            }
            RefreshFramesScore(frames);
        }
    }
}

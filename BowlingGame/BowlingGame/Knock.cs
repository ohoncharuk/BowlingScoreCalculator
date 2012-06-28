namespace BowlingGame
{
    public class Knock
    {
        public int RemainPins { get; set; }
        public int PinsKnocked { get; set; }
        public ThrowOrder Order { get; set; }

        public Knock(ThrowOrder order, int pinsKnocked, int remainPins)
        {
            PinsKnocked = pinsKnocked;
            RemainPins = remainPins;
            Order = order;
        }

        public KnockType Result
        {
            get
            {
                if (Order == ThrowOrder.First && RemainPins == 0)
                {
                    return KnockType.Strike;
                }

                if (Order == ThrowOrder.Second && RemainPins == 0)
                {
                    return KnockType.Spare;
                }

                if (Order == ThrowOrder.Third && RemainPins == 0)
                {
                    return KnockType.Spare;
                }

                return KnockType.Default;
            }
        }
    }
}

namespace StormCloud.Evolutionary
{
    public class EvoParams
    {
        public EvoParams(int genSize, double mutRate)
        {
            this.GenSize = genSize;
            this.MutRate = mutRate;
        }

        public int GenSize;
        public double MutRate;
    }
}

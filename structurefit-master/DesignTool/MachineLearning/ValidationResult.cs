
namespace StructureEngine.MachineLearning
{
    public class ValidationResult
    {
        public ValidationResult(ErrorMeasures e, double p, Regression m)
        {
            Error = e;
            Parameter = p;
            Model = m;
        }

        public Regression Model
        {
            get;
            set;
        }

        public ErrorMeasures Error
        {
            get;
            set;
        }

        public double Parameter
        {
            get;
            set;
        }
    }
}

using System.Collections.Generic;

namespace StructureEngine.Grammar
{
    public class ShapeComputation
    {
        private IGrammar Grammar;

        public ShapeComputation(IGrammar g)
        {
            Grammar = g;
            List<IShape> h = new List<IShape>();
            IShape s = Grammar.GetStartShape();
            h.Add(s);
            Current = s;
            History = h;
        }

        public void ApplyRule(IShape s)
        {
            IShape copy = s.Clone();
            int n = History.Count - 1; // index of final model in history
            int k = History.IndexOf(Current); // index of Current
            
            // Remove history beyond current shape
            if (k < n) // if current is not the last model in history
            {
                for (int i = 0; i < n - k; i++) // remove each model beyond the current
                {
                    History.RemoveAt(k + 1); 
                }
            }

            // Add new shape to history
            this.History.Add(copy);
            this.Current = copy;
        }

        public void GoBack()
        {
            int k = History.IndexOf(Current); // index of Current
            if (k > 0) // if current is not the first model in history
            {
                this.Current = History[k - 1]; // set current backward by one
            }
            else
            {
                this.Current = History[k]; // no change
            }
        }

        public void GoForward()
        {
            int n = History.Count - 1; // index of final model in history
            int k = History.IndexOf(Current); // index of Current
            if (k < n) // if current is not the last model in history
            {
                this.Current = History[k + 1]; // set current forward by one
            }
            else
            {
                this.Current = History[k]; // no change
            }
        }

        public ShapeComputation Clone()
        {
            ShapeComputation copy = new ShapeComputation(Grammar);
            copy.History.Clear();
            foreach (IShape shape in this.History)
            {
                IShape shapeCopy = shape.Clone();
                copy.History.Add(shapeCopy);
            }
            copy.Current = copy.History[this.History.IndexOf(this.Current)];
            return copy;
        }

        public List<IShape> History
        {
            get;
            set;
        }

        public IShape Current
        {
            get;
            set;
        }
    }
}

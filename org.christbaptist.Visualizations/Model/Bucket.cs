using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.christbaptist.Visualizations.Model
{
    [Serializable]
    public class Bucket
    {
        public int Id;
        public string Name;
        public string Color;
        public int Order;
        public string DisplayAs;

        public Array data;

        public override int GetHashCode()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Bucket))
            {
                return Id == (obj as Bucket).Id;
            }
            else
            {
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Filter
/// </summary>
/// 
namespace org.christbaptist.Visualizations.Model
{
    [Serializable]
    public class Filter
    {
        public string Id;
        public string DisplayAs;
        public string DataViewName;
        public string CSS;
        public bool ActiveByDefault = true;
        public int Order;

        public override int GetHashCode()
        {
            return int.Parse(Id);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Filter))
            {
                return Id == (obj as Filter).Id;
            }
            else
            {
                return false;
            }
        }

        public Array data;
    }
}
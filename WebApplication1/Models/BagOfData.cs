
using System.Collections;

namespace WebApplication1.Models
{
    public class BagOfData
    {
        public Dictionary<string, dynamic> UnorderedData { get; set; }
        public BagOfData() 
        {
            this.UnorderedData = new Dictionary<string, dynamic>();
        }

        public void SetData(string name, dynamic data) 
        {
            UnorderedData.Add(name, data);
        }

        public void ReplaceData(string name, dynamic data) 
        {
            UnorderedData[name] = data;
        }

        public bool ContainsData(string name)
        {
            return UnorderedData.ContainsKey(name);
        }

        public dynamic GetData(string name)
        {
            return UnorderedData[name];
        }
    }
}

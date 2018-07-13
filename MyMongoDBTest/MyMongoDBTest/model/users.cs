using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMongoDBTest.model
{
    public  class users : BaseEntity
    {
        public string Account { get; set; }
       
        public string Tel { get; set; }
       
        public string Address { get; set; }
      
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMongoDBTest.model
{
    public class mongoCustomer: BaseEntity
    {
        /// <summary>
        /// 购方名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxCode { get; set; }
        /// <summary>
        /// 地址电话
        /// </summary>
        public string AddressPhone { get; set; }
        /// <summary>
        /// 开户行及账号
        /// </summary>
        public string BankAccount { get; set; }
    }
}

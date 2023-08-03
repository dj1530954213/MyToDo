using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDo.Common.Models
{
    public class BaseDto
    {

        private int _id;

        public int Id
        {
            get => _id;
            set => _id = value;
        }

        private DateTime _createDate;

        public DateTime CreateDate
        {
            get => _createDate;
            set => _createDate = value;
        }

        private DateTime _updateDate;

        public DateTime UpdateDate
        {
            get => _updateDate;
            set => _updateDate = value;
        }
    }
}

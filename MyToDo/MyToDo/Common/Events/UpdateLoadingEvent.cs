﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace MyToDo.Common.Events
{
    public class UpdateModel
    {
        public bool IsOpen { get; set; }
    }

    public class UpdateLoadingEvent:PubSubEvent<UpdateModel>
    {

    }
}
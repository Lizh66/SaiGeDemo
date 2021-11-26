using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaiGeDemo.UserView
{
    public class LogModel
    {

        private string User { get; set; }

        public string Text { get; set; }

        public LogModel(string text)
        {
            User = "A User";

            Text= $@"[{User}] [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] : {text}";
        }

       
    }
}

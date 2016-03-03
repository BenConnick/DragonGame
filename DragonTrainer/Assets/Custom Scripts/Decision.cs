using System;
using System.Collections.Generic;

namespace Assets.Custom_Scripts
{
    public class Decision
    {
        protected Decision yes;
        protected Decision no;
        protected string content;

        public Decision Yes
        {
            get
            {
                return yes;
            }
        }

        public Decision No
        {
            get
            {
                return no;
            }
        }

        public string Content
        {
            get
            {
                return content;
            }
        }

        // constructor
        public Decision(string _content) {
            content = _content;
        }

        // add a node 
        public void AddYesNode(Decision d) {
            yes = d;
        }

        // add a node 
        public void AddNoNode(Decision d)
        {
            no = d;
        }

    }
}

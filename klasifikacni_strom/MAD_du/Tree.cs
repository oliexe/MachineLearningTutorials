using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAD1
{
    //Strom
    public class Node
    {
        private ArrayList mChilds = null;

        private Attribute mAttribute;

        internal int totalData;

        public Node(Attribute attribute)
        {
            if (attribute.values != null)
            {
                mChilds = new ArrayList(attribute.values.Length);
                for (int i = 0; i < attribute.values.Length; i++)
                    mChilds.Add(null);
            }
            else
            {
                mChilds = new ArrayList(1);
                mChilds.Add(null);
            }
            mAttribute = attribute;
        }

        public void AddTreeNode(Node treeNode, string ValueName)
        {
            int index = mAttribute.indexValue(ValueName);
            mChilds[index] = treeNode;
        }





        public int totalChilds
        {
            get
            {
                return mChilds.Count;
            }
        }

        public Node getChild(int index)
        {
            return (Node)mChilds[index];
        }

        public Attribute attribute
        {
            get
            {
                return mAttribute;
            }
        }

        public Node getChildByBranchName(string branchName)
        {
            int index = mAttribute.indexValue(branchName);
            return (Node)mChilds[index];
        }
    }
}

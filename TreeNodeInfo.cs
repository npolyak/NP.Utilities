// (c) Nick Polyak 2013 - http://awebpros.com/
// License: Code Project Open License (CPOL) 1.92(http://www.codeproject.com/info/cpol10.aspx)
//
// short overview of copyright rules:
// 1. you can use this framework in any commercial or non-commercial 
//    product as long as you retain this copyright message
// 2. Do not blame the author(s) of this software if something goes wrong. 
// 
// Also as a courtesy, please, mention this software in any documentation for the 
// products that use it.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NP.Utilities
{
    public class TreeNodeInfo<NodeType>
    {
        /// <summary>
        /// A tree node object
        /// </summary>
        public NodeType TheNode;

        /// <summary>
        /// Integer specifying a distance between the original level 
        /// and the TreeNode object within the tree hierarchy.
        /// </summary>
        public int Level { get; set; }
    }
}

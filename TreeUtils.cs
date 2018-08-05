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

using NP.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.Utilities
{
    public static class TreeUtils
    {
        /// <summary>
        /// Returns a collection of all ancestors of a node.
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="node">Original Node</param>
        /// <param name="toParentFunction"></param>
        /// <returns></returns>
        public static IEnumerable<NodeType> Ancestors<NodeType>
        (
            this NodeType node,
            Func<NodeType, NodeType> toParentFunction
        ) 
        {
            if (node == null)
                yield break;

            NodeType parentNode = toParentFunction(node);

            foreach(NodeType ancestorNode in parentNode.SelfAndAncestors(toParentFunction))
            {
                yield return ancestorNode;
            }
        }

        public static NodeType AncestorByDepth<NodeType>
        (
            this NodeType node,
            int depth, 
            Func<NodeType, NodeType> toParentFunction
        )
            where NodeType : class
        {
            int currentDepth = 0;
            foreach(NodeType ancestor in node.SelfAndAncestors(toParentFunction))
            {
                if (currentDepth == depth)
                {
                    return ancestor;
                }

                currentDepth++;
            }

            return null;
        }

        /// <summary>
        /// Returns the node itself and all its ancestors a part of a collection.
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="node"></param>
        /// <param name="toParentFunction"></param>
        /// <returns></returns>
        public static IEnumerable<NodeType> SelfAndAncestors<NodeType>
        (
            this NodeType node,
            Func<NodeType, NodeType> toParentFunction
        ) 
        {
            NodeType currentNode = node;

            while (true)
            {
                if (currentNode == null)
                    break;

                yield return currentNode;

                currentNode = toParentFunction(currentNode);
            }
        }

        public static NodeType GetRootNode<NodeType>
        (
            this NodeType node,
            Func<NodeType, NodeType> toParentFunction
        )
        {
            return node.SelfAndAncestors(toParentFunction).Last();
        }

        static IEnumerable<TreeNodeInfo<NodeType>> CollectionDescendants<NodeType>
        (
            this IEnumerable<NodeType> nodeTypeCollection,
            Func<NodeType, IEnumerable<NodeType>> toChildrenFunction,
            int level
        )
        {
            if (nodeTypeCollection != null)
            {
                foreach (NodeType child in nodeTypeCollection)
                {
                    foreach (TreeNodeInfo<NodeType> descendantNode in child.SelfAndDescendantsWithLevelInfo(toChildrenFunction, level))
                    {
                        yield return descendantNode;
                    }
                }
            }
        }


        /// <summary>
        /// returns itself and all its descendants as part of a collection
        /// of TreeChildInfo object that contain the node itself and the 
        /// distance from to original node (called Level). 
        /// Original node passed as an agument to this function 
        /// has its level specified by the level argument (the default is 0)
        /// its children will have Level property set to 1, grandchildren - to 2 etc.
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="node">Original Node</param>
        /// <param name="toChildrenFunction"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static IEnumerable<TreeNodeInfo<NodeType>> SelfAndDescendantsWithLevelInfo<NodeType>
        (
            this NodeType node,
            Func<NodeType, IEnumerable<NodeType>> toChildrenFunction,
            int level = 0
        )
        {
            if (node != null)
            {
                yield return new TreeNodeInfo<NodeType> { Level = level, TheNode = node };

                IEnumerable<NodeType> children = toChildrenFunction(node);

                foreach (TreeNodeInfo<NodeType> descendant in children.CollectionDescendants<NodeType>(toChildrenFunction, level + 1))
                {
                    yield return descendant;
                }
            }
        }

        /// <summary>
        /// Returns the descendants nodes with level info (just like SelfAndDescendantsWithLevelInfo)
        /// only within the original node itself. 
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="node">Original Node</param>
        /// <param name="toChildrenFunction"></param>
        /// <returns></returns>
        public static IEnumerable<TreeNodeInfo<NodeType>> DescendantsWithLevelInfo<NodeType>
        (
            this NodeType node,
            Func<NodeType, IEnumerable<NodeType>> toChildrenFunction
        )
        {
            return node.SelfAndDescendantsWithLevelInfo(toChildrenFunction).Skip(1);
        }

        /// <summary>
        /// Returns the original node and its descendants as part of a collection
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="node">Original Node</param>
        /// <param name="toChildrenFunction"></param>
        /// <returns></returns>
        public static IEnumerable<NodeType> SelfAndDescendants<NodeType>
        (
            this NodeType node,
            Func<NodeType, IEnumerable<NodeType>> toChildrenFunction
        )
        {
            return node.SelfAndDescendantsWithLevelInfo(toChildrenFunction).Select((treeChildInfo) => treeChildInfo.TheNode);
        }

        public static IEnumerable<ResultType> SelfAndDescendants<NodeType, ResultType>
        (
            this NodeType node,
            Func<NodeType, IEnumerable<NodeType>> toChildrenFunction
        ) where ResultType : NodeType
        {
            return node.SelfAndDescendantsWithLevelInfo(toChildrenFunction)
                .Select((treeChildInfo) => treeChildInfo.TheNode)
                .GetItemsOfType<NodeType, ResultType>();
        }

        /// <summary>
        /// Returns the descendants of an original node as a collection
        /// </summary>
        /// <typeparam name="NodeType">Original Node</typeparam>
        /// <param name="node"></param>
        /// <param name="toChildrenFunction"></param>
        /// <returns></returns>
        public static IEnumerable<NodeType> Descendants<NodeType>
        (
            this NodeType node,
            Func<NodeType, IEnumerable<NodeType>> toChildrenFunction
        )
        {
            return node.DescendantsWithLevelInfo(toChildrenFunction).Select((treeChildInfo) => treeChildInfo.TheNode);
        }

        public static IEnumerable<ResultType> Descendants<NodeType, ResultType>
        (
            this NodeType node,
            Func<NodeType, IEnumerable<NodeType>> toChildrenFunction
        ) where ResultType : NodeType
        {
            return node.DescendantsWithLevelInfo(toChildrenFunction)
                .Select((treeChildInfo) => treeChildInfo.TheNode)
                .GetItemsOfType<NodeType, ResultType>();
        }

        /// <summary>
        /// Returns the anscestors of the current node (starting from the Root node) 
        /// and the current node's descendants. Level specifies the 
        /// distance from the Root Node (top node)
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="node"></param>
        /// <param name="toParentFunction"></param>
        /// <param name="toChildrenFunction"></param>
        /// <returns></returns>
        public static IEnumerable<TreeNodeInfo<NodeType>> AncestorsAndDescendantsFromTop<NodeType>
        (
            this NodeType node,
            Func<NodeType, NodeType> toParentFunction,
            Func<NodeType, IEnumerable<NodeType>> toChildrenFunction
        )
        {
            List<NodeType> ancestorsStartingFromRoot = node.Ancestors(toParentFunction).Reverse().ToList();

            int level = 0;
            foreach (NodeType ancestor in ancestorsStartingFromRoot)
            {
                yield return new TreeNodeInfo<NodeType> { Level = level, TheNode = ancestor };

                level++;
            }

            foreach (TreeNodeInfo<NodeType> nodeInfo in node.SelfAndDescendantsWithLevelInfo(toChildrenFunction, level))
            {
                yield return nodeInfo;
            }
        }

        /// <summary>
        /// returns all the nodes of the tree except for the
        /// original node itself, its descendents and ancestors (the top node is still returned
        /// even thought it is an ascestor).
        /// </summary>
        /// <typeparam name="NodeType"></typeparam>
        /// <param name="node"></param>
        /// <param name="toParentFunction"></param>
        /// <param name="toChildrenFunction"></param>
        /// <returns></returns>
        public static IEnumerable<TreeNodeInfo<NodeType>> AllButAncestorsAndDescendants<NodeType>
        (
            this NodeType node, 
            Func<NodeType, NodeType> toParentFunction,
            Func<NodeType, IEnumerable<NodeType>> toChildrenFunction
        )
        {
            if (node == null)
                return new List<TreeNodeInfo<NodeType>>();

            List<NodeType> ancestors = node.SelfAndAncestors(toParentFunction).ToList();

            int numAncestors = ancestors.Count;

            NodeType topAncestor = ancestors[numAncestors - 1];

            if (object.ReferenceEquals(topAncestor, node))
                return new List<TreeNodeInfo<NodeType>>();

            NodeType topLevelChildAncestor = ancestors[numAncestors - 2];

            IEnumerable<NodeType> topLevelChildrenWithoutChildAncestor = 
                toChildrenFunction(topAncestor).
                    Where((topLevelChild) => (!object.ReferenceEquals(topLevelChild, topLevelChildAncestor)));

            return topLevelChildrenWithoutChildAncestor.CollectionDescendants(toChildrenFunction, 1);
        }
    }
}

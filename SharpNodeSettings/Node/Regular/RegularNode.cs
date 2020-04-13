using SharpNodeSettings.Node.NodeBase;

namespace SharpNodeSettings.Node.Regular {
    /// <summary>
    ///     字节解析规则节点，该节点下挂载解析节点的子项
    /// </summary>
    public class RegularNode : NodeClass {
        /// <summary>
        ///     实例化一个默认的解析对象
        /// </summary>
        public RegularNode() {
            NodeType = NodeClassInfo.RegularNode;
            NodeHead = "RegularNode";
        }
    }
}
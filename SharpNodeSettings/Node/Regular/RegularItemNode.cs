﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using HslCommunication.BasicFramework;
using HslCommunication.Core;
using SharpNodeSettings.Node.NodeBase;

namespace SharpNodeSettings.Node.Regular {
    /// <summary>
    ///     程序解析规则的节点
    /// </summary>
    public class RegularItemNode : NodeClass, IComparable<RegularItemNode> {
        /// <summary>
        ///     示例化一个默认的对象
        /// </summary>
        public RegularItemNode() {
            NodeType = NodeClassInfo.RegularItemNode;
            NodeHead = "RegularItemNode";
        }


        /// <summary>
        ///     类型的代号，详细参见const数据
        /// </summary>
        public int RegularCode { get; set; } = RegularNodeTypeItem.Int16.Code;

        /// <summary>
        ///     类型的长度，对于string来说，就是字符串长度，其他的来说，就是数组长度
        /// </summary>
        public int TypeLength { get; set; }

        /// <summary>
        ///     数据位于字节数据的索引，对于bool变量来说，就是按照位的索引
        /// </summary>
        public int Index { get; set; }


        #region IComparable Interface

        /// <summary>
        ///     实现了比较的接口，可以用来方便的排序
        /// </summary>
        /// <param name="other">规则文件进行解析</param>
        /// <returns>是否大小</returns>
        public int CompareTo(RegularItemNode other) {
            return GetStartedByteIndex().CompareTo(other.GetStartedByteIndex());
        }

        #endregion

        #region Object Override

        /// <summary>
        ///     获取本对象的字符串表示形式
        /// </summary>
        /// <returns>字符串</returns>
        public override string ToString() {
            return $"RegularNode[{Name}]";
        }

        #endregion

        #region Public Method

        /// <summary>
        ///     获取在字节流中的起始点
        /// </summary>
        /// <returns>起始位置</returns>
        public int GetStartedByteIndex() {
            if (RegularCode == RegularNodeTypeItem.Bool.Code)
                return Index / 8;
            return Index;
        }


        /// <summary>
        ///     获取当前的数据信息实际值
        /// </summary>
        /// <param name="data"></param>
        /// <param name="byteTransform"></param>
        /// <returns></returns>
        public dynamic GetValue(byte[] data, IByteTransform byteTransform) {
            if (RegularCode == RegularNodeTypeItem.Bool.Code) {
                if (TypeLength == 1) {
                    var tmp = SoftBasic.ByteToBoolArray(data, data.Length * 8);
                    return tmp[Index];
                } else {
                    var tmp = SoftBasic.ByteToBoolArray(data, data.Length * 8);
                    var array = new bool[TypeLength];
                    for (var i = 0; i < array.Length; i++) array[i] = tmp[Index + i];
                    return array;
                }
            }

            if (RegularCode == RegularNodeTypeItem.Int16.Code) {
                if (TypeLength == 1)
                    return byteTransform.TransInt16(data, Index);
                return byteTransform.TransInt16(data, Index, TypeLength);
            }

            if (RegularCode == RegularNodeTypeItem.UInt16.Code) {
                if (TypeLength == 1)
                    return byteTransform.TransUInt16(data, Index);
                return byteTransform.TransUInt16(data, Index, TypeLength);
            }

            if (RegularCode == RegularNodeTypeItem.Int32.Code) {
                if (TypeLength == 1)
                    return byteTransform.TransInt32(data, Index);
                return byteTransform.TransInt32(data, Index, TypeLength);
            }

            if (RegularCode == RegularNodeTypeItem.UInt32.Code) {
                if (TypeLength == 1)
                    return byteTransform.TransUInt32(data, Index);
                return byteTransform.TransUInt32(data, Index, TypeLength);
            }

            if (RegularCode == RegularNodeTypeItem.Int64.Code) {
                if (TypeLength == 1)
                    return byteTransform.TransInt64(data, Index);
                return byteTransform.TransInt64(data, Index, TypeLength);
            }

            if (RegularCode == RegularNodeTypeItem.UInt64.Code) {
                if (TypeLength == 1)
                    return byteTransform.TransUInt64(data, Index);
                return byteTransform.TransUInt64(data, Index, TypeLength);
            }

            if (RegularCode == RegularNodeTypeItem.Float.Code) {
                if (TypeLength == 1)
                    return byteTransform.TransSingle(data, Index);
                return byteTransform.TransSingle(data, Index, TypeLength);
            }

            if (RegularCode == RegularNodeTypeItem.Double.Code) {
                if (TypeLength == 1)
                    return byteTransform.TransDouble(data, Index);
                return byteTransform.TransDouble(data, Index, TypeLength);
            }

            if (RegularCode == RegularNodeTypeItem.StringAscii.Code)
                return Encoding.ASCII.GetString(data, Index, TypeLength);
            throw new Exception("Not Supported Data Type");
        }


        /// <summary>
        ///     获取当前的解析规则的节点所占用的最长字节
        /// </summary>
        /// <returns>长度</returns>
        public int GetLengthByte() {
            if (RegularCode == RegularNodeTypeItem.Bool.Code) {
                if ((Index + TypeLength) % 8 == 0)
                    return TypeLength / 8 + GetStartedByteIndex();
                return TypeLength / 8 + 1 + GetStartedByteIndex();
            }

            if (RegularCode == RegularNodeTypeItem.StringAscii.Code ||
                RegularCode == RegularNodeTypeItem.StringUnicode.Code ||
                RegularCode == RegularNodeTypeItem.StringUtf8.Code)
                return TypeLength + Index;
            if (RegularCode == RegularNodeTypeItem.UInt16.Code ||
                RegularCode == RegularNodeTypeItem.Int16.Code)
                return TypeLength * 2 + Index;
            if (RegularCode == RegularNodeTypeItem.Int32.Code ||
                RegularCode == RegularNodeTypeItem.UInt32.Code ||
                RegularCode == RegularNodeTypeItem.Float.Code)
                return TypeLength * 4 + Index;
            if (RegularCode == RegularNodeTypeItem.Int64.Code ||
                RegularCode == RegularNodeTypeItem.UInt64.Code ||
                RegularCode == RegularNodeTypeItem.Double.Code)
                return TypeLength * 8 + Index;
            return TypeLength + Index;
        }

        #endregion

        #region Override NodeClass

        /// <summary>
        ///     从XElement加载数据
        /// </summary>
        /// <param name="element">Xml元素</param>
        public override void LoadByXmlElement(XElement element) {
            base.LoadByXmlElement(element);
            RegularCode = int.Parse(element.Attribute("TypeCode").Value);
            TypeLength = int.Parse(element.Attribute("TypeLength").Value);
            Index = int.Parse(element.Attribute("Index").Value);
        }

        /// <summary>
        ///     转化Xml存储文件
        /// </summary>
        /// <returns>Xml元素</returns>
        public override XElement ToXmlElement() {
            var element = base.ToXmlElement();
            element.SetAttributeValue("Index", Index);
            element.SetAttributeValue("TypeCode", RegularCode);
            element.SetAttributeValue("TypeLength", TypeLength);
            return element;
        }


        /// <summary>
        ///     获取用于在数据表信息中显示的键值数据对信息
        /// </summary>
        /// <returns>键值数据对列表</returns>
        public override List<NodeClassRenderItem> GetNodeClassRenders() {
            var list = base.GetNodeClassRenders();
            list.Add(new NodeClassRenderItem("索引位置", Index.ToString()));
            list.Add(new NodeClassRenderItem("类型值", RegularCode.ToString()));
            list.Add(new NodeClassRenderItem("类型数据长度", TypeLength.ToString()));

            return list;
        }

        #endregion
    }
}
﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cheng.Properties {
    using System;
    
    
    /// <summary>
    ///   一个强类型的资源类，用于查找本地化的字符串等。
    /// </summary>
    // 此类是由 StronglyTypedResourceBuilder
    // 类通过类似于 ResGen 或 Visual Studio 的工具自动生成的。
    // 若要添加或移除成员，请编辑 .ResX 文件，然后重新运行 ResGen
    // (以 /str 作为命令选项)，或重新生成 VS 项目。
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   返回此类使用的缓存的 ResourceManager 实例。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Cheng.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   重写当前线程的 CurrentUICulture 属性，对
        ///   使用此强类型资源类的所有资源查找执行重写。
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   查找类似 索引超出范围 的本地化字符串。
        /// </summary>
        internal static string ArrayValue_IndexOut {
            get {
                return ResourceManager.GetString("ArrayValue_IndexOut", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 参数是表示为null的空引用 的本地化字符串。
        /// </summary>
        internal static string Exception_ArgNullError {
            get {
                return ResourceManager.GetString("Exception_ArgNullError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 Base64编码出现错误 的本地化字符串。
        /// </summary>
        internal static string Exception_Base64EncoderError {
            get {
                return ResourceManager.GetString("Exception_Base64EncoderError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 给定方法的参数不合规 的本地化字符串。
        /// </summary>
        internal static string Exception_FuncArgError {
            get {
                return ResourceManager.GetString("Exception_FuncArgError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 给定参数超出范围 的本地化字符串。
        /// </summary>
        internal static string Exception_FuncArgOutOfRange {
            get {
                return ResourceManager.GetString("Exception_FuncArgOutOfRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 无法解析json文本 的本地化字符串。
        /// </summary>
        internal static string Exception_NotParserJsonText {
            get {
                return ResourceManager.GetString("Exception_NotParserJsonText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 不支持此功能 的本地化字符串。
        /// </summary>
        internal static string Exception_NotSupportedText {
            get {
                return ResourceManager.GetString("Exception_NotSupportedText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 流已经被释放 的本地化字符串。
        /// </summary>
        internal static string Exception_StreamDisposeText {
            get {
                return ResourceManager.GetString("Exception_StreamDisposeText", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 读取的流超出给定的范围 的本地化字符串。
        /// </summary>
        internal static string Exception_StreamReaderOutRange {
            get {
                return ResourceManager.GetString("Exception_StreamReaderOutRange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 自定义写入异常 的本地化字符串。
        /// </summary>
        internal static string StreamParserDef_CunstomWriterExc {
            get {
                return ResourceManager.GetString("StreamParserDef_CunstomWriterExc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 无法解析数据 的本地化字符串。
        /// </summary>
        internal static string StreamParserDef_NotConver {
            get {
                return ResourceManager.GetString("StreamParserDef_NotConver", resourceCulture);
            }
        }
        
        /// <summary>
        ///   查找类似 流不支持查找或读取 的本地化字符串。
        /// </summary>
        internal static string StreamParserDef_NotSeekAndRead {
            get {
                return ResourceManager.GetString("StreamParserDef_NotSeekAndRead", resourceCulture);
            }
        }
    }
}

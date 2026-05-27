using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Runtime.Remoting;
using System.Text;

namespace Cheng.DEBUG
{

    public class TestTextReader : TextReader
    {

        public TestTextReader(TextReader reader)
        {
            p_reader = reader;
        }

        private TextReader p_reader;

        public override int Read()
        {
            return p_reader.Read();
        }

        public override object InitializeLifetimeService()
        {
            return p_reader.InitializeLifetimeService();
        }

        public override ObjRef CreateObjRef(Type requestedType)
        {
            return p_reader.CreateObjRef(requestedType);
        }

        public override void Close()
        {
            p_reader.Close();
        }


        public override int Peek()
        {
            return p_reader.Peek();
        }

        public override int Read(char[] buffer, int index, int count)
        {
            return p_reader.Read(buffer, index, count);
        }

        public override string ReadToEnd()
        {
            return p_reader.ReadToEnd();
        }

        public override int ReadBlock(char[] buffer, int index, int count)
        {
            return p_reader.ReadBlock(buffer, index, count);
        }

        public override string ReadLine()
        {
            return p_reader.ReadLine();
        }

    }


    public class TestTextWriter : TextWriter
    {

        public TestTextWriter(TextWriter writer)
        {
            p_writer = writer;
        }

        private TextWriter p_writer;

        public override Encoding Encoding => p_writer.Encoding;

        public override IFormatProvider FormatProvider => p_writer.FormatProvider;

        public override string NewLine { get => p_writer.NewLine; set => p_writer.NewLine = value; }

        public override object InitializeLifetimeService()
        {
            return p_writer.InitializeLifetimeService();
        }

        public override ObjRef CreateObjRef(Type requestedType)
        {
            return p_writer.CreateObjRef(requestedType);
        }

        public override void Close()
        {
            p_writer.Close();
            "Close".printl();
        }

        protected override void Dispose(bool disposing)
        {            
        }

        public override void Flush()
        {
            p_writer.Flush();
            "Flush".printl();
        }

        public override void Write(char value)
        {
            p_writer.Write(value);
            ("writer:" + value).printl();
        }

        public override void Write(char[] buffer)
        {
            p_writer.Write(buffer);
            ("writer:" + new string(buffer)).printl();
        }

        public override void Write(char[] buffer, int index, int count)
        {
            p_writer.Write(buffer, index, count);
            ("writer:" + new string(buffer, index, count)).printl();
        }

        public override void Write(string value)
        {
            p_writer.Write(value);
            ("writer:" + value).printl();
        }

        public override void WriteLine()
        {
            p_writer.WriteLine();
            "writer:NewLine".printl();
        }
        
    }

}

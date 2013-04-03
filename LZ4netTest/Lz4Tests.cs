using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lz4Net;
using System.Collections.Generic;

namespace LZ4netTest
{
    [TestClass]
    public class Lz4Tests
    {
        string sampleTxt = "teste string teste string teste teste string 1234567890 ";

        [TestMethod]
        public void StringCompression_SimpleTest ()
        {
            var txt = sampleTxt + sampleTxt + sampleTxt + sampleTxt + sampleTxt + sampleTxt;
            
            var comp = Lz4.CompressString (txt);
            var txt2 = Lz4.DecompressString (comp);
            Assert.IsTrue (txt == txt2);

            int sz1 = comp.Length;
            Assert.IsTrue (sz1 < txt.Length);

            comp = Lz4.CompressString (txt, Lz4Mode.HighCompression);
            txt2 = Lz4.DecompressString (comp);
            Assert.IsTrue (txt == txt2);

            Assert.IsTrue (sz1 >= comp.Length);
        }

        [TestMethod]
        public void StreamCompression_SimpleTest ()
        {
            var txt = sampleTxt + sampleTxt + sampleTxt + sampleTxt + sampleTxt + sampleTxt;
            
            var mem = new System.IO.MemoryStream ();
            using (var stream = new Lz4CompressionStream (mem, 1 << 18, Lz4Mode.HighCompression))
            {
                var b = System.Text.Encoding.UTF8.GetBytes (txt);
                stream.Write (b, 0, b.Length);
            }
            // reset stream position
            mem.Position = 0;

            using (var stream = new Lz4DecompressionStream (mem))
            {
                var b = new byte[1024];
                int sz = stream.Read (b, 0, b.Length);

                // validate
                Assert.IsTrue (txt == System.Text.Encoding.UTF8.GetString (b, 0, sz));
            }            
        }

        [TestMethod]
        public void StreamCompression_FileStream_1Mb ()
        {   
            var filename = System.IO.Path.GetRandomFileName ();
            int currentSz = 0;
            int sz = 0;
            int maxSz = 1024 * 1024 * 10;
            List<uint> hashes = new List<uint> (1024);
            byte[] buffer;

            try
            {
                // create file
                using (var stream = new Lz4CompressionStream (new System.IO.FileStream (filename, System.IO.FileMode.Create), 1 << 18, Lz4Mode.HighCompression, true))
                {
                    while (currentSz < maxSz)
                    {
                        buffer = GetStringsAsByte (1024);
                        stream.Write (buffer, 0, buffer.Length);
                        currentSz += buffer.Length;
                        // save hash
                        hashes.Add (SimpleHash (buffer));
                    }
                }

                // read
                buffer = new byte[1024];
                int ix = 0;
                using (var stream = new Lz4DecompressionStream (new System.IO.FileStream (filename, System.IO.FileMode.Open), true))
                { 
                    while ((sz = stream.Read (buffer, 0, buffer.Length)) > 0)
                    {
                        // check hash
                        Assert.IsTrue (SimpleHash (buffer) == hashes[ix++], "Hash mismatch");
                    }
                }
            }
            finally
            {
                System.IO.File.Delete (filename);
            }
        }

        private byte[] GetStringsAsByte (int size)
        {
            byte[] buffer = new byte[size];
            char ch;
            int ix = 0;
            while (ix < size)
            {
                for (int i = 0; i < sampleTxt.Length && ix < size; i++)
                    buffer[ix++] = (byte)sampleTxt[i];
            }

            return buffer;
        }

        private static Random rnd = new Random ((int)DateTime.Now.Ticks);
        static  string random_chars = GetRandomASCIICharList ();        
        static string GetRandomASCIICharList ()
        {
            var builder = new System.Text.StringBuilder ();
            char ch;
            for (int i = 0; i < Byte.MaxValue; i++)
            {
                ch = (char)i;
                if ((ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || ch == ' ')
                    builder.Append (ch);
            }
            return builder.ToString ();
        }

        private string RandomString (int size)
        {
            return new string (RandomStringAsChar (size));
        }

        private char[] RandomStringAsChar (int size)
        {
            char[] buffer = new char[size];
            char ch;
            for (int i = 0; i < size; i++)
            {
                buffer[i] = random_chars[rnd.Next (random_chars.Length)];
            }
            return buffer;
        }

        private byte[] RandomStringAsByte (int size)
        {
            byte[] buffer = new byte[size];
            char ch;
            int ix = 0;
            for (int i = 0; i < size; i++)
            {
                buffer[i] = (byte)random_chars[rnd.Next (random_chars.Length)];
            }
            return buffer;
        }

        /// <summary> Função rápida para geração de hash de string :: djb2 hash function</summary>
        public static uint SimpleHash (string s)
        {
            uint hash = 5381;
            for (int i = 0; i < s.Length; ++i)
            {
                hash = ((hash << 5) + hash) + s[i];
            }
            return hash;
        }

        public static uint SimpleHash (char[] s)
        {
            uint hash = 5381;
            for (int i = 0; i < s.Length; ++i)
            {
                hash = ((hash << 5) + hash) + s[i];
            }
            return hash;
        }

        public static uint SimpleHash (byte[] s)
        {
            uint hash = 5381;
            for (int i = 0; i < s.Length; ++i)
            {
                hash = ((hash << 5) + hash) + s[i];
            }
            return hash;
        }
    }
}

lz4-net
=======
<h1>Lz4.Net is a wrapper of the Lz4 lib</h1>

Lz4 Project: [http://cyan4973.github.io/lz4/]<br/>
Lastest Lz4 version: [https://code.google.com/p/lz4/source/detail?r=94 r94]

<h2>Lz4 Compression Algorithm</h2>
LZ4 is a very fast lossless compression algorithm, providing compression speed at 300 MB/s per core, scalable with multi-cores CPU. It also features an extremely fast decoder, with speeds up and beyond 1GB/s per core, typically reaching RAM speed limits on multi-core systems.

A high compression derivative, called LZ4_HC, is also provided. It trades CPU for compression ratio.

<h2>Lz4.Net</h2>
Lz4.Net is a wrapper using the native dll (x86/x64) and has some helper methods and stream implementation.

<h2>Examples</h2>

Lz4.Net implement some helpers methods for compression and decompression that keeps a 8-byte header on each compressed buffer.
If you prefer to avoid this small overhead, you can use the Lz4 native methods.

<h3>Simple byte[] compression</h3>
```csharp

// out data:  some byte array
byte[] buffer = Encoding.UTF8.GetBytes ("large text");
// compress
byte[] compressed = Lz4Net.Lz4.CompressBytes (buffer, 0, buffer.Length, Lz4Net.Lz4Mode.Fast);

```

<h3>Simple byte[] decompression</h3>
```csharp

// decompress
byte[] buffer = Lz4Net.Lz4.DecompressBytes (compressed);

```

<h3>String Compression Helpers</h3>
These help functions will compress the string bytes and return a base 64 encoded string.
```csharp

// compress
string compressed = Lz4Net.Lz4.CompressString ("large text");

// decompress
string uncompressed = Lz4Net.Lz4.DecompressString (compressed);
```

<h3>Exposed native methods</h3>

```csharp
unsafe
{
    // compress 

    // our data 
    byte[] data = Encoding.UTF8.GetBytes ("large text");
    int originalSize = data.Length;

    // get minimum work buffer 
    buffer = new byte[LZ4_compressBound (originalSize)];

    int sz;
    // get buffers pointers
    fixed (byte* pData = &data[0], pBuffer = &buffer[0])
    {
        // compress
        sz = Lz4Net.Lz4.LZ4_compress (pData, pBuffer, data.Length); 
    }

    // adjust final array size
    Array.Resize (ref buffer, sz);

    // decompress phase

    // since we know the original size
    byte[] uncompressed = new byte[originalSize];

    // get again the pointers
    fixed (byte* pSrc = &buffer[0], pDst = &uncompressed [0])
    {
        // decompress
        Lz4Net.Lz4.LZ4_uncompress (pSrc, pDst, originalSize);
    }

}

```


<h3>Streams</h3>

<i>Lz4CompressionStream</i> to handle compression.<br/>
<i>Lz4DecompressionStream</i>  to handle decompression.

```csharp

// create file
using (var stream = new Lz4CompressionStream (new FileStream (filename, FileMode.Create), 1 << 18, Lz4Mode.HighCompression, true))
{
    stream.Write (buffer, 0, buffer.Length);
}

// read                
int sz = 0;
using (var stream = new Lz4DecompressionStream (new FileStream (filename, FileMode.Open), true))
{ 
    while ((sz = stream.Read (buffer, 0, buffer.Length)) > 0)
    {
        // ...
    }
}

```

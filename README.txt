This is a minimal zip reader originally written for unity over course of pretty much single evening.

It exists pretty much only because ZipArchive is not available by default in certain API levels in unity.

A word of warning:

* This thing is DUMB.
* It does minimum amount of error checking.
* It only supports compression method 0x8 which is deflate.
* It does not support zip64.
* It relies on availability of System.IO.Compression.DeflateStream (because I'm not in the mood for implementing inflate/deflate C# in addition to that)
* Also, all the data used for creating the writer was taken off wikipedia article about zip files.

So. To use it, create an instance, and call "load()". After that you can call "loadFile()" to get a file (the file it knows about) as byte array, or 
you could poke internal structures via ".numFiles", ".getFiles()", ".getFileRecord()", ".getFileIndex()" and such.

The license is ZLib (https://en.wikipedia.org/wiki/Zlib_License)

And that's pretty much all there is to it.

Enjoy.

Victor "NegInfinity" Eremin, 2018/07/05.

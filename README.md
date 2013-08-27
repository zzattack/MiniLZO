MiniLZO
=======
This is a C# port of the LZO real-time data compression library as found on http://www.oberhumer.com/opensource/lzo/.

It is really a copy-paste of the original code after putting it through the MSVC preprocessor,
changing only the used datatypes to their .NET equivalents and fixing up some control flow to
amend for the semantic differences between C's and C#'s goto statement.

If at all possible, avoid using this port, it does not come with any of the guarantees of the C version.
That means it
* has not been thoroughly tested to be reliable 
* does not guarantee pretty fast compression and *extremely* fast decompression
* does not come with military-grade stability and robustness

But.. it might just work for you.
